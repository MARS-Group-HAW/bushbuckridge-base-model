using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace BushbuckridgeBase.Agents
{
    /// <summary>
    /// This class represents an airport on the airport vector layer, enabling agents to use it as entry and exit
    /// gateway.
    /// </summary>
    public class Airport : IVectorFeature
    {
        /// <summary>
        /// The pair of geocoordinates that represents the position of the airport.
        /// </summary>
        public Position Position { get; private set; }
        
        /// <summary>
        /// The initialization data passed to this object upon initialization.
        /// </summary>
        public VectorStructuredData VectorStructured { get; set; }

        /// <summary>
        /// The initialization method of the Airport, used to obtain the position of the airport from external data.
        /// </summary>
        /// <param name="layer">The managing layer</param>
        /// <param name="data">The external data object representing this airport</param>
        public void Init(ILayer layer, VectorStructuredData data)
        {
            var centroid = data.Geometry.Centroid;
            Position = Position.CreatePosition(centroid.X, centroid.Y);
            VectorStructured = data;
        }

        public void Update(VectorStructuredData data)
        {
            //do nothing
        }
    }
}