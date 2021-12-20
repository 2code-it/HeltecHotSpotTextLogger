using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HeltecTextLoggerApp.Services
{
	public class AppIniFileService : IAppIniFileService
	{
		public AppIniFileService(): this(".\\app.ini")
		{
		}

		public AppIniFileService(string filename)
		{
			_data = File.ReadAllText(filename);
		}

		public AppIniFileService(byte[] data)
		{
			_data = Encoding.UTF8.GetString(data);
		}

		private IDictionary<string, IDictionary<string, string>> _sections;
		private string _data;
		public string GetItem(string sectionName, string keyName)
		{
			EnsureSectionsLoaded();
			if (!_sections.ContainsKey(sectionName))
			{
				throw new ArgumentException($"Section '{sectionName}' not found", "sectionName");
			}
			if (!_sections[sectionName].ContainsKey(keyName))
			{
				throw new ArgumentException($"Key '{keyName}' not found", "keyName");
			}

			return _sections[sectionName][keyName];
		}

		public T GetObjectFromSection<T>(string sectionName)
		{
			EnsureSectionsLoaded();
			if (!_sections.ContainsKey(sectionName))
			{
				throw new ArgumentException($"Section '{sectionName}' not found", "sectionName");
			}
			T target = Activator.CreateInstance<T>();
			typeof(T).GetProperties().Where(x => x.CanWrite).ToList().ForEach(x =>
			{
				if (_sections[sectionName].ContainsKey(x.Name))
				{
					object value = null;
					try
					{
						value = Convert.ChangeType(_sections[sectionName][x.Name], x.PropertyType);
					}
					catch(Exception e)
					{
						throw new InvalidOperationException($"Failed to convert value for property '{x.Name}'", e);
					}
					x.SetValue(target, value);
				}
			});
			return target;
		}

		public bool Exists(string sectionName, string keyName)
		{
			EnsureSectionsLoaded();
			return _sections.ContainsKey(sectionName) && _sections[sectionName].ContainsKey(keyName);
		}

		public string TryGetItem(string sectionName, string keyName, string defaultValue = null)
		{
			EnsureSectionsLoaded();
			if (!_sections.ContainsKey(sectionName))
			{
				return defaultValue;
			}
			if (!_sections[sectionName].ContainsKey(keyName))
			{
				return defaultValue;
			}

			return _sections[sectionName][keyName];
		}

		public string[] GetSections()
		{
			EnsureSectionsLoaded();
			return _sections.Keys.ToArray();
		}

		private void EnsureSectionsLoaded()
		{
			if(_sections == null)
			{
				_sections = new Dictionary<string, IDictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);
				string[] dataSections = _data.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);
				foreach(string dataSection in dataSections)
				{
					string[] lines = dataSection.Split("\r\n");
					if (lines.Length > 0)
					{
						string sectionName = lines[0].Trim('[', ']');
						IDictionary<string, string> sectionDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
						for(int i=1; i<lines.Length; i++)
						{
							if (lines[i].Length == 0 || lines[i][0] == ';') continue;
							int equalsIndex = lines[i].IndexOf('=');
							if (equalsIndex == -1) continue;
							sectionDictionary.Add(lines[i].Substring(0, equalsIndex), lines[i].Substring(equalsIndex + 1));
						}
						_sections.Add(sectionName, sectionDictionary);
					}
				}
			}
		}
	}
}
