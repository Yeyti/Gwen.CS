using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Containers", Order = 301)]
	public class ToolWindow : GUnit
	{
		public ToolWindow(ControlBase parent)
            : base(parent)
        {
			Gwen.Control.Layout.VerticalLayout layout = new Gwen.Control.Layout.VerticalLayout(this);
			layout.HorizontalAlignment = HorizontalAlignment.Left;

			Control.Button button;

			button = new Control.Button(layout);
			button.Margin = Margin.Five;
			button.Text = "Open a ToolBar";
			button.Clicked += OpenToolBar;

			button = new Control.Button(layout);
			button.Margin = Margin.Five;
			button.Text = "Open a tool window";
			button.Clicked += OpenToolWindow;
		}

		void OpenToolBar(ControlBase control, EventArgs args)
        {
			Control.ToolWindow window = new Control.ToolWindow(this);
			window.Padding = Padding.Five;
			window.HorizontalAlignment = HorizontalAlignment.Left;
			window.VerticalAlignment = VerticalAlignment.Top;
			window.StartPosition = StartPosition.CenterCanvas;

			HorizontalLayout layout = new HorizontalLayout(window);

			for (int i = 0; i < 5; i++)
			{
				Control.Button button = new Control.Button(layout);
				button.Size = new Size(36, 36);
				button.UserData = window;
				button.Clicked += Close;
			}
		}

		void OpenToolWindow(ControlBase control, EventArgs args)
		{
			Control.ToolWindow window = new Control.ToolWindow(this);
			window.Padding = Padding.Five;
			window.HorizontalAlignment = HorizontalAlignment.Left;
			window.VerticalAlignment = VerticalAlignment.Top;
			window.StartPosition = StartPosition.CenterParent;
			window.Vertical = true;

			Control.Layout.GridLayout layout = new Control.Layout.GridLayout(window);
			layout.ColumnCount = 2;

			Control.Button button = new Control.Button(layout);
			button.Size = new Size(100, 40);
			button.UserData = window;
			button.Clicked += Close;

			button = new Control.Button(layout);
			button.Size = new Size(100, 40);
			button.UserData = window;
			button.Clicked += Close;

			button = new Control.Button(layout);
			button.Size = new Size(100, 40);
			button.UserData = window;
			button.Clicked += Close;

			button = new Control.Button(layout);
			button.Size = new Size(100, 40);
			button.UserData = window;
			button.Clicked += Close;
		}

		void Close(ControlBase control, EventArgs args)
		{
			Control.ToolWindow window = control.UserData as Control.ToolWindow;
			window.Close();
			window.Parent.RemoveChild(window, true);
		}
    }
}
