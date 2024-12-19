using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Npgsql;
using proyecto_multidisciplinar.model;
using proyecto_multidisciplinar;
using static proyecto_multidisciplinar.model.Conexion;
using static proyecto_multidisciplinar.model.ControlFtp;


namespace proyecto_multidisciplinar.view;

public partial class ViewFtpUser : Window
{

    private List<Button> botonesFicheros = new List<Button>();
    private List<Button> botonesDirectorio = new List<Button>();

    private string username;

    private string FtpUrl = "ftp://185.27.134.11";
    private string FtpUser = "if0_37886491";
    private string FtpPass = "Sanjose2425";
    private string userDirectory;

    public ViewFtpUser(string username)
    {
        InitializeComponent();
        this.username = username;        
        userDirectory = $"/{username}";
        usernameLabel.Content = "Logged user:" + " " + username;
    }
    
    public void AccionArchivos(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear();
        Funcion.Children.Clear();
        crearBotonesFichero();
        
        botonesFicheros[0].Click += AccionSubirFichero;
        botonesFicheros[1].Click += AccionDescargar;
        botonesFicheros[2].Click += AccionEliminar;
        botonesFicheros[3].Click += AccionRenombrar;
    }



    public void AccionDirectorio(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear();
        Funcion.Children.Clear();
        crearBotonesDirectorio();
        
        botonesDirectorio[0].Click += AccionCrearDirectorio;
        botonesDirectorio[1].Click += AccionEliminarDirectorio;
        botonesDirectorio[2].Click += AccionAccesoCarpeta;
    }


    public void AccionConsultas(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear(); 
        Funcion.Children.Clear(); // Limpiar los controles de la interfaz si ya hay algo cargado

        // Crear un TreeView para mostrar la jerarquía
        TreeView jerarquiaTreeView = new TreeView()
        {
            Margin = new Thickness(10),
            Width = 400,
            Height = 300
        };

        Funcion.Children.Add(jerarquiaTreeView); // Agregar el TreeView directamente a la interfaz

        var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);

