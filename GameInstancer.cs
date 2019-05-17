using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace GameInstancerNS
{
    public class GameInstancer
    {
        #region Kernel32ExitImport
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(CloseEvent CloseEventHandler, bool add);

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        #endregion
        private delegate bool CloseEvent(CtrlType sig); 
        CloseEvent CloseEventHandle;

        GamesConfig Config = new GamesConfig(); 
        
        public List<Game> Games { get { return ((Game[])Config.Games.ToArray().Clone()).ToList(); } }

        GameInstance RunningGame = null;

        public GameInstancer()
        {
            #region Kernel32EventTieing
            CloseEventHandle += new CloseEvent(CloseEventAction);
            SetConsoleCtrlHandler(CloseEventHandle, true);
            #endregion
        }

        public event GameEndedEventHandler GameHasEndedEvent
        {
            add
            {
                lock (RunningGame)
                    RunningGame.GameHasEndedEvent += value;
            }
            remove
            {
                lock(RunningGame)
                    RunningGame.GameHasEndedEvent -= value;
            }
        }

        public string RunningGameName
        {
            get
            {
                if (RunningGame == null)
                    return "No Games are currently running";
                return RunningGame.Info.FileName;
            }
        }

        public bool GameIsRunning
        {
            get
            {
                lock (RunningGame)
                {
                    if (RunningGame == null)
                        return false;
                    return RunningGame.IsAlive;
                }
            }
        }

        public void StartGame(GameEndedEventHandler GameHasEndedEvent, int GameNumberToStart = 0)
        {
            RunningGame = new GameInstance(Config[GameNumberToStart]);
            this.GameHasEndedEvent += GameHasEndedEvent;
            RunningGame.StartGame();
        }

        public void StartGame(GameEndedEventHandler GameHasEndedEvent, string GameNameToStart)
        {
            RunningGame = new GameInstance(Config[GameNameToStart]);
            this.GameHasEndedEvent += GameHasEndedEvent;
            RunningGame.StartGame();
        }

        public void KillGame()
        {
            lock(RunningGame)
                RunningGame.Kill();
        }

        private bool CloseEventAction(CtrlType sig)
        {
            KillGame();
            return true;
        }

    }
}
