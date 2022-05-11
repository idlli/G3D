using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace G3D
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static Main md;
        public static DataSet1 Ds = new DataSet1();
        public static Regex _userName = new Regex(@"^(?=[a-zA-Z_])[-\w]{2,19}([a-zA-Z\d]|(?<![-.])_)$");
        public static Regex _password = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,100}$");
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var culture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            DbContext.ReFillFirstUsing();
        }

        static SolidColorBrush sc1 = new SolidColorBrush(Color.FromRgb(0, 119, 208));
        static SolidColorBrush sc2 = new SolidColorBrush(Color.FromRgb(218, 220, 224));
        static SolidColorBrush br = new SolidColorBrush(Color.FromRgb(63, 67, 71));

        public static void MouseEnter(object sender)
        {
            if (sender is TextBox)
                ((TextBox)sender).BorderThickness = new Thickness(2, 2, 2, 2);
            else if (sender is PasswordBox)
                ((PasswordBox)sender).BorderThickness = new Thickness(2, 2, 2, 2);
        }

        public static void MouseLeave(object sender)
        {
            if (sender is TextBox)
            {
                if (!((TextBox)sender).IsFocused)
                    ((TextBox)sender).BorderThickness = new Thickness(1);
                else
                    ((TextBox)sender).BorderThickness = new Thickness(2);
            }
            else if (sender is PasswordBox)
            {
                if (!((PasswordBox)sender).IsFocused)
                    ((PasswordBox)sender).BorderThickness = new Thickness(1);
                else
                    ((PasswordBox)sender).BorderThickness = new Thickness(2);
            }
        }

        public static void GotFocus(object sender)
        {
            if (sender is TextBox)
            {
                ((TextBox)sender).BorderBrush = sc1;
                ((TextBox)sender).BorderThickness = new Thickness(2);
            }
            else if (sender is PasswordBox)
            {
                ((PasswordBox)sender).BorderBrush = sc1;
                ((PasswordBox)sender).BorderThickness = new Thickness(2);
            }
            else if (sender is ComboBox)
            {
                ((ComboBox)sender).BorderBrush = sc1;
                ((ComboBox)sender).BorderThickness = new Thickness(2);
            }
        }

        public static void LostFocus(object sender)
        {
            if (sender is TextBox)
            {
                ((TextBox)sender).BorderBrush = sc2;
                ((TextBox)sender).BorderThickness = new Thickness(1);
            }
            else if (sender is PasswordBox)
            {
                ((PasswordBox)sender).BorderBrush = sc2;
                ((PasswordBox)sender).BorderThickness = new Thickness(1);
            }
            else if (sender is ComboBox)
            {
                ((ComboBox)sender).BorderBrush = sc2;
                ((ComboBox)sender).BorderThickness = new Thickness(1);
            }
        }
        public static void ShowPasswordChecked(PasswordBox MotPasse, TextBox MotPasseText)
        {
            MotPasse.Visibility = Visibility.Collapsed;
            MotPasseText.Visibility = Visibility.Visible;
            MotPasseText.Text = new NetworkCredential(string.Empty, MotPasse.SecurePassword).Password;
            MotPasseText.Focus();
        }

        public static void ShowPasswordUnChecked(PasswordBox MotPasse, TextBox MotPasseText)
        {
            MotPasse.Visibility = Visibility.Visible;
            MotPasseText.Visibility = Visibility.Collapsed;
            MotPasse.Password = MotPasseText.Text;
            MotPasseText.Text = "";
            MotPasse.Focus();
        }
        public static void PasswordChanged(object sender, PasswordBox MotPasse, TextBox MotPasseText, ToggleButton ShowPassword)
        {
            if (sender is PasswordBox && MotPasse.Password.Length != 0)
            {
                if (ShowPassword.Visibility == Visibility.Collapsed)
                    ShowPassword.Visibility = Visibility.Visible;
                return;
            }
            else if (sender is TextBox)
            {
                if (MotPasseText.Text.Length != 0)
                {
                    if (ShowPassword.Visibility == Visibility.Collapsed)
                        ShowPassword.Visibility = Visibility.Visible;
                    return;
                }
                else
                {
                    if (ShowPassword.Visibility == Visibility.Visible && MotPasse.Password.Length == 0)
                        ShowPassword.Visibility = Visibility.Collapsed;
                    MotPasseText.Visibility = Visibility.Collapsed;
                    MotPasse.Visibility = Visibility.Visible;
                    ShowPassword.IsChecked = false;
                    return;
                }
            }
            ShowPassword.Visibility = Visibility.Collapsed;
        }
    }
}
