using System;
using Gwen.Control;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Layout", Order = 401)]
	public class FlowLayout : GUnit
	{
		public FlowLayout(ControlBase parent)
			: base(parent)
		{
			ControlBase layout = new Control.Layout.DockLayout(this);

			Control.Layout.FlowLayout flowLayout = new Control.Layout.FlowLayout(layout);
			flowLayout.Width = 200;
			flowLayout.Padding = Padding.Five;
			flowLayout.Dock = Dock.Fill;
			flowLayout.DrawDebugOutlines = true;
			{
				Control.Button button;
				int buttonNum = 1;
				const int buttonCount = 10;

				for (int n = 0; n < buttonCount; n++)
				{
					button = new Control.Button(flowLayout);
					button.VerticalAlignment = VerticalAlignment.Top;
					button.HorizontalAlignment = HorizontalAlignment.Left;
					button.Margin = Margin.Five;
					button.Padding = Padding.Five;
					button.ShouldDrawBackground = false;
					button.Text = String.Format("Button {0}", buttonNum++);
					button.SetImage("test16.png", ImageAlign.Above);
				}
			}

			Control.HorizontalSlider flowLayoutWidth = new HorizontalSlider(layout);
			flowLayoutWidth.Margin = Margin.Five;
			flowLayoutWidth.Width = 500;
			flowLayoutWidth.Dock = Dock.Top;
			flowLayoutWidth.Min = 50;
			flowLayoutWidth.Max = 500;
			flowLayoutWidth.Value = flowLayout.Width;
			flowLayoutWidth.ValueChanged += (control, args) => { flowLayout.Width = (int)flowLayoutWidth.Value; };
		}
	}
}
