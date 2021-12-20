using System;

namespace HeltecTextLoggerApp.Services
{
	public interface IMessengerService: IDisposable
	{
		void Publish<T>(T message);
		void Subscribe<T>(object receiver, Action<T> messageHandler);
		void Unsubscribe<T>(object receiver);
	}
}