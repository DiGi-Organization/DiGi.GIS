#### [DiGi\.GIS](DiGi.GIS.Overview.md 'DiGi\.GIS\.Overview')

## DiGi\.GIS\.Constants Namespace
### Classes

<a name='DiGi.GIS.Constants.CoordinateSystem'></a>

## CoordinateSystem Class

Provides the coordinate system definitions and the projection parameters shared by the EPSG conversions of this library\.

```csharp
public static class CoordinateSystem
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → CoordinateSystem
### Fields

<a name='DiGi.GIS.Constants.CoordinateSystem.EPSG2180DomainMargin'></a>

## CoordinateSystem\.EPSG2180DomainMargin Field

The slack in metres added around the EPSG:2180 box derived from the Polish geographic window\.

```csharp
public const double EPSG2180DomainMargin = 25000;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.EPSG2180Wkt'></a>

## CoordinateSystem\.EPSG2180Wkt Field

The OGC WKT definition of EPSG:2180 \(ETRS89 / Poland CS92\)\. Single source of truth for both directions, so they cannot drift apart\.

```csharp
public const string EPSG2180Wkt = "PROJCS["ETRS89 / Poland CS92",GEOGCS["ETRS89",DATUM["European_Terrestrial_Reference_System_1989",SPHEROID["GRS 1980",6378137,298.257222101,AUTHORITY["EPSG","7019"]],AUTHORITY["EPSG","6258"]],PRIMEM["Greenwich",0],UNIT["degree",0.0174532925199433],AUTHORITY["EPSG","4258"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",19],PARAMETER["scale_factor",0.9993],PARAMETER["false_easting",500000],PARAMETER["false_northing",-5300000],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AXIS["X",EAST],AXIS["Y",NORTH],AUTHORITY["EPSG","2180"]]";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.CoordinateSystem.EPSG3035Wkt'></a>

## CoordinateSystem\.EPSG3035Wkt Field

The OGC WKT definition of EPSG:3035 \(ETRS89 / LAEA Europe\)\. It names the source system reported by Create\.EPSG2180TransformationFromEPSG3035; it is never handed to ProjNet to build a transformation, because ProjNET4GeoAPI 1\.4\.1 registers no lambert\_azimuthal\_equal\_area projection and CreateFromCoordinateSystems throws NotSupportedException for it\.

```csharp
public const string EPSG3035Wkt = "PROJCS["ETRS89 / LAEA Europe",GEOGCS["ETRS89",DATUM["European_Terrestrial_Reference_System_1989",SPHEROID["GRS 1980",6378137,298.257222101,AUTHORITY["EPSG","7019"]],AUTHORITY["EPSG","6258"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4258"]],PROJECTION["Lambert_Azimuthal_Equal_Area"],PARAMETER["latitude_of_center",52],PARAMETER["longitude_of_center",10],PARAMETER["false_easting",4321000],PARAMETER["false_northing",3210000],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AXIS["Northing",NORTH],AXIS["Easting",EAST],AUTHORITY["EPSG","3035"]]";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.CoordinateSystem.InverseFlattening'></a>

## CoordinateSystem\.InverseFlattening Field

The inverse flattening of the GRS 1980 ellipsoid \(EPSG:7019\)\.

```csharp
public const double InverseFlattening = 298.257222101;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.LambertAzimuthalEqualAreaCentralMeridian'></a>

## CoordinateSystem\.LambertAzimuthalEqualAreaCentralMeridian Field

The longitude of the EPSG:3035 projection centre in degrees\.

```csharp
public const double LambertAzimuthalEqualAreaCentralMeridian = 10;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.LambertAzimuthalEqualAreaFalseEasting'></a>

## CoordinateSystem\.LambertAzimuthalEqualAreaFalseEasting Field

The false easting of EPSG:3035 in metres\.

```csharp
public const double LambertAzimuthalEqualAreaFalseEasting = 4321000;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.LambertAzimuthalEqualAreaFalseNorthing'></a>

## CoordinateSystem\.LambertAzimuthalEqualAreaFalseNorthing Field

The false northing of EPSG:3035 in metres\.

```csharp
public const double LambertAzimuthalEqualAreaFalseNorthing = 3210000;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.LambertAzimuthalEqualAreaLatitudeOfOrigin'></a>

