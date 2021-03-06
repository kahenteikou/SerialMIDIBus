using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SerialMIDIBus
{
    public class W32MIDI
    {

        [StructLayout(LayoutKind.Sequential)]
        private struct _NMidiInCaps
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public uint dwSupport;
        }
        public struct MidiInCaps
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            public string szPname;
            public uint dwSupport;
            public uint indexnumber;
        }
        public static IEnumerable<MidiInCaps> GetmidiList()
        {
            uint midiInNumDevs = midiInGetNumDevs();
            for(uint i = 0; i < midiInNumDevs; i++)
            {
                _NMidiInCaps midiInCaps = new _NMidiInCaps();
                midiInGetDevCaps(i,out midiInCaps, Marshal.SizeOf(typeof(MidiInCaps)));
                MidiInCaps newcaps=new MidiInCaps();
                newcaps.dwSupport = midiInCaps.dwSupport;
                newcaps.szPname=midiInCaps.szPname;
                newcaps.vDriverVersion=midiInCaps.vDriverVersion;
                newcaps.wMid=midiInCaps.wMid;
                newcaps.wPid=midiInCaps.wPid;
                newcaps.indexnumber = i;
                yield return newcaps;
            }
        }

        [DllImport("winmm.dll")]
        extern static uint midiInGetNumDevs();
        [DllImport("winmm.dll")]
        extern static uint midiInGetDevCaps(uint uDevID, out _NMidiInCaps pmic, int cbmic);

        [DllImport("winmm.dll")]
        public static extern uint midiInOpen(
        out IntPtr handle,
        uint id,
        MidiInProcDelegate callback,
        IntPtr hInstance,
        uint flags
        );
        [DllImport("winmm.dll")]
        public static extern uint midiInStart(IntPtr hMidiIn);

        [DllImport("winmm.dll")]
        public static extern uint midiInClose(IntPtr hMidiIn);
        public delegate void MidiInProcDelegate(IntPtr hMidiIn, uint wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);
    }
    public class MidiLib:IDisposable
    {

        static public Logger logger = LogManager.GetCurrentClassLogger();
        public bool IsOpen { get; private set; }
        public MidiLib()
        {
            this.IsOpen = false;
        }
        public delegate void _CallbackEventHandler(byte state, byte dt1, byte dt2);
        public event _CallbackEventHandler MidiRecieveEvent;
        private IntPtr MidiDevice_handler=IntPtr.Zero;
        private W32MIDI.MidiInProcDelegate _callbackoniisan = null;
        public void Open(uint deviceid)
        {
            logger.Info("Opening Midi Device..");
            _callbackoniisan = new W32MIDI.MidiInProcDelegate(Callback_ev);
            W32MIDI.midiInOpen(
                out MidiDevice_handler, deviceid, _callbackoniisan, IntPtr.Zero, 0x30000);
            IsOpen = true;
        }
        public void Start()
        {
            logger.Info("Starting Midi Service..");
            if (MidiDevice_handler != IntPtr.Zero)
            {
                W32MIDI.midiInStart(MidiDevice_handler);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            logger.Info("Closing Midi Device..");
            if (MidiDevice_handler != IntPtr.Zero)
            {
                W32MIDI.midiInClose(MidiDevice_handler);
            }
            IsOpen = false;
        }
        public void Dispose()
        {
            Dispose(true);
        }
        ~MidiLib()
        {
            Dispose(false);
        }
        private void Callback_ev(IntPtr midiIn,uint wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2)
        {
            if(wMsg == 0x3C3)
            {
                if (MidiRecieveEvent != null)
                {
                    MidiRecieveEvent((byte)((int)dwParam1 & 0xff),(byte)( (int)dwParam1 >> 8 & 0xff),(byte)( (int)dwParam1 >> 16 & 0xff));
                }
            }
        }
    }
}
