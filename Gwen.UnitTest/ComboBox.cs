using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Standard", Order = 204)]
	public class ComboBox : GUnit
	{
		public ComboBox(ControlBase parent)
			: base(parent)
		{
			VerticalLayout layout = new VerticalLayout(this);

			{
				Control.ComboBox combo = new Control.ComboBox(layout);
				combo.Margin = Margin.Five;
				combo.Width = 200;

				combo.AddItem("Option One", "one");
				combo.AddItem("Number Two", "two");
				combo.AddItem("Door Three", "three");
				combo.AddItem("Four Legs", "four");
				combo.AddItem("Five Birds", "five");

				combo.ItemSelected += OnComboSelect;
			}

			{
				// Empty
				Control.ComboBox combo = new Control.ComboBox(layout);
				combo.Margin = Margin.Five;
				combo.Width = 200;
			}

			{
				// Lots of things
				Control.ComboBox combo = new Control.ComboBox(layout);
				combo.Margin = Margin.Five;
				combo.Width = 200;

				for (int i = 0; i < 500; i++)
					combo.AddItem(String.Format("Option {0}", i));

				combo.ItemSelected += OnComboSelect;
			}

			{
				// Editable
				Control.EditableComboBox combo = new EditableComboBox(layout);
				combo.Margin = Margin.Five;
				combo.Width = 200;

				combo.AddItem("Option One", "one");
				combo.AddItem("Number Two", "two");
				combo.AddItem("Door Three", "three");
				combo.AddItem("Four Legs", "four");
				combo.AddItem("Five Birds", "five");

				combo.ItemSelected += (s, a) => UnitPrint(String.Format("ComboBox: OnComboSelect: {0}", combo.SelectedItem.Text)); ;

				combo.TextChanged += (s, a) => UnitPrint(String.Format("ComboBox: OnTextChanged: {0}", combo.Text));
				combo.SubmitPressed += (s, a) => UnitPrint(String.Format("ComboBox: OnSubmitPressed: {0}", combo.Text));
			}

			{
				HorizontalLayout hlayout = new HorizontalLayout(layout);
				{
					// In-Code Item Change
					Control.ComboBox combo = new Control.ComboBox(hlayout);
					combo.Margin = Margin.Five;
					combo.Width = 200;

					MenuItem Triangle = combo.AddItem("Triangle");
					combo.AddItem("Red", "color");
					combo.AddItem("Apple", "fruit");
					combo.AddItem("Blue", "color");
					combo.AddItem("Green", "color", 12);
					combo.ItemSelected += OnComboSelect;

					//Select by Menu Item
					{
						Control.Button TriangleButton = new Control.Button(hlayout);
						TriangleButton.Text = "Triangle";
						TriangleButton.Width = 100;
						TriangleButton.Clicked += delegate (ControlBase sender, ClickedEventArgs args)
						{
							combo.SelectedItem = Triangle;
						};
					}

					//Select by Text
					{
						Control.Button TestBtn = new Control.Button(hlayout);
						TestBtn.Text = "Red";
						TestBtn.Width = 100;
						TestBtn.Clicked += delegate (ControlBase sender, ClickedEventArgs args)
						{
							combo.SelectByText("Red");
						};
					}

					//Select by Name
					{
						Control.Button TestBtn = new Control.Button(hlayout);
						TestBtn.Text = "Apple";
						TestBtn.Width = 100;
						TestBtn.Clicked += delegate (ControlBase sender, ClickedEventArgs args)
						{
							combo.SelectByName("fruit");
						};
					}

					//Select by UserData
					{
						Control.Button TestBtn = new Control.Button(hlayout);
						TestBtn.Text = "Green";
						TestBtn.Width = 100;
						TestBtn.Clicked += delegate (ControlBase sender, ClickedEventArgs args)
						{
							combo.SelectByUserData(12);
						};
					}
				}
			}
		}

		void OnComboSelect(ControlBase control, EventArgs args)
		{
			Control.ComboBox combo = control as Control.ComboBox;
			UnitPrint(String.Format("ComboBox: OnComboSelect: {0}", combo.SelectedItem.Text));
		}
	}
}
