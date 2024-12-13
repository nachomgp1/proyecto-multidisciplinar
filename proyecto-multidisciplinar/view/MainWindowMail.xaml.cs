using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace proyecto_multidisciplinar.view
{
    public partial class MainWindowMail : Window
    {
        private string? username;

        private const string ApplicationName = "Gmail Login WPF App";
        private static string[] Scopes = { GmailService.Scope.GmailReadonly , GmailService.Scope.GmailSend, GmailService.Scope.MailGoogleCom};
        private UserCredential credential;

        public MainWindowMail(string? username)
        {
            this.username = username;
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
           
            int typeUser = MainWindow.ObtainUserPermissions(username);

            if (typeUser == 3)
            {
                PrincipalMenuAdmin viewAdmid = new PrincipalMenuAdmin(username);
                viewAdmid.Show();
            }
            else if (typeUser == 1)
            {
                view.PrincipalMenuStandardUser viewPrincipalMenuUser = new view.PrincipalMenuStandardUser(username);
                viewPrincipalMenuUser.Show();
                
            }
            else if (typeUser == 2)
            {
                view.PrincipalMenuGroupUser viewPrincipalGroupUser = new view.PrincipalMenuGroupUser(username);
                viewPrincipalGroupUser.Show();
               
            }
            this.Close();
        }

        private async void GoogleLoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../resources/credentials.json");

                using (var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read))
                {
                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credentials/gmail-wpf-quickstart");

                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Scopes,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(folderPath, true));
                }

                // Crear servicio de Gmail
                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Obtener el correo del usuario autenticado
                var userProfile = await service.Users.GetProfile("me").ExecuteAsync();
                string authenticatedEmail = userProfile.EmailAddress;

                // Obtener el correo asociado al usuario desde la base de datos
                string expectedEmail = MainWindow.GetEmail(username);

                // Comparar los correos
                if (authenticatedEmail.Equals(expectedEmail, StringComparison.OrdinalIgnoreCase))
                {
                    // Abrir ventana de correos si los correos coinciden
                    EmailsWindow emailsWindow = new EmailsWindow(service, credential, this);
                    emailsWindow.Show();
                    this.Hide();
                }
                else
                {
                    // Mostrar error si los correos no coinciden
                    MessageBox.Show("El correo de Google no coincide con el correo asociado a su cuenta.",
                        "Error de Autenticación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    // Opcional: Revocar la autorización para limpiar las credenciales
                    await credential.RevokeTokenAsync(CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en el inicio de sesión: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
