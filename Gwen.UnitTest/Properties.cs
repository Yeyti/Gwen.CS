using System;
using Gwen.Control;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Containers", Order = 303)]
	public class Properties : GUnit
    {
        public Properties(ControlBase parent)
            : base(parent)
        {
            {
                Control.Properties props = new Control.Properties(this);
				props.Dock = Dock.Top;
				props.Width = 300;
                props.ValueChanged += OnChanged;

                {
					{
						Control.PropertyRow pRow = props.Add("First Name");
                    }

                    props.Add("Middle Name");
                    props.Add("Last Name");
				}
			}

			{
                Control.PropertyTree ptree = new Control.PropertyTree(this);
				ptree.Dock = Dock.Top;
				ptree.Width = 300;
				ptree.AutoSizeToContent = true;

				{
					Control.Properties props = ptree.Add("Item One");
                    props.ValueChanged += OnChanged;

                    props.Add("Middle Name");
                    props.Add("Last Name");
                    props.Add("Four");
                }

                {
                    Control.Properties props = ptree.Add("Item Two");
                    props.ValueChanged += OnChanged;
                    
                    props.Add("More Items");
                    props.Add("Bacon", new Control.Property.Check(props), "1");
                    props.Add("To Fill");
                    props.Add("Color", new Control.Property.Color(props), "255 0 0");
                    props.Add("Out Here");
                }

                ptree.ExpandAll();
            }
        }

        void OnChanged(ControlBase control, EventArgs args)
        {
            PropertyRow row = control as PropertyRow;
            UnitPrint(String.Format("Property changed: {0}", row.Value));
        }
    }
}
