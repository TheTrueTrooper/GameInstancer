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
    /// <summary>
    /// a class that handles the games starting and stoping
    /// </summary>
    public class GameInstancer
    {
        #region Kernel32ExitImport
        /// <summary>
        /// Kernal32 fuction import to allow us to grab our applications closing and respond to it as an event
        /// </summary>
        /// <param name="CloseEventHandler">the Event or funtion to call</param>
        /// <param name="add"></param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(CloseEvent CloseEventHandler, bool add = true);

        /// <summary>
        /// the Control type pass back based on C++ enums
        /// </summary>
        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        #endregion

        /// <summary>
        /// the Event to tye to the sys 32 event is handled inside and passed out through event chaining
        /// </summary>
        /// <param name="sig">the systems pass back values</param>
        /// <returns></returns>
        private delegate bool CloseEvent(CtrlType sig); 
        /// <summary>
        /// A instance of the event
        /// </summary>
        CloseEvent CloseEventHandle;

        /// <summary>
        /// the Config to use
        /// </summary>
        public IGameConfig Config = new XMLGamesConfig(); 
        
        public List<ConfigGame> Games { get { return ((ConfigGame[])Config.Games.ToArray().Clone()).ToList(); } }

        InstanceGame RunningGame = null;

        public GameInstancer()
        {
            #region Kernel32EventTieing
            CloseEventHandle += new CloseEvent(CloseEventAction);
            SetConsoleCtrlHandler(CloseEventHandle);
            #endregion
        }

        public event GameEndedEventHandler GameHasEndedEvent;

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

        public void StartGame(int GameNumberToStart = 0)
        {
            RunningGame = new InstanceGame(Config[GameNumberToStart]);
            RunningGame.GameHasEndedEvent += GameEndedEventChain;
            RunningGame.StartGame();
        }

        public void StartGame(string GameNameToStart)
        {
            RunningGame = new InstanceGame(Config[GameNameToStart]);
            RunningGame.GameHasEndedEvent += GameEndedEventChain;
            RunningGame.StartGame();
        }

        private void GameEndedEventChain(object sender, GameEndedEventArgs e)
        {
            GameHasEndedEvent.Invoke(sender, e);
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
