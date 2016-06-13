using System;
using System.Linq;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Standard", Order = 205)]
	public class ListBox : GUnit
    {
        public ListBox(ControlBase parent)
            : base(parent)
        {
			HorizontalLayout hlayout = new HorizontalLayout(this);
			hlayout.Dock = Dock.Top;

			{
                Control.ListBox ctrl = new Control.ListBox(hlayout);
				ctrl.AutoSizeToContent = true;
				ctrl.AllowMultiSelect = true;

				ctrl.AddRow("First");
                ctrl.AddRow("Blue");
                ctrl.AddRow("Yellow");
                ctrl.AddRow("Orange");
                ctrl.AddRow("Brown");
                ctrl.AddRow("Black");
                ctrl.AddRow("Green");
                ctrl.AddRow("Dog");
                ctrl.AddRow("Cat Blue");
                ctrl.AddRow("Shoes");
                ctrl.AddRow("Shirts");
                ctrl.AddRow("Chair");
                ctrl.AddRow("I'm autosized");
                ctrl.AddRow("Last");

                ctrl.SelectRowsByRegex("Bl.e|Dog");

                ctrl.RowSelected += RowSelected;
                ctrl.RowUnselected += RowUnSelected;
            }

            {
                Table ctrl = new Table(hlayout);

				ctrl.AddRow("First");
                ctrl.AddRow("Blue");
                ctrl.AddRow("Yellow");
                ctrl.AddRow("Orange");
                ctrl.AddRow("Brown");
                ctrl.AddRow("Black");
                ctrl.AddRow("Green");
                ctrl.AddRow("Dog");
                ctrl.AddRow("Cat Blue");
                ctrl.AddRow("Shoes");
                ctrl.AddRow("Shirts");
                ctrl.AddRow("Chair");
                ctrl.AddRow("I'm autosized");
                ctrl.AddRow("Last");

                ctrl.SizeToContent();
            }

            {
                Control.ListBox ctrl = new Control.ListBox(hlayout);
				ctrl.AutoSizeToContent = true;
				ctrl.ColumnCount = 3;
                ctrl.RowSelected += RowSelected;
                ctrl.RowUnselected += RowUnSelected;

                {
                    TableRow row = ctrl.AddRow("Baked Beans");
                    row.SetCellText(1, "Heinz");
                    row.SetCellText(2, "£3.50");
                }

                {
                    TableRow row = ctrl.AddRow("Bananas");
                    row.SetCellText(1, "Trees");
                    row.SetCellText(2, "£1.27");
                }

                {
                    TableRow row = ctrl.AddRow("Chicken");
                    row.SetCellText(1, "\u5355\u5143\u6D4B\u8BD5");
                    row.SetCellText(2, "£8.95");
                }
			}

			VerticalLayout vlayout = new VerticalLayout(hlayout);

            {
                // fixed-size list box
                Control.ListBox ctrl = new Control.ListBox(vlayout);
				ctrl.AutoSizeToContent = true;
				ctrl.HorizontalAlignment = HorizontalAlignment.Left;
                ctrl.ColumnCount = 3;

                ctrl.SetColumnWidth(0, 150);
                ctrl.SetColumnWidth(1, 150);
                ctrl.SetColumnWidth(2, 150);

				var row1 = ctrl.AddRow("Row 1");
                row1.SetCellText(1, "R1 cell 1");
                row1.SetCellText(2, "Row 1 cell 2");

                ctrl.AddRow("Row 2, slightly bigger");
                ctrl[1].SetCellText(1, "Center cell");

                ctrl.AddRow("Row 3, medium");
                ctrl[2].SetCellText(2, "Last cell");
			}

            {
                // autosized list box
                Control.ListBox ctrl = new Control.ListBox(vlayout);
				ctrl.AutoSizeToContent = true;
				ctrl.HorizontalAlignment = HorizontalAlignment.Left;
				ctrl.ColumnCount = 3;

                var row1 = ctrl.AddRow("Row 1");
                row1.SetCellText(1, "R1 cell 1");
                row1.SetCellText(2, "Row 1 cell 2");

                ctrl.AddRow("Row 2, slightly bigger");
                ctrl[1].SetCellText(1, "Center cell");

                ctrl.AddRow("Row 3, medium");
                ctrl[2].SetCellText(2, "Last cell");
			}

			hlayout = new HorizontalLayout(this);
			hlayout.Dock = Dock.Top;

			/* Selecting Rows in Code */
			{
				Control.ListBox ctrl = new Control.ListBox(hlayout);
				ctrl.AutoSizeToContent = true;

				ListBoxRow Row = ctrl.AddRow("Row");
                ctrl.AddRow("Text");
                ctrl.AddRow("InternalName", "Name");
                ctrl.AddRow("UserData", "Internal", 12);

                Control.LabeledCheckBox multiline = new Control.LabeledCheckBox(this);
				multiline.Dock = Dock.Top;
				multiline.Text = "Enable MultiSelect";
				multiline.CheckChanged += delegate(ControlBase sender, EventArgs args)
                {
                    ctrl.AllowMultiSelect = multiline.IsChecked;
                };

				vlayout = new VerticalLayout(hlayout);
                //Select by Menu Item
                {
                    Control.Button TriangleButton = new Control.Button(vlayout);
                    TriangleButton.Text = "Row";
                    TriangleButton.Width = 100;
                    TriangleButton.Clicked += delegate(ControlBase sender, ClickedEventArgs args)
                    {
                        ctrl.SelectedRow = Row;
                    };
                }

                //Select by Text
                {
                    Control.Button TestBtn = new Control.Button(vlayout);
                    TestBtn.Text = "Text";
                    TestBtn.Width = 100;
                    TestBtn.Clicked += delegate(ControlBase sender, ClickedEventArgs args)
                    {
                        ctrl.SelectByText("Text");
                    };
                }

                //Select by Name
                {
                    Control.Button TestBtn = new Control.Button(vlayout);
                    TestBtn.Text = "Name";
                    TestBtn.Width = 100;
                    TestBtn.Clicked += delegate(ControlBase sender, ClickedEventArgs args)
                    {
                        ctrl.SelectByName("Name");
                    };
                }

                //Select by UserData
                {
                    Control.Button TestBtn = new Control.Button(vlayout);
                    TestBtn.Text = "UserData";
                    TestBtn.Width = 100;
                    TestBtn.Clicked += delegate(ControlBase sender, ClickedEventArgs args)
                    {
                        ctrl.SelectByUserData(12);
                    };
                }
            }
        }

		void RowSelected(ControlBase control, EventArgs args)
        {
            Control.ListBox list = control as Control.ListBox;
            UnitPrint(String.Format("ListBox: RowSelected: {0} [{1}]", list.SelectedRows.Last().Text, list[list.SelectedRowIndex].Text));
        }

		void RowUnSelected(ControlBase control, EventArgs args)
        {
            // todo: how to determine which one was unselected (store somewhere)
            // or pass row as the event param?
            Control.ListBox list = control as Control.ListBox;
            UnitPrint(String.Format("ListBox: OnRowUnselected"));
        }
    }
}
