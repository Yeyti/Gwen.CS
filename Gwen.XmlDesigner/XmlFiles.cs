using System;
using Gwen.Control;
using Gwen.Xml;

namespace Gwen.XmlDesigner
{
	public class XmlFileSelectedEventArgs : EventArgs
	{
		public XmlFile XmlFile { get; set; }
	}

	public class XmlFiles : Component
	{
		[XmlEvent]
		public event ControlBase.GwenEventHandler<XmlFileSelectedEventArgs> XmlFileSelected;

		public XmlFiles(ControlBase parent)
			: base(parent, new XmlStringSource(Xml))
		{
			m_XmlFileSelectedEventArgs = new XmlFileSelectedEventArgs();
		}

		protected override void OnCreated()
		{
			m_Items = GetControl<ListBox>("Items");
		}

		public void SetFileList(XmlFileList fileList, XmlFile current)
		{
			m_Items.Clear();

			foreach (XmlFile xmlFile in fileList.XmlFiles)
			{
				m_Items.AddRow(xmlFile.Name, xmlFile.FileName, xmlFile);
			}

			if (current == null && m_Items.RowCount > 0)
				m_Items.SelectedRowIndex = 0;
			else
				m_Items.SelectByUserData(current);
		}

		public void OnItemSelected(ControlBase sender, ItemSelectedEventArgs args)
		{
			ListBoxRow row = args.SelectedItem as ListBoxRow;
			if (row != null)
			{
				XmlFile xmlFile = row.UserData as XmlFile;
				if (xmlFile != null)
				{
					if (XmlFileSelected != null)
					{
						m_XmlFileSelectedEventArgs.XmlFile = xmlFile;
						XmlFileSelected(View, m_XmlFileSelectedEventArgs);
					}
				}
			}
		}

		private ListBox m_Items;

		private XmlFileSelectedEventArgs m_XmlFileSelectedEventArgs;

		private static readonly string Xml = @"<?xml version='1.0' encoding='UTF-8'?>
			<DockLayout>
				<Container Dock='Fill' Title='Xml Files'>
					<ListBox Name='Items' RowSelected='OnItemSelected' />
				</Container>
			</DockLayout>
			";
	}
}
