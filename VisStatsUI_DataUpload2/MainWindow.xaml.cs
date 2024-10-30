using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisStatsBL.Interfaces;
using VisStatsBL.Manager;
using VisStatsDL_File;
using VisStatsDL_SQL;

namespace VisStatsUI_DataUpload2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog(); //zorgt ervoor dat je een bestand kan selecteren als op uplaod bestand klikt 
        string conn = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=VisStats;Integrated Security=True;Trust Server Certificate=True";
        IFileProcessor _fileProcessor;
        IVisStatsRepository _visStatsRepository;
        VisStatsManager _visStatsManager;

        public MainWindow()
        {
            InitializeComponent();  //werkt soms wel, soms niet. waaaaaarom??? 
            dialog.DefaultExt = ".txt"; //default file extension
            dialog.Filter = "Text documtents (.txt)|*.txt"; //filster files by extension
            dialog.InitialDirectory = @"C:\Hogent\ProgrammerenGevorderd\project1\Vis"; //start in die diractory, waardoor je minder miet klikken 
            dialog.Multiselect = true;
            _fileProcessor = new FileProcessor();
            _visStatsRepository = new VisStatsRepository(conn);
            _visStatsManager = new VisStatsManager(_fileProcessor, _visStatsRepository);
        }

        private void Button_Click_Vissoorten(object sender, RoutedEventArgs e)
        {
            bool? result = dialog.ShowDialog(); //als venster wordt getoond contoleer dan of het true is. bij annuleren niets doen
            if (result == true)
            {
                var filenames = dialog.FileNames;
                VissoortenFileListBox.ItemsSource = filenames; //itemsouce = lijst van items
                dialog.FileName = null;
            }
            else VissoortenFileListBox.ItemsSource = null; //op annulleren klikken is box terug leeg maken 
        }

        private void Button_Click_UploadVissoorten(object sender, RoutedEventArgs e)
        {
            foreach (string fileName in VissoortenFileListBox.ItemsSource)
            {
                _visStatsManager.UploadVissoorten(fileName);
            }
            MessageBox.Show("Upload klaar", "VisStats");
        }

        private void Button_Click_Havens(object sender, RoutedEventArgs e)
        {
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                var filenames = dialog.FileNames;
                HavensFileListBox.ItemsSource = filenames;
                dialog.FileName = null;
            }
            else HavensFileListBox.ItemsSource = null;
        }

        private void Button_Click_UploadHavens(object sender, RoutedEventArgs e)
        {
            foreach (string fileName in HavensFileListBox.ItemsSource)
            {
                _visStatsManager.UploadHavens(fileName);
            }
            MessageBox.Show("Upload klaar", "VisStats");
        }

        private void Button_Click_Statistieken(object sender, RoutedEventArgs e)
        {
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                var filenames = dialog.FileNames;
                StatistiekenFileListBox.ItemsSource = filenames;
                dialog.FileName = null;
            }
            else StatistiekenFileListBox.ItemsSource = null;
        }

        private void Button_Click_UploadStatistieken(object sender, RoutedEventArgs e)
        {
            foreach (string fileName in StatistiekenFileListBox.ItemsSource)
            {
                _visStatsManager.UploadStatistieken(fileName);
            }
            MessageBox.Show("Upload klaar", "VisStats");
        }
    }
}