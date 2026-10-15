using DiGi.Core.Classes;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.Interfaces;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents the stored solar radiation of one receiving surface (an external wall or roof) of a building: its summary values over the weather period and its weather-independent hourly shading factors.
    /// </summary>
    public class SurfaceSolarRadiation : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(Area))]
        private readonly double area = 0;

        [JsonInclude, JsonPropertyName(nameof(Beam))]
        private readonly double beam = 0;

        [JsonInclude, JsonPropertyName(nameof(Diffuse))]
        private readonly double diffuse = 0;

        [JsonInclude, JsonPropertyName(nameof(Energy))]
        private readonly double energy = 0;

        [JsonInclude, JsonPropertyName(nameof(Ground))]
        private readonly double ground = 0;

        [JsonInclude, JsonPropertyName(nameof(GroundVisibility))]
        private readonly double groundVisibility = 1;

        [JsonInclude, JsonPropertyName(nameof(Irradiation))]
        private readonly double irradiation = 0;

        [JsonInclude, JsonPropertyName(nameof(IrradiationUnshaded))]
        private readonly double irradiationUnshaded = 0;

        [JsonInclude, JsonPropertyName(nameof(Normal))]
        private readonly Vector3D? normal = null;

        [JsonInclude, JsonPropertyName(nameof(Reference))]
        private readonly string? reference = null;

        // Base64 rather than byte[]: the DiGi serializer writes a byte[] as a JSON number array, about 2.6 times larger.
        [JsonInclude, JsonPropertyName(nameof(ShadingFactors))]
        private readonly string? shadingFactors = null;

        [JsonInclude, JsonPropertyName(nameof(SkyVisibility))]
        private readonly double skyVisibility = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceSolarRadiation"/> class.
        /// <para>The constructor assigns the values as given; quantizing solved shading factors into bytes is done by <see cref="Convert.ToSystem_Bytes(System.Collections.Generic.IEnumerable{double})"/> before the call.</para>
        /// </summary>
        /// <param name="reference">The <c>GuidReference</c> string of the building component the surface belongs to.</param>
        /// <param name="area">The area of the surface, in m².</param>
        /// <param name="normal">The outward unit normal of the surface. It is cloned.</param>
        /// <param name="skyVisibility">The unblocked share of the isotropic sky view factor, 0..1.</param>
        /// <param name="groundVisibility">The unblocked share of the isotropic ground view factor, 0..1.</param>
        /// <param name="irradiation">The shaded irradiation over the weather period, in kWh/m² (the sum of <paramref name="beam"/>, <paramref name="diffuse"/> and <paramref name="ground"/>).</param>
        /// <param name="beam">The beam irradiation over the weather period, in kWh/m² of the whole surface.</param>
        /// <param name="diffuse">The sky diffuse irradiation over the weather period, in kWh/m².</param>
        /// <param name="ground">The ground-reflected irradiation over the weather period, in kWh/m².</param>
        /// <param name="irradiationUnshaded">The irradiation over the weather period under an open sky, in kWh/m².</param>
        /// <param name="energy">The energy received by the whole surface over the weather period, in kWh.</param>
        /// <param name="shadingFactors">The quantized hourly shading factors, one byte per hour of the solar reference year, or null. They are stored as base64.</param>
        public SurfaceSolarRadiation(
            string? reference,
            double area,
            Vector3D? normal,
            double skyVisibility,
            double groundVisibility,
            double irradiation,
            double beam,
            double diffuse,
            double ground,
            double irradiationUnshaded,
            double energy,
            byte[]? shadingFactors)
            : base()
        {
            this.reference = reference;
            this.area = area;
            this.normal = Core.Query.Clone(normal);
            this.skyVisibility = skyVisibility;
            this.groundVisibility = groundVisibility;
            this.irradiation = irradiation;
            this.beam = beam;
            this.diffuse = diffuse;
            this.ground = ground;
            this.irradiationUnshaded = irradiationUnshaded;
            this.energy = energy;
            this.shadingFactors = shadingFactors is null ? null : System.Convert.ToBase64String(shadingFactors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceSolarRadiation"/> class by copying an existing instance.
        /// </summary>
        /// <param name="surfaceSolarRadiation">The <see cref="SurfaceSolarRadiation"/> to copy.</param>
        public SurfaceSolarRadiation(SurfaceSolarRadiation? surfaceSolarRadiation)
            : base(surfaceSolarRadiation)
        {
            if (surfaceSolarRadiation != null)
            {
                reference = surfaceSolarRadiation.reference;
                area = surfaceSolarRadiation.area;
                normal = Core.Query.Clone(surfaceSolarRadiation.normal);
                skyVisibility = surfaceSolarRadiation.skyVisibility;
                groundVisibility = surfaceSolarRadiation.groundVisibility;
                irradiation = surfaceSolarRadiation.irradiation;
                beam = surfaceSolarRadiation.beam;
                diffuse = surfaceSolarRadiation.diffuse;
                ground = surfaceSolarRadiation.ground;
                irradiationUnshaded = surfaceSolarRadiation.irradiationUnshaded;
                energy = surfaceSolarRadiation.energy;
                shadingFactors = surfaceSolarRadiation.shadingFactors;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceSolarRadiation"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the surface solar radiation data.</param>
        public SurfaceSolarRadiation(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the area of the surface, in m².
        /// </summary>
        [JsonIgnore]
        public double Area
        {
            get
            {
                return area;
            }
        }

        /// <summary>
        /// Gets the beam irradiation over the weather period, in kWh/m² of the whole surface.
        /// </summary>
        [JsonIgnore]
        public double Beam
        {
            get
            {
                return beam;
            }
        }

        /// <summary>
        /// Gets the sky diffuse irradiation over the weather period, in kWh/m².
        /// </summary>
        [JsonIgnore]
        public double Diffuse
        {
            get
            {
                return diffuse;
            }
        }

        /// <summary>
        /// Gets the energy received by the whole surface over the weather period, in kWh.
        /// </summary>
        [JsonIgnore]
        public double Energy
        {
            get
            {
                return energy;
            }
        }

        /// <summary>
        /// Gets the ground-reflected irradiation over the weather period, in kWh/m².
        /// </summary>
        [JsonIgnore]
        public double Ground
        {
            get
            {
                return ground;
            }
        }

        /// <summary>
        /// Gets the unblocked share of the isotropic ground view factor, 0..1.
        /// </summary>
        [JsonIgnore]
        public double GroundVisibility
        {
            get
            {
                return groundVisibility;
            }
        }

        /// <summary>
        /// Gets the shaded irradiation over the weather period, in kWh/m² (the sum of <see cref="Beam"/>, <see cref="Diffuse"/> and <see cref="Ground"/>).
        /// </summary>
        [JsonIgnore]
        public double Irradiation
        {
            get
            {
                return irradiation;
            }
        }

        /// <summary>
        /// Gets the irradiation over the weather period under an open sky, in kWh/m².
        /// </summary>
        [JsonIgnore]
        public double IrradiationUnshaded
        {
            get
            {
                return irradiationUnshaded;
            }
        }

        /// <summary>
        /// Gets a copy of the outward unit normal of the surface, or null.
        /// </summary>
        [JsonIgnore]
        public Vector3D? Normal
        {
            get
            {
                return Core.Query.Clone(normal);
            }
        }

        /// <summary>
        /// Gets the <c>GuidReference</c> string of the building component the surface belongs to.
        /// </summary>
        [JsonIgnore]
        public string? Reference
        {
            get
            {
                return reference;
            }
        }

        /// <summary>
        /// Gets a new array of the quantized hourly shading factors, or null when none are stored.
        /// <para>One byte per hour of the solar reference year (8 760 values, index = hour of the year, sampled at mid-hour), each <c>round(clamp(f, 0, 1) · 255)</c>; 0 where the sun is down. The factors depend on the sun position only, so they are valid for any weather source. Decode with <see cref="Convert.ToSystem_Doubles(byte[])"/>.</para>
        /// </summary>
        [JsonIgnore]
        public byte[]? ShadingFactors
        {
            get
            {
                return shadingFactors is null ? null : System.Convert.FromBase64String(shadingFactors);
            }
        }

        /// <summary>
        /// Gets the unblocked share of the isotropic sky view factor, 0..1.
        /// </summary>
        [JsonIgnore]
        public double SkyVisibility
        {
            get
            {
                return skyVisibility;
            }
        }
    }
}
