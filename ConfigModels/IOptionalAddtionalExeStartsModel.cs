using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInstancerNS
{
    /// <summary>
    /// A Implmentable interface for game model's OptionalAddtionalExeStartsModel
    /// </summary>
    public interface IOptionalAddtionalExeStartsModel
    {
        /// <summary>
        /// The delay that sould occure if the the exe requires delay
        /// </summary>
        int? Delay { get; }
        /// <summary>
        /// The Path to the optional exe
        /// </summary>
        string Path { get; }
        /// <summary>
        /// optional arguments to use when starting the game
        /// </summary>
        string StartOptions { get; }
    }
}
