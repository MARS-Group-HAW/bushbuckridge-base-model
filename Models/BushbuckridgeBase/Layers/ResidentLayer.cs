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

namespace BushbuckridgeBase.Layers;

/// <summary>
/// An abstract layer responsible for initializing and managing Resident agents during the simulation.
/// </summary>
public class ResidentLayer : AbstractMultimodalLayer
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger(typeof(ResidentLayer));

    // **********************************
    // Properties relevant to agents and entities:
    // **********************************

    /// <summary>
    /// Collection storing all agents that have been registered and spawned on the layer.
    /// </summary>
    public IDictionary<Guid, Resident> ResidentMap { get; set; }

    // **********************************
    // References to travel networks that are associated with the resident layer:
    // **********************************

    /// <summary>
    /// A network environment containing the Bushbuckridge municipality's pedestrian sidewalks and roads.
    /// </summary>
    [PropertyDescription]
    public SpatialGraphMediatorLayer TravelEnvironment { get; set; }

    /// <summary>
    /// The initialization method of this layer type, responsible for registering agents (in this case, Resident)
    /// and entities (in this case, Car) that reside and move on the layer.
    /// </summary>
    /// <param name="layerInitData">External initialization data passed to the layer for further processing</param>
    /// <param name="registerAgentHandle">A handle for registering agents in the simulation and enabling them to
    /// participate in it</param>
    /// <param name="unregisterAgentHandle">A handle for unregistering agents and removing them from the
    /// simulation</param>
    /// <returns>A boolean that states if layer initialization was successful</returns>
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

    /// <summary>
    /// Returns a random node of the travel environment.
    /// </summary>
    /// <returns></returns>
    public ISpatialNode GetRandomNode()
    {
        return TravelEnvironment.Environment.GetRandomNode();
    }

    /// <summary>
    /// Returns a collection of nodes that are within the given radius (in meters) from the given position.
    /// </summary>
    /// <param name="position">The given position</param>
    /// <param name="radiusInMeter">The given radius in meters</param>
    /// <returns>A collection of identified nodes</returns>
    public IEnumerable<ISpatialNode> GetNodesWithinRange(Position position, int radiusInMeter)
    {
        return TravelEnvironment.Environment.NearestNodes(position, radiusInMeter);
    }
}