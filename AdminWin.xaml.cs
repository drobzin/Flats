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

namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class AdminWin : Window
    {
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private readonly DataSet ds = new DataSet("Table names");
        private readonly DataSet ds_load = new DataSet("All tables");


        public AdminWin()
        {

            InitializeComponent();
            LoadComboBox();

        }
        private void LoadComboBox()
        {
            string query = "show tables";
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, dataConnect);
            adapter.Fill(ds);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var table_name = ds.Tables[0].Rows[i][0]; // Первое значение по ячейка вертикали, второе по горизонтали
                Tables.Items.Add(table_name);
            }
        }
        private void Tables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                search_box.Clear();
                ds_load.Reset();

                string query_load = $"select * from {Tables.SelectedItem}";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query_load, dataConnect);
                adapter.Fill(ds_load);
                Database.DataContext = ds_load.Tables[0];
                Load_names_list(ds_load.Tables[0]);

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Create_Row_Click(object sender, RoutedEventArgs e)
        {
            Database.SelectedIndex = -1;
            switch (Tables.SelectedIndex)
            {
                case 0:
                    Add_New_Appartment_Row();
                    break;
                case 1:
                    Add_New_Client_Row();
                    break;
                case 2:
                    Add_New_ClientPhone_Row();
                    break;
                case 3:
                    Add_New_District_Row();
                    break;
                case 4:
                    Add_New_Street_Row();
                    break;
                case 5:
                    Add_New_Treety_Row();
                    break;
            }
        }
        private void Add_New_Appartment_Row()
        {
            WinAddNewAppartmentRow win_add_new_apt_row = new WinAddNewAppartmentRow(Database.SelectedIndex);
            win_add_new_apt_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_apt_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }
        }
        private void Add_New_Client_Row()
        {
            WinAddNewClientRow win_add_new_client_row = new WinAddNewClientRow(Database.SelectedIndex);
            win_add_new_client_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_client_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }

        }

        private void Add_New_ClientPhone_Row()
        {
            WinAddNewClientphoneRow win_add_new_clientphone_row = new WinAddNewClientphoneRow(Database.SelectedIndex);
            win_add_new_clientphone_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_clientphone_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }

        }
        private void Add_New_District_Row()
        {
            WinAddNewDistrict win_add_new_district_row = new WinAddNewDistrict(Database.SelectedIndex);
            win_add_new_district_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_district_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }
        }
        private void Add_New_Street_Row()
        {
            WinAddNewStreet win_add_new_street_row = new WinAddNewStreet(Database.SelectedIndex);
            win_add_new_street_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_street_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }

        }
        private void Add_New_Treety_Row()
        {
            WinAddNewTreetyRow win_add_new_treety_row = new WinAddNewTreetyRow(Database.SelectedIndex);
            win_add_new_treety_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_treety_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }

        }

        private void Delete_row_Click(object sender, RoutedEventArgs e)
        {
            int index;
            string sql = $"select * from {Tables.SelectedItem}";
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, dataConnect);

            index = Database.SelectedIndex;
            try
            {
                ds_load.Tables[0].Rows[index].Delete();
                MySqlCommandBuilder cmd = new MySqlCommandBuilder(adapter);
                adapter.DeleteCommand = cmd.GetDeleteCommand();
                adapter.Update(ds_load.Tables[0]);
                ds_load.AcceptChanges();

                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException)
            {
                MessageBox.Show("Выберите строку");
            }
        }
        private void Edit_Row_Click(object sender, RoutedEventArgs e)
        {
            if (Database.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите строку для изменения");
            }
            else
            {
                switch (Tables.SelectedIndex)
                {
                    case 0:
                        Edit_Appartment_Row();
                        break;
                    case 1:
                        Edit_Client_Row();
                        break;
                    case 2:
                        Edit_ClientPhone_Row();
                        break;
                    case 3:
                        Edit_District_Row();
                        break;
                    case 4:
                        Edit_Street_Row();
                        break;
                    case 5:
                        Edit_Treety_Row();
                        break;
                }
            }
        }

        private void Edit_Appartment_Row()
        {
            WinAddNewAppartmentRow win_add_new_apt_row = new WinAddNewAppartmentRow(Database.SelectedIndex);
            win_add_new_apt_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_apt_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }
        }
        private void Edit_Client_Row()
        {
            WinAddNewClientRow win_add_new_client_row = new WinAddNewClientRow(Database.SelectedIndex);
            win_add_new_client_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_client_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }
        }
        private void Edit_ClientPhone_Row()
        {
            WinAddNewClientphoneRow win_add_new_clientphone_row = new WinAddNewClientphoneRow(Database.SelectedIndex);
            win_add_new_clientphone_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_clientphone_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }

        }
        private void Edit_District_Row()
        {
            WinAddNewDistrict win_add_new_district_row = new WinAddNewDistrict(Database.SelectedIndex);
            win_add_new_district_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_district_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }
        }
        private void Edit_Street_Row()
        {
            WinAddNewStreet win_add_new_street_row = new WinAddNewStreet(Database.SelectedIndex);
            win_add_new_street_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_street_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }
        }
        private void Edit_Treety_Row()
        {
            WinAddNewTreetyRow win_add_new_treety_row = new WinAddNewTreetyRow(Database.SelectedIndex);
            win_add_new_treety_row.ShowDialog();
            try
            {
                ds_load.Clear();
                ds_load.Merge(win_add_new_treety_row.ds_edit);
                Database.DataContext = ds_load.Tables[0];
            }
            catch (System.IndexOutOfRangeException) { }

        }

        private void Load_names_list(DataTable helptable)
        {
            names_list.Items.Clear();
            foreach (DataColumn helpcolumn in helptable.Columns)
            {
                names_list.Items.Add(helpcolumn.ColumnName);
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
                    GetColumnDataType(ds_load.Tables[0], ref src_column_type);
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
            ds_load.Clear();
            string sql = $"select * from {Tables.SelectedItem}";
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, dataConnect);
            adapter.Fill(ds_load);
            Database.DataContext = ds_load.Tables[0];
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
            DataRow[] help_DataRows = ds_load.Tables[0].Copy().Select(filter);
            ds_load.Clear();
            for (int i = 0; i < help_DataRows.Length; i++)
            {
                ds_load.Tables[0].ImportRow(help_DataRows[i]);

            }

            Database.DataContext = ds_load.Tables[0];
        }
        private void HideBoxItems()
        {
            string[] src_column_type = new string[names_list.Items.Count];
            GetColumnDataType(ds_load.Tables[0], ref src_column_type);
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

/* 
 * Добавить комбобоксы всякие для подходящих строк в таблице и выбор файлов тоже было бы здорово конечно))
 * Исправить интерфейс у датагрида, чтоб он не выглядил так ущербно (желательно сделать изменение размеров в зависимости от размеров таблицы)(поправил но все равно противный он)
 * Разобраться с датасетами (в конструкторе окон редактирования передавать ds_load и работать уже с ним)
 * ПО ХОРОШЕМУ
 * разобраться с редактированием строк в датагриде
 * мб посмотреть UI
 */

