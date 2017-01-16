using System;
using System.Collections.Generic;

namespace bDiscord.Classes
{
    class Tabulate
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
        public string convert(string[][] table)
        {
            List<int> spacing = getColumnsLongest(table);
            string response = "```";

            for (int i = 0; i < table.Length; i++)
            {
                response += "\n" + joinRow(table[i], spacing);
            }
            return response + "```";
        }

        private string joinRow(string[] row, List<int> spacing)
        {
            string res = "";
            for (int i = 0; i < row.Length; i++)
            {
                string form = "{0, -" + spacing[i] + "} ";
                res += String.Format(form, row[i]);
            }
            return res;
        }

        private List<int> getColumnsLongest(string[][] table)
        {
            List<int> longest = new List<int>();
            
            for (int i = 0; i < table[0].Length; i++)
            {
                longest.Add(colLongest(table, i));
            }

            return longest;
        }

        private int colLongest(string[][] table, int column)
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
