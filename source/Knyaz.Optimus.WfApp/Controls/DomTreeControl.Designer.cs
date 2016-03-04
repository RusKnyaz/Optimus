namespace WebBrowser.WfApp.Controls
{
	partial class DomTreeControl
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
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.clickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Margin = new System.Windows.Forms.Padding(4);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(220, 292);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeViewAfterSelect);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clickToolStripMenuItem,
            this.setAttributeToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(182, 84);
			// 
			// clickToolStripMenuItem
			// 
			this.clickToolStripMenuItem.Name = "clickToolStripMenuItem";
			this.clickToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
			this.clickToolStripMenuItem.Text = "click";
			this.clickToolStripMenuItem.Click += new System.EventHandler(this.clickToolStripMenuItem_Click);
			// 
			// setAttributeToolStripMenuItem
			// 
			this.setAttributeToolStripMenuItem.Name = "setAttributeToolStripMenuItem";
			this.setAttributeToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
			this.setAttributeToolStripMenuItem.Text = "set attribute";
			this.setAttributeToolStripMenuItem.Click += new System.EventHandler(this.setAttributeToolStripMenuItem_Click);
			// 
			// DomTreeControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.treeView1);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "DomTreeControl";
			this.Size = new System.Drawing.Size(220, 292);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem clickToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setAttributeToolStripMenuItem;
	}
}
