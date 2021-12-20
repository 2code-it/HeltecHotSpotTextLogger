using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Services
{
	public class MessengerService : IMessengerService
	{
		private List<Subscription> _subscriptions = new List<Subscription>();
		private static object _lock = new object();

		public void Subscribe<T>(object receiver, Action<T> messageHandler)
		{
			lock (_lock)
			{
				if (_subscriptions.Count(x => x.Receiver == receiver && x.MessageType == typeof(T)) > 0)
				{
					throw new InvalidOperationException("Subscription already exists");
				}

				_subscriptions.Add(new Subscription(receiver, typeof(T), messageHandler));
			}
		}

		public void Unsubscribe<T>(object receiver)
		{
			lock (_lock)
			{
				Subscription subscription = _subscriptions.Where(x => x.Receiver == receiver && x.MessageType == typeof(T)).FirstOrDefault();
				if (subscription == null)
				{
					throw new InvalidOperationException("Subscription not found");
				}
			}
		}

		public void Publish<T>(T message)
		{
			lock (_lock)
			{
				var subscriptions = _subscriptions.Where(x => x.MessageType == typeof(T));
				foreach (Subscription subscription in subscriptions)
				{
					((Action<T>)subscription.MessageHandler).Invoke(message);
				}
			}
		}

		public void Dispose()
		{
			_subscriptions.Clear();
		}

		private class Subscription
		{
			public Subscription(object receiver, Type messageType, Delegate messageHandler)
			{
				Receiver = receiver;
				MessageType = messageType;
				MessageHandler = messageHandler;
			}
			public object Receiver { get; set; }
			public Type MessageType { get; set; }
			public Delegate MessageHandler { get; set; }
		}
	}
}
