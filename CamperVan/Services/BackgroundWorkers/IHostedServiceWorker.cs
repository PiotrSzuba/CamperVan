using CamperVan.Models;
using System.Text.Json;

namespace CamperVan.Services.BackgroundWorkers;

public interface IHostedServiceWorker<T>
{

    public T? GetData();
}
