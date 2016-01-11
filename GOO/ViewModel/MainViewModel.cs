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
            System.Diagnostics.Stopwatch sw = null;
            long BasicSolutionGenerationTimeInMiliSeconds = 0;
            long OptimizedSolutionGenerationTimeInMiliSeconds = 0;
            double BasicSolutionScore = 0.0;
            double OptimizedSolutionScore = 0.0;
            string basicSolutionString = "";
            string optimizedSolutionString = "";
            
            sw = System.Diagnostics.Stopwatch.StartNew();
            // Solution.GenerateBasicSolution();
            solution = Solver.generateSolution();
            sw.Stop();
            BasicSolutionGenerationTimeInMiliSeconds = sw.ElapsedMilliseconds;
            BasicSolutionScore = solution.GetSolutionScore();
            basicSolutionString = solution.ToString();
            Console.ReadKey();

            // Determine Maxprogress (get SAO variables)
            // update Progress along the way
            sw.Restart();
            // Solution.GenerateOptimizedSolution();
            solution = Solver.optimizeSolution(solution);
            sw.Stop();
            OptimizedSolutionGenerationTimeInMiliSeconds = sw.ElapsedMilliseconds;
            OptimizedSolutionScore = solution.GetSolutionScore();
            optimizedSolutionString = solution.ToString();
            
            System.Windows.MessageBox.Show(
                String.Format("The basic solution generated in: {0}ms with a score of: {1}\nThe optimized solution generated in: {2}ms with a score of: {3}",
                    BasicSolutionGenerationTimeInMiliSeconds, BasicSolutionScore, OptimizedSolutionGenerationTimeInMiliSeconds, OptimizedSolutionScore),
                "Solution Generation", System.Windows.MessageBoxButton.OK);

            System.Windows.MessageBoxResult SaveSolutions = System.Windows.MessageBox.Show(
                "Do you want to save your solutions?", "Save your solutions?", System.Windows.MessageBoxButton.YesNo);
            if(SaveSolutions == System.Windows.MessageBoxResult.Yes)
                SaveSolution(basicSolutionString, optimizedSolutionString);

            System.Windows.MessageBoxResult confirmResult = System.Windows.MessageBox.Show("Do you want to exit the application or run the application again?", "Exit confirmation", System.Windows.MessageBoxButton.YesNo);
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
                sfdOptimizedSolution.FileName = "OptimizedSolution";
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