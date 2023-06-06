using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для WinChangeClientApt.xaml
    /// </summary>
    public partial class WinChangeClientApt : Window
    {
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private readonly int regId;
        private readonly string aptId;
        private readonly DataRow rowToChange;
        private string insertedAptId;
        public WinChangeClientApt(int _regId, DataRow selectedRow = null, string idApt ="0")
        {
            InitializeComponent();
            regId = _regId;
            aptId = idApt;
            rowToChange = selectedRow;
            LoadComboBox();
            if (rowToChange != null)
            {
                FillTextBoxes();   
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
           
            string insertInstruction = "INSERT INTO appartament SET Street = @0,House = @1,Flat = @2," +
                                        "DistrictId = (SELECT idDistrict from district where District = @districtName),Floors = @3,Floor = @4,TypeHouse = @typeHouse," +
                                        "TypeToilet = @typeToilet,TypePlan = @typePlan,SqAll = @5,Private = @isPrivate,Phone = @isPhone, " +
                                        "Photo = null, Plan = null, RegID = @regId";
            string updateInstruction = "UPDATE appartament SET Street = @0, House = @1, Flat =@2, DistrictId = (SELECT idDistrict from district where District = @districtName)," +
                                        "Floors = @3,Floor = @4,TypeHouse = @typeHouse," +
                                        "TypeToilet = @typeToilet,TypePlan = @typePlan,SqAll = @5,Private = @isPrivate,Phone = @isPhone, " +
                                        $"Photo = null, Plan = null, RegID = @regId WHERE idAppartament = '{aptId}'";
            if (rowToChange != null) AddRow(updateInstruction);
            else AddRow(insertInstruction);
        }

        private void AddRow(string instruction)
        {
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            MySqlCommand cmd = new MySqlCommand(instruction, connection);

            int i = 0;
            foreach (Control item in wrapPanel.Children)
            {
                if (item is TextBox textbox)
                {
                    if (textbox.Text != "")
                    {
                        cmd.Parameters.AddWithValue($"@{i}", textbox.Text);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue($"@{i}", null);
                    }
                    i++;
                }
            }
            cmd.Parameters.AddWithValue("@districtName", district_comboBox.SelectionBoxItem);
            cmd.Parameters.AddWithValue("@typeHouse", typeHouse_comboBox.SelectionBoxItem);
            cmd.Parameters.AddWithValue("@typeToilet", typeToilet_comboBox.SelectionBoxItem);
            cmd.Parameters.AddWithValue("@typePlan", typePlan_comboBox.SelectionBoxItem);
            cmd.Parameters.AddWithValue("@isPrivate", private_comboBox.SelectedIndex);
            cmd.Parameters.AddWithValue("@isPhone", phone_comboBox.SelectedIndex);
            cmd.Parameters.AddWithValue("@regId", regId);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();                
                connection.Close();
                Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                MessageBox.Show("Неправильный формат данных");
            }
        }
       
        private void LoadComboBox()
        {
            string districtInstruction = "SELECT District FROM district";
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(districtInstruction, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Close();
            cmd.CommandText = districtInstruction;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                district_comboBox.Items.Add(reader.GetString(0));
            }
            connection.Close();
        }
        private void FillTextBoxes()
        {
            int i = 0;
            foreach (Control item in wrapPanel.Children)
            {
                if (item is TextBox textBox )
                {
                    textBox.Text = rowToChange[i].ToString();
                    i++;
                }
                else if (item is ComboBox ) i++;
            }
        }
    }
}
