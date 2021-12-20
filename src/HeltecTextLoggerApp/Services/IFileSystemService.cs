using System;
using System.Collections.Generic;

namespace HeltecTextLoggerApp.Services
{
	public interface IFileSystemService
	{
		void FileAppendAllLines(string path, IEnumerable<string> contents);
		bool FileExists(string path);
		DateTime FileGetLastWriteTime(string path);
		string GetBaseDirectory();
		string PathCombine(params string[] paths);
		
	}
}