
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace proyecto_multidisciplinar.model
{
    //Hola soy Nacho
        public class Conexion
        {
            private NpgsqlConnection connection;

            public Conexion()
            {
            string connectionString = "Host=ep-solitary-hall-a2pwd4fg.eu-central-1.aws.neon.tech;" +
                          "Database=ProyectoMulti;" +
                          "Username=ProyectoMulti_owner;" +
                          "Password=Xmn5zOt1AZES;" +
                          "SSL Mode=Require;";
            connection = new NpgsqlConnection(connectionString);
            }

            public bool AbrirConexion()
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexión establecida con éxito");
                    return true;
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine($"Error al establecer la conexión con la base de datos: {ex.Message}");
                    return false;
                }
            }

            public void CerrarConexion()
            {
                try
                {
                    connection.Close();
                    Console.WriteLine("Conexión cerrada con éxito");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cerrar la conexión con la base de datos: {ex.Message}");
                }
            }

            public NpgsqlDataReader EjecutarConsulta(string query, params NpgsqlParameter[] parametros)
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                foreach (var parametro in parametros)
                {
                    command.Parameters.Add(parametro);
                }
                try
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    return reader;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error al ejecutar la consulta: " + e.Message);
                    throw;
                }
            }

            public void EjecutarNonQuery(string query, params NpgsqlParameter[] parametros)
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                foreach (var parametro in parametros)
                {
                    command.Parameters.Add(parametro);
                }
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Operación realizada con éxito");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error al ejecutar la operación: " + e.Message);
                }
            }
        }
    }

