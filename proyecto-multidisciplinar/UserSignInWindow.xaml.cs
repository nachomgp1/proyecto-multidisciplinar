using System;
using System.Collections.Generic;
using System.Linq;
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

namespace proyecto_multidisciplinar
{
    /// <summary>
    /// Lógica de interacción para UserSignInWindow.xaml
    /// </summary>
    public partial class UserSignInWindow : Window
    {
        public UserSignInWindow()
        {
            InitializeComponent();
        }
        private void addGroups(object sender, RoutedEventArgs e)
        {
            
            String postgreSQL= "postgresql://ProyectoMulti_owner:Xmn5zOt1AZES@ep-solitary-hall-a2pwd4fg.eu-central-1.aws.neon.tech/ProyectoMulti?sslmode=require";
           
            RadioButton radioButton = sender as RadioButton;

            ComboBox existingComboBox = null;
            foreach (var child in userSignIngGrid.Children)
            {
                if (child is ComboBox comboBox && comboBox.Name == "groupsComboBox")
                {
                    existingComboBox = comboBox;
                    break;
                }
            }

            if (existingComboBox != null)
            {
                userSignIngGrid.Children.Remove(existingComboBox);
            }

            
            if (radioButton.Name == "groupUser")
            {
                ComboBox groups = new ComboBox
                {
                    Name = "groupsComboBox",
                    Width = 200,
                    Height = 30,
                    Margin = new Thickness(500, 100, 0, 0)
                   
                };

               //deberia de añadirse mediante la base de datos

                groups.Items.Add("Grupo 1");
                groups.Items.Add("Grupo 2");
                groups.Items.Add("Grupo 3");
                groups.SelectedIndex = 0;
                userSignIngGrid.Children.Add(groups);
            }
        }
            
           


        }
        
    
}
