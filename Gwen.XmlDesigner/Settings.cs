using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Gwen.XmlDesigner
{
	public class Settings
	{
		private Settings()
		{
			Skin = "DefaultSkin.png";
			DebugOutlines = false;
		}

		public static Settings Open(string fileName, out Exception exception)
		{
			Settings settings;

			exception = null;
			if (File.Exists(fileName))
			{
				try
				{
					using (StreamReader reader = new StreamReader(fileName))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(Settings));
						settings = serializer.Deserialize(reader) as Settings;
					}
				}
				catch (Exception ex)
				{
					exception = ex;

					settings = new Settings();
				}
			}
			else
			{
				settings = new Settings();
			}

			settings.m_FileName = fileName;

			return settings;
		}

		public bool Save(out Exception exception)
		{
			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(m_FileName)))
					Directory.CreateDirectory(Path.GetDirectoryName(m_FileName));

				using (StreamWriter writer = new StreamWriter(m_FileName))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Settings));
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

		public List<string> GetAvailableSkins()
		{
			List<string> skins = new List<string>();
			DirectoryInfo currentDirInfo;
			IOrderedEnumerable<FileInfo> files = null;
			try
			{
				currentDirInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skins"));
				files = currentDirInfo.GetFiles().OrderBy(fi => fi.Name);
			}
			catch (Exception)
			{
				return skins;
			}

			foreach (FileInfo fi in files)
			{
				skins.Add(fi.FullName);
			}

			return skins;
		}

		public string Skin { get; set; }
		public bool DebugOutlines { get; set; }

		private string m_FileName;
	}
}
