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
        public IGameConfig Config = null; 
        
        /// <summary>
        /// Gets a list of your games to work with
        /// </summary>
        public List<IGameModel> Games { get { return Config.Games; } }

        /// <summary>
        /// The last instance runngng on the machine.
        /// </summary>
        InstanceGame RunningGame = null;

        /// <summary>
        /// The event to tie to this games end.
        /// </summary>
        public event GameEndedEventHandler GameHasEndedEvent;

        /// <summary>
        /// The Event to tie to the game after the start
        /// </summary>
        public event GameStartedEventHandler GameHasStartedEvent;

        /// <summary>
        /// The Event to tie to the game durring start
        /// </summary>
        public event GameStartingEventHandler GameIsStartingEvent;

        /// <summary>
        /// Builds an instancer with the kernel32's event catch
        /// </summary>
        public GameInstancer(IGameConfig Config)
        {
            this.Config = Config;

            if (Config == null)
                throw new Exception("Not a valid config. Please set to a valid IGameConfig Interface.");

            #region Kernel32EventTieing
            CloseEventHandle += new CloseEvent(CloseEventAction);
            SetConsoleCtrlHandler(CloseEventHandle);
            #endregion
        }

        /// <summary>
        /// Returns the name of the game running or a sting stating no games are running
        /// </summary>
        public string RunningGameName
        {
            get
            {
                if (RunningGame == null)
                    return "No Games are currently running";
                return RunningGame.Info.FileName;
            }
        }

        /// <summary>
        /// returns if the game is running or not
        /// </summary>
        public bool GameIsRunning
        {
            get
            {
                if (RunningGame != null)
                {
                    lock (RunningGame)
                    {
                        return RunningGame.IsAlive;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Instances A game  by index with a defualt of the first game avaible
        /// </summary>
        /// <param name="GameNumberToStart">The index of the game in the config</param>
        public void StartGame(object StartRequester, string GameGUIDToStart)
        {
            GameIsStartingEvent?.Invoke(this, new GameStartingEventArgs { GameName = Config[GameGUIDToStart].Name, RequestingObj = StartRequester });
            RunningGame = new InstanceGame(Config[GameGUIDToStart]);
            RunningGame.GameHasEndedEvent += GameEndedEventChain;
            RunningGame.StartGame();
            GameHasStartedEvent?.Invoke(this, new GameStartedEventArgs { GameName = Config[GameGUIDToStart].Name });
        }

        /// <summary>
        /// Instances A game by its name
        /// </summary>
        /// <param name="GameNumberToStart">The index of the game in the config</param>
        public void StartGameByName(object StartRequester, string GameNameToStart)
        {
            IGameModel gameConfig = Config.GetGameByName(GameNameToStart);
            GameIsStartingEvent?.Invoke(this, new GameStartingEventArgs { GameName = gameConfig.Name, RequestingObj = StartRequester });
            RunningGame = new InstanceGame(gameConfig);
            RunningGame.GameHasEndedEvent += GameEndedEventChain;
            RunningGame.StartGame();
            GameHasStartedEvent?.Invoke(this, new GameStartedEventArgs { GameName = gameConfig.Name });
        }

        /// <summary>
        /// simply an internal invoke to chain events to the outside
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEndedEventChain(object sender, GameEndedEventArgs e)
        {
            try
            {
                GameHasEndedEvent?.Invoke(sender, e);
            }
            catch
            {}
        }

        /// <summary>
        /// Kills the game and removes it from currentRunning
        /// </summary>
        public void KillGame()
        {
            try
            {
                lock (RunningGame)
                RunningGame?.Kill();
                RunningGame = null;
            }
            catch
            { }
        }

        /// <summary>
        /// The Kern 32 event
        /// </summary>
        /// <param name="sig"></param>
        /// <returns></returns>
        private bool CloseEventAction(CtrlType sig)
        {
            KillGame();
            return true;
        }

    }
}
