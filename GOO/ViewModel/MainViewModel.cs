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

            Environment.Exit(0);
        }

        private void SolveSolution()
        {
            // Initialization of variables
            Solution solution = null;
            System.Diagnostics.Stopwatch sw = null;
            long ClustersGenerationTimeInMiliSeconds = 0;
            long SolutionGenerationTimeInMiliSeconds = 0;
            double SolutionScore = 0.0;
            string solutionString = "";

            // Start generating clusters
            sw = System.Diagnostics.Stopwatch.StartNew();
            solution = Solver.generateClusters();
            sw.Stop();
            ClustersGenerationTimeInMiliSeconds = sw.ElapsedMilliseconds;
            Console.WriteLine("Elapsed time for generating clusters: {0:N}ms", ClustersGenerationTimeInMiliSeconds);
            // Clusters generated.

            ProgressMaximum = Solver.getMaximumNumberOfSAIterations();
            // update progress along the way

            // Generating and optimizing solution
            sw.Restart();
            solution = Solver.optimizeSolution(solution, this);
            sw.Stop();
            SolutionGenerationTimeInMiliSeconds = sw.ElapsedMilliseconds;
            Console.WriteLine("Elapsed time for generating optimized solution: {0:N}ms", SolutionGenerationTimeInMiliSeconds);
            SolutionScore = solution.SolutionScore;
            solutionString = solution.ToString();
            // Solution generated and optimized


            System.Windows.MessageBoxResult SaveSolutions = System.Windows.MessageBox.Show(
                "Do you want to save this solution?", "Save your solution?", System.Windows.MessageBoxButton.YesNo);
            if (SaveSolutions == System.Windows.MessageBoxResult.Yes)
                SaveSolution(solutionString);

            System.Windows.MessageBoxResult confirmResult = System.Windows.MessageBox.Show("Do you want to exit the application or run the application again?", "Exit confirmation", System.Windows.MessageBoxButton.YesNo);
            if (confirmResult == System.Windows.MessageBoxResult.No)
                SolveSolution();
        }

        private void SaveSolution(String solutionString)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            sfd.FileName = "Solution";
            sfd.DefaultExt = ".txt";
            sfd.Filter = "Text documents (.txt)|*.txt";

            if (sfd.ShowDialog() == true)
            {
                string filename = sfd.FileName;
                System.IO.File.WriteAllText(filename, solutionString);
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