using DiGi.GIS.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Groups orthophoto index rows that register the same image, within a typename and across typenames.
        /// <para>Two rows are the same image when either holds (the groups are the union of both):</para>
        /// <para>A — they share a <see cref="OrtoSheet.DownloadUrl"/> (one physical file).</para>
        /// <para>B — they share <see cref="OrtoSheet.SheetId"/>, <see cref="OrtoSheet.Date"/>, <see cref="OrtoSheet.PixelSize"/>, <see cref="OrtoSheet.ColorMode"/> and <see cref="OrtoSheet.OrtoType"/>. Each registration of an image gets a fresh file, so rule A alone misses re-registrations: sheet <c>N-34-139-C-c-1-1</c> appears twice in 1976 with different files, both black and white at 0.1 m, both registered on 2025-06-02 (324 such groups in the national index of 2026-11-03).</para>
        /// <para>The colour mode is part of the key because one flight is published as a CIR and an RGB composite, two different files: sheet <c>N-34-138-B-d-1-4</c> appears twice in 2024, both dated 2024-07-18 at 0.25 m, once CIR and once RGB. Without it 277 043 groups of the national index (2026-11-03, 1 337 341 rows) joined CIR and RGB files, and keeping one canonical row dropped the other product. The type is part of the key so a true orthophoto is never merged with a standard sheet: the two products share sheet identifiers and flights (verified 2026-11-03 in Warsaw: <c>N-34-138-D-d-2-4</c> is a standard sheet dated 2025-04-27 and a true orthophoto sheet dated 2025-10-21, the day of standard sheet <c>7.169.20.05</c>).</para>
        /// <para>Neither rule merges by sheet identifier alone: the same sheet in 2025 with a different file, date (2025-04-27) and resolution (0.05 m) is a genuine re-acquisition.</para>
        /// <para>Every row is returned in exactly one group, singletons included. Within a group the canonical row comes first: the earliest <see cref="OrtoSheet.Year"/>, then the earliest <see cref="OrtoSheet.RegisterDate"/>.</para>
        /// </summary>
        /// <param name="ortoSheets">The rows to group; nulls are ignored.</param>
        /// <returns>The groups, ordered by their canonical rows, or null when the input is null.</returns>
        public static List<List<OrtoSheet>>? OrtoSheetDuplicateGroups(this IEnumerable<OrtoSheet?>? ortoSheets)
        {
            if (ortoSheets == null)
            {
                return null;
            }

            List<OrtoSheet> ortoSheets_Temp = [];
            foreach (OrtoSheet? ortoSheet in ortoSheets)
            {
                if (ortoSheet != null)
                {
                    ortoSheets_Temp.Add(ortoSheet);
                }
            }

            int[] parents = new int[ortoSheets_Temp.Count];
            for (int i = 0; i < parents.Length; i++)
            {
                parents[i] = i;
            }

            Dictionary<string, int> indexes_DownloadUrl = [];
            Dictionary<string, int> indexes_SheetDate = [];

            for (int i = 0; i < ortoSheets_Temp.Count; i++)
            {
                OrtoSheet ortoSheet = ortoSheets_Temp[i];

                if (!string.IsNullOrWhiteSpace(ortoSheet.DownloadUrl))
                {
                    Join(indexes_DownloadUrl, ortoSheet.DownloadUrl!, i);
                }

                if (!string.IsNullOrWhiteSpace(ortoSheet.SheetId) && ortoSheet.Date is DateTimeOffset date)
                {
                    string pixelSize = ortoSheet.PixelSize is double value ? value.ToString("R", CultureInfo.InvariantCulture) : string.Empty;
                    Join(indexes_SheetDate, ortoSheet.SheetId + "|" + date.UtcTicks.ToString(CultureInfo.InvariantCulture) + "|" + pixelSize + "|" + ((int)ortoSheet.ColorMode).ToString(CultureInfo.InvariantCulture) + "|" + ((int)ortoSheet.OrtoType).ToString(CultureInfo.InvariantCulture), i);
                }
            }

            Dictionary<int, List<OrtoSheet>> dictionary = [];
            for (int i = 0; i < ortoSheets_Temp.Count; i++)
            {
                int root = Find(i);
                if (!dictionary.TryGetValue(root, out List<OrtoSheet>? group))
                {
                    group = [];
                    dictionary[root] = group;
                }

                group.Add(ortoSheets_Temp[i]);
            }

            List<List<OrtoSheet>> result = [];
            foreach (List<OrtoSheet> group in dictionary.Values)
            {
                result.Add(group.OrderBy(x => x.Year).ThenBy(x => x.RegisterDate ?? DateTimeOffset.MaxValue).ToList());
            }

            return result.OrderBy(x => x[0].Year).ThenBy(x => x[0].SheetId, StringComparer.Ordinal).ThenBy(x => x[0].DownloadUrl, StringComparer.Ordinal).ToList();

            void Join(Dictionary<string, int> indexes, string key, int index)
            {
                if (indexes.TryGetValue(key, out int index_Existing))
                {
                    int root_1 = Find(index_Existing);
                    int root_2 = Find(index);
                    if (root_1 != root_2)
                    {
                        parents[root_2] = root_1;
                    }
                }
                else
                {
                    indexes[key] = index;
                }
            }

            int Find(int index)
            {
                while (parents[index] != index)
                {
                    parents[index] = parents[parents[index]];
                    index = parents[index];
                }

                return index;
            }
        }
    }
}
