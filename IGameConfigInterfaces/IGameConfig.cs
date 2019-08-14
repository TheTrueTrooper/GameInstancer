
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInstancerNS
{
    /// <summary>
    /// An interface should you want to use a custom config (defualt is XMLGamesConfig)
    /// All of the use of this class should use load on construtors 
    /// </summary>
    public interface IGameConfig
    {
        /// <summary>
        /// an accessor to get the Game by name
        /// </summary>
        /// <param name="StartGameName"> the sting name of the game in config</param>
        /// <returns>the Game (config info)</returns>
        IGameModel GetGameByName(string GameName);

        /// <summary>
        /// an accessor to get the Game by entry number
        /// </summary>
        /// <param name="ID">it's number in the config list(based Database ID)</param>
        /// <returns>the Game (config info)</returns>
        IGameModel this[string ID] { get; }

        /// <summary>
        /// Should get the list of games this sould be only implemented as a getter
        /// </summary>
        List<IGameModel> Games { get; }
    }
}
