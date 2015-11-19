using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;
using HtmlElement = WebBrowser.Dom.Elements.HtmlElement;

namespace WebBrowser.WfApp.Controls
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
					_document.RemoveEventListener("DOMContentLoaded",  OnDocumentLoaded, false);
				}
				treeView1.Nodes.Clear();
				_document = value;
				if (_document != null)
				{
					_document.NodeInserted += DocumentOnDomNodeInserted;
					_document.NodeRemoved += DocumentOnNodeRemoved;
					_document.AddEventListener("DOMContentLoaded", OnDocumentLoaded, false);

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

		private void OnDocumentLoaded(Event @event)
		{
			foreach (var childNode in _document.DocumentElement.ChildNodes)
			{
				DocumentOnDomNodeInserted(childNode);
			}
		}

		private Dictionary<Node, TreeNode> _nodes = new Dictionary<Node, TreeNode>();

		//todo: the index on node inserted should be considered.
		private void DocumentOnDomNodeInserted(Node child)
		{
			var targetTreeNodeCollection = FindTreeNodeCollection(child.ParentNode);
			if (targetTreeNodeCollection == null)
				return;

			this.SafeInvoke(() =>
				{
					var newTreeNode = CreateBranch(child);
					if (newTreeNode != null)
						targetTreeNodeCollection.Add(newTreeNode);		
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
				var comment = node as Comment;
				if (comment != null)
				{
					name = "<!-- " + comment.Text.Substring(0, Math.Min(comment.Text.Length, 50)) + "-->";
				}
				else
				{
					var attr = node as Attr;
					if (attr != null)
					{
						name = attr.Name;
						if (attr.Value != null)
						{
							name += ": " + attr.Value;
						}
					}
					else
					{
						var text = node as Text;
						if (text != null)
						{
							name = "\"" + text.Data.Substring(0, Math.Min(text.Data.Length, 50)) + "\"";
						}
					}
				}
			}
				
			var treeNode = new TreeNode(name) {Tag = node};

			foreach (var child in node.ChildNodes.ToArray().Select(x => CreateBranch(x)).Where(x => x!=null))
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
					elt.Click();
					return;
				}
			}

			MessageBox.Show("Nothing to click");
		}
	}
}
