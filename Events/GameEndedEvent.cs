using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInstancerNS
{
    /// <summary>
    /// A class to handle data collection on the event args
    /// </summary>
    public sealed class GameEndedEventArgs : EventArgs
    {
        /// <summary>
        /// the Games name
        /// </summary>
        public string GameName { get; internal set; }
        /// <summary>
        /// the reason for the games death
        /// </summary>
        public string Reason { get; internal set; }
        /// <summary>
        /// The time that it happened
        /// </summary>
        public DateTime time { get; } = DateTime.Now;
        /// <summary>
        /// That the process was allowed to run for
        /// </summary>
        public ulong AllowedTimeInMS { get; internal set; }
    }
}
