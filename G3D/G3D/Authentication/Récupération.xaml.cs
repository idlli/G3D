using System;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;

namespace G3D.Authentication
{
    /// <summary>
    /// Interaction logic for Récupération.xaml
    /// </summary>
    public partial class Récupération : UserControl
    {
        public Récupération()
        {
            InitializeComponent();
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

        private void ShowNewPassword_Checked(object sender, RoutedEventArgs e)
        {
            App.ShowPasswordChecked(NewPassword, NewPasswordText);
        }

        private void ShowNewPassword_UnChecked(object sender, RoutedEventArgs e)
        {
            App.ShowPasswordUnChecked(NewPassword, NewPasswordText);
        }

        private void ShowConfirmePassword_Checked(object sender, RoutedEventArgs e)
        {
            App.ShowPasswordChecked(ConfirmePassword, ConfirmePasswordText);
        }

        private void ShowConfirmePassword_UnChecked(object sender, RoutedEventArgs e)
        {
            App.ShowPasswordUnChecked(ConfirmePassword, ConfirmePasswordText);
        }

        private void NewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            App.PasswordChanged(sender, NewPassword, NewPasswordText, ShowNewPassword);
        }
        private void ConfirmePassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            App.PasswordChanged(sender, ConfirmePassword, ConfirmePasswordText, ShowConfirmePassword);
        }

        int _count = 0;
        string _mailWarning = "Saisissez une adresse e-mail ou nom d'utilisateur";
        string _cléWarning = "Saisissez un clé de produit";

        bool _isDirty = false;
        bool _isMail = false;
        bool? _isHere = false;
        int? _id = null;
        bool? _capacity = null;
        int? _user = null;
        private void Suivant_Click(object sender, RoutedEventArgs e)
        {
            _isDirty = false;
            IdentitéValidation.Visibility = CléProduitValidation.Visibility = SuivantValidation.Visibility = MotPasseValidationRegex.Visibility = Visibility.Collapsed;
            IdentitéValidationText.Text = CléProduitValidationText.Text = SuivantValidationText.Text = "";
            try
            {
                if (string.IsNullOrWhiteSpace(Identité.Text) || string.IsNullOrWhiteSpace(CléProduit.Text))
                {
                    if (string.IsNullOrWhiteSpace(Identité.Text))
                    {
                        IdentitéValidation.Visibility = Visibility.Visible;
                        IdentitéValidationText.Text = _mailWarning;
                    }
                    if (string.IsNullOrWhiteSpace(CléProduit.Text))
                    {
                        CléProduitValidation.Visibility = Visibility.Visible;
                        CléProduitValidationText.Text = _cléWarning;
                    }
                    return;
                }
                try
                {
                    var _mail = new MailAddress(Identité.Text);
                    _isMail = true;
                    if (!App.Ds.Utilisateur.Where(o => o.EMail == Identité.Text).Any())
                    {
                        Identité.Visibility = Visibility.Visible;
                        Identité.Text = "Cette adresse e-mail est introuvable";
                        _isDirty = true;
                    }
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

                DbContext.CléProduitAdapter.CheckClé(CléProduit.Text, ref _id, ref _capacity);
                if (_id == null)
                {
                    CléProduitValidation.Visibility = Visibility.Visible;
                    CléProduitValidationText.Text = "Saisissez le clé de produit valide";
                    _isDirty = true;
                }
                else if (_capacity == false)
                {
                    CléProduitValidation.Visibility = Visibility.Visible;
                    CléProduitValidationText.Text = "Cette clé de produit est déjà utilisée";
                    _isDirty = true;
                }
                _count++;
                if (_count == 4)
                {
                    SuivantValidation.Visibility = Visibility.Visible;
                    SuivantValidationText.Text = "Vous avez dépassé votre limite pour cette session, veuillez réessayer ultérieurement";
                    Suivant.IsEnabled = false;
                    _isDirty = true;
                }
                if (_isDirty) return;
                if (_isMail)
                {
                    _user = App.Ds.Utilisateur.Where(o => o.CléProduit == _id && o.EMail == Identité.Text).Select(o => o.Id).FirstOrDefault();
                    if (_user == null || _user < 1)
                    {
                        SuivantValidation.Visibility = Visibility.Visible;
                        SuivantValidationText.Text = "L'adresse e-mail et la clé d'accès sont incompatibles";
                        return;
                    }
                }
                else
                {
                    _user = App.Ds.Utilisateur.Where(o => o.CléProduit == _id && o.NomUtilisateur == Identité.Text).Select(o => o.Id).FirstOrDefault();
                    if (_user == null || _user < 1)
                    {
                        SuivantValidation.Visibility = Visibility.Visible;
                        SuivantValidationText.Text = "Nom d'utilisateur et la clé d'accès sont incompatibles";
                        return;
                    }
                }
                AccountFind.Visibility = Visibility.Collapsed;
                ChangePassword.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SuivantValidation.Visibility = Visibility.Visible;
                SuivantValidationText.Text = ex.Message;
            }
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            _isDirty = false;
            NewPasswordValidation.Visibility = ConfirmePasswordValidation.Visibility = ModifierValidation.Visibility = Visibility.Collapsed;
            NewPasswordValidationText.Text = ConfirmePasswordValidationText.Text = ModifierValidationText.Text = "";
            try
            {
                if (string.IsNullOrWhiteSpace(NewPassword.Password) || string.IsNullOrWhiteSpace(ConfirmePassword.Password))
                {
                    if (string.IsNullOrWhiteSpace(NewPassword.Password))
                    {
                        NewPasswordValidation.Visibility = Visibility.Visible;
                        NewPasswordValidationText.Text = "Saisissez votre nouveau mot de passe";
                    }
                    if (string.IsNullOrWhiteSpace(CléProduit.Text))
                    {
                        ConfirmePasswordValidation.Visibility = Visibility.Visible;
                        ConfirmePasswordValidationText.Text = "Re-Saisissez votre nouveau mot de passe";
                    }
                    return;
                }
                if (!App._password.IsMatch(NewPassword.Password))
                {
                    MotPasseValidationRegex.Visibility = Visibility.Visible;
                    _isDirty = true;
                }
                if (NewPassword.Password != ConfirmePassword.Password)
                {
                    ConfirmePasswordValidation.Visibility = Visibility.Visible;
                    ConfirmePasswordValidationText.Text = "Le champe n'est pas compatible avec nouveau mot de passe";
                    _isDirty = true;
                }
                if (_isDirty) return;
                try
                {
                    DbContext.UtilisateurAdapter.UtilisateurChangePassword(_user, ConfirmePassword.Password);
                    DbContext.ReFillFirstUsing();
                    Connexion _connexion = new Connexion();
                    _connexion.Show();
                    Identification._iden.Close();
                }
                catch (Exception ex)
                {
                    ModifierValidation.Visibility = Visibility.Visible;
                    ModifierValidationText.Text = ex.Message.ToString();
                }
            }
            catch (Exception ex)
            {
                ModifierValidation.Visibility = Visibility.Visible;
                ModifierValidationText.Text = ex.Message;
            }
        }
    }
}
