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
        const string RootNode = "Games";
        const string AGameNode = "Game";
        const string AddtionalExeStartNode = "AddtionalExeStart";
        const string NameAttribute = "Name";
        const string PathAttribute = "Path";
        const string PlayTimeAttribute = "PlayTime";
        const string CostToPlayAttribute = "CostToPlay";
        const string ImagePathAttribute = "ImagePath";
        const string DelayAttribute = "Delay";
        const string NotSetError = "On either one of the optional exes or primary game you have left the Path, PlayTime, or Delay empty in the XML config.\nMinimal configuration requires these to be set on each game and their extra exes.\nNote that a value of 0 will be infinite play time or no start delay.";

        public List<ConfigGame> Games { private set; get; }

        public ConfigGame this[string StartGameName]
        {
            get {
                return Games.Where(x => x.Name == StartGameName).FirstOrDefault();
            }
        }

        public ConfigGame this[int Index]
        {
            get
            {
                return Games[Index];
            }
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

                // read the json from a stream
                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                Games = serializer.Deserialize<List<ConfigGame>>(reader).ToList();
            }

            if (Games.Any(x => x.Path == null || x.PlayTime == null || x.OptionalExes.Any(j => j.Path == null || j.Delay == null)))
                throw new Exception(NotSetError);
        }

        int? IntParseOrNull(string ParseString)
        {
            try
            {
                return int.Parse(ParseString);
            }
            catch
            {
                return null;
            }
        }

        ulong? ulongParseOrNull(string ParseString)
        {
            try
            {
                return ulong.Parse(ParseString);
            }
            catch
            {
                return null;
            }
        }
    }
}