## CoordinateSystem\.LambertAzimuthalEqualAreaLatitudeOfOrigin Field

The latitude of the EPSG:3035 projection centre in degrees\.

```csharp
public const double LambertAzimuthalEqualAreaLatitudeOfOrigin = 52;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.PolandMaximumLatitude'></a>

## CoordinateSystem\.PolandMaximumLatitude Field

The northern limit, in degrees north, of the geographic window bounding the Polish domain\.

```csharp
public const double PolandMaximumLatitude = 55.5;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.PolandMaximumLongitude'></a>

## CoordinateSystem\.PolandMaximumLongitude Field

The eastern limit, in degrees east, of the geographic window bounding the Polish domain\.

```csharp
public const double PolandMaximumLongitude = 25.5;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.PolandMinimumLatitude'></a>

## CoordinateSystem\.PolandMinimumLatitude Field

The southern limit, in degrees north, of the geographic window bounding the Polish domain\.

```csharp
public const double PolandMinimumLatitude = 48.5;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.PolandMinimumLongitude'></a>

## CoordinateSystem\.PolandMinimumLongitude Field

The western limit, in degrees east, of the geographic window bounding the Polish domain\. Mainland Poland spans 14\.12 E to 24\.15 E and 49\.00 N to 54\.85 N; about a degree of slack keeps a footprint at the border inside the domain instead of clipping it\.

```csharp
public const double PolandMinimumLongitude = 13;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.CoordinateSystem.SemiMajorAxis'></a>

## CoordinateSystem\.SemiMajorAxis Field

The semi\-major axis of the GRS 1980 ellipsoid in metres\.

```csharp
public const double SemiMajorAxis = 6378137;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.DirectoryName'></a>

## DirectoryName Class

Provides constant values for directory names used within the GIS system\.

```csharp
public static class DirectoryName
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → DirectoryName
### Fields

<a name='DiGi.GIS.Constants.DirectoryName.AdministrativeAreals'></a>

## DirectoryName\.AdministrativeAreals Field

The directory name for administrative areals\.

```csharp
public const string AdministrativeAreals = "administrativeAreals";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.DirectoryName.Building2D'></a>

## DirectoryName\.Building2D Field

The directory name for Building 2D data\.

```csharp
public const string Building2D = "Building2D";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.DirectoryName.Buildings'></a>

## DirectoryName\.Buildings Field

The directory name for buildings\.

```csharp
public const string Buildings = "buildings";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.DirectoryName.OrtoData'></a>

## DirectoryName\.OrtoData Field

The directory name for Orto data\.

```csharp
public const string OrtoData = "OrtoData";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.DirectoryName.OrtoRange'></a>

## DirectoryName\.OrtoRange Field

The directory name for Orto range\.

```csharp
public const string OrtoRange = "OrtoRange";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileExtension'></a>

## FileExtension Class

Provides constant values for file extensions used within the GIS system\.

```csharp
public static class FileExtension
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → FileExtension
### Fields

<a name='DiGi.GIS.Constants.FileExtension.Building2DYearBuiltPredictionsFile'></a>

## FileExtension\.Building2DYearBuiltPredictionsFile Field

The file extension for Building 2D Year Built Predictions files\.

```csharp
public const string Building2DYearBuiltPredictionsFile = "bypf";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileExtension.GISModelFile'></a>

## FileExtension\.GISModelFile Field

The file extension for GIS Model files\.

```csharp
public const string GISModelFile = "gmf";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileExtension.OrtoDatasFile'></a>

## FileExtension\.OrtoDatasFile Field

The file extension for Orto Datas files\.

```csharp
public const string OrtoDatasFile = "odf";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileExtension.OrtoRangeFile'></a>

## FileExtension\.OrtoRangeFile Field

The file extension for Orto Range files\.

```csharp
public const string OrtoRangeFile = "orf";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileExtension.StatisticalDataCollectionFile'></a>

## FileExtension\.StatisticalDataCollectionFile Field

The file extension for Statistical Data Collection files\.

```csharp
public const string StatisticalDataCollectionFile = "sdcf";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileExtension.StatisticalUnitFile'></a>

