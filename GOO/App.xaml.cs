using System;
using System.Runtime.InteropServices;
using System.Windows;

using GOO.View;
using GOO.ViewModel;
using GOO.Utilities;
using GOO.Model;
using System.Diagnostics;

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
            Route route = new Route();
            Stopwatch sw = Stopwatch.StartNew();
            route.CreateRouteList(100000, 43200.0d, 200);
            sw.Stop();
            Console.WriteLine("Elapsed time: {0}", sw.ElapsedMilliseconds);

            new MainView() { DataContext = new MainViewModel() }.Show();
        }
    }
}