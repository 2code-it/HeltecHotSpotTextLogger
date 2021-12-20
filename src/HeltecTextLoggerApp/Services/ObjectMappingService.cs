using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Services
{
	public class ObjectMappingService
	{
		private static IDictionary<Type, PropertyInfo[]> _propertyCache = new Dictionary<Type, PropertyInfo[]>();
		private static object _lock = new object();

		public T Map<T, S>(S source)
		{
			T target = Activator.CreateInstance<T>();
			return Map(target, source);
		}

		public T Map<T, S>(T target, S source)
		{
			PropertyInfo[] propertiesTarget = GetPropertyInfos(typeof(T));
			PropertyInfo[] propertiesSource = GetPropertyInfos(typeof(S));
			foreach (PropertyInfo property in propertiesTarget)
			{
				PropertyInfo propertySource = propertiesSource.Where(x => x.Name == property.Name).FirstOrDefault();
				if (propertySource != null)
				{
					object value = propertySource.GetValue(source);
					property.SetValue(target, value);
				}
			}
			return target;
		}

		private static PropertyInfo[] GetPropertyInfos(Type type)
		{
			lock (_lock)
			{
				if (!_propertyCache.ContainsKey(type))
				{
					PropertyInfo[] propertyInfos = type.GetProperties().Where(x => x.CanWrite && x.CanRead).ToArray();
					_propertyCache.Add(type, propertyInfos);
				}
				return _propertyCache[type];
			}
		}
	}
}
