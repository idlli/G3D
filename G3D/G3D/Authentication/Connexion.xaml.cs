using G3D.Properties;
using System;
using System.Linq;
using System.Net.Mail;
using System.Windows;

namespace G3D.Authentication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Connexion : Window
    {
        int _count = 0;
        string _mailWarning = "Saisissez une adresse e-mail ou nom d'utilisateur";
        string _passWarning = "Saisissez un mot de passe";
        bool _isDirty = false;
        bool _isMail = false;
        bool? _isHere = false;
        int? _id = null;

        public Connexion()
        {
            InitializeComponent();

            if (!App.Ds.Utilisateur.Any()) DbContext.ReFillFirstUsing();
        }


        private void GotFocusFunc(object sender, EventArgs e)
        {
            App.GotFocus(sender);
        }

        private void LostFocusFunc(object sender, EventArgs e)
        {
            App.LostFocus(sender);
        }

        private void MouseEnterFunc(object sender, EventArgs e)
        {
            App.MouseEnter(sender);
        }

        private void MouseLeaveFunc(object sender, EventArgs e)
        {
            App.MouseLeave(sender);
        }

        private void Diplôme_Checked(object sender, RoutedEventArgs e)
        {
            AnnéeFormation.Visibility = Visibility.Visible;
        }

        private void Autre_Checked(object sender, RoutedEventArgs e)
        {
            AnnéeFormation.Visibility = Visibility.Collapsed;
        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            App.ShowPasswordChecked(MotPasse, MotPasseText);
        }

        private void ShowPassword_UnChecked(object sender, RoutedEventArgs e)
        {
            App.ShowPasswordUnChecked(MotPasse, MotPasseText);
        }

        private void MotPasse_PasswordChanged(object sender, RoutedEventArgs e)
        {
            App.PasswordChanged(sender, MotPasse, MotPasseText, ShowPassword);
        }

        private void ChangeFormIden(object sender)
        {
            Identification _iden = new Identification();
            if (sender is Inscription)
            {
                _iden.IdentificationContent.Children.Add(new Inscription());
                Identification._iden.Title = "Créez votre compte";
            }
            else
            {
                _iden.IdentificationContent.Children.Add(new Récupération());
                Identification._iden.Title = "Réinitialisez votre mot de passe";
            }
            _iden.Show();
            this.Close();
        }
        private void Créer_Compte_Click(object sender, RoutedEventArgs e)
        {
            ChangeFormIden(new Inscription());

        }

        private void Récupération_Click(object sender, RoutedEventArgs e)
        {
            ChangeFormIden(new Récupération());
        }

        private void ConnexionClick(object sender, RoutedEventArgs e)
        {
            _isDirty = false;
            IdentitéValidation.Visibility = MotPasseValidation.Visibility = ConnexionValidation.Visibility = Visibility.Collapsed;
            IdentitéValidationText.Text = MotPasseValidationText.Text = ConnexionValidationText.Text = "";
            try
            {
                if (string.IsNullOrWhiteSpace(Identité.Text) || string.IsNullOrWhiteSpace(MotPasse.Password))
                {
                    if (string.IsNullOrWhiteSpace(Identité.Text))
                    {
                        IdentitéValidation.Visibility = Visibility.Visible;
                        IdentitéValidationText.Text = _mailWarning;
                    }
                    if (string.IsNullOrWhiteSpace(MotPasse.Password))
                    {
                        MotPasseValidation.Visibility = Visibility.Visible;
                        MotPasseValidationText.Text = _passWarning;
                    }
                    return;
                }
                try
                {
                    var _mail = new MailAddress(Identité.Text);
                    _isMail = true;
                }
                catch (FormatException ex)
                {
                    if (!App._userName.IsMatch(Identité.Text))
                    {
                        IdentitéValidation.Visibility = Visibility.Visible;
                        IdentitéValidationText.Text = $"{_mailWarning} valide";
                        _isDirty = true;
                    }
                }
                if (!App._password.IsMatch(MotPasse.Password))
                {
                    MotPasseValidation.Visibility = Visibility.Visible;
                    MotPasseValidationText.Text = $"{_passWarning} valide";
                    _isDirty = true;
                }
                if (_isDirty) return;
                DbContext.UtilisateurAdapter.FindUtilisateur(_isMail, Identité.Text, MotPasse.Password, ref _isHere, ref _id);
                if (_isHere.Value)
                {
                    if (Diplôme.IsChecked.Value)
                    {
                        _count++;
                        try
                        {
                            _etab = App.Ds.Utilisateur.Where(o => o.Id == _id).Select(o => o.CléProduitRow.Établissement).FirstOrDefault();
                            AnnéeFormation.ItemsSource = App.Ds.FilièreAnnée.Where(o => o.Établissement == _etab).Select(o => o.AnnéeFormation).Distinct();
                        }
                        catch
                        {
                            AnnéeFormation.ItemsSource = App.Ds.FilièreAnnée.Select(o => o.AnnéeFormation).Distinct();
                        }
                        MainDialog.IsOpen = true;
                    }
                    else
                    {
                        Settings.Default.ChaîneConnexion = DbContext.UtilisateurConnectéAdapter.InsertQuery(_id).ToString();
                        if (RememberMe.IsChecked ??= false)
                        {
                            Settings.Default.RestezConnecté = true;
                        }
                        Main _main;
                        if (Duplicata.IsChecked.Value)
                            Settings.Default["Section"] = Duplicata.Content;
                        else
                            Settings.Default["Section"] = Archive.Content;
                        Settings.Default.JustStayConnected = true;
                        Settings.Default.Save();
                        _main = new Main();
                        _main.Show();
                        this.Close();
                    }
                    return;
                }
                ConnexionValidation.Visibility = Visibility.Visible;
                ConnexionValidationText.Text = "Adresse e-mail ou le nom d'utilisateur ou le mot de passe ne sont pas corrects, veuillez revérifier vos informations avant de vous connecter";
                _count++;
                if (_count == 4)
                {
                    ConnexionValidationText.Text = "Vous avez dépassé votre limite pour cette session, veuillez réessayer ultérieurement";
                    ConnexionBtn.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                ConnexionValidation.Visibility = Visibility.Visible;
                ConnexionValidationText.Text = ex.Message;
            }
        }

        string _etab;

        private void Continuer_Click(object sender, RoutedEventArgs e)
        {
            ConnexionValidation.Visibility = Visibility.Collapsed;
            ConnexionValidationText.Text = "";
            try
            {
                Settings.Default.ChaîneConnexion = DbContext.UtilisateurConnectéAdapter.InsertQuery(_id).ToString();
                Settings.Default.Section = Diplôme.Content.ToString();
                Settings.Default.AnnéeFormation = AnnéeFormation.SelectedValue == null ? "" : AnnéeFormation.SelectedValue.ToString();
                Settings.Default.Établissement = _etab ??= "";
                Settings.Default.JustStayConnected = true;
                Settings.Default.Save();
                if (RememberMe.IsChecked ??= false)
                {
                    Settings.Default.RestezConnecté = true;
                }
                Main _main = new Main();
                _main.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MainDialog.IsOpen = false;
                ConnexionValidation.Visibility = Visibility.Visible;
                ConnexionValidationText.Text = ex.Message;
            }
        }
    }
}