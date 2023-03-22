using Mars.Common.Core;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace BushbuckridgeBase.Agents;

/// <summary>
/// This class represents a river on the river vector layer.
/// </summary>
public class River : IVectorFeature
{
    /// <summary>
    /// The pair of geocoordinates that represents the position of the river.
    /// </summary>
    public Position Position { get; private set; }

    /// <summary>
    /// The name of the river, obtained from geospatial external data).
    /// </summary>
    public string Name { get; set; }

    public VectorStructuredData VectorStructured { get; set; }

    /// <summary>
    /// The initialization method of the River, used to obtain the position and name of the river from external
    /// data.
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