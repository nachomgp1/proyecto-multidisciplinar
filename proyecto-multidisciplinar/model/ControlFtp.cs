using System.IO;
using System.Net;
using System.Windows;

namespace proyecto_multidisciplinar.model;

public class ControlFtp
{
    private string ftpServer;
    private string username;
    private string password;
    
    public ControlFtp(string ftpServer, string username, string password)
    {
        this.ftpServer = ftpServer;
        this.username = username;
        this.password = password;
    }
    
    // Método para listar directorios
    public List<string> ListDirectories(string path)
    {
        List<string> directories = new List<string>();
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{path}");
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        request.Credentials = new NetworkCredential(username, password);

        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                directories.Add(line);
            }
        }
        return directories;
    }

    // Método para subir un archivo
    public void UploadFile(string localFilePath, string remoteFilePath)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{remoteFilePath}");
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.Credentials = new NetworkCredential(username, password);

        byte[] fileContents = File.ReadAllBytes(localFilePath);
        request.ContentLength = fileContents.Length;

        using (Stream requestStream = request.GetRequestStream())
        {
            requestStream.Write(fileContents, 0, fileContents.Length);
        }
    }

    // Método para eliminar un archivo
    public void DeleteFile(string remoteFilePath)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{remoteFilePath}");
        request.Method = WebRequestMethods.Ftp.DeleteFile;
        request.Credentials = new NetworkCredential(username, password);

        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
        {
            // Confirmación de eliminación
        }
    }

    public void DownloadFile(string remoteFilePath, string localFilePath)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{remoteFilePath}");
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        request.Credentials = new NetworkCredential(username, password);

        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
        using (Stream responseStream = response.GetResponseStream())
        using (FileStream fileStream = new FileStream(localFilePath, FileMode.Create))
        {
            responseStream.CopyTo(fileStream);
        }
    }
    
    public void RenameFile(string oldFilePath, string newFilePath)
    {
        try
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri($"{ftpServer}/{oldFilePath}"));
            request.Method = WebRequestMethods.Ftp.Rename;
            request.Credentials = new NetworkCredential(username, password);
            request.RenameTo = newFilePath;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                // El archivo fue renombrado correctamente
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al renombrar el archivo en el servidor FTP: " + ex.Message);
        }
    }
    
    public List<string> ListFiles(string remoteDirectory)
    {
        List<string> files = new List<string>();

        try
        {
            // Crear una solicitud FTP para obtener el listado de archivos
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri($"{ftpServer}{remoteDirectory}"));
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;  // Obtiene detalles sobre los archivos
            request.Credentials = new NetworkCredential(username, password);

            // Obtener la respuesta del servidor FTP
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Procesar cada línea (que representa un archivo o directorio)
                    string fileName = ParseFileName(line);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        files.Add(fileName);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al listar los archivos: " + ex.Message);
        }

        return files;
    }

    private string ParseFileName(string details)
    {
        // Aquí, puedes implementar una lógica para extraer el nombre del archivo del string `details`.
        // Dependiendo del formato del servidor FTP, esto podría necesitar ajuste.
        // Un formato común es Unix, en el que el nombre de archivo está al final de la línea.

        string[] parts = details.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[parts.Length - 1] : null;
    }

}