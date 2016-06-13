using System;
using System.IO;
using Gwen;
using Gwen.Control;
using Gwen.Xml;
using Gwen.CommonDialog;

namespace Gwen.XmlDesigner
{
	public class ExitEventArgs : EventArgs
	{
		public bool Restart { get; set; }
	}

	public class XmlDesigner : Component
	{
		static XmlDesigner()
		{
			Component.Register<XmlDesigner>();
			Component.Register<TextEditor>();
			Component.Register<Viewer>();
			Component.Register<ElementBrowser>();
			Component.Register<XmlFiles>();
			Component.Register<Container>();

			Parser.RegisterEventHandlerConverter(typeof(XmlChangedEventArgs), (attribute, value) =>
			{
				return new Control.ControlBase.GwenEventHandler<XmlChangedEventArgs>(new XmlEventHandler<XmlChangedEventArgs>(value, attribute).OnEvent);
			});
			Parser.RegisterEventHandlerConverter(typeof(ExceptionEventArgs), (attribute, value) =>
			{
				return new Control.ControlBase.GwenEventHandler<ExceptionEventArgs>(new XmlEventHandler<ExceptionEventArgs>(value, attribute).OnEvent);
			});
			Parser.RegisterEventHandlerConverter(typeof(ElementSelectedEventArgs), (attribute, value) =>
			{
				return new Control.ControlBase.GwenEventHandler<ElementSelectedEventArgs>(new XmlEventHandler<ElementSelectedEventArgs>(value, attribute).OnEvent);
			});
			Parser.RegisterEventHandlerConverter(typeof(AttributeSelectedEventArgs), (attribute, value) =>
			{
				return new Control.ControlBase.GwenEventHandler<AttributeSelectedEventArgs>(new XmlEventHandler<AttributeSelectedEventArgs>(value, attribute).OnEvent);
			});
			Parser.RegisterEventHandlerConverter(typeof(XmlFileSelectedEventArgs), (attribute, value) =>
			{
				return new Control.ControlBase.GwenEventHandler<XmlFileSelectedEventArgs>(new XmlEventHandler<XmlFileSelectedEventArgs>(value, attribute).OnEvent);
			});
			Parser.RegisterEventHandlerConverter(typeof(ExitEventArgs), (attribute, value) =>
			{
				return new Control.ControlBase.GwenEventHandler<ExitEventArgs>(new XmlEventHandler<ExitEventArgs>(value, attribute).OnEvent);
			});
		}

		public static Settings Settings
		{
			get
			{
				if (m_Settings == null)
				{
					Exception ex;
					m_Settings = Settings.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GwenXmlDesigner", "Settings.xml"), out ex);
				}

				return m_Settings;
			}
		}

		[XmlEvent]
		public event ControlBase.GwenEventHandler<ExitEventArgs> Exit;

		public XmlDesigner(ControlBase parent)
			: base(parent, new XmlStringSource(Xml))
		{
			m_Timer = new Timer();
			m_Timer.Elapsed += TimerElapsed;
			m_Timer.Interval = 0;
			m_Timer.IsOneTime = true;
			m_Timer.Start();
		}

		protected override void OnCreated()
		{
			m_TextEditor = GetControl<TextEditor>("TextEditor");
			m_Viewer = GetControl<Viewer>("Viewer");
			m_StatusBar = GetControl<StatusBar>("StatusBar");
			m_XmlFiles = GetControl<XmlFiles>("XmlFiles");
			m_ElementBrowser = GetControl<ElementBrowser>("ElementBrowser");
			m_SaveAsXmlFile = GetControl<MenuItem>("SaveAsXmlFile");
			m_RemoveXmlFile = GetControl<MenuItem>("RemoveXmlFile");
			m_DebugOutlines = GetControl<LabeledCheckBox>("DebugOutlines");
			m_Skins = GetControl<ComboBox>("Skins");

			SetCurrentXmlFile(null);

			Exception ex;
			m_FileList = XmlFileList.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GwenXmlDesigner", "XmlFiles.xml"), out ex);
			UpdateFileList();

			m_DebugOutlines.IsChecked = Settings.DebugOutlines;

			foreach (string path in Settings.GetAvailableSkins())
			{
				m_Skins.AddItem(Path.GetFileNameWithoutExtension(path), path, path);
			}

			m_Skins.SelectByUserData(Settings.Skin);
		}

		private void OnXmlChanged(ControlBase sender, XmlChangedEventArgs args)
		{
			SetXml(args.Xml);
		}

		private void OnElementSelected(ControlBase sender, ElementSelectedEventArgs args)
		{
			m_TextEditor.AddElement(args.Element);
		}

		private void OnAttributeSelected(ControlBase sender, AttributeSelectedEventArgs args)
		{
			m_TextEditor.AddAttribute(args.Attribute);
		}

