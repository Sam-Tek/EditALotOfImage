using EditALotOfImage.ViewModel;
using System.Windows;


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

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _mvm.RunEditorPreview();
        }
    }
}
