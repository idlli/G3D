using System.Windows;

namespace G3D.Authentication
{
    /// <summary>
    /// Interaction logic for Identification.xaml
    /// </summary>
    public partial class Identification : Window
    {
        public static Identification _iden;
        public Identification()
        {
            InitializeComponent();
            _iden = this;
        }

        private void Connexion_Click(object sender, RoutedEventArgs e)
        {
            Connexion connexion = new Connexion();
            connexion.Show();
            this.Close();
        }
    }
}
