using System.Diagnostics;
using System.Windows;
using UILogic;


namespace YOLOApp
{
    public partial class MainWindow : Window, IUIRealization
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ContextBase(this);
        }

        public string getDirByDialog()
        {
            string path = "";

            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = dialog.SelectedPath;
            }
            return path;
        }

        public void showImage(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        public void showLoardingMenu()
        {
            listbox.Visibility = Visibility.Collapsed;
            loading_panel.Visibility = Visibility.Visible;
        }

        public void hideLoardingMenu()
        {
            loading_panel.Visibility = Visibility.Collapsed;
            listbox.Visibility = Visibility.Visible;
        }
    }
}