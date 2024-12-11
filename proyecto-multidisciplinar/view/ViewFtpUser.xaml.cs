using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FTPHelper;
using proyecto_multidisciplinar.model;
using static proyecto_multidisciplinar.model.Conexion;
using static proyecto_multidisciplinar.model.ControlFtp;

namespace proyecto_multidisciplinar.view;

public partial class ViewFtpUser : Window
{

    private List<Button> botones = new List<Button>();
    private string username;

    private string FtpUrl = "ftp://185.27.134.11";
    private string FtpUser = "if0_37886491";
    private string FtpPass = "Sanjose2425";

    public ViewFtpUser(string username)
    {
        InitializeComponent();
        this.username = username;
        usernameLabel.Content = "Logged user:" + " " + username;

    }

    public void AccionArchivos(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear();
        Funcion.Children.Clear();
        crearBotonesFichero();
    }



    public void AccionDirectorio(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear();
    }


    public void AccionConsultas(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear(); 
    }

    public void AccionSalida(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        this.Close();
        mainWindow.Show();

    }
    
    /**
     * Accione botones
     */
    
    // Accion de subir ficheros
    private void AccionSubirFichero(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Botón de subir archivo clickeado.");  // Verificar si el botón es presionado
        Funcion.Children.Clear();

        Label directorioLabel = new Label()
        {
            Content = "Directorio:",
            Margin = new Thickness(10)
        };

        ComboBox directoriosComboBox = new ComboBox()
        {
            Width = 200,
            Margin = new Thickness(10)
        };

        // Llenar el ComboBox con los directorios disponibles
        var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
        var directorios = ftp.ListDirectories("/");
        
        foreach (var dir in directorios)
        {
            directoriosComboBox.Items.Add(dir);
        }

        Label ficheroLabel = new Label()
        {
            Content = "Archivo:",
            Margin = new Thickness(10)
        };

        TextBox ficheroTextBox = new TextBox()
        {
            Width = 200,
            Margin = new Thickness(10)
        };

        Button subirButton = new Button()
        {
            Content = "Subir Archivo",
            Width = 100,
            Margin = new Thickness(10)
        };

        subirButton.Click += (s, ev) =>
        {
            string selectedDirectory = directoriosComboBox.SelectedItem?.ToString();
            string localFilePath = ficheroTextBox.Text;

            if (!string.IsNullOrEmpty(selectedDirectory) && File.Exists(localFilePath))
            {
                ftp.UploadFile(localFilePath, $"{selectedDirectory}/{Path.GetFileName(localFilePath)}");
                MessageBox.Show("Archivo subido correctamente.");
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un directorio y un archivo válido.");
            }
        };

        Funcion.Children.Add(directorioLabel);
        Funcion.Children.Add(directoriosComboBox);
        Funcion.Children.Add(ficheroLabel);
        Funcion.Children.Add(ficheroTextBox);
        Funcion.Children.Add(subirButton);
    }
    
