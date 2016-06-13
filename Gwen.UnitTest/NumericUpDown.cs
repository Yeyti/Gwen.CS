using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Standard", Order = 206)]
	public class NumericUpDown : GUnit
    {
        public NumericUpDown(ControlBase parent)
            : base(parent)
        {
			VerticalLayout layout = new VerticalLayout(this);

            Control.NumericUpDown ctrl = new Control.NumericUpDown(layout);
			ctrl.Margin = Margin.Five;
			ctrl.Width = 70;
			ctrl.Value = 50;
            ctrl.Max = 100;
            ctrl.Min = -100;
            ctrl.ValueChanged += OnValueChanged;

			ctrl = new Control.NumericUpDown(layout);
			ctrl.Margin = Margin.Five;
			ctrl.Width = 70;
			ctrl.Value = 50;
			ctrl.Max = 100;
			ctrl.Min = -100;
			ctrl.Step = 5;
			ctrl.ValueChanged += OnValueChanged;

			ctrl = new Control.NumericUpDown(layout);
			ctrl.Margin = Margin.Five;
			ctrl.Width = 70;
			ctrl.Value = 50;
			ctrl.Max = 100;
			ctrl.Min = -100;
			ctrl.Step = 0.1f;
			ctrl.ValueChanged += OnValueChanged;

			ctrl = new Control.NumericUpDown(layout);
			ctrl.Margin = Margin.Five;
			ctrl.Width = 70;
			ctrl.Max = Single.MaxValue;
			ctrl.Min = 0;
			ctrl.Step = 1f;
			ctrl.ValueChanged += OnValueChanged;
		}

		void OnValueChanged(ControlBase control, EventArgs args)
        {
            UnitPrint(String.Format("NumericUpDown: ValueChanged: {0}", ((Control.NumericUpDown)control).Value));
        }
    }
}
