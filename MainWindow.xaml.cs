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
using Npgsql;

namespace kurs_rab
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string conn;
            string log = login.Text;
            string pass = password.Password.ToString();
            switch(log + pass) 
            { 
                case("admin" + "admin"):
                        conn = "Host=localhost;Port=5432;Database=FilesDb;Username=admin;Password=admin";
                    break;
                case("guest" + "guest"):
                        conn = "Host=localhost;Port=5432;Database=FilesDb;Username=guest;Password=guest";
                    break;
                default: 
                        MessageBox.Show("Неправильно введенный логин или пароль!");
                    return;
            }
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection(conn);
            var window1 = new Window1(npgsqlConnection);
            window1.Show();
            this.Close();
        }
    }
}
