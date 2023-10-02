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
using static kurs_rab.Devices;

namespace kurs_rab
{
    /// <summary>
    /// Логика взаимодействия для FilesType.xaml
    /// </summary>
    public partial class FilesType : Window
    {
        public readonly NpgsqlConnection conn;
        public FilesType(NpgsqlConnection conn)
        {
            this.conn = conn;
            InitializeComponent();
            InitializationGrid();
            if (conn.UserName == "guest")
            {
                AddButt.Visibility = Visibility.Hidden;
                DeleteButt.Visibility = Visibility.Hidden;
                UpdateButt.Visibility = Visibility.Hidden;
            }
        }

        private void AddButt_Click(object sender, RoutedEventArgs e)
        {
            int key = 1;
            var add = new UpAddFileType(conn, key, 0);
            add.Show();
            add.Closing += FillGridEvent;
            add.Show();
        }

        public void FillGrid()
        {
            string str = "Select * from view_typeoffiles";
            var typeoffiles = new List<View_typeoffiles>();
            var comm = new NpgsqlCommand(str, conn);
            conn.Open();
            var reader = comm.ExecuteReader();

            while (reader.Read())
            {
                var type = new View_typeoffiles()
                {
                    Id = Int32.Parse(reader["id"].ToString()),
                    NameType = reader["name_type"].ToString(),
                    Expansion = reader["expansion"].ToString(),
                    Programs = reader["programs"].ToString()
                };
                typeoffiles.Add(type);
            }
            filestype_grid.ItemsSource = typeoffiles;
            filestype_grid.Columns[0].Visibility = Visibility.Hidden;
            conn.Close();
        }

        public void InitializationGrid()
        {
            Type itemType = typeof(View_typeoffiles);

            var properties = itemType.GetProperties();

            foreach (var property in properties)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                DisplayAttribute displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                column.Header = displayAttribute != null ? displayAttribute.Name : property.Name;
                column.Binding = new Binding(property.Name);
                filestype_grid.Columns.Add(column);
            }

            filestype_grid.AutoGenerateColumns = false;

            FillGrid();
        }

        private void UpdateButt_Click(object sender, RoutedEventArgs e)
        {
            var filetype = filestype_grid.SelectedItem as View_typeoffiles;
            var id = filetype.Id;
            int key = 2;
            var update = new UpAddFileType(conn, key, id);
            update.Closing += FillGridEvent;
            update.Show();
        }
        public void FillGridEvent(object sender, CancelEventArgs e)
        {
            FillGrid();
        }

        private void DeleteButt_Click(object sender, RoutedEventArgs e)
        {
            var filetype = filestype_grid.SelectedItem as View_typeoffiles;
            var id = filetype.Id;

            var dialog = MessageBox.Show("Вы уверены, что хотите удалить этот тип файлов?", "Удаление", MessageBoxButton.YesNoCancel);
            if (dialog == MessageBoxResult.Yes)
            {
                conn.Open();
                var command = new NpgsqlCommand("delete_filetype", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@delete_id", id);
                command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Тип файлов удален!");
                FillGrid();
            }
        }
    }
    public class View_typeoffiles
    {
        public int Id { get; set; }
        [Display(Name = "Имя типа")]
        public string NameType { get; set; }
        [Display(Name = "Расширение")]
        public string Expansion { get; set; }
        [Display(Name = "Программы")]
        public string Programs { get; set; }
    }
}
