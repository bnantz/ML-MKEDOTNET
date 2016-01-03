// <copyright file="DataImputation.cs" company="ge.com">
//     General Electric. All rights reserved.
// </copyright>
namespace ML.MKE.Net
{
    using System;
    using System.Data;
    using System.Globalization;

    /// <summary>
    /// Class to handle missing data
    /// </summary>
    public static class DataImputation
    {
        /// <summary>
        /// Removes any rows in the data table that have missing data.
        /// </summary>
        /// <param name="dataTable">The data table</param>
        /// <returns>The updated data table</returns>
        public static DataTable RemoveMissing(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    if (item == DBNull.Value || 
                        string.Compare(Convert.ToString(item, CultureInfo.CurrentCulture), "NULL", true, CultureInfo.CurrentCulture) == 0)
                    {
                        row.Delete();
                    }
                }
            }

            dataTable.AcceptChanges();

            return dataTable;
        }
    }
}