		private void OnXmlFileSelected(ControlBase sender, XmlFileSelectedEventArgs args)
		{
			SetCurrentXmlFile(args.XmlFile);
		}

		private void OnException(ControlBase sender, ExceptionEventArgs args)
		{
			if (args.Exception != null)
			{
				m_StatusBar.Text = args.Exception.Message;
				m_StatusBar.TextColor = Color.Red;
			}
			else
			{
				m_StatusBar.Text = "Ok";
				m_StatusBar.TextColor = Color.Green;
			}
		}

		private void OnNewXmlFile(ControlBase sender, EventArgs args)
		{
			if (m_InDialog)
				return;

			m_InDialog = true;
			SaveFileDialog dialog = Component.Create<SaveFileDialog>(View);
			dialog.Title = "New XML File";
			dialog.OkButtonText = "Create";
			dialog.Filters = "XML Files (*.xml)|*.xml";
			dialog.InitialFolder = m_FileList.CurrentPath;
			dialog.Callback = path =>
			{
				m_InDialog = false;

				if (!String.IsNullOrWhiteSpace(path))
				{
					path = Path.ChangeExtension(path, ".xml");
					XmlFile xmlFile = new XmlFile(path);
					m_FileList.XmlFiles.Add(xmlFile);
					m_FileList.CurrentPath = Path.GetDirectoryName(path);
					m_CurrentXmlFile = xmlFile;
					UpdateFileList();
				}
			};
		}

		private void OnOpenXmlFile(ControlBase sender, EventArgs args)
		{
			if (m_InDialog)
				return;

			m_InDialog = true;
			OpenFileDialog dialog = Component.Create<OpenFileDialog>(View);
			dialog.Title = "Open XML File";
			dialog.OkButtonText = "Open";
			dialog.Filters = "XML Files (*.xml)|*.xml";
			dialog.InitialFolder = m_FileList.CurrentPath;
			dialog.Callback = path =>
			{
				m_InDialog = false;

				if (!String.IsNullOrWhiteSpace(path))
				{
					Exception ex;
					XmlFile xmlFile = XmlFile.Open(path, out ex);
					if (xmlFile == null)
					{
						MessageBox.Show(View, "Unable to open XML. " + ex.Message);
						return;
					}
					m_FileList.XmlFiles.Add(xmlFile);
					m_FileList.CurrentPath = Path.GetDirectoryName(path);
					m_CurrentXmlFile = xmlFile;
					UpdateFileList();
				}
			};
		}

		private void OnSaveAsXmlFile(ControlBase sender, EventArgs args)
		{
			if (m_InDialog)
				return;

			if (m_CurrentXmlFile == null)
				return;

			m_InDialog = true;
			SaveFileDialog dialog = Component.Create<SaveFileDialog>(View);
			dialog.Title = "Save XML File As";
			dialog.OkButtonText = "Save";
			dialog.Filters = "XML Files (*.xml)|*.xml";
			dialog.CurrentItem = m_CurrentXmlFile.FileName;
			dialog.Callback = path =>
			{
				m_InDialog = false;

				if (!String.IsNullOrWhiteSpace(path))
				{
					path = Path.ChangeExtension(path, ".xml");
					m_CurrentXmlFile.FileName = path;
					m_FileList.CurrentPath = Path.GetDirectoryName(path);
					UpdateFileList();
				}
			};
		}

		private void OnRemoveXmlFile(ControlBase sender, EventArgs args)
		{
			if (m_InDialog)
				return;

			if (m_CurrentXmlFile == null)
				return;

			m_FileList.XmlFiles.Remove(m_CurrentXmlFile);
			m_CurrentXmlFile.Dispose();
			SetCurrentXmlFile(null);
			UpdateFileList();
		}

		private void OnExit(ControlBase sender, EventArgs args)
		{
			SetCurrentXmlFile(null);

			if (Exit != null)
				Exit(View, new ExitEventArgs() { Restart = false });
		}

		private void OnAbout(ControlBase sender, EventArgs args)
		{
			Component.Create<About>(View);
		}

		private void OnDebugOutlinesChanged(ControlBase sender, EventArgs args)
		{
			Settings.DebugOutlines = m_DebugOutlines.IsChecked;
			SaveSettings();

			m_Viewer.DebugOutlines = Settings.DebugOutlines;
		}

		private void OnSkinSelected(ControlBase sender, ItemSelectedEventArgs args)
		{
			if (args.SelectedItem != null)
			{
				Settings.Skin = (string)args.SelectedItem.UserData;
				SaveSettings();
			}

			if (Exit != null)
				Exit(View, new ExitEventArgs() { Restart = true });
		}

		private void TimerElapsed(object sender, EventArgs args)
		{
			if (m_CurrentXmlFile != null)
			{
				m_Viewer.SetXml(m_CurrentXmlFile.Xml);
			}
		}

