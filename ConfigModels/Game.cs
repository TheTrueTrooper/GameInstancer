using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameInstancerNS
{
    public class Game
    {
        public string Name { get; internal set; }
        public string Path { get; internal set; }
        public string ImagePath { get; internal set; }

        public List<OptionalExe> OptionalExes { internal set; get; } = new List<OptionalExe>();
    }
}
