using System;
using Gwen.Control;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Containers", Order = 304)]
	public class TabControl : GUnit
    {
        private readonly Control.TabControl m_DockControl;

        public TabControl(ControlBase parent)
            : base(parent)
        {
            {
                m_DockControl = new Control.TabControl(this);
				m_DockControl.Margin = Margin.Zero;
				m_DockControl.Width = 200;
				//m_DockControl.Height = 150;
				m_DockControl.Dock = Dock.Top;

                {
                    Control.Internal.TabButton button = m_DockControl.AddPage("Controls");
                    ControlBase page = button.Page;

                    {
						Control.GroupBox group = new Control.GroupBox(page);
						group.Text = "Tab position";
						Control.RadioButtonGroup radio = new Control.RadioButtonGroup(group);

                        radio.AddOption("Top").Select();
                        radio.AddOption("Bottom");
                        radio.AddOption("Left");
                        radio.AddOption("Right");

                        radio.SelectionChanged += OnDockChange;
                    }
                }

                m_DockControl.AddPage("Red");
                m_DockControl.AddPage("Green");
                m_DockControl.AddPage("Blue");
				m_DockControl.AddPage("Blue");
				m_DockControl.AddPage("Blue");
			}

			{
                Control.TabControl dragMe = new Control.TabControl(this);
				dragMe.Margin = Margin.Five;
				dragMe.Width = 200;
				dragMe.Dock = Dock.Top;

                dragMe.AddPage("You");
                dragMe.AddPage("Can");
                dragMe.AddPage("Reorder").SetImage("test16.png");
                dragMe.AddPage("These");
                dragMe.AddPage("Tabs");

                dragMe.AllowReorder = true;
            }
        }

		void OnDockChange(ControlBase control, EventArgs args)
        {
            RadioButtonGroup rc = (RadioButtonGroup)control;

            if (rc.SelectedLabel == "Top") m_DockControl.TabStripPosition = Dock.Top;
            if (rc.SelectedLabel == "Bottom") m_DockControl.TabStripPosition = Dock.Bottom;
            if (rc.SelectedLabel == "Left") m_DockControl.TabStripPosition = Dock.Left;
            if (rc.SelectedLabel == "Right") m_DockControl.TabStripPosition = Dock.Right;
        }
    }
}
