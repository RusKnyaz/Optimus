namespace Knyaz.Optimus.WfApp.Controls
{
	partial class TimeLineControl
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
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this._listView = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.OnTick);
			// 
			// _listView
			// 
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this._listView.Location = new System.Drawing.Point(0, 0);
			this._listView.Name = "_listView";
			this._listView.OwnerDraw = true;
			this._listView.Size = new System.Drawing.Size(573, 236);
			this._listView.TabIndex = 0;
			this._listView.UseCompatibleStateImageBehavior = false;
			this._listView.View = System.Windows.Forms.View.Details;
			this._listView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.OnListDrawItem);
			this._listView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.OnListDrawSubItem);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "URI";
			this.columnHeader1.Width = 200;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Lifeline";
			this.columnHeader2.Width = 1000;
			// 
			// TimeLineControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._listView);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "TimeLineControl";
			this.Size = new System.Drawing.Size(573, 236);
			this.SizeChanged += new System.EventHandler(this.TimeLineControl_SizeChanged);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ListView _listView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
	}
}
