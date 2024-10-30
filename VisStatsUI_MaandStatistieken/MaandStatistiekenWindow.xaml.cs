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
using System.Windows.Shapes;
using VisStatsBL.Enum;
using VisStatsBL.Model;
using VisStatsDL_SQL;

namespace VisStatsUI_MaandStatistieken
{
    /// <summary>
    /// Interaction logic for MaandStatistiekenWindow.xaml
    /// </summary>
    public partial class MaandStatistiekenWindow : Window
    {
        public MaandStatistiekenWindow(List<int> jaar, List<Haven> haven, Vissoort vangst, Eenheid eenheid)
        {
            InitializeComponent();
            GeselecteerdeHavensListBox.ItemsSource = haven.ToList();
            GeselecteerdeJarenListBox.ItemsSource = jaar.ToList();
            //VissoortTextBox.Text = vangst.ToString();
            EenheidTextBox.Text = eenheid.ToString();
            //StatistiekenDataGrid.ItemsSource = vangst;
            StatistiekenDataGrid.AutoGeneratingColumn += StatistiekenDataGrid_AutoGeneratingColumn;
        }

        private void StatistiekenDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(double) || e.PropertyType == typeof(float) || e.PropertyType == typeof(decimal))
            {
                var dataGridTextColomn = e.Column as DataGridTextColumn;
                if (dataGridTextColomn != null)
                {
                    dataGridTextColomn.Binding.StringFormat = "N2";
                    Style cellStyle = new Style(typeof(DataGridCell));
                    cellStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
                    dataGridTextColomn.CellStyle = cellStyle;
                }
            }
        }
    }
}
