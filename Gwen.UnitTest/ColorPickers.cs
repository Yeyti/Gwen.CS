using System;
using Gwen.Control;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Non-standard", Order = 501)]
	public class ColorPickers : GUnit
    {
        public ColorPickers(ControlBase parent)
            : base(parent)
        {
            /* RGB Picker */
            {
                ColorPicker rgbPicker = new ColorPicker(this);
				rgbPicker.Dock = Dock.Top;
				rgbPicker.ColorChanged += ColorChanged;
            }

            /* HSVColorPicker */
            {
                HSVColorPicker hsvPicker = new HSVColorPicker(this);
				hsvPicker.Dock = Dock.Fill;
				hsvPicker.HorizontalAlignment = HorizontalAlignment.Left;
				hsvPicker.VerticalAlignment = VerticalAlignment.Top;
				hsvPicker.ColorChanged += ColorChanged;
            }

            /* HSVColorPicker in Window */
            {
				Control.Window window = new Control.Window(base.GetCanvas());
                window.Size = new Size(300, 200);
                window.Collapse();
				Control.Layout.DockLayout layout = new Control.Layout.DockLayout(window);

                HSVColorPicker hsvPicker = new HSVColorPicker(layout);
				hsvPicker.Margin = Margin.Two;
				hsvPicker.Dock = Dock.Fill;
                hsvPicker.ColorChanged += ColorChanged;

                Control.Button OpenWindow = new Control.Button(this);
				OpenWindow.Dock = Dock.Bottom;
				OpenWindow.HorizontalAlignment = HorizontalAlignment.Left;
				OpenWindow.Text = "Open Window";
				OpenWindow.Clicked += delegate(ControlBase sender, ClickedEventArgs args)
                {
                    window.Show();
                };
            }
        }

		void ColorChanged(ControlBase control, EventArgs args)
        {
            IColorPicker picker = control as IColorPicker;
            Color c = picker.SelectedColor;
            HSV hsv = c.ToHSV();
            String text = String.Format("Color changed: RGB: {0:X2}{1:X2}{2:X2} HSV: {3:F1} {4:F2} {5:F2}",
                                        c.R, c.G, c.B, hsv.H, hsv.S, hsv.V);
            UnitPrint(text);
        }
    }
}
