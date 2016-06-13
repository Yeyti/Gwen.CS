using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Layout", Order = 402)]
	public class AnchorLayout : GUnit
	{
		private readonly Font m_Font;

		public AnchorLayout(ControlBase parent)
			: base(parent)
		{
			m_Font = new Font(Skin.Renderer, "Arial", 10);

			Control.Layout.AnchorLayout layout = new Control.Layout.AnchorLayout(this);
			layout.Size = new Size(445, 165);
			layout.Padding = Padding.Five;
			layout.AnchorBounds = new Rectangle(0, 0, 445, 165);

			Control.Button button = new Control.Button(layout);
			button.Font = m_Font;
			button.Text = "Left Top";
			button.AnchorBounds = new Rectangle(10, 10, 100, 20);
			button.Anchor = Anchor.LeftTop;

			button = new Control.Button(layout);
			button.Font = m_Font;
			button.Text = "Center Top";
			button.AnchorBounds = new Rectangle(150, 10, 100, 20);
			button.Anchor = new Anchor(50, 0, 50, 0);

			button = new Control.Button(layout);
			button.Font = m_Font;
			button.Text = "Right Top";
			button.AnchorBounds = new Rectangle(290, 10, 100, 20);
			button.Anchor = Anchor.RightTop;

			button = new Control.Button(layout);
			button.Font = m_Font;
			button.Text = "Left Center";
			button.AnchorBounds = new Rectangle(10, 50, 100, 20);
			button.Anchor = new Anchor(0, 50, 0, 50);

			button = new Control.Button(layout);
			button.Font = m_Font;
			button.Text = "Center";
			button.AnchorBounds = new Rectangle(150, 50, 100, 20);
			button.Anchor = new Anchor(50, 50, 50, 50);

			button = new Control.Button(layout);
			button.Font = m_Font;
			button.Text = "Right Center";
			button.AnchorBounds = new Rectangle(290, 50, 100, 20);
			button.Anchor = new Anchor(100, 50, 100, 50);

			button = new Control.Button(layout);
			button.Font = m_Font;
			button.Text = "Left Bottom";
			button.AnchorBounds = new Rectangle(10, 90, 100, 20);
			button.Anchor = Anchor.LeftBottom;

			button = new Control.Button(layout);
			button.Font = m_Font;
			button.Text = "Center Bottom";
			button.AnchorBounds = new Rectangle(150, 90, 100, 20);
			button.Anchor = new Anchor(50, 100, 50, 100);

			button = new Control.Button(layout);
			button.Font = m_Font;
			button.Text = "Right Bottom";
			button.AnchorBounds = new Rectangle(290, 90, 100, 20);
			button.Anchor = Anchor.RightBottom;

			Control.HorizontalSlider horz = new Control.HorizontalSlider(layout);
			horz.AnchorBounds = new Rectangle(10, 125, 380, 25);
			horz.Anchor = new Anchor(0, 100, 100, 100);

			Control.VerticalSlider vert = new Control.VerticalSlider(layout);
			vert.AnchorBounds = new Rectangle(405, 10, 25, 100);
			vert.Anchor = new Anchor(100, 0, 100, 100);

			Control.HorizontalSlider width = new Control.HorizontalSlider(this);
			width.Min = 445;
			width.Max = 800;
			width.Height = 25;
			width.Dock = Dock.Bottom;
			width.Padding = Padding.Five;
			width.ValueChanged += (control, args) => { layout.Width = (int)(control as Control.HorizontalSlider).Value; };

			Control.VerticalSlider height = new Control.VerticalSlider(this);
			height.Min = 165;
			height.Max = 400;
			height.Width = 25;
			height.Dock = Dock.Right;
			height.Padding = Padding.Five;
			height.ValueChanged += (control, args) => { layout.Height = (int)(control as Control.VerticalSlider).Value; };
		}

		public override void Dispose()
		{
			m_Font.Dispose();
			base.Dispose();
		}
	}
}