        try
        {
            // Crear el nodo raíz con el directorio inicial
            var raizNode = ftp.CrearNodoJerarquia(userDirectory);

            // Agregar el nodo raíz al TreeView
            jerarquiaTreeView.Items.Add(raizNode);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading hierarchy: {ex.Message}");
        }
    }


    public void AccionSalida(object sender, RoutedEventArgs e)
    {
        PrincipalMenuStandardUser menuusr = new PrincipalMenuStandardUser(username);
        this.Close();
        menuusr.Show();
    }
    
    /**
     * Accione botones ficheros
     */
    
    // Accion de subir ficheros
    private void AccionSubirFichero(object sender, RoutedEventArgs e)
    {
            Funcion.Children.Clear();

    Label directorioLabel = new Label()
    {
        Content = "FTP Directory:",
        Margin = new Thickness(10)
    };

    ComboBox directoriosComboBox = new ComboBox()
    {
        Width = 200,
        Margin = new Thickness(10)
    };

    // Llenar el ComboBox con los directorios disponibles en el FTP
    var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
    var directorios = ftp.ListDirectories(userDirectory);

    foreach (var dir in directorios)
    {
        directoriosComboBox.Items.Add(dir);
    }

    Label ficheroLabel = new Label()
    {
        Content = "File to Upload:",
        Margin = new Thickness(10)
    };

    Button seleccionarArchivoButton = new Button()
    {
        Content = "Select File",
        Width = 150,
        Margin = new Thickness(10)
    };

    TextBlock archivoSeleccionado = new TextBlock()
    {
        Text = "No file selected",
        Margin = new Thickness(10)
    };

    // Evento para abrir el cuadro de diálogo de selección de archivo
    seleccionarArchivoButton.Click += (s, ev) =>
    {
        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Select file to upload",
            Filter = "All files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            archivoSeleccionado.Text = openFileDialog.FileName;
        }
    };

    Button subirButton = new Button()
    {
        Content = "Upload File",
        Width = 150,
        Margin = new Thickness(10)
    };

    // Evento para subir el archivo al FTP
    subirButton.Click += (s, ev) =>
    {
        string selectedDirectory = directoriosComboBox.SelectedItem?.ToString();
        string localFilePath = archivoSeleccionado.Text;

        if (!string.IsNullOrEmpty(selectedDirectory) && File.Exists(localFilePath))
        {
            try
            {
                ftp.UploadFile(localFilePath, $"{selectedDirectory}/{Path.GetFileName(localFilePath)}");
                MessageBox.Show("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading file: {ex.Message}");
            }
        }
        else
        {
            MessageBox.Show("Please select a valid directory and file.");
        }
    };

    // Agregar controles a la interfaz
    Funcion.Children.Add(directorioLabel);
    Funcion.Children.Add(directoriosComboBox);
    Funcion.Children.Add(ficheroLabel);
    Funcion.Children.Add(seleccionarArchivoButton);
    Funcion.Children.Add(archivoSeleccionado);
    Funcion.Children.Add(subirButton);
    }
    
    // Accion de descargar ficheros
    public void AccionDescargar(object sender, RoutedEventArgs e)
    {
        Funcion.Children.Clear();

        // Crear controles dinámicos para la funcionalidad
        Label archivoLabel = new Label()
        {
            Content = "File to Download:",
            Margin = new Thickness(10)
        };

        ComboBox archivosComboBox = new ComboBox()
        {
            Width = 200,
            Margin = new Thickness(10)
        };

        // Llenar el ComboBox con los archivos disponibles
        var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
        var archivos = ftp.ListFiles(userDirectory);
        foreach (var archivo in archivos)
        {
            archivosComboBox.Items.Add(archivo);
        }

        Button descargarButton = new Button()
        {
            Content = "Download File",
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
                    Filter = "All files |*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string localPath = saveFileDialog.FileName;
                    ftp.DownloadFile(selectedArchivo, localPath);
                    MessageBox.Show("File downloaded successfully.");
                }
            }
            else
            {
                MessageBox.Show("Please select a file.");
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
        Funcion.Children.Clear();

        Label archivoLabel = new Label()
        {
            Content = "File to Delete:",
            Margin = new Thickness(10)
        };

        ComboBox archivosComboBox = new ComboBox()
        {
            Width = 200,
            Margin = new Thickness(10)
        };

        // Llenar el ComboBox con los archivos disponibles
        var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
        var archivos = ftp.ListFiles(userDirectory);
        foreach (var archivo in archivos)
        {
            archivosComboBox.Items.Add(archivo);
        }

        Button eliminarButton = new Button()
        {
            Content = "Delete File",
            Width = 100,
            Margin = new Thickness(10)
        };

        eliminarButton.Click += (s, ev) =>
        {
            string selectedArchivo = archivosComboBox.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedArchivo))
            {
                ftp.DeleteFile(selectedArchivo);
                MessageBox.Show("File deleted successfully.");
            }
            else
            {
                MessageBox.Show("Please select a file.");
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
        Content = "File to Rename:",
        Margin = new Thickness(10)
    };

    ComboBox archivosComboBox = new ComboBox()
    {
        Width = 200,
        Margin = new Thickness(10)
    };

    // Llenar el ComboBox con los archivos disponibles
    var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
    var archivos = ftp.ListFiles(userDirectory); // Aquí usamos ListFiles en vez de ListDirectories para obtener archivos
    
    foreach (var archivo in archivos)
    {
        archivosComboBox.Items.Add(archivo);
    }

    // Crear el campo de texto para el nuevo nombre del archivo
    Label nuevoNombreLabel = new Label()
    {
        Content = "New Name:",
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
        Content = "Rename File",
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
                MessageBox.Show("File renamed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error renaming file: {ex.Message}");
            }
        }
        else
        {
            MessageBox.Show("Please select a file and provide a new name.");
        }
    };

    // Agregar los controles a la interfaz
    Funcion.Children.Add(archivoLabel);
    Funcion.Children.Add(archivosComboBox);
    Funcion.Children.Add(nuevoNombreLabel);
    Funcion.Children.Add(nuevoNombreTextBox);
    Funcion.Children.Add(renombrarButton);
}


    /**
     * Funciones de los botones de Directorio
     */

    // Accion crear directorio 
    public void AccionCrearDirectorio(object sender, RoutedEventArgs e)
    {
        // Limpiar los controles de la interfaz si ya hay algo cargado
    Funcion.Children.Clear();

    // Crear controles dinámicos para la funcionalidad de crear carpeta
    Label rutaLabel = new Label()
    {
        Content = "Select Route:",
        Margin = new Thickness(10)
    };

    ComboBox rutaComboBox = new ComboBox()
    {
        Width = 200,
        Margin = new Thickness(10)
    };

    // Llenar el ComboBox con las rutas disponibles
    var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
    var rutas = ftp.ListDirectories(userDirectory); // Usamos ListDirectories para obtener las rutas

    foreach (var ruta in rutas)
    {
        rutaComboBox.Items.Add(ruta);
    }

    // Crear el campo de texto para el nuevo nombre de la carpeta
    Label nuevoNombreLabel = new Label()
    {
        Content = "New Folder Name:",
        Margin = new Thickness(10)
    };

    TextBox nuevoNombreTextBox = new TextBox()
    {
        Width = 200,
        Margin = new Thickness(10)
    };

    // Crear el botón de "Crear Carpeta"
    Button crearCarpetaButton = new Button()
    {
        Content = "Create Folder",
        Width = 150,
        Margin = new Thickness(10)
    };

    // Lógica para crear la carpeta cuando se hace clic en el botón
    crearCarpetaButton.Click += (s, ev) =>
    {
        string rutaSeleccionada = rutaComboBox.SelectedItem?.ToString();
        string nuevaCarpeta = nuevoNombreTextBox.Text;

        if (!string.IsNullOrEmpty(rutaSeleccionada) && !string.IsNullOrEmpty(nuevaCarpeta))
        {
            try
            {
                // Crear la carpeta en el servidor FTP
                string nuevaRuta = Path.Combine(rutaSeleccionada, nuevaCarpeta);
                ftp.CreateDirectory(nuevaRuta);
                MessageBox.Show("Folder created successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating folder: {ex.Message}");
            }
        }
        else if (!string.IsNullOrEmpty(rutaSeleccionada))
        {
            try
            {
                // Crear la carpeta en el servidor FTP
                rutaSeleccionada = userDirectory;
                string nuevaRuta = Path.Combine(rutaSeleccionada, nuevaCarpeta);
                ftp.CreateDirectory(nuevaRuta);
                MessageBox.Show("Folder created successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating folder: {ex.Message}");
            }
        }
        else
        {
            MessageBox.Show("Please select a path and provide a name for the folder.");

        }
    };

        // Agregar los controles a la interfaz
        Funcion.Children.Add(rutaLabel);
        Funcion.Children.Add(rutaComboBox);
        Funcion.Children.Add(nuevoNombreLabel);
        Funcion.Children.Add(nuevoNombreTextBox);
        Funcion.Children.Add(crearCarpetaButton);
    }
    
    // Accion eleminar directorio 
    public void AccionEliminarDirectorio(object sender, RoutedEventArgs e)
{
    // Limpiar los controles de la interfaz si ya hay algo cargado
    Funcion.Children.Clear();

    // Crear controles dinámicos para la funcionalidad de eliminar carpeta

    // Etiqueta para seleccionar la ruta base
    Label rutaLabel = new Label()
    {
        Content = "Select Main Route:",
        Margin = new Thickness(10)
    };

    ComboBox rutaComboBox = new ComboBox()
    {
        Width = 200,
        Margin = new Thickness(10)
    };

    // Etiqueta para seleccionar la carpeta dentro de la ruta
    Label carpetaLabel = new Label()
    {
        Content = "Select Folder to Delete:",
        Margin = new Thickness(10)
    };

    ComboBox carpetaComboBox = new ComboBox()
    {
        Width = 200,
        Margin = new Thickness(10)
    };

    // Botón para eliminar la carpeta
    Button eliminarCarpetaButton = new Button()
    {
        Content = "Delete Folder",
        Width = 150,
        Margin = new Thickness(10)
    };

    // Llenar el ComboBox de rutas base con los directorios disponibles
    var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
    var rutasBase = ftp.ListDirectories(userDirectory); // Directorios base

    foreach (var ruta in rutasBase)
    {
        rutaComboBox.Items.Add(ruta);
    }

    // Lógica para actualizar las carpetas al seleccionar una ruta base
    rutaComboBox.SelectionChanged += (s, ev) =>
    {
        carpetaComboBox.Items.Clear(); // Limpiar las carpetas previas

        string rutaSeleccionada = rutaComboBox.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(rutaSeleccionada))
        {
            var carpetas = ftp.ListDirectories(rutaSeleccionada); // Obtener carpetas de esa ruta

            foreach (var carpeta in carpetas)
            {
                carpetaComboBox.Items.Add(carpeta);
            }
        }
    };

    // Lógica para eliminar la carpeta seleccionada
    eliminarCarpetaButton.Click += (s, ev) =>
    {
        string rutaSeleccionada = rutaComboBox.SelectedItem?.ToString();
        string carpetaSeleccionada = carpetaComboBox.SelectedItem?.ToString();

        if (!string.IsNullOrEmpty(rutaSeleccionada) && !string.IsNullOrEmpty(carpetaSeleccionada))
        {
            try
            {
                // Construir la ruta completa de la carpeta
                string carpetaCompleta = Path.Combine(rutaSeleccionada, carpetaSeleccionada);

                // Verificar si la carpeta está vacía
                var archivosEnCarpeta = ftp.ListFiles(carpetaCompleta);

                if (archivosEnCarpeta.Count > 0)
                {
                    MessageBox.Show($"Cannot delete folder'{carpetaSeleccionada}' because it is not empty.");
                    return;
                }

                // Eliminar la carpeta en el servidor FTP
                ftp.DeleteDirectory(carpetaCompleta);
                MessageBox.Show("Folder deleted successfully.");

                // Actualizar el ComboBox de carpetas eliminando la carpeta eliminada
                carpetaComboBox.Items.Remove(carpetaSeleccionada);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting folder: {ex.Message}");
            }
        }
        else
        {
            MessageBox.Show("Please select a path and folder to delete.");
        }
    };

    // Agregar los controles a la interfaz
    Funcion.Children.Add(rutaLabel);
    Funcion.Children.Add(rutaComboBox);
    Funcion.Children.Add(carpetaLabel);
    Funcion.Children.Add(carpetaComboBox);
    Funcion.Children.Add(eliminarCarpetaButton);
}
    
    
    // Accion acceso
