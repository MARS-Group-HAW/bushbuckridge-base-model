using System;
using System.Linq;
using BushbuckridgeBase.Layers;
using BushbuckridgeBase.Misc;
using Mars.Interfaces.Annotations;
using SOHMultimodalModel.Model;

namespace BushbuckridgeBase.Agents
{
    // The agent type Resident is a resident of the Bushbuckridge municipality that can pursue different activities
    // during the simulation. It inherits its multi-capable abilities (being able to travel on foot and by drive) from
    // the MultiCapableAgent.
    // The agent's structure consists of an Init method, a Tick method, and several properties (attributes).
    public class Resident : MultiCapableAgent<ResidentLayer>
    {
        // **********************************
        // General properties for agents:
        // **********************************

        // Stores agent's current activity state (this property is relevant for "picnicking" agents only)
        public Enum State { get; set; }

        // An agent's planned activity (obtained from agent initialization file)
        [PropertyDescription(Name = "activity")]
        public string Activity { get; set; }

        // **********************************
        // Properties for picnicking agents:
        // **********************************

        // An agent's arrival time at its picnic location (this property is relevant for "picnicking" agents only)
        public DateTime ArrivalTime { get; set; }

        // An agent's picnic duration is obtained from an agent initialization file (at the beginning of the
        // simulation) and is stored in PicnicDuration.
        [PropertyDescription(Name = "picnicDuration")]
        public int PicnicDuration { get; set; }
        
        [PropertyDescription(Name = "maxTravelDistanceInMeters")]
        public int MaxTravelDistanceInMeters { get; set; }

        // An agent's departure time from its picnic location (this property is relevant for "picnicking" agents only)
        public DateTime DepartureTime { get; set; }

        // **********************************
        // Layer references:
        // **********************************

        // An agent's reference to the resident layer on which it resides (obtained from configuration files)
        [PropertyDescription] public ResidentLayer ResidentLayer { get; set; }

        // An agent's reference to the local precipitation rate (obtained from configuration files)
        [PropertyDescription] public RasterPrecipitationLayer RasterPrecipitationLayer { get; set; }

        // An agent's reference to the region's waterways (obtained from configuration files)
        [PropertyDescription] public VectorWaterLayer VectorWaterLayer { get; set; }

        // An agent's reference to the region's airports (obtained from configuration files)
        [PropertyDescription] public VectorAirportsLayer VectorAirportsLayer { get; set; }

        // An agent's reference to the local minimum temperature (obtained from configuration files)
        [PropertyDescription] public RasterTemperatureMinLayer RasterTemperatureMinLayer { get; set; }
        
        // An agent's reference to the local maximum temperature (obtained from configuration files)
        [PropertyDescription] public RasterTemperatureMaxLayer RasterTemperatureMaxLayer { get; set; }

