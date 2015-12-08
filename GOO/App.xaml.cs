using System;
using System.Runtime.InteropServices;
using System.Windows;

using GOO.View;
using GOO.ViewModel;
using GOO.Utilities;

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

            new MainView() { DataContext = new MainViewModel() }.Show();
        }
    }
}