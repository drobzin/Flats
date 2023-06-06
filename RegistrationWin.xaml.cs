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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Panels;

namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWin.xaml
    /// </summary>
    public partial class RegistrationWin : Window
    {
        private readonly static string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private readonly string insertInstruction = "INSERT INTO client " +
                                    "( Login, Password, Name, Street, House,Document)" +
                                    " VALUES (@0,@1,@3, @4,@5,@6)";
        private string phoneBoxEmpty;
       
        public RegistrationWin()
        {
            
            InitializeComponent();
            phoneBoxEmpty = phone_box.Text;
            
           
            
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (phone_box.Text != phoneBoxEmpty)
            {
                if (password_box.Text == passwordCheck_box.Text)
                {
                    string getRegId = "SELECT last_insert_id()";
                    string insetrPhoneIsntruction = "INSERT INTO clientphone (RegId, Phone) VALUES (@regId, @phone)";
                    MySqlConnection connection = new MySqlConnection();
                    connection.ConnectionString = dataConnect;
                    MySqlCommand cmd = new MySqlCommand(insertInstruction, connection);
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
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = getRegId;
                        string regId = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = insetrPhoneIsntruction;
                        cmd.Parameters.AddWithValue("@regId", regId);
                        cmd.Parameters.AddWithValue("@phone", phone_box.Text);
                        cmd.ExecuteNonQuery();
                        connection.Close();
                        Close();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);//"Неправильный формат данных"
                    }

                }
                else MessageBox.Show("Введенные пароли не совпадают");
            }
            else MessageBox.Show("Введите номер");
            
        }
        protected override void OnClosed(EventArgs e)
        {
            Owner.Show(); 
            base.OnClosed(e);
        }
    }
}
