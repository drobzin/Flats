using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Xceed.Wpf.Toolkit;

namespace Flats
{
  
    public partial class WinChangePhone : Window
    {
        public ObservableCollection<string> phoneNumbers;
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private int regId;
        public WinChangePhone(ObservableCollection<string> _phoneNumbers, int _regId)
        {
            InitializeComponent();
            regId = _regId;
            phoneNumbers = _phoneNumbers;
            phonesList.ItemsSource = phoneNumbers;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (phonesList.SelectedIndex != -1)
            {
                string deleteInstruction = "DELETE FROM clientphone WHERE (Phone = @phone AND RegId = @regId)";
                MySqlConnection connection = new MySqlConnection();
                connection.ConnectionString = dataConnect;

                MySqlCommand cmd = new MySqlCommand(deleteInstruction, connection);
                cmd.Parameters.AddWithValue("@regId", regId);
                cmd.Parameters.AddWithValue("@phone", phonesList.SelectedItem.ToString());

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                phoneNumbers.Remove(phonesList.SelectedItem.ToString());

            }
            else 
            System.Windows.MessageBox.Show("Выберите номер для удаления");
            
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            phone_box.Visibility = Visibility.Visible;
            save.Visibility = Visibility.Visible;
            


        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string insertInstruction = "INSERT INTO clientphone ( RegId, Phone) VALUES ( @regId, @phone)";
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            MySqlCommand cmd = new MySqlCommand(insertInstruction, connection);
            cmd.Parameters.AddWithValue("@regId", regId);
            cmd.Parameters.AddWithValue("@phone", phone_box.Text);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                phoneNumbers.Add(phone_box.Text);
                phone_box.Clear();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Неправильный формат данных");
            }
        }
    }
}
