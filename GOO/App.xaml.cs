using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

using GOO.Model;
using GOO.Model.Optimizers;
using GOO.Utilities;
using GOO.View;
using GOO.ViewModel;
using GOO.Model.Optimizers.Strategies;

namespace GOO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AllocConsole();

            #if DEBUG
            Console.WriteLine("Program booting up...");
            FilesInitializer.InitializeFiles();
            Console.WriteLine("Files have been processed");

            Stopwatch sw = Stopwatch.StartNew();
            Solution start = Solver.generateSolution();
            sw.Stop();

            Console.WriteLine("Elapsed time for generating begin solution: {0}ms", sw.ElapsedMilliseconds);

            string THE_SOLUTION_STRING = start.ToString();

            Console.WriteLine("Start Solution:");
            Console.WriteLine(THE_SOLUTION_STRING);
            Console.WriteLine("End Solution");

            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/StartSolution.txt", THE_SOLUTION_STRING);

            sw.Restart();
            start = Solver.optimizeSolution(start);
            sw.Stop();
            string THE_NEW_STRING = start.ToString();

            Console.WriteLine("Start New Solution:");
            Console.WriteLine(THE_NEW_STRING);
            Console.WriteLine("End New Solution");
            Console.WriteLine("Elapsed time for generating begin solution: {0}ms", sw.ElapsedMilliseconds);

            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/OptimizedSolution.txt", THE_SOLUTION_STRING);

            #endif

            #if !DEBUG
            MainViewModel mainVM = new MainViewModel();
            new MainView() { DataContext = mainVM }.Show();
            System.Threading.Tasks.Task.Factory.StartNew(() => mainVM.InitialRun());
            #endif

            // Run Optimizers
        }
        #region Previous test code
        //        protected override void OnStartup(StartupEventArgs e)
//        {
//            base.OnStartup(e);
//#if DEBUG
//            AllocConsole();
//#endif
//            Console.WriteLine("Program booting up...");

//            FilesInitializer.InitializeFiles();
//            Console.WriteLine("Files have been processed");

//            Stopwatch sw = Stopwatch.StartNew();

//            Route testRoute = new Route(Days.Monday);
//            Random random = new Random();

//            List<int> alreadyUsedINTS = new List<int>();
//            Solution w00tSolution = new Solution(new List<Cluster>());
//            Dictionary<int, Order> allOrders = Data.Orders;
//            Order order;
//            for (int i = 1; i < 50; i++)
//            {
//                int index;
//                do
//                {
//                    index = random.Next(1, allOrders.Count);
//                }
//                while (alreadyUsedINTS.Contains(index));

//                alreadyUsedINTS.Add(index);
//                order = allOrders[allOrders.Keys.ToArray()[index]];
//                if (order.OrderNumber != 0)
//                    testRoute.AddOrder(order);
//                else
//                    i--;
//            }

//            w00tSolution.AddNewItemToPlanning(Days.Monday, 1, new List<Route>() { testRoute });
//            sw.Stop();
//            Console.WriteLine("Elapsed time for generating test solution: {0}ms", sw.ElapsedMilliseconds);

//            string THE_SOLUTION_STRING = w00tSolution.ToString();

//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/StartSolution.txt", THE_SOLUTION_STRING);


//            RandomRouteOpt3Strategy strategyOpt3 = new RandomRouteOpt3Strategy();
//            RandomRouteOpt3HalfStrategy strategyOpt3A = new RandomRouteOpt3HalfStrategy();
//            RandomRouteOpt2Strategy strategyOpt2 = new RandomRouteOpt2Strategy();
//            RandomRouteOpt2HalfStrategy strategyOpt2A = new RandomRouteOpt2HalfStrategy();

//            sw.Restart();
//            strategyOpt3.executeStrategy(w00tSolution);
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Opt3Solution.txt", THE_SOLUTION_STRING);
//            strategyOpt3.undoStrategy(w00tSolution);
//            sw.Stop();
//            Console.WriteLine("Elapsed time for Opt3 solution: {0}ms", sw.ElapsedMilliseconds);


//            sw.Restart();
//            strategyOpt3A.executeStrategy(w00tSolution);
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Opt3ASolution.txt", THE_SOLUTION_STRING);
//            strategyOpt3A.undoStrategy(w00tSolution);
//            sw.Stop();
//            Console.WriteLine("Elapsed time for Opt3.5 solution: {0}ms", sw.ElapsedMilliseconds);


//            sw.Restart();
//            strategyOpt2.executeStrategy(w00tSolution);
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Opt2Solution.txt", THE_SOLUTION_STRING);
//            strategyOpt2.undoStrategy(w00tSolution);
//            sw.Stop();
//            Console.WriteLine("Elapsed time for Opt2 solution: {0}ms", sw.ElapsedMilliseconds);


//            sw.Restart();
//            strategyOpt2A.executeStrategy(w00tSolution);
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Opt2ASolution.txt", THE_SOLUTION_STRING);
//            strategyOpt2A.undoStrategy(w00tSolution);
//            sw.Stop();
//            Console.WriteLine("Elapsed time for Opt2.5 solution: {0}ms", sw.ElapsedMilliseconds);


