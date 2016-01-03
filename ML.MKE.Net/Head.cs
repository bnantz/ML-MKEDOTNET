// <copyright file="Head.cs" company="ge.com">
//     General Electric. All rights reserved.
// </copyright>
namespace ML.MKE.Net
{
    using System.Data;
    using System.Text;
    using log4net;

    /// <summary>
    /// Prints out the head (or first few rows) of an ADO.Net data table
    /// </summary>
    public static class Head
    {
        /// <summary>
        /// The Logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Head));

        /// <summary>
        /// Prints out the head (or first few rows) of an ADO.Net data table
        /// </summary>
        /// <param name="dataTable">An ADO.Net data table</param>
        public static void PrintHead(DataTable dataTable)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DataColumn column in dataTable.Columns)
            {
                stringBuilder.AppendFormat("{0}|", column.ColumnName);
            }

            Logger.Info(stringBuilder.ToString());

            stringBuilder = new StringBuilder();
            for (int index = 0; index < 5; index++)
            {
                foreach (var item in dataTable.Rows[index].ItemArray)
                {
                    stringBuilder.AppendFormat("{0}|", item);
                }

                Logger.Info(stringBuilder.ToString());
            }
        }
    }
}
