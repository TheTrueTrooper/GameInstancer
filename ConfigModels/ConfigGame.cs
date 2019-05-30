using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameInstancerNS
{
    /// <summary>
    /// A class to hold Game info in volitie memory
    /// </summary>
    public class ConfigGame
    {
        /// <summary>
        /// The Name of the game.
        /// </summary>
        public string Name { get; protected internal set; }
        /// <summary>
        /// The Path of the game.
        /// </summary>
        public string Path { get; protected internal set; }
        /// <summary>
        /// The path to a display image for a game.
        /// </summary>
        public string ImagePath { get; protected internal set; }
        /// <summary>
        /// The Play time for a game.
        /// </summary>
        public ulong? PlayTime { get; protected internal set; }
        /// <summary>
        /// The Cost to Play for a game.
        /// </summary>
        public int? CostToPlay { get; protected internal set; }

        /// <summary>
        /// The Optional Exes with a game
        /// </summary>
        public List<ConfigOptionalExe> OptionalExes { protected internal set; get; } = new List<ConfigOptionalExe>();
    }
}
