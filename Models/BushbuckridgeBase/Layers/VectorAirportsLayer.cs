using BushbuckridgeBase.Agents;
using Mars.Components.Layers;
using Mars.Interfaces.Environments;

namespace BushbuckridgeBase.Layers
{
    // The VectorAirportLayer hods a set of Airport objects that agents can query and interact with.
    public class VectorAirportsLayer : VectorLayer<Airport>
    {
        // This method enables an agent to query the layer to locate the airport that is closest to its position.
        public Airport Nearest(Position position)
        {
            return Nearest(position.PositionArray);
        }
    }
}