using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Layout", Order = 400)]
	public class Docking : GUnit
	{
		private readonly Font font;
		private Control.ControlBase outer;

		public Docking(ControlBase parent)
			: base(parent)
		{
			font = Skin.DefaultFont.Copy();
			font.Size *= 2;

			Control.Label inner1, inner2, inner3, inner4, inner5;

			HorizontalLayout hlayout = new HorizontalLayout(this);
			{
				VerticalLayout vlayout = new VerticalLayout(hlayout);
				{
					outer = new Control.Layout.DockLayout(vlayout);
					outer.Size = new Size(400, 400);
					{
						inner1 = new Control.Label(outer);
						inner1.Alignment = Alignment.Center;
						inner1.Text = "1";
						inner1.Font = font;
						inner1.Size = new Size(100, Util.Ignore);
						inner1.Dock = Dock.Left;

						inner2 = new Control.Label(outer);
						inner2.Alignment = Alignment.Center;
						inner2.Text = "2";
						inner2.Font = font;
						inner2.Size = new Size(Util.Ignore, 100);
						inner2.Dock = Dock.Top;

						inner3 = new Control.Label(outer);
						inner3.Alignment = Alignment.Center;
						inner3.Text = "3";
						inner3.Font = font;
						inner3.Size = new Size(100, Util.Ignore);
						inner3.Dock = Dock.Right;

						inner4 = new Control.Label(outer);
						inner4.Alignment = Alignment.Center;
						inner4.Text = "4";
						inner4.Font = font;
						inner4.Size = new Size(Util.Ignore, 100);
						inner4.Dock = Dock.Bottom;

						inner5 = new Control.Label(outer);
						inner5.Alignment = Alignment.Center;
						inner5.Text = "5";
						inner5.Font = font;
						inner5.Size = new Size(Util.Ignore, Util.Ignore);
						inner5.Dock = Dock.Fill;
					}

					outer.DrawDebugOutlines = true;

					HorizontalLayout hlayout2 = new HorizontalLayout(vlayout);
					{
						Control.Label l_padding = new Control.Label(hlayout2);
						l_padding.Text = "Padding:";

						Control.HorizontalSlider padding = new Control.HorizontalSlider(hlayout2);
						padding.Min = 0;
						padding.Max = 200;
						padding.Value = 10;
						padding.Width = 100;
						padding.ValueChanged += PaddingChanged;
					}
				}

				Control.Layout.GridLayout controlsLayout = new Control.Layout.GridLayout(hlayout);
				controlsLayout.ColumnCount = 2;
				{
					inner1.UserData = CreateControls(inner1, Dock.Left, "Control 1", controlsLayout);
					inner2.UserData = CreateControls(inner2, Dock.Top, "Control 2", controlsLayout);
					inner3.UserData = CreateControls(inner3, Dock.Right, "Control 3", controlsLayout);
					inner4.UserData = CreateControls(inner4, Dock.Bottom, "Control 4", controlsLayout);
					inner5.UserData = CreateControls(inner5, Dock.Fill, "Control 5", controlsLayout);
				}
			}
			//DrawDebugOutlines = true;
		}

		ControlBase CreateControls(Control.ControlBase subject, Dock docking, string name, ControlBase container)
		{
			Control.GroupBox gb = new Control.GroupBox(container);
			gb.Text = name;
			{
				HorizontalLayout hlayout = new HorizontalLayout(gb);
				{
					Control.GroupBox dgb = new Control.GroupBox(hlayout);
					dgb.Text = "Dock";
					{
						Control.RadioButtonGroup dock = new RadioButtonGroup(dgb);
						dock.UserData = subject;
						dock.AddOption("Left", null, Dock.Left);
						dock.AddOption("Top", null, Dock.Top);
						dock.AddOption("Right", null, Dock.Right);
						dock.AddOption("Bottom", null, Dock.Bottom);
						dock.AddOption("Fill", null, Dock.Fill);
						dock.SelectByUserData(docking);
						dock.SelectionChanged += DockChanged;
					}

					VerticalLayout vlayout = new VerticalLayout(hlayout);
					{
						HorizontalLayout hlayout2 = new HorizontalLayout(vlayout);
						{
							Control.GroupBox hgb = new Control.GroupBox(hlayout2);
							hgb.Text = "H. Align";
							{
								Control.RadioButtonGroup halign = new RadioButtonGroup(hgb);
								halign.UserData = subject;
								halign.AddOption("Left", null, HorizontalAlignment.Left);
								halign.AddOption("Center", null, HorizontalAlignment.Center);
								halign.AddOption("Right", null, HorizontalAlignment.Right);
								halign.AddOption("Stretch", null, HorizontalAlignment.Stretch);
								halign.SelectByUserData(subject.HorizontalAlignment);
								halign.SelectionChanged += HAlignChanged;
							}

							Control.GroupBox vgb = new Control.GroupBox(hlayout2);
							vgb.Text = "V. Align";
							{
								Control.RadioButtonGroup valign = new RadioButtonGroup(vgb);
								valign.UserData = subject;
								valign.AddOption("Top", null, VerticalAlignment.Top);
								valign.AddOption("Center", null, VerticalAlignment.Center);
								valign.AddOption("Bottom", null, VerticalAlignment.Bottom);
								valign.AddOption("Stretch", null, VerticalAlignment.Stretch);
								valign.SelectByUserData(subject.VerticalAlignment);
								valign.SelectionChanged += VAlignChanged;
							}
						}

						Control.Layout.GridLayout glayout = new Control.Layout.GridLayout(vlayout);
						glayout.SetColumnWidths(Control.Layout.GridLayout.AutoSize, Control.Layout.GridLayout.Fill);
						{
							Control.Label l_width = new Control.Label(glayout);
							l_width.Text = "Width:";

							Control.HorizontalSlider width = new HorizontalSlider(glayout);
							width.Name = "Width";
							width.UserData = subject;
							width.Min = 50;
							width.Max = 350;
							width.Value = 100;
							width.ValueChanged += WidthChanged;

							Control.Label l_height = new Control.Label(glayout);
							l_height.Text = "Height:";

							Control.HorizontalSlider height = new Control.HorizontalSlider(glayout);
							height.Name = "Height";
							height.UserData = subject;
							height.Min = 50;
							height.Max = 350;
							height.Value = 100;
							height.ValueChanged += HeightChanged;

							Control.Label l_margin = new Control.Label(glayout);
							l_margin.Text = "Margin:";

							Control.HorizontalSlider margin = new HorizontalSlider(glayout);
							margin.Name = "Margin";
							margin.UserData = subject;
							margin.Min = 0;
							margin.Max = 50;
							margin.Value = 0;
							margin.ValueChanged += MarginChanged;
						}
					}
				}
			}

			return gb;
		}

		void PaddingChanged(ControlBase control, EventArgs args)
		{
			Control.Internal.Slider val = control as Control.Internal.Slider;
			int i = (int)val.Value;
			outer.Padding = new Padding(i, i, i, i);
		}

		void MarginChanged(ControlBase control, EventArgs args)
		{
			ControlBase inner = control.UserData as ControlBase;
			Control.Internal.Slider val = control as Control.Internal.Slider;
			int i = (int)val.Value;
			inner.Margin = new Margin(i, i, i, i);
		}

		void WidthChanged(ControlBase control, EventArgs args)
		{
			ControlBase inner = control.UserData as ControlBase;
			Control.Internal.Slider val = control as Control.Internal.Slider;
			if (inner.HorizontalAlignment != HorizontalAlignment.Stretch)
				inner.Width = (int)val.Value;
		}

		void HeightChanged(ControlBase control, EventArgs args)
		{
			ControlBase inner = control.UserData as ControlBase;
			Control.Internal.Slider val = control as Control.Internal.Slider;
			if (inner.VerticalAlignment != VerticalAlignment.Stretch)
				inner.Height = (int)val.Value;
		}

		void HAlignChanged(ControlBase control, EventArgs args)
		{
			ControlBase inner = control.UserData as ControlBase;
			RadioButtonGroup rbg = (RadioButtonGroup)control;
			inner.HorizontalAlignment = (HorizontalAlignment)rbg.Selected.UserData;
			if (inner.HorizontalAlignment == HorizontalAlignment.Stretch)
				inner.Width = Util.Ignore;
		}

		void VAlignChanged(ControlBase control, EventArgs args)
		{
			ControlBase inner = control.UserData as ControlBase;
			RadioButtonGroup rbg = (RadioButtonGroup)control;
			inner.VerticalAlignment = (VerticalAlignment)rbg.Selected.UserData;
			if (inner.VerticalAlignment == VerticalAlignment.Stretch)
				inner.Height = Util.Ignore;
		}

		void DockChanged(ControlBase control, EventArgs args)
		{
			ControlBase inner = (ControlBase) control.UserData;
			RadioButtonGroup rbg = (RadioButtonGroup) control;
			ControlBase gb = inner.UserData as ControlBase;
			int w = (int)(gb.FindChildByName("Width", true) as Control.Internal.Slider).Value;
			int h = (int)(gb.FindChildByName("Height", true) as Control.Internal.Slider).Value;
			inner.Dock = (Dock)rbg.Selected.UserData;

			switch (inner.Dock)
			{
				case Dock.Left:
					inner.Size = new Size(w, Util.Ignore);
					break;
				case Dock.Top:
					inner.Size = new Size(Util.Ignore, h);
					break;
				case Dock.Right:
					inner.Size = new Size(w, Util.Ignore);
					break;
				case Dock.Bottom:
					inner.Size = new Size(Util.Ignore, h);
					break;
				case Dock.Fill:
					inner.Size = new Size(Util.Ignore, Util.Ignore);
					break;
			}
		}

		public override void Dispose()
		{
			font.Dispose();
			base.Dispose();
		}
	}
}