## FileExtension\.StatisticalUnitFile Field

The file extension for Statistical Unit files\.

```csharp
public const string StatisticalUnitFile = "suf";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileExtension.YearBuiltDataFile'></a>

## FileExtension\.YearBuiltDataFile Field

The file extension for Year Built Data files\.

```csharp
public const string YearBuiltDataFile = "ybdf";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileFilter'></a>

## FileFilter Class

Provides a set of predefined file filters used for filtering files in the GIS module\.

```csharp
public static class FileFilter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → FileFilter
### Properties

<a name='DiGi.GIS.Constants.FileFilter.Building2DYearBuiltPredictionsFile'></a>

## FileFilter\.Building2DYearBuiltPredictionsFile Property

Gets the file filter for building 2D year built predictions files\.

```csharp
public static DiGi.Core.IO.Classes.FileFilter Building2DYearBuiltPredictionsFile { get; }
```

#### Property Value
[DiGi\.Core\.IO\.Classes\.FileFilter](https://learn.microsoft.com/en-us/dotnet/api/digi.core.io.classes.filefilter 'DiGi\.Core\.IO\.Classes\.FileFilter')

<a name='DiGi.GIS.Constants.FileFilter.GISModelFile'></a>

## FileFilter\.GISModelFile Property

Gets the file filter for GIS model files\.

```csharp
public static DiGi.Core.IO.Classes.FileFilter GISModelFile { get; }
```

#### Property Value
[DiGi\.Core\.IO\.Classes\.FileFilter](https://learn.microsoft.com/en-us/dotnet/api/digi.core.io.classes.filefilter 'DiGi\.Core\.IO\.Classes\.FileFilter')

<a name='DiGi.GIS.Constants.FileFilter.OrtoDatasFile'></a>

## FileFilter\.OrtoDatasFile Property

Gets the file filter for ortho data files\.

```csharp
public static DiGi.Core.IO.Classes.FileFilter OrtoDatasFile { get; }
```

#### Property Value
[DiGi\.Core\.IO\.Classes\.FileFilter](https://learn.microsoft.com/en-us/dotnet/api/digi.core.io.classes.filefilter 'DiGi\.Core\.IO\.Classes\.FileFilter')

<a name='DiGi.GIS.Constants.FileFilter.OrtoRangeFile'></a>

## FileFilter\.OrtoRangeFile Property

Gets the file filter for ortho range files\.

```csharp
public static DiGi.Core.IO.Classes.FileFilter OrtoRangeFile { get; }
```

#### Property Value
[DiGi\.Core\.IO\.Classes\.FileFilter](https://learn.microsoft.com/en-us/dotnet/api/digi.core.io.classes.filefilter 'DiGi\.Core\.IO\.Classes\.FileFilter')

<a name='DiGi.GIS.Constants.FileFilter.StatisticalDataCollectionFile'></a>

## FileFilter\.StatisticalDataCollectionFile Property

Gets the file filter for statistical data collection files\.

```csharp
public static DiGi.Core.IO.Classes.FileFilter StatisticalDataCollectionFile { get; }
```

#### Property Value
[DiGi\.Core\.IO\.Classes\.FileFilter](https://learn.microsoft.com/en-us/dotnet/api/digi.core.io.classes.filefilter 'DiGi\.Core\.IO\.Classes\.FileFilter')

<a name='DiGi.GIS.Constants.FileFilter.StatisticalUnitFile'></a>

## FileFilter\.StatisticalUnitFile Property

Gets the file filter for statistical unit files\.

```csharp
public static DiGi.Core.IO.Classes.FileFilter StatisticalUnitFile { get; }
```

#### Property Value
[DiGi\.Core\.IO\.Classes\.FileFilter](https://learn.microsoft.com/en-us/dotnet/api/digi.core.io.classes.filefilter 'DiGi\.Core\.IO\.Classes\.FileFilter')

<a name='DiGi.GIS.Constants.FileFilter.YearBuiltDataFile'></a>

## FileFilter\.YearBuiltDataFile Property

Gets the file filter for year built data files\.

```csharp
public static DiGi.Core.IO.Classes.FileFilter YearBuiltDataFile { get; }
```

#### Property Value
[DiGi\.Core\.IO\.Classes\.FileFilter](https://learn.microsoft.com/en-us/dotnet/api/digi.core.io.classes.filefilter 'DiGi\.Core\.IO\.Classes\.FileFilter')

<a name='DiGi.GIS.Constants.FileName'></a>

## FileName Class

Provides constant values for file names used within the GIS system\.

```csharp
public static class FileName
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → FileName
### Fields