        // An agent's Init method is used to initialize the agent and equip it with all values and information
        // that it requires at the start of the simulation.
        public override void Init(ResidentLayer residentLayer)
        {
            // store reference to residentLayer in public property
            ResidentLayer = residentLayer;

            // assign a starting position to the resident at which it will be located at the beginning of the simulation
            StartPosition ??= residentLayer.GetRandomNode().Position;

            // call constructor of resident's layer to register agents and enable multimodal features (i.e., cars)
            base.Init(residentLayer);

            // In the resident initialization file, the attribute "activity" is used to assign a routine to each
            // resident which it will pursue during the simulation.

            // Activity 1:
            // Goal: make a multimodal trip from starting position to an airport
            // Process:
            // 1. Find the nearest airport.
            // 2. Set its location as your goal position.
            // 3. Plan a multimodal route from your start position to the goal position.
            // Technical aspects: interaction with a vector layer (VectorAirportsLayer) to find nearest node
            if (Activity == "_airport")
            {
                var nearestAirport = VectorAirportsLayer.Nearest(Position);
                var goalPosition = nearestAirport.Position;
                MultimodalRoute = ResidentLayer.RouteFinder.Search(
                    this, Position, goalPosition, Capabilities);
            }

            // Activity 2:
            // Goal: make a trip to the sand river
            // Process:
            // 1. Search all rivers in VectorWaterLayer to find one with name == "Sand River".
            // 2. Set goal position to river's position.
            // 3. Plan a multimodal route from your start position to the goal position.
            // Technical aspects: retrieved a vector feature (River) from a vector layer (VectorWaterLayer)
            // which has a specific property (name == "Sand River")
            else if (Activity == "_river")
            {
                foreach (var river in VectorWaterLayer.Features.OfType<River>())
                    if (river.Name == "Sand River")
                    {
                        var goalPosition = river.Position;
                        MultimodalRoute = ResidentLayer.RouteFinder.Search(
                            this, Position, goalPosition, Capabilities);
                    }
            }

            // Activity 3:
            // Goal: Make a picnic trip, depending on current amount of local precipitation
            // Process:
            // 1. Query the RasterPrecipitationLayer for precipitation amount at current position.
            // 2. If precipitation amount is not too high, search for picnic spots within a given radius (in meters).
            // 3. If a suitable place has been found, set its location to goal position and plan a multimodal trip.
            // 4. If no suitable place has been found, stay at home.
            // 5. The Enum type "State" indicates what part of the routine the agent is currently in.
            // Technical aspects:
            // - interacting with a raster layer (which holds time series data) and retrieving a cell value
            // - working with enum variables
            else if (Activity == "picnic")
            {
                var precipitation = RasterPrecipitationLayer.GetValue(Position);
                if (precipitation <= 500)
                {
                    var possiblePos = residentLayer.GetNodesWithinRange(Position, MaxTravelDistanceInMeters).ToList();
                    if (possiblePos.Count >= 2)
                    {
                        State = PicnicState.GoingToPlace;
                        var goalPosition = possiblePos[1].Position;
                        MultimodalRoute = ResidentLayer.RouteFinder.Search(
                            this, Position, goalPosition, Capabilities);
                    }
                    else
                    {
                        State = PicnicState.Home;
                    }
                }
                else
                {
                    State = PicnicState.Home;
                }
            }
        }

        // An agent's Tick method contains the behavioral routine that the agent carries out during each time step
        // of the simulation.
        public override void Tick()
        {
            // If current goal has not yet been reached, do the following:
            // 1. Make a move.
            // 2. Store current position in trips collection object (for visualization after simulation)
            if (!GoalReached)
            {
                Move();
            }
            // If goal has not been reached, step into else branch.
            else
            {
                // This section applies to agents who pursue a picnicking activity. These agents' routine consists of
                // making a round-trip between their starting position and their chosen picnic location.
                if (Activity == "picnic")
                {
                    // check if agent is currently going to picnic location
                    if (State.Equals(PicnicState.GoingToPlace))
                    {
                        // Agent has arrived. Change "State" to "Arrived" and set arrival time (time at which agent
                        // begins picnicking) and departure time (time at which agent stops picking and starts going
                        // home).
                        State = PicnicState.Arrived;
                        ArrivalTime = ResidentLayer.Context.CurrentTimePoint.GetValueOrDefault();
                        DepartureTime = ArrivalTime.AddMinutes(PicnicDuration);
                    }
                    // If agent is already at picnic location, check if it is time to go home.
                    else if (State.Equals(PicnicState.Arrived) && DepartureTime
                        .Subtract(ResidentLayer.Context.CurrentTimePoint.GetValueOrDefault())
                        .Minutes == 0)
                    {
                        // It is time to go home. Set "State" to "GoingToHome" and plan a multimodal trip from current
                        // position to home.
                        State = PicnicState.GoingToHome;
                        var goalPosition = StartPosition;
                        MultimodalRoute = ResidentLayer.RouteFinder.Search(
                            this, Position, goalPosition, Capabilities);
                    }

                    // Check if agent has arrived at home.
                    if (Position.Equals(StartPosition)) State = PicnicState.Home;
                }
            }
        }
    }
}