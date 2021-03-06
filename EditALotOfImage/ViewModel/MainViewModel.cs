using EditALotOfImage.EditImage;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using SimpleBinding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace EditALotOfImage.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public const string DEFAULTIMAGE = @"https://cdn2.iconfinder.com/data/icons/flat-pro-imaging-set-2/32/select-picture-512.png";

        public event PropertyChangedEventHandler PropertyChanged;

        private MainImageFactory _mif;

        private string _pathDirectory;

        private ObservableCollection<string> _itemDirectory;

        private string _imagePreview;

        private BitmapImage _imagePreviewChanged;

        private ObservableCollection<int> _itemResize;

        private int _itemResizeSelected;

        private int _valueContrast;

        private int _valueBrightness;

        private int _progressBar;

        private Progress<int> _progressIndicator;

        private CancellationTokenSource _cancelTokenSource;
        public MainViewModel()
        {
            _mif = new MainImageFactory();
            _pathDirectory = "Path Of Directory";
            _imagePreview = DEFAULTIMAGE;
            _imagePreviewChanged = new BitmapImage(new Uri(DEFAULTIMAGE));
            _itemResize = new ObservableCollection<int>() { 25, 50, 75, 100 };
            _itemResizeSelected = 100;
            _valueContrast = 0;
            _valueBrightness = 0;
            _progressBar = 0;
            _progressIndicator = new Progress<int>(ReportProgress);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ICommand SelectPathDirectory
        {
            get
            {
                return new RelayCommand(param =>
                {
                    System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
                    var result = openFileDlg.ShowDialog();
                    if (result.ToString() != string.Empty)
                    {
                        PathDirectory = openFileDlg.SelectedPath;
                        if (PathDirectory.Length > 0)
                        {
                            Task<ObservableCollection<string>> taskResult = Task.Run<ObservableCollection<string>>(() =>
                                new ObservableCollection<string>(Directory.GetFiles(PathDirectory).Where(f => (new Regex(@"(\.jpg|\.png)$", RegexOptions.IgnoreCase)).IsMatch(f))));
                            ItemDirectory = taskResult.Result;
                        }
                        else
                            PathDirectory = "Path Of Directory";
                    }
                });
            }
        }

        public ICommand SaveAllImage
        {
            get
            {
                return new RelayCommand(param =>
                {
                    _cancelTokenSource = new CancellationTokenSource();
                    _mif.ListPathImage = ItemDirectory.ToList<string>();
                    Task.Run(() => _mif.EditAll(ItemResizeSelected, ValueContrast, ValueBrightness, PathDirectory, _progressIndicator, _cancelTokenSource.Token));
                });
            }
        }

        public ICommand cancelSaveAction
        {
            get
            {
                return new RelayCommand(param =>
                {

                    _cancelTokenSource.Cancel();
                    _cancelTokenSource.Dispose();
                    _cancelTokenSource = null;
                },p => _cancelTokenSource != null);
            }
        }

        public string PathDirectory
        {
            get { return _pathDirectory; }
            set { _pathDirectory = value; OnPropertyChanged("PathDirectory"); }
        }

        public ObservableCollection<string> ItemDirectory
        {
            get
            {
                return _itemDirectory;
            }
            set
            {
                _itemDirectory = value;
                OnPropertyChanged("ItemDirectory");
            }
        }


        public string ImagePreview
        {
            get { return _imagePreview; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _imagePreview = value;
                    ImagePreviewChanged = new BitmapImage(new Uri(value));
                }
                else
                    _imagePreview = DEFAULTIMAGE;
                OnPropertyChanged("ImagePreview");
            }
        }


        public BitmapImage ImagePreviewChanged
        {
            get { return _imagePreviewChanged; }
            set
            {
                _imagePreviewChanged = value;
                OnPropertyChanged("ImagePreviewChanged");
            }
        }

        public ObservableCollection<int> ItemResize
        {
            get { return _itemResize; }
            set
            {
                _itemResize = value;
            }
        }

        public int ItemResizeSelected
        {
            get { return _itemResizeSelected; }
            set
            {
                _itemResizeSelected = value;
                RunEditorPreview();
            }
        }

        public int ValueContrast
        {
            get
            {
                return _valueContrast;
            }
            set
            {
                _valueContrast = value;
            }
        }

        public int ValueBrightness
        {
            get
            {
                return _valueBrightness;
            }
            set
            {
                _valueBrightness = value;
            }
        }

        //https://devblogs.microsoft.com/dotnet/async-in-4-5-enabling-progress-and-cancellation-in-async-apis/
        public int ProgressBar
        {
            get { return _progressBar; }
            set
            {
                _progressBar = value;
                OnPropertyChanged("ProgressBar");
            }
        }
        void ReportProgress(int value)
        {
            ProgressBar = value;
        }
        public async void RunEditorPreview()
        {
            _mif.RemoveBuffer(PathDirectory);
            if (ImagePreview != DEFAULTIMAGE)
            {
                //Task<string> taskResult = Task.Run<string>(() => _mif.Edit(ItemResizeSelected, ValueContrast, ValueBrightness, ImagePreviewChanged));
                //ImagePreviewChanged = taskResult.Result; 
                ImagePreviewChanged = await _mif.Edit(ItemResizeSelected, ValueContrast, ValueBrightness, ImagePreview, _progressIndicator);
            }
        }
    }
}
