using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SerialMIDIBus.converters
{
    [ValueConversion(typeof(bool),typeof(bool))]
    public class HantenConverter:BaseDTConverter<bool,bool>
    {
        public HantenConverter()
        {
            Convert = v => ((bool)v) ? false : true;
            ConvertBack = v => ((bool)v) ? false : true;
        }
    }
}
