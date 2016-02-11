using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using HamegOszi;

namespace HamegOsziVisualiser {
	public partial class MainForm : Form {

		HamegOsziAdapter adapter;
		private static readonly ILog log = LogManager.GetLogger(typeof(MainForm));
		public MainForm() {
			InitializeComponent();
			log4net.Config.BasicConfigurator.Configure();
			log.Info("Test");
		}

		private void MainForm_Load(object sender, EventArgs e) {
			SerialPortComboBox.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
			SerialPortComboBox.SelectedItem = SerialPortComboBox.Items[0];
		}

		private void ConnectButton_Click(object sender, EventArgs e) {
			adapter = new HamegOsziAdapter(SerialPortComboBox.SelectedItem as string, 115200);
			adapter.SetTimeBase(0.001f);
		}

		private void GetButton_Click(object sender, EventArgs e) {
			if(!adapter.IsConnected)
				return;
				adapter.ResetMeasurmentStatistics(1);
			log.Info(adapter.GetMeasurmentResult(1));
			log.Info(adapter.GetMeasurmentAVG(2));
			log.Info(adapter.GetMeasurmentMin(1));
			log.Info(adapter.GetMeasurmentMax(1));
			log.Info(adapter.GetMeasurmentStdDev(1));
			log.Info(adapter.GetMeasurmentCount(1));
			/*var tmp = adapter.GetWaveformPoints(1);
			var tmp2 = tmp.ToArray();
			DataGraph.UpdateData(ref tmp2, tmp.Count);
			DataGraph.Update();*/
		}
	}
}
