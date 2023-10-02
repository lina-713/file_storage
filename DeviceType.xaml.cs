using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
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

namespace kurs_rab
{
    /// <summary>
    /// Логика взаимодействия для DeviceType.xaml
    /// </summary>
    public partial class DeviceType : Window
    {
        public readonly NpgsqlConnection conn;
        public DeviceType(NpgsqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
            InitializationGrid();
            if(conn.UserName == "guest")
            {
                AddButt.Visibility = Visibility.Hidden;
                deleteButt.Visibility = Visibility.Hidden;
                UpdateButt.Visibility = Visibility.Hidden;
            }
        }

        private void AddButt_Click(object sender, RoutedEventArgs e)
        {
            int id = 0;
            var add = new UpAddDeviceType(conn, id);
            add.Show();
        }

        private void UpdateButt_Click(object sender, RoutedEventArgs e)
        {
            var deviceType = gridDeviceType.SelectedItem as DeviceTypes;
            var id = deviceType.Id;
            var update = new UpAddDeviceType(conn, id);
            update.Closing += DeviceTypeGridEvent;
            update.Show();
        }

        public void FillGrid()
        {
            string str = "Select * from view_devicetype";
            var typeofdevice = new List<DeviceTypes>();
            var comm = new NpgsqlCommand(str, conn);
            conn.Open();
            var reader = comm.ExecuteReader();

            while (reader.Read())
            {
                var type = new DeviceTypes()
                {
                    Id = Int32.Parse(reader["id"].ToString()),
                    NameType = reader["name"].ToString(),
                    Count = Int32.Parse(reader["device_count"].ToString())
                };
                typeofdevice.Add(type);
            }
            gridDeviceType.ItemsSource = typeofdevice;
            gridDeviceType.Columns[0].Visibility = Visibility.Hidden;
            conn.Close();
        }

        public void DeviceTypeGridEvent(object sender, CancelEventArgs e)
        {
            FillGrid();
        }

        public void InitializationGrid()
        {
            Type itemType = typeof(DeviceTypes);

            var properties = itemType.GetProperties();

            foreach (var property in properties)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                DisplayAttribute displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                column.Header = displayAttribute != null ? displayAttribute.Name : property.Name;
                column.Binding = new Binding(property.Name);
                gridDeviceType.Columns.Add(column);
            }

            gridDeviceType.AutoGenerateColumns = false;
            FillGrid();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void deleteButt_Click(object sender, RoutedEventArgs e)
        {
            var device = gridDeviceType.SelectedItem as DeviceTypes;
            var id = device.Id;

            var dialog = MessageBox.Show("Вы уверены, что хотите удалить этот тип устройств?", "Удаление", MessageBoxButton.YesNoCancel);
            if (dialog == MessageBoxResult.Yes)
            {
                conn.Open();
                var command = new NpgsqlCommand("delete_devicetype", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@delete_id", id);
                command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Тип устройств удалено!");
                FillGrid();
            }
        }
    }

    public class DeviceTypes
    {
        public int Id { get; set; }
        [Display(Name ="Название типа устойств")]
        public string NameType { get; set; }
        [Display(Name = "Количество устройств")]
        public int Count { get; set; }
    }

}
