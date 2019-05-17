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
    public class OptionalExeInstance
    {
        internal Process OptionalExe { get; private set; }
        public int Delay;

        internal OptionalExeInstance(OptionalExe OptionalExe)
        {
            Delay = OptionalExe.Delay.Value;
            this.OptionalExe = new Process();
            this.OptionalExe.StartInfo = new ProcessStartInfo()
            {
                FileName = OptionalExe.Path,
                //UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(OptionalExe.Path)
            };
        }

        internal void StartExe()
        {
            Thread.Sleep(Delay);
            OptionalExe.Start();
        }

        internal void Kill()
        {
            OptionalExe.Kill();
        }
    }
}
