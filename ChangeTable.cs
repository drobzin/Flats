using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Flats
{
    internal class ChangeTable
    {
        private readonly static string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        public static void Add_Row(string instruction, WrapPanel wrapPanel, Window win, int id = -1)
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
            cmd.Parameters.AddWithValue($"@{i}", id);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                win.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                MessageBox.Show("Неправильный формат данных");
            }

        }
        public static void FillTextBoxes(DataRow row, WrapPanel wrapPanel)
        {
            int i = 1;
            foreach (Control item in wrapPanel.Children)
            {
                if (item is TextBox textBox)
                {
                    textBox.Text = row[i].ToString();
                    i++;
                }
            }

        }
    }
}
