using System;

using GOO.Model;
using GOO.Utilities;

namespace GOO.ViewModel
{
    public class MainViewModel : INPC
    {
        public MainViewModel()
        {
            ProgressMaximum = 1;
            ProgressValue = 0;
        }

        public void InitialRun()
        {
            FilesInitializer.InitializeFiles();
            SolveSolution();

            System.Environment.Exit(0);
        }

        private void SolveSolution()
        {
            Solution solution = null;

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            // Solution.GenerateBasicSolution();
            solution = Solver.generateSolution();
            sw.Stop();
            long BasicSolutionGenerationTimeInMiliSeconds = sw.ElapsedMilliseconds;
            string basicSolutionString = solution.ToString();

            sw.Restart();
            // Solution.GenerateOptimizedSolution();
            solution = Solver.optimizeSolution(solution);
            sw.Stop();
            long OptimizedSolutionGenerationTimeInMiliSeconds = sw.ElapsedMilliseconds;
            string optimizedSolutionString = solution.ToString();

            System.Windows.MessageBox.Show(
                String.Format("The basic solution generated in: {0}ms\nThe optimized solution generated in: {1}ms\n",
                    BasicSolutionGenerationTimeInMiliSeconds, OptimizedSolutionGenerationTimeInMiliSeconds),
                "Solution Generation", System.Windows.MessageBoxButton.OK);

            SaveSolution(basicSolutionString, optimizedSolutionString);

            System.Windows.MessageBoxResult confirmResult = System.Windows.MessageBox.Show("Do you want to exit the application?", "Exit confirmation", System.Windows.MessageBoxButton.YesNo);

            if (confirmResult == System.Windows.MessageBoxResult.No)
                SolveSolution();
        }

        private void SaveSolution(String basicSolutionString, String optimizedSolutionString)
        {
            Microsoft.Win32.SaveFileDialog sfdStartSolution = new Microsoft.Win32.SaveFileDialog();
            sfdStartSolution.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            sfdStartSolution.FileName = "StartSolution";
            sfdStartSolution.DefaultExt = ".txt";
            sfdStartSolution.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = sfdStartSolution.ShowDialog();
            if (result == true)
            {
                string filename = sfdStartSolution.FileName;
                System.IO.File.WriteAllText(filename, basicSolutionString);

                Microsoft.Win32.SaveFileDialog sfdOptimizedSolution = new Microsoft.Win32.SaveFileDialog();
                sfdOptimizedSolution.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                sfdOptimizedSolution.FileName = "StartSolution";
                sfdOptimizedSolution.DefaultExt = ".txt";
                sfdOptimizedSolution.Filter = "Text documents (.txt)|*.txt";

                result = sfdOptimizedSolution.ShowDialog();
                if (result == true)
                {
                    filename = sfdOptimizedSolution.FileName;
                    System.IO.File.WriteAllText(filename, optimizedSolutionString);
                }
            }
        }

        private double progressMaximum;
        public double ProgressMaximum
        {
            get { return progressMaximum; }
            set
            {
                if (value < 1)
                    value = 1;

                progressMaximum = value;
                OnPropertyChanged("ProgressMaximum");
            }
        }

        private double progressValue;
        public double ProgressValue
        {
            get { return progressValue; }
            set
            {
                if (value < 0)
                    value = 0;

                progressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }
    }
}