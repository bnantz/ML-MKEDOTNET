// <copyright file="Validator.cs" company="ge.com">
//     General Electric. All rights reserved.
// </copyright>
namespace ML.MKE.Net
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Analysis;
    using log4net;

    /// <summary>
    /// Validates either an Accord.Net DecisionTree or MulticlassSupportVectorMachine class
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Validator));

        /// <summary>
        /// Validate than Accord.Net DecisionTree or MulticlassSupportVectorMachine class
        /// </summary>
        /// <param name="test">The test dataset</param>
        /// <param name="obj">The Accord.Net DecisionTree or MulticlassSupportVectorMachine object</param>
        public static void Validate(DataTable test, object obj)
        {
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

                expected.Add(Convert.ToInt32(row["DMIndicator"], CultureInfo.CurrentCulture));
                predicted.Add(output);
            }

            var confusionMatrix = new ConfusionMatrix(predicted.ToArray(), expected.ToArray());
            Logger.Info("The following is the confusion matrix (aka truth table).  Look for TP (true positive), FP (false positive), etc...");
            Logger.Info("Accuracy :" + confusionMatrix.ToString());
            Logger.Info("Hit Enter to continue....");
            Console.ReadLine();
        }
    }
}
