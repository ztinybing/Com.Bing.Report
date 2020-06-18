using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System.Data;


namespace Com.Bing.Report
{
    public class TreeListUtilHelper
    {
		public delegate void OptPerNode(TreeListNode node);
        private TreeListUtilHelper() { }
        public static void CheckMount(TreeListNode treeNode)
        {
            foreach (TreeListNode tn in treeNode.Nodes)
            {
                tn.Checked = treeNode.Checked;
                if (tn.Nodes.Count != 0)
                {
                    CheckMount(tn);
                }
            }
        }
        public static TreeListNodes GetCheckedNode(TreeList treeList)
        {
            TreeListNodes nodes = new TreeListNodes(treeList);
			traversalTreeNode(treeList, new OptPerNode(delegate(TreeListNode node) {
				if (node.Checked && !node.HasChildren)
					nodes.Add(node);
			}));
            return nodes;
        }		
        private static void traversalTreeNode(object treeListOrNode ,OptPerNode optNode )
        {
			if (treeListOrNode is TreeList)
			{
				TreeList treeList = treeListOrNode as TreeList;
				foreach (TreeListNode node in treeList.Nodes)
				{
					optNode(node);
					if (node.HasChildren)
						traversalTreeNode(node, optNode);
				}
			}
			else
			{
				if( treeListOrNode is TreeListNode)
				{
					TreeListNode pnode = treeListOrNode as TreeListNode;
					foreach (TreeListNode node in pnode.Nodes)
					{
						optNode(node);
						if (node.HasChildren)
							traversalTreeNode(node, optNode);
					}
				}
			}
        }
		
    }
}
