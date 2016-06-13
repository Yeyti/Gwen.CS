using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gwen.Platform
{
	public class Android : IPlatform
	{
		private DateTime m_FirstTime = DateTime.Now;

		/// <summary>
		/// Gets text from clipboard.
		/// </summary>
		/// <returns>Clipboard text.</returns>
		public string GetClipboardText()
		{
			return String.Empty;
		}

		/// <summary>
		/// Sets the clipboard text.
		/// </summary>
		/// <param name="text">Text to set.</param>
		/// <returns>True if succeeded.</returns>
		public bool SetClipboardText(string text)
		{
			return false;
		}

		/// <summary>
		/// Gets elapsed time since this class was initalized.
		/// </summary>
		/// <returns>Time interval in seconds.</returns>
		public float GetTimeInSeconds()
		{
			return (float)((DateTime.Now - m_FirstTime).TotalSeconds);
		}

		/// <summary>
		/// Changes the mouse cursor.
		/// </summary>
		/// <param name="cursor">Cursor type.</param>
		public void SetCursor(Cursor cursor)
		{
		}

		/// <summary>
		/// Get special folders of the system.
		/// </summary>
		/// <returns>List of folders.</returns>
		public IEnumerable<ISpecialFolder> GetSpecialFolders()
		{
			List<SpecialFolder> folders = new List<SpecialFolder>();

			try
			{
				folders.Add(new SpecialFolder("Documents", "Libraries", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
				folders.Add(new SpecialFolder("Music", "Libraries", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
				folders.Add(new SpecialFolder("Pictures", "Libraries", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));
				folders.Add(new SpecialFolder("Videos", "Libraries", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)));
			}
			catch (Exception)
			{

			}

			DriveInfo[] drives = null;
			try
			{
				drives = DriveInfo.GetDrives();
			}
			catch (Exception) { }

			if (drives != null)
			{
				foreach (DriveInfo driveInfo in drives)
				{
					try
					{
						if (driveInfo.IsReady)
						{
							if (String.IsNullOrWhiteSpace(driveInfo.VolumeLabel))
								folders.Add(new SpecialFolder(driveInfo.Name, "Computer", driveInfo.Name));
							else
								folders.Add(new SpecialFolder(String.Format("{0} ({1})", driveInfo.VolumeLabel, driveInfo.Name), "Computer", driveInfo.Name));
						}
					}
					catch (Exception) { }
				}
			}

			return folders;
		}

		public string GetFileName(string path)
		{
			return Path.GetFileName(path);
		}

		public string GetDirectoryName(string path)
		{
			return Path.GetDirectoryName(path);
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public string Combine(string path1, string path2)
		{
			return Path.Combine(path1, path2);
		}

		public string Combine(string path1, string path2, string path3)
		{
			return Path.Combine(path1, path2, path3);
		}

		public string Combine(string path1, string path2, string path3, string path4)
		{
			return Path.Combine(path1, path2, path3, path4);
		}

		public string CurrentDirectory { get { return Environment.CurrentDirectory; } }

		public IEnumerable<IFileSystemDirectoryInfo> GetDirectories(string path)
		{
			DirectoryInfo di = new DirectoryInfo(path);
			return di.GetDirectories().Select(d => new FileSystemDirectoryInfo(d.FullName, d.LastWriteTime) as IFileSystemDirectoryInfo);
		}

		public IEnumerable<IFileSystemFileInfo> GetFiles(string path, string filter)
		{
			DirectoryInfo di = new DirectoryInfo(path);
			return di.GetFiles(filter).Select(f => new FileSystemFileInfo(f.FullName, f.LastWriteTime, f.Length) as IFileSystemFileInfo);

		}

		public Stream GetFileStream(string path, bool isWritable)
		{
			return new FileStream(path, isWritable ? FileMode.Create : FileMode.Open, isWritable ? FileAccess.Write : FileAccess.Read);
		}

		private class SpecialFolder : ISpecialFolder
		{
			public SpecialFolder(string name, string category, string path)
			{
				this.Name = name;
				this.Category = category;
				this.Path = path;
			}

			public string Name { get; internal set; }
			public string Category { get; internal set; }
			public string Path { get; internal set; }
		}

		public class FileSystemItemInfo : IFileSystemItemInfo
		{
			public FileSystemItemInfo(string path, DateTime lastWriteTime)
			{
				this.Name = Path.GetFileName(path);
				this.FullName = path;
				this.FormattedLastWriteTime = String.Format("{0} {1}", lastWriteTime.ToShortDateString(), lastWriteTime.ToLongTimeString());
			}

			public string Name { get; internal set; }
			public string FullName { get; internal set; }
			public string FormattedLastWriteTime { get; internal set; }
		}

		public class FileSystemDirectoryInfo : FileSystemItemInfo, IFileSystemDirectoryInfo
		{
			public FileSystemDirectoryInfo(string path, DateTime lastWriteTime)
				: base(path, lastWriteTime)
			{

			}
		}

		public class FileSystemFileInfo : FileSystemItemInfo, IFileSystemFileInfo
		{
			public FileSystemFileInfo(string path, DateTime lastWriteTime, long length)
				: base(path, lastWriteTime)
			{
				this.FormattedFileLength = FormatFileLength(length);
			}

			private string FormatFileLength(long length)
			{
				if (length > 1024 * 1024 * 1024)
					return String.Format("{0:0.0} GB", (double)length / (1024 * 1024 * 1024));
				else if (length > 1024 * 1024)
					return String.Format("{0:0.0} MB", (double)length / (1024 * 1024));
				else if (length > 1024)
					return String.Format("{0:0.0} kB", (double)length / 1024);
				else
					return String.Format("{0} B", length);
			}

			public string FormattedFileLength { get; internal set; }
		}
	}
}
