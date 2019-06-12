using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameInstancerNS
{
    /// <summary>
    /// A Basic class to hold Game info in volitie memory
    /// </summary>
    public class ConfigGameModel : IGameModel
    {
        /// <summary>
        /// the Databases ID or specified via the config file
        /// </summary>
        [JsonProperty("ID")]
        public int? ID { get; protected internal set; }
        /// <summary>
        /// The Name of the game.
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; protected internal set; }
        /// <summary>
        /// The Path of the game.
        /// </summary>
        [JsonProperty("Path")]
        public string Path { get; protected internal set; }
        /// <summary>
        /// The path to a display image for a game.
        /// </summary>
        [JsonProperty("ImagePath")]
        public string ImagePath { get; protected internal set; }
        /// <summary>
        /// The Play time for a game.
        /// </summary>
        [JsonProperty("PlayTime")]
        public ulong? PlayTime { get; protected internal set; }
        /// <summary>
        /// The Cost to Play for a game.
        /// </summary>
        [JsonProperty("CostToPlay")]
        public double? CostToPlay { get; protected internal set; }
        /// <summary>
        /// optional arguments to use when starting the game
        /// </summary>
        [JsonProperty("StartOptions")]
        public string StartOptions { get; protected internal set; }
        /// <summary>
        /// The Optional Exes with a game
        /// </summary>
        [JsonProperty("OptionalAddtionalExeStarts")]
        public List<ConfigOptionalAddtionalExeStartsModel> OptionalAddtionalExeStartsRaw { protected internal set; get; } = new List<ConfigOptionalAddtionalExeStartsModel>();

        public List<IOptionalAddtionalExeStartsModel> IOptionalAddtionalExeStarts { get => OptionalAddtionalExeStartsRaw.Cast<IOptionalAddtionalExeStartsModel>().ToList(); set => OptionalAddtionalExeStartsRaw = value.Cast<ConfigOptionalAddtionalExeStartsModel>().ToList(); }


    }

//    [
//  {
//    "Name": "Phantom Breaker: Battle Grounds",
//    "Path": "I:\\Games\\Steam\\steamapps\\common\\Phantom Breaker Battle Grounds\\bin\\pbbg_win32.exe",
//    "ImagePath": null,
//    "PlayTime": 0,
//    "CostToPlay": 0,
//    "StartOptions": "",
//    "OptionalExes": [
//      {
//        "Delay": 200,
//        "Path": "C:\\Windows\\System32\\notepad.exe",
//        "StartOptions": null
//      }
//    ]
//  }
//]
}
