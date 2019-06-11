using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInstancerNS
{
    /// <summary>
    /// A call back for the end of a game exe's life
    /// </summary>
    /// <param name="sender">a calling GameInstance</param>
    /// <param name="e"></param>
    public delegate void GameEndedEventHandler(object sender, GameEndedEventArgs e);

    /// <summary>
    /// A call back for the end of a game exe's life
    /// </summary>
    /// <param name="sender">a calling GameInstance</param>
    /// <param name="e"></param>
    public delegate void GameStartingEventHandler(object sender, GameStartingEventArgs e);

    /// <summary>
    /// A call back for the end of a game exe's life
    /// </summary>
    /// <param name="sender">a calling GameInstance</param>
    /// <param name="e"></param>
    public delegate void GameStartedEventHandler(object sender, GameStartedEventArgs e);
}
