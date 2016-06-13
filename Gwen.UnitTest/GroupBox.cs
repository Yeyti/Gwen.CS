using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Non-Interactive", Order = 103)]
	public class GroupBox : GUnit
    {
        public GroupBox(ControlBase parent)
			: base(parent)
        {
			Control.Layout.GridLayout layout = new Control.Layout.GridLayout(this);
			layout.ColumnCount = 2;

            {
                Control.GroupBox gb = new Control.GroupBox(layout);
				gb.Size = new Size(200, 100);
                gb.Text = "Group Box";
            }

            {
                Control.GroupBox gb = new Control.GroupBox(layout);
				gb.HorizontalAlignment = HorizontalAlignment.Left;
				gb.VerticalAlignment = VerticalAlignment.Top;
                gb.Text = "With Label (autosized)";
                Control.Label label = new Control.Label(gb);
				label.Dock = Dock.Fill;
                label.Text = "I'm a label";
            }

            {
                Control.GroupBox gb = new Control.GroupBox(layout);
				gb.HorizontalAlignment = HorizontalAlignment.Left;
				gb.VerticalAlignment = VerticalAlignment.Top;
				gb.Text = "With Label (autosized)";
                Control.Label label = new Control.Label(gb);
				label.Dock = Dock.Fill;
				label.Text = "I'm a label. I'm a really long label!";
            }

            {
                Control.GroupBox gb = new Control.GroupBox(layout);
				gb.HorizontalAlignment = HorizontalAlignment.Left;
				gb.VerticalAlignment = VerticalAlignment.Top;
				gb.Text = "Two docked Labels (autosized)";
				Control.ControlBase gbl = new Control.Layout.DockLayout(gb);
                Control.Label label1 = new Control.Label(gbl);
                label1.Text = "I'm a label";
                label1.Dock = Dock.Top;
                Control.Label label2 = new Control.Label(gbl);
                label2.Text = "I'm a label. I'm a really long label!";
                label2.Dock = Dock.Top;
            }

            {
                Control.GroupBox gb = new Control.GroupBox(layout);
				gb.HorizontalAlignment = HorizontalAlignment.Left;
				gb.VerticalAlignment = VerticalAlignment.Top;
				gb.Text = "Empty (autosized)";
            }

            {
                Control.GroupBox gb1 = new Control.GroupBox(layout);
				gb1.HorizontalAlignment = HorizontalAlignment.Left;
				gb1.VerticalAlignment = VerticalAlignment.Top;
				gb1.Padding = Padding.Five;
                gb1.Text = "Yo dawg,";
				Control.ControlBase gb1l = new Control.Layout.DockLayout(gb1);

				Control.GroupBox gb2 = new Control.GroupBox(gb1l);
                gb2.Text = "I herd";
                gb2.Dock = Dock.Left;
                gb2.Margin = Margin.Three;
                gb2.Padding = Padding.Five;
                
                Control.GroupBox gb3 = new Control.GroupBox(gb1l);
                gb3.Text = "You like";
                gb3.Dock = Dock.Fill;
				Control.ControlBase gb3l = new Control.Layout.DockLayout(gb3);

				Control.GroupBox gb4 = new Control.GroupBox(gb3l);
                gb4.Text = "Group Boxes,";
                gb4.Dock = Dock.Top;

                Control.GroupBox gb5 = new Control.GroupBox(gb3l);
                gb5.Text = "So I put Group";
                gb5.Dock = Dock.Fill;
				Control.ControlBase gb5l = new Control.Layout.DockLayout(gb5);

				Control.GroupBox gb6 = new Control.GroupBox(gb5l);
                gb6.Text = "Boxes in yo";
                gb6.Dock = Dock.Left;

                Control.GroupBox gb7 = new Control.GroupBox(gb5l);
                gb7.Text = "Boxes so you can";
                gb7.Dock = Dock.Top;
				Control.ControlBase gb7l = new Control.Layout.DockLayout(gb7);

				Control.GroupBox gb8 = new Control.GroupBox(gb7l);
                gb8.Text = "Group Box while";
                gb8.Dock = Dock.Top;
                gb8.Margin = Gwen.Margin.Five;

                Control.GroupBox gb9 = new Control.GroupBox(gb7l);
                gb9.Text = "u Group Box";
                gb9.Dock = Dock.Bottom;
                gb9.Padding = Gwen.Padding.Five;
            }
        }
    }
}
