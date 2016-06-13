using System;
using System.IO;
using System.Xml.Serialization;

namespace Gwen.XmlDesigner
{
	public class XmlFile : IDisposable
	{
		private string m_Xml;
		private string m_FileName;
		private bool m_Modified;

		public string FileName
		{
			get { return m_FileName; }
			set
			{
				if (m_FileName != null)
					UserComponent.Unregister(m_FileName);

				m_FileName = value;

				UserComponent.Register(m_FileName);
			}
		}

		[XmlIgnore]
		public string Xml { get { return m_Xml; } set { if (value == m_Xml) return; m_Xml = value; m_Modified = true; } }

		[XmlIgnore]
		public string Name { get { return Path.GetFileNameWithoutExtension(m_FileName); } }

		public static XmlFile Open(string fileName, out Exception exception)
		{
			XmlFile xmlFile = new XmlFile(fileName);

			if (xmlFile.OpenXml(out exception))
			{
				return xmlFile;
			}

			return null;
		}

		public XmlFile()
		{
		}

		public XmlFile(string fileName)
		{
			FileName = fileName;
			Xml = "<?xml version='1.0' encoding='UTF-8'?>\n";
		}

		public bool OpenXml(out Exception exception)
		{
			try
			{
				string xml;
				using (StreamReader reader = new StreamReader(m_FileName))
				{
					xml = reader.ReadToEnd();
				}

				m_Xml = xml;

				exception = null;
				return true;
			}
			catch (Exception ex)
			{
				exception = ex;
				return false;
			}
		}

		public bool SaveXml(out Exception exception)
		{
			if (!m_Modified)
			{
				exception = null;
				return true;
			}

			try
			{
				using (StreamWriter writer = new StreamWriter(m_FileName))
				{
					writer.Write(m_Xml);
				}

				m_Modified = false;
				exception = null;
				return true;
			}
			catch (Exception ex)
			{
				exception = ex;
				return false;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_FileName != null)
					UserComponent.Unregister(m_FileName);
			}
		}
	}
}
