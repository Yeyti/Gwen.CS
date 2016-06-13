using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Containers", Order = 305)]
	public class ScrollControl : GUnit
	{
		public ScrollControl(ControlBase parent)
			: base(parent)
		{
			Control.Layout.GridLayout layout = new Control.Layout.GridLayout(this);
			layout.ColumnCount = 6;

			Control.Button pTestButton;

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Twice As Big";
				pTestButton.Size = new Size(200, 200);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Same Size";
				pTestButton.Size = new Size(100, 100);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Wide";
				pTestButton.Size = new Size(200, 50);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Tall";
				pTestButton.Size = new Size(50, 200);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);
				ctrl.EnableScroll(false, true);

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Vertical";
				pTestButton.Size = new Size(200, 200);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);
				ctrl.EnableScroll(true, false);

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Horizontal";
				pTestButton.Size = new Size(200, 200);
			}

			// Bottom Row

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);
				ctrl.AutoHideBars = true;

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Twice As Big";
				pTestButton.Size = new Size(200, 200);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);
				ctrl.AutoHideBars = true;

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Same Size";
				pTestButton.Size = new Size(100, 100);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);
				ctrl.AutoHideBars = true;

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Wide";
				pTestButton.Size = new Size(200, 50);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);
				ctrl.AutoHideBars = true;

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Tall";
				pTestButton.Size = new Size(50, 200);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);
				ctrl.AutoHideBars = true;
				ctrl.EnableScroll(false, true);

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Vertical";
				pTestButton.Size = new Size(200, 200);
			}

			{
				Control.ScrollControl ctrl = new Control.ScrollControl(layout);
				ctrl.Margin = Margin.Three;
				ctrl.Size = new Size(100, 100);
				ctrl.AutoHideBars = true;
				ctrl.EnableScroll(true, false);

				pTestButton = new Control.Button(ctrl);
				pTestButton.Text = "Horinzontal";
				pTestButton.Size = new Size(200, 200);
			}
		}
	}
}
