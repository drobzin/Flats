using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;


namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для ClientWin.xaml
    /// </summary>
    public partial class ClientWin : Window
    {
        public int regId;
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private DataTable dtApt = new DataTable("Appartament");
        private string adressText = "";
        private DataTable dtId = new DataTable("ApartmentId");
        private ObservableCollection<string> phoneNumbers = new ObservableCollection<string>();
        private ObservableCollection<string> appartmentId = new ObservableCollection<string>();
        string ids;
        public ClientWin(int _regId)
        {
            InitializeComponent();
            regId = _regId;
            LoadDataGrid();
            LoadTextBoxes();

        }
        private void LoadDataGrid()
        {
            dtId.Reset();
            appartmentId.Clear();
            dtApt.Clear();
            string query =  $"SELECT Street, House, Flat, district.District, Floors, Floor, TypeHouse, TypeToilet, TypePlan, SqAll, Private, Phone, Cost," +
                            $" Photo, Plan FROM appartament, district" +
                            $" WHERE(RegId = {regId}  AND appartament.DistrictId = district.idDistrict); ";
            string idquery = $"SELECT idAppartament FROM appartament, district WHERE(RegId = {regId}  AND appartament.DistrictId = district.idDistrict)";
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, dataConnect);           
            adapter.Fill(dtApt);
            
            MySqlDataAdapter idAdapter = new MySqlDataAdapter (idquery , dataConnect);
            idAdapter.Fill(dtId);
            for (int i = 0; i < dtId.Rows.Count; i++)
            {
                appartmentId.Add(dtId.Rows[i][0].ToString());
            }
           
            apt.ItemsSource = dtApt.DefaultView;
        }
        private void LoadTextBoxes()
        {
            string query =  $"SELECT Name, client.Street, House, clientphone.Phone FROM client,clientphone" +
                            $" WHERE (client.RegID = {regId}  AND clientphone.RegId = client.RegID)";
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            int i = 0;
            while(reader.Read()) 
            {
                i++;
                if (i < 2)
                {
                    name.Text += $" {reader.GetString(0)}";
                    adressText = $"{reader.GetString(1)},{reader.GetString(2)}";
                    adress.Text += $" {adressText}";
                    phoneNumbers.Add(reader.GetString(3));
                }
                else phoneNumbers.Add(reader.GetString(3));
            }
            numbers.ItemsSource = phoneNumbers; 
            connection.Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            Owner.Show();
            base.OnClosed(e);
        }

        private void ChangeAdress_Click(object sender, RoutedEventArgs e)
        {
            WinChangeAdress changeAdress = new WinChangeAdress(adressText, regId);
            changeAdress.ShowDialog();
            if (changeAdress.newAdress != "")
            {
                adressText = changeAdress.newAdress;
                adress.Text = $"Адрес: {adressText}";
            }
        }

        private void ChangePhone_Click(object sender, RoutedEventArgs e) 
        { 
            WinChangePhone changePhone = new WinChangePhone(phoneNumbers, regId);
            changePhone.ShowDialog();
            phoneNumbers = changePhone.phoneNumbers;
            
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AddApt_Click(object sender, RoutedEventArgs e)
        {
            WinChangeClientApt winChangeClientApt = new WinChangeClientApt(regId);
            winChangeClientApt.ShowDialog();
            LoadDataGrid();

        }

        private void ChangeApt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WinChangeClientApt winChangeClientApt = new WinChangeClientApt(regId, dtApt.Rows[apt.SelectedIndex], appartmentId[apt.SelectedIndex]);
                winChangeClientApt.ShowDialog();
                LoadDataGrid();
            }
            catch (System.IndexOutOfRangeException)
            {
                MessageBox.Show("Выберите строку для изменения");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string deleteInstruction = $"DELETE FROM appartament  WHERE (idAppartament = @id)";
                MySqlConnection connection = new MySqlConnection();
                connection.ConnectionString = dataConnect;
                MySqlCommand cmd = new MySqlCommand(deleteInstruction, connection);
                cmd.Parameters.AddWithValue("@id", appartmentId[apt.SelectedIndex]);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                LoadDataGrid() ;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("Выберите строку для удаления");
            }
        }
    }
}