<a name='DiGi.GIS.Constants.FileName.AdministrativeAreal2D'></a>

## FileName\.AdministrativeAreal2D Field

The filename for Administrative Areal 2D data\.

```csharp
public const string AdministrativeAreal2D = "AdministrativeAreal2D.json";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileName.Building2D'></a>

## FileName\.Building2D Field

The filename for Building 2D data\.

```csharp
public const string Building2D = "Building2D.json";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileNameSufix'></a>

## FileNameSufix Class

Provides constant values for file name suffixes used in the GIS system\.

```csharp
public static class FileNameSufix
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → FileNameSufix
### Fields

<a name='DiGi.GIS.Constants.FileNameSufix.OT_ADJA_A'></a>

## FileNameSufix\.OT\_ADJA\_A Field

The suffix for OT ADJA A XML files\.

```csharp
public const string OT_ADJA_A = "__OT_ADJA_A.xml";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileNameSufix.OT_ADMS_A'></a>

## FileNameSufix\.OT\_ADMS\_A Field

The suffix for OT ADMS A XML files\.

```csharp
public const string OT_ADMS_A = "__OT_ADMS_A.xml";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileNameSufix.OT_BUBD_A'></a>

## FileNameSufix\.OT\_BUBD\_A Field

The suffix for OT BUBD A XML files\.

```csharp
public const string OT_BUBD_A = "__OT_BUBD_A.xml";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileTypeName'></a>

## FileTypeName Class

Provides a set of constant strings that define the names of various file types used within the GIS system\.

```csharp
public static class FileTypeName
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → FileTypeName
### Fields

<a name='DiGi.GIS.Constants.FileTypeName.Building2DYearBuiltPredictionsFile'></a>

## FileTypeName\.Building2DYearBuiltPredictionsFile Field

The name of the Building2D Year Built Predictions File\.

```csharp
public const string Building2DYearBuiltPredictionsFile = "Building2D Year Built Predictions File";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileTypeName.GISModelFile'></a>

## FileTypeName\.GISModelFile Field

The name of the GIS Model File\.

```csharp
public const string GISModelFile = "GIS Model File";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileTypeName.OrtoDatasFile'></a>

## FileTypeName\.OrtoDatasFile Field

The name of the Orto Datas File\.

```csharp
public const string OrtoDatasFile = "Orto Datas File";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileTypeName.StatisticalDataCollectionFile'></a>

## FileTypeName\.StatisticalDataCollectionFile Field

The name of the Statistical Data Collection File\.

```csharp
public const string StatisticalDataCollectionFile = "Statistical Data Collection File";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileTypeName.StatisticalUnitFile'></a>

## FileTypeName\.StatisticalUnitFile Field

The name of the Statistical Unit File\.

```csharp
public const string StatisticalUnitFile = "Statistical Unit File";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.FileTypeName.YearBuiltDatasFile'></a>

## FileTypeName\.YearBuiltDatasFile Field

The name of the Year Built Datas File\.

```csharp
public const string YearBuiltDatasFile = "Year Built Datas File";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.OrtoBlank'></a>

## OrtoBlank Class

Provides the parameters of the dependency\-free blank check on orthophoto JPEG tiles \([IsOrtoJpegBlank\(this byte\[\]\)](DiGi.GIS.md#DiGi.GIS.Query.IsOrtoJpegBlank(thisbyte[]) 'DiGi\.GIS\.Query\.IsOrtoJpegBlank\(this byte\[\]\)')\)\.

```csharp
public static class OrtoBlank
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → OrtoBlank
### Fields

<a name='DiGi.GIS.Constants.OrtoBlank.Tolerance'></a>

## OrtoBlank\.Tolerance Field

