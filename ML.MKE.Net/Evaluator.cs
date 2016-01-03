// <copyright file="Evaluator.cs" company="ge.com">
//     General Electric. All rights reserved.
// </copyright>
namespace ML.MKE.Net
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Analysis;
    using log4net;

    /// <summary>
    /// Evaluates an Accord.Net machine learning algorithm
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Evaluator));

        /// <summary>
        ///  Evaluates an Accord.Net machine learning algorithm
        /// </summary>
        /// <param name="test">An ADO.Net data table</param>
        /// <param name="obj">The Accord.Net object</param>
        public static void Evaluate(DataTable test, object obj)
        {
            // Evaluate 
            DataTable testResults = new DataTable();
            testResults.Locale = CultureInfo.CurrentCulture;
            testResults.Columns.Add("DMIndicator");
            testResults.Columns.Add("PredictedResults");

            List<int> expected = new List<int>();
            List<int> predicted = new List<int>();

            foreach (DataRow row in test.Rows)
            {
                double gender = 1;
                if (string.Compare((string)row["Gender"], "F", true, CultureInfo.CurrentCulture) == 0)
                {
                    gender = 0;
                }

                double[] testQuery = new double[] 
                                                 {
                                                    gender, Convert.ToDouble(row["YearOfBirth"], CultureInfo.CurrentCulture), Convert.ToDouble(row["SmokingEffectiveYear"], CultureInfo.CurrentCulture), Convert.ToDouble(row["NISTcode"], CultureInfo.CurrentCulture),
                                                    Convert.ToDouble(row["Height"], CultureInfo.CurrentCulture), Convert.ToDouble(row["Weight"], CultureInfo.CurrentCulture), Convert.ToDouble(row["BMI"], CultureInfo.CurrentCulture),
                                                    Convert.ToDouble(row["SystolicBP"], CultureInfo.CurrentCulture), Convert.ToDouble(row["DiastolicBP"], CultureInfo.CurrentCulture), Convert.ToDouble(row["RespiratoryRate"], CultureInfo.CurrentCulture),
                                                    Convert.ToDouble(row["Temperature"], CultureInfo.CurrentCulture)
                                                  };

                int output = -1;
                if (obj is DecisionTree)
                {
                    output = ((DecisionTree)obj).Compute(testQuery);
                }
                else if (obj is MulticlassSupportVectorMachine)
                {
                    output = ((MulticlassSupportVectorMachine)obj).Compute(testQuery);
                }
                else
                {
                    throw new ArgumentException("Unknown algorithm for validation.");
                }

                DataRow resultRow = testResults.NewRow();
                resultRow["DMIndicator"] = row["DMIndicator"];
                resultRow["PredictedResults"] = output;
                testResults.Rows.Add(resultRow);

                expected.Add(Convert.ToInt32(row["DMIndicator"], CultureInfo.CurrentCulture));
                predicted.Add(output);
            }

            // Save the results to a CSV file
            FileInfo treeOutputFileInfo = new FileInfo("treeOutput.csv");
            DataTableCsvConvertor.SaveDataTableToCsv(testResults, treeOutputFileInfo);

            var confusionMatrix = new ConfusionMatrix(predicted.ToArray(), expected.ToArray());
            Logger.Info("Accuracy :" + confusionMatrix.Accuracy);
            Logger.Info("Hit Enter to continue....");
            Console.ReadLine();
        }
    }
}
