using DiGi.Core.Classes;
using DiGi.Core.Enums;
using DiGi.GIS.Classes;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts an <see cref="AddressPoint"/> to a <see cref="Address"/>.
        /// <para>The conversion is <b>lossy</b>: the position, the ULIC and SIMC identifiers and the PRG identifier and version are dropped.</para>
        /// <para>The street is <c>"&lt;street&gt; &lt;house number&gt;"</c>; without a street it is <c>"&lt;locality&gt; &lt;house number&gt;"</c>, the Polish convention for villages without streets.</para>
        /// </summary>
        /// <param name="addressPoint">The source address point.</param>
        /// <returns>An <see cref="Address"/> in Poland; null when the input is null.</returns>
        public static Address? ToDiGi(this AddressPoint? addressPoint)
        {
            if (addressPoint is null)
            {
                return null;
            }

            string? name = string.IsNullOrWhiteSpace(addressPoint.StreetName) ? addressPoint.LocalityName : addressPoint.StreetName;

            string? street = name;
            if (!string.IsNullOrWhiteSpace(addressPoint.HouseNumber))
            {
                street = string.IsNullOrWhiteSpace(name) ? addressPoint.HouseNumber : string.Format("{0} {1}", name, addressPoint.HouseNumber);
            }

            return new Address(street, addressPoint.LocalityName, addressPoint.PostalCode, CountryCode.PL);
        }
    }
}
