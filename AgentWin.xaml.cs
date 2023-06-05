using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
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
        private  string query;
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private int agentId;
        private DataTable dtApt = new DataTable("Appartament");
        private DataTable dtId = new DataTable("ApartmentId");
        private ObservableCollection<string> phones = new ObservableCollection<string>();
        private int selectedIndex;
        private bool isInitialized = false;
        
        public AgentWin( int _agentId)
        {
            InitializeComponent();
            isInitialized = true;
            agentId = _agentId;
            LoadTextBoxes();
            
          
        }
        private void LoadDataGrid(string query)
        {
            dtId.Reset();
            //appartmentId.Clear();
            dtApt.Clear();                       
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, dataConnect);
            adapter.Fill(dtApt);                       
            apt.ItemsSource = dtApt.DefaultView;
            apt.Columns[0].Visibility = Visibility.Collapsed;
            
           
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
            cmd.Parameters.AddWithValue("@aptId", dtApt.Rows[apt.SelectedIndex][0]);
            connection.Open();
            cmd.ExecuteNonQuery();          
            connection.Close();
            SetProlong();
            LoadDataGrid(query);
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
            cmd.Parameters.AddWithValue("@aptId", dtApt.Rows[apt.SelectedIndex][0]);
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
                

                switch (typeFlats.SelectedIndex)
                {
                    case 0: // Квартиры для оценки
                        query = $"SELECT appartament.idAppartament, appartament.Street, appartament.House, Flat, district.District, Floors, Floor, TypeHouse, TypeToilet, TypePlan, SqAll, Private, Phone, Plan," +
                               $" Photo,client.Name,Cost  FROM appartament, district,client" +
                               $" WHERE(Cost is NULL  AND appartament.DistrictId = district.idDistrict AND appartament.RegID = client.RegId AND AgentId IS NULL); ";
                       
                        LoadDataGrid(query);
                        LoadNamesList();
                        startWork.Visibility = Visibility.Visible;
                        changeCost.Visibility = Visibility.Collapsed;
                        break;
                    case 1: // Квартиры в обработке
                        query = $"SELECT  appartament.idAppartament, appartament.Street, appartament.House, Flat, district.District, Floors, Floor, TypeHouse, TypeToilet, TypePlan, SqAll, Private, Phone, Plan," +
                           $" Photo,client.Name,Cost  FROM appartament, district,client" +
                           $" WHERE(Cost is NULL  AND appartament.DistrictId = district.idDistrict AND appartament.RegID = client.RegId AND AgentId  = {agentId}); ";
                        

                        LoadDataGrid(query);
                        LoadNamesList();
                        startWork.Visibility = Visibility.Collapsed;
                        changeCost.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void LoadNamesList()
        {
            names_list.SelectedIndex = -1;
            names_list.Items.Clear();
            foreach (DataColumn column in dtApt.Columns)
            {
                if (column.ColumnName != "idAppartament")
                names_list.Items.Add(column.ColumnName);
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
            cmd.Parameters.AddWithValue("@aptId", dtApt.Rows[apt.SelectedIndex][0]);
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
            phones.Clear();
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
                string check = dtApt.Rows[0][apt.SelectedIndex].ToString();
                cmd.Parameters.AddWithValue("@aptId", dtApt.Rows[apt.SelectedIndex][0]);
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
        private void Search_btn_Click(object sender, RoutedEventArgs e)
        {
            string filter_value;
            string filter_columname;
            string[] src_column_type = new string[names_list.Items.Count];

            if (search_box.Text == string.Empty)
            {
                filter_value = string.Empty;
                MessageBox.Show("Не введено условие для поиска");

            }
            else
            {
                filter_value = search_box.Text;
                if (names_list.SelectedItems.Count == 0)
                {
                    filter_columname = null;
                    MessageBox.Show("Не выбран столбец для поиска");
                }
                else
                {
                    filter_columname = names_list.SelectedItem.ToString();
                    GetColumnDataType(dtApt, ref src_column_type);
                    switch (src_column_type[names_list.SelectedIndex])
                    {
                        case "System.Date":
                            DateTime result_date;
                            bool success_data = DateTime.TryParse(filter_value, out result_date);
                            if (success_data)
                            {
                                Search(filter_columname, filter_value);
                            }
                            else
                            {
                                MessageBox.Show("Введенное вами значение не является датой");
                            }
                            break;
                        case "System.Int32":
                            Int16 result_int;
                            bool success_int = Int16.TryParse(filter_value, out result_int);
                            if (success_int)
                            {
                                Search(filter_columname, filter_value);
                            }
                            else
                            {
                                MessageBox.Show("Введенное вами значение не является числом!");
                            }
                            break;
                        case "System.String":
                            Search(filter_columname, filter_value);
                            break;

                        default:
                            Search(filter_columname, filter_value);
                            break;

                    }

                }
            }
        }

        private void Reset_search_btn_Click(object sender, RoutedEventArgs e)
        {
            dtApt.Clear();
            LoadDataGrid(query);
        }

        private void GetColumnDataType(DataTable helpTable, ref string[] columnType)
        {
            int j;
            foreach (DataColumn helpColumn in helpTable.Columns)
            {
                j = helpColumn.Ordinal;
                columnType[j] = helpColumn.DataType.ToString();
            }
            
            
        }

        private void Search(string filter_columname, string filter_value)
        {
            string filter = "";
            switch (filter_type.SelectedIndex)
            {
                case 0:
                    filter = $"{filter_columname} ='{filter_value}'";
                    break;
                case 1:
                    filter = $"{filter_columname} LIKE '%{filter_value}%'";
                    break;
                case 2:
                    filter = $"{filter_columname} LIKE '{filter_value}%'";
                    break;
                case 3:
                    filter = $"{filter_columname} > '{filter_value}'";
                    break;
                case 4:
                    filter = $"{filter_columname} >= '{filter_value}'";
                    break;
                case 5:
                    filter = $"{filter_columname} <'{filter_value}'";
                    break;
                case 6:
                    filter = $"{filter_columname} <='{filter_value}'";
                    break;
            }
            DataRow[] help_DataRows = dtApt.Copy().Select(filter);
            dtApt.Clear();
            for (int i = 0; i < help_DataRows.Length; i++)
            {
                dtApt.ImportRow(help_DataRows[i]);

            }
            
        }
        private void HideBoxItems()
        {
            if (names_list.SelectedIndex != -1)
            {
                string[] src_column_type = new string[names_list.Items.Count];
                GetColumnDataType(dtApt, ref src_column_type);
                switch (src_column_type[names_list.SelectedIndex])
                {
                    case "System.Date":
                        on_entry.Visibility = Visibility.Visible;
                        starts_with.Visibility = Visibility.Visible;
                        higher.Visibility = Visibility.Collapsed;
                        higher_equals.Visibility = Visibility.Collapsed;
                        lower.Visibility = Visibility.Collapsed;
                        lower_equals.Visibility = Visibility.Collapsed;
                        break;
                    case "System.String":
                        on_entry.Visibility = Visibility.Visible;
                        starts_with.Visibility = Visibility.Visible;
                        higher.Visibility = Visibility.Collapsed;
                        higher_equals.Visibility = Visibility.Collapsed;
                        lower.Visibility = Visibility.Collapsed;
                        lower_equals.Visibility = Visibility.Collapsed;
                        break;
                    case "System.Int32":
                        on_entry.Visibility = Visibility.Collapsed;
                        starts_with.Visibility = Visibility.Collapsed;
                        higher.Visibility = Visibility.Visible;
                        higher_equals.Visibility = Visibility.Visible;
                        lower.Visibility = Visibility.Visible;
                        lower_equals.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void Names_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            search_box.Clear();
            HideBoxItems();
        }
        protected override void OnClosed(EventArgs e)
        {
            Owner.Show();
            base.OnClosed(e);
        }
    }
}
