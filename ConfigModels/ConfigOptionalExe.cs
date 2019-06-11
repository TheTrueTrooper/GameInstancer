﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInstancerNS
{
    /// <summary>
    /// A class that holds the volitile data for OptionalExe
    /// </summary>
    public class ConfigOptionalExe
    {
        /// <summary>
        /// The delay that sould occure if the the exe requires delay
        /// </summary>
        [JsonProperty("Delay")]
        public int? Delay { get; protected internal set; }
        /// <summary>
        /// The Path to the optional exe
        /// </summary>
        [JsonProperty("Path")]
        public string Path { get; protected internal set; }
        /// <summary>
        /// optional arguments to use when starting the game
        /// </summary>
        [JsonProperty("StartOptions")]
        public string StartOptions { get; protected internal set; }
    }
}
