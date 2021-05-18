using EditALotOfImage.ViewModel;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Size = System.Drawing.Size;

namespace EditALotOfImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel _mvm;
        public MainWindow()
        {
            InitializeComponent();
            _mvm = new MainViewModel();
            DataContext = _mvm;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_mvm.RemoveBuffer();
        }
    }
}
