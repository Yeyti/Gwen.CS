using System;
using Gwen.Control;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Non-Interactive", Order = 101)]
	public class LinkLabel : GUnit
	{
		private readonly Font font1;
		private readonly Font fontHover1;

		public LinkLabel(ControlBase parent)
			: base(parent)
        {
            {
				Control.LinkLabel label = new Control.LinkLabel(this);
				label.Dock = Dock.Top;
				label.HoverColor = new Color(255, 255, 255, 255);
				label.Text = "Link Label (default font)";
				label.Link = "Test Link";
				label.LinkClicked += OnLinkClicked;
            }
			{
				font1 = new Font(Skin.Renderer, "Comic Sans MS", 25);
				fontHover1 = new Font(Skin.Renderer, "Comic Sans MS", 25);
				fontHover1.Underline = true;

				Control.LinkLabel label = new Control.LinkLabel(this);
				label.Dock = Dock.Top;
				label.Font = font1;
				label.HoverFont = fontHover1;
				label.TextColor = new Color(255, 0, 80, 205);
				label.HoverColor = new Color(255, 0, 100, 255);
				label.Text = "Custom Font (Comic Sans 25)";
				label.Link = "Custom Font Link";
				label.LinkClicked += OnLinkClicked;
			}
		}

		public override void Dispose()
		{
			font1.Dispose();
			fontHover1.Dispose();
			base.Dispose();
		}

		private void OnLinkClicked(ControlBase control, LinkClickedEventArgs args)
		{
			UnitPrint("Link Clicked: " + args.Link);
		}
	}
}