The largest spread \(maximum minus minimum, per colour component, on 0–255\) of the 8 × 8 block means for which a tile still counts as one uniform colour\.

The blank tile the GUGiK WMS answers decodes as exactly uniform white, so every block mean is the same and the tolerance only absorbs rounding of the DC quantisation. Same value as the pixel-based check in DiGi.GIS.Emgu.CV.

```csharp
public const int Tolerance = 8;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.Constants.OrtoUsability'></a>

## OrtoUsability Class

Provides the weights and factor values of the orthophoto usability rubric \([OrtoSheetUsability\(this OrtoSheet, Nullable&lt;double&gt;\)](DiGi.GIS.md#DiGi.GIS.Query.OrtoSheetUsability(thisDiGi.GIS.Classes.OrtoSheet,System.Nullable_double_) 'DiGi\.GIS\.Query\.OrtoSheetUsability\(this DiGi\.GIS\.Classes\.OrtoSheet, System\.Nullable\<double\>\)')\)\.

Any change to a value here changes every score, so [Version](DiGi.GIS.Constants.md#DiGi.GIS.Constants.OrtoUsability.Version 'DiGi\.GIS\.Constants\.OrtoUsability\.Version') must be bumped with it — tables scored under different versions are not comparable. The `OrtoSheetUsability` fact pins the values against the version and fails when one moves alone.

```csharp
public static class OrtoUsability
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → OrtoUsability
### Fields

<a name='DiGi.GIS.Constants.OrtoUsability.AnalogSourceValue'></a>

## OrtoUsability\.AnalogSourceValue Field

The source factor of scanned analog photography \(scan artefacts, radiometric drift, year\-precision date\)\.

```csharp
public const double AnalogSourceValue = 0.6;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.BlackAndWhiteValue'></a>

## OrtoUsability\.BlackAndWhiteValue Field

The colour mode factor of black and white imagery \(texture only\)\.

```csharp
public const double BlackAndWhiteValue = 0.5;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.CIRValue'></a>

## OrtoUsability\.CIRValue Field

The colour mode factor of colour infrared imagery\.

```csharp
public const double CIRValue = 1;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.ClarityWeight'></a>

## OrtoUsability\.ClarityWeight Field

The weight of the clarity factor \(one minus the measured obstacle fraction\)\.

```csharp
public const double ClarityWeight = 0.25;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.ColorModeWeight'></a>

## OrtoUsability\.ColorModeWeight Field

The weight of the colour mode factor\.

```csharp
public const double ColorModeWeight = 0.15;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.DigitalSourceValue'></a>

## OrtoUsability\.DigitalSourceValue Field

The source factor of digital photography\.

```csharp
public const double DigitalSourceValue = 1;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.LeafOnPrior'></a>

## OrtoUsability\.LeafOnPrior Field

The clarity prior subtracted for an unprobed sheet acquired in a leaf\-on month \(April to October\)\. A prior, not a measurement\.

```csharp
public const double LeafOnPrior = 0.05;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.OrtoTypeWeight'></a>

## OrtoUsability\.OrtoTypeWeight Field

The weight of the orthophoto type factor\.

```csharp
public const double OrtoTypeWeight = 0.15;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.ResolutionKnots'></a>

## OrtoUsability\.ResolutionKnots Field

