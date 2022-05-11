using System;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;

namespace G3D.Authentication
{
    /// <summary>
    /// Interaction logic for Inscription.xaml
    /// </summary>
    public partial class Inscription : UserControl
    {
        string _mailWarning = "Saisissez une adresse e-mail ou nom d'utilisateur";
        string _passWarning = "Saisissez un mot de passe";
        bool _isDirty = false;
        bool _isMail = false;
        int? _id = null;
        bool? _capacity = null;
        int _count = 0;
        public Inscription()
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
        string _nameWarning;
        private void Inscription_Click(object sender, RoutedEventArgs e)
        {
            _isDirty = false;
            NomValidation.Visibility = UserNameValidation.Visibility = EMailValidation.Visibility = MotPasseValidation.Visibility = CléProduitValidation.Visibility = InscriptionValidation.Visibility = MotPasseValidationRegex.Visibility = Visibility.Collapsed;
            NomValidationText.Text = UserNameValidationText.Text = EMailValidationText.Text = MotPasseValidationText.Text = CléProduitValidationText.Text = InscriptionValidationText.Text = "";
            try
            {
                if (string.IsNullOrWhiteSpace(Prénom.Text) || string.IsNullOrWhiteSpace(NomFamille.Text))
                {
                    if (string.IsNullOrWhiteSpace(NomFamille.Text) && string.IsNullOrWhiteSpace(Prénom.Text))
                        _nameWarning = "Saisissez votre prénom et votre nom de famille";
                    else if (string.IsNullOrWhiteSpace(Prénom.Text))
                        _nameWarning = "Saisissez votre prénom";
                    else if (string.IsNullOrWhiteSpace(NomFamille.Text))
                        _nameWarning = "Saisissez votre nom de famille";
                    NomValidation.Visibility = Visibility.Visible;
                    NomValidationText.Text = _mailWarning;
                    _isDirty = true;
                }
                if (!string.IsNullOrWhiteSpace(UserName.Text))
                {
                    if (!App._userName.IsMatch(UserName.Text))
                    {
                        UserNameValidation.Visibility = Visibility.Visible;
                        UserNameValidationText.Text = "Saisissez un nom d'utilisateur valide";
                        _isDirty = true;
                    }
                    else if (App.Ds.Utilisateur.Where(o => o.NomUtilisateur == UserName.Text).Any())
                    {
                        UserNameValidation.Visibility = Visibility.Visible;
                        UserNameValidationText.Text = "Cette nom d'utilisateur est déjà utilisée";
                        _isDirty = true;
                    }
                }
                if (string.IsNullOrWhiteSpace(EMail.Text))
                {
                    EMailValidation.Visibility = Visibility.Visible;
                    EMailValidationText.Text = "Saisissez une adresse e-mail";
                    _isDirty = true;
                }
                else
                {
                    try
                    {
                        var _mail = new MailAddress(EMail.Text);
                        if (App.Ds.Utilisateur.Where(o => o.EMail == EMail.Text).Any())
                        {
                            EMailValidation.Visibility = Visibility.Visible;
                            EMailValidationText.Text = "Cette adresse e-mail est déjà utilisée";
                            _isDirty = true;
                        }
                    }
                    catch (FormatException ex)
                    {
                        EMailValidation.Visibility = Visibility.Visible;
                        EMailValidationText.Text = "Saisissez une adresse e-mail valide";
                        _isDirty = true;
                    }
                }
                if (string.IsNullOrWhiteSpace(MotPasse.Password))
                {
                    MotPasseValidation.Visibility = Visibility.Visible;
                    MotPasseValidationText.Text = "Saisissez votre mot de passe";
                    _isDirty = true;
                }
                else if (!App._password.IsMatch(MotPasse.Password))
                {
                    MotPasseValidationRegex.Visibility = Visibility.Visible;
                    _isDirty = true;
                }
                if (string.IsNullOrWhiteSpace(CléProduit.Text))
                {
                    CléProduitValidation.Visibility = Visibility.Visible;
                    CléProduitValidationText.Text = "Saisissez le clé de produit";
                    _isDirty = true;
                }
                else
                {
                    _count++;
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
                }
                if (_count == 10)
                {
                    InscriptionValidation.Visibility = Visibility.Visible;
                    InscriptionValidationText.Text = "Vous avez dépassé votre limite pour cette session, veuillez réessayer ultérieurement";
                    InscriptionBtn.IsEnabled = false;
                    _isDirty = true;
                }
                if (_isDirty) return;
                DbContext.UtilisateurAdapter.UtilisateurInsertQuery(NomFamille.Text, Prénom.Text, UserName.Text, MotPasse.Password, EMail.Text, _id);
                DbContext.ReFillFirstUsing();
                Connexion _connexion = new Connexion();
                _connexion.Show();
                Identification._iden.Close();
            }

            catch (Exception ex)
            {
                InscriptionValidation.Visibility = Visibility.Visible;
                InscriptionValidationText.Text = ex.Message;
            }
        }
    }
}
