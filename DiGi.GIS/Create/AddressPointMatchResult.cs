using DiGi.Geometry.Core.Enums;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Planar.Interfaces;
using DiGi.GIS.Classes;
using DiGi.GIS.Enums;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Create
    {
        /// <summary>
        /// Assigns PRG address points to the building footprints of one county and returns one <see cref="AddressPointMatch"/> per address point, in input order.
        /// <para>Rules: a point inside a footprint, or on its boundary within <paramref name="tolerance"/>, is assigned to it (<see cref="AddressPointMatchType.Inside"/>); otherwise it is assigned to the nearest footprint within <paramref name="maxDistance"/> (<see cref="AddressPointMatchType.Nearest"/>); otherwise, or when it has no location, it is reported as <see cref="AddressPointMatchType.Unmatched"/>. A point belongs to at most one building; a building receives any number of points.</para>
        /// <para>Ties are deterministic and independent of input order: when the point is inside several (overlapping) footprints, or several footprints lie at distances equal within <paramref name="tolerance"/>, the building with the ordinally smallest <see cref="GISGuidObject2D.Reference"/> wins, then the smallest guid.</para>
        /// <para>The footprints are indexed in an NTS <see cref="STRtree{TItem}"/> built inside the call; nothing is cached between calls.</para>
        /// </summary>
        /// <param name="addressPoints">The address points to assign. Null items are skipped.</param>
        /// <param name="building2Ds">The building footprints, in the same coordinate system as the points (EPSG:2180). Null buildings and buildings without a footprint are skipped.</param>
        /// <param name="maxDistance">The maximum distance, in m, from a point outside every footprint to the footprint it is assigned to. The default is 10 m: PRG entrance points usually sit just outside the wall, and 10 m was kept as the confirmed default (DiGi.GIS.PostgreSQL#104) rather than re-measured on a county import.</param>
        /// <param name="tolerance">The distance tolerance, in m, for the boundary test and for ties.</param>
        /// <returns>The matches, or null when <paramref name="addressPoints"/> is null or <paramref name="maxDistance"/> is negative or not finite.</returns>
        public static AddressPointMatchResult? AddressPointMatchResult(this IEnumerable<AddressPoint?>? addressPoints, IEnumerable<Building2D?>? building2Ds, double maxDistance = 10, double tolerance = Core.Constants.Tolerance.Distance)
        {
            if (addressPoints == null || double.IsNaN(maxDistance) || double.IsInfinity(maxDistance) || maxDistance < 0)
            {
                return null;
            }

            List<Building2D> building2Ds_Valid = [];
            List<PolygonalFace2D> polygonalFace2Ds = [];
            List<BoundingBox2D> boundingBox2Ds = [];

            if (building2Ds != null)
            {
                foreach (Building2D? building2D in building2Ds)
                {
                    // PolygonalFace2D returns a clone on every access, so it is read once per building.
                    PolygonalFace2D? polygonalFace2D = building2D?.PolygonalFace2D;
                    if (building2D == null || polygonalFace2D == null)
                    {
                        continue;
                    }

                    BoundingBox2D? boundingBox2D = polygonalFace2D.GetBoundingBox();
                    if (boundingBox2D == null)
                    {
                        continue;
                    }

                    building2Ds_Valid.Add(building2D);
                    polygonalFace2Ds.Add(polygonalFace2D);
                    boundingBox2Ds.Add(boundingBox2D);
                }
            }

            STRtree<int> sTRtree = new();
            for (int i = 0; i < boundingBox2Ds.Count; i++)
            {
                sTRtree.Insert(new Envelope(boundingBox2Ds[i].Min.X, boundingBox2Ds[i].Max.X, boundingBox2Ds[i].Min.Y, boundingBox2Ds[i].Max.Y), i);
            }

            sTRtree.Build();

            // Built on first use within this call only: most footprints are never a candidate of any point.
            PolygonalFace2DPointRelationSolver?[] polygonalFace2DPointRelationSolvers = new PolygonalFace2DPointRelationSolver?[polygonalFace2Ds.Count];
            List<Segment2D>?[] segment2Ds = new List<Segment2D>?[polygonalFace2Ds.Count];

            PointRelation Relation(int index, Point2D point2D)
            {
                PolygonalFace2DPointRelationSolver? polygonalFace2DPointRelationSolver = polygonalFace2DPointRelationSolvers[index];
                if (polygonalFace2DPointRelationSolver == null)
                {
                    polygonalFace2DPointRelationSolver = new PolygonalFace2DPointRelationSolver(polygonalFace2Ds[index], tolerance);
                    polygonalFace2DPointRelationSolvers[index] = polygonalFace2DPointRelationSolver;
                }

                polygonalFace2DPointRelationSolver.Input = point2D;
                return polygonalFace2DPointRelationSolver.Solve() ? polygonalFace2DPointRelationSolver.Output : PointRelation.Undefined;
            }

            // Distance to the nearest ring: for a point outside the outline that is the outline, for a point in a courtyard it is the courtyard ring.
            double RingDistance(int index, Point2D point2D)
            {
                List<Segment2D>? segment2Ds_Building = segment2Ds[index];
                if (segment2Ds_Building == null)
                {
                    segment2Ds_Building = [];

                    PolygonalFace2D polygonalFace2D = polygonalFace2Ds[index];

                    List<IPolygonal2D> polygonal2Ds = [];
                    if (polygonalFace2D.ExternalEdge is IPolygonal2D polygonal2D_External)
                    {
                        polygonal2Ds.Add(polygonal2D_External);
                    }

                    if (polygonalFace2D.InternalEdges is List<IPolygonal2D> polygonal2Ds_Internal)
                    {
                        polygonal2Ds.AddRange(polygonal2Ds_Internal);
                    }

                    foreach (IPolygonal2D polygonal2D in polygonal2Ds)
                    {
                        if (polygonal2D.GetSegments() is List<Segment2D> segment2Ds_Polygonal2D)
                        {
                            segment2Ds_Building.AddRange(segment2Ds_Polygonal2D);
                        }
                    }

                    segment2Ds[index] = segment2Ds_Building;
                }

                double result = double.MaxValue;
                foreach (Segment2D segment2D in segment2Ds_Building)
                {
                    double distance = segment2D.Distance(point2D);
                    if (distance < result)
                    {
                        result = distance;
                    }
                }

                return result;
            }

            // True when building index_1 wins the tie against index_2: ordinal reference, then guid.
            bool Precedes(int index_1, int index_2)
            {
                int comparison = string.CompareOrdinal(building2Ds_Valid[index_1].Reference, building2Ds_Valid[index_2].Reference);
                if (comparison != 0)
                {
                    return comparison < 0;
                }

                return building2Ds_Valid[index_1].Guid.CompareTo(building2Ds_Valid[index_2].Guid) < 0;
            }

            double searchDistance = Math.Max(maxDistance, tolerance);

            List<AddressPointMatch> addressPointMatches = [];
            foreach (AddressPoint? addressPoint in addressPoints)
            {
                if (addressPoint == null)
                {
                    continue;
                }

                Point2D? point2D = addressPoint.Location;
                if (point2D == null)
                {
                    addressPointMatches.Add(new AddressPointMatch(addressPoint, AddressPointMatchType.Unmatched, null, null, null));
                    continue;
                }

                Envelope envelope = new(point2D.X - searchDistance, point2D.X + searchDistance, point2D.Y - searchDistance, point2D.Y + searchDistance);

                int index_Inside = -1;
                int index_Nearest = -1;
                double distance_Nearest = double.MaxValue;

                foreach (int index in sTRtree.Query(envelope))
                {
                    PointRelation pointRelation = Relation(index, point2D);
                    if (pointRelation == PointRelation.Inside || pointRelation == PointRelation.On)
                    {
                        if (index_Inside == -1 || Precedes(index, index_Inside))
                        {
                            index_Inside = index;
                        }

                        continue;
                    }

                    if (index_Inside != -1)
                    {
                        continue;
                    }

                    double distance = RingDistance(index, point2D);
                    if (distance > maxDistance)
                    {
                        continue;
                    }

                    if (index_Nearest == -1 || distance < distance_Nearest - tolerance || (Math.Abs(distance - distance_Nearest) <= tolerance && Precedes(index, index_Nearest)))
                    {
                        index_Nearest = index;
                        distance_Nearest = distance;
                    }
                }

                if (index_Inside != -1)
                {
                    Building2D building2D = building2Ds_Valid[index_Inside];
                    addressPointMatches.Add(new AddressPointMatch(addressPoint, AddressPointMatchType.Inside, building2D.Reference, building2D.Guid, 0));
                }
                else if (index_Nearest != -1)
                {
                    Building2D building2D = building2Ds_Valid[index_Nearest];
                    addressPointMatches.Add(new AddressPointMatch(addressPoint, AddressPointMatchType.Nearest, building2D.Reference, building2D.Guid, distance_Nearest));
                }
                else
                {
                    addressPointMatches.Add(new AddressPointMatch(addressPoint, AddressPointMatchType.Unmatched, null, null, null));
                }
            }

            return new AddressPointMatchResult(maxDistance, tolerance, addressPointMatches);
        }
    }
}
