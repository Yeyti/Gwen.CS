using System;
using System.Collections.Generic;
using Gwen.Control;
using Gwen.Xml;
using System.Linq;

namespace Gwen.XmlDesigner
{
	public class ElementSelectedEventArgs : EventArgs
	{
		public string Element { get; set; }
	}

	public class AttributeSelectedEventArgs : EventArgs
	{
		public string Attribute { get; set; }
	}

	public class ElementBrowser : Component
	{
		[XmlEvent]
		public event ControlBase.GwenEventHandler<ElementSelectedEventArgs> ElementSelected;

		[XmlEvent]
		public event ControlBase.GwenEventHandler<AttributeSelectedEventArgs> AttributeSelected;

		public ElementBrowser(ControlBase parent)
			: base(parent, new XmlStringSource(Xml))
		{
			m_ElementSelectedEventArgs = new ElementSelectedEventArgs();
			m_AttributeSelectedEventArgs = new AttributeSelectedEventArgs();
		}

		protected override void OnCreated()
		{
			m_Elements = GetControl<TreeControl>("Elements");
			m_Attributes = GetControl<TreeControl>("Attributes");

			var elements = Parser.GetElements().ToList();
			elements.Sort((t1, t2) =>
			{
				return String.Compare(t1.Key, t2.Key);
			});

			TreeNode staticElements = m_Elements.AddNode("Static Controls");
			TreeNode commonElements = m_Elements.AddNode("Common Controls");
			TreeNode layoutElements = m_Elements.AddNode("Layout Controls");
			TreeNode containerElements = m_Elements.AddNode("Container Controls");
			m_XmlFiles = m_Elements.AddNode("Project Files");

			foreach (var element in elements)
			{
				if (!m_IgnoredElements.Contains(element.Key))
				{
					if (m_StaticElements.Contains(element.Key))
						staticElements.AddNode(element.Key, element.Key, element.Value);
					else if (m_CommonElements.Contains(element.Key))
						commonElements.AddNode(element.Key, element.Key, element.Value);
					else if (m_LayoutElements.Contains(element.Key))
						layoutElements.AddNode(element.Key, element.Key, element.Value);
					else if (m_ContainerElements.Contains(element.Key))
						containerElements.AddNode(element.Key, element.Key, element.Value);
					else
						m_Elements.AddNode(element.Key, element.Key, element.Value);
				}
			}

			m_Elements.ExpandAll();
		}

		public void SetFileList(XmlFileList fileList)
		{
			m_XmlFiles.RemoveAllNodes();

			foreach (XmlFile xmlFile in fileList.XmlFiles)
			{
				m_XmlFiles.AddNode(xmlFile.Name, xmlFile.FileName, xmlFile);
			}
		}

		public void OnElementSelected(ControlBase sender, EventArgs args)
		{
			TreeNode node = sender as TreeNode;
			if (node != null)
			{
				string element = node.Name;
				if (element == null)
					return;

				m_Attributes.RemoveAllNodes();

				var attr = Parser.GetAttributes(element);
				if (attr != null)
				{
					var attributes = attr.ToList();
					attributes.Sort((t1, t2) =>
					{
						return String.Compare(t1.Key, t2.Key);
					});

					TreeNode commonAttributes = m_Attributes.AddNode("Common Attributes");
					TreeNode layoutAttributes = m_Attributes.AddNode("Layout Attributes");

					foreach (var attribute in attributes)
					{
						if (m_CommonAttributes.Contains(attribute.Key))
							commonAttributes.AddNode(attribute.Key, attribute.Key, attribute.Value);
						else if (m_LayoutAttributes.Contains(attribute.Key))
							layoutAttributes.AddNode(attribute.Key, attribute.Key, attribute.Value);
						else
							m_Attributes.AddNode(attribute.Key, attribute.Key, attribute.Value);
					}

					m_Attributes.ExpandAll();
				}
			}
		}

		public void OnElementDoubleClicked(ControlBase sender, EventArgs args)
		{
			TreeNode node = sender as TreeNode;
			if (node != null)
			{
				if (ElementSelected != null)
				{
					m_ElementSelectedEventArgs.Element = node.Name;
					ElementSelected(View, m_ElementSelectedEventArgs);
				}
			}
		}

		public void OnAttributeSelected(ControlBase sender, EventArgs args)
		{
			TreeNode node = sender as TreeNode;
			if (node != null)
			{
			}
		}

		public void OnAttributeDoubleClicked(ControlBase sender, EventArgs args)
		{
			TreeNode node = sender as TreeNode;
			if (node != null)
			{
				if (AttributeSelected != null)
				{
					m_AttributeSelectedEventArgs.Attribute = node.Name;
					AttributeSelected(View, m_AttributeSelectedEventArgs);
				}
			}
		}

		private TreeControl m_Elements;
		private TreeControl m_Attributes;
		private TreeNode m_XmlFiles;

		private ElementSelectedEventArgs m_ElementSelectedEventArgs;
		private AttributeSelectedEventArgs m_AttributeSelectedEventArgs;

		private static readonly HashSet<string> m_IgnoredElements = new HashSet<string>
		{
			"XmlDesigner", "TextEditor", "Viewer", "ElementBrowser", "XmlFiles", "Container", "FileDialog", "OpenFileDialog", "SaveFileDialog", "FolderBrowserDialog"
		};

		private static readonly HashSet<string> m_StaticElements = new HashSet<string>
		{
			"Label", "GroupBox", "ProgressBar", "ImagePanel", "StatusBar"
		};

		private static readonly HashSet<string> m_CommonElements = new HashSet<string>
		{
			"LinkLabel", "RichLabel", "Button", "TextBox", "MultilineTextBox", "TextBoxNumeric", "TextBoxPassword", "CheckBox", "LabeledCheckBox", "RadioButton", "LabeledRadioButton", "NumericUpDown", "HorizontalSlider", "VerticalSlider", "ComboBox"
		};

		private static readonly HashSet<string> m_LayoutElements = new HashSet<string>
		{
			"AnchorLayout", "DockLayout", "FlowLayout", "GridLayout", "HorizontalLayout", "VerticalLayout", "StackLayout", "HorizontalSplitter", "VerticalSplitter", "CrossSplitter"
		};

		private static readonly HashSet<string> m_ContainerElements = new HashSet<string>
		{
			"Border", "Window", "ToolWindow", "ListBox", "ListBoxRow", "TreeControl", "TreeNode", "Properties", "TabControl", "ScrollControl", "MenuStrip", "Menu", "MenuItem"
		};

		private static readonly HashSet<string> m_CommonAttributes = new HashSet<string>
		{
			"Text", "Name", "UserData", "Font", "Size", "Width", "Height", "Margin", "Padding"
		};

		private static readonly HashSet<string> m_LayoutAttributes = new HashSet<string>
		{
			"Dock", "Alignment", "ImageAlignment", "HorizontalAlignment", "VerticalAlignment", "Anchor", "AnchorBounds", "MinimumSize", "MaximumSize", "Position", "Left", "Top", "ColumnWidths", "RowHeights"
		};

		private static readonly string Xml = @"<?xml version='1.0' encoding='UTF-8'?>
			<HorizontalSplitter SplitterSize='2' Value='0.7'>
				<Container Title='Controls'>
					<TreeControl Name='Elements' Selected='OnElementSelected' NodeDoubleClicked='OnElementDoubleClicked' />
				</Container>
				<Container Title='Properties'>
					<TreeControl Name='Attributes' Selected='OnAttributeSelected' NodeDoubleClicked='OnAttributeDoubleClicked'/>
				</Container>
			</HorizontalSplitter>
			";
	}
}
