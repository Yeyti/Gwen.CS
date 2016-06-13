using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Standard", Order = 200)]
	public class Button : GUnit
    {
        public Button(ControlBase parent)
            : base(parent)
        {
			HorizontalLayout hlayout = new HorizontalLayout(this);
			{
				VerticalLayout vlayout = new VerticalLayout(hlayout);
				vlayout.Width = 300;
				{
					Gwen.Control.Button button;

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Text = "Button";

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Text = "Image button (default)";
					button.SetImage("test16.png");

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Text = "Image button (above)";
					button.SetImage("test16.png", ImageAlign.Above);

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Alignment = Alignment.Left | Alignment.CenterV;
					button.Text = "Image button (left)";
					button.SetImage("test16.png");

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Alignment = Alignment.Right | Alignment.CenterV;
					button.Text = "Image button (right)";
					button.SetImage("test16.png");

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Text = "Image button (image left)";
					button.SetImage("test16.png", ImageAlign.Left | ImageAlign.CenterV);

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Text = "Image button (image right)";
					button.SetImage("test16.png", ImageAlign.Right | ImageAlign.CenterV);

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Text = "Image button (image fill)";
					button.SetImage("test16.png", ImageAlign.Fill);

					HorizontalLayout hlayout2 = new HorizontalLayout(vlayout);
					{
						button = new Control.Button(hlayout2);
						button.HorizontalAlignment = HorizontalAlignment.Left;
						button.Padding = Padding.Three;
						button.Margin = Margin.Five;
						button.SetImage("test16.png");
						button.ImageSize = new Size(32, 32);

						button = new Control.Button(hlayout2);
						button.HorizontalAlignment = HorizontalAlignment.Left;
						button.VerticalAlignment = VerticalAlignment.Center;
						button.Padding = Padding.Three;
						button.Margin = Margin.Five;
						button.SetImage("test16.png");

						button = new Control.Button(hlayout2);
						button.HorizontalAlignment = HorizontalAlignment.Left;
						button.VerticalAlignment = VerticalAlignment.Center;
						button.Padding = Padding.Three;
						button.Margin = Margin.Five;
						button.SetImage("test16.png");
						button.ImageTextureRect = new Rectangle(4, 4, 8, 8);

						button = new Control.Button(hlayout2);
						button.HorizontalAlignment = HorizontalAlignment.Left;
						button.VerticalAlignment = VerticalAlignment.Center;
						button.Padding = Padding.Three;
						button.Margin = Margin.Five;
						button.SetImage("test16.png");
						button.ImageColor = Color.DarkGrey;
					}

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = new Padding(20, 20, 20, 20);
					button.Text = "Toggle me";
					button.IsToggle = true;
					button.Toggled += onToggle;
					button.ToggledOn += onToggleOn;
					button.ToggledOff += onToggleOff;

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Text = "Disabled";
					button.IsDisabled = true;

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Text = "With Tooltip";
					button.SetToolTipText("This is tooltip");

					button = new Control.Button(vlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Text = "Autosized";
					button.HorizontalAlignment = HorizontalAlignment.Left;
				}

				{
					Control.Button button = new Control.Button(hlayout);
					button.Margin = Margin.Five;
					button.Padding = Padding.Three;
					button.Text = "Event tester";
					button.Size = new Size(300, 200);
					button.Pressed += onButtonAp;
					button.Clicked += onButtonAc;
					button.Released += onButtonAr;
				}
			}
		}

		private void onButtonAc(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: Clicked");
        }

		private void onButtonAp(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: Pressed");
        }

		private void onButtonAr(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: Released");
        }

		private void onToggle(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: Toggled");
        }

		private void onToggleOn(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: ToggleOn");
        }

		private void onToggleOff(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: ToggledOff");
        }
    }
}
