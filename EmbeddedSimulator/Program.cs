using CamperVan.Models;

var rabbitClient = new CamperVan.RabbitMQService();

var airConditionSender = Task.Run(() =>
{
    rabbitClient.Server<AirCondition>("airCondition");
});

var ConsumptionSender = Task.Run(() =>
{
    rabbitClient.Server<Consumption>("consumption");
});

var EnergySender = Task.Run(() =>
{
    rabbitClient.Server<Energy>("energy");
});

var GasSender = Task.Run(() =>
{
    rabbitClient.Server<Gas>("gas");
});

var HeatingSender = Task.Run(() =>
{
    rabbitClient.Server<Heating>("heating");
});

var solarPanelSender = Task.Run(() =>
{
    rabbitClient.Server<SolarPanel>("solarPanel");
});

var waterSender = Task.Run(() =>
{
    rabbitClient.Server<Water>("water");
});

var weatherSender = Task.Run(() =>
{
    rabbitClient.Server<Weather>("weather");
});

var awaiter = Task.Run(() =>
{
    while (true)
    {
        Thread.Sleep(10000);
    }
});

await airConditionSender;
await ConsumptionSender;
await EnergySender;
await GasSender;
await HeatingSender;
await solarPanelSender;
await waterSender;
await weatherSender;
await awaiter;
