using BushbuckridgeBase.Agents;
using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace BushbuckridgeBase.Layers;

/// <summary>
/// A vector layer that holds Airport features.
/// </summary>
public class VectorAirportsLayer : VectorLayer<Airport>
{
    /// <summary>
    /// Returns the Airport that is nearest to the given position.
    /// </summary>
    /// <param name="position">The given position</param>
    /// <returns>The identified airport</returns>
    public Airport Nearest(Position position)
    {
        return Nearest(position.PositionArray);
    }
}