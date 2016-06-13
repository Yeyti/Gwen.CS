using System;
using Gwen.Control;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Containers", Order = 300)]
	public class Window : GUnit
    {
        private int m_WindowCount;
        private readonly Random m_Rand;

        public Window(ControlBase parent)
            : base(parent)
        {
			m_Rand = new Random();

			Gwen.Control.Layout.VerticalLayout layout = new Gwen.Control.Layout.VerticalLayout(this);
			layout.HorizontalAlignment = HorizontalAlignment.Left;

			Control.Button button;

            button = new Control.Button(layout);
			button.Margin = Margin.Five;
			button.Text = "Open a Window";
            button.Clicked += OpenWindow;

			button = new Control.Button(layout);
			button.Margin = Margin.Five;
			button.Text = "Open a Window (with menu)";
			button.Clicked += OpenWindowWithMenuAndStatusBar;

			button = new Control.Button(layout);
			button.Margin = Margin.Five;
			button.Text = "Open a Window (auto size)";
			button.Clicked += OpenWindowAutoSizing;

			button = new Control.Button(layout);
			button.Margin = Margin.Five;
			button.Text = "Open a Window (modal)";
			button.Clicked += OpenWindowModal;

			button = new Control.Button(layout);
			button.Margin = Margin.Five;
			button.Text = "Open a MessageBox";
            button.Clicked += OpenMsgbox;

			button = new Control.Button(layout);
			button.Margin = Margin.Five;
			button.Text = "Open a Long MessageBox";
			button.Clicked += OpenLongMsgbox;

			m_WindowCount = 0;
        }

		private void OpenWindow(ControlBase control, EventArgs args)
        {
            Control.Window window = new Control.Window(this);
            window.Title = String.Format("Window ({0})", ++m_WindowCount);
			window.Size = new Size(m_Rand.Next(200, 400), m_Rand.Next(200, 400));
			window.Left = m_Rand.Next(700);
			window.Top = m_Rand.Next(400);
			window.Padding = new Padding(6, 3, 6, 6);

			Control.RadioButtonGroup rbg = new RadioButtonGroup(window);
			rbg.Dock = Dock.Top;
			rbg.AddOption("Resize disabled", "None").Checked += (c, a) => window.Resizing = Resizing.None;
			rbg.AddOption("Resize width", "Width").Checked += (c, a) => window.Resizing = Resizing.Width;
			rbg.AddOption("Resize height", "Height").Checked += (c, a) => window.Resizing = Resizing.Height;
			rbg.AddOption("Resize both", "Both").Checked += (c, a) => window.Resizing = Resizing.Both;
			rbg.SetSelectionByName("Both");

			Control.LabeledCheckBox dragging = new Control.LabeledCheckBox(window);
			dragging.Dock = Dock.Top;
			dragging.Text = "Dragging";
			dragging.IsChecked = true;
			dragging.CheckChanged += (c, a) => window.IsDraggingEnabled = dragging.IsChecked;
        }

		private void OpenWindowWithMenuAndStatusBar(ControlBase control, EventArgs args)
		{
			Control.Window window = new Control.Window(this);
			window.Title = String.Format("Window ({0})", ++m_WindowCount);
			window.Size = new Size(m_Rand.Next(200, 400), m_Rand.Next(200, 400));
			window.Left = m_Rand.Next(700);
			window.Top = m_Rand.Next(400);
			window.Padding = new Padding(1, 0, 1, 1);

			Control.Layout.DockLayout layout = new Control.Layout.DockLayout(window);

			Control.MenuStrip menuStrip = new Control.MenuStrip(layout);
			menuStrip.Dock = Dock.Top;

			/* File */
			{
				Control.MenuItem root = menuStrip.AddItem("File");
				root.Menu.AddItem("Load", "test16.png", "Ctrl+L");
				root.Menu.AddItem("Save", String.Empty, "Ctrl+S");
				root.Menu.AddItem("Save As..", String.Empty, "Ctrl+A");
				root.Menu.AddItem("Quit", String.Empty, "Ctrl+Q").SetAction((c ,a) => window.Close());
			}
			/* Resizing */
			{
				Control.MenuItem root = menuStrip.AddItem("Resizing");
				root.Menu.AddItem("Disabled").SetAction((c, a) => window.Resizing = Resizing.None);
				root.Menu.AddItem("Width").SetAction((c, a) => window.Resizing = Resizing.Width);
				root.Menu.AddItem("Height").SetAction((c, a) => window.Resizing = Resizing.Height);
				root.Menu.AddItem("Both").SetAction((c, a) => window.Resizing = Resizing.Both);
			}

			Control.StatusBar statusBar = new Control.StatusBar(layout);
			statusBar.Dock = Dock.Bottom;
			statusBar.Text = "Status bar";

			{
				Control.Button br = new Control.Button(statusBar);
				br.Text = "Right button";
				statusBar.AddControl(br, true);
			}
		}

		private void OpenWindowAutoSizing(ControlBase control, EventArgs args)
		{
			Control.Window window = new Control.Window(this);
			window.Title = String.Format("Window ({0})", ++m_WindowCount);
			window.Left = m_Rand.Next(700);
			window.Top = m_Rand.Next(400);
			window.Padding = new Padding(6, 3, 6, 6);
			window.HorizontalAlignment = HorizontalAlignment.Left;
			window.VerticalAlignment = VerticalAlignment.Top;
			window.Resizing = Resizing.None;

			VerticalLayout layout = new VerticalLayout(window);

			Control.GroupBox grb = new Control.GroupBox(layout);
			grb.Text = "Auto size";
			layout = new VerticalLayout(grb);

			{
				Control.Label label = new Control.Label(layout);
				label.Margin = Margin.Six;
				label.Text = "Label text";

				Control.Button button = new Control.Button(layout);
				button.Margin = Margin.Six;
				button.Text = "Click Me";
				button.Width = 200;

				label = new Control.Label(layout);
				label.Margin = Margin.Six;
				label.Text = "Hide / Show Label";
				//label.IsCollapsed = true;

				button.Clicked += (s, a) => label.IsCollapsed = !label.IsCollapsed;
			}
		}

		private void OpenWindowModal(ControlBase control, EventArgs args)
		{
			Control.Window window = new Control.Window(this);
			window.Title = String.Format("Modal Window ({0})", ++m_WindowCount);
			window.Left = m_Rand.Next(700);
			window.Top = m_Rand.Next(400);
			window.Padding = new Padding(6, 3, 6, 6);
			window.HorizontalAlignment = HorizontalAlignment.Left;
			window.VerticalAlignment = VerticalAlignment.Top;
			window.Resizing = Resizing.None;
			window.MakeModal(true);

			VerticalLayout layout = new VerticalLayout(window);

			Control.GroupBox grb = new Control.GroupBox(layout);
			grb.Text = "Auto size";
			layout = new VerticalLayout(grb);

			{
				Control.Label label = new Control.Label(layout);
				label.Margin = Margin.Six;
				label.Text = "Label text";

				Control.Button button = new Control.Button(layout);
				button.Margin = Margin.Six;
				button.Text = "Button";
				button.Width = 200;
			}
		}

		private void OpenMsgbox(ControlBase control, EventArgs args)
        {
            MessageBox window = new MessageBox(this, "Message box test text.");
			window.Dismissed += OnDismissed;
			window.SetPosition(m_Rand.Next(700), m_Rand.Next(400));
        }

		private void OpenLongMsgbox(ControlBase control, EventArgs args)
		{
			MessageBox window = new MessageBox(this, @"In olden times when wishing still helped one, there lived a king whose daughters were all beautiful, but the youngest was so beautiful that the sun itself, which has seen so much, was astonished whenever it shone in her face. Close by the king's castle lay a great dark forest, and under an old lime-tree in the forest was a well, and when the day was very warm, the king's child went out into the forest and sat down by the side of the cool fountain, and when she was bored she took a golden ball, and threw it up on high and caught it, and this ball was her favorite plaything.", "Long Text", MessageBoxButtons.AbortRetryIgnore);
			window.Dismissed += OnDismissed;
			window.SetPosition(m_Rand.Next(700), m_Rand.Next(400));
		}

		private void OnDismissed(ControlBase sender, MessageBoxResultEventArgs args)
		{
			UnitTest.PrintText("Message box result: " + args.Result);
		}
	}
}
