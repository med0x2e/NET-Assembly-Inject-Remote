using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatzNetAssembly
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("[+]: Calling KatzNetAssembly.Katz.Exec()");
            KatzNetAssembly.Katz.Exec(args);
        }
    }
}
