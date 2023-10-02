using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
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
    /// Логика взаимодействия для UpAddDevice.xaml
    /// </summary>
    public partial class UpAddDevice : Window
    {
        public readonly NpgsqlConnection conn;
        public readonly int key, id;
        public UpAddDevice(NpgsqlConnection npgsqlConnection, int key, int id)
        {
            InitializeComponent();
            conn = npgsqlConnection;
            this.key = key;
            this.id = id;
            TypeOfDeviceDictionary();
            if (id == 0)
            {
                this.Title= "Добавление устройства";
                butt.Content = "Добавить";
            }
            else
            {
                this.Title = "Изменение устройства";
                butt.Content = "Изменить";
                EnterUpdate(id);
            }
        }
        public void EnterUpdate(int id)
        {
            conn.Open();
            string str = $"SELECT name, memory_size, type_id FROM device WHERE id = {id}";
            var command = new NpgsqlCommand(str, conn);
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                reader.Read();
                nameDevice.Text = reader.GetString(0);
                memory.Text = reader.GetInt32(1).ToString();
                comboDevice.SelectedValue = reader.GetInt32(2);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (id == 0)
            {
                AddDevice();
            }
            else
            {
                UpdateDevice(id);
            }
        }

        public void AddDevice()
        {
            conn.Open();
            var command = new NpgsqlCommand("add_new_device",conn);
            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@new_name", nameDevice.Text);
                command.Parameters.AddWithValue("@new_memory_size", Int32.Parse(memory.Text));
                command.Parameters.AddWithValue("@new_type_id", comboDevice.SelectedValue);
                command.ExecuteNonQuery();
                MessageBox.Show("Устройство добавлено!");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                conn.Close();
            }
            var devices = new Devices(conn);
            devices.FillGrid();
            this.Close();
        }

        public void UpdateDevice(int id)
        {
            conn.Open();
            var command = new NpgsqlCommand("update_device", conn);
            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@new_name", nameDevice.Text);
                command.Parameters.AddWithValue("@new_memory_size", Convert.ToInt32(memory.Text));
                command.Parameters.AddWithValue("@new_type_id", comboDevice.SelectedValue);
                command.Parameters.AddWithValue("@new_id", id);
                command.ExecuteNonQuery();
                MessageBox.Show("Устройство обновлено!");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                conn.Close();
            }
            var devices = new Devices(conn);
            devices.FillGrid();
            this.Close();
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
    }
}
