﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.WfApp.Tools;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;

namespace Knyaz.Optimus.WfApp.Controls
{
	public partial class DomTreeControl : UserControl
	{
		private Engine _engine;
		private Document _document;

		public DomTreeControl()
		{
			InitializeComponent();
		}

		public Engine Engine
		{
			get { return _engine; }
			set
			{
				if (_engine != null)
				{
					Document = null;
					_engine.DocumentChanged -= OnDocumentChanged;
				}
					
				_engine = value;
				if (_engine != null)
				{
					Document = _engine.Document;
					_engine.DocumentChanged += OnDocumentChanged;

					this.SafeBeginInvoke(() => treeView1.Nodes.Clear());
				}
			}
		}

		private void OnDocumentChanged()
		{
			Document = _engine.Document;
		}


		private Document Document
		{
			get { return _document; }
			set
			{
				if (_document != null)
				{
					_document.NodeInserted -= DocumentOnDomNodeInserted;
					_document.NodeRemoved -= DocumentOnNodeRemoved;
				}
				treeView1.Nodes.Clear();
				_document = value;
				if (_document != null)
				{
					_document.NodeInserted += DocumentOnDomNodeInserted;
					_document.NodeRemoved += DocumentOnNodeRemoved;

					//show exist nodes;
					foreach (var node in _document.ChildNodes)
					{
						DocumentOnDomNodeInserted(node);
					}
				}
			}
		}

		private void DocumentOnNodeRemoved(Node parent, Node node)
		{
			var nodeToremove = FindTreeNode(node);

			if (nodeToremove != null)
				this.SafeInvoke(() => nodeToremove.Parent.Nodes.Remove(nodeToremove));
		}

		private Dictionary<Node, TreeNode> _nodes = new Dictionary<Node, TreeNode>();

		//todo: the index on node inserted should be considered.
		private void DocumentOnDomNodeInserted(Node child)
		{
			var targetTreeNodeCollection = FindTreeNodeCollection(child.ParentNode);
			if (targetTreeNodeCollection == null)
				return;

			this.SafeBeginInvoke(() =>
				{
					var newTreeNode = CreateBranch(child);

					if (newTreeNode != null)
					{
						var idx = child.ParentNode.ChildNodes.IndexOf(child);
						targetTreeNodeCollection.Insert(idx, newTreeNode);
					}
				});
		}

		private TreeNode CreateBranch(Node node)
		{
			string name = null;

			var element = node as Element;
			if (element != null)
			{
				name = "<" + element.TagName + ">";
			}
			else
			{
				if (node is Comment comment)
				{
					name = "<!-- " + comment.Data.Substring(0, Math.Min(comment.Data.Length, 50)) + "-->";
				}
				else
				{
					if (node is Attr attr)
					{
						name = attr.Name;
						if (attr.Value != null)
						{
							name += ": " + attr.Value;
						}
					}
					else
					{
						if (node is Text text)
						{
							name = "\"" + text.Data.Substring(0, Math.Min(text.Data.Length, 50)) + "\"";
						}
					}
				}
			}

			if (name == null)
				name = node.ToString();
				
			var treeNode = new TreeNode(name) {Tag = node};


			var nodes = node.ChildNodes.CopyListThreadSafe();	
			
			foreach (var child in nodes.Select(CreateBranch).Where(x => x!=null))
			{
				treeNode.Nodes.Add(child);
			}
			if(!_nodes.ContainsKey(node))
				_nodes.Add(node, treeNode);
			return treeNode;
		}

		private TreeNode FindTreeNode(Node node)
		{
			TreeNode treeNode;
			return _nodes.TryGetValue(node, out treeNode) ? treeNode : null;
		}

		private TreeNodeCollection FindTreeNodeCollection(Node parent)
		{
			if (parent == _engine.Document)
				return treeView1.Nodes;

			TreeNode treeNode;
			if (_nodes.TryGetValue(parent, out treeNode))
				return treeNode.Nodes;

			return null;
		}

		public event Action<TreeNode> NodeSelected;

		private void OnTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			if (NodeSelected != null)
				NodeSelected(e.Node);
		}

		private void clickToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var node = treeView1.SelectedNode;
			if (node != null)
			{
				var elt = node.Tag as HtmlElement;
				if (elt != null)
				{
					Task.Run(() => elt.Click());
					return;
				}
			}

			MessageBox.Show("Nothing to click");
		}

		private void setAttributeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var node = treeView1.SelectedNode;
			if (node == null)
				return;

			var elt = node.Tag as HtmlElement;
			if(elt == null)
				return;

			var dlg = new SetAttributeForm {Element = elt};
			dlg.Show();
		}
	}
}
