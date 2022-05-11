using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace G3D.Duplicata
{
    /// <summary>
    /// Interaction logic for ÉtapesDeSignatureD.xaml
    /// </summary>
    public partial class Étapes : UserControl
    {
        public static string _step = "";
        public Étapes()
        {
            InitializeComponent();

            FillAutoCompleteBox();

            InitialStyle();

            Valider.IsEnabled = false;

            Calendar.Opacity = 0.4;
        }

        void FillAutoCompleteBox()
        {
            try
            {
                if (SearchByBox != null)
                {
                    SearchByBox.Text = "";
                    SearchByBox.ItemsSource = App.Ds.DuplicataFiche.Select(o => $"{o.NomPrénom} ~ {o.Cef} ~ [{o.DupId}]");
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        SolidColorBrush _blue = new SolidColorBrush(Color.FromRgb(33, 150, 243));
        SolidColorBrush _gray = new SolidColorBrush(Color.FromRgb(87, 96, 111));
        SolidColorBrush _lightGray = new SolidColorBrush(Color.FromRgb(164, 176, 190));
        SolidColorBrush _white = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        void InitialStyle()
        {
            try
            {
                EditéBorder.Background = _white;
                EditéBorder.BorderBrush = EditéIcon.Foreground = EditéText.Foreground = _lightGray;
                EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                EditéDescription.Text = "Ici la date sélectionnée";

                EnvoyéBorder.Background = _white;
                EnvoyéLine.Background = EnvoyéBorder.BorderBrush = EnvoyéIcon.Foreground = EnvoyéText.Foreground = _lightGray;
                EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                EnvoyéDescription.Text = "Ici la date sélectionnée";
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

        void Refill(int? _get)
        {
            try
            {
                if (_get != null)
                {
                    var _etat = App.Ds.StagiaireÉtatSignature.Where(o => o.StagiaireGroupe.Equals(_get)).FirstOrDefault();
                    Valider.IsEnabled = true;
                    Calendar.Opacity = 1;
                    if (_etat != null)
                    {
                        if (_etat.État.Equals("Edité", StringComparison.InvariantCultureIgnoreCase))
                        {
                            EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _blue;
                            EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                            EditéIcon.Foreground = _white;
                            EditéDescription.Text = _etat.ÉtatDate.Date.ToLongDateString().Substring(0, 1).ToUpper() + _etat.ÉtatDate.Date.ToLongDateString().Substring(1);

                            EnvoyéLine.Background = EnvoyéBorder.Background = EnvoyéBorder.BorderBrush = EnvoyéIcon.Foreground = EnvoyéText.Foreground = _gray;
                            EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                            EnvoyéIcon.Foreground = _white;
                            EnvoyéDescription.Text = "Sélectionnez une date";

                            _step = "Envoyé";
                        }
                        else if (_etat.État.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var _editDate = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get)).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                            if (_editDate != null)
                            {
                                EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _blue;
                                EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                EditéIcon.Foreground = _white;
                                var _edit = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get) && DateTime.Compare(o.DateEdité, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                                if (_edit != null)
                                    EditéDescription.Text = _edit.DateEdité.ToLongDateString().Substring(0, 1).ToUpper() + _edit.DateEdité.Date.ToLongDateString().Substring(1);

                                EnvoyéLine.Background = EnvoyéBorder.Background = EnvoyéBorder.BorderBrush = EnvoyéText.Foreground = _blue;
                                EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                EnvoyéIcon.Foreground = _white;
                                EnvoyéDescription.Text = _etat.ÉtatDate.ToLongDateString().Substring(0, 1).ToUpper() + _etat.ÉtatDate.Date.ToLongDateString().Substring(1);

                                Valider.IsEnabled = false;

                            }
                        }
                    }

                    else
                    {
                        EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _gray;
                        EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                        EditéIcon.Foreground = _white;
                        EditéDescription.Text = "Sélectionnez une date";

                        EnvoyéDescription.Text = "";

                        _step = "Edité";
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        string[] _full;
        private void SearchByBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (SearchByBox.SelectedItem != null || e == null)
                {
                    _full = SearchByBox.Text.Split('~');
                    for (int i = 0; i < _full.Length; i++) { _full[i] = _full[i].Trim(); }
                    _full[2] = _full[2].Substring(1, _full[2].Length - 2);
                    InitialStyle();
                    var _get = App.Ds.Duplicata.Where(o =>
                        o.Id.ToString().Equals(_full[2])
                        ).Select(o => o.StagiaireGroupe).FirstOrDefault();
                    Refill(_get);
                }
                else
                {
                    if (SearchByBox.Text.Length <= 0)
                    {
                        InitialStyle();
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void SearchByBox_TextChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SearchByBox.Text.Length <= 0)
            {
                InitialStyle();
            }
        }

        private void Valider_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                int? _get = App.Ds.Duplicata.Where(o =>
                        o.Id.ToString().Equals(_full[2])
                        ).Select(o => o.StagiaireGroupe).FirstOrDefault();
                if (_get != null)
                {
                    if (_step.Equals("Edité", StringComparison.InvariantCultureIgnoreCase))
                        DbContext.EditéAdapter.Insert(_get, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                    else
                    {
                        if (DateTime.Compare(App.Ds.StagiaireÉtatSignature.Where(o => o.StagiaireGroupe == _get).Select(o => o.ÉtatDate).First(), Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now) < 0)
                        {
                            if (_step.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase))
                                DbContext.EnvoyéAdapter.Insert(_get, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                        }
                    }
                }


                App.Ds.StagiaireÉtatSignature.Clear();
                App.Ds.Edité.Clear();
                //App.Ds.Envoyé.Clear();

                DbContext.StagiaireÉtatSignatureAdapter.Fill(App.Ds.StagiaireÉtatSignature);
                DbContext.EditéAdapter.Fill(App.Ds.Edité);
                //DbContext.EnvoyéAdapter.Fill(App.Ds.Envoyé);

                SearchByBox_SelectionChanged(sender, null);
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

    }
}
