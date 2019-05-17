using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInstancerNS
{
    public class GameEndedEventArgs : EventArgs
    {
        public string GameName { get; internal set; }
        public string Reason { get; internal set; }
        public DateTime time { get; } = DateTime.Now;
    }
}
