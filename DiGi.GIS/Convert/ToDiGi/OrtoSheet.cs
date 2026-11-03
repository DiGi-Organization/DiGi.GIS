using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts one feature of the GUGiK orthophoto index (an element such as <c>gugik:SkorowidzOrtofomapy2005</c>) to an <see cref="OrtoSheet"/>.
        /// <para><b>Axis order.</b> With <c>SRSNAME=EPSG:2180</c> the WFS 1.1.0 response writes every <c>posList</c> pair northing first (the official EPSG:2180 axis order); <see cref="Point2D"/> holds easting in X, so each pair is swapped here. Verified 2026-11-03: sheet <c>N-33-117-D-a-3-4</c> (latitude about 52.7°) reads <c>547909 316630</c>, which is only in Poland as northing 547909, easting 316630.</para>
        /// <para><b>Date precision.</b> This is the one place it is classified. Analog-era rows carry a <c>YYYY-01-01</c> placeholder in <c>akt_data</c>, so a 1 January date of an analog source (<c>zrodlo_danych</c> containing <c>analog</c>) is <see cref="OrtoDatePrecision.Year"/>; every other parsed date is <see cref="OrtoDatePrecision.Day"/>.</para>
        /// <para>The footprint is the exterior ring of the first polygon of <c>msGeometry</c>. The orthophoto type is not in the index and is left <see cref="OrtoType.Undefined"/>.</para>
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

            OrtoDatePrecision ortoDatePrecision = OrtoDatePrecision.Undefined;
            if (date != null)
            {
                bool analog = source != null && source.IndexOf("analog", StringComparison.OrdinalIgnoreCase) >= 0;
                ortoDatePrecision = analog && date.Value.Month == 1 && date.Value.Day == 1 ? OrtoDatePrecision.Year : OrtoDatePrecision.Day;
            }

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
                OrtoType.Undefined,
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
    }
}
