using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static proyecto_multidisciplinar.model.Conexion;

namespace proyecto_multidisciplinar.view;

public partial class ViewFtpUser : Window
{

    private List<Button> botones = new List<Button>();
    private string username;

    public ViewFtpUser(string username)
    {
        InitializeComponent();
        this.username = username;
        usernameLabel.Content = "Logged user:" + " " + username;

    }

    public void AccionArchivos(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear();
        FuncionPrincipal.Children.Clear();
        crearBotonesFichero();
        
        
        botones[0].Click += AccionSubirFichero;
        botones[1].Click += AccionDescargar;
        botones[2].Click += AccionEliminar;
        botones[3].Click += AccionConsultar;
        
    }



    public void AccionDirectorio(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear();
        FuncionPrincipal.Children.Clear();
    }


    public void AccionConsultas(object sender, RoutedEventArgs e)
    {
        BotonesFunciones.Children.Clear();
        FuncionPrincipal.Children.Clear();
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
    public void AccionSubirFichero(object sender, RoutedEventArgs e)
    {
        //FuncionPrincipal.Children.Clear();
        Label archivo = new Label()
        {
            Content = "Directorio",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            FontSize = 16,
            Margin = new Thickness(0, 10, 0, 10),// Separación alrededor del Label
            Foreground = new SolidColorBrush(Colors.Black) // Cambiado a negro
        };
        ComboBox listaArchivos = new ComboBox()
        {
            Margin = new Thickness(0, 10, 0, 10), // Separación alrededor del ComboBox
            Width = 200 // Establece un ancho para que sea visible
        };
        FuncionPrincipal.Children.Add(archivo);
        FuncionPrincipal.Children.Add(listaArchivos);

    }
    
    public void AccionDescargar(object sender, RoutedEventArgs e)
    {
        FuncionPrincipal.Children.Clear();

    }
    
    public void AccionEliminar(object sender, RoutedEventArgs e)
    {
        
    }

    public void AccionConsultar(object sender, RoutedEventArgs e)
    {
        FuncionPrincipal.Children.Clear();

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
        
        BotonesFunciones.Children.Add(botones[0]);
        BotonesFunciones.Children.Add(botones[1]);
        BotonesFunciones.Children.Add(botones[2]);
        BotonesFunciones.Children.Add(botones[3]);
    }
}