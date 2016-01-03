// <copyright file="Program.cs" company="ge.com">
//     General Electric. All rights reserved.
// </copyright>
namespace ML.MKE.Net
{
    using System;
    using System.Data;
    using System.IO;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Filters;
    using Accord.Statistics.Kernels;
    using log4net;
    using log4net.Config;

    /// <summary>
    /// Sample Machine Learning Command Line Program using C# Open Source Accord.Net Framework
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the program
        /// </summary>
        public static void Main()
        {
            try
            {
                #region Exploratory Data Analysis Explanation
                /*
                    John Tukey coined the term Exploratory Data Analysis in his seminal book of the same name.  There really is not a prescribed way to do an EDA.  
                    Tools I use for EDA include Microsoft Excel, plots and visual inspection of the data.  Without creating an early bias, gut feelings do play a role in a good EDA.
                    Some objectives of EDA are to:
                        •	Identify the types of data in the dataset
                        •	Examine the statistical properties of the data
                        •	Look for invalid data (may need Domain or Subject Matter experts)
                        •	Understand the provenance of the data
                        •	Aide in the selection of appropriate statistical tools and techniques

                    For our diabetes dataset, notice that there is both quantitative and qualitative data.  Note that the result or outcome variable (which indicates if the person has 
                    diabetes) is nominal data with only two states.  This is called dichotomous or binary categorical data which rules out some machine learning algorithms and directs 
                    us to others.
                */
                #endregion
                // Because of time constraints, the loading of the DataTables and EDA is complete.
                XmlConfigurator.Configure();

                Logger.Info("Exploratory Data Analysis");

                FileInfo fi = new FileInfo("training.csv");
                DataTable training = DataTableCsvConvertor.GetDataTableFromCsv(fi);

                fi = new FileInfo("test.csv");
                DataTable test = DataTableCsvConvertor.GetDataTableFromCsv(fi);

                // Print out the first few table rows.
                Head.PrintHead(training);

                //Logger.Info(string.Empty);
                //BasicStatistics.BasicStats(training); // For most EDA's Basic Descriptive statistics are important, but this outputs a lot of information

                #region Data Imputation & Cleanup Explanation
                /*
                    Keep in mind that Machine Learning algorithms operate on numerical data only, something will have to be done with the data is text or NULL.  Also predictor
                    variables(aka features or columns of data) that do not vary will not be predictive and may need to be removed.  Due to time constraints the EDA, ETL (Extract, Transform and Load)
                    and data cleaning is already completed in the solution.  For this analysis, the HeartRate column because it is all NULL and remove any rows of data that contain NULLs.
                */
                #endregion
                // Delete any columns that are not needed.
                training.Columns.Remove("HeartRate");
                test.Columns.Remove("HeartRate");

                // How to handle rows containing missing or NA data - data imputation or deletion?
                training = DataImputation.RemoveMissing(training);
                test = DataImputation.RemoveMissing(test);

                Codification codebook = new Codification(training);
                int outputClasses = 2;

                string[] inputColumns =
                {
                    "Gender", "YearOfBirth", "SmokingEffectiveYear", "NISTcode", "Height", "Weight", "BMI", "SystolicBP", "DiastolicBP", "RespiratoryRate", "Temperature"
                };

                string outputColumn = "DMIndicator";

                // Translate our training data into integer symbols using our codebook:
                DataTable symbols = codebook.Apply(training);
                double[][] inputs = symbols.ToArray(inputColumns);
                int[] outputs = Matrix.ToArray<int>(training, outputColumn);


                #region Decision Tree Overview
                /*
                    Decision Trees are very powerful, especially with a binary classification model, and are somewhat resistant to over-fitting the data.
                    Additionally, they are intuitive to explain to stakeholders.
                */
                #endregion
                Logger.Info(string.Empty);
                Logger.Info("Decision Tree");

                DecisionVariable[] attributes =
                {
                    new DecisionVariable("Gender", 2), // 2 possible values (Male, Female)
                    new DecisionVariable("YearOfBirth", DecisionVariableKind.Continuous),
                    new DecisionVariable("SmokingEffectiveYear", DecisionVariableKind.Continuous),
                    new DecisionVariable("NISTcode", DecisionVariableKind.Continuous),
                    new DecisionVariable("Height", DecisionVariableKind.Continuous),
                    new DecisionVariable("Weight", DecisionVariableKind.Continuous), 
                    new DecisionVariable("BMI", DecisionVariableKind.Continuous), 
                    new DecisionVariable("SystolicBP", DecisionVariableKind.Continuous),
                    new DecisionVariable("DiastolicBP", DecisionVariableKind.Continuous),
                    new DecisionVariable("RespiratoryRate", DecisionVariableKind.Continuous),
                    new DecisionVariable("Temperature", DecisionVariableKind.Continuous)
                };

                DecisionTree tree = new DecisionTree(attributes, outputClasses);

                C45Learning c45learning = new C45Learning(tree);

                // Learn the training instances!
                c45learning.Run(inputs, outputs);

                // The next two lines are optional to save the model into IL for future use.
                // Convert to an expression tree
                var expression = tree.ToExpression();
                // Compiles the expression to IL
                var func = expression.Compile();

                #region Evaluation Explanation
                /*
                    To evaluate the model, now use each row of the test dataset to predict the output variable (DMIndicator) using the DecisionTree’s compute method passing in the same
                    variables that were used to train the model.  Store the test dataset’s value of DMIndicator and the predicted value in a DataTable and integer collection for future 
                    validation of the model.
                */
                #endregion
                Evaluator.Evaluate(test, tree);

                #region Validation Explanation
                /*
                    There are many ways to validate models, but we will use a confusion matrix because it is intuitive and a very accepted way to validate binary classification models.
                    Most conveniently the Accord.Net has a ConfusionMatrix class to create this matrix for you.  Passing in the collection of integers of predicted and actual values 
                    stored earlier to the ConfusionMatrix class and output the matrix and accuracy.
                */
                #endregion
                Validator.Validate(test, tree);


                #region Support Vector Machine Overview
                /*
                    Support Vector Machines are powerful classification machine learning algorithms with very few knobs to turn.  The kernel of the SVM can be exchanged to use
                    a number of different mathematical algorithms including polynomials, neural networks and Gaussian functions.
                */
                #endregion
                Logger.Info(string.Empty);
                Logger.Info("Support Vector Machine");

                // Add SVM code here
                IKernel kernel = new Linear();

                // Create the Multi-class Support Vector Machine using the selected Kernel
                int inputDimension = inputs[0].Length;
                var ksvm = new MulticlassSupportVectorMachine(inputDimension, kernel, outputClasses);

                // Create the learning algorithm using the machine and the training data
                var ml = new MulticlassSupportVectorLearning(ksvm, inputs, outputs)
                {
                    Algorithm = (svm, classInputs, classOutputs, i, j) =>
                    {
                        return new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                        {
                            CacheSize = 0
                        };
                    }
                };

                double svmError = ml.Run();

                #region Evaluation Explanation
                /*
                    To evaluate the model, now use each row of the test dataset to predict the output variable (DMIndicator) using the DecisionTree’s compute method passing in the same
                    variables that were used to train the model.  Store the test dataset’s value of DMIndicator and the predicted value in a DataTable and integer collection for future 
                    validation of the model.
                */
                #endregion
                Evaluator.Evaluate(test, ksvm);

                #region Validation Explanation
                /*
                    There are many ways to validate models, but we will use a confusion matrix because it is intuitive and a very accepted way to validate binary classification models.
                    Most conveniently the Accord.Net has a ConfusionMatrix class to create this matrix for you.  Passing in the collection of integers of predicted and actual values 
                    stored earlier to the ConfusionMatrix class and output the matrix and accuracy.
                */
                #endregion
                Validator.Validate(test, ksvm);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
    }
}
