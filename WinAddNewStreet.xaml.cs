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
using System.Windows.Shapes;
using System.Data;
using MySql.Data.MySqlClient;

namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для WinAddNewStreet.xaml
    /// </summary>
    public partial class WinAddNewStreet : Window
    {
        private readonly static string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        public DataSet ds_edit = new DataSet("Edited Row");
        static readonly String sql = "select * from street";
        private readonly MySqlDataAdapter adapter = new MySqlDataAdapter(sql, dataConnect);
        private readonly int selected_index;
        int row_id;
        public WinAddNewStreet(int datagrid_index)
        {
            selected_index = datagrid_index;            
            InitializeComponent();
            adapter.Fill(ds_edit);
            if (selected_index != -1)
            {
               FillTextBoxes(selected_index);
            }
            Test();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            DataRow row;
            int row_count = ds_edit.Tables[0].Rows.Count;
            try
            {
                if (selected_index == -1)
                {
                    row_id = Convert.ToInt32(ds_edit.Tables[0].Rows[row_count - 1][0]);
                    row = Create_Row(row_id + 1);
                    ds_edit.Tables[0].Rows.Add(row);
                    MySqlCommandBuilder cmd = new MySqlCommandBuilder(adapter);
                    adapter.InsertCommand = cmd.GetInsertCommand();
                    adapter.Update(ds_edit.Tables[0]);

                }
                else
                {
                    MySqlCommandBuilder cmd = new MySqlCommandBuilder(adapter);
                    EditRows(row_id);
                    adapter.UpdateCommand = cmd.GetUpdateCommand();
                    adapter.Update(ds_edit.Tables[0]);
                    ds_edit.AcceptChanges();
                }
                this.Close();

            }

            catch (System.ArgumentException)
            {
                MessageBox.Show("Неправильный формат данных");
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                MessageBox.Show("Неправильный внешний ключ");

            }


        }

        private DataRow Create_Row(int row_id)
        {
            
            var new_row = ds_edit.Tables[0].NewRow();
            new_row[0] = row_id;
            new_row[1] = streetname_box.Text;
            return new_row;
        }
        private void FillTextBoxes(int edit_index)
        {
            var edited_row = ds_edit.Tables[0].NewRow();
            for (int i=0; i<ds_edit.Tables[0].Columns.Count; i++)
            {
                edited_row[i] = ds_edit.Tables[0].Rows[selected_index][i];
            }
            row_id = Convert.ToInt32(edited_row[0]);
            streetname_box.Text = edited_row[1].ToString();
        }
        private void EditRows(int row_id)
        {
            ds_edit.Tables[0].Rows[selected_index][0] = row_id;
            ds_edit.Tables[0].Rows[selected_index][1] = streetname_box.Text;           
        }
        private void Test()
        {
            foreach(Control tb in wp.Children)
            {
                if ( tb is TextBox)
                {
                    TextBox tbb = (TextBox)tb;
                    tbb.Text = "PIDORASI";
                }
            }
        }
    }
   
}
