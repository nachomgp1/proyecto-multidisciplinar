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

namespace MAIL.view
{
    public partial class MainWindowMail : Window
    {
        private const string ApplicationName = "Gmail Login WPF App";
        private static string[] Scopes = { GmailService.Scope.GmailReadonly , GmailService.Scope.GmailSend, GmailService.Scope.MailGoogleCom};
        private UserCredential credential;

        public MainWindowMail()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para volver atrás
            this.Close();
        }

        private async void GoogleLoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../recursos/credentials.json");

                
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

                // Abrir ventana de correos
                EmailsWindow emailsWindow = new EmailsWindow(service, credential,this);
                emailsWindow.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en el inicio de sesión: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