		private void SaveSettings()
		{
			Exception ex;
			if (!Settings.Save(out ex))
				MessageBox.Show(View, "Unable to save settings. " + ex.Message);
		}

		private void SetXml(string xml)
		{
			if (m_CurrentXmlFile != null)
			{
				m_CurrentXmlFile.Xml = xml;
				SaveCurrentXmlFile();
				m_Viewer.SetXml(xml);
			}
		}

		private void SetCurrentXmlFile(XmlFile xmlFile)
		{
			if (m_CurrentXmlFile != null)
			{
				SaveCurrentXmlFile();
			}

			m_CurrentXmlFile = xmlFile;

			if (xmlFile != null)
			{
				m_TextEditor.SetXml(String.Format("{0} - {1}", m_CurrentXmlFile.Name, m_CurrentXmlFile.FileName), m_CurrentXmlFile.Xml);
				m_Viewer.SetXml(m_CurrentXmlFile.Xml);
			}
			else
			{
				m_TextEditor.SetXml("", null);
			}
		}

		private void SaveCurrentXmlFile()
		{
			Exception ex;
			if (!m_CurrentXmlFile.SaveXml(out ex))
			{
				MessageBox.Show(View, "Unable to save XML. " + ex.Message);
			}
		}

		private void UpdateFileList()
		{
			m_XmlFiles.SetFileList(m_FileList, m_CurrentXmlFile);
			m_ElementBrowser.SetFileList(m_FileList);

			UpdateControls();

			Exception ex;
			if (!m_FileList.Save(out ex))
			{
				MessageBox.Show(View, "Unable to save XML file list. " + ex.Message);
			}
		}

		private void UpdateControls()
		{
			m_SaveAsXmlFile.IsDisabled = m_CurrentXmlFile == null;
			m_RemoveXmlFile.IsDisabled = m_CurrentXmlFile == null;
		}

		private TextEditor m_TextEditor;
		private Viewer m_Viewer;
		private StatusBar m_StatusBar;
		private XmlFiles m_XmlFiles;
		private ElementBrowser m_ElementBrowser;
		private MenuItem m_SaveAsXmlFile;
		private MenuItem m_RemoveXmlFile;
		private LabeledCheckBox m_DebugOutlines;
		private ComboBox m_Skins;

		private XmlFileList m_FileList;
		private XmlFile m_CurrentXmlFile;

		private bool m_InDialog;

		private Timer m_Timer;

		private static Settings m_Settings = null;

		public static string Skin = "DefaultSkin.png";

		private static readonly string Xml = @"<?xml version='1.0' encoding='UTF-8'?>
			<DockLayout Dock='Fill'>

				<MenuStrip Dock='Top'>
					<MenuItem Text='File'>
						<MenuItem Name='NewXmlFile' Text='New...' Clicked='OnNewXmlFile' />
						<MenuDivider />
						<MenuItem Name='OpenXmlFile' Text='Open...' Clicked='OnOpenXmlFile' />
						<MenuDivider />
						<MenuItem Name='SaveAsXmlFile' Text='Save As...' Clicked='OnSaveAsXmlFile' />
						<MenuItem Name='RemoveXmlFile' Text='Remove' Clicked='OnRemoveXmlFile' />
						<MenuDivider />
						<MenuItem Name='Exit' Text='Exit' Clicked='OnExit' />
					</MenuItem>
					<MenuItem Text='Help'>
						<MenuItem Name='About' Text='About...' Clicked='OnAbout' />
					</MenuItem>
				</MenuStrip>

				<VerticalSplitter Margin='2' Dock='Fill' SplitterSize='2' Value='0.2'>
					<XmlFiles Name='XmlFiles' Dock='Fill' XmlFileSelected='OnXmlFileSelected' />
					<VerticalSplitter Dock='Fill' SplitterSize='2' Value='0.8'>
						<HorizontalSplitter Dock='Fill' SplitterSize='2' Value='0.8'>
							<Viewer Name='Viewer' Dock='Fill' Exception='OnException'/>
							<TextEditor Name='TextEditor' Dock='Fill' XmlChanged='OnXmlChanged' />
						</HorizontalSplitter>
						<ElementBrowser Name='ElementBrowser' Dock='Fill' ElementSelected='OnElementSelected' AttributeSelected='OnAttributeSelected' />
					</VerticalSplitter>
				</VerticalSplitter>

				<StatusBar Name='StatusBar' Dock='Bottom'>
					<LabeledCheckBox Name='DebugOutlines' Margin='2,0,2,0' Text='Debug Outlines' CheckChanged='OnDebugOutlinesChanged' />
					<ComboBox Name='Skins' Margin='2,0,2,0' Width='150' ItemSelected='OnSkinSelected' />
					<Label Margin='2,0,2,0' Text='Skin:' />
				</StatusBar>

			</DockLayout>
			";
	}
}
