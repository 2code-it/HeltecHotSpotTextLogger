using System.Collections.Generic;

namespace HeltecTextLoggerApp.Services
{
	public interface IAppIniFileService
	{
		string GetItem(string section, string name);
		string TryGetItem(string sectionName, string keyName, string defaultValue = null);
		string[] GetSections();
		T GetObjectFromSection<T>(string sectionName);
		bool Exists(string sectionName, string keyName);

	}
}