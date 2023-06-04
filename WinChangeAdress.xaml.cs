using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для WinChangeAdress.xaml
    /// </summary>
    public partial class WinChangeAdress : Window
    {
        public string newAdress ="";
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private readonly int regId;
        private readonly string adressText;
        string[] splitedAdress;
        public WinChangeAdress(string _adressText , int _regId)
        {
            InitializeComponent();
            regId = _regId;
            adressText = _adressText;
            LoadTextBoxes();
        }
        private void LoadTextBoxes()
        {
            splitedAdress = adressText.Split(',');
            street_box.Text = splitedAdress[0];
            house_box.Text= splitedAdress[1];
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            string updateInstruction =  $"UPDATE client SET Street = @streetName , client.House = @house" +
                                        $" WHERE (Street = '{splitedAdress[0]}' AND client.RegId = '{regId}')";
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            MySqlCommand cmd = new MySqlCommand(updateInstruction, connection);
            cmd.Parameters.AddWithValue("@streetName", street_box.Text);
            cmd.Parameters.AddWithValue("@house", house_box.Text);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                newAdress = $"{street_box.Text},{house_box.Text}";
                this.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                MessageBox.Show("Неправильный формат данных");
            }

        }
    }
}
