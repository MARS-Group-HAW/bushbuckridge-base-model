using System;
using System.Collections.Generic;
using System.Linq;
using BushbuckridgeBase.Agents;
using Mars.Common.Core.Logging;
using Mars.Common.IO;
using Mars.Components.Environments;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using SOHDomain.Graph;
using SOHMultimodalModel.Multimodal;

namespace BushbuckridgeBase.Layers
{
    public class ResidentLayer : AbstractMultimodalLayer
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(ResidentLayer));

        // **********************************
        // Properties relevant to agents and entities:
        // **********************************

        /// <summary>
        ///     Collection storing all agents that have been registered and spawned on the layer
        /// </summary>
        public IDictionary<Guid, Resident> ResidentMap { get; set; }

        // **********************************
        // References to travel networks that are associated with the resident layer:
        // **********************************

        /// <summary>
        ///     Network environment mapping the Bushbuckridge municipality's pedestrian sidewalks and roads
        /// </summary>
        [PropertyDescription]
        public SpatialGraphMediatorLayer TravelEnvironment { get; set; }

        // A layer's initLayer method serves to register agents (in this case, Resident) and entities
        // (in this case, Car) that reside and move on the layer.
        public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
            UnregisterAgent unregisterAgentHandle = null)
        {
            // call the super class's InitLayer method to register agents on the layer
            base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

            // spawn agents on layer and store them in ResidentMap collection
            var agentManager = layerInitData.Container.Resolve<IAgentManager>();
            
            ResidentMap = agentManager.Spawn<Resident, ResidentLayer>().ToDictionary(resident => resident.ID);

            Logger.LogInfo("Created Agents: " + ResidentMap.Count);

            return ResidentMap.Count != 0;
        }

        public ISpatialNode GetRandomNode()
        {
            return TravelEnvironment.Environment.GetRandomNode();
        }

        public IEnumerable<ISpatialNode> GetNodesWithinRange(Position position, int radiusInMeter)
        {
            return TravelEnvironment.Environment.NearestNodes(position, radiusInMeter);
        }
    }
}