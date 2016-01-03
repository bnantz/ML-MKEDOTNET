// <copyright file="BasicStatistics.cs" company="ge.com">
//     General Electric. All rights reserved.
// </copyright>
namespace ML.MKE.Net
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using log4net;
    using MathNet.Numerics.Statistics;

    /// <summary>
    /// Class to the generate the basic descriptive statistics 
    /// </summary>
    public static class BasicStatistics
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BasicStatistics));

        /// <summary>
        /// Generates the basic descriptive statistics
        /// </summary>
        /// <param name="dataTable">The data table</param>
        public static void BasicStats(DataTable dataTable)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Now list all the column names
            foreach (DataColumn column in dataTable.Columns)
            {
                stringBuilder.AppendFormat("{0}|", column.ColumnName);
            }

            Logger.Info(stringBuilder.ToString());

            // Compute the statistics
            stringBuilder = new StringBuilder();
            stringBuilder.Append("nobs|");

            foreach (DataColumn column in dataTable.Columns)
            {
                int numberObservations = 0;

                foreach (DataRow row in dataTable.Rows)
                {
                    var item = row[column];
                    if (item != DBNull.Value)
                    {
                        numberObservations = numberObservations + 1;
                    }
                }

                stringBuilder.AppendFormat("{0}|", numberObservations);
            }

            Logger.Info(stringBuilder.ToString());

            stringBuilder = new StringBuilder();
            stringBuilder.Append("NAs|");

            foreach (DataColumn column in dataTable.Columns)
            {
                int numberNas = 0;

                foreach (DataRow row in dataTable.Rows)
                {
                    var item = row[column];
                    if (item == DBNull.Value)
                    {
                        numberNas = numberNas + 1;
                    }
                }

                stringBuilder.AppendFormat("{0}|", numberNas);
            }

            Logger.Info(stringBuilder.ToString());

            stringBuilder = new StringBuilder();
            foreach (DataColumn column in dataTable.Columns)
            {
                IList<double> data = new List<double>();

                foreach (DataRow row in dataTable.Rows)
                {
                    var item = row[column];
                    if (item != DBNull.Value &&
                        IsNumber(item))
                    {
                        data.Add(Convert.ToDouble(item, CultureInfo.CurrentCulture));
                    }
                }

                if (data.Count > 0)
                {
                    DescriptiveStatistics desc = new DescriptiveStatistics(data);

                    stringBuilder.AppendFormat("Minimum|{0}|", desc.Minimum);
                    stringBuilder.AppendFormat("Maximum|{0}|", desc.Maximum);
                    stringBuilder.AppendFormat("Mean|{0}|", desc.Mean);
                    stringBuilder.AppendFormat("StandardDeviation|{0}|", desc.StandardDeviation);
                    stringBuilder.AppendFormat("Variance|{0}|", desc.Variance);
                    stringBuilder.AppendFormat("Skewness|{0}|", desc.Skewness);
                    stringBuilder.AppendFormat("Kurtosis|{0}|", desc.Kurtosis);
                }

                Logger.Info(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// Helper class to determine if the object is numeric
        /// </summary>
        /// <param name="value">object to check</param>
        /// <returns>if the object is numeric</returns>
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }
}
