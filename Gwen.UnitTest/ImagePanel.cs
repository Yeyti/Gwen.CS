using System;
using Gwen.Control;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Non-Interactive", Order = 105)]
	public class ImagePanel : GUnit
    {
        public ImagePanel(ControlBase parent)
            : base(parent)
        {
            /* Normal */
            {
                Control.ImagePanel img = new Control.ImagePanel(this);
				img.Margin = Margin.Five;
				img.Dock = Dock.Top;
				img.Size = new Size(100, 100);
				img.ImageName = "gwen.png";
            }

            /* Missing */
            {
                Control.ImagePanel img = new Control.ImagePanel(this);
				img.Margin = Margin.Five;
				img.Dock = Dock.Top;
				img.Size = new Size(100, 100);
				img.ImageName = "missingimage.png";
            }

			/* Clicked */
			{
				Control.ImagePanel img = new Control.ImagePanel(this);
				img.Margin = Margin.Five;
				img.Dock = Dock.Top;
				img.Size = new Size(100, 100);
				img.ImageName = "gwen.png";
				img.Clicked += Image_Clicked;
			}
        }

		void Image_Clicked(ControlBase control, EventArgs args) {
			UnitPrint("Image: Clicked");
		}
    }
}
