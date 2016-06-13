using System;
using Gwen.Control;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Non-standard", Order = 500)]
	public class CollapsibleList : GUnit
    {
        public CollapsibleList(ControlBase parent)
            : base(parent)
        {
            Control.CollapsibleList control = new Control.CollapsibleList(this);
			control.Dock = Dock.Fill;
			control.HorizontalAlignment = HorizontalAlignment.Left;
            control.ItemSelected += OnSelection;
            control.CategoryCollapsed += OnCollapsed;

            {
                Control.CollapsibleCategory cat = control.Add("Category One");
                cat.Add("Hello");
                cat.Add("Two");
                cat.Add("Three");
                cat.Add("Four");
            }

            {
                Control.CollapsibleCategory cat = control.Add("Shopping");
                cat.Add("Special");
                cat.Add("Two Noses");
                cat.Add("Orange ears");
                cat.Add("Beer");
                cat.Add("Three Eyes");
                cat.Add("Special");
                cat.Add("Two Noses");
                cat.Add("Orange ears");
                cat.Add("Beer");
                cat.Add("Three Eyes");
                cat.Add("Special");
                cat.Add("Two Noses");
                cat.Add("Orange ears");
                cat.Add("Beer");
                cat.Add("Three Eyes");
            }

            {
                Control.CollapsibleCategory cat = control.Add("Category One");
                cat.Add("Hello");
                cat.Add("Two");
                cat.Add("Three");
                cat.Add("Four");
            }
        }

		void OnSelection(ControlBase control, EventArgs args)
        {
            Control.CollapsibleList list = control as Control.CollapsibleList;
            UnitPrint(String.Format("CollapsibleList: Selected: {0}", list.GetSelectedButton().Text));
        }

		void OnCollapsed(ControlBase control, EventArgs args)
        {
            Control.CollapsibleCategory cat = control as Control.CollapsibleCategory;
            UnitPrint(String.Format("CollapsibleCategory: CategoryCollapsed: {0} {1}", cat.Text, cat.IsCollapsed));
        }
    }
}
