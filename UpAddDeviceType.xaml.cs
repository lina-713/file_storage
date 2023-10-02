using Npgsql;
using NpgsqlTypes;
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
using System.Windows.Shapes;

namespace kurs_rab
{
    /// <summary>
    /// Логика взаимодействия для UpAddDeviceType.xaml
    /// </summary>
    public partial class UpAddDeviceType : Window
    {
        public readonly NpgsqlConnection conn;
        public readonly int id;
        public UpAddDeviceType(NpgsqlConnection conn, int id)
        {
            InitializeComponent();
            this.conn = conn;
            this.id = id;
            if(id == 0)
            
                this.Title = "Добавление типа устройства";
            
            else
            {
                this.Title = "Изменение типа устройства";
                EnterUpdate(id);
            }

        }
        public void EnterUpdate(int id) 
        {
            conn.Open();
            string str = $"SELECT name FROM type_of_device WHERE id = {id}";
            var command = new NpgsqlCommand(str, conn);
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                reader.Read();
                typeBox.Text = reader.GetString(0);
                reader.Close();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void UpAddDeviceType_Click(object sender, RoutedEventArgs e)
        {
            AddOrUpdateDevice(id);
        }
        public void AddOrUpdateDevice(int id)
        {
            conn.Open();
            var command = new NpgsqlCommand("updateoradd_devicetype", conn);
            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@new_id", id);
                command.Parameters.AddWithValue("@new_name", typeBox.Text);
                command.ExecuteNonQuery();
                MessageBox.Show($@"Тип устройства {(id == 0 ? "добавлен" : "обновлен")}!");

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                conn.Close();
            }
            var deviceType = new DeviceType(conn);
            deviceType.FillGrid();
            this.Close();
        }
    }
}
