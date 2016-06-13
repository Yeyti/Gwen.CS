using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Standard", Order = 207)]
	public class Slider : GUnit
    {
        public Slider(ControlBase parent)
            : base(parent)
        {
			HorizontalLayout hlayout = new HorizontalLayout(this);

			VerticalLayout vlayout = new VerticalLayout(hlayout);

            {
                Control.HorizontalSlider slider = new Control.HorizontalSlider(vlayout);
				slider.Margin = Margin.Ten;
				slider.Width = 150;
                slider.SetRange(0, 100);
                slider.Value = 25;
                slider.ValueChanged += SliderMoved;
            }

            {
                Control.HorizontalSlider slider = new Control.HorizontalSlider(vlayout);
				slider.Margin = Margin.Ten;
				slider.Width = 150;
                slider.SetRange(0, 100);
                slider.Value = 20;
                slider.NotchCount = 10;
                slider.SnapToNotches = true;
                slider.ValueChanged += SliderMoved;
            }

            {
                Control.VerticalSlider slider = new Control.VerticalSlider(hlayout);
				slider.Margin = Margin.Ten;
				slider.Height = 200;
                slider.SetRange(0, 100);
                slider.Value = 25;
                slider.ValueChanged += SliderMoved;
            }

            {
                Control.VerticalSlider slider = new Control.VerticalSlider(hlayout);
				slider.Margin = Margin.Ten;
				slider.Height = 200;
                slider.SetRange(0, 100);
                slider.Value = 20;
                slider.NotchCount = 10;
                slider.SnapToNotches = true;
                slider.ValueChanged += SliderMoved;
            }
        }

		void SliderMoved(ControlBase control, EventArgs args)
        {
            Control.Internal.Slider slider = control as Control.Internal.Slider;
            UnitPrint(String.Format("Slider moved: ValueChanged: {0}", slider.Value));
        }
    }
}
