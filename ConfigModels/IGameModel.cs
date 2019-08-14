using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInstancerNS
{
    /// <summary>
    /// A Implmentable interface for game models
    /// </summary>
    public interface IGameModel
    {
        /// <summary>
        /// the Databases ID or specified via the config file
        /// </summary>
        string GUID { get; }
        /// <summary>
        /// The Name of the game.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The Path of the game.
        /// </summary>
        string Path { get; }
        /// <summary>
        /// The path to a display image for a game.
        /// </summary>
        string ImagePath { get; }
        /// <summary>
        /// The Play time for a game.
        /// </summary>
        ulong? PlayTime { get; }
        /// <summary>
        /// The Cost to Play for a game.
        /// </summary>
        double? CostToPlay { get; }
        /// <summary>
        /// optional arguments to use when starting the game
        /// </summary>
        string StartOptions { get; }
        /// <summary>
        /// The Optional Exes with a game
        /// </summary>
        List<IOptionalAddtionalExeStartsModel> IOptionalAddtionalExeStarts { get; }
    }
}