    // Accion de descargar ficheros
    public void AccionDescargar(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Botón de subir archivo clickeado.");  // Verificar si el botón es presionado
        Funcion.Children.Clear();

        // Crear controles dinámicos para la funcionalidad
        Label archivoLabel = new Label()
        {
            Content = "Archivo a Descargar:",
            Margin = new Thickness(10)
        };

        ComboBox archivosComboBox = new ComboBox()
        {
            Width = 200,
            Margin = new Thickness(10)
        };

        // Llenar el ComboBox con los archivos disponibles
        var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
        var archivos = ftp.ListDirectories("/");
        foreach (var archivo in archivos)
        {
            archivosComboBox.Items.Add(archivo);
        }

        Button descargarButton = new Button()
        {
            Content = "Descargar Archivo",
            Width = 150,
            Margin = new Thickness(10)
        };

        descargarButton.Click += (s, ev) =>
        {
            string selectedArchivo = archivosComboBox.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedArchivo))
            {
                // Mostrar diálogo para seleccionar ubicación local
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = selectedArchivo,
                    Filter = "Todos los archivos|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string localPath = saveFileDialog.FileName;
                    ftp.DownloadFile(selectedArchivo, localPath);
                    MessageBox.Show("Archivo descargado correctamente.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un archivo.");
            }
        };

        // Agregar controles a la interfaz
        Funcion.Children.Add(archivoLabel);
        Funcion.Children.Add(archivosComboBox);
        Funcion.Children.Add(descargarButton);

    }
    
    
    // Accion de eliminar fichero
    public void AccionEliminar(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Botón de subir archivo clickeado.");  // Verificar si el botón es presionado
        Funcion.Children.Clear();

        Label archivoLabel = new Label()
        {
            Content = "Archivo a Eliminar:",
            Margin = new Thickness(10)
        };

        ComboBox archivosComboBox = new ComboBox()
        {
            Width = 200,
            Margin = new Thickness(10)
        };

        // Llenar el ComboBox con los archivos disponibles
        var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
        var archivos = ftp.ListDirectories("/");
        foreach (var archivo in archivos)
        {
            archivosComboBox.Items.Add(archivo);
        }

        Button eliminarButton = new Button()
        {
            Content = "Eliminar Archivo",
            Width = 100,
            Margin = new Thickness(10)
        };

        eliminarButton.Click += (s, ev) =>
        {
            string selectedArchivo = archivosComboBox.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedArchivo))
            {
                ftp.DeleteFile(selectedArchivo);
                MessageBox.Show("Archivo eliminado correctamente.");
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un archivo.");
            }
        };

        Funcion.Children.Add(archivoLabel);
        Funcion.Children.Add(archivosComboBox);
        Funcion.Children.Add(eliminarButton);
    }
    
    public void AccionRenombrar(object sender, RoutedEventArgs e)
{
    // Limpiar los controles de la interfaz si ya hay algo cargado
    Funcion.Children.Clear();

    // Crear controles dinámicos para la funcionalidad de renombrar
    Label archivoLabel = new Label()
    {
        Content = "Archivo a Renombrar:",
        Margin = new Thickness(10)
    };

    ComboBox archivosComboBox = new ComboBox()
    {
        Width = 200,
        Margin = new Thickness(10)
    };

    // Llenar el ComboBox con los archivos disponibles
    var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
    var archivos = ftp.ListFiles("/"); // Aquí usamos ListFiles en vez de ListDirectories para obtener archivos
    foreach (var archivo in archivos)
    {
        archivosComboBox.Items.Add(archivo);
    }

    // Crear el campo de texto para el nuevo nombre del archivo
    Label nuevoNombreLabel = new Label()
    {
        Content = "Nuevo Nombre:",
        Margin = new Thickness(10)
    };

    TextBox nuevoNombreTextBox = new TextBox()
    {
        Width = 200,
        Margin = new Thickness(10)
    };

    // Crear el botón de "Renombrar"
    Button renombrarButton = new Button()
    {
        Content = "Renombrar Archivo",
        Width = 150,
        Margin = new Thickness(10)
    };

    // Lógica para renombrar el archivo cuando se hace clic en el botón
    renombrarButton.Click += (s, ev) =>
    {
        string selectedArchivo = archivosComboBox.SelectedItem?.ToString();
        string nuevoNombre = nuevoNombreTextBox.Text;

        if (!string.IsNullOrEmpty(selectedArchivo) && !string.IsNullOrEmpty(nuevoNombre))
        {
            try
            {
                // Renombrar el archivo en el servidor FTP
                string nuevoArchivoPath = Path.Combine(Path.GetDirectoryName(selectedArchivo), nuevoNombre);
                ftp.RenameFile(selectedArchivo, nuevoArchivoPath);
                MessageBox.Show("Archivo renombrado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al renombrar el archivo: {ex.Message}");
            }
        }
        else
        {
            MessageBox.Show("Por favor, selecciona un archivo y proporciona un nuevo nombre.");
        }
    };

    // Agregar los controles a la interfaz
    Funcion.Children.Add(archivoLabel);
    Funcion.Children.Add(archivosComboBox);
    Funcion.Children.Add(nuevoNombreLabel);
    Funcion.Children.Add(nuevoNombreTextBox);
    Funcion.Children.Add(renombrarButton);
}

    public void AccionConsultar(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear();

    }

    
/**
 * Funcion para crear los botones de archivos
 */
    
    private void crearBotonesFichero()
    {
       botones.Add(new Button()
        {
            Content = "Subir archivo",
            Width = 100,
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
        });
        botones.Add(new Button()
        {
            Content = "Descargar archivos",
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
        });
        botones.Add(new Button()
        {
            Content = "Eliminar archivos",
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
        });
        botones.Add(new Button()
        {
            Content = "Renombrar",
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
        });
        
        botones[0].Click += AccionSubirFichero;
        botones[1].Click += AccionDescargar;
        botones[2].Click += AccionEliminar;
        botones[3].Click += AccionRenombrar;
        
        BotonesFunciones.Children.Add(botones[0]);
        BotonesFunciones.Children.Add(botones[1]);
        BotonesFunciones.Children.Add(botones[2]);
        BotonesFunciones.Children.Add(botones[3]);
    }
}