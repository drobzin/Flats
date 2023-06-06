using MySql.Data.MySqlClient;
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

namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для LogInWIn.xaml
    /// </summary>
    public partial class LogInWin : Window
    {
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        public LogInWin()
        {
            InitializeComponent();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            switch (user.SelectedIndex) 
            {
                case 0: // Клиент
                    string userSelection = "SELECT RegId FROM CLIENT WHERE (Login =@login AND Password = @password)";
                    object userAnswer = FindUser(userSelection);
                    if (userAnswer != null)
                    {
                        ClientWin clientWin = new ClientWin(Convert.ToInt32(userAnswer));
                        clientWin.Owner = this;
                        clientWin.Show();
                        this.Hide();
                    }
                    else
                    {
                        if (MessageBox.Show("Такого пользователя не существует, перейти к регистрации?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            RegistrationWin registrationWin = new RegistrationWin();
                            registrationWin.Owner = this;
                            Hide();
                            registrationWin.Show();
                        }
                       
                        
                    }
                    break;
                case 1: // Агент
                    string agentSelection = "SELECT idagent FROM agent WHERE ( login = @login AND password = @password)";
                    object agentAnswer = FindUser(agentSelection);
                    if (agentAnswer != null) 
                    { 
                        AgentWin agentWin = new AgentWin(Convert.ToInt32(agentAnswer));
                        agentWin.Owner = this;
                        agentWin.Show();
                        this.Hide();
                    }
                    else MessageBox.Show("fuuuuuuuuu"); // Тут вывод сообщения об ошибке
                    break;
                case 2: // Админ
                    if (login_box.Text == "admin" && password_box.Password == "123")
                    {
                        AdminWin adminWin = new AdminWin();
                        adminWin.Owner = this;
                        adminWin.Show();
                        this.Hide();
                         
                       
                        
                    }
                    else MessageBox.Show("fuuuuuuuuu"); // Вывод сообщения о недостпуности
                    break;
                default:
                    MessageBox.Show("Выберите аккаунт");
                    break;
            }
        }

        private object FindUser(string selection)
        {            
            MySqlConnection connection = new MySqlConnection();
            connection.ConnectionString = dataConnect;
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(selection, connection);
            cmd.Parameters.AddWithValue("@login", login_box.Text);
            cmd.Parameters.AddWithValue("@password", password_box.Password);
            var answer = cmd.ExecuteScalar();
            connection.Close();
            return answer;
        }
        protected override void OnClosed(EventArgs e)
        {
            try
            {
                Owner.Show();
            }
            catch(Exception) { }
            base.OnClosed(e);
        }
    }
}