public void AccionAccesoCarpeta(object sender, RoutedEventArgs e)
{
    // Limpiar los controles de la interfaz si ya hay algo cargado
    Funcion.Children.Clear();

    // Etiqueta para seleccionar la ruta base
    Label rutaLabel = new Label()
    {
        Content = "Select Main Route:",
        Margin = new Thickness(10)
    };

    ComboBox rutaComboBox = new ComboBox()
    {
        Width = 200,
        Margin = new Thickness(10)
    };

    // Etiqueta para mostrar los permisos
    Label permisosLabel = new Label()
    {
        Content = "Folder Permissions:",
        Margin = new Thickness(10)
    };

    ListBox permisosListBox = new ListBox()
    {
        Width = 300,
        Height = 150,
        Margin = new Thickness(10)
    };

    // Botón para consultar los permisos
    Button consultarPermisosButton = new Button()
    {
        Content = "Check Permissions",
        Width = 150,
        Margin = new Thickness(10)
    };

    // Llenar el ComboBox de rutas base con los directorios disponibles
    var ftp = new ControlFtp(FtpUrl, FtpUser, FtpPass);
    var rutasBase = ftp.ListDirectories("/"); // Directorios base

    foreach (var ruta in rutasBase)
    {
        rutaComboBox.Items.Add(ruta);
    }

    // Lógica para consultar los permisos al hacer clic en el botón
    consultarPermisosButton.Click += (s, ev) =>
    {
        try
        {
            string rutaBase = rutaComboBox.SelectedItem?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(rutaBase))
            {
                MessageBox.Show("Please select a route.");
                return;
            }

            if (rutaBase != username)
            {
                permisosListBox.Items.Add($"File: {rutaBase} - You do not have permissions on the folder.");
            }
            else
            {
                // Agregar los permisos al ListBox para mostrar
                permisosListBox.Items.Add($"File: {rutaBase} - you have permissions on the folder.");
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error checking permissions: {ex.Message}");
        }
    };

    // Agregar los controles a la interfaz
    Funcion.Children.Add(rutaLabel);
    Funcion.Children.Add(rutaComboBox);
    Funcion.Children.Add(permisosLabel);
    Funcion.Children.Add(permisosListBox);
    Funcion.Children.Add(consultarPermisosButton);
}

    
    
