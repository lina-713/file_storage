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
using System.Xml.Linq;
using static kurs_rab.Devices;

namespace kurs_rab
{
    /// <summary>
    /// Логика взаимодействия для Files.xaml
    /// </summary>
    public partial class Files : Window
    {
        public readonly NpgsqlConnection conn;
        public Files(NpgsqlConnection npgsqlConnection)
        {
            InitializeComponent();
            conn = npgsqlConnection; 
            InitializationGrid();
            if (conn.UserName == "guest")
            {
                AddButt.Visibility = Visibility.Hidden;
                UpdateButt.Visibility = Visibility.Hidden;
                DeleteButt.Visibility = Visibility.Hidden;
            }
        }

        private void AddButt_Click(object sender, RoutedEventArgs e)
        {
            int key = 1;
            var addFiles = new AddFile(conn, key, 0);
            addFiles.Closing += FillGridEvent;
            addFiles.Show();
        }
        public void FillGridEvent(object sender, CancelEventArgs e)
        {
            FillGrid();
        }

        public void FillGrid()
        {
            string str = "Select * from view_files";
            var files = new List<View_Files>();
            var comm = new NpgsqlCommand(str, conn);
            conn.Open();
            var reader = comm.ExecuteReader();

            while (reader.Read())
            {
                var file = new View_Files()
                {
                    Id = Int32.Parse(reader["id"].ToString()),
                    Name = reader["name"].ToString(),
                    Size = Int32.Parse(reader["size"].ToString()),
                    Date = DateTime.Parse(reader["date"].ToString()),
                    Device = reader["devicename"].ToString(),
                    NameType = reader["name_type"].ToString(),
                    Expansion = reader["expansion"].ToString(),
                    Programs = reader["programs"].ToString()
                };
                files.Add(file);
            }
            filesGrid.ItemsSource = files;
            filesGrid.Columns[0].Visibility = Visibility.Hidden;
            conn.Close();
        }

        public void InitializationGrid()
        {
            Type itemType = typeof(View_Files);

            var properties = itemType.GetProperties();

            foreach (var property in properties)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                DisplayAttribute displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                column.Header = displayAttribute != null ? displayAttribute.Name : property.Name;
                column.Binding = new Binding(property.Name);
                filesGrid.Columns.Add(column);
            }

            filesGrid.AutoGenerateColumns = false;
            FillGrid();
        }

        private void UpdateButt_Click(object sender, RoutedEventArgs e)
        {
            var files = filesGrid.SelectedItem as View_Files;
            var id = files.Id;
            int key = 2;
            var update = new AddFile(conn, key, id);
            update.Closing += FillGridEvent;
            update.Show();
        }

        private void DeleteButt_Click(object sender, RoutedEventArgs e)
        {
            var file = filesGrid.SelectedItem as View_Files;
            var id = file.Id;

            var dialog = MessageBox.Show("Вы уверены, что хотите удалить этот файл?", "Удаление", MessageBoxButton.YesNoCancel);
            if (dialog == MessageBoxResult.Yes)
            {
                conn.Open();
                var command = new NpgsqlCommand("delete_files", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@delete_id", id);
                command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Файл удален!");
                FillGrid();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }
    }

    public class View_Files
    {
        public int Id { get; set; }
        [Display(Name = "Имя файла")]
        public string Name { get; set; }
        [Display(Name = "Размер")]
        public int Size { get; set; }
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        [Display(Name = "Название устройства")]
        public string Device { get; set; }
        [Display(Name = "Тип устройства")]
        public string NameType { get; set; }
        [Display(Name = "Расширение")]
        public string Expansion { get; set; }
        [Display(Name = "Программы")]
        public string Programs { get; set; }

    }
}
