using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace TridionItemCreator
{
    public class ExcelReader
    {
        // Items to be created in the CM
        public IEnumerable<string> PendingItems { get; }

        public ExcelReader(string xlsxPath)
        {
            PendingItems = DeserializeFile(xlsxPath);
        }

        /// <summary>
        /// Deserialize the Excel file (.xlsx) into strings of columns converted to webDAV path (i.e. /Columnn A/Column B).
        /// </summary>
        /// <param name="xlsxPath">Path to the Excel file of keywords, folders or structure groups.</param>
        /// <returns></returns>
        public IEnumerable<string> DeserializeFile(string xlsxPath)
        {
            // Get the first table in the sheet if it exists.
            var table = GetDataTable(xlsxPath);

            if (table == null)
            {
                throw new InvalidDataException($"Couldn't get the Excel table {xlsxPath}.");
            }

            var objectPaths = new List<string>();

            for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                var path = string.Empty;
                for (var columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                {
                    var data = table.Rows[rowIndex][columnIndex];

                    if (!string.IsNullOrEmpty(data.ToString()))
                    {
                        path += data.ToString().Trim();

                        // Add the slash if we're not on the last loop
                        if (columnIndex < table.Columns.Count - 1) path += Constants.Slash;
                    }
                }

                // Make sure the path doesn't end on a slash
                if (path.EndsWith(Constants.Slash)) path = path.Substring(0, path.Length - 1);

                objectPaths.Add(path);
            }
            
            return objectPaths;
        }

        /// <summary>
        /// Get the first table returned in the file specified in the .xlsx path method parameter.
        /// </summary>
        /// <param name="xlsxPath"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string xlsxPath)
        {
            if (string.IsNullOrEmpty(xlsxPath)) return null;

            using (var stream = File.Open(xlsxPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    return reader.AsDataSet().Tables.Count < 1 ? null : reader.AsDataSet().Tables[0];
                }
            }
        }
    }

}