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

        private string _imagePreviewChanged;

        private ObservableCollection<int> _itemResize;

        private int _itemResizeSelected;

        private int _valueContrast;

        private int _valueBrightness;


        public MainViewModel()
        {
            _mif = new MainImageFactory();
            _pathDirectory = "Path Of Directory";
            _imagePreview = DEFAULTIMAGE;
            _imagePreviewChanged = DEFAULTIMAGE;
            _itemResize = new ObservableCollection<int>() { 25, 50, 75, 100 };
            _itemResizeSelected = 100;
            _valueContrast = 0;
            _valueBrightness = 0;
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
                            Regex rx = new Regex(@"(\.jpg|\.png)$", RegexOptions.IgnoreCase);
                            ItemDirectory = new ObservableCollection<string>(Directory.GetFiles(PathDirectory).Where(f => rx.IsMatch(f)));
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
                    _mif.ListPathImage = ItemDirectory.ToList<string>();
                    _mif.EditAll(ItemResizeSelected, ValueContrast, ValueBrightness, PathDirectory);
                    ImagePreviewChanged = ImagePreview;
                    MessageBox.Show("Images are saved !!!");
                });
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
                    ImagePreviewChanged = value;
                }
                else
                    _imagePreview = @"C:\Users\samyr\Desktop\d85ccd36ba07942847806d5408d9e0d1.jpg";
                OnPropertyChanged("ImagePreview");
            }
        }


        public string ImagePreviewChanged
        {
            get { return _imagePreviewChanged; }
            set
            {
                _mif.RemoveBuffer(PathDirectory);
                if (value != DEFAULTIMAGE)
                {
                    Task<string> taskResult = Task.Run<string>(() => _mif.Edit(ItemResizeSelected, ValueContrast, ValueBrightness, value));
                    _imagePreviewChanged = taskResult.Result;
                }
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
                ImagePreviewChanged = ImagePreview;
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
                ImagePreviewChanged = ImagePreview;
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
                ImagePreviewChanged = ImagePreview;
            }
        }
    }
}
