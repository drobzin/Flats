using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using System.Windows.Media.TextFormatting;

namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class AdminWin : Window
    {

        private readonly static string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private string pkName = "";
        private List<string> columnDataType = new List<string>();
        private List<string> columnNames = new List<string>();
        private DataTable dataTable = new DataTable("Content");
        bool editing;
        DataRow selectedRow;


        public AdminWin()
        {

            InitializeComponent();
            LoadNames();
        }
        private void LoadNames()
        {
            string instruction = "show tables";
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(instruction, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                TableNames.Items.Add(reader.GetString(0));
            }


        }


        private void TableNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTable();
            LoadNamesList();
        }

        private void RefreshTable()
        {
            dataTable.Reset();

            string showAllInstruction = $"select * from {TableNames.SelectedItem} ";
            try
            {

                columnNames.Clear();
                columnDataType.Clear();
                pkName = "";
                MySqlConnection connection = new MySqlConnection();
                connection.ConnectionString = dataConnect;
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(showAllInstruction, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                pkName = reader.GetName(0);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    DataColumn column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = reader.GetName(i);
                    columnDataType.Add(reader.GetDataTypeName(i).ToString());
                    columnNames.Add(reader.GetName(i).ToString());
                    dataTable.Columns.Add(column);
                }
                connection.Close();

                FillListView(showAllInstruction);

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Create_Row_Click(object sender, RoutedEventArgs e)
        {
            selectedRow = null;
            editing = false;
            OpenWindow();

        }

        private void OpenWindow()
        {
            switch (TableNames.SelectedIndex)
            {
                case 0:
                    WinAddNewAgentRow agentWin = new WinAddNewAgentRow (selectedRow, editing); 
                    ChangeRow(agentWin);
                    break;
                case 1:
                    WinAddNewAppartmentRow aptWin = new WinAddNewAppartmentRow(selectedRow, editing);
                    ChangeRow(aptWin);
                    break;
                case 2:
                    WinAddNewClientRow clientWin = new WinAddNewClientRow(selectedRow, editing);
                    ChangeRow(clientWin);
                    break;
                case 3:
                    WinAddNewClientphoneRow clientPhoneWin = new WinAddNewClientphoneRow(selectedRow, editing);
                    ChangeRow(clientPhoneWin);
                    break;

                case 4:
                    WinAddNewDistrict districtWin = new WinAddNewDistrict(selectedRow, editing);
                    ChangeRow(districtWin);
                    break;             
                case 5:
                    WinAddNewTreetyRow treetyWin = new WinAddNewTreetyRow(selectedRow, editing);
                    ChangeRow(treetyWin);
                    break;
            }

        }

        private void ChangeRow(Window window)
        {
            window.ShowDialog();
            RefreshTable();
        }


        private void Edit_Row_Click(object sender, RoutedEventArgs e)
        {
            selectedRow = null;
            try
            {
                selectedRow = dataTable.Rows[content.SelectedIndex];
                editing = true;
                OpenWindow();
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Выберите строку");
            }
        }

        private void Delete_row_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(dataTable.Rows[content.SelectedIndex][0]);
                string deleteInstruction = $"DELETE FROM {TableNames.SelectedItem} WHERE ({pkName} = @id)";
                MySqlConnection connection = new MySqlConnection();
                connection.ConnectionString = dataConnect;
                MySqlCommand cmd = new MySqlCommand(deleteInstruction, connection);
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                RefreshTable();
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("Выберите строку для удаления");
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
                    Search(filter_columname, filter_value);



                }
            }
        }
        private void Search(string filter_columname, string filter_value)
        {
            string selectInstruction = $"SELECT * FROM {TableNames.SelectedItem} WHERE {filter_columname}";
            switch (filter_type.SelectedIndex)
            {
                case 0:
                    selectInstruction += $"= '{filter_value}'";
                    break;
                case 1:
                    selectInstruction += $" LIKE '%{filter_value}%'";
                    break;
                case 2:
                    selectInstruction += $" LIKE '{filter_value}%'";
                    break;
                case 3:
                    selectInstruction += $" > '{filter_value}'";
                    break;
                case 4:
                    selectInstruction += $" >= '{filter_value}'";
                    break;
                case 5:
                    selectInstruction += $" < '{filter_value}'";
                    break;
                case 6:
                    selectInstruction += $" <= '{filter_value}'";
                    break;

            }
            FillListView(selectInstruction);

        }

        private void FillListView(string instruction)
        {
            dataTable.Clear();
            DataTable _dataTable = new DataTable("ShowContent");
            _dataTable = dataTable.Copy();
            DataRow newRow;
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(instruction, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                newRow = _dataTable.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    try
                    {

                        newRow[i] = $"{reader.GetString(i)}";
                    }
                    catch (System.Data.SqlTypes.SqlNullValueException)
                    {
                        newRow[i] = "Null";
                    }
                }
                //viewList.Add(newRow);
                _dataTable.Rows.Add(newRow);
            }
            connection.Close();
            content.DataContext = _dataTable;
            dataTable.Merge(_dataTable);
        }

        private void Reset_search_btn_Click(object sender, RoutedEventArgs e)
        {
            RefreshTable();
        }

        private void Names_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (names_list.SelectedIndex != -1)
            {
                search_box.Clear();
                HideBoxItems();
            }

        }
        private void LoadNamesList()
        {
            names_list.SelectedIndex = -1;
            names_list.Items.Clear();
            foreach (string columnName in columnNames)
            {
                names_list.Items.Add(columnName);
            }
        }
        private void HideBoxItems()
        {
            switch (columnDataType[names_list.SelectedIndex])
            {
                case "INT":
                case "DECIMAL":
                    on_entry.Visibility = Visibility.Collapsed;
                    starts_with.Visibility = Visibility.Collapsed;
                    higher.Visibility = Visibility.Visible;
                    higher_equals.Visibility = Visibility.Visible;
                    lower.Visibility = Visibility.Visible;
                    lower_equals.Visibility = Visibility.Visible;
                    break;
                case "VARCHAR":
                case "DATE":
                    on_entry.Visibility = Visibility.Visible;
                    starts_with.Visibility = Visibility.Visible;
                    higher.Visibility = Visibility.Collapsed;
                    higher_equals.Visibility = Visibility.Collapsed;
                    lower.Visibility = Visibility.Collapsed;
                    lower_equals.Visibility = Visibility.Collapsed;
                    break;
                default:
                    on_entry.Visibility = Visibility.Collapsed;
                    starts_with.Visibility = Visibility.Collapsed;
                    higher.Visibility = Visibility.Collapsed;
                    higher_equals.Visibility = Visibility.Collapsed;
                    lower.Visibility = Visibility.Collapsed;
                    lower_equals.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            Owner.Show();
            base.OnClosed(e);
        }
    }
}


