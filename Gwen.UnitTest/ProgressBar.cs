using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Non-Interactive", Order = 104)]
	public class ProgressBar : GUnit
    {
        public ProgressBar(ControlBase parent) : base(parent)
        {
			HorizontalLayout hlayout = new HorizontalLayout(this);
			hlayout.VerticalAlignment = VerticalAlignment.Top;
			{
				VerticalLayout vlayout = new VerticalLayout(hlayout);
				vlayout.Width = 200;
				{
					{
						Control.ProgressBar pb = new Control.ProgressBar(vlayout);
						pb.Margin = Margin.Five;
						pb.Value = 0.03f;
					}

					{
						Control.ProgressBar pb = new Control.ProgressBar(vlayout);
						pb.Margin = Margin.Five;
						pb.Value = 0.66f;
						pb.Alignment = Alignment.Right | Alignment.CenterV;
					}

					{
						Control.ProgressBar pb = new Control.ProgressBar(vlayout);
						pb.Margin = Margin.Five;
						pb.Value = 0.88f;
						pb.Alignment = Alignment.Left | Alignment.CenterV;
					}

					{
						Control.ProgressBar pb = new Control.ProgressBar(vlayout);
						pb.Margin = Margin.Five;
						pb.AutoLabel = false;
						pb.Value = 0.20f;
						pb.Alignment = Alignment.Right | Alignment.CenterV;
						pb.Text = "40,245 MB";
					}

					{
						Control.ProgressBar pb = new Control.ProgressBar(vlayout);
						pb.Margin = Margin.Five;
						pb.AutoLabel = false;
						pb.Value = 1.00f;
					}

					{
						Control.ProgressBar pb = new Control.ProgressBar(vlayout);
						pb.Margin = Margin.Five;
						pb.AutoLabel = false;
						pb.Value = 0.00f;
					}

					{
						Control.ProgressBar pb = new Control.ProgressBar(vlayout);
						pb.Margin = Margin.Five;
						pb.AutoLabel = false;
						pb.Value = 0.50f;
					}
				}
			}

            {
                Control.ProgressBar pb = new Control.ProgressBar(hlayout);
				pb.Margin = Margin.Five;
				pb.IsHorizontal = false;
                pb.Value = 0.25f;
                pb.Alignment = Alignment.Top | Alignment.CenterH;
            }

            {
                Control.ProgressBar pb = new Control.ProgressBar(hlayout);
				pb.Margin = Margin.Five;
				pb.IsHorizontal = false;
                pb.Value = 0.40f;
            }

            {
                Control.ProgressBar pb = new Control.ProgressBar(hlayout);
				pb.Margin = Margin.Five;
				pb.IsHorizontal = false;
                pb.Alignment = Alignment.Bottom | Alignment.CenterH;
                pb.Value = 0.65f;
            }
        }
    }
}
