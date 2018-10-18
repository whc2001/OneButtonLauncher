/* This program is free software. It comes without any warranty, to
     * the extent permitted by applicable law. You can redistribute it
     * and/or modify it under the terms of the Do What The Fuck You Want
     * To Public License, Version 2, as published by Sam Hocevar. See
     * http://www.wtfpl.net/ for more details. */
     
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OneButtonLauncher
{
    static class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(int threadId, uint msg, uint wParam, uint lParam);

        public const uint NEW_SIGNAL = 0xA55A;

        public static Stopwatch sw = new Stopwatch();

        public static uint count = 1;

        public static Process GetFirstRunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            if(processes.Length > 0)
            {
                Array.Sort(processes, (a, b) => a.StartTime > b.StartTime ? 1 : -1);
                return processes[0];
            }
            return null;
        }

        [STAThread]
        static void Main()
        {
            bool isFirst;
            Mutex mutex = new Mutex(true, "_*_*_ONE_BUTTON_LAUNCHER_*_*_", out isFirst);
            if(isFirst)
            {
                ProcessEventHandler eventHandler = new ProcessEventHandler(NEW_SIGNAL);
                eventHandler.OnEventTriggered += EventHandler_OnEventTriggered;
                Application.AddMessageFilter(eventHandler);
                Ini ini = new Ini($"{Application.StartupPath}\\config.ini");
                uint timeout = uint.TryParse(ini.Read("App", "TimeOut"), out timeout) ? timeout : 2000;
                sw.Start();
                while(sw.ElapsedMilliseconds < timeout)
                {
                    Thread.Sleep(1);
                    Application.DoEvents();
                }
                string launch = ini.Read("Launch", count.ToString());
                if (!string.IsNullOrEmpty(launch))
                {
                    try
                    {
                        Process.Start(launch);
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                Process firstProcess = GetFirstRunningInstance();
                if(firstProcess != null)
                {
                    PostThreadMessage(firstProcess.Threads[0].Id, NEW_SIGNAL, 0, 0);
                    Environment.Exit(0);
                }
            }
        }

        private static void EventHandler_OnEventTriggered(object sender, uint e)
        {
            ++count;
            sw.Restart();
        }
    }
}
