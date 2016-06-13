using System;
using Gwen.Control;
using Gwen.Xml;
using Gwen.CommonDialog;
using Gwen.Control.Layout;

namespace Gwen.UnitTest
{
	[UnitTest(Category = "Xml", Order = 602)]
	public class CommonDialogs : GUnit
	{
		public CommonDialogs(ControlBase parent)
			: base(parent)
		{
			Control.Layout.GridLayout grid = new Control.Layout.GridLayout(this);
			grid.Dock = Dock.Fill;
			grid.SetColumnWidths(Control.Layout.GridLayout.AutoSize, Control.Layout.GridLayout.Fill);

			Control.Button button;

			{
				Control.Label openFile = null;

				button = new Control.Button(grid);
				button.Margin = Margin.Five;
				button.Text = "OpenFileDialog";
				button.Clicked += (sender, args) =>
				{
					openFile.Text = "";
					OpenFileDialog dialog = Component.Create<OpenFileDialog>(this);
					dialog.InitialFolder = "C:\\";
					dialog.Filters = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
					dialog.Callback = (path) => openFile.Text = path != null ? path : "Cancelled";
				};

				openFile = new Control.Label(grid);
				openFile.TextPadding = new Padding(3, 0, 0, 0);
				openFile.Alignment = Alignment.Left | Alignment.CenterV;
			}

			{
				Control.Label saveFile = null;

				button = new Control.Button(grid);
				button.Margin = Margin.Five;
				button.Text = "SaveFileDialog";
				button.Clicked += (sender, args) =>
				{
					saveFile.Text = "";
					SaveFileDialog dialog = Component.Create<SaveFileDialog>(this);
					dialog.Callback = (path) => saveFile.Text = path != null ? path : "Cancelled";
				};

				saveFile = new Control.Label(grid);
				saveFile.TextPadding = new Padding(3, 0, 0, 0);
				saveFile.Alignment = Alignment.Left | Alignment.CenterV;
			}

			{
				Control.Label createFile = null;

				button = new Control.Button(grid);
				button.Margin = Margin.Five;
				button.Text = "SaveFileDialog (create)";
				button.Clicked += (sender, args) =>
				{
					createFile.Text = "";
					SaveFileDialog dialog = Component.Create<SaveFileDialog>(this);
					dialog.Title = "Create File";
					dialog.OkButtonText = "Create";
					dialog.Callback = (path) => createFile.Text = path != null ? path : "Cancelled";
				};

				createFile = new Control.Label(grid);
				createFile.TextPadding = new Padding(3, 0, 0, 0);
				createFile.Alignment = Alignment.Left | Alignment.CenterV;
			}

			{
				Control.Label selectFolder = null;

				button = new Control.Button(grid);
				button.Margin = Margin.Five;
				button.Text = "FolderBrowserDialog";
				button.Clicked += (sender, args) =>
				{
					selectFolder.Text = "";
					FolderBrowserDialog dialog = Component.Create<FolderBrowserDialog>(this);
					dialog.InitialFolder = "C:\\";
					dialog.Callback = (path) => selectFolder.Text = path != null ? path : "Cancelled";
				};

				selectFolder = new Control.Label(grid);
				selectFolder.TextPadding = new Padding(3, 0, 0, 0);
				selectFolder.Alignment = Alignment.Left | Alignment.CenterV;
			}
		}
	}
}
