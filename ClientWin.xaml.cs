using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
        string adressText = "";
        public ClientWin(int _regId)
        {
            InitializeComponent();
            regId = _regId;
            LoadDataGrid();
            LoadTextBoxes();
        }
        private void LoadDataGrid()
        {
            string query =  $"SELECT street.StreetName, House, Flat, district.District, Floors, Floor, TypeHouse, TypeToilet, TypePlan, SqAll, Private, Phone, Cost," +
                            $" Photo, Plan FROM appartament, district,street" +
                            $" WHERE(RegId = {regId} AND appartament.StreetId = street.StreetId AND appartament.DistrictId = district.idDistrict); ";
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, dataConnect);
            adapter.Fill(dtApt);
            apt.DataContext = dtApt;
        }
        private void LoadTextBoxes()
        {
            string query =  $"SELECT Name, street.StreetName, House, clientphone.Phone FROM client, street,clientphone" +
                            $" WHERE (client.RegID = {regId} AND street.StreetId = client.StreetId AND clientphone.RegId = client.RegID)";
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
                    adressText = $"{reader.GetString(1)},{reader.GetString(2)}" ;
                    adress.Text += $" {adressText}";
                    phone.Text += $" {reader.GetString(3)}";
                }
                else phone.Text += $"\n\t {reader.GetString(3)}";

            }
            
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
            adress.Text = changeAdress.newAdress;

        }

        private void ChangePhone_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Я НЕ ЕБУ ЧТО С ЭТИМ ДЕЛАТЬ ПОМЩЬ! ПОМОЩЬ!");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
