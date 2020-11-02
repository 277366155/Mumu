using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Tax.Common.Extention;

namespace Tax.Common.RabbitMQ
{
    public class Publisher : BaseRabbitMQFactory, IDisposable
	{
		private static ConnectionFactory _connFac;
		private static IConnection _connection;
		private static Dictionary<string, IModel> _channelDic = new Dictionary<string, IModel>();

		public Publisher(RabbitMqOptions options,string channelName) : base(options, channelName)
		{

		}

		public void Send<T>(T obj)
		{
			Send(obj,_options.ExchangeName, _options.RoutingKey);
		}

		public void Send<T>(T obj, string exchangeName , string routingKey )
		{
			if (obj == null)
			{
				return;
			}
			var body = Encoding.UTF8.GetBytes(obj.ToJson());
			Channel.BasicPublish(exchangeName, routingKey, false, null, body);
		}

		private static object LockConnObj = new object();

		public override IConnection RabbitConnection
		{
			get
			{
				if (_connection == null)
				{
					lock (LockConnObj)
					{
						if (_connFac == null)
						{
							_connFac = new ConnectionFactory()
							{
								RequestedHeartbeat = TimeSpan.FromSeconds(30),
								RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
								AutomaticRecoveryEnabled = true,

								HostName = _options.HostName,
								UserName = _options.UserName,
								Password = _options.Password,
								VirtualHost = _options.VirtualHost,
								Port = _options.Port
							};
						}
						if (_connection == null)
						{
							_connection = _connFac.CreateConnection();
						}
					}
				}
				return _connection;
			}
		}

		private static object LockChannelObj = new object();
		public override IModel Channel
		{
			get
			{
				if (!_channelDic.ContainsKey(ChannelName))
				{
					lock (LockChannelObj)
					{
						if (!_channelDic.ContainsKey(ChannelName))
						{
							_channelDic.Add(ChannelName, RabbitConnection.CreateModel());
						}
					}
				}
				return _channelDic[ChannelName];
			}
		}

		public new  void Dispose()
		{
			base.Dispose();
		}
		~Publisher()
		{
			Dispose();
		}
	}
}
