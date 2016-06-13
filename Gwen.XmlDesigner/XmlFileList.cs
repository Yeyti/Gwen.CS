using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Gwen.XmlDesigner
{
	public class XmlFileList
	{
		public static XmlFileList Open(string fileName, out Exception exception)
		{
			XmlFileList xmlFileList;

			exception = null;
			if (File.Exists(fileName))
			{
				try
				{
					using (StreamReader reader = new StreamReader(fileName))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(XmlFileList));
						xmlFileList = serializer.Deserialize(reader) as XmlFileList;
					}
				}
				catch (Exception ex)
				{
					exception = ex;

					xmlFileList = new XmlFileList();
				}
			}
			else
			{
				xmlFileList = new XmlFileList();
			}

			xmlFileList.m_FileName = fileName;
			
			foreach (XmlFile xmlfile in xmlFileList.m_XmlFiles)
			{
				if (!xmlfile.OpenXml(out exception))
				{
					xmlfile.Xml = "<?xml version='1.0' encoding='UTF-8'?>\n";
				}
			}

			return xmlFileList;
		}

		private XmlFileList()
		{
			CurrentPath = Environment.CurrentDirectory;
		}

		public bool Save(out Exception exception)
		{
			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(m_FileName)))
					Directory.CreateDirectory(Path.GetDirectoryName(m_FileName));

				using (StreamWriter writer = new StreamWriter(m_FileName))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(XmlFileList));
					serializer.Serialize(writer, this);
				}

				exception = null;
				return true;
			}
			catch (Exception ex)
			{
				exception = ex;
				return false;
			}
		}

		public string CurrentPath { get; set; }

		public List<XmlFile> XmlFiles { get { return m_XmlFiles; } }

		private List<XmlFile> m_XmlFiles = new List<XmlFile>();

		private string m_FileName;
	}
}