//            sw.Restart();
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/EndSolution.txt", THE_SOLUTION_STRING);

//            sw.Stop();
//            Console.WriteLine("Elapsed time for executing strategy : {0}ms", sw.ElapsedMilliseconds);
//        }


//        protected override void OnStartup(StartupEventArgs e)
//        {
//            base.OnStartup(e);
//#if DEBUG
//            AllocConsole();
//#endif
//            Console.WriteLine("Program booting up...");

//            FilesInitializer.InitializeFiles();
//            Console.WriteLine("Files have been processed");

//            Stopwatch sw = Stopwatch.StartNew();

//            Route testRoute = new Route(Days.Monday);
//            Random random = new Random();

//            List<int> alreadyUsedINTS = new List<int>();
//            Solution w00tSolution = new Solution(new List<Cluster>());
//            Dictionary<int, Order> allOrders = Data.Orders;
//            Order order;
//            for (int i = 1; i < 50; i++)
//            {
//                int index;
//                do{
//                    index = random.Next(1, allOrders.Count);
//                }
//                while(alreadyUsedINTS.Contains(index));

//                alreadyUsedINTS.Add(index);
//                order = allOrders[allOrders.Keys.ToArray()[index]];
//                if (order.OrderNumber != 0)
//                    testRoute.AddOrder(order);
//                else
//                    i--;
//            }

//            w00tSolution.AddNewItemToPlanning(Days.Monday, 1, new List<Route>() { testRoute });
//            sw.Stop();
//            Console.WriteLine("Elapsed time for generating test solution: {0}ms", sw.ElapsedMilliseconds);

//            string THE_SOLUTION_STRING = w00tSolution.ToString();

//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/StartSolution.txt", THE_SOLUTION_STRING);

            
//            RandomRouteOpt3Strategy strategyOpt3 = new RandomRouteOpt3Strategy();        
//            RandomRouteOpt3AltStrategy strategyOpt3A = new RandomRouteOpt3AltStrategy();
//            RandomRouteOpt2Strategy strategyOpt2 = new RandomRouteOpt2Strategy();
//            RandomRouteOpt2AltStrategy strategyOpt2A = new RandomRouteOpt2AltStrategy();

//            sw.Restart();
//            strategyOpt3.executeStrategy(w00tSolution);
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Opt3Solution.txt", THE_SOLUTION_STRING);
//            strategyOpt3.undoStrategy(w00tSolution);
//            sw.Stop();
//            Console.WriteLine("Elapsed time for Opt3 solution: {0}ms", sw.ElapsedMilliseconds);


//            sw.Restart();
//            strategyOpt3A.executeStrategy(w00tSolution);
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Opt3ASolution.txt", THE_SOLUTION_STRING);
//            strategyOpt3A.undoStrategy(w00tSolution);
//            sw.Stop();
//            Console.WriteLine("Elapsed time for Opt3A solution: {0}ms", sw.ElapsedMilliseconds);


//            sw.Restart();
//            strategyOpt2.executeStrategy(w00tSolution);
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Opt2Solution.txt", THE_SOLUTION_STRING);
//            strategyOpt2.undoStrategy(w00tSolution);
//            sw.Stop();
//            Console.WriteLine("Elapsed time for Opt2 solution: {0}ms", sw.ElapsedMilliseconds);


//            sw.Restart();
//            strategyOpt2A.executeStrategy(w00tSolution);
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/Opt2ASolution.txt", THE_SOLUTION_STRING);
//            strategyOpt2A.undoStrategy(w00tSolution);
//            sw.Stop();
//            Console.WriteLine("Elapsed time for Opt2A solution: {0}ms", sw.ElapsedMilliseconds);


//            sw.Restart();
//            THE_SOLUTION_STRING = w00tSolution.ToString();
//            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/EndSolution.txt", THE_SOLUTION_STRING);

//            sw.Stop();
//            Console.WriteLine("Elapsed time for executing strategy : {0}ms", sw.ElapsedMilliseconds);
//        }

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);
        //    #if DEBUG
        //    AllocConsole();
        //    #endif
        //    Console.WriteLine("Program booting up...");

        //    FilesInitializer.InitializeFiles();
        //    Console.WriteLine("Files have been processed");

        //    Stopwatch sw = Stopwatch.StartNew();
        //    Solver.generateSolution();
        //    sw.Stop();

        //    Console.WriteLine("Elapsed time for generating clusters: {0}ms", sw.ElapsedMilliseconds);
        //    Console.WriteLine();

        //    string THE_CLUSTER_SOLUTION = "";
        //    sw = Stopwatch.StartNew();
        //    //THE_CLUSTER_SOLUTION = KSolver.generateRouteSolution();
        //    sw.Stop();

        //    Console.WriteLine("Elapsed time for generating clusters as route solution: {0}ms", sw.ElapsedMilliseconds);
        //    Console.WriteLine();

        //    // Temporarily
        //    Console.ReadKey();
        //    FreeConsole();
        //    Environment.Exit(0);

        //    //new MainView() { DataContext = new MainViewModel() }.Show();
        //}

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
#endregion
    }
}