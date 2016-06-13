using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Standard", Order = 203)]
	public class RadioButton : GUnit
    {
        enum Choices
        {
            OptionA,
            DoorB,
            HallC
        }

        public RadioButton(ControlBase parent)
            : base(parent)
        {
			VerticalLayout layout = new VerticalLayout(this);

			Control.GroupBox group = new Control.GroupBox(layout);
			group.Margin = Margin.Five;
			group.Text = "Sample radio group";
			{
				Control.RadioButtonGroup rbg = new Control.RadioButtonGroup(group);

				rbg.AddOption("Option 1");
				rbg.AddOption("Option 2");
				rbg.AddOption("Option 3");
				rbg.AddOption("\u0627\u0644\u0622\u0646 \u0644\u062D\u0636\u0648\u0631");

				rbg.SelectionChanged += OnChange;
			}

			{
				EnumRadioButtonGroup<Choices> erbg = new EnumRadioButtonGroup<Choices>(layout);
				erbg.Margin = Margin.Five;
				erbg.SelectedValue = Choices.HallC;
			}
        }

        void OnChange(ControlBase control, EventArgs args)
        {
            RadioButtonGroup rbc = control as RadioButtonGroup;
            LabeledRadioButton rb = rbc.Selected;
            UnitPrint(String.Format("RadioButton: SelectionChanged: {0}", rb.Text));
        }
    }
}
