namespace Knyaz.Optimus.WfApp.Controls
{
	partial class AddressBar
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBoxUrl = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBoxUrl
			// 
			this.textBoxUrl.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Knyaz.Optimus.WfApp.Properties.Settings.Default, "LastUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxUrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxUrl.Location = new System.Drawing.Point(0, 0);
			this.textBoxUrl.Margin = new System.Windows.Forms.Padding(4);
			this.textBoxUrl.Name = "textBoxUrl";
			this.textBoxUrl.Size = new System.Drawing.Size(899, 22);
			this.textBoxUrl.TabIndex = 0;
			this.textBoxUrl.Text = global::Knyaz.Optimus.WfApp.Properties.Settings.Default.LastUrl;
			this.textBoxUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxUrl_KeyDown);
			// 
			// AddressBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBoxUrl);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "AddressBar";
			this.Size = new System.Drawing.Size(899, 29);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxUrl;
	}
}
