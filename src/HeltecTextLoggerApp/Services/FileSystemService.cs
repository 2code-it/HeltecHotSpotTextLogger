using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Services
{
	public class FileSystemService : IFileSystemService
	{
		public string GetBaseDirectory()
		{
			return AppDomain.CurrentDomain.BaseDirectory;
		}

		public async void FileAppendAllLines(string path, IEnumerable<string> contents)
		{
			await File.AppendAllLinesAsync(path, contents);
		}

		public DateTime FileGetLastWriteTime(string path)
		{
			return File.GetLastWriteTime(path);
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public string PathCombine(params string[] paths)
		{
			return Path.Combine(paths);
		}
	}
}
