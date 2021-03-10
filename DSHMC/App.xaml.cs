﻿using System.Windows;
using Serilog;

namespace Nefarius.DsHidMini
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/DSHMC.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
