using DiGi.Core;
using DiGi.Core.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiGi.GIS
{
    public static partial class Create
    {
        /// <summary>
        /// Asynchronously retrieves orthophoto data for a specified building within a given range of years.
        /// </summary>
        /// <param name="building2D">The building object used to determine the area of interest.</param>
        /// <param name="years">The range of years for which to retrieve orthophoto data.</param>
        /// <param name="offset">The offset distance to expand the bounding box around the building.</param>
        /// <param name="width">The desired width of the resulting image data.</param>
        /// <param name="reduce">A value indicating whether to reduce the result set.</param>
        /// <param name="squared">A value indicating whether the requested area should be forced into a square shape.</param>
        /// <param name="ortoSheets">The orthophoto index sheets of the area, used to date every image by the sheet it shows (<see cref="OrtoData(int, byte[], double, BoundingBox2D, IEnumerable{OrtoSheet}, int?)"/>); null leaves the images undated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the OrtoDatas if successful; otherwise, null.</returns>
        public static async Task<OrtoDatas?> OrtoDatas(this Building2D? building2D, Range<int>? years, double offset = 5, double width = 320, bool reduce = true, bool squared = false, IEnumerable<OrtoSheet?>? ortoSheets = null)
        {
            if (building2D == null || years == null)
            {
                return null;
            }

            BoundingBox2D? boundingBox2D = building2D.PolygonalFace2D?.GetBoundingBox();
            if (boundingBox2D is null)
            {
                return null;
            }

            boundingBox2D.Offset(offset);

            if (squared && boundingBox2D.Width != boundingBox2D.Height)
            {
                double max_Temp = Math.Max(boundingBox2D.Width, boundingBox2D.Height);

                boundingBox2D = Geometry.Planar.Create.BoundingBox2D(boundingBox2D.GetCentroid(), max_Temp, max_Temp);
            }

            if (boundingBox2D is null)
            {
                return null;
            }

            double scale = width / boundingBox2D.Width;

            return await OrtoDatas(boundingBox2D, building2D.Reference, years, scale, reduce, ortoSheets);
        }

        /// <summary>
        /// Asynchronously retrieves orthophoto data for a specified building using a provided HttpClient and within a given range of years.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to perform the network requests.</param>
        /// <param name="building2D">The building object used to determine the area of interest.</param>
        /// <param name="years">The range of years for which to retrieve orthophoto data.</param>
        /// <param name="offset">The offset distance to expand the bounding box around the building.</param>
        /// <param name="width">The desired width of the resulting image data.</param>
        /// <param name="reduce">A value indicating whether to reduce the result set.</param>
        /// <param name="squared">A value indicating whether the requested area should be forced into a square shape.</param>
        /// <param name="initialRequestCount">The initial number of requests to perform when querying data.</param>
        /// <param name="ortoSheets">The orthophoto index sheets of the area, used to date every image by the sheet it shows (<see cref="OrtoData(int, byte[], double, BoundingBox2D, IEnumerable{OrtoSheet}, int?)"/>); null leaves the images undated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the OrtoDatas if successful; otherwise, null.</returns>
        public static async Task<OrtoDatas?> OrtoDatas(
            this HttpClient httpClient,
            Building2D? building2D,
            Range<int>? years,
            double offset = 5,
            double width = 320,
            bool reduce = true,
            bool squared = false,
            int initialRequestCount = 8,
            IEnumerable<OrtoSheet?>? ortoSheets = null)
        {
            if (building2D is null || years is null || httpClient is null)
            {
                return null;
            }

            BoundingBox2D? boundingBox2D = building2D.PolygonalFace2D?.GetBoundingBox();
            if (boundingBox2D is null)
            {
                return null;
            }

            boundingBox2D.Offset(offset);

            if (squared && boundingBox2D.Width != boundingBox2D.Height)
            {
                double max_Temp = Math.Max(boundingBox2D.Width, boundingBox2D.Height);

                boundingBox2D = Geometry.Planar.Create.BoundingBox2D(boundingBox2D.GetCentroid(), max_Temp, max_Temp);
            }

            if (boundingBox2D is null)
            {
                return null;
            }

            double scale = width / boundingBox2D.Width;

            return await OrtoDatas(httpClient, boundingBox2D, building2D.Reference, years, scale, reduce, initialRequestCount, ortoSheets);
        }

        /// <summary>
        /// Asynchronously retrieves orthophoto data based on an existing OrtoRange and a specified year range.
        /// </summary>
        /// <param name="ortoRange">The source range containing the bounding box and unique identifier.</param>
        /// <param name="years">The range of years for which to retrieve orthophoto data.</param>
        /// <param name="scale">The scale factor used for the image request.</param>
        /// <param name="reduce">A value indicating whether to reduce the result set.</param>
        /// <param name="squared">A value indicating whether the requested area should be forced into a square shape.</param>
        /// <param name="ortoSheets">The orthophoto index sheets of the area, used to date every image by the sheet it shows (<see cref="OrtoData(int, byte[], double, BoundingBox2D, IEnumerable{OrtoSheet}, int?)"/>); null leaves the images undated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the OrtoDatas if successful; otherwise, null.</returns>
        public static async Task<OrtoDatas?> OrtoDatas(this OrtoRange? ortoRange, Range<int>? years, double scale, bool reduce = true, bool squared = false, IEnumerable<OrtoSheet?>? ortoSheets = null)
        {
            if (double.IsNaN(scale))
            {
                return null;
            }

            BoundingBox2D? boundingBox2D = ortoRange?.BoundingBox2D;
            if (boundingBox2D == null)
            {
                return null;
            }

            if (squared && boundingBox2D.Width != boundingBox2D.Height)
            {
                double max_Temp = Math.Max(boundingBox2D.Width, boundingBox2D.Height);

                boundingBox2D = Geometry.Planar.Create.BoundingBox2D(boundingBox2D.GetCentroid(), max_Temp, max_Temp);
            }

            return await OrtoDatas(boundingBox2D, ortoRange?.UniqueId, years, scale, reduce, ortoSheets);
        }

        /// <summary>
        /// Asynchronously retrieves orthophoto data for a specific bounding box and year range.
        /// </summary>
        /// <param name="boundingBox2D">The geographic area to retrieve data for.</param>
        /// <param name="reference">A reference string associated with the requested data.</param>
        /// <param name="years">The range of years for which to retrieve orthophoto data.</param>
        /// <param name="scale">The scale factor used for the image request.</param>
        /// <param name="reduce">A value indicating whether to reduce the result set.</param>
        /// <param name="ortoSheets">The orthophoto index sheets of the area, used to date every image by the sheet it shows (<see cref="OrtoData(int, byte[], double, BoundingBox2D, IEnumerable{OrtoSheet}, int?)"/>); null leaves the images undated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the OrtoDatas if successful; otherwise, null.</returns>
        public static async Task<OrtoDatas?> OrtoDatas(this BoundingBox2D? boundingBox2D, string? reference, Range<int>? years, double scale, bool reduce = true, IEnumerable<OrtoSheet?>? ortoSheets = null)
        {
            if (boundingBox2D is null)
            {
                return null;
            }

            List<OrtoData> values = [];

            List<int>? yearsList = years.ToSystem(1);
            if (yearsList is null)
            {
                return null;
            }

            if (yearsList.Count != 0)
            {
                Dictionary<int, byte[]>? dictionary = await Query.BytesDictionary(boundingBox2D, yearsList, scale);
                if (dictionary != null && dictionary.Count != 0)
                {
                    Dictionary<int, int> lastYears = Query.ContentRunEndYears(dictionary)!;
                    foreach (KeyValuePair<int, byte[]> keyValuePair in dictionary)
                    {
                        OrtoData? value = OrtoData(keyValuePair.Key, keyValuePair.Value, scale, boundingBox2D, ortoSheets, lastYears[keyValuePair.Key]);
                        if (value != null)
                        {
                            values.Add(value);
                        }
                    }
                }
            }

            OrtoDatas result = new(reference, values);
            if (reduce)
            {
                result.Reduce();
            }

            return result;
        }

        /// <summary>
        /// Asynchronously retrieves orthophoto data for a specific bounding box using a provided HttpClient and year range.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to perform the network requests.</param>
        /// <param name="boundingBox2D">The geographic area to retrieve data for.</param>
        /// <param name="reference">A reference string associated with the requested data.</param>
        /// <param name="years">The range of years for which to retrieve orthophoto data.</param>
        /// <param name="scale">The scale factor used for the image request.</param>
        /// <param name="reduce">A value indicating whether to reduce the result set.</param>
        /// <param name="initialRequestCount">The initial number of requests to perform when querying data.</param>
        /// <param name="ortoSheets">The orthophoto index sheets of the area, used to date every image by the sheet it shows (<see cref="OrtoData(int, byte[], double, BoundingBox2D, IEnumerable{OrtoSheet}, int?)"/>); null leaves the images undated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the OrtoDatas if successful; otherwise, null.</returns>
        public static async Task<OrtoDatas?> OrtoDatas(
            this HttpClient httpClient,
            BoundingBox2D? boundingBox2D,
            string? reference,
            Range<int>? years,
            double scale,
            bool reduce = true,
            int initialRequestCount = 8,
            IEnumerable<OrtoSheet?>? ortoSheets = null)
        {
            if (boundingBox2D is null || httpClient is null)
            {
                return null;
            }

            List<OrtoData> values = [];

            List<int>? yearsList = years.ToSystem(1);
            if (yearsList is null)
            {
                return null;
            }

            if (yearsList.Count != 0)
            {
                Dictionary<int, byte[]>? dictionary = await Query.BytesDictionary(httpClient, boundingBox2D, yearsList, scale, initialRequestCount);
                if (dictionary != null && dictionary.Count != 0)
                {
                    Dictionary<int, int> lastYears = Query.ContentRunEndYears(dictionary)!;
                    foreach (KeyValuePair<int, byte[]> keyValuePair in dictionary)
                    {
                        OrtoData? value = OrtoData(keyValuePair.Key, keyValuePair.Value, scale, boundingBox2D, ortoSheets, lastYears[keyValuePair.Key]);
                        if (value != null)
                        {
                            values.Add(value);
                        }
                    }
                }
            }

            OrtoDatas result = new(reference, values);
            if (reduce)
            {
                result.Reduce();
            }

            return result;
        }
        /// <summary>
        /// Re-dates a stored orthophoto series by the orthophoto sheet index: every image is rebuilt with <see cref="OrtoData(int, byte[], double, BoundingBox2D, IEnumerable{OrtoSheet}, int?)"/>, dated against the last year of its run (<see cref="Query.OrtoDataRunEndYears"/>).
        /// <para>Stored series were reduced when they were fetched, so their runs of identical images are gone and <see cref="Query.ContentRunEndYears"/> cannot be used; each image is taken to stand for every year up to the next stored image. The area of an image is rebuilt from its location, scale and size (<see cref="OrtoData.GetBoundingBox(Enums.GeometryContext)"/>); an image whose size cannot be read is kept unchanged.</para>
        /// <para>The result keeps the <see cref="OrtoDatas.Reference"/> and the <see cref="Core.Classes.GuidObject.Guid"/> of the input, and re-dating it again gives the same series.</para>
        /// </summary>
        /// <param name="ortoDatas">The stored series.</param>
        /// <param name="ortoSheets">The orthophoto index sheets of the area; null leaves every image undated.</param>
        /// <param name="lastYear">The last year requested when the series was fetched (<c>OrtoDatasOptions.Years</c>).</param>
        /// <returns>The re-dated series, or null when the input is null.</returns>
        public static OrtoDatas? OrtoDatas(this OrtoDatas? ortoDatas, IEnumerable<OrtoSheet?>? ortoSheets, int lastYear)
        {
            if (ortoDatas == null)
            {
                return null;
            }

            Dictionary<DateTime, int> lastYears = Query.OrtoDataRunEndYears(ortoDatas, lastYear)!;

            List<OrtoData> values = [];
            foreach (OrtoData ortoData in ortoDatas)
            {
                if (ortoData == null)
                {
                    continue;
                }

                BoundingBox2D? boundingBox2D = ortoData.GetBoundingBox(Enums.GeometryContext.Global);
                OrtoData? value = boundingBox2D == null ? null : OrtoData(ortoData.DateTime.Year, ortoData.Bytes, ortoData.Scale, boundingBox2D, ortoSheets, lastYears[ortoData.DateTime]);

                values.Add(value ?? ortoData);
            }

            return new Classes.OrtoDatas(ortoDatas.Reference, values).Duplicate(ortoDatas.Guid) as OrtoDatas;
        }
    }
}