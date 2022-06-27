using System.Text.Json;
using System;

namespace CamperVan.Services.BackgroundWorkers;

public class GenericHostedService<T> : IHostedService, IDisposable, IHostedServiceWorker<T>
{
    public RabbitMQService RabbitClient { get; set; }
    public string Topic { get; set; }

    public GenericHostedService(IServiceProvider serviceProvider)
    {
        RabbitClient = serviceProvider.GetRequiredService<RabbitMQService>();
        Topic = typeof(T).Name;
        Topic = string.Concat(Topic[..1].ToLower(), Topic.AsSpan(1));
    }
    public T? GetData()
    {
        if (!RabbitClient.Data.ContainsKey(Topic))
        {
            return default;
        }

        if (RabbitClient.Data[Topic].Length == 0)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(RabbitClient.Data[Topic]);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => RabbitClient.Client(Topic), cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
