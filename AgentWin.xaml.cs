using MySql.Data.MySqlClient;
using System;
using System.Collections;
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
using Xceed.Wpf.AvalonDock.Themes;

namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для AgentWin.xaml
    /// </summary>
    public partial class AgentWin : Window
    {
        private readonly string inWorkAptQuery;
        private readonly string inWorkAptIdQuery;
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private int agentId;
        private DataTable dtApt = new DataTable("Appartament");
        private DataTable dtId = new DataTable("ApartmentId");
        private ObservableCollection<string> phones = new ObservableCollection<string>();
        private ObservableCollection<string> appartmentId = new ObservableCollection<string>();
        private int selectedIndex;
        private bool isInitialized = false;
        
        public AgentWin( int _agentId)
        {
            InitializeComponent();
            isInitialized = true;
            agentId = _agentId;
            LoadTextBoxes();
            inWorkAptQuery = $"SELECT appartament.Street, appartament.House, Flat, district.District, Floors, Floor, TypeHouse, TypeToilet, TypePlan, SqAll, Private, Phone, Plan," +
                           $" Photo,client.Name,Cost  FROM appartament, district,client" +
                           $" WHERE(Cost is NULL  AND appartament.DistrictId = district.idDistrict AND appartament.RegID = client.RegId AND AgentId  = {agentId}); ";
            inWorkAptIdQuery = $"SELECT idAppartament FROM appartament, district, client " +
                                $"WHERE(Cost is NULL  AND appartament.DistrictId = district.idDistrict AND appartament.RegID = client.RegId AND AgentId = {agentId})";
            LoadDataGrid(inWorkAptQuery,inWorkAptIdQuery);
        }
        private void LoadDataGrid(string query, string idquery)
        {
            dtId.Reset();
            appartmentId.Clear();
            dtApt.Clear();
           

            
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
            string updateInstruction = "UPDATE appartament, treety SET  appartament.Cost = @cost WHERE (treety.AppartamentId = @aptId AND appartament.idAppartament = @aptId AND treety.AgentId = @agentId)";
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
            LoadDataGrid(inWorkAptQuery, inWorkAptIdQuery);
            cost.Visibility = Visibility.Collapsed;
            accept.Visibility = Visibility.Collapsed;
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
            timeStop = (DateTime)cmd.ExecuteScalar();
            if (timeNow.CompareTo(timeStop) > 0 )
            {
                string updateInstruction = "UPDATE Treety SET Prolong = @prolong WHERE AppartamentId = @aptId ";
                cmd.CommandText = updateInstruction;
                cmd.Parameters.AddWithValue("@prolong", timeNow);
                cmd.ExecuteNonQuery();
            }
            connection.Close();
        }

        private void StartWork_Click(object sender, RoutedEventArgs e)
        {
            AddTreetyRow();
            typeFlats.SelectedIndex = 1;
        }

        private void TypeFlats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                string query;
                string idquery;
                switch (typeFlats.SelectedIndex)
                {
                    case 0: // Квартиры для оценки
                        query = $"SELECT appartament.Street, appartament.House, Flat, district.District, Floors, Floor, TypeHouse, TypeToilet, TypePlan, SqAll, Private, Phone, Plan," +
                               $" Photo,client.Name,Cost  FROM appartament, district,client" +
                               $" WHERE(Cost is NULL  AND appartament.DistrictId = district.idDistrict AND appartament.RegID = client.RegId AND AgentId IS NULL); ";
                        idquery = $"SELECT idAppartament FROM appartament, district, client " +
                                    $"WHERE(Cost is NULL  AND appartament.DistrictId = district.idDistrict AND appartament.RegID = client.RegId AND AgentId IS NULL)";
                        LoadDataGrid(query, idquery);
                        startWork.Visibility = Visibility.Visible;
                        changeCost.Visibility = Visibility.Collapsed;
                        break;
                    case 1: // Квартиры в обработке
                        query = inWorkAptQuery;
                        idquery = inWorkAptIdQuery;

                        LoadDataGrid(query, idquery);
                        startWork.Visibility = Visibility.Collapsed;
                        changeCost.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
        private void AddTreetyRow() 
        {
            string insertInstruction = "INSERT INTO treety SET DateStart = CURDATE(), DateStop =DATE_ADD(CURDATE(), INTERVAL 1 MONTH), Bonus = @bonus, AppartamentId = @aptId, AgentId =@agentId ";
            string updateInstruction = "UPDATE appartament SET AgentId = @agentId WHERE idAppartament = @aptId";
            int bonus = (int)dtApt.Rows[apt.SelectedIndex][dtApt.Columns.IndexOf("SqAll")] * 100;
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            MySqlCommand cmd = new MySqlCommand(insertInstruction, connection);
            cmd.Parameters.AddWithValue("@aptId", appartmentId[apt.SelectedIndex]);
            cmd.Parameters.AddWithValue("@bonus", bonus);
            cmd.Parameters.AddWithValue("@agentId", agentId);
            connection.Open();
            cmd.ExecuteNonQuery();
            cmd.CommandText = updateInstruction;
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private void ShowPhone_Click(object sender, RoutedEventArgs e)
        {
            if (apt.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите строку");
            }
            else
            {
                string selectRegId = "SELECT RegID FROM appartament WHERE idAppartament = @aptId";
                string selectPhones = "SELECT Phone FROM clientphone WHERE RegId = @regId";

                MySqlConnection connection = new MySqlConnection();
                connection.ConnectionString = dataConnect;
                MySqlCommand cmd = new MySqlCommand(selectRegId, connection);
                cmd.Parameters.AddWithValue("@aptId", appartmentId[apt.SelectedIndex]);
                connection.Open();
                string regId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = selectPhones;
                cmd.Parameters.AddWithValue("@regId", regId);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    phones.Add(reader.GetString(0));
                }
                connection.Close();



                string phonesRow = "";
                foreach (string phone in phones)
                {
                    phonesRow += $"{phone}\n";
                }
                MessageBox.Show(phonesRow);
            }
        }
    }
}
