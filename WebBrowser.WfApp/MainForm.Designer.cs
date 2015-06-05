namespace WebBrowser.WfApp
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.addressBar = new WebBrowser.WfApp.Controls.AddressBar();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.domTreeControl = new WebBrowser.WfApp.Controls.DomTreeControl();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.consoleControl = new WebBrowser.WfApp.Controls.ConsoleControl();
			this.textBox1 = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// addressBar
			// 
			this.addressBar.Dock = System.Windows.Forms.DockStyle.Top;
			this.addressBar.Engine = null;
			this.addressBar.Location = new System.Drawing.Point(0, 0);
			this.addressBar.Name = "addressBar";
			this.addressBar.Size = new System.Drawing.Size(820, 32);
			this.addressBar.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 32);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.domTreeControl);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(820, 539);
			this.splitContainer1.SplitterDistance = 212;
			this.splitContainer1.TabIndex = 1;
			// 
			// domTreeControl
			// 
			this.domTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.domTreeControl.Engine = null;
			this.domTreeControl.Location = new System.Drawing.Point(0, 0);
			this.domTreeControl.Name = "domTreeControl";
			this.domTreeControl.Size = new System.Drawing.Size(212, 539);
			this.domTreeControl.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.textBox1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.consoleControl);
			this.splitContainer2.Size = new System.Drawing.Size(604, 539);
			this.splitContainer2.SplitterDistance = 405;
			this.splitContainer2.TabIndex = 0;
			// 
			// consoleControl
			// 
			this.consoleControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.consoleControl.Engine = null;
			this.consoleControl.Location = new System.Drawing.Point(0, 0);
			this.consoleControl.Name = "consoleControl";
			this.consoleControl.Size = new System.Drawing.Size(604, 130);
			this.consoleControl.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(604, 405);
			this.textBox1.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(820, 571);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.addressBar);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.AddressBar addressBar;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private Controls.DomTreeControl domTreeControl;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private Controls.ConsoleControl consoleControl;
		private System.Windows.Forms.TextBox textBox1;

	}
}

