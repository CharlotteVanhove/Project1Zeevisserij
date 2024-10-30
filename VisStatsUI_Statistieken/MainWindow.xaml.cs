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

namespace VisStatsUI_Statistieken
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string conn = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=VisStats;Integrated Security=True;Trust Server Certificate=True";
        
        IFileProcessor _fileProcessor;
        IVisStatsRepository _visStatsRepository;
        VisStatsManager _visStatsManager;
        ObservableCollection<Vissoort> AlleVissoorten;  //ObservableCollection zijn om lijsten te bewerken de knoppen van de pijltjes
        ObservableCollection<Vissoort> GeselecteerdeVissoorten;
        public MainWindow()
        {
            InitializeComponent();
            _fileProcessor = new FileProcessor();
            _visStatsRepository = new VisStatsRepository(conn);
            _visStatsManager = new VisStatsManager(_fileProcessor, _visStatsRepository);
            AlleVissoorten = new ObservableCollection<Vissoort>(_visStatsManager.GeefVissoorten());
            AlleSoortenListBox.ItemsSource = AlleVissoorten;
            GeselecteerdeVissoorten = new ObservableCollection<Vissoort>(); //je kan een collectie niet direct aanpassen wanneer deze wordt overlopen. eerst dus een copy maken en daarna aanpassen
            GeselecteerdeSoortenLisBox.ItemsSource = GeselecteerdeVissoorten;
            HavenComboBox.ItemsSource = _visStatsManager.GeefHavens();
            HavenComboBox.SelectedIndex = 0;
            JaarComboBox.ItemsSource = _visStatsManager.GeefJaartallen();
            JaarComboBox.SelectedIndex = 0;
        }

        private void VoegSoortenToeButton_Click(object sender, RoutedEventArgs e)
        {
            List<Vissoort> soorten = new(); // aparte lijst, want je mag niet aanpassen in een lijst als je erin werkt 

            foreach (Vissoort v in AlleSoortenListBox.SelectedItems) //kijken wat er geselecteerd is en in een lijst stoppen 
            {
                soorten.Add(v);
            }

            foreach (Vissoort v in soorten)
            {
                GeselecteerdeVissoorten.Add(v);
                AlleVissoorten.Remove(v);
            }
        }
        private void VerwijderSoortenButton_Click(object sender, RoutedEventArgs e)
        {
            List<Vissoort> soorten = new();
            foreach (Vissoort v in GeselecteerdeSoortenLisBox.SelectedItems)
            {
                soorten.Add(v);
            }

            foreach (Vissoort v in soorten)
            {
                GeselecteerdeVissoorten.Remove(v);
                AlleVissoorten.Add(v);
            }
        }
        private void VoegAlleSoortenToeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Vissoort v in AlleVissoorten)
            {
                GeselecteerdeVissoorten.Add(v);
            }
            AlleVissoorten.Clear();
            //alles uit ene lijst toevoegen aan de andere en eigen lijst leegmaken. omdat ObservableCollection gaat hij die lijsten zelf updaten 
        }
        private void VerwijderAlleSoortenButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Vissoort v in GeselecteerdeVissoorten)
            {
                AlleVissoorten.Add(v);
            }
            GeselecteerdeVissoorten.Clear();
        }
        private void ToonStatistiekenButton_Click(object sender, RoutedEventArgs e)
        {
            Eenheid eenheid = Eenheid.kg;

            List<JaarVangst> vangst = _visStatsManager.GeefVangst((int)JaarComboBox.SelectedItem, (Haven)HavenComboBox.SelectedItem, GeselecteerdeVissoorten.ToList(), eenheid);


            if ((bool)KgRadioButton.IsChecked)
            {
                eenheid = Eenheid.kg;
            }
            else
            {
                eenheid = Eenheid.euro;
            }
            
            StatistiekenWindow w = new StatistiekenWindow((int)JaarComboBox.SelectedItem, (Haven)HavenComboBox.SelectedItem, vangst, eenheid);
            w.ShowDialog(); //toont gewoon venster
        }


    }
}