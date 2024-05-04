using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using YOLOLogic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UILogic
{
    public class ContextBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] String propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<BitmapImage> images { get; set; }

        public ICommand chooseDirCommand { get; private set; }
        public ICommand chooseSaveDirCommand { get; private set; }
        public ICommand runInferenceCommand { get; private set; }
        public ICommand imageClickCommand { get; private set; }

        IUIRealization ui;

        private string _loadDirPath;
        public string loadDirPath 
        {
            get { return _loadDirPath; } 
            set { _loadDirPath = value; RaisePropertyChanged("loadDirPath"); } 
        }
        private string _saveDirPath;
        public string saveDirPath
        {
            get { return _saveDirPath; }
            set { _saveDirPath = value; RaisePropertyChanged("saveDirPath"); }
        }

        public double precision { get; set; }

        public ContextBase(IUIRealization ui)
        {
            precision = 0.5;
            this.ui = ui;
            images = new ObservableCollection<BitmapImage>();

            chooseDirCommand = new RelayCommand(chooseDir, _ => true);
            chooseSaveDirCommand = new RelayCommand(chooseSaveDir, _ => true);
            runInferenceCommand = new RelayCommand(runInference,
                                                   _ => !string.IsNullOrEmpty(saveDirPath) 
                                                   && !string.IsNullOrEmpty(loadDirPath)
                                                   && precision > 0 && precision <= 1);
            imageClickCommand = new RelayCommand(onClick, _ => images.Count > 0);
        }

        private void chooseDir(object sender)
        {
            var tmp = ui.getDirByDialog();
            if (!string.IsNullOrEmpty(tmp))
                loadDirPath = tmp;
        }

        private void chooseSaveDir(object sender)
        {
            var tmp = ui.getDirByDialog();
            if (!string.IsNullOrEmpty(tmp))
                saveDirPath = tmp;
        }

        private async void runInference(object sender)
        {
            ui.showLoardingMenu();
            images.Clear();
            
            await Task.Run(() =>
            {

                var paths = new ModelParams(saveDirPath);
                paths.precision = precision;
                DirectoryInfo imagesDir = new DirectoryInfo(loadDirPath);
                foreach (FileInfo imageFile in imagesDir.GetFiles("*.jpg"))
                {
                    paths.addPath(imageFile.FullName);
                }

                var model = new Model(paths);
                model.infer();
            });
           
            DirectoryInfo imagesDir = new DirectoryInfo(saveDirPath);
            foreach (FileInfo imageFile in imagesDir.GetFiles("*.jpg"))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageFile.FullName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();
                images.Add(bitmap); 
            }
            ui.hideLoardingMenu();
        }

        public void onClick(object info)
        {
            ui.showImage((info as Uri).ToString());
        }

    }
}