The ground resolutions in metres at the knots of the resolution factor, finest first; paired with [ResolutionValues](DiGi.GIS.Constants.md#DiGi.GIS.Constants.OrtoUsability.ResolutionValues 'DiGi\.GIS\.Constants\.OrtoUsability\.ResolutionValues') and interpolated linearly between knots\.

```csharp
public static readonly double[] ResolutionKnots;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

<a name='DiGi.GIS.Constants.OrtoUsability.ResolutionValues'></a>

## OrtoUsability\.ResolutionValues Field

The resolution factor at each of [ResolutionKnots](DiGi.GIS.Constants.md#DiGi.GIS.Constants.OrtoUsability.ResolutionKnots 'DiGi\.GIS\.Constants\.OrtoUsability\.ResolutionKnots')\. Below about 0\.5 m small structures and roof edges stop being separable\.

```csharp
public static readonly double[] ResolutionValues;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

<a name='DiGi.GIS.Constants.OrtoUsability.ResolutionWeight'></a>

## OrtoUsability\.ResolutionWeight Field

The weight of the ground resolution factor\.

```csharp
public const double ResolutionWeight = 0.35;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.RGBValue'></a>

## OrtoUsability\.RGBValue Field

The colour mode factor of true colour imagery\.

```csharp
public const double RGBValue = 0.9;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.SheetIncompleteCap'></a>

## OrtoUsability\.SheetIncompleteCap Field

The upper score of a sheet the index marks as not completely filled \(`czy_ark_wypelniony` other than `TAK`\)\.

```csharp
public const double SheetIncompleteCap = 0.5;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.SnowRiskPrior'></a>

## OrtoUsability\.SnowRiskPrior Field

The clarity prior subtracted for an unprobed sheet acquired in a snow\-risk month \(December to February\)\. A prior, not a measurement\.

```csharp
public const double SnowRiskPrior = 0.05;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.SourceWeight'></a>

## OrtoUsability\.SourceWeight Field

The weight of the source \(analog or digital photography\) factor\.

```csharp
public const double SourceWeight = 0.1;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.StandardOrthoValue'></a>

## OrtoUsability\.StandardOrthoValue Field

The orthophoto type factor of a standard orthophoto \(radial roof offset of several metres for a 10 m building\)\.

```csharp
public const double StandardOrthoValue = 0.7;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.TrueOrthoValue'></a>

## OrtoUsability\.TrueOrthoValue Field

The orthophoto type factor of a true orthophoto \(roofs projected orthogonally\)\.

```csharp
public const double TrueOrthoValue = 1;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.UndefinedColorModeValue'></a>

## OrtoUsability\.UndefinedColorModeValue Field

The colour mode factor when the colour mode is not known\.

```csharp
public const double UndefinedColorModeValue = 0.6;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.UndefinedOrthoValue'></a>

## OrtoUsability\.UndefinedOrthoValue Field

The orthophoto type factor when the type is not known\.

```csharp
public const double UndefinedOrthoValue = 0.5;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.UndefinedSourceValue'></a>

## OrtoUsability\.UndefinedSourceValue Field

The source factor when the source is not recognised\.

```csharp
public const double UndefinedSourceValue = 0.8;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.OrtoUsability.Version'></a>

## OrtoUsability\.Version Field

The version of the rubric the values below define\.

```csharp
public const int Version = 1;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.Constants.ReferenceKind'></a>

## ReferenceKind Class

Discriminator tokens for the reference types defined in DiGi\.GIS\.

These values are a persisted contract: they are written into stored reference strings, so they are
            append-only. Renaming one silently invalidates every string already stored in that format. A token must be
            unique across every repository, and must contain neither a comma (which would make it parse as a full type
            name) nor a colon.

This class is deliberately NOT named Reference. A local Constants.Reference would win
            innermost-namespace lookup over DiGi.Core.Constants.Reference and silently shadow the shared grammar
            constants - a bug this codebase has already had once, in DiGi.PostgreSQL.

```csharp
public static class ReferenceKind
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → ReferenceKind
### Fields

<a name='DiGi.GIS.Constants.ReferenceKind.GISModelAreal2D'></a>

## ReferenceKind\.GISModelAreal2D Field

Discriminator for [GISModelAreal2DReference](DiGi.GIS.Classes.md#DiGi.GIS.Classes.GISModelAreal2DReference 'DiGi\.GIS\.Classes\.GISModelAreal2DReference')\.

```csharp
public const string GISModelAreal2D = "GISModelAreal2D";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.ReferenceKind.GISModelFileGuidObject'></a>

## ReferenceKind\.GISModelFileGuidObject Field

Discriminator for [GISModelFileGuidObjectReference](DiGi.GIS.Classes.md#DiGi.GIS.Classes.GISModelFileGuidObjectReference 'DiGi\.GIS\.Classes\.GISModelFileGuidObjectReference')\.

```csharp
public const string GISModelFileGuidObject = "GISModelFileGuidObject";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService'></a>

## WebService Class

Provides the GUGiK \(Geoportal\) service endpoints and layer names used by the orthophoto inventory\.

Every value here was verified against the live services on 2026-11-03 (DiGi-Organization/DiGi.GIS#15). The services fail quietly when asked the wrong way, so the quirk each value guards against is stated beside it.

```csharp
public static class WebService
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → WebService
### Fields

<a name='DiGi.GIS.Constants.WebService.HighResolutionTime'></a>

## WebService\.HighResolutionTime Field

The time\-enabled high resolution orthophoto WMS \(layer [HighResolutionTimeLayer](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.HighResolutionTimeLayer 'DiGi\.GIS\.Constants\.WebService\.HighResolutionTimeLayer'), monthly `TIME` dimension from 2012\)\.

```csharp
public const string HighResolutionTime = "https://mapy.geoportal.gov.pl/wss/service/PZGIK/ORTO/WMS/HighResolutionTime";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.HighResolutionTimeLayer'></a>

## WebService\.HighResolutionTimeLayer Field

The imagery layer of [HighResolutionTime](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.HighResolutionTime 'DiGi\.GIS\.Constants\.WebService\.HighResolutionTime')\. Asking this service for [StandardResolutionTimeLayer](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.StandardResolutionTimeLayer 'DiGi\.GIS\.Constants\.WebService\.StandardResolutionTimeLayer') answers `ServiceException: LayerNotDefined`\.

```csharp
public const string HighResolutionTimeLayer = "Image";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.OrtoMaxScaleDenominator'></a>

## WebService\.OrtoMaxScaleDenominator Field

The `MaxScaleDenominator` both time\-enabled WMS services advertise\. A request at a coarser scale answers a uniform white image with HTTP 200 and no exception\.

```csharp
public const double OrtoMaxScaleDenominator = 47247.02381;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.WebService.OrtoSheetIndex'></a>

## WebService\.OrtoSheetIndex Field

The WFS service publishing the per\-sheet orthophoto index, one typename per year \([OrtoSheetTypeNamePrefix](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.OrtoSheetTypeNamePrefix 'DiGi\.GIS\.Constants\.WebService\.OrtoSheetTypeNamePrefix') followed by the year\)\.

Answers GML only (3.1.1 for WFS 1.1.0); `OUTPUTFORMAT=application/json` is rejected. `GetFeature` without `VERSION` answers `MissingParameterValue`.

```csharp
public const string OrtoSheetIndex = "https://mapy.geoportal.gov.pl/wss/service/PZGIK/ORTO/WFS/Skorowidze";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.OrtoSheetPageSize'></a>

## WebService\.OrtoSheetPageSize Field

The number of features requested per WFS page\. 5 000 features are about 8\.9 MB of GML and answer in about 4 s\.

```csharp
public const int OrtoSheetPageSize = 5000;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.Constants.WebService.OrtoSheetTypeNamePrefix'></a>

## WebService\.OrtoSheetTypeNamePrefix Field

The typename prefix of the per\-year orthophoto index layers of [OrtoSheetIndex](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.OrtoSheetIndex 'DiGi\.GIS\.Constants\.WebService\.OrtoSheetIndex') \(`gugik:SkorowidzOrtofomapy2005`\)\.

```csharp
public const string OrtoSheetTypeNamePrefix = "gugik:SkorowidzOrtofomapy";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.StandardRenderingPixelSize'></a>

## WebService\.StandardRenderingPixelSize Field

The size of a screen pixel in metres the OGC WMS specification uses to turn a ground resolution into a scale denominator\.

```csharp
public const double StandardRenderingPixelSize = 0.00028;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.Constants.WebService.StandardResolutionTime'></a>

## WebService\.StandardResolutionTime Field

The time\-enabled standard resolution orthophoto WMS \(layer [StandardResolutionTimeLayer](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.StandardResolutionTimeLayer 'DiGi\.GIS\.Constants\.WebService\.StandardResolutionTimeLayer'), monthly `TIME` dimension from 1995\)\.

```csharp
public const string StandardResolutionTime = "https://mapy.geoportal.gov.pl/wss/service/PZGIK/ORTO/WMS/StandardResolutionTime";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.StandardResolutionTimeLayer'></a>

## WebService\.StandardResolutionTimeLayer Field

The imagery layer of [StandardResolutionTime](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.StandardResolutionTime 'DiGi\.GIS\.Constants\.WebService\.StandardResolutionTime')\.

```csharp
public const string StandardResolutionTimeLayer = "Raster";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.TrueOrtho'></a>

## WebService\.TrueOrtho Field

The true orthophoto WMS \(index layer [TrueOrthoIndexLayer](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.TrueOrthoIndexLayer 'DiGi\.GIS\.Constants\.WebService\.TrueOrthoIndexLayer'), imagery layer [TrueOrthoLayer](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.TrueOrthoLayer 'DiGi\.GIS\.Constants\.WebService\.TrueOrthoLayer')\)\. Its WFS interface answers `Unauthorized`, so the true orthophoto sheets are read per point through `GetFeatureInfo` \([TrueOrthoSheetsAsync\(this HttpClient, Point2D, int, TimeSpan, Nullable&lt;DateTimeOffset&gt;, CancellationToken\)](DiGi.GIS.md#DiGi.GIS.Query.TrueOrthoSheetsAsync(thisSystem.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Point2D,int,System.TimeSpan,System.Nullable_System.DateTimeOffset_,System.Threading.CancellationToken) 'DiGi\.GIS\.Query\.TrueOrthoSheetsAsync\(this System\.Net\.Http\.HttpClient, DiGi\.Geometry\.Planar\.Classes\.Point2D, int, System\.TimeSpan, System\.Nullable\<System\.DateTimeOffset\>, System\.Threading\.CancellationToken\)')\)\. True orthophotos are a product of their own \(downloads under `DaneFotogrametryczne/TrueOrtho/`, 1:2 500 and 1:5 000 sheets\): they are not rows of [OrtoSheetIndex](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.OrtoSheetIndex 'DiGi\.GIS\.Constants\.WebService\.OrtoSheetIndex')\.

```csharp
public const string TrueOrtho = "https://mapy.geoportal.gov.pl/wss/service/PZGIK/ORTO/WMS/TrueOrtho";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.TrueOrthoIndexLayer'></a>

## WebService\.TrueOrthoIndexLayer Field

The queryable sheet index layer of [TrueOrtho](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.TrueOrtho 'DiGi\.GIS\.Constants\.WebService\.TrueOrtho')\.

```csharp
public const string TrueOrthoIndexLayer = "Skorowidze";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.TrueOrthoInfoFormatGml'></a>

## WebService\.TrueOrthoInfoFormatGml Field

The `GetFeatureInfo` format of [TrueOrtho](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.TrueOrtho 'DiGi\.GIS\.Constants\.WebService\.TrueOrtho') that carries the sheet boxes \(GML 2 `gml:Box`, `gml:coordinates` easting first, unlike the northing\-first `posList` of [OrtoSheetIndex](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.OrtoSheetIndex 'DiGi\.GIS\.Constants\.WebService\.OrtoSheetIndex')\) and no attributes, in the same order as [TrueOrthoInfoFormatHtml](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.TrueOrthoInfoFormatHtml 'DiGi\.GIS\.Constants\.WebService\.TrueOrthoInfoFormatHtml')\.

```csharp
public const string TrueOrthoInfoFormatGml = "application/vnd.ogc.gml";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.TrueOrthoInfoFormatHtml'></a>

## WebService\.TrueOrthoInfoFormatHtml Field

The `GetFeatureInfo` format of [TrueOrtho](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.TrueOrtho 'DiGi\.GIS\.Constants\.WebService\.TrueOrtho') that carries the sheet attributes: an HTML page whose script calls `skor.push({url:"…", godlo:"…", aktualnosc:"…", …})` once per sheet\. It carries no geometry\.

```csharp
public const string TrueOrthoInfoFormatHtml = "text/html";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.Constants.WebService.TrueOrthoLayer'></a>

## WebService\.TrueOrthoLayer Field

The imagery layer of [TrueOrtho](DiGi.GIS.Constants.md#DiGi.GIS.Constants.WebService.TrueOrtho 'DiGi\.GIS\.Constants\.WebService\.TrueOrtho')\.

```csharp
public const string TrueOrthoLayer = "PrawdziwaOrtofotomapa";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')