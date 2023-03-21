using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace BushbuckridgeBase.Agents
{
    /// <summary>
    ///     This class represents an airport on the airport vector layer,
    ///     enabling agents to use it as entry and exit gateway.
    /// </summary>
    public class Airport : IVectorFeature
    {
        public Position Position { get; private set; }
        public VectorStructuredData VectorStructured { get; set; }

        /// <summary>
        ///     Obtain geographic position from external
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