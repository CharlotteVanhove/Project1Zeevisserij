using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using VisStatsBL.Manager;
using VisStatsBL.Model;

namespace VisStatsUI_Statistieken
{
    /// <summary>
    /// Interaction logic for StatistiekenWindow.xaml
    /// </summary>
    public partial class StatistiekenWindow : Window
    {
        public StatistiekenWindow(int jaar, Haven haven, List<JaarVangst> vangst, Eenheid eenheid)
        {
            InitializeComponent();
            HavenTextBox.Text = haven.ToString();
            JaarTextBox.Text = jaar.ToString();
            EenheidTextBox.Text = eenheid.ToString();
            StatistiekenDataGrid.AutoGeneratingColumn += StatistiekenDataGrid_AutoGenerateColumns;
            StatistiekenDataGrid.ItemsSource = vangst;
        }

        private void StatistiekenDataGrid_AutoGenerateColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //rechts uitlijnen en 2 cijfers na de komma
            if (e.PropertyType == typeof(double) || e.PropertyType == typeof(float) || e.PropertyType == typeof(decimal))
            {
                var dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
                {
                    // format afronden op 2 decimalen
                    dataGridTextColumn.Binding.StringFormat = "N2";
                    Style cellStyle = new Style(typeof(DataGridCell));
                    cellStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
                    dataGridTextColumn.CellStyle = cellStyle;
                }
            }
        }

        private void StatistiekenDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
