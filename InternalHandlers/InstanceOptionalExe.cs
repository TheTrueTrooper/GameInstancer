using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameInstancerNS
{
    public class InstanceOptionalExe
    {
        /// <summary>
        /// the optional 
        /// </summary>
        protected internal Process OptionalExe { get; private set; }

        /// <summary>
        /// the optional delay to take befor starting useful for sinc
        /// </summary>
        protected internal int Delay;

        /// <summary>
        /// creates an optional Exe Instance
        /// </summary>
        /// <param name="OptionalExe">based on OptionalExe</param>
        protected internal InstanceOptionalExe(IOptionalAddtionalExeStartsModel OptionalExe)
        {
            Delay = OptionalExe.Delay.Value;
            this.OptionalExe = new Process();
            this.OptionalExe.StartInfo = new ProcessStartInfo()
            {
                FileName = OptionalExe.Path,
                //UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(OptionalExe.Path),
                Arguments = OptionalExe.StartOptions
            };
        }

        /// <summary>
        /// starts the optional exe
        /// </summary>
        protected internal virtual void StartExe()
        {
            Thread.Sleep(Delay);
            OptionalExe.Start();
        }

        /// <summary>
        /// Kills the optional EXE
        /// </summary>
        protected internal virtual void Kill()
        {
            OptionalExe.Kill();
        }
    }
}
