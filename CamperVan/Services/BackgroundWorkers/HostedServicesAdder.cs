using CamperVan.Models;
namespace CamperVan.Services.BackgroundWorkers;

public static class HostedServicesAdder
{
    public static void AddLocalHostedServices(this IServiceCollection service)
    {
        service.AddSingleton<GenericHostedService<AirCondition>>();
        service.AddHostedService(provider => provider.GetService<GenericHostedService<AirCondition>>()!);

        service.AddSingleton<GenericHostedService<Consumption>>();
        service.AddHostedService(provider => provider.GetService<GenericHostedService<Consumption>>()!);

        service.AddSingleton<GenericHostedService<Energy>>();
        service.AddHostedService(provider => provider.GetService<GenericHostedService<Energy>>()!);

        service.AddSingleton<GenericHostedService<Gas>>();
        service.AddHostedService(provider => provider.GetService<GenericHostedService<Gas>>()!);

        service.AddSingleton<GenericHostedService<Heating>>();
        service.AddHostedService(provider => provider.GetService<GenericHostedService<Heating>>()!);

        service.AddSingleton<GenericHostedService<SolarPanel>>();
        service.AddHostedService(provider => provider.GetService<GenericHostedService<SolarPanel>>()!);

        service.AddSingleton<GenericHostedService<Water>>();
        service.AddHostedService(provider => provider.GetService<GenericHostedService<Water>>()!);

        service.AddSingleton<GenericHostedService<Weather>>();
        service.AddHostedService(provider => provider.GetService<GenericHostedService<Weather>>()!);
    }
}
