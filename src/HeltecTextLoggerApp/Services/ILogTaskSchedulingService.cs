using HeltecTextLoggerApp.Models;

namespace HeltecTextLoggerApp.Services
{
	public interface ILogTaskSchedulingService
	{
		void Configure(ILogInfo logInfo);
		void Start();
		void Stop();
	}
}