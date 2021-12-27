using NLog;
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
        static public Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
        }
        private MidiLib midilibkun=null;
        private List<string> comportlist=new List<string>();
        private SerialPort serialPort = new SerialPort();
        private void SerialPortRefresh()
        {
            logger.Info("Refreshing SerialPort Device.");
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
                    logger.Debug("Found Serial Device : " + ptrname);
                    SerialPortCombo.Items.Add(prt.GetPropertyValue("Caption"));
                    comportlist.Add(ptrname.Substring(indexNo + 1).Replace(")", ""));
                }
            }
            if(SerialPortCombo.Items.Count > 0)
            {
                SerialPortCombo.SelectedIndex = 0;
            }
            logger.Info("Refreshed SerialPort Device.");
        }
        private void midiRefresh()
        {
            logger.Info("Refreshing MIDI Device.");
            MidiComboBox.Items.Clear();
            foreach (W32MIDI.MidiInCaps midiCapskun in W32MIDI.GetmidiList())
            {
                logger.Debug("Found Midi Device : " + midiCapskun.szPname);
                MidiComboBox.Items.Add(midiCapskun.szPname);
            }
            if(MidiComboBox.Items.Count > 0)
            {
                MidiComboBox.SelectedIndex = 0;
            }
            logger.Info("Refreshed MIDI Device.");
        }
        private void BT_Refresh()
        {
            StartBT.IsEnabled = false;
            if (serialPort.IsOpen)
            {
                if(midilibkun == null)
                {
                    StartBT.IsEnabled = true;
                }else
                if (!midilibkun.IsOpen)
                {
                    StartBT.IsEnabled = true;
                }
            }
            SerialCloseBT.IsEnabled = false;
            SerialOpenBT.IsEnabled = true;
            if (serialPort.IsOpen)
            {
                SerialCloseBT.IsEnabled = true;
                SerialOpenBT.IsEnabled = false;
            }
            STOPBT.IsEnabled = false;
            if(midilibkun != null)
            {
                STOPBT.IsEnabled = true; 
                if (!midilibkun.IsOpen)
                {
                    STOPBT.IsEnabled = false;
                }
            }
            {
                SerialPortStatusLabel.Content = (serialPort.IsOpen) ? "Open" : "Closed";
                if(midilibkun != null)
                MIDIStatusLabel.Content = (midilibkun.IsOpen) ? "Open" : "Closed";
            }
        }
        private void StartBT_Click(object sender, RoutedEventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                logger.Error("Closed COM Port....");
                MessageBox.Show("Please open COM Port!");
                return;
            }
            midilibkun = new MidiLib();
            midilibkun.MidiRecieveEvent += (byte status, byte dt1, byte dt2) =>
            {
                //System.Diagnostics.Debug.Print($"0x{status:X} 0x{dt1:X} 0x{dt2:X}");
                logger.Debug($"MIDI Event 0x{status:X} 0x{dt1:X} 0x{dt2:X}");
                try
                {
                    serialPort.Write(new byte[] { status, dt1, dt2 }, 0, 3);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    logger.Error(ex.Message);
                    logger.Error(ex.StackTrace);
                    try
                    {
                        serialPort.DiscardInBuffer();
                        serialPort.Close();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.Message);
                        logger.Error(ex2.Message);
                        logger.Error(ex2.StackTrace);
                    }
                    return;
                }

            };
            midilibkun.Open((uint)MidiComboBox.SelectedIndex);
            midilibkun.Start();
            BT_Refresh();
        }
        private void MIDIRefreshBT_Click(object sender, RoutedEventArgs e)
        {
            midiRefresh();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            logger.Info("Window Loaded!");
            midiRefresh();
            SerialPortRefresh();
            BT_Refresh();
        }

        private void SerialPortRefresh_Click(object sender, RoutedEventArgs e)
        {
            SerialPortRefresh();
        }

        private void STOPBT_Click(object sender, RoutedEventArgs e)
        {
            midilibkun.Dispose();
            BT_Refresh();
        }

        private void SerialCloseBT_Click(object sender, RoutedEventArgs e)
        {
            if(midilibkun != null)
            {
                if (midilibkun.IsOpen)
                {
                    midilibkun.Dispose();
                }
            }

            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                try
                {

                    logger.Info("Closing Serial Port");
                    serialPort.Close();

                    logger.Info("Closed Serial Port");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error(ex.StackTrace);
                    MessageBox.Show(ex.Message);
                }
            }
            BT_Refresh();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(midilibkun != null)
            midilibkun.Dispose();
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                try
                {

                    logger.Info("Closing Serial Port");
                    serialPort.Close();

                    logger.Info("Closed Serial Port");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error(ex.StackTrace);
                    MessageBox.Show(ex.Message);
                }
            }
            logger.Info("Application Closed...");
        }

        private void SerialOpenBT_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Opening Serial Port...");
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                try
                {
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    logger.Error(ex.StackTrace);
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
                logger.Info("Opened Serial Port");

            }catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                MessageBox.Show(ex.Message);
            }
            BT_Refresh();
        }

    }
}
