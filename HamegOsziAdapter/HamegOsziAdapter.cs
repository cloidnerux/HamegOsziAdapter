using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using log4net;

namespace HamegOszi {
	public class HamegOsziAdapter {

		private SerialPort dataPort;
		private static readonly ILog log = LogManager.GetLogger(typeof(HamegOsziAdapter));

		private string deviceInfo;

		public enum WavefromRate {
			Auto,
			MaxWfm,
			MaxSamples
		};

		/// <summary>
		/// The channel coupling methods
		/// DC is with 50Ohm termination, DCLimit with 1Mohm
		/// </summary>
		public enum ChannelCoupling
		{
			DC, 
			DCLimit,
			AC,
			ACLimit,
			GND
		};

		public bool IsConnected
		{
			get
			{
				if(dataPort == null)
					return false;
				return dataPort.IsOpen && deviceInfo != string.Empty;
			}
		}

		public HamegOsziAdapter(string port, int baudrate)
		{
			try{
			log.Info("Try to connecto to measurment device on port " + port + " with baud rate " + baudrate.ToString());
				dataPort = new SerialPort(port, baudrate);
				dataPort.Open();
				if(!dataPort.IsOpen)
				{
					log.Error("Could not open comport on " + port + " with baudrate " + baudrate);
					dataPort.Close();
					return;
				}
				dataPort.WriteLine("*IDN?");
				System.DateTime start = DateTime.Now;
				while(dataPort.BytesToRead == 0)
				{
					if((DateTime.Now - start).TotalMilliseconds > 1000)
					{
						log.Error("No device response to *IDN? within 1s!");
						dataPort.Close();
						return;
					}
				}
				deviceInfo = dataPort.ReadExisting();
				log.Info("Connected to device " + deviceInfo);
			}
			catch(Exception ex)
			{
				log.Error(ex);
			}
		}

		~HamegOsziAdapter()
		{
			if(dataPort.IsOpen)
			{
				dataPort.ReadExisting();
				dataPort.Close();
			}
		}

		public void Close()
		{
			if(dataPort.IsOpen) {
				dataPort.ReadExisting();
				dataPort.Close();
			}
		}

		/// <summary>
		/// Send a message to the connected device
		/// </summary>
		/// <param name="message">The string message to send</param>
		/// <returns>True on success</returns>
		private bool SendMessage(string message)
		{
			if(!dataPort.IsOpen)
			{
				log.Error("Data port is not open!");
				return false;
			}
			dataPort.WriteLine(message);
			return true;
		}

		/// <summary>
		/// Send a query and wait for the answer
		/// </summary>
		/// <param name="message">The message to send</param>
		/// <returns>The received string or an empty one</returns>
		private string QueryResponse(string message)
		{
			if(!dataPort.IsOpen)
			{
				log.Error("Data port is not open!");
				return "";
			}
			dataPort.WriteLine(message);
			System.DateTime start = DateTime.Now;
			while(dataPort.BytesToRead == 0) {
				if((DateTime.Now - start).TotalMilliseconds > 1000) {
					log.Error("No device response to " + message + " within 1s!");
					dataPort.Close();
					return "";
				}
			}
			string result = dataPort.ReadExisting();
			start = DateTime.Now;
			while(!result.Contains('\n') && (DateTime.Now - start).TotalMilliseconds < 1000)
			{
				if(dataPort.BytesToRead > 0)
					result += dataPort.ReadExisting();
			}
			if((DateTime.Now - start).TotalMilliseconds >= 1000)
				return "";
			return result;
		}

		/// <summary>
		/// starts continous aquistion
		/// </summary>
		/// <returns>True on success</returns>
		public bool Run()
		{
			return SendMessage("RUN");
		}

		/// <summary>
		/// Stops aquisition
		/// </summary>
		/// <returns>True on success</returns>
		public bool Stop() {
			return SendMessage("STOP");
		}

