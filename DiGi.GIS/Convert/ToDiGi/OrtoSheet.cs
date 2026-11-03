using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts one feature of the GUGiK orthophoto index (an element such as <c>gugik:SkorowidzOrtofomapy2005</c>) to an <see cref="OrtoSheet"/>.
        /// <para><b>Axis order.</b> With <c>SRSNAME=EPSG:2180</c> the WFS 1.1.0 response writes every <c>posList</c> pair northing first (the official EPSG:2180 axis order); <see cref="Point2D"/> holds easting in X, so each pair is swapped here. Verified 2026-11-03: sheet <c>N-33-117-D-a-3-4</c> (latitude about 52.7°) reads <c>547909 316630</c>, which is only in Poland as northing 547909, easting 316630.</para>
        /// <para><b>Date precision</b> is classified by <see cref="Query.OrtoSheetDatePrecision(DateTimeOffset?, string)"/>: analog-era rows carry a <c>YYYY-01-01</c> placeholder in <c>akt_data</c>.</para>
        /// <para>The footprint is the exterior ring of the first polygon of <c>msGeometry</c>. Every index row is <see cref="OrtoType.Standard"/>: its download is under <c>/ortofotomapa/</c>, while true orthophotos are a separate product with their own sheets and downloads (verified 2026-11-03 at Łódź: the 2021-04-11 flight is index sheet <c>M-34-3-D-b-4-2</c> at 1:5 000 and true orthophoto sheet <c>M-34-3-D-b-4-2-3</c> at 1:2 500). Read those with <see cref="ToDiGi_OrtoSheets(string, string, DateTimeOffset?)"/>.</para>
        /// </summary>
        /// <param name="xElement">The feature element.</param>
        /// <param name="probeDate">The moment the index was read.</param>
        /// <returns>The sheet, or null when the element is missing or carries neither a sheet identifier nor a year.</returns>
        public static OrtoSheet? ToDiGi_OrtoSheet(this XElement? xElement, DateTimeOffset? probeDate = null)
        {
            if (xElement == null)
            {
                return null;
            }

            Dictionary<string, string> values = [];
            foreach (XElement xElement_Child in xElement.Elements())
            {
                values[xElement_Child.Name.LocalName] = xElement_Child.Value.Trim();
            }

            string? sheetId = Value("godlo");
            if (string.IsNullOrWhiteSpace(sheetId) || !int.TryParse(Value("akt_rok"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int year))
            {
                return null;
            }

            string? source = Value("zrodlo_danych");

            DateTimeOffset? date = Date(Value("akt_data"));

            OrtoDatePrecision ortoDatePrecision = Query.OrtoSheetDatePrecision(date, source);

            OrtoColorMode ortoColorMode = Value("kolor")?.ToUpperInvariant() switch
            {
                "B/W" => OrtoColorMode.BlackAndWhite,
                "RGB" => OrtoColorMode.RGB,
                "CIR" => OrtoColorMode.CIR,
                _ => OrtoColorMode.Undefined,
            };

            bool? sheetFilled = Value("czy_ark_wypelniony")?.ToUpperInvariant() switch
            {
                "TAK" => true,
                "NIE" => false,
                _ => null,
            };

            Polygon2D? footprint = null;
            XElement? xElement_PosList = xElement.Elements().FirstOrDefault(x => x.Name.LocalName == "msGeometry")?.Descendants().FirstOrDefault(x => x.Name.LocalName == "posList");
            if (xElement_PosList != null)
            {
                string[] values_PosList = xElement_PosList.Value.Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

                List<Point2D?> point2Ds = [];
                for (int i = 0; i + 1 < values_PosList.Length; i += 2)
                {
                    if (double.TryParse(values_PosList[i], NumberStyles.Float, CultureInfo.InvariantCulture, out double northing) && double.TryParse(values_PosList[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out double easting))
                    {
                        point2Ds.Add(new Point2D(easting, northing));
                    }
                }

                footprint = Geometry.Planar.Create.Polygon2D(point2Ds);
            }

            return new OrtoSheet(
                sheetId,
                xElement.Name.LocalName,
                year,
                date,
                ortoDatePrecision,
                Double(Value("piksel")),
                ortoColorMode,
                source,
                Value("uklad_xy"),
                Value("modul_archiwizacji"),
                Value("nr_zglosz"),
                sheetFilled,
                Value("url_do_pobrania"),
                Date(Value("dt_pzgik")),
                Double(Value("wlk_pliku_mb")),
                footprint,
                OrtoType.Standard,
                probeDate);

            string? Value(string localName)
            {
                return values.TryGetValue(localName, out string? value) && !string.IsNullOrWhiteSpace(value) ? value : null;
            }

            static DateTimeOffset? Date(string? value)
            {
                if (value != null && DateTimeOffset.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset result))
                {
                    return result;
                }

                return null;
            }

            static double? Double(string? value)
            {
                if (value != null && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                {
                    return result;
                }

                return null;
            }
        }

        /// <summary>
        /// Streams the features of a GUGiK orthophoto index WFS response (a GML <c>wfs:FeatureCollection</c>) as <see cref="OrtoSheet"/> objects, one feature held as XML at a time.
        /// <para><see cref="GML.Query.FeatureMembers(string)"/> has the same streaming shape but reads a file path and skips feature types no assembly models, and the yearly index typenames are not modelled; this walks the stream directly and hands each feature to <see cref="ToDiGi_OrtoSheet(XElement, DateTimeOffset?)"/>. Both <c>gml:featureMember</c> (one feature) and <c>gml:featureMembers</c> (several) are read.</para>
        /// <para>A feature that does not convert is skipped; compare the count with the <c>RESULTTYPE=hits</c> answer to detect loss.</para>
        /// </summary>
        /// <param name="stream">The response stream; it is not disposed.</param>
        /// <param name="probeDate">The moment the index was read.</param>
        /// <returns>The sheets in document order; empty when the stream is null or its root is not a feature collection.</returns>
        public static IEnumerable<OrtoSheet> ToDiGi_OrtoSheets(this Stream? stream, DateTimeOffset? probeDate = null)
        {
            if (stream == null)
            {
                yield break;
            }

            XmlReaderSettings xmlReaderSettings = new()
            {
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreWhitespace = true,
                IgnoreComments = true,
                CloseInput = false
            };

            using XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings);

            if (xmlReader.MoveToContent() != XmlNodeType.Element || xmlReader.LocalName != "FeatureCollection")
            {
                yield break;
            }

            xmlReader.Read();
            while (!xmlReader.EOF)
            {
                if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.LocalName == "featureMember" || xmlReader.LocalName == "featureMembers" || xmlReader.LocalName == "member"))
                {
                    if (XNode.ReadFrom(xmlReader) is XElement xElement_Member)
                    {
                        foreach (XElement xElement_Feature in xElement_Member.Elements())
                        {
                            if (ToDiGi_OrtoSheet(xElement_Feature, probeDate) is OrtoSheet ortoSheet)
                            {
                                yield return ortoSheet;
                            }
                        }
                    }
                }
                else
                {
                    xmlReader.Read();
                }
            }
        }

        /// <summary>
        /// Converts the two <c>GetFeatureInfo</c> answers of the true orthophoto WMS for one point (<see cref="Query.Url_TrueOrthoSheets(Point2D, string, int)"/>) to <see cref="OrtoSheet"/> objects of type <see cref="OrtoType.TrueOrtho"/>.
        /// <para><b>Attributes</b> come from <paramref name="html"/> (<see cref="Constants.WebService.TrueOrthoInfoFormatHtml"/>), the only format that carries them: its script calls <c>skor.push({url:"…", godlo:"…", aktualnosc:"…", wielkoscPiksela:"…", ukladWspolrzednych:"…", modulArchiwizacji:"…", zrodloDanych:"…", numerZgloszeniaPracy:"…", aktualnoscRok:"…"})</c> once per sheet. Values are read as written; an escaped quote inside a value is not expected and is not handled. Colour, sheet completeness, register date and file size are not published and stay null or <see cref="OrtoColorMode.Undefined"/>.</para>
        /// <para><b>Footprints</b> come from <paramref name="gml"/> (<see cref="Constants.WebService.TrueOrthoInfoFormatGml"/>), whose <c>gml:Box</c> elements list the same sheets in the same order with <c>gml:coordinates</c> easting first. They are paired by position, and only when both answers list the same number of sheets; otherwise every footprint is null.</para>
        /// </summary>
        /// <param name="html">The HTML answer.</param>
        /// <param name="gml">The GML answer, or null to leave the footprints null.</param>
        /// <param name="probeDate">The moment the service was read.</param>
        /// <returns>The sheets in the order of the answer (empty when no sheet covers the point), or null when <paramref name="html"/> is missing or is not the HTML answer (a service exception, for example).</returns>
        public static List<OrtoSheet>? ToDiGi_OrtoSheets(this string? html, string? gml, DateTimeOffset? probeDate = null)
        {
            if (html == null || html.IndexOf("ServiceException", StringComparison.Ordinal) >= 0 || html.IndexOf("<head", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return null;
            }

            List<Dictionary<string, string>> valuesList = [];
            foreach (Match match in Regex.Matches(html, @"skor\.push\(\{(.*?)\}\)", RegexOptions.Singleline))
            {
                Dictionary<string, string> values = [];
                foreach (Match match_Value in Regex.Matches(match.Groups[1].Value, @"([A-Za-z]+)\s*:\s*""([^""]*)"""))
                {
                    values[match_Value.Groups[1].Value] = match_Value.Groups[2].Value.Trim();
                }

                valuesList.Add(values);
            }

            List<Polygon2D?>? footprints = Footprints(gml);
            if (footprints != null && footprints.Count != valuesList.Count)
            {
                footprints = null;
            }

            List<OrtoSheet> result = [];
            for (int i = 0; i < valuesList.Count; i++)
            {
                Dictionary<string, string> values = valuesList[i];

                string? sheetId = Value(values, "godlo");
                if (string.IsNullOrWhiteSpace(sheetId) || !int.TryParse(Value(values, "aktualnoscRok"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int year))
                {
                    continue;
                }

                string? source = Value(values, "zrodloDanych");

                DateTimeOffset? date = null;
                if (Value(values, "aktualnosc") is string value_Date && DateTimeOffset.TryParseExact(value_Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset date_Temp))
                {
                    date = date_Temp;
                }

                double? pixelSize = null;
                if (Value(values, "wielkoscPiksela") is string value_PixelSize && double.TryParse(value_PixelSize, NumberStyles.Float, CultureInfo.InvariantCulture, out double pixelSize_Temp))
                {
                    pixelSize = pixelSize_Temp;
                }

                result.Add(new OrtoSheet(
                    sheetId,
                    Constants.WebService.TrueOrthoIndexLayer,
                    year,
                    date,
                    Query.OrtoSheetDatePrecision(date, source),
                    pixelSize,
                    OrtoColorMode.Undefined,
                    source,
                    Value(values, "ukladWspolrzednych"),
                    Value(values, "modulArchiwizacji"),
                    Value(values, "numerZgloszeniaPracy"),
                    null,
                    Value(values, "url"),
                    null,
                    null,
                    footprints?[i],
                    OrtoType.TrueOrtho,
                    probeDate));
            }

            return result;

            static string? Value(Dictionary<string, string> values, string key)
            {
                return values.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value) ? value : null;
            }

            static List<Polygon2D?>? Footprints(string? gml)
            {
                if (string.IsNullOrWhiteSpace(gml))
                {
                    return null;
                }

                XDocument xDocument;
                try
                {
                    xDocument = XDocument.Parse(gml);
                }
                catch (XmlException)
                {
                    return null;
                }

                if (xDocument.Root?.Name.LocalName != "msGMLOutput")
                {
                    return null;
                }

                List<Polygon2D?> footprints = [];
                foreach (XElement xElement_Box in xDocument.Descendants().Where(x => x.Name.LocalName == "Box"))
                {
                    Polygon2D? footprint = null;

                    string[]? values_Coordinates = xElement_Box.Elements().FirstOrDefault(x => x.Name.LocalName == "coordinates")?.Value.Split([' ', ',', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
                    if (values_Coordinates != null && values_Coordinates.Length == 4 &&
                        double.TryParse(values_Coordinates[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double minX) &&
                        double.TryParse(values_Coordinates[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double minY) &&
                        double.TryParse(values_Coordinates[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double maxX) &&
                        double.TryParse(values_Coordinates[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double maxY))
                    {
                        footprint = Geometry.Planar.Create.Polygon2D([new Point2D(minX, minY), new Point2D(maxX, minY), new Point2D(maxX, maxY), new Point2D(minX, maxY)]);
                    }

                    footprints.Add(footprint);
                }

                return footprints;
            }
        }
    }
}
