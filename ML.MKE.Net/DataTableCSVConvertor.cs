// <copyright file="DataTableCSVConvertor.cs" company="ge.com">
//     General Electric. All rights reserved.
// </copyright>
namespace ML.MKE.Net
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Converts between CDV files and Data tables.
    /// </summary>
    public static class DataTableCsvConvertor
    {
        /// <summary>
        /// Turns a Comma Separated File into a ADO.Net data table.
        /// </summary>
        /// <param name="filePath">Path to CSV file</param>
        /// <param name="isFirstRowHeader">Indicates if the first row is a header</param>
        /// <returns>ADO.Net data table</returns>
        public static DataTable GetDataTableFromCsv(FileInfo filePath, bool isFirstRowHeader = true)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath", "filePath cannot be a null reference (Nothing in Visual Basic)");
            }

            if (!File.Exists(filePath.FullName))
            {
                throw new FileNotFoundException("Unable to open file.", filePath.FullName);
            }

            string header = isFirstRowHeader ? "Yes" : "No";

            string sql = @"SELECT * FROM [" + filePath.Name + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath.DirectoryName +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            {
                using (OleDbCommand command = new OleDbCommand(sql, connection))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Locale = CultureInfo.CurrentCulture;
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        /// <summary>
        /// Saves ADO.Net data table to a Comma Separated Value file
        /// </summary>
        /// <param name="dataTable">The ADO.Net data table</param>
        /// <param name="filePath">The path to the Comma Separated Value file</param>
        public static void SaveDataTableToCsv(DataTable dataTable, FileInfo filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath", "filePath cannot be a null reference (Nothing in Visual Basic)");
            }

            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable", "dataTable cannot be a null reference (Nothing in Visual Basic)");
            }

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filePath.FullName, sb.ToString());
        }
    }
}