		/// <summary>
		/// Set the timebase to the specified timebase
		/// </summary>
		/// <param name="timebase">The timebase in seconds</param>
		/// <returns>True on success</returns>
		public bool SetTimeBase(float timebase)
		{
			return SendMessage("TIM:SCAL " + timebase.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Returns the real aquisition time used by the device
		/// </summary>
		/// <returns>The aquisiton time or 0 on failure</returns>
		public float GetAcquisitionTime()
		{
			string response = QueryResponse("TIM:RAT?");
			if(response == "")
			{
				return 0.0f;
			}
			float result;
			if(!float.TryParse(response, out result))
			{
				log.Error("Could not parse response " + response + " to float!");
				return 0.0f;
			}
			return result;
		}
		
		/// <summary>
		/// Set the trigger position 
		/// </summary>
		/// <param name="position">The position of the trigger in seconds, has to be between -500 and 500</param>
		/// <returns>True on success</returns>
		public bool SetStrigger(float position)
		{
			if(position < -500.0f || position > -500.0f)
			{
				return false;
			}
			return SendMessage("TIM:POS " + position.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Set the waveform update rate
		/// </summary>
		/// <param name="rate">The rate to set</param>
		/// <returns>True on success</returns>
		public bool SetWaveformRate(WavefromRate rate)
		{
			switch(rate)
			{
			case WavefromRate.Auto:
				return SendMessage("ACQ:WRAT AUTO");
			case WavefromRate.MaxSamples:
				return SendMessage("ACQ:WRAT MSAM");
			case WavefromRate.MaxWfm:
				return SendMessage("ACQ:WRAT MWAV");
			}
			return false;
		}

		/// <summary>
		/// Enables or disables the roll mode
		/// </summary>
		/// <param name="state">The state of the roll mode, false = off</param>
		/// <returns>True on success</returns>
		public bool SetRollMode(bool state)
		{
			if(!state)
			{
				return SendMessage("TIM:ROLL:ENAB OFF");
			}
			return SendMessage("TIM:ROLL:ENAB ON");
		}

		/// <summary>
		/// Sets the specified channel to the specified state
		/// </summary>
		/// <param name="channel">The input channel to set, has to be between 1 and 4</param>
		/// <param name="state">The state to set, false = off</param>
		/// <returns>True on success</returns>
		public bool SetChannel(int channel, bool state)
		{
			if(channel < 1 || channel > 4)
			{
				log.Warn("Channel out of bounds!");
				return false;
			}
			if(!state)
				return SendMessage("CHAN"+channel.ToString()+":STAT OFF");
			return SendMessage("CHAN" + channel.ToString() + ":STAT ON");
		}

		/// <summary>
		/// Sets the coupling method for the specified channel
		/// </summary>
		/// <param name="channel">The channel to set</param>
		/// <param name="coupling">The coupling to set</param>
		/// <returns>True on success</returns>
		public bool SetChannelCoupling(int channel, ChannelCoupling coupling)
		{
			if(channel < 1 || channel > 4) {
				log.Warn("Channel out of bounds!");
				return false;
			}
			switch(coupling)
			{
				case ChannelCoupling.AC:
					return SendMessage("CHAN" + channel.ToString() + ":COUP AC");
				case ChannelCoupling.ACLimit:
					return SendMessage("CHAN" + channel.ToString() + ":COUP ACL");
				case ChannelCoupling.DC:
					return SendMessage("CHAN" + channel.ToString() + ":COUP DC");
				case ChannelCoupling.DCLimit:
					return SendMessage("CHAN" + channel.ToString() + ":COUP DCL");
				case ChannelCoupling.GND:
					return SendMessage("CHAN" + channel.ToString() + ":COUP GND");
			}
			return false;
		}

		/// <summary>
		/// Set the channel scale for the specified channel
		/// </summary>
		/// <param name="channel">The channel to set</param>
		/// <param name="scale">The scale of the channel, has to be between 0.001V/div and 10V/div</param>
		/// <returns>True on success</returns>
		public bool SetChannelScale(int channel, float scale)
		{
			if(channel < 1 || channel > 4) {
				log.Warn("Channel out of bounds!");
				return false;
			}
			if(scale < 0.001f || scale > 10f)
			{
				log.Warn("Chanel level out of bounds!");
				return false;
			}
			return SendMessage("CHAN" + channel.ToString() + ":SCAL " + scale.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Set the vertical channel position 
		/// </summary>
		/// <param name="channel">The channel to set</param>
		/// <param name="position">The position if the channel, has to be between -5 and 5</param>
		/// <returns>True on success</returns>
		public bool SetChannelPosition(int channel, float position)
		{
			if(channel < 1 || channel > 4) {
				log.Warn("Channel out of bounds!");
				return false;
			}
			if(position < -5f || position > 5f) {
				log.Warn("Chanel position out of bounds!");
				return false;
			}
			return SendMessage("CHAN" + channel.ToString() + ":POS " + position.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Querys the data present on the given channel
		/// </summary>
		/// <param name="channel">The channel to query</param>
		/// <returns>The waveform data as list of float values</returns>
		public List<float> GetWaveform(int channel)
		{
			if(channel < 1 || channel > 4) {
				log.Warn("Channel out of bounds!");
				return null;
			}
			SendMessage("CHAN" + channel.ToString() + ":DATA?");
			System.DateTime start = DateTime.Now;
			///Initial failure condition, no response
			while(dataPort.BytesToRead == 0) {
				if((DateTime.Now - start).TotalMilliseconds > 1000) {
					log.Error("No device response to data query on channel " + channel.ToString() + " within 1s!");
					return null;
				}
			}
			///now get all the data
			string response = dataPort.ReadExisting();
			DateTime lastResponse = DateTime.Now;
			while((lastResponse - DateTime.Now).TotalMilliseconds < 5000)
			{
				if(dataPort.BytesToRead > 0)
				{
					lastResponse = DateTime.Now;
					response += dataPort.ReadExisting();
				}
			}
			var tmp = response.Split(',');
			List<float> result = new List<float>();
			float temp;
			foreach(string s in tmp)
			{
				if(!float.TryParse(s, out temp))
				{
					log.Warn("Could not parse " + s + " to float, abort data query!");
					return null;
				}
				result.Add(temp);
			}
			return result;
		}

		/// <summary>
		/// Sets the trigger mode between auto and normal
		/// </summary>
		/// <param name="normal">If true, trigger mode is normal</param>
		/// <returns>True on success</returns>
		public bool SetTriggerMode(bool normal)
		{
			return SendMessage("TRIG:A:MODE " + ((normal)?("NORM"):("AUTO")));
		}
		
		public bool SetMeasurment(int measurment, bool state){
			if(measurment < 1 || measurment > 6)
			{
				log.Warn("Measurment channel out of bounds!");
				return false;
			}
			return SendMessage("MEAS" + measurment.ToString() + ((state)?("ON"):("OFF")));
		}

		public bool ResetMeasurmentStatistics(int measurment)
		{
			if(measurment < 1 || measurment > 6) {
				log.Warn("Measurment channel out of bounds!");
				return false;
			}
			return SendMessage("MEAS" + measurment.ToString() + ":STAT:RES");
			}

		public string GetMeasurmentResult(int measurment)
		{
			if(measurment < 1 || measurment > 6) {
				log.Warn("Measurment channel out of bounds!");
				return "";
			}
			return QueryResponse("MEAS" + measurment.ToString() + ":RES?");
		}

		/// <summary>
		/// Querys the result of the average statistical analysis
		/// </summary>
		/// <param name="measurment">The measrument to fecth</param>
		/// <returns>The string of data received</returns>
		public float GetMeasurmentAVG(int measurment)
		{
			if(measurment < 1 || measurment > 6) {
				log.Warn("Measurment channel out of bounds!");
				return 0.0f;
			}
			return ParseResult(QueryResponse("MEAS" + measurment.ToString() + ":RES:AVG?"));
		}


		/// <summary>
		/// Querys the max value of the analysed value
		/// </summary>
		/// <param name="measurment">The measrument to fecth</param>
		/// <returns>The string of data received</returns>
		public float GetMeasurmentMax(int measurment) {
			if(measurment < 1 || measurment > 6) {
				log.Warn("Measurment channel out of bounds!");
				return 0.0f;
			}
			return ParseResult(QueryResponse("MEAS" + measurment.ToString() + ":RES:PPE?"));
		}


		/// <summary>
		/// Querys the min value of analysed value
		/// </summary>
		/// <param name="measurment">The measrument to fecth</param>
		/// <returns>The string of data received</returns>
		public float GetMeasurmentMin(int measurment) {
			if(measurment < 1 || measurment > 6) {
				log.Warn("Measurment channel out of bounds!");
				return 0.0f;
			}
			return ParseResult(QueryResponse("MEAS" + measurment.ToString() + ":RES:NPE?"));
		}


		/// <summary>
		/// Querys the std deviation of the analysed value
		/// </summary>
		/// <param name="measurment">The measrument to fecth</param>
		/// <returns>The string of data received</returns>
		public float GetMeasurmentStdDev(int measurment) {
			if(measurment < 1 || measurment > 6) {
				log.Warn("Measurment channel out of bounds!");
				return 0.0f;
			}
			return ParseResult(QueryResponse("MEAS" + measurment.ToString() + ":RES:STDD?"));
		}


		/// <summary>
		/// Querys the count of waveforms used for the analysis
		/// </summary>
		/// <param name="measurment">The measrument to fecth</param>
		/// <returns>The string of data received</returns>
		public float GetMeasurmentCount(int measurment) {
			if(measurment < 1 || measurment > 6) {
				log.Warn("Measurment channel out of bounds!");
				return 0.0f;
			}
			return ParseResult(QueryResponse("MEAS" + measurment.ToString() + ":RES:WFMC?"));
		}

		// System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.AllowDecimalPoint
		public float ParseResult(string result)
		{
			float ret;
			if(!float.TryParse(result, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out ret))
			{
				log.Warn("Could not parse return value " + result);
				return 0.0f;
			}
			return ret;
		}
	}
}
