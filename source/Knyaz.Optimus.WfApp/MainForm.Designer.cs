using Knyaz.Optimus.WfApp.Controls;

namespace Knyaz.Optimus.WfApp
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.domTreeControl = new DomTreeControl();
			this.consoleControl = new ConsoleControl();
			this.timeLineControl = new TimeLineControl();
			this.addressBar = new AddressBar();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.domTreeControl);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(1093, 429);
			this.splitContainer1.SplitterDistance = 458;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 1;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
			this.splitContainer2.Size = new System.Drawing.Size(630, 429);
			this.splitContainer2.SplitterDistance = 322;
			this.splitContainer2.SplitterWidth = 5;
			this.splitContainer2.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(630, 322);
			this.textBox1.TabIndex = 0;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 39);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.splitContainer1);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.timeLineControl);
			this.splitContainer3.Size = new System.Drawing.Size(1093, 664);
			this.splitContainer3.SplitterDistance = 429;
			this.splitContainer3.TabIndex = 2;
			// 
			// domTreeControl
			// 
			this.domTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.domTreeControl.Engine = null;
			this.domTreeControl.Location = new System.Drawing.Point(0, 0);
			this.domTreeControl.Margin = new System.Windows.Forms.Padding(5);
			this.domTreeControl.Name = "domTreeControl";
			this.domTreeControl.Size = new System.Drawing.Size(458, 429);
			this.domTreeControl.TabIndex = 0;
			// 
			// consoleControl
			// 
			this.consoleControl.AutoScroll = true;
			this.consoleControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.consoleControl.Engine = null;
			this.consoleControl.Location = new System.Drawing.Point(0, 0);
			this.consoleControl.Margin = new System.Windows.Forms.Padding(5);
			this.consoleControl.Name = "consoleControl";
			this.consoleControl.Size = new System.Drawing.Size(630, 102);
			this.consoleControl.TabIndex = 0;
			// 
			// timeLineControl
			// 
			this.timeLineControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.timeLineControl.Engine = null;
			this.timeLineControl.Location = new System.Drawing.Point(0, 0);
			this.timeLineControl.Name = "timeLineControl";
			this.timeLineControl.Size = new System.Drawing.Size(1093, 231);
			this.timeLineControl.TabIndex = 0;
			// 
			// addressBar
			// 
			this.addressBar.Dock = System.Windows.Forms.DockStyle.Top;
			this.addressBar.Engine = null;
			this.addressBar.Location = new System.Drawing.Point(0, 0);
			this.addressBar.Margin = new System.Windows.Forms.Padding(5);
			this.addressBar.Name = "addressBar";
			this.addressBar.Size = new System.Drawing.Size(1093, 39);
			this.addressBar.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1093, 703);
			this.Controls.Add(this.splitContainer3);
			this.Controls.Add(this.addressBar);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private AddressBar addressBar;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private DomTreeControl domTreeControl;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private ConsoleControl consoleControl;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private TimeLineControl timeLineControl;

	}
}

