using Npgsql;
using proyecto_multidisciplinar.view;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace proyecto_multidisciplinar.model
{
    class Logs
    {
        public int id {  get; set; }
        public string? username { get; set; }
        public string? action { get; set; }
        public DateTime date { get; set; }
        public string? ip { get; set; }
        public string? user_email { get; set; }

        public Logs(int id, string? username, string? action, DateTime date, string? ip, string? user_email)
        {
            this.id = id;
            this.username = username;
            this.action = action;
            this.date = date;
            this.ip = ip;
            this.user_email = user_email;
        }

        public Logs() { }

        public static void CreateTableLogs(StackPanel mainPanel)
        {
            List<Logs> logs = GetLogs();

            DataTable dataTable = new DataTable("User logs");

            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("USERNAME", typeof(string));
            dataTable.Columns.Add("ACTION", typeof(string));
            dataTable.Columns.Add("DATE", typeof(DateTime));
            dataTable.Columns.Add("IP", typeof(string));
            dataTable.Columns.Add("EMAIL", typeof(string));

            foreach (var log in logs)
            {
                dataTable.Rows.Add(log.id, log.username, log.action, log.date, log.ip, log.user_email);  
            }

            DataGrid dataGrid = new DataGrid
            {
                AutoGenerateColumns = true,
                ItemsSource = dataTable.DefaultView,
                Height = 300,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(10),
                FontSize= 14,
            };

            ScrollViewer scrollViewer = new ScrollViewer()
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = dataGrid,
            };

            mainPanel.Children.Add(scrollViewer);
            
        }

        public static void CreateButtons(StackPanel buttonsPanel, Action onBackButtonClick)
        {
            Button buttonBack = new Button()
            {
                FontSize = 16,
                BorderThickness = new Thickness(1),
                Content = "Exit",
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = 100,
                Height = 40,
                Margin=new Thickness(10),
                Background = new SolidColorBrush(Colors.LightSalmon),

            };
            buttonBack.Click += (sender, e) =>
            {
                onBackButtonClick.Invoke();
                

            };

            buttonsPanel.Children.Add(buttonBack);

        }

        public static List<Logs> GetLogs()
        {
            var logsList = new List<Logs>();

            Conexion conexion = new Conexion();

            if (conexion.AbrirConexion())
            {
                string query = "SELECT * FROM \"Logs\" ";

                NpgsqlDataReader reader = conexion.EjecutarConsulta(query);

                while (reader.Read())
                {
                    Logs log = new Logs
                    {
                        id = reader.GetInt32(0),
                        username = reader.GetString(1),
                        action = reader.GetString(2),
                        date = reader.GetDateTime(3),
                        ip = reader.GetString(4),
                        user_email = reader.GetString(5),
                    };
                    logsList.Add(log);
                }
            }

            return logsList;
        }

        public static void InsertLogs(string? username, string? action, DateTime date, string? ip, string? email)
        {
            Conexion conexion = new Conexion();
            

            if (conexion.AbrirConexion())
            {

                string query = "INSERT INTO \"Logs\" (username, action, date, user_ip, user_email) VALUES (@username, @action, @date, @ip, @email)";

                NpgsqlParameter[] parameters = new NpgsqlParameter[] {
                new NpgsqlParameter("@username", username),
                new NpgsqlParameter("@action", action),
                new NpgsqlParameter("@date", date),
                new NpgsqlParameter("@ip", ip),
                new NpgsqlParameter("@email", email),
                
        };
                try
                {
                    conexion.EjecutarNonQuery(query, parameters);
                }
                catch (Exception ex) {
                    MessageBox.Show("Error inserting user log" + ex);

                }
            
            }
            else
            {
                MessageBox.Show("Error connecting to database");

            }

            conexion.CerrarConexion();
        }
    }
}