/**
 * Funcion para crear los botones de archivos
 */
    
    private void crearBotonesFichero()
    {
        botonesFicheros.Add(new Button()
        {
            Content = "Upload file",
            Width = 100,
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
        });
        botonesFicheros.Add(new Button()
        {
            Content = "Download files",
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
        });
        botonesFicheros.Add(new Button()
        {
            Content = "Delete files",
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
        });
        botonesFicheros.Add(new Button()
        {
            Content = "Rename",
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
        });
        

        
        BotonesFunciones.Children.Add(botonesFicheros[0]);
        BotonesFunciones.Children.Add(botonesFicheros[1]);
        BotonesFunciones.Children.Add(botonesFicheros[2]);
        BotonesFunciones.Children.Add(botonesFicheros[3]);
    }

    /**
     * Funcion para crear los botones de Directorio
     */
    
    private void crearBotonesDirectorio()
    {
        botonesDirectorio.Add(new Button()
       {
            Content = "Create Directory",
            Width = 100,
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
       });
        botonesDirectorio.Add(new Button()
       {
            Content = "Delete Directory",
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
       });
        botonesDirectorio.Add(new Button()
        {
            Content = "Manage access",
            Margin = new Thickness(10, 0, 10, 0),
            Background = new SolidColorBrush(Color.FromRgb(250, 214, 165)), // "#FAD6A5" convertido a RGB
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            BorderThickness = new Thickness(0),
        });
        
        
            
            BotonesFunciones.Children.Add(botonesDirectorio[0]);
            BotonesFunciones.Children.Add(botonesDirectorio[1]);
            BotonesFunciones.Children.Add(botonesDirectorio[2]);

    }
}