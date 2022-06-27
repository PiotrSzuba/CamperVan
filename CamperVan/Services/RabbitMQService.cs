using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace CamperVan;

public class RabbitMQService : IDisposable
{
    private readonly string _hostName = "localhost";

    private IConnection? connection;
    private IModel? channel;
    public Dictionary<string, string> Data = new();
    public RabbitMQService()
    {
        connection = GetRabbitMQConnection();
        if (connection == null)
        {
            return;
        }
        channel = connection.CreateModel();
    }

    private IConnection? GetRabbitMQConnection()
    {
        ConnectionFactory connectionFactory = new()
        {
            HostName = _hostName,
            AutomaticRecoveryEnabled = true
        };
        try
        {
            return connectionFactory.CreateConnection();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string? SendData(string topic, string message)
    {
        if (connection == null || channel == null)
        {
            return null;
        }
        var body = Encoding.UTF8.GetBytes(message);
        try
        {
            channel.ExchangeDeclare(exchange: topic, type: ExchangeType.Topic);
            channel.BasicPublish(exchange: topic, routingKey: "", basicProperties: null, body: body);
        }
        catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
        {
            return null;
        }

        return message;
    }

    public void Client(string topic)
    {
        if (channel == null)
        {
            return;
        }
        try
        {
            channel.ExchangeDeclare(exchange: topic, type: ExchangeType.Topic);
            channel.QueueDeclare(queue: topic, autoDelete: true);
            channel.QueueBind(queue: topic, exchange: topic, routingKey: "");
        }
        catch { }
        if (!Data.ContainsKey(topic))
        {
            lock (Data)
            {
                Data.Add(topic, "");
            }
        }
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            lock(Data)
            {
                Data[topic] = response;
            }
        };
        channel.BasicConsume(consumer: consumer, queue: topic, autoAck: true);
    }

    public async Task<string> CallServer(string topic, string message)
    {
        if (channel == null)
        {
            return "";
        }
        var serverTopic = topic + "_server";
        var body = Encoding.UTF8.GetBytes(message);
        try
        {
            channel.ExchangeDeclare(exchange: serverTopic, type: ExchangeType.Topic);
        }
        catch {}

        if (!Data.ContainsKey(topic))
        {
            lock (Data)
            {
                Data.Add(topic, "");
            }
        }

        lock (Data)
        {
            Data[topic] = "";
        }
        channel.BasicPublish(exchange: serverTopic, routingKey: "", basicProperties: null, body: body);

        var res = "";

        var getData = Task.Run(() =>
        {
            var timer = new Stopwatch();
            timer.Start();
            while (Data[topic].Length == 0 || Data[topic] == null)
            {
            }
            timer.Stop();
        });
        await getData;

        lock (Data)
        {
            res = Data[topic];
        }

        return res;
    }

    public void Server<T>(string topic)
    {
        if (channel == null)
        {
            return;
        }
        var serverTopic = topic + "_server";
        channel.ExchangeDeclare(exchange: topic, type: ExchangeType.Topic);
        channel.ExchangeDeclare(exchange: serverTopic, type: ExchangeType.Topic);
        channel.QueueDeclare(queue: serverTopic);
        channel.QueueBind(queue: serverTopic, exchange: serverTopic, routingKey: "");
        channel.BasicQos(0, 1, false);
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue: serverTopic, autoAck: false, consumer: consumer);

        consumer.Received += (model, ea) =>
        {
            string? response = null;

            var body = ea.Body.ToArray();
            try
            {
                var message = Encoding.UTF8.GetString(body);
                T? data = JsonSerializer.Deserialize<T>(message);
                response = JsonSerializer.Serialize(data);
                Console.WriteLine($"Embedded got: {typeof(T).Name} {response}");
            }
            catch
            {
                response = "";
            }
            finally
            {
                if (response == null)
                {
                    response = "";
                }
                var responseBytes = Encoding.UTF8.GetBytes(response);
                channel.BasicPublish(exchange: topic, routingKey: "", basicProperties: null, body: responseBytes);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };
    }

    public void Dispose()
    {
        if(channel != null)
        {
            channel.Close();
            channel.Dispose();
        }
        if(connection != null)
        {
            connection.Close();
            connection.Dispose();
        }
    }
}
