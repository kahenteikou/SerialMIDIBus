using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerialMIDIBus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private List<string> comportlist=new List<string>();
        private void SerialPortRefresh()
        {
            int indexNo = 0;
            SerialPortCombo.Items.Clear();
            ManagementClass devicekun = new ManagementClass("Win32_PnPEntity");
            foreach(ManagementObject prt in devicekun.GetInstances())
            {
                string ptrname = (string)prt.GetPropertyValue("Name");
                if (ptrname == null) continue;
                indexNo = ptrname.IndexOf("(COM");
                if(indexNo >= 0)
                {
                    SerialPortCombo.Items.Add(prt.GetPropertyValue("Caption"));
                    comportlist.Add(ptrname.Substring(indexNo + 1).Replace(")", ""));
                }
            }
        }
        private void midiRefresh()
        {
            MidiComboBox.Items.Clear();
            IEnumerable<W32MIDI.MidiInCaps> midiInCapsEnumerator = W32MIDI.GetmidiList();
            foreach (W32MIDI.MidiInCaps midiCapskun in midiInCapsEnumerator)
            {
                MidiComboBox.Items.Add(midiCapskun.szPname);
            }

        }
        private void StartBT_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MIDIRefreshBT_Click(object sender, RoutedEventArgs e)
        {
            midiRefresh();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            midiRefresh();
            SerialPortRefresh();
        }

        private void SerialPortRefresh_Click(object sender, RoutedEventArgs e)
        {
            SerialPortRefresh();
        }
    }
}
