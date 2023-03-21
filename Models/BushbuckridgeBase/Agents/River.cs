using Mars.Common.Core;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace BushbuckridgeBase.Agents
{
    public class River : IVectorFeature
    {
        public Position Position { get; private set; }

        /// <summary>
        ///     river's name (obtained from geospatial external data (see above)
        /// </summary>
        public string Name { get; set; }

        public VectorStructuredData VectorStructured { get; set; }

        /// <summary>
        ///     Initialize the river object with an input data object
        /// </summary>
        /// <param name="layer">The managing layer</param>
        /// <param name="data">The external data object</param>
        public void Init(ILayer layer, VectorStructuredData data)
        {
            var centroid = data.Geometry.Centroid;
            Position = Position.CreatePosition(centroid.X, centroid.Y);
            VectorStructured = data;
            // retrieve the value of the key "name" from the vector feature
            // and, if it exists, assign it to the property Name
            Name = data.Attributes.Exists("name") ? data.Attributes["name"].Value<string>() : "na";
        }

        public void Update(VectorStructuredData data)
        {
            //do nothing
        }
    }
}