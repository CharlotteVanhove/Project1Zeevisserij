using Microsoft.Win32;
using System.Collections.ObjectModel;
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
using VisStatsBL.Enum;
using VisStatsBL.Interfaces;
using VisStatsBL.Manager;
using VisStatsBL.Model;
using VisStatsDL_File;
using VisStatsDL_SQL;

namespace VisStatsUI_MaandStatistieken
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        string connectionString = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=VisStats;Integrated Security=True;Trust Server Certificate=True";
        IVisStatsRepository visStatsRepository;
        VisStatsManager visStatsManager;
        IFileProcessor fileProcessor;
        ObservableCollection<int> AlleJaren;
        ObservableCollection<int> GeselecteerdeJaren;
        ObservableCollection<Haven> AlleHavens;
        ObservableCollection<Haven> GeselecteerdeHavens;
        public MainWindow()
        {
            InitializeComponent();
            fileProcessor = new FileProcessor();
            visStatsRepository = new VisStatsRepository(connectionString);
            visStatsManager = new VisStatsManager(fileProcessor, visStatsRepository);
            AlleJarenListBox.ItemsSource = visStatsRepository.LeesJaartallen();
            AlleJarenListBox.SelectedIndex = 0;
            AlleHavensListBox.ItemsSource = visStatsManager.GeefHavens();
            AlleHavensListBox.SelectedIndex = 0;
            AlleJaren = new ObservableCollection<int>(visStatsManager.GeefJaartallen());
            AlleJarenListBox.ItemsSource = AlleJaren;
            AlleHavens = new ObservableCollection<Haven>(visStatsManager.GeefHavens());
            AlleHavensListBox.ItemsSource = AlleHavens;
            GeselecteerdeJaren = new ObservableCollection<int>();
            GeselecteerdeJarenListBox.ItemsSource = GeselecteerdeJaren;
            GeselecteerdeHavens = new ObservableCollection<Haven>();
            GeselecteerdeHavensListBox.ItemsSource = GeselecteerdeHavens;
            VissoortComboBox.ItemsSource = visStatsManager.GeefVissoorten();
            VissoortComboBox.SelectedIndex = 0;
        }

        private void VoegAlleJarenToeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (int jaar in AlleJaren)
            {
                GeselecteerdeJaren.Add(jaar);
            }
            AlleJaren.Clear();
        }

        private void VoegJarenToeButton_Click(object sender, RoutedEventArgs e)
        {
            List<int> jaren = new();
            foreach (int jaar in AlleJarenListBox.SelectedItems)
            {
                jaren.Add(jaar);
            }
            foreach (int jaar in jaren)
            {
                GeselecteerdeJaren.Add(jaar);
                AlleJaren.Remove(jaar);
            }
        }

        private void VerwijderdJarenButton_Click(object sender, RoutedEventArgs e)
        {
            List<int> jaren = new();
            foreach (int jaar in GeselecteerdeJarenListBox.SelectedItems)
            {
                jaren.Add(jaar);
            }
            foreach (int jaar in jaren)
            {
                GeselecteerdeJaren.Remove(jaar);
                AlleJaren.Add(jaar);
            }
        }

        private void VerwijderAlleJarenButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (int jaar in GeselecteerdeJaren)
            {
                AlleJaren.Add(jaar);
            }
            GeselecteerdeJaren.Clear();
        }

        private void VoegAlleHavensToeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Haven haven in AlleHavens)
            {
                GeselecteerdeHavens.Add(haven);
            }
            AlleHavens.Clear();
        }

        private void VoegHavensToeButton_Click(object sender, RoutedEventArgs e)
        {
            List<Haven> havens = new();
            foreach (Haven haven in AlleHavensListBox.SelectedItems)
            {
                havens.Add(haven);
            }
            foreach (Haven haven in havens)
            {
                GeselecteerdeHavens.Add(haven);
                AlleHavens.Remove(haven);
            }
        }

        private void VerwijderdHavensButton_Click(object sender, RoutedEventArgs e)
        {
            List<Haven> havens = new();
            foreach (Haven haven in GeselecteerdeHavensListBox.SelectedItems)
            {
                havens.Add(haven);
            }
            foreach (Haven haven in havens)
            {
                GeselecteerdeHavens.Remove(haven);
                AlleHavens.Add(haven);
            }
        }

        private void VerwijderAlleHavensButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Haven haven in GeselecteerdeHavens)
            {
                AlleHavens.Add(haven);
            }
            GeselecteerdeHavens.Clear();
        }

        private void ToonStatistiekenButton_Click(object sender, RoutedEventArgs e)
        {
            Eenheid eenheid;
            if ((bool)kgRadioButton.IsChecked) eenheid = Eenheid.kg; else eenheid = Eenheid.euro;
            //List<MaandVangst> vangst = visStatsManager.GeefMaandVangst(GeselecteerdeJaren.ToList(), GeselecteerdeHavens.ToList(), (Vissoort)VissoortComboBox.SelectedItem, eenheid);
            MaandStatistiekenWindow statistiekenWindow = new MaandStatistiekenWindow (GeselecteerdeJaren.ToList(), GeselecteerdeHavens.ToList(), (Vissoort)VissoortComboBox.SelectedItem, eenheid);
            statistiekenWindow.ShowDialog();
        }
    }
}