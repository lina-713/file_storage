using Npgsql;
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
    /// Логика взаимодействия для UpAddFileType.xaml
    /// </summary>
    public partial class UpAddFileType : Window
    {
        public readonly NpgsqlConnection conn;
        public readonly int key, id;
        public UpAddFileType(NpgsqlConnection conn, int key, int id)
        {
            InitializeComponent();
            this.conn = conn;
            this.id = id;
            this.key = key;
            if (id == 0)
            {
                this.Title = "Добавление типа файл";
            }
            else
            {
                this.Title = "Изменение типа файла";
                EnterUpdateType(id);
            }
        }

        private void Butt_Click(object sender, RoutedEventArgs e)
        {
            var files = new FilesType(conn);

            if (key == 1)
                AddTypeFile();
            else
                UpdateTypeFile(id);
            this.Close();
            files.FillGrid();
        }
        public void EnterUpdateType(int id)
        {
            conn.Open();
            string str = $"SELECT name_type, expansion, programs FROM typeoffiles WHERE id = {id}";
            var command = new NpgsqlCommand(str, conn);
            try
            {
                var reader = command.ExecuteReader();
                reader.Read();
                nameType.Text = reader.GetString(0);
                expansionType.Text = reader.GetString(1);
                programsType.Text = reader.GetString(2);
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
        public void AddTypeFile()
        {
            conn.Open();
            var command = new NpgsqlCommand("add_new_filetype", conn);
            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@new_name", nameType.Text);
                command.Parameters.AddWithValue("@new_expansion", expansionType.Text);
                command.Parameters.AddWithValue("@new_programs", programsType.Text);
                command.ExecuteNonQuery();
                MessageBox.Show("Тип файла добавлен!");
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
        public void UpdateTypeFile(int id)
        {
            conn.Open();
            var command = new NpgsqlCommand("update_filetype", conn);
            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@new_name", nameType.Text);
                command.Parameters.AddWithValue("@new_expansion", expansionType.Text);
                command.Parameters.AddWithValue("@new_programs", programsType.Text);
                command.Parameters.AddWithValue("@new_id", id);
                command.ExecuteNonQuery();
                MessageBox.Show("Тип файла обновлен!");
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
    }
}
