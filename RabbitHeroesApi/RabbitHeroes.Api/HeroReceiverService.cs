using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitHeroes.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitHeroesApi
{
    public class HeroReceiverService : BackgroundService
    {
        private IServiceProvider _sp;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        public HeroReceiverService(IServiceProvider sp)
        {
            _sp = sp;
            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "heroes", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _channel.Dispose();
                _connection.Dispose();

                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                
                Task.Run(() =>
                {
                    var chunks = message.Split("|");

                    var hero = new Hero();
                    if(chunks.Length == 7)
                    {
                        hero.Name = chunks[1];
                        hero.Powers = chunks[2];
                        hero.HasCape = chunks[3] == "1";
                        hero.IsAlive = chunks[5] == "1";
                        hero.Category = Enum.Parse<Category>(chunks[6]);
                    }

                    using (var scope = _sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<IHeroesRepository>();
                        db.Create(hero);
                    }
                });
            };

            _channel.BasicConsume(queue: "heroes", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
