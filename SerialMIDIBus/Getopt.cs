using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialMIDIBus
{
    public class Getopt
    {
        public static void ParseOptions(string[] args,Func<string,string,bool> runned_func)
        {
            for(int i = 0; i < args.Length; i++)
            {
                if ((i + 1) < args.Length)
                {
                    if (!runned_func(args[i], args[i + 1]))
                    {
                        i++;
                    }
                }
                else 
                    if (!runned_func(args[i],""))
                {
                    i++;
                }
            }
        }
    }
}
