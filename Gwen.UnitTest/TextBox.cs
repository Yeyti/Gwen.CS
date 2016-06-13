using System;
using System.IO;
using Gwen.Control;
using Gwen.Control.Layout;
using Gwen.CommonDialog;
using Gwen.Xml;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Standard", Order = 201)]
	public class TextBox : GUnit
    {
        private readonly Font m_Font1;
        private readonly Font m_Font2;
        private readonly Font m_Font3;

        public TextBox(ControlBase parent)
            : base(parent)
		{
			m_Font1 = Skin.DefaultFont.Copy();
			m_Font1.FaceName = "Courier New"; // fixed width font!

			m_Font2 = Skin.DefaultFont.Copy();
			m_Font2.FaceName = "Times New Roman";
			m_Font2.Size *= 3;

			m_Font3 = Skin.DefaultFont.Copy();
			m_Font3.Size += 5;

			VerticalLayout vlayout = new VerticalLayout(this);
			{
				DockLayout dockLayout = new DockLayout(vlayout);
				{
					VerticalLayout vlayout2 = new VerticalLayout(dockLayout);
					vlayout2.Dock = Dock.Left;
					vlayout2.Width = 200;
					{
						/* Vanilla Textbox */
						{
							Control.TextBox textbox = new Control.TextBox(vlayout2);
							textbox.Margin = Margin.Five;
							textbox.SetText("Type something here");
							textbox.TextChanged += OnEdit;
							textbox.SubmitPressed += OnSubmit;
						}

						{
							Control.TextBoxPassword textbox = new Control.TextBoxPassword(vlayout2);
							textbox.Margin = Margin.Five;
							//textbox.MaskCharacter = '@';
							textbox.SetText("secret");
							textbox.TextChanged += OnEdit;
						}

						{
							Control.TextBox textbox = new Control.TextBox(vlayout2);
							textbox.Margin = Margin.Five;
							textbox.SetText("Select All Text On Focus");
							textbox.SelectAllOnFocus = true;
						}

						{
							Control.TextBox textbox = new Control.TextBox(vlayout2);
							textbox.Margin = Margin.Five;
							textbox.SetText("Different Coloured Text, for some reason");
							textbox.TextColor = Color.Green;
						}

						{
							Control.TextBox textbox = new Control.TextBoxNumeric(vlayout2);
							textbox.Margin = Margin.Five;
							textbox.SetText("200456698");
							textbox.TextColor = Color.Red;
						}
					}

					/* Multiline Textbox */
					{
						Control.MultilineTextBox textbox = new Control.MultilineTextBox(dockLayout);
						textbox.Dock = Dock.Fill;
						textbox.Margin = Margin.Five;
						textbox.Font = m_Font1;
						textbox.AcceptTabs = true;
						textbox.SetText("In olden times when wishing still helped one, there lived a king whose daughters were all beautiful,\nbut the youngest was so beautiful that the sun itself, which has seen so much, \nwas astonished whenever it shone in her face. \nClose by the king's castle lay a great dark forest, \nand under an old lime-tree in the forest was a well, and when the day was very warm, \nthe king's child went out into the forest and sat down by the side of the cool fountain, \nand when she was bored she took a golden ball, and threw it up on high and caught it, \nand this ball was her favorite plaything.");
					}
					{
						Control.Button pad = new Control.Button(dockLayout);
						pad.Dock = Dock.Right;
						pad.Margin = Margin.Five;
						pad.Text = "Pad";
						pad.Clicked += (s, a) => new TextPad(this);
					}
				}

				{
					Control.TextBox textbox = new Control.TextBox(vlayout);
					textbox.Margin = Margin.Five;
					textbox.SetText("In olden times when wishing still helped one, there lived a king whose daughters were all beautiful, but the youngest was so beautiful that the sun itself, which has seen so much, was astonished whenever it shone in her face. Close by the king's castle lay a great dark forest, and under an old lime-tree in the forest was a well, and when the day was very warm, the king's child went out into the forest and sat down by the side of the cool fountain, and when she was bored she took a golden ball, and threw it up on high and caught it, and this ball was her favorite plaything.");
					textbox.TextColor = Color.Black;
					textbox.Font = m_Font3;
				}

				{
					Control.TextBox textbox = new Control.TextBox(vlayout);
					textbox.Margin = Margin.Five;
					textbox.Width = 150;
					textbox.HorizontalAlignment = HorizontalAlignment.Right;
					textbox.SetText("あおい　うみから　やってきた");
					textbox.TextColor = Color.Black;
					textbox.Font = m_Font3;
				}

				{
					Control.TextBox textbox = new Control.TextBox(vlayout);
					textbox.Margin = Margin.Five;
					textbox.HorizontalAlignment = HorizontalAlignment.Left;
					textbox.FitToText = "Fit the text";
					textbox.SetText("FitToText");
					textbox.TextColor = Color.Black;
					textbox.Font = m_Font3;
				}

				{
					Control.TextBox textbox = new Control.TextBox(vlayout);
					textbox.Margin = Margin.Five;
					textbox.HorizontalAlignment = HorizontalAlignment.Left;
					textbox.Width = 200;
					textbox.SetText("Width = 200");
					textbox.TextColor = Color.Black;
					textbox.Font = m_Font3;
				}

				{
					Control.TextBox textbox = new Control.TextBox(vlayout);
					textbox.Margin = Margin.Five;
					textbox.SetText("Different Font");
					textbox.Font = m_Font2;
				}
			}
		}

        public override void Dispose()
        {
            m_Font1.Dispose();
            m_Font2.Dispose();
            m_Font3.Dispose();
            base.Dispose();
        }

		void OnEdit(ControlBase control, EventArgs args)
        {
            Control.TextBox box = control as Control.TextBox;
            UnitPrint(String.Format("TextBox: OnEdit: {0}", box.Text));
        }

		void OnSubmit(ControlBase control, EventArgs args)
        {
            Control.TextBox box = control as Control.TextBox;
            UnitPrint(String.Format("TextBox: OnSubmit: {0}", box.Text));
        }

		class TextPad : Control.Window
		{
			private readonly Control.MultilineTextBox m_TextBox;

			private readonly Font m_Font;

			private string m_Path;

			public TextPad(ControlBase parent)
				: base(parent)
			{
				StartPosition = StartPosition.CenterParent;
				Size = new Size(400, 300);
				Padding = new Padding(1, 0, 1, 1);
				Title = "TextPad";

				DockLayout layout = new DockLayout(this);
				layout.Dock = Dock.Fill;

				Control.MenuStrip menuStrip = new MenuStrip(layout);
				menuStrip.Dock = Dock.Top;
				Control.MenuItem fileMenu = menuStrip.AddItem("File");
				fileMenu.Menu.AddItem("Open...", String.Empty, "Ctrl+O").SetAction((s, a) => OnOpen(s, a));
				fileMenu.Menu.AddItem("Save", String.Empty, "Ctrl+S").SetAction((s, a) => OnSave(s, a));
				fileMenu.Menu.AddItem("Save As...").SetAction((s, a) => OnSaveAs(s, a));
				fileMenu.Menu.AddItem("Quit", String.Empty, "Ctrl+Q").SetAction((s, a) => Close());

				m_Font = Skin.DefaultFont.Copy();
				m_Font.FaceName = "Courier New";

				Control.StatusBar statusBar = new Gwen.Control.StatusBar(layout);
				statusBar.Dock = Dock.Bottom;

				Control.Label length = new Control.Label(statusBar);
				length.Margin = new Margin(5, 0, 5, 0);

				Control.Label label = new Control.Label(statusBar);
				label.Margin = new Margin(5, 0, 5, 0);
				label.Text = "Length:";

				Control.Label lines = new Control.Label(statusBar);
				lines.Margin = new Margin(5, 0, 5, 0);

				label = new Control.Label(statusBar);
				label.Margin = new Margin(5, 0, 5, 0);
				label.Text = "Lines:";

				m_TextBox = new Control.MultilineTextBox(layout);
				m_TextBox.Dock = Dock.Fill;
				m_TextBox.ShouldDrawBackground = false;
				m_TextBox.Font = m_Font;
				m_TextBox.TextChanged += (sender, arguments) => { lines.Text = m_TextBox.TotalLines.ToString(); length.Text = m_TextBox.Text.Length.ToString(); };
				m_TextBox.Text = "";

				m_Path = null;
			}

			public override void Dispose()
			{
				m_Font.Dispose();
				base.Dispose();
			}

			private void OnOpen(ControlBase sender, EventArgs args)
			{
				OpenFileDialog dialog = Component.Create<OpenFileDialog>(this);
				dialog.Title = "Open Text File";
				dialog.OkButtonText = "Open";
				dialog.Filters = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
				dialog.InitialFolder = m_Path == null ? Platform.Platform.CurrentDirectory : Platform.Platform.GetDirectoryName(m_Path);
				dialog.Callback = path =>
				{
					if (!String.IsNullOrWhiteSpace(path))
					{
						try
						{
							StreamReader reader = new StreamReader(Platform.Platform.GetFileStream(path, false), System.Text.Encoding.UTF8);
							m_TextBox.Text = reader.ReadToEnd();
							m_Path = path;
							Title = Platform.Platform.GetFileName(m_Path) + " - TextPad";
						}
						catch (Exception)
						{
						}
					}
				};
			}

			private void OnSave(ControlBase sender, EventArgs args)
			{
				if (m_Path != null)
				{
					try
					{
						StreamWriter writer = new StreamWriter(Platform.Platform.GetFileStream(m_Path, true), System.Text.Encoding.UTF8);
						writer.Write(m_TextBox.Text);
					}
					catch (Exception)
					{
					}
				}
				else
				{
					OnSaveAs(sender, args);
				}
			}

			private void OnSaveAs(ControlBase sender, EventArgs args)
			{
				SaveFileDialog dialog = Component.Create<SaveFileDialog>(this);
				dialog.Title = "Save Text File As";
				dialog.OkButtonText = "Save";
				dialog.Filters = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
				if (m_Path == null)
					dialog.InitialFolder = Platform.Platform.CurrentDirectory;
				else
					dialog.CurrentItem = m_Path;
				dialog.Callback = path =>
				{
					if (!String.IsNullOrWhiteSpace(path))
					{
						try
						{
							StreamWriter writer = new StreamWriter(Platform.Platform.GetFileStream(path, true), System.Text.Encoding.UTF8);
							writer.Write(m_TextBox.Text);
							m_Path = path;
							Title = Platform.Platform.GetFileName(m_Path) + " - TextPad";
						}
						catch (Exception)
						{
						}
					}
				};
			}
		}
    }
}
