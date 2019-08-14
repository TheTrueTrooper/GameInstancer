using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameInstancerNS
{
    /// <summary>
    /// A class to load the Game configs from XML
    /// </summary>
    public sealed class XMLGamesConfig : IGameConfig
    {
        const string ConfigFile = "GameInstancerConfig.xml";
        const string RootNode = "Games";
        const string AGameNode = "Game";
        const string AddtionalExeStartNode = "OptionalAddtionalExeStarts";
        const string IDAttribute = "GUID";
        const string NameAttribute = "Name";
        const string PathAttribute = "Path";
        const string PlayTimeAttribute = "PlayTime";
        const string CostToPlayAttribute = "CostToPlay";
        const string ImagePathAttribute = "ImagePath";
        const string DelayAttribute = "Delay";
        const string StartOptionsAttribute = "StartOptions";
        const string NotSetError = "On either one of the optional exes or primary game you have left the Path, PlayTime, or Delay empty in the XML config.\nMinimal configuration requires these to be set on each game and their extra exes.\nNote that a value of 0 will be infinite play time or no start delay.";

        /// <summary>
        /// The list of games. This is an access that is up as an interface for the config.
        /// </summary>
        public List<IGameModel> Games { private set; get; } = new List<IGameModel>();

        /// <summary>
        /// This is the getter interface implmentation. 
        /// It retrives the game via a string value
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public IGameModel this[string GUID]
        {
            get
            {
                return Games.Where(x => x.GUID == GUID).FirstOrDefault();
            }
        }

        /// <summary>
        /// This is the getter interface implmentation. 
        /// It retrives the game via a string value
        /// </summary>
        /// <param name="StartGameName">the name of the game</param>
        /// <returns></returns>
        public IGameModel GetGameByName(string StartGameName)
        {
            return Games.Where(x => x.Name == StartGameName).FirstOrDefault();
        }

        public XMLGamesConfig(Stream Stream = null)
        {
            XElement Config;
            if (Stream == null)
                Stream = new FileStream(ConfigFile, FileMode.Open);
            using (Stream)
                Config = XElement.Load(Stream);
            Games = (from GameXML in Config.Descendants(AGameNode)
                     select new ConfigGameModel()
                     {
                         GUID = GameXML.Attribute(IDAttribute)?.Value,
                         Name = GameXML.Attribute(NameAttribute)?.Value,
                         Path = GameXML.Attribute(PathAttribute)?.Value,
                         ImagePath = GameXML.Attribute(ImagePathAttribute)?.Value,
                         PlayTime = ulongParseOrNull(GameXML.Attribute(PlayTimeAttribute)?.Value),
                         CostToPlay = IntParseOrNull(GameXML.Attribute(CostToPlayAttribute)?.Value),
                         StartOptions = GameXML.Attribute(StartOptionsAttribute)?.Value,
                         IOptionalAddtionalExeStarts = (from ExeXML in GameXML.Descendants(AddtionalExeStartNode)
                                         select new ConfigOptionalAddtionalExeStartsModel()
                                         {
                                             Path = ExeXML.Attribute(PathAttribute)?.Value,
                                             Delay = IntParseOrNull(ExeXML.Attribute(DelayAttribute)?.Value)
                                         }).Cast<IOptionalAddtionalExeStartsModel>().ToList()
                     }).Cast<IGameModel>().ToList();

            //a trick to quickly convert to json when making the json config class test subject. Not really relavat to the code however.
            //string Result = JsonConvert.SerializeObject(Games);

            if (Games.Any(x => x.GUID == null || x.Path == null || x.PlayTime == null || x.IOptionalAddtionalExeStarts.Any(j => j.Path == null || j.Delay == null)))
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
