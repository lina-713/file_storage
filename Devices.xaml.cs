using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
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
    /// Логика взаимодействия для Devices.xaml
    /// </summary>
    public partial class Devices : Window
    {
        public readonly NpgsqlConnection conn;
        public Devices(NpgsqlConnection connecction)
        {
            InitializeComponent();
            conn = connecction;
            InitializationGrid();
            TypeOfDeviceDictionary();
            if (conn.UserName == "guest")
            {
                AddNewDevice.Visibility = Visibility.Hidden;
                UpdateDevice.Visibility = Visibility.Hidden;
                DeleteDevice.Visibility = Visibility.Hidden;
            }
        }

        public void TypeOfDeviceDictionary()
        {
            comboDevice.Items.Clear();
            string str = "SELECT * FROM type_of_device";
            var da = new NpgsqlDataAdapter(str, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "type_of_device");
            comboDevice.ItemsSource = ds.Tables[0].DefaultView;
            comboDevice.DisplayMemberPath = ds.Tables[0].Columns["name"].ToString();
            comboDevice.SelectedValuePath = ds.Tables[0].Columns["id"].ToString();
        }
       
        public void FillGrid()
        {
            string str = "Select * from view_device";
            var devices = new List<View_Device>();
            var comm = new NpgsqlCommand(str, conn);
            conn.Open();
            var reader = comm.ExecuteReader();

            while (reader.Read())
            {
                var device = new View_Device()
                {
                    Id = Int32.Parse(reader["id"].ToString()),
                    NameDevice = reader["name"].ToString(),
                    Memory_size = Int32.Parse(reader["memory_size"].ToString()),
                    Type = reader["name_of_type"].ToString()
                };
                devices.Add(device);
            }
            device_grid.ItemsSource = devices;
            device_grid.Columns[0].Visibility = Visibility.Hidden;
            conn.Close();       
        }

        public void InitializationGrid()
        {
            Type itemType = typeof(View_Device);

            var properties = itemType.GetProperties();

            foreach (var property in properties)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                DisplayAttribute displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                column.Header = displayAttribute != null ? displayAttribute.Name : property.Name;
                column.Binding = new Binding(property.Name);
                device_grid.Columns.Add(column);
            }

            device_grid.AutoGenerateColumns = false;

            FillGrid();
        }

        private void AddNewDevice_Click(object sender, RoutedEventArgs e)
        {
            int key = 1;
            var add = new UpAddDevice(conn, key, 0);
            add.Show(); 
            add.Closing += FillGridEvent;
        }
        public void FillGridEvent(object sender, CancelEventArgs e)
        {
            FillGrid();
        }

        private void EnterDevice_Click(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void UpdateDevice_Click(object sender, RoutedEventArgs e)
        {
            var device = device_grid.SelectedItem as View_Device;
            var id = device.Id;
            int key = 2;
            var update = new UpAddDevice(conn, key, id);
            update.Show();
            update.Closing += FillGridEvent;
        }

        private void DeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            var device = device_grid.SelectedItem as View_Device;
            var id = device.Id;

            var dialog = MessageBox.Show("Вы уверены, что хотите удалить это устойство?", "Удаление", MessageBoxButton.YesNoCancel);
            if (dialog == MessageBoxResult.Yes)
            {
                conn.Open();
                var command = new NpgsqlCommand("delete_device", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@delete_id", id);
                command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Устройство удалено!");
                FillGrid();
            }
        }

        private void comboDevice_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var id = (e.AddedItems[0] as DataRowView).Row.ItemArray[0];
            string str = $"Select * from view_device where name_of_type = (select name from type_of_device where id = {id})";
            var devices = new List<View_Device>();
            var comm = new NpgsqlCommand(str, conn);
            conn.Open();
            var reader = comm.ExecuteReader();

            while (reader.Read())
            {
                var device = new View_Device()
                {
                    Id = Int32.Parse(reader["id"].ToString()),
                    NameDevice = reader["name"].ToString(),
                    Memory_size = Int32.Parse(reader["memory_size"].ToString()),
                    Type = reader["name_of_type"].ToString()
                };
                devices.Add(device);
            }
            device_grid.ItemsSource = devices;
            device_grid.Columns[0].Visibility = Visibility.Hidden;
            conn.Close();
        }
    }
    public class View_Device
    {
        public int Id { get; set; }
        [Display(Name = "Имя устройства")]
        public string NameDevice { get; set; }
        [Display(Name = "Объем памяти")]
        public int Memory_size { get; set; }
        [Display(Name = "Тип устройства")]
        public string Type { get; set; }
    }
}
