using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SerialMIDIBus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public Logger logger=LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.1.0")]
        public static void Main()
        {
            LogInit();
            logger.Debug("Application booting...");
            SerialMIDIBus.App app = new SerialMIDIBus.App();
            app.InitializeComponent();
            app.Run();
        }
        public static void LogInit()
        {
            var config = new LoggingConfiguration();
            var logconsole = new NLog.Targets.ColoredConsoleTarget("logconsole");
            logconsole.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
                "level == LogLevel.Debug",
                ConsoleOutputColor.Magenta,
                ConsoleOutputColor.NoChange
                )
            );
            logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
                "level == LogLevel.Info",
                ConsoleOutputColor.Green,
                ConsoleOutputColor.NoChange
                )
            );
            logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
                "level == LogLevel.Warn",
                ConsoleOutputColor.Yellow,
                ConsoleOutputColor.NoChange
                )
            );
            logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
                "level == LogLevel.Error",
                ConsoleOutputColor.Red,
                ConsoleOutputColor.NoChange
                )
            );
            logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule(
                "level == LogLevel.Fatal",
                ConsoleOutputColor.Red,
                ConsoleOutputColor.White
                )
            );
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            NLog.LogManager.Configuration = config;

        }
    }
}
