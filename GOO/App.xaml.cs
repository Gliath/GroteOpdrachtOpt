using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

using GOO.Model;
using GOO.Model.Optimizers;
using GOO.Model.Optimizers.Strategies;
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
            Solution start = Solver.generateClusters();
            sw.Stop();

            Console.WriteLine("Elapsed time for generating clusters: {0:N}ms", sw.ElapsedMilliseconds);

            sw.Restart();
            start = Solver.optimizeSolution(start);
            sw.Stop();
            string THE_NEW_STRING = start.ToString();

            Console.WriteLine("Start Solution:");
            Console.WriteLine(THE_NEW_STRING);
            Console.WriteLine("End Solution");
            Console.WriteLine("Elapsed time for generating and optimizing the solution: {0:N}ms", sw.ElapsedMilliseconds);

            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/TheSolution.txt", THE_NEW_STRING);
            #endif

            #if !DEBUG
            MainViewModel mainVM = new MainViewModel();
            new MainView() { DataContext = mainVM }.Show();
            System.Threading.Tasks.Task.Factory.StartNew(() => mainVM.InitialRun());
            #endif 
        }
    }
}