using DiGi.Geometry.Planar.Classes;
using GeoAPI.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Reprojects a polygonal face to EPSG:2180, keeping its external edge and every internal edge.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The face travels as a NetTopologySuite polygon: <see cref="PolygonalFace2D"/> exposes no public way to build a face from rings - its ring constructors are internal to DiGi.Geometry - and the public <c>DiGi.Geometry.Planar.Create</c> factory runs a bounding box, containment and area pass per hole and silently drops every hole that fails containment, which costs time per footprint and breaks the requirement that the ring count survives conversion.
        /// </para>
        /// <para>
        /// <see cref="PolygonalFace2D.ExternalEdge"/> and <see cref="PolygonalFace2D.InternalEdges"/> clone through JSON on every read, so this path pays that clone once per face. Where a run converts many footprints, converting the source reader's NetTopologySuite geometry with <see cref="ToEPSG2180(Polygon, ICoordinateTransformation)"/> and building the face once afterwards is the cheaper order.
        /// </para>
        /// </remarks>
        /// <param name="polygonalFace2D">The source face.</param>
        /// <param name="coordinateTransformation">A transformation from <see cref="Create.EPSG2180TransformationFromEPSG4326"/> or <see cref="Create.EPSG2180TransformationFromEPSG3035"/>.</param>
        /// <returns>The face in EPSG:2180 with the same number of holes, or <see langword="null"/>.</returns>
        public static PolygonalFace2D? ToEPSG2180(this PolygonalFace2D? polygonalFace2D, ICoordinateTransformation? coordinateTransformation)
        {
            if (polygonalFace2D == null)
            {
                return null;
            }

            if (DiGi.Geometry.Planar.Convert.ToNTS(polygonalFace2D) is not Polygon polygon)
            {
                return null;
            }

            if (ToEPSG2180(polygon, coordinateTransformation) is not Polygon polygon_EPSG2180)
            {
                return null;
            }

            return DiGi.Geometry.Planar.Convert.ToDiGi(polygon_EPSG2180);
        }
    }
}
