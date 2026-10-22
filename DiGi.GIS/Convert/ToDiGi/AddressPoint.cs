using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.PRG.Geometry.Classes;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts a PRG <see cref="PunktAdresowy"/> to an <see cref="AddressPoint"/>.
        /// <para>The position is taken as is: it is already easting first, the BDOT10k frame, so it must not be swapped again.</para>
        /// </summary>
        /// <param name="punktAdresowy">The source PRG address point, with its street and locality already resolved.</param>
        /// <returns>An <see cref="AddressPoint"/>; null when the input is null or has no position.</returns>
        public static AddressPoint? ToDiGi(this PunktAdresowy? punktAdresowy)
        {
            if (punktAdresowy is null)
            {
                return null;
            }

            Point2D? point2D = punktAdresowy.Geometry;
            if (point2D is null)
            {
                return null;
            }

            return new AddressPoint(
                point2D,
                punktAdresowy.ulicaNazwaPelna,
                punktAdresowy.identyfikatorULIC,
                punktAdresowy.numerPorzadkowy,
                punktAdresowy.miejscowoscNazwa,
                punktAdresowy.identyfikatorSIMC,
                punktAdresowy.kodPocztowy,
                punktAdresowy.lokalnyId,
                punktAdresowy.wersjaId);
        }
    }
}
