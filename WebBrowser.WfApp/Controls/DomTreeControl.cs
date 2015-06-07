using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

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
					_engine.DocumentChanged -= EngineOnDocumentChanged;
				_engine = value;
				if(_engine != null)
					_engine.DocumentChanged+=EngineOnDocumentChanged;
			}
		}

		private void EngineOnDocumentChanged()
		{
			Document = _engine.Document;
			this.SafeInvoke(() => treeView1.Nodes.Clear());
		}

		private Document Document
		{
			get { return _document; }
			set
			{
				if (_document != null)
					_document.DomNodeInserted -= DocumentOnDomNodeInserted;
					
				_document = value;
				if(_document != null)
					_document.DomNodeInserted += DocumentOnDomNodeInserted;
			}
		}

		private Dictionary<Node, TreeNode> _nodes = new Dictionary<Node, TreeNode>();

		//todo: the index on node inserted should be considered.
		private void DocumentOnDomNodeInserted(Node parent, Node child)
		{
			//todo: do not fire event if item not in document.
			Node praParent = parent;
			while (praParent.ParentNode != null)
				praParent = praParent.ParentNode;

			if (praParent != Document)
				return;

			this.SafeInvoke(() =>
				{
					var targetTreeNodeCollection = FindTreeNodeCollection(parent);
					if (targetTreeNodeCollection == null)
						return;

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

/*
			foreach (var child in node.ChildNodes.ToArray().Select(x => CreateBranch(x)).Where(x => x!=null))
			{
				treeNode.Nodes.Add(child);
			}*/
			if(!_nodes.ContainsKey(node))
				_nodes.Add(node, treeNode);
			return treeNode;
		}

		private TreeNodeCollection FindTreeNodeCollection(Node parent)
		{
			if (parent == _engine.Document.DocumentElement)
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
	}
}
