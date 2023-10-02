using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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
using Npgsql;

namespace kurs_rab
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public readonly NpgsqlConnection conn;
        public Window1(NpgsqlConnection connection)
        {
            InitializeComponent();
            conn = connection;
        }

        private void Files_Click(object sender, RoutedEventArgs e)
        {
            var files = new Files(conn);
            files.Show();
        }

        private void Devices_Click(object sender, RoutedEventArgs e)
        {
            var device = new Devices(conn);
            device.Show();

        }

        private void InfAboutFiles_Click(object sender, RoutedEventArgs e)
        {
            var filesinfo = new FilesType(conn);
            filesinfo.Show();   
        }

        private void TypeFiles_Click(object sender, RoutedEventArgs e)
        {
            var deviceType = new DeviceType(conn);
            deviceType.Show();
        }
    }
}
