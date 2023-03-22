using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using BushbuckridgeBase.Agents;
using BushbuckridgeBase.Layers;
using Mars.Common.Core.Logging;
using Mars.Components.Starter;
using Mars.Core.Simulation;
using Mars.Interfaces.Model;
using SOHCarModel.Model;
using SOHCarModel.Parking;
using SOHDomain.Graph;
using VectorPoiLayer = SOHMultimodalModel.Layers.VectorPoiLayer;

namespace BushbuckridgeBaseBox
{
    internal static class Program
    {
        private static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("EN-US");

            // Turning logger on or off
            LoggerFactory.SetLogLevel(LogLevel.Info);

            // Register each layer at the runtime system
            var description = new ModelDescription();
            description.AddLayer<RasterPrecipitationLayer>();
            description.AddLayer<RasterTemperatureMaxLayer>();
            description.AddLayer<RasterTemperatureMinLayer>();
            description.AddLayer<RasterVegetationLayer>();
            description.AddLayer<SpatialGraphMediatorLayer>(new[] { typeof(ISpatialGraphLayer) });
            description.AddLayer<VectorWaterLayer>();
            description.AddLayer<VectorAirportsLayer>();
            description.AddLayer<VectorPoiLayer>();
            description.AddLayer<VectorLandfillLayer>();
            description.AddLayer<CarParkingLayer>();
            description.AddLayer<ResidentLayer>();

            // Add entities
            description.AddEntity<Car>();

            // Register agent types with their respective layer type
            description.AddAgent<Resident, ResidentLayer>();

            // load config file to initialize simulation and obtain external data (georeferenced and time series)
            var file = File.ReadAllText("config.json");
            var simConfig = SimulationConfig.Deserialize(file);
            var application = SimulationStarter.BuildApplication(description, simConfig);

            var simulation = application.Resolve<ISimulation>();

            // start the simulation
            var watch = Stopwatch.StartNew();
            var state = simulation.StartSimulation();

            watch.Stop();

            Console.WriteLine($"Executed iterations {state.Iterations} lasted {watch.Elapsed}");
        }
    }
}