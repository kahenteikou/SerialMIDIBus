using System;
using System.Collections.Generic;
using System.Linq;
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
        private void midiRefresh()
        {

            IEnumerable<W32MIDI.MidiInCaps> midiInCapsEnumerator = W32MIDI.GetmidiList();
            foreach (W32MIDI.MidiInCaps midiCapskun in midiInCapsEnumerator)
            {
                MessageBox.Show(midiCapskun.szPname);
                MessageBox.Show(midiCapskun.indexnumber.ToString());
            }

        }
        private void StartBT_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
