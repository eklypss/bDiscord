using System.Collections.Generic;

namespace bDiscord.Source
{
    internal class Tabulate
    {
        /// <summary>
        /// Convert Two-dimensional list to a nice table string.
        /// </summary>
        /// <param name="table">
        /// Two dimensional array where outer lists are rows
        /// </param>
        /// <returns>
        /// Table string with code prefix and suffix for discord monospaced formatting.
        /// </returns>
        public string Convert(string[][] table)
        {
            List<int> spacing = GetColumnsLongest(table);
            string response = "```";

            for (int i = 0; i < table.Length; i++)
            {
                response += "\n" + JoinRow(table[i], spacing);
            }
            return response + "```";
        }

        private string JoinRow(string[] row, List<int> spacing)
        {
            string res = string.Empty;
            for (int i = 0; i < row.Length; i++)
            {
                string form = "{0, -" + spacing[i] + "} ";
                res += string.Format(form, row[i]);
            }
            return res;
        }

        private List<int> GetColumnsLongest(string[][] table)
        {
            List<int> longest = new List<int>();

            for (int i = 0; i < table[0].Length; i++)
            {
                longest.Add(ColLongest(table, i));
            }

            return longest;
        }

        private int ColLongest(string[][] table, int column)
        {
            int longest = -1;
            for (int row = 0; row < table.Length; row++)
            {
                int length = table[row][column].Length;
                if (length > longest) longest = length;
            }

            return longest + 1;
        }
    }
}