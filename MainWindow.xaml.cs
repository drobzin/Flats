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


namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string dataConnect = "server = localhost; user = root; database = center; password = 3245107869m";
        private readonly DataSet ds = new DataSet("Table names");
        private readonly DataSet ds_load = new DataSet("All tables");


        public MainWindow()
        {

            InitializeComponent();

        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            LogInWin logInWin = new LogInWin();
            logInWin.Owner = this;
            logInWin.Show();
            this.Hide();
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWin registrationWin = new RegistrationWin();
            registrationWin.Owner = this;
            registrationWin.Show();
            this.Hide();
        }
    }
}



