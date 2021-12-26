using System;
using System.Collections.Generic;
using System.IO.Ports;
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
        private MidiLib midilibkun=null;
        private List<string> comportlist=new List<string>();
        private SerialPort serialPort = new SerialPort();
        private void SerialPortRefresh()
        {
            int indexNo = 0;
            comportlist.Clear();
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
            if(SerialPortCombo.Items.Count > 0)
            {
                SerialPortCombo.SelectedIndex = 0;
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
            if(MidiComboBox.Items.Count > 0)
            {
                MidiComboBox.SelectedIndex = 0;
            }
        }
        private void StartBT_Click(object sender, RoutedEventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("Please Open COM Port!");
                return;
            }
            midilibkun = new MidiLib();
            midilibkun.MidiRecieveEvent += (byte status, byte dt1, byte dt2) =>
            {
                System.Diagnostics.Debug.Print($"0x{status:X} 0x{dt1:X} 0x{dt2:X}");
                try
                {
                    serialPort.Write(new byte[] { status, dt1, dt2 }, 0, 3);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    serialPort.DiscardInBuffer();
                    try
                    {
                        serialPort.Close();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message);
                    }
                    return;
                }
            };
            midilibkun.Open((uint)MidiComboBox.SelectedIndex);
            midilibkun.Start();
        }
        private void SerialMIDIWrite(byte[] datakun)
        {
            try
            {
                serialPort.Write(datakun, 0, datakun.GetLength(0));
            }catch(Exception ex)
            {
                throw;
            }
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

        private void STOPBT_Click(object sender, RoutedEventArgs e)
        {
            midilibkun.Dispose();
        }

        private void SerialCloseBT_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                try
                {
                    serialPort.Close();
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                try
                {
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SerialOpenBT_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                try
                {
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            serialPort.PortName = comportlist[SerialPortCombo.SelectedIndex];
            serialPort.BaudRate = 31250;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Handshake = Handshake.None;
            serialPort.Encoding = Encoding.Default;
            try
            {
                serialPort.Open();

            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
