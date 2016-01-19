using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading;
using SystemTestService.EemCommands;
using SystemTestService.Utility;
using EikonEnvManager.ProcessManagement;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;

namespace SystemTestService
{
    internal static class Program
    {
        private static readonly ILogger ALogger = Logger.Default;
        private static Timer _timer;

        #region Unhandled Exception Handler

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exc = e.ExceptionObject as Exception;
            if (null != exc)
            {
                Logger.Default.LogError("Unhandled exception occurred: {0}", exc);
                Logger.Default.LogException(exc);
            }

            FileSystemUtils.GenerateDump();
        }

        #endregion

        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            XmlConfigurator.Configure();
            ALogger.LogInfo("SystemTestService initializing...");
            SetConsoleTitle("SystemTestService");
            
            var intervalInMin = ConfigUtil.IntervalMinute;
            ALogger.LogInfo("SystemTestService check dump stat every {0} mins", intervalInMin);
            ALogger.LogInfo("SystemTestService dump ST stat every {0} mins", ConfigUtil.JobDumpIntervalMinute);
            var service = new SystemTestLoader();

            _timer = new Timer(statQueryJob =>
            {
                if (ConfigUtil.DumpSTStatFlag && (DateTime.Now.TimeOfDay.TotalMinutes % ConfigUtil.JobDumpIntervalMinute) < 1)
                {
                    bool isNoPreviousInstance;
                    var mt = new Mutex(true, "Reuters.RST.UTF", out isNoPreviousInstance);
                    if (isNoPreviousInstance)
                    {
                        try
                        {
                            ALogger.LogInfo("[statQueryJob] ****** starting dump job *****");
                            service.DumpAndUploadRSTStatInterval();
                        }
                        catch (Exception e)
                        {
                            ALogger.LogError("[statQueryJob] exception in job, " + e.Message);
                        }
                        ALogger.LogInfo("[statQueryJob] ****** releasing mutex *****");
                        mt.ReleaseMutex();
                        mt.Close();
                    }
                    else
                    {
                        ALogger.LogInfo("[statQueryJob] ***** busy *****");
                    }
                }
                else
                {
                    if (DateTime.Now.Minute == 0)
                    {
                        ALogger.LogInfo("[statQueryJob] ***** Triggered but not match criteria {0}:{1}*****", ConfigUtil.JobStartHour, ConfigUtil.JobStartMinute);
                    }
                }                
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(intervalInMin));

            using (var host = CreateServiceHost())
            {
                host.Open();
                foreach (var ep in host.Description.Endpoints)
                {
                    ALogger.LogInfo("Name: " + ep.Name + "; Address: " + ep.Address);
                }

                ALogger.LogInfo("SystemTestService started");

                EnvironmentManager.Instance
                    .RegisterCommand<DumpDBCommand>()
                    .RegisterCommand<DumpSystemTest>()
                    .RegisterCommand<UpdateSetting>()
                    .RegisterCommand<CleanTempFile>()
                    .RegisterCommand<MergeTestID>()
                    .WaitForShutdown();
            }
        }
       

        #region Create service

        private static ServiceHost CreateServiceHost()
        {
            var hostFactory = new TR.AppServer.ServiceRouting.RoutedServiceHostFactory();
            var host = hostFactory.CreateServiceHost(typeof(SystemTestLoader));
            return host;
        }       

        #endregion

        #region helpers

        public static void SetConsoleTitle(string title)
        {
            if (Process.GetCurrentProcess().MainWindowHandle == IntPtr.Zero)
            {
                return;
            }

            int pid = Process.GetCurrentProcess().Id;
            Console.Title = string.Format("{0} ({1}) [pid: {2}]", title, PlatformInfo.Name, pid);
        }

        /// <summary>
        /// Platform information utility class.
        /// </summary>
        public static class PlatformInfo
        {
            /// <summary>
            /// Gets the name of the system architecture that the CLR is running under.
            /// </summary>
            public static string Name
            {
                get
                {
                    if (Marshal.SizeOf(typeof(IntPtr)) == 8)
                    {
                        return "x64";
                    }

                    return "x86";
                }
            }
        }

        #endregion
    }
}