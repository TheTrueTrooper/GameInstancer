using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameInstancerNS
{
    public class GamesConfig
    {
        public static string ConfigFile = "GameInstancerConfig.xml";
        const string RootNode = "Games";
        const string AGameNode = "Game";
        const string AddtionalExeStartNode = "AddtionalExeStart";
        const string NameAttribute = "Name";
        const string PathAttribute = "Path";
        const string ImagePathAttribute = "ImagePath";
        const string DelayAttribute = "Delay";
        const string NotSetError = "On either one of the optional exes or primary game you have left the Path or Delay empty in the XML config.\nMinimal configuration requires these to be set on each game and their extra exes.";

        public List<Game> Games { private set; get; } = new List<Game>();

        public Game this[string StartGameName]
        {
            get {
                return Games.Where(x => x.Name == StartGameName).FirstOrDefault();
            }
        }

        public Game this[int Index]
        {
            get
            {
                return Games[Index];
            }
        }

        internal GamesConfig()
        {
            LoadData();
        }

        internal void LoadData()
        {
            XElement Config = XElement.Load(ConfigFile);
            Games = (from GameXML in Config.Descendants(AGameNode)
                                       select new Game()
                                       {
                                           Name = GameXML.Attribute(NameAttribute)?.Value,
                                           Path = GameXML.Attribute(PathAttribute)?.Value,
                                           ImagePath = GameXML.Attribute(ImagePathAttribute)?.Value,
                                           OptionalExes = (from ExeXML in GameXML.Descendants(AddtionalExeStartNode)
                                                           select new OptionalExe()
                                                           {
                                                               Path = ExeXML.Attribute(PathAttribute)?.Value,
                                                               Delay = int.Parse(ExeXML.Attribute(DelayAttribute)?.Value)
                                                           }).ToList()
                                       }).ToList();

            if (Games.Any(x => x.Path == null || x.OptionalExes.Any(j => j.Path == null || j.Delay == null)))
                throw new Exception(NotSetError);
        }

       
    }
}
