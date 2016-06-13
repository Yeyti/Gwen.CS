using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Layout", Order = 404)]
	public class GridLayout : GUnit
	{
		public GridLayout(ControlBase parent)
			: base(parent)
		{
			Control.Layout.GridLayout grid = CreateGrid(this);
			grid.Dock = Dock.Fill;
		}

		private Control.Layout.GridLayout CreateGrid(ControlBase parent)
		{
			Control.Layout.GridLayout grid = new Control.Layout.GridLayout(parent);

			grid.SetColumnWidths(0.2f, Control.Layout.GridLayout.AutoSize, 140.0f, 0.8f);
			grid.SetRowHeights(0.2f, Control.Layout.GridLayout.AutoSize, 140.0f, 0.8f);

			CreateControl(grid, "C: 20%, R: 20%");
			CreateControl(grid, "C: Auto R: 20%");
			CreateControl(grid, "C: 140, R: 20%");
			CreateControl(grid, "C: 80%, R: 20%");

			CreateControl(grid, "C: 20%, R: Auto");
			CreateControl(grid, "C: Auto R: Auto");
			CreateControl(grid, "C: 140, R: Auto");
			CreateControl(grid, "C: 80%, R: Auto");

			CreateControl(grid, "C: 20%, R: 140");
			CreateControl(grid, "C: Auto R: 140");
			CreateControl(grid, "C: 140, R: 140");
			CreateControl(grid, "C: 80%, R: 140");

			CreateControl(grid, "C: 20%, R: 80%");
			CreateControl(grid, "C: Auto R: 80%");
			CreateControl(grid, "C: 140, R: 80%");
			CreateControl(grid, "C: 80%, R: 80%");

			return grid;
		}

		private void CreateControl(ControlBase parent, string text)
		{
			Control.Button button = new Control.Button(parent);
			button.Text = text;
		}
	}
}
