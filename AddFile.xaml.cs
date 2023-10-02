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
    /// Логика взаимодействия для AddFile.xaml
    /// </summary>
    public partial class AddFile : Window
    {
        public readonly NpgsqlConnection conn;
        public readonly int key, id;
        public AddFile(NpgsqlConnection conn, int key, int id)
        {
            this.conn = conn;
            this.key = key;
            this.id = id;
            InitializeComponent();
            TypeOfFilesDictionary();
            DeviceDictionary();
            if (id == 0)
            {
                this.Title = "Добавление файл";
            }
            else
            {
                this.Title = "Изменение файла";
                EnterUpdate(id);
            }
        }

        public void EnterUpdate(int id)
        {
            conn.Open();
            string str = $"SELECT name, size, type_id, device_id  FROM files WHERE id = {id}";
            var command = new NpgsqlCommand(str, conn);
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                reader.Read();
                nameFile.Text = reader.GetString(0);
                fileSize.Text = reader.GetInt32(1).ToString();
                combotype.SelectedValue = reader.GetInt32(2);
                filedevice.SelectedValue = reader.GetInt32(3);
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

        private void Butt_Click(object sender, RoutedEventArgs e)
        {
            if (key == 1)
                AddFiles();
            else
                UpdateFiles(id);
        }

        public void AddFiles()
        {
            conn.Open();
            var command = new NpgsqlCommand("add_new_file", conn);
            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@new_name", nameFile.Text);
                command.Parameters.AddWithValue("@new_size", Convert.ToInt32(fileSize.Text));
                command.Parameters.AddWithValue("@type_id", combotype.SelectedValue);
                command.Parameters.AddWithValue("@new_date", DateTime.Now);
                command.Parameters.AddWithValue("@new_device_id", filedevice.SelectedValue);
                command.ExecuteNonQuery();
                MessageBox.Show("Файл добавлен!");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                conn.Close();
            }
            var files = new Files(conn);
            files.FillGrid();
            this.Close();
        }

        public void UpdateFiles(int id)
        {
            conn.Open();
            var command = new NpgsqlCommand("update_file", conn);
            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@new_name", nameFile.Text);
                command.Parameters.AddWithValue("@new_size", Convert.ToInt32(fileSize.Text));
                command.Parameters.AddWithValue("@new_type_id", combotype.SelectedValue);
                command.Parameters.AddWithValue("@update_id", id);
                command.Parameters.AddWithValue("@new_device_id", filedevice.SelectedValue);
                command.ExecuteNonQuery();
                MessageBox.Show("Файл обновлен!");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                conn.Close();
            }
            var files = new Files(conn);
            files.FillGrid();
            this.Close();
        }

        public void TypeOfFilesDictionary()
        {
            combotype.Items.Clear();
            string str = "SELECT id, name_type FROM typeoffiles";
            var da = new NpgsqlDataAdapter(str, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "typeoffiles");
            combotype.ItemsSource = ds.Tables[0].DefaultView;
            combotype.DisplayMemberPath = ds.Tables[0].Columns["name_type"].ToString();
            combotype.SelectedValuePath = ds.Tables[0].Columns["id"].ToString();
        }

        public void DeviceDictionary()
        {
            filedevice.Items.Clear();
            string str = "SELECT id, name FROM device";
            var da = new NpgsqlDataAdapter(str, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "device");
            filedevice.ItemsSource = ds.Tables[0].DefaultView;
            filedevice.DisplayMemberPath = ds.Tables[0].Columns["name"].ToString();
            filedevice.SelectedValuePath = ds.Tables[0].Columns["id"].ToString();
        }
    }
}
