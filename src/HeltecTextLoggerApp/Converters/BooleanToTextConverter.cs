using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HeltecTextLoggerApp.Converters
{
	public class BooleanToTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool)) throw new ArgumentException("Expected boolean", "value");
			if (!(parameter is string)) throw new ArgumentException("Expected string", "parameter");

			string[] parts = ((string)parameter).Split(',');
			return ((bool)value) ? parts[0] : parts[1];

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
