using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace GameInstancerNS
{
    public delegate void GameEndedEventHandler(object sender, GameEndedEventArgs e);

    public class GameInstance
    {
        #region user32.dll
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };
        #endregion

        const string DefualtKillReason = "Killed on Request.";
        const string ProcessEndedKillReason = "One or more of the Process has or was ended.";

        public static int ThreadStartExtraDelay = 5000;

        Process PrimaryGameExe;
        OptionalExeInstance[] OptionalExes;

        internal ProcessStartInfo Info { get { return PrimaryGameExe.StartInfo; } }
        internal ProcessStartInfo[] OptionalInfo { get { return (from Infos in OptionalExes select Infos.OptionalExe.StartInfo).ToArray(); } }

        Thread MontorThread = new Thread(new ParameterizedThreadStart(Monitor));

        internal bool IsAlive { get; private set; } = false;

        internal event GameEndedEventHandler GameHasEndedEvent;

        internal GameInstance(Game Gamer)
        {
            PrimaryGameExe = new Process();
            PrimaryGameExe.StartInfo = new ProcessStartInfo()
            {
                FileName = Gamer.Path,
                //UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(Gamer.Path)
            };
            OptionalExes = new OptionalExeInstance[Gamer.OptionalExes.Count()];
            for (int i = 0; i < Gamer.OptionalExes.Count(); i++)
            {
                OptionalExes[i] = new OptionalExeInstance(Gamer.OptionalExes[i]);
            }
        }

        internal void StartGame()
        {
            // for code reliablity kill all open process of type
            List<Process> GameProcessesTokill = Process.GetProcesses().Where(pr => pr.ProcessName == Path.GetFileName(PrimaryGameExe.StartInfo.FileName).Replace(".exe", "")).ToList();
            foreach (Process process in GameProcessesTokill)
            {
                try
                {
                    process.Kill();
                }
                catch
                { }
            }
            foreach (OptionalExeInstance exe in OptionalExes)
            {
                List<Process> OptionalProcessesTokill = Process.GetProcesses().Where(pr => pr.ProcessName == Path.GetFileName(exe.OptionalExe.StartInfo.FileName).Replace(".exe", "")).ToList();
                foreach (Process process in OptionalProcessesTokill)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    { }
                }
            }


            IsAlive = true;
            PrimaryGameExe.Start();

            foreach (OptionalExeInstance exe in OptionalExes)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(exe.Delay);
                    exe.StartExe();
                });

                if (PrimaryGameExe.MainWindowHandle == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(PrimaryGameExe.Handle, ShowWindowEnum.Restore);
                }

                SetForegroundWindow(PrimaryGameExe.MainWindowHandle);
            }

            Thread.Sleep(OptionalExes.Max(x=>x.Delay + ThreadStartExtraDelay));

            if (PrimaryGameExe.MainWindowHandle == IntPtr.Zero)
            {
                // the window is hidden so try to restore it before setting focus.
                ShowWindow(PrimaryGameExe.Handle, ShowWindowEnum.Restore);
            }
            //set user the focus to the window
            SetForegroundWindow(PrimaryGameExe.MainWindowHandle);

            MontorThread.Start(this);
        }

        internal void Kill(string Reason = DefualtKillReason)
        {
            lock (PrimaryGameExe)
                if(!PrimaryGameExe.HasExited)
                    PrimaryGameExe.Kill();
            foreach (OptionalExeInstance exe in OptionalExes)
            {
                lock (OptionalExes)
                    if (!exe.OptionalExe.HasExited)
                        exe.Kill();
            }
            IsAlive = false;
            GameHasEndedEvent?.Invoke(this, new GameEndedEventArgs() { GameName = PrimaryGameExe.StartInfo.FileName, Reason = Reason });
        }

        static void Monitor(object GameInstanceIn)
        {
            GameInstance GameInstance = (GameInstance)GameInstanceIn;
            bool IsAlive = true;
            while (IsAlive)
            {
                lock (GameInstance.PrimaryGameExe)
                    if (IsAlive)
                        IsAlive = !GameInstance.PrimaryGameExe.HasExited;
                if (!IsAlive)
                    break;
                lock (GameInstance.OptionalExes)
                    if (IsAlive)
                        IsAlive = !GameInstance.OptionalExes.All(x =>x.OptionalExe.HasExited);
                if (!IsAlive)
                    break;
            }
            lock (GameInstance)
                GameInstance.Kill(ProcessEndedKillReason);
        }


    }
}
