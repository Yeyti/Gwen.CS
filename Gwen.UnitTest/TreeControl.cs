using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Containers", Order = 302)]
	public class TreeControl : GUnit
	{
		public TreeControl(ControlBase parent)
			: base(parent)
		{
			Control.CrossSplitter layout = new Control.CrossSplitter(this);
			layout.Dock = Dock.Fill;

			/* Simple Tree Control */
			{
				Control.TreeControl ctrl = new Control.TreeControl(layout);

				ctrl.AddNode("Node One");
				Control.TreeNode node = ctrl.AddNode("Node Two");
				{
					node.AddNode("Node Two Inside");

					node.AddNode("Eyes");
					{
						node.AddNode("Brown").AddNode("Node Two Inside").AddNode("Eyes").AddNode("Brown");
					}

					Control.TreeNode imgnode = node.AddNode("Image");
					imgnode.SetImage("test16.png");

					imgnode = node.AddNode("Image_Kids");
					imgnode.SetImage("test16.png");
					{
						imgnode.AddNode("Kid1");
						imgnode.AddNode("Kid2");
					}

					node.AddNode("Nodes");
				}
				ctrl.AddNode("Node Three");

				node = ctrl.AddNode("Clickables");
				{
					Control.TreeNode click = node.AddNode("Single Click");
					click.Clicked += NodeClicked;
					click.RightClicked += NodeClicked;

					click = node.AddNode("Double Click");
					click.DoubleClicked += NodeDoubleClicked;
				}

				ctrl.ExpandAll();

				ctrl.Selected += NodeSelected;
				ctrl.Expanded += NodeExpanded;
				ctrl.Collapsed += NodeCollapsed;
			}

			/* Multi select Tree Control */
			{
				Control.TreeControl ctrl = new Control.TreeControl(layout);

				ctrl.AllowMultiSelect = true;

				ctrl.AddNode("Node One");
				Control.TreeNode node = ctrl.AddNode("Node Two");
				node.AddNode("Node Two Inside");
				node.AddNode("Eyes");
				Control.TreeNode nodeTwo = node.AddNode("Brown").AddNode("Node Two Inside").AddNode("Eyes");
				nodeTwo.AddNode("Brown");
				nodeTwo.AddNode("Green");
				nodeTwo.AddNode("Slime");
				nodeTwo.AddNode("Grass");
				nodeTwo.AddNode("Pipe");
				node.AddNode("More");
				node.AddNode("Nodes");

				ctrl.AddNode("Node Three");

				ctrl.ExpandAll();

				ctrl.Selected += NodeSelected;
				ctrl.Expanded += NodeExpanded;
				ctrl.Collapsed += NodeCollapsed;
			}

			/* Normal Tree Control (without using the AddNode function */
			{
				Control.TreeControl ctrl = new Control.TreeControl(layout);

				Control.TreeNode node = new TreeNode(ctrl);
				node.Text = "First";

				new TreeNode(node).Text = "2nd first";

				node = new TreeNode(ctrl);
				node.Text = "Second";

				node = new TreeNode(node);
				node.Text = "Other 2nd";

				ctrl.ExpandAll();
			}

			/* Not expanded Tree Control */
			{
				Control.TreeControl ctrl = new Control.TreeControl(layout);

				ctrl.AddNode("Node One");
				Control.TreeNode node = ctrl.AddNode("Node Two");
				node.AddNode("Node Two Inside");
				node.AddNode("Eyes");
				Control.TreeNode nodeTwo = node.AddNode("Brown").AddNode("Node Two Inside").AddNode("Eyes");
				nodeTwo.AddNode("Brown");
				nodeTwo.AddNode("Green");
				nodeTwo.AddNode("Slime");
				nodeTwo.AddNode("Grass");
				nodeTwo.AddNode("Pipe");
				node.AddNode("More");
				node.AddNode("Nodes");

				ctrl.AddNode("Node Three");

				ctrl.Selected += NodeSelected;
				ctrl.Expanded += NodeExpanded;
				ctrl.Collapsed += NodeCollapsed;
			}
		}

		void NodeCollapsed(ControlBase control, EventArgs args)
		{
			TreeNode node = control as TreeNode;
			UnitPrint(String.Format("Node collapsed: {0}", node.Text));
		}

		void NodeExpanded(ControlBase control, EventArgs args)
		{
			TreeNode node = control as TreeNode;
			UnitPrint(String.Format("Node expanded: {0}", node.Text));
		}

		void NodeSelected(ControlBase control, EventArgs args)
		{
			TreeNode node = control as TreeNode;
			UnitPrint(String.Format("Node selected: {0}", node.Text));
		}

		void NodeClicked(ControlBase control, ClickedEventArgs args)
		{
			TreeNode node = control as TreeNode;
			UnitPrint(String.Format("Node clicked: {0} @({1}, {2})", node.Text, args.X, args.Y));
		}

		void NodeDoubleClicked(ControlBase control, ClickedEventArgs args)
		{
			TreeNode node = control as TreeNode;
			UnitPrint(String.Format("Node double clicked: {0}", node.Text));
		}
	}
}
