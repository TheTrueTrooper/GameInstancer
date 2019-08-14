using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GameInstancerNS
{
    /// <summary>
    /// A class to load the Game configs from XML
    /// </summary>
    public sealed class JSONGamesConfig : IGameConfig
    {
        public static string ConfigFile = "GameInstancerConfig.json";
        const string NotSetError = "On either one of the optional exes or primary game you have left the Path, PlayTime, or Delay empty in the XML config.\nMinimal configuration requires these to be set on each game and their extra exes.\nNote that a value of 0 will be infinite play time or no start delay.";

        public List<IGameModel> Games { private set; get; } = new List<IGameModel>();


        public IGameModel this[string GUID]
        {
            get
            {
                return Games.Where(x => x.GUID == GUID).FirstOrDefault();
            }
        }

        public IGameModel GetGameByName(string StartGameName)
        {
            return Games.Where(x => x.Name == StartGameName).FirstOrDefault();
        }

        public JSONGamesConfig(Stream Stream = null)
        {
            if (Stream == null)
                Stream = new FileStream(ConfigFile, FileMode.Open);
            using (Stream)
            using (StreamReader sr = new StreamReader(Stream))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<ConfigGameModel> ScopedList = serializer.Deserialize<List<ConfigGameModel>>(reader).ToList();
                // read the json from a stream
                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                foreach (ConfigGameModel CGM in ScopedList)
                    Games.Add(CGM);
            }

            if (Games.Any(x => x.GUID == null || x.Path == null || x.PlayTime == null || x.IOptionalAddtionalExeStarts.Any(j => j.Path == null || j.Delay == null)))
                throw new Exception(NotSetError);
        }

    }
}
