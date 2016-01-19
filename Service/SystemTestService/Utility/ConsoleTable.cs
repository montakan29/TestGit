﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTestService.Utility
{
    public class ConsoleTable
    {
        public IList<object> Columns { get; protected set; }
        public IList<object[]> Rows { get; protected set; }

        public ConsoleTable(params string[] columns)
        {
            Columns = new List<object>(columns);
            Rows = new List<object[]>();
        }

        public ConsoleTable AddColumn(IEnumerable<string> names)
        {
            foreach (var name in names)
                Columns.Add(name);

            return this;
        }

        public ConsoleTable AddRow(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (!Columns.Any())
                throw new Exception("Please set the columns first");

            if (Columns.Count != values.Length)
                throw new Exception(string.Format("The number columns in the row ({0}) does not match the values ({1}",
                    Columns.Count, values.Length));

            Rows.Add(values);
            return this;
        }

        public static ConsoleTable From<T>(IEnumerable<T> values)
        {
            var table = new ConsoleTable();

            var columns = typeof (T).GetProperties().Select(x => x.Name).ToArray();
            table.AddColumn(columns);

            foreach (
                var propertyValues in
                    values.Select(
                        value => columns.Select(column => typeof (T).GetProperty(column).GetValue(value, null))))
                table.AddRow(propertyValues.ToArray());

            return table;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            // find the longest column by searching each row
            var columnLengths = Columns
                .Select((t, i) => Rows.Select(x => x[i])
                    .Union(Columns)
                    .Where(x => x != null)
                    .Select(x => x.ToString().Length).Max())
                .ToList();

            // create the string format with padding
            var format = Enumerable.Range(0, Columns.Count)
                .Select(i => " | {" + i + ", -" + columnLengths[i] + " }")
                .Aggregate((s, a) => s + a) + " |";

            var results = new List<string>();

            // find the longest formatted line
            var maxRowLength = Math.Max(0, Rows.Any() ? Rows.Max(row => string.Format(format, row).Length) : 0);
            var columnHeaders = string.Format(format, Columns.ToArray());

            // longest line is greater of formatted columnHeader and longest row
            var longestLine = Math.Max(maxRowLength, columnHeaders.Length);

            // add each row
            Array.ForEach(Rows.Select(row => string.Format(format, row)).ToArray(), results.Add);

            // create the divider
            var divider = " " + string.Join("", Enumerable.Repeat("-", longestLine - 1)) + " ";

            builder.AppendLine(divider);
            builder.AppendLine(columnHeaders);

            foreach (var row in results)
            {
                builder.AppendLine(divider);
                builder.AppendLine(row);
            }

            builder.AppendLine(divider);
            builder.AppendLine("");
            builder.AppendFormat(" Count: {0}", Rows.Count);

            return builder.ToString();
        }
    }
}
