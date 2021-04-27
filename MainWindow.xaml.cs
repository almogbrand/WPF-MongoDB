using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace D2P_Exam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static MongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("patientsDB");
        static IMongoCollection<Patient> collection = db.GetCollection<Patient>("patient");

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ReadAll();
        }

        public void ReadAll()
        {
            List<Patient> list = collection.AsQueryable().ToList<Patient>();
            dg_patients.ItemsSource = list;
            if (list.Count > 0) dg_patients.SelectedIndex = 0;
        }
        
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            Add addWindow = new Add();

            addWindow.Height = this.Height;
            addWindow.Width = this.Width;
            addWindow.Left = this.Left;
            addWindow.Top = this.Top;

            addWindow.SetCollection(collection);
            addWindow.Show();
            this.Close();
        }

        private static DataGridCell GetDataGridCell(DataGridCellInfo cellInfo)
        {
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);
            if (cellContent != null)
                return (DataGridCell)cellContent.Parent;

            return null;
        }

        private void dg_patients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Patient patient = (Patient) dg_patients.SelectedItem;
            if (patient == null) return;

            var name_cell = GetDataGridCell(dg_patients.SelectedCells.ElementAt(1));

            name_cell.Focus();
            name_cell.IsEditing = true;
            name_cell.Focus();
        }

        private void btb_view_Click(object sender, RoutedEventArgs e)
        {
            Patient patient = (Patient)dg_patients.SelectedItem;

            if (patient == null) return;

            SpecificPatientDataWindow w = new SpecificPatientDataWindow();

            w.SetPatient(patient);

            w.Height = this.Height;
            w.Width = this.Width;
            w.Left = this.Left;
            w.Top = this.Top;

            this.Hide();
            w.Show();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            Patient patient = (Patient)dg_patients.SelectedItem;

            if (patient == null) return;
            collection.DeleteOne(p => p.Id == patient.Id);

            ReadAll();
        }

        private void dg_patients_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel) return;

            Patient patient = (Patient)dg_patients.SelectedItem;
            TextBox editedTextbox = e.EditingElement as TextBox;
            string newName = editedTextbox.Text.ToString();
            MessageBox.Show("Renamed " + patient.Name + " to " + newName);
            var updateDef = Builders<Patient>.Update.Set(p => p.Name, newName);
            collection.UpdateOne(p => p.Id == patient.Id, updateDef);
        }
    }
}
