using System;
using System.Collections.Generic;
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
        public static string ConfigFile = "GameInstancerConfig.xml";
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

        public List<ConfigGame> Games { private set; get; } = new List<ConfigGame>();

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

        public XMLGamesConfig()
        {
            XElement Config = XElement.Load(ConfigFile);
            Games = (from GameXML in Config.Descendants(AGameNode)
                     select new ConfigGame()
                     {
                         Name = GameXML.Attribute(NameAttribute)?.Value,
                         Path = GameXML.Attribute(PathAttribute)?.Value,
                         ImagePath = GameXML.Attribute(ImagePathAttribute)?.Value,
                         PlayTime = ulongParseOrNull(GameXML.Attribute(PlayTimeAttribute)?.Value),
                         CostToPlay = IntParseOrNull(GameXML.Attribute(CostToPlayAttribute)?.Value),
                         OptionalExes = (from ExeXML in GameXML.Descendants(AddtionalExeStartNode)
                                         select new ConfigOptionalExe()
                                         {
                                             Path = ExeXML.Attribute(PathAttribute)?.Value,
                                             Delay = IntParseOrNull(ExeXML.Attribute(DelayAttribute)?.Value)
                                         }).ToList()
                     }).ToList();

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
