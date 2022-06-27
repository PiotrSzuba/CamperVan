using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamperVan.Migrations
{
    public partial class db_init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirCondition",
                columns: table => new
                {
                    AirConditionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainAirConditionStatus = table.Column<bool>(type: "bit", nullable: false),
                    MainAirConditionPower = table.Column<int>(type: "int", nullable: false),
                    MainAirConditionTemperature = table.Column<int>(type: "int", nullable: false),
                    BathroomAirConditionStatus = table.Column<bool>(type: "bit", nullable: false),
                    BathroomAirConditionPower = table.Column<int>(type: "int", nullable: false),
                    BathroomConditionTemperature = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirCondition", x => x.AirConditionId);
                });

            migrationBuilder.CreateTable(
                name: "Consumption",
                columns: table => new
                {
                    ConsumptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Total = table.Column<int>(type: "int", nullable: false),
                    Heating = table.Column<int>(type: "int", nullable: false),
                    Boiler = table.Column<int>(type: "int", nullable: false),
                    Fridge = table.Column<int>(type: "int", nullable: false),
                    Lights = table.Column<int>(type: "int", nullable: false),
                    Other = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumption", x => x.ConsumptionId);
                });

            migrationBuilder.CreateTable(
                name: "Energie",
                columns: table => new
                {
                    EnergyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alternator = table.Column<bool>(type: "bit", nullable: false),
                    ExternalPower = table.Column<bool>(type: "bit", nullable: false),
                    Converter = table.Column<bool>(type: "bit", nullable: false),
                    Solar = table.Column<bool>(type: "bit", nullable: false),
                    BatteryLevel = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Energie", x => x.EnergyId);
                });

            migrationBuilder.CreateTable(
                name: "Gas",
                columns: table => new
                {
                    GasId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Volume = table.Column<int>(type: "int", nullable: false),
                    Valve = table.Column<bool>(type: "bit", nullable: false),
                    Boiler = table.Column<bool>(type: "bit", nullable: false),
                    Fridge = table.Column<bool>(type: "bit", nullable: false),
                    Stove = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gas", x => x.GasId);
                });

            migrationBuilder.CreateTable(
                name: "Heating",
                columns: table => new
                {
                    HeatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndoorTemp = table.Column<int>(type: "int", nullable: false),
                    OutdoorTemp = table.Column<int>(type: "int", nullable: false),
                    FuelHeating = table.Column<bool>(type: "bit", nullable: false),
                    ElectricHeating = table.Column<bool>(type: "bit", nullable: false),
                    FloorHeating = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heating", x => x.HeatingId);
                });

            migrationBuilder.CreateTable(
                name: "Leds",
                columns: table => new
                {
                    LedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Brightness = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leds", x => x.LedId);
                });

            migrationBuilder.CreateTable(
                name: "Lights",
                columns: table => new
                {
                    LightsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lights", x => x.LightsId);
                });

            migrationBuilder.CreateTable(
                name: "Modes",
                columns: table => new
                {
                    ModeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeatSaving = table.Column<bool>(type: "bit", nullable: false),
                    DimLights = table.Column<bool>(type: "bit", nullable: false),
                    ColdBoiler = table.Column<bool>(type: "bit", nullable: false),
                    Cameras = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modes", x => x.ModeId);
                });

            migrationBuilder.CreateTable(
                name: "SolarPanel",
                columns: table => new
                {
                    SolarPanelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Voltage = table.Column<int>(type: "int", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarPanel", x => x.SolarPanelId);
                });

            migrationBuilder.CreateTable(
                name: "Water",
                columns: table => new
                {
                    WaterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CleanTank = table.Column<int>(type: "int", nullable: false),
                    WasteTank = table.Column<int>(type: "int", nullable: false),
                    Pump = table.Column<bool>(type: "bit", nullable: false),
                    Sink = table.Column<bool>(type: "bit", nullable: false),
                    Shower = table.Column<bool>(type: "bit", nullable: false),
                    Toilet = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Water", x => x.WaterId);
                });

            migrationBuilder.CreateTable(
                name: "Weather",
                columns: table => new
                {
                    WeatherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Temperature = table.Column<int>(type: "int", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weather", x => x.WeatherId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirCondition");

            migrationBuilder.DropTable(
                name: "Consumption");

            migrationBuilder.DropTable(
                name: "Energie");

            migrationBuilder.DropTable(
                name: "Gas");

            migrationBuilder.DropTable(
                name: "Heating");

            migrationBuilder.DropTable(
                name: "Leds");

            migrationBuilder.DropTable(
                name: "Lights");

            migrationBuilder.DropTable(
                name: "Modes");

            migrationBuilder.DropTable(
                name: "SolarPanel");

            migrationBuilder.DropTable(
                name: "Water");

            migrationBuilder.DropTable(
                name: "Weather");
        }
    }
}
