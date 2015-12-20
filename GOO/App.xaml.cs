using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

using GOO.Model;
using GOO.Model.Optimizers;
using GOO.Utilities;
using GOO.View;
using GOO.ViewModel;

namespace GOO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #if DEBUG
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
        #endif

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            #if DEBUG
            AllocConsole();
            #endif
            Console.WriteLine("Program booting up...");

            FilesInitializer.InitializeFiles();
            Console.WriteLine("Files have been processed");

            Stopwatch sw = Stopwatch.StartNew();
            Solver.generateSolution();
            sw.Stop();

            Console.WriteLine("Elapsed time for generating clusters: {0}ms", sw.ElapsedMilliseconds);
            Console.WriteLine();

            string THE_CLUSTER_SOLUTION = "";
            sw = Stopwatch.StartNew();
            //THE_CLUSTER_SOLUTION = KSolver.generateRouteSolution();
            sw.Stop();

            Console.WriteLine("Elapsed time for generating clusters as route solution: {0}ms", sw.ElapsedMilliseconds);
            Console.WriteLine();

            // Temporarily
            Console.ReadKey();
            FreeConsole();
            Environment.Exit(0);

            //new MainView() { DataContext = new MainViewModel() }.Show();
        }

        /*
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            #if DEBUG
            AllocConsole();
            #endif
            Console.WriteLine("Program booting up...");

            FilesInitializer.InitializeFiles();
            Console.WriteLine("Files have been processed");

            Stopwatch sw = Stopwatch.StartNew();
            Solution THE_SOLUTION = new Solution();
            THE_SOLUTION.GenerateSolution();
            sw.Stop();
            Console.WriteLine("Elapsed time for generating route: {0}ms", sw.ElapsedMilliseconds);

            Console.WriteLine();
            string THE_SOLUTION_STRING;
            sw = Stopwatch.StartNew();
            THE_SOLUTION_STRING = THE_SOLUTION.ToString();
            sw.Stop();
            Console.WriteLine("Elapsed time for generating the solution string: {0}ms", sw.ElapsedMilliseconds);
            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/StartSolution.txt", THE_SOLUTION_STRING);
            Console.WriteLine();
            Console.ReadKey();

            Console.WriteLine("Commence the Simulated Annealing!");
            sw = Stopwatch.StartNew();
            SimulatedAnnealingOptimizer optimizer = new SimulatedAnnealingOptimizer();
            Solution OPTIMAL_PRIME = optimizer.runOptimizer(THE_SOLUTION);
            THE_SOLUTION_STRING = OPTIMAL_PRIME.ToString();
            sw.Stop();

            Console.WriteLine("Elapsed time for generating the optimal solution string: {0}ms", sw.ElapsedMilliseconds);
            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/OptimalSolution.txt", THE_SOLUTION_STRING);
            Console.WriteLine();

            // Temporarily
            Console.ReadKey();
            FreeConsole();
            Environment.Exit(0);

            //new MainView() { DataContext = new MainViewModel() }.Show();
        }
        */

        /* Santa's TODO list

           - On the first day of Christmas, Santa gave this advice to me:   Do not DeepCopy the entire OrdersCounter object in the solution class, only DeepCopy the OrderCounter items that need to be changed
           - On the second day of Christmas, Santa gave this advice to me:  Optimize the ToString method in Solution class, by recycling
           - On the third day of Christmas, Santa gave this advice to me:   Utilize the window by using a progressbar for Simulated Annealing
           - On the fourth day of Christmas, Santa gave this advice to me:  Make the following stategies: 
           - On the fifth day of Christmas, Santa gave this advice to me:   
           - On the sixth day of Christmas, Santa gave this advice to me:   
           - On the seventh day of Christmas, Santa gave this advice to me: 
           - On the eighth day of Christmas, Santa gave this advice to me:  
           - On the ninth day of Christmas, Santa gave this advice to me:   
           - On the tenth day of Christmas, Santa gave this advice to me:   
           - 
        
        */
    }
}