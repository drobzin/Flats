using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AgentWin.xaml
    /// </summary>
    public partial class AgentWin : Window
    {
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private int agentId;
        private DataTable dtApt = new DataTable("Appartament");
        private string adressText = "";
        private DataTable dtId = new DataTable("ApartmentId");
        private ObservableCollection<string> phoneNumbers = new ObservableCollection<string>();
        private ObservableCollection<string> appartmentId = new ObservableCollection<string>();
        private int selectedIndex;
        public AgentWin( int _agentId)
        {
            InitializeComponent();
            agentId = _agentId;
            LoadTextBoxes();
            LoadDataGrid();
        }
        private void LoadDataGrid()
        {
            dtId.Reset();
            appartmentId.Clear();
            dtApt.Clear();
            string query =  $"SELECT appartament.Street, appartament.House, Flat, district.District, Floors, Floor, TypeHouse, TypeToilet, TypePlan, SqAll, Private, Phone, Plan," +
                            $" Photo,client.Name,Cost  FROM appartament, district,client" +
                            $" WHERE(Cost is NULL  AND appartament.DistrictId = district.idDistrict AND appartament.RegID = client.RegId); ";

            string idquery = $"SELECT idAppartament FROM appartament, district, client WHERE(Cost is NULL  AND appartament.DistrictId = district.idDistrict AND appartament.RegID = client.RegId)";
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, dataConnect);
            adapter.Fill(dtApt);

            MySqlDataAdapter idAdapter = new MySqlDataAdapter(idquery, dataConnect);
            idAdapter.Fill(dtId);
            for (int i = 0; i < dtId.Rows.Count; i++)
            {
                appartmentId.Add(dtId.Rows[i][0].ToString());
            }

            apt.ItemsSource = dtApt.DefaultView;
        }
        private void LoadTextBoxes()
        {
            string query = $"SELECT Name FROM agent" +
                            $" WHERE (idagent = {agentId})";
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            name.Text = $"Фамилия: {cmd.ExecuteScalar()}";
            connection.Close();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        protected override void OnClosed(EventArgs e)
        {
            Owner.Show();
            base.OnClosed(e);
        }

        private void ChangeCost_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                selectedIndex = apt.SelectedIndex;
                if (selectedIndex == -1) 
                {
                    throw new System.ArgumentOutOfRangeException();
                }
                cost.Visibility = Visibility.Visible;
                accept.Visibility = Visibility.Visible;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("Выберите строку");
            }
        }

        private void Accept_click(object sender, RoutedEventArgs e)
        {
            string updateInstruction = "UPDATE appartament, treety SET treety.AgentId = @agentId, appartament.Cost = @cost WHERE (treety.AppartamentId = @aptId AND appartament.idAppartament = @aptId)";
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            MySqlCommand cmd = new MySqlCommand(updateInstruction, connection);
            cmd.Parameters.AddWithValue("@agentId", agentId);
            cmd.Parameters.AddWithValue("@cost", cost.Text);
            cmd.Parameters.AddWithValue("@aptId", appartmentId[selectedIndex]);
            connection.Open();
            cmd.ExecuteNonQuery();          
            connection.Close();
            SetProlong();
            LoadDataGrid();
        }
        private void SetProlong()
        {
            string instruction = "Select DateStop FROM treety WHERE AppartamentId = @aptId";
            DateTime timeNow = DateTime.Now;
            DateTime timeStop;            
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            MySqlCommand cmd = new MySqlCommand(instruction, connection);
             cmd.Parameters.AddWithValue("@aptId", appartmentId[apt.SelectedIndex]);
            connection.Open();
            //cmd.ExecuteNonQuery();
            timeStop = (DateTime)cmd.ExecuteScalar();
            if (timeNow.CompareTo(timeStop) > 0 )
            {
                string updateInstruction = "UPDATE Treety SET Prolong = @prolong WHERE AppartamentId = @aptId ";
                cmd.CommandText = updateInstruction;
                cmd.Parameters.AddWithValue("@prolong", timeNow);
                //cmd.Parameters.AddWithValue("@aptId", appartmentId[selectedIndex]);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}
