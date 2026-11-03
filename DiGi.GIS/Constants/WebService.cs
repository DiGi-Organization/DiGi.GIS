namespace DiGi.GIS.Constants
{
    /// <summary>
    /// Provides the GUGiK (Geoportal) service endpoints and layer names used by the orthophoto inventory.
    /// <para>Every value here was verified against the live services on 2026-11-03 (DiGi-Organization/DiGi.GIS#15). The services fail quietly when asked the wrong way, so the quirk each value guards against is stated beside it.</para>
    /// </summary>
    public static class WebService
    {
        /// <summary>
        /// The WFS service publishing the per-sheet orthophoto index, one typename per year (<see cref="OrtoSheetTypeNamePrefix"/> followed by the year).
        /// <para>Answers GML only (3.1.1 for WFS 1.1.0); <c>OUTPUTFORMAT=application/json</c> is rejected. <c>GetFeature</c> without <c>VERSION</c> answers <c>MissingParameterValue</c>.</para>
        /// </summary>
        public const string OrtoSheetIndex = "https://mapy.geoportal.gov.pl/wss/service/PZGIK/ORTO/WFS/Skorowidze";

        /// <summary>
        /// The time-enabled standard resolution orthophoto WMS (layer <see cref="StandardResolutionTimeLayer"/>, monthly <c>TIME</c> dimension from 1995).
        /// </summary>
        public const string StandardResolutionTime = "https://mapy.geoportal.gov.pl/wss/service/PZGIK/ORTO/WMS/StandardResolutionTime";

        /// <summary>
        /// The time-enabled high resolution orthophoto WMS (layer <see cref="HighResolutionTimeLayer"/>, monthly <c>TIME</c> dimension from 2012).
        /// </summary>
        public const string HighResolutionTime = "https://mapy.geoportal.gov.pl/wss/service/PZGIK/ORTO/WMS/HighResolutionTime";

        /// <summary>
        /// The true orthophoto WMS (index layer <see cref="TrueOrthoIndexLayer"/>, imagery layer <see cref="TrueOrthoLayer"/>). Its WFS interface is disabled, so there is no per-sheet true orthophoto feed.
        /// </summary>
        public const string TrueOrtho = "https://mapy.geoportal.gov.pl/wss/service/PZGIK/ORTO/WMS/TrueOrtho";

        /// <summary>
        /// The typename prefix of the per-year orthophoto index layers of <see cref="OrtoSheetIndex"/> (<c>gugik:SkorowidzOrtofomapy2005</c>).
        /// </summary>
        public const string OrtoSheetTypeNamePrefix = "gugik:SkorowidzOrtofomapy";

        /// <summary>
        /// The imagery layer of <see cref="StandardResolutionTime"/>.
        /// </summary>
        public const string StandardResolutionTimeLayer = "Raster";

        /// <summary>
        /// The imagery layer of <see cref="HighResolutionTime"/>. Asking this service for <see cref="StandardResolutionTimeLayer"/> answers <c>ServiceException: LayerNotDefined</c>.
        /// </summary>
        public const string HighResolutionTimeLayer = "Image";

        /// <summary>
        /// The queryable sheet index layer of <see cref="TrueOrtho"/>.
        /// </summary>
        public const string TrueOrthoIndexLayer = "Skorowidze";

        /// <summary>
        /// The imagery layer of <see cref="TrueOrtho"/>.
        /// </summary>
        public const string TrueOrthoLayer = "PrawdziwaOrtofotomapa";

        /// <summary>
        /// The <c>MaxScaleDenominator</c> both time-enabled WMS services advertise. A request at a coarser scale answers a uniform white image with HTTP 200 and no exception.
        /// </summary>
        public const double OrtoMaxScaleDenominator = 47247.023810;

        /// <summary>
        /// The size of a screen pixel in metres the OGC WMS specification uses to turn a ground resolution into a scale denominator.
        /// </summary>
        public const double StandardRenderingPixelSize = 0.00028;

        /// <summary>
        /// The number of features requested per WFS page. 5 000 features are about 8.9 MB of GML and answer in about 4 s.
        /// </summary>
        public const int OrtoSheetPageSize = 5000;
    }
}
