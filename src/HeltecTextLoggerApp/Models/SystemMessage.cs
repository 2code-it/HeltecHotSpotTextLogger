using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Models
{
	public class SystemMessage
	{
		public SystemMessage(string severity, string message) 
		{
			Severity = severity;
			Message = message;
			
		}
		public SystemMessage (Exception exception)
		{
			Severity = "error";
			Message = exception.Message;
		}
		public DateTime Created { get; set; } = DateTime.Now;
		public string Severity { get; set; }
		public string Message { get; set; }
	}
}
