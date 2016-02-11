namespace HamegOsziVisualiser {
	partial class MainForm {
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent() {
			this.SerialPortComboBox = new System.Windows.Forms.ComboBox();
			this.ConnectButton = new System.Windows.Forms.Button();
			this.DataGraph = new Hebeler.UI.PointGraph();
			this.GetButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// SerialPortComboBox
			// 
			this.SerialPortComboBox.FormattingEnabled = true;
			this.SerialPortComboBox.Location = new System.Drawing.Point(12, 12);
			this.SerialPortComboBox.Name = "SerialPortComboBox";
			this.SerialPortComboBox.Size = new System.Drawing.Size(121, 21);
			this.SerialPortComboBox.TabIndex = 0;
			// 
			// ConnectButton
			// 
			this.ConnectButton.Location = new System.Drawing.Point(140, 9);
			this.ConnectButton.Name = "ConnectButton";
			this.ConnectButton.Size = new System.Drawing.Size(75, 23);
			this.ConnectButton.TabIndex = 1;
			this.ConnectButton.Text = "Connect";
			this.ConnectButton.UseVisualStyleBackColor = true;
			this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
			// 
			// DataGraph
			// 
			this.DataGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.DataGraph.BackColor = System.Drawing.Color.WhiteSmoke;
			this.DataGraph.BoundaryLimit = 0.01F;
			this.DataGraph.colourEnd = System.Drawing.Color.LightGray;
			this.DataGraph.colourGradientStyle = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
			this.DataGraph.colourStart = System.Drawing.Color.White;
			this.DataGraph.DisplayName = null;
			this.DataGraph.DotColor = System.Drawing.Color.Blue;
			this.DataGraph.LeftRegionWidth = 40;
			this.DataGraph.LineColor = System.Drawing.Color.Red;
			this.DataGraph.Location = new System.Drawing.Point(12, 39);
			this.DataGraph.MismatchLevel = 6;
			this.DataGraph.Name = "DataGraph";
			this.DataGraph.PaddingTopBottom = 2;
			this.DataGraph.PointCollectionMaximumSize = 5000;
			this.DataGraph.SigmaLevel = 1F;
			this.DataGraph.Size = new System.Drawing.Size(461, 367);
			this.DataGraph.TabIndex = 2;
			this.DataGraph.TextPosition = Hebeler.UI.PointGraph.TextPositionStyle.Top;
			// 
			// GetButton
			// 
			this.GetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.GetButton.Location = new System.Drawing.Point(12, 412);
			this.GetButton.Name = "GetButton";
			this.GetButton.Size = new System.Drawing.Size(75, 23);
			this.GetButton.TabIndex = 3;
			this.GetButton.Text = "Get";
			this.GetButton.UseVisualStyleBackColor = true;
			this.GetButton.Click += new System.EventHandler(this.GetButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 439);
			this.Controls.Add(this.GetButton);
			this.Controls.Add(this.DataGraph);
			this.Controls.Add(this.ConnectButton);
			this.Controls.Add(this.SerialPortComboBox);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox SerialPortComboBox;
		private System.Windows.Forms.Button ConnectButton;
		private Hebeler.UI.PointGraph DataGraph;
		private System.Windows.Forms.Button GetButton;
	}
}

