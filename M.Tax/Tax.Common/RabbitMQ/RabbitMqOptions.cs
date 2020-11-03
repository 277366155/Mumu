namespace Tax.Common.RabbitMQ
{
	public class RabbitMqOptions
	{
		public string ClientServiceName { get; set; }

		/// <summary>
		///mq服务地址cp
		/// </summary>
		public string HostName { get; set; }
		/// <summary>
		/// mq服务端口 cp
		/// </summary>
		public int Port { get; set; }
		/// <summary>
		/// 虚拟机路径 "/" ,"/testHost"等 cp
		/// </summary>
		public string VirtualHost { get; set; }
		/// <summary>
		/// 连接账号cp
		/// </summary>
		public string UserName { get; set; }
		/// <summary>
		/// 连接密码cp
		/// </summary>
		public string Password { get; set; }
		/// <summary>
		/// 交换机名cp
		/// </summary>
		public string ExchangeName { get; set; }
		/// <summary>
		/// 交换机类型cp
		/// </summary>
		public string ExchangeType { get; set; }
		/// <summary>
		/// 交换机是否持久化cp
		/// </summary>
		public bool ExchangeDurable { get; set; }
		/// <summary>
		/// 交换机是否自动删除cp
		/// </summary>
		public bool ExchangeAutoDelete { get;set;}
		/// <summary>
		/// 队列名cp
		/// </summary>
		public string QueueName { get; set; }
		/// <summary>
		/// 队列是否持久化cp
		/// </summary>
		public bool QueueDurable { get; set; }
		/// <summary>
		/// 是否是排他队列cp
		/// </summary>
		public bool QueueExclusive { get; set; }
		/// <summary>
		/// 队列是否自动删除cp
		/// </summary>
		public bool QueueAutoDelete { get; set; }
		/// <summary>
		/// 是否自动应答c
		/// </summary>
		public bool QueueAutoAck { get; set; }
		/// <summary>
		/// 路由地址p
		/// </summary>
		public string RoutingKey { get; set; }

		/// <summary>
		/// 重试消息的转发器
		/// </summary>
		public string RetryExchangeName { get; set; }
		/// <summary>
		/// 重试消息的队列
		/// </summary>
		public string RetryQueueName { get; set; }
		/// <summary>
		/// 失败消息的转发器
		/// </summary>
		public string FailExchangeName { get; set; }
		/// <summary>
		/// 失败消息的队列
		/// </summary>
		public string FailQueueName { get; set; }
	}
}
