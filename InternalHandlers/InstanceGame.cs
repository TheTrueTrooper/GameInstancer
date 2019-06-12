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

    /// <summary>
    /// A class used to start and stop game exes with other optional exes
    /// </summary>
    public class InstanceGame
    {
        #region user32.dll
        /// <summary>
        /// Importys the user 32 system to allow for window to me made non minimized 
        /// </summary>
        /// <param name="hWnd">A pointer to the window</param>
        /// <param name="flags">how we want to show the window</param>
        /// <returns>bool for success (I think not really using it...)</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        /// <summary>
        /// a user 32 import to force the window to the forground
        /// </summary>
        /// <param name="hwnd">A pointer to the window</param>
        /// <returns>int indication of success or errors</returns>
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        /// <summary>
        /// the posible ways to set the windows state based one the windows import fuctions enums.
        /// </summary>
        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };
        #endregion

        /// <summary>
        /// Our defualt message for a process kill
        /// </summary>
        protected const string DefualtKillReason = "Killed on Request.";
        /// <summary>
        /// Process Ended Reason to Kills message
        /// </summary>
        protected const string ProcessEndedKillReason = "One or more of the Process has or was ended.\nAs such a full shut down was started.";
        protected const string PlayTimeAllowedEndedKillReason = "The alloted allowed game time has ended."; 

        /// <summary>
        /// The tweekable extra delay to allow all process to open propertly
        /// </summary>
        public int ThreadStartExtraDelay;

        /// <summary>
        /// A thread to monitor process for kill conditions
        /// </summary>
        protected Thread MontorThread = new Thread(new ParameterizedThreadStart(Monitor));

        /// <summary>
        /// An Event handle
        /// </summary>
        protected internal event GameEndedEventHandler GameHasEndedEvent;

        /// <summary>
        /// the Primary game process that we started
        /// </summary>
        protected Process PrimaryGameExe;
        /// <summary>
        /// The optional exes that we can attach and relate back to the main exe
        /// </summary>
        protected InstanceOptionalExe[] OptionalExes;

        /// <summary>
        /// A timer we can use to time the current exection and prevent game over time
        /// </summary>
        protected Stopwatch Timer = new Stopwatch();

        /// <summary>
        /// A getter that returns the info of the game we started
        /// </summary>
        protected internal ProcessStartInfo Info { get { return PrimaryGameExe.StartInfo; } }
        /// <summary>
        /// A getter that returns the info of the optional exes we started
        /// </summary>
        protected internal ProcessStartInfo[] OptionalInfo { get { return (from Infos in OptionalExes select Infos.OptionalExe.StartInfo).ToArray(); } }

        /// <summary>
        /// A bool to indicate if the game has exited out due to something
        /// </summary>
        protected internal bool IsAlive { get; private set; } = false;
        /// <summary>
        /// the Allowed play time on the game. 0 = infinite
        /// </summary>
        protected internal ulong PlayTimeAllowed { get; private set; }

        /// <summary>
        /// Creates an Instance
        /// </summary>
        /// <param name="Gamer"></param>
        /// <param name="ThreadStartExtraDelay"></param>
        protected internal InstanceGame(IGameModel Gamer, int ThreadStartExtraDelay = 5000)
        {
            PrimaryGameExe = new Process();
            PlayTimeAllowed = Gamer.PlayTime.Value;
            this.ThreadStartExtraDelay = ThreadStartExtraDelay;
            PrimaryGameExe.StartInfo = new ProcessStartInfo()
            {
                FileName = Gamer.Path,
                //UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(Gamer.Path),
                Arguments = Gamer.StartOptions
            };
            OptionalExes = new InstanceOptionalExe[Gamer.IOptionalAddtionalExeStarts.Count()];
            for (int i = 0; i < Gamer.IOptionalAddtionalExeStarts.Count(); i++)
            {
                OptionalExes[i] = new InstanceOptionalExe(Gamer.IOptionalAddtionalExeStarts[i]);
            }
        }

        /// <summary>
        /// Starts a game
        /// </summary>
        protected internal virtual void StartGame()
        {
            // for code reliablity kill all open process of type. this will prevent conflictions, shared resources problems, and lock up issuses
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
            foreach (InstanceOptionalExe exe in OptionalExes)
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

            //make sure the out side world can tell if we are alive
            IsAlive = true;
            //Start the game exe
            PrimaryGameExe.Start();

            // start all optional exes
            foreach (InstanceOptionalExe exe in OptionalExes)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(exe.Delay);
                    exe.StartExe();
                });
            }
            //try to focus
            if (PrimaryGameExe.MainWindowHandle == IntPtr.Zero)
            {
                // the window is hidden so try to restore it before setting focus.
                ShowWindow(PrimaryGameExe.Handle, ShowWindowEnum.Restore);
            }
            //set user the focus to the window
            SetForegroundWindow(PrimaryGameExe.MainWindowHandle);

            // Make sure the main thread doesnt get too far ahead if the other threads given any delays they may have
            Thread.Sleep(OptionalExes.Max(x=>x.Delay + ThreadStartExtraDelay));
            Timer.Start();

            //final focus
            if (PrimaryGameExe.MainWindowHandle == IntPtr.Zero)
            {
                // the window is hidden so try to restore it before setting focus.
                ShowWindow(PrimaryGameExe.Handle, ShowWindowEnum.Restore);
            }
            //set user the focus to the window
            SetForegroundWindow(PrimaryGameExe.MainWindowHandle);

            //start our process monitoring thread
            MontorThread.Start(this);
        }

        /// <summary>
        /// Kills a thread and invokes a event to allow you to react acordingly
        /// </summary>
        /// <param name="Reason"></param>
        protected internal virtual void Kill(string Reason = DefualtKillReason)
        {
            //kill the exe if it has yet to end
            lock (PrimaryGameExe)
                if(!PrimaryGameExe.HasExited)
                    PrimaryGameExe.Kill();

            //kill the optional exe's
            foreach (InstanceOptionalExe exe in OptionalExes)
            {
                lock (OptionalExes)
                    if (!exe.OptionalExe.HasExited)
                        exe.Kill();
            }
            //mark as dead and rest timers
            Timer.Stop();
            GameEndedEventArgs EndData = new GameEndedEventArgs() { GameName = PrimaryGameExe.StartInfo.FileName, Reason = Reason, AllowedTimeInMS = (ulong)Timer.ElapsedMilliseconds };
            Timer.Reset();
            //call reacting code
            if(IsAlive)
                GameHasEndedEvent?.Invoke(this, EndData);
            IsAlive = false;
        }

        /// <summary>
        /// A fuction for threads that will monitor the process and kill all on end or issue
        /// </summary>
        /// <param name="GameInstanceIn">the game Intances for listing(this)</param>
        protected static void Monitor(object GameInstanceIn)
        {
            string Reason = "";
            //grab this GameInstance to watch
            InstanceGame GameInstance = (InstanceGame)GameInstanceIn;
            //set up a loop
            bool IsAlive = true;
            while (IsAlive)
            {
                //check for any reasons to end like primary exe has died
                lock (GameInstance.PrimaryGameExe)
                    if (IsAlive)
                    {
                        IsAlive = !GameInstance.PrimaryGameExe.HasExited;
                        Reason = ProcessEndedKillReason;
                    }
                if (!IsAlive)
                    break;
                //check if any optional exe's have died
                lock (GameInstance.OptionalExes)
                    if (IsAlive)
                    {
                        IsAlive = !GameInstance.OptionalExes.All(x => x.OptionalExe.HasExited);
                        Reason = ProcessEndedKillReason;
                    }
                if (!IsAlive)
                    break;
                //check if we still have play time
                lock (GameInstance)
                    if (IsAlive && 0 != GameInstance.PlayTimeAllowed && (ulong)GameInstance.Timer.ElapsedMilliseconds < GameInstance.PlayTimeAllowed)
                    {
                        IsAlive = false;
                        Reason = PlayTimeAllowedEndedKillReason;
                    }
            }
            //Kill on end
            lock (GameInstance)
                GameInstance.Kill(Reason);
        }


    }
}
