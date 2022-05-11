using G3D.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for ÉtapesDeSignature.xaml
    /// </summary>
    public partial class Étapes : UserControl
    {
        public static Étapes Etp;
        public static string _step = "";
        public Étapes()
        {
            InitializeComponent();

            Etp = this;

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
                    ListViewModel._Data.Clear();
                    ListItemsPanel.Visibility = Visibility.Collapsed;
                    if (SearchBy.SelectedIndex == 0)
                    {
                        SearchByBox.ItemsSource = App.Ds.StagiaireGroupe.Select(o => $"{o.GroupeRow.FilièreAnnéeRow.Filière}{o.GroupeRow.Numéro} ~ {o.StagiaireRow.NomPrénom} ~ {o.StagiaireRow.Cin} ~ {o.Stagiaire}");
                        SelectedTap.Visibility = Visibility.Collapsed;
                    }
                    else if (SearchBy.SelectedIndex == 1)
                    {
                        SearchByBox.ItemsSource = App.Ds.Groupe.Select(o => $"{o.FilièreAnnéeRow.Filière}{o.Numéro} ~ [{o.FilièreAnnéeRow.Établissement}]");
                        SelectedTap.Visibility = Visibility.Visible;
                        SelectedTap.IsEnabled = false;
                    }
                    else
                    {
                        SearchByBox.ItemsSource = App.Ds.FilièreAnnée.Select(o => $"{o.Filière} ~ [{o.Établissement}]");
                        SelectedTap.Visibility = Visibility.Visible;
                        SelectedTap.IsEnabled = false;
                    }
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

                RejetéBorder.Background = _white;
                RejetéLine.Background = RejetéBorder.BorderBrush = RejetéIcon.Foreground = RejetéText.Foreground = _lightGray;
                RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                RejetéDescription.Text = "Ici la date sélectionnée";

                CorrigéBorder.Background = _white;
                CorrigéLine.Background = CorrigéBorder.BorderBrush = CorrigéIcon.Foreground = CorrigéText.Foreground = _lightGray;
                CorrigéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                CorrigéDescription.Text = "Ici la date sélectionnée";

                EnvoyéBorder.Background = _white;
                EnvoyéLine.Background = EnvoyéBorder.BorderBrush = EnvoyéIcon.Foreground = EnvoyéText.Foreground = _lightGray;
                EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                EnvoyéDescription.Text = "Ici la date sélectionnée";

                SignéBorder.Background = _white;
                SignéLine.Background = SignéBorder.BorderBrush = SignéIcon.Foreground = SignéText.Foreground = _lightGray;
                SignéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                SignéDescription.Text = "Ici la date sélectionnée";

                Motif.Visibility = ReEditer.Visibility = Visibility.Collapsed;
                Motif.Text = "";

                ReEditerDateWarning.Visibility = Visibility.Collapsed;
                ReEditerDateWarningText.Text = "";
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

        void Refill(List<int> _get)
        {
            try
            {
                if (_get.Any())
                {
                    var _etat = App.Ds.StagiaireÉtatSignature.Where(o => o.StagiaireGroupe.Equals(_get.First())).FirstOrDefault();
                    Valider.IsEnabled = true;
                    Calendar.Opacity = 1;
                    if (_etat != null)
                    {
                        if (_etat.État.Equals("Edité", StringComparison.InvariantCultureIgnoreCase))
                        {
                            EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _blue;
                            EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                            EditéIcon.Foreground = _white;
                            EditéDescription.Text = _etat.ÉtatDate.Date.ToLongDateString().Substring(0,1).ToUpper() + _etat.ÉtatDate.Date.ToLongDateString().Substring(1);

                            RejetéBorder.Background = _white;
                            RejetéLine.Background = RejetéBorder.BorderBrush = RejetéIcon.Foreground = RejetéText.Foreground = _lightGray;
                            RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                            RejetéDescription.Text = "";

                            CorrigéBorder.Background = _white;
                            CorrigéLine.Background = CorrigéBorder.BorderBrush = CorrigéIcon.Foreground = CorrigéText.Foreground = _lightGray;
                            CorrigéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                            CorrigéDescription.Text = "";

                            EnvoyéLine.Background = EnvoyéBorder.Background = EnvoyéBorder.BorderBrush = EnvoyéIcon.Foreground = EnvoyéText.Foreground = _gray;
                            EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                            EnvoyéIcon.Foreground = _white;
                            EnvoyéDescription.Text = "Sélectionnez une date";

                            SignéDescription.Text = "";

                            _step = "Envoyé";
                        }
                        else if (_etat.État.Equals("Rejeté", StringComparison.InvariantCultureIgnoreCase))
                        {
                            EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _blue;
                            EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                            EditéIcon.Foreground = _white;
                            var _editDate = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateEdité, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                            if (_editDate != null)
                                EditéDescription.Text = _editDate.DateEdité.ToLongDateString().Substring(0, 1).ToUpper() + _editDate.DateEdité.ToLongDateString().Substring(1);

                            RejetéLine.Background = RejetéBorder.BorderBrush = RejetéBorder.Background = RejetéText.Foreground = _blue;
                            RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                            RejetéIcon.Foreground = _white;
                            RejetéDescription.Text = _etat.ÉtatDate.ToLongDateString().Substring(0, 1).ToUpper() + _etat.ÉtatDate.ToLongDateString().Substring(1);

                            CorrigéLine.Background = CorrigéBorder.Background = CorrigéBorder.BorderBrush = CorrigéIcon.Foreground = CorrigéText.Foreground = _gray;
                            CorrigéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                            CorrigéIcon.Foreground = _white;
                            CorrigéDescription.Text = "Sélectionnez une date";

                            EnvoyéDescription.Text = SignéDescription.Text = "";

                            _step = "Corrigé";
                        }
                        else if (_etat.État.Equals("Corrigé", StringComparison.InvariantCultureIgnoreCase))
                        {
                            EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _blue;
                            EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                            EditéIcon.Foreground = _white;
                            var _editDate = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateEdité, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                            if (_editDate != null)
                                EditéDescription.Text = _editDate.DateEdité.ToLongDateString().Substring(0, 1).ToUpper() + _editDate.DateEdité.ToLongDateString().Substring(1);

                            RejetéLine.Background = RejetéBorder.Background = RejetéBorder.BorderBrush = RejetéText.Foreground = _blue;
                            RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                            RejetéIcon.Foreground = _white;
                            var _rejetDate = App.Ds.Rejeté.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateRejeté, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateRejeté).FirstOrDefault();
                            if (_rejetDate != null)
                                RejetéDescription.Text = _rejetDate.DateRejeté.ToLongDateString().Substring(0, 1).ToUpper() + _rejetDate.DateRejeté.ToLongDateString().Substring(1);

                            CorrigéLine.Background = CorrigéBorder.BorderBrush = CorrigéBorder.Background = CorrigéText.Foreground = _blue;
                            CorrigéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                            CorrigéIcon.Foreground = _white;
                            CorrigéDescription.Text = _etat.ÉtatDate.ToLongDateString().Substring(0, 1).ToUpper() + _etat.ÉtatDate.ToLongDateString().Substring(1);

                            EnvoyéLine.Background = EnvoyéBorder.Background = EnvoyéBorder.BorderBrush = EnvoyéIcon.Foreground = EnvoyéText.Foreground = _gray;
                            EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                            EnvoyéIcon.Foreground = _white;
                            EnvoyéDescription.Text = "Sélectionnez une date";

                            SignéDescription.Text = "";

                            _step = "Envoyé";
                        }
                        else if (_etat.État.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var _editDate = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get.First())).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                            if (_editDate != null)
                            {

                                if (App.Ds.Rejeté.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateRejeté, _editDate.DateEdité) >= 0).Any())
                                {
                                    EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _blue;
                                    EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    EditéIcon.Foreground = _white;
                                    var _edit = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateEdité, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                                    if (_edit != null)
                                        EditéDescription.Text = _edit.DateEdité.ToLongDateString().Substring(0, 1).ToUpper() + _edit.DateEdité.ToLongDateString().Substring(1);

                                    RejetéLine.Background = RejetéBorder.Background = RejetéBorder.BorderBrush = RejetéText.Foreground = _blue;
                                    RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    RejetéIcon.Foreground = _white;
                                    var _rejetDate = App.Ds.Rejeté.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateRejeté, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateRejeté).FirstOrDefault();
                                    if (_rejetDate != null)
                                        RejetéDescription.Text = _rejetDate.DateRejeté.ToLongDateString().Substring(0, 1).ToUpper() + _rejetDate.DateRejeté.ToLongDateString().Substring(1);

                                    CorrigéLine.Background = CorrigéBorder.Background = CorrigéBorder.BorderBrush = CorrigéText.Foreground = _blue;
                                    CorrigéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    CorrigéIcon.Foreground = _white;
                                    var _CorrigDate = App.Ds.Corrigé.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateCorrigé, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateCorrigé).FirstOrDefault();
                                    if (_CorrigDate != null)
                                        CorrigéDescription.Text = _CorrigDate.DateCorrigé.ToLongDateString().Substring(0, 1).ToUpper() + _CorrigDate.DateCorrigé.ToLongDateString().Substring(1);

                                    EnvoyéLine.Background = EnvoyéBorder.Background = EnvoyéBorder.BorderBrush = EnvoyéText.Foreground = _blue;
                                    EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    EnvoyéIcon.Foreground = _white;
                                    EnvoyéDescription.Text = _etat.ÉtatDate.ToLongDateString().Substring(0, 1).ToUpper() + _etat.ÉtatDate.ToLongDateString().Substring(1);

                                    SignéLine.Background = SignéBorder.Background = SignéBorder.BorderBrush = SignéText.Foreground = _gray;
                                    SignéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                                    SignéIcon.Foreground = _white;
                                    SignéDescription.Text = "Sélectionnez une date";
                                }
                                else
                                {
                                    EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _blue;
                                    EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    EditéIcon.Foreground = _white;
                                    var _edit = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateEdité, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                                    if (_edit != null)
                                        EditéDescription.Text = _edit.DateEdité.ToLongDateString().Substring(0, 1).ToUpper() + _edit.DateEdité.ToLongDateString().Substring(1);

                                    RejetéBorder.Background = _white;
                                    RejetéLine.Background = RejetéBorder.BorderBrush = RejetéIcon.Foreground = RejetéText.Foreground = _lightGray;
                                    RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                                    RejetéDescription.Text = "";

                                    CorrigéBorder.Background = _white;
                                    CorrigéLine.Background = CorrigéBorder.BorderBrush = CorrigéIcon.Foreground = CorrigéText.Foreground = _lightGray;
                                    CorrigéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                                    CorrigéDescription.Text = "";

                                    EnvoyéLine.Background = EnvoyéBorder.Background = EnvoyéBorder.BorderBrush = EnvoyéText.Foreground = _blue;
                                    EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    EnvoyéIcon.Foreground = _white;
                                    EnvoyéDescription.Text = _etat.ÉtatDate.ToLongDateString().Substring(0, 1).ToUpper() + _etat.ÉtatDate.ToLongDateString().Substring(1);

                                    SignéLine.Background = SignéBorder.Background = SignéBorder.BorderBrush = SignéText.Foreground = _gray;
                                    SignéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                                    SignéIcon.Foreground = _white;
                                    SignéDescription.Text = "Sélectionnez une date";
                                }
                                _step = "Signé";
                                if (SearchBy.SelectedIndex == 0)
                                {
                                    ReEditer.Visibility = Visibility.Visible;
                                }

                            }
                        }
                        else if (_etat.État.Equals("Signé", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var _editDate = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get.First())).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                            if (_editDate != null)
                            {

                                if (App.Ds.Rejeté.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateRejeté, _editDate.DateEdité) >= 0).Any())
                                {
                                    EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _blue;
                                    EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    EditéIcon.Foreground = _white;
                                    var _edit = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateEdité, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                                    if (_edit != null)
                                        EditéDescription.Text = _edit.DateEdité.ToLongDateString().Substring(0, 1).ToUpper() + _edit.DateEdité.ToLongDateString().Substring(1);

                                    RejetéLine.Background = RejetéBorder.Background = RejetéBorder.BorderBrush = RejetéText.Foreground = _blue;
                                    RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    RejetéIcon.Foreground = _white;
                                    var _rejetDate = App.Ds.Rejeté.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateRejeté, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateRejeté).FirstOrDefault();
                                    if (_rejetDate != null)
                                        RejetéDescription.Text = _rejetDate.DateRejeté.ToLongDateString().Substring(0, 1).ToUpper() + _rejetDate.DateRejeté.ToLongDateString().Substring(1);

                                    CorrigéLine.Background = CorrigéBorder.Background = CorrigéBorder.BorderBrush = CorrigéText.Foreground = _blue;
                                    CorrigéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    CorrigéIcon.Foreground = _white;
                                    var _CorrigDate = App.Ds.Corrigé.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateCorrigé, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateCorrigé).FirstOrDefault();
                                    if (_CorrigDate != null)
                                        CorrigéDescription.Text = _CorrigDate.DateCorrigé.ToLongDateString().Substring(0, 1).ToUpper() + _CorrigDate.DateCorrigé.ToLongDateString().Substring(1);

                                    EnvoyéLine.Background = EnvoyéBorder.Background = EnvoyéBorder.BorderBrush = EnvoyéText.Foreground = _blue;
                                    EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    EnvoyéIcon.Foreground = _white;
                                    var _EnvoyeDate = App.Ds.Envoyé.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateEnvoyé, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateEnvoyé).FirstOrDefault();
                                    if (_EnvoyeDate != null)
                                        EnvoyéDescription.Text = _EnvoyeDate.DateEnvoyé.ToLongDateString().Substring(0, 1).ToUpper() + _EnvoyeDate.DateEnvoyé.ToLongDateString().Substring(1);

                                    SignéLine.Background = SignéBorder.Background = SignéBorder.BorderBrush = SignéText.Foreground = _blue;
                                    SignéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    SignéIcon.Foreground = _white;
                                    SignéDescription.Text = _etat.ÉtatDate.ToLongDateString().Substring(0, 1).ToUpper() + _etat.ÉtatDate.ToLongDateString().Substring(1);
                                }
                                else
                                {
                                    EditéBorder.Background = EditéBorder.BorderBrush = EditéText.Foreground = _blue;
                                    EditéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    EditéIcon.Foreground = _white;
                                    var _edit = App.Ds.Edité.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateEdité, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateEdité).FirstOrDefault();
                                    if (_edit != null)
                                        EditéDescription.Text = _edit.DateEdité.ToLongDateString().Substring(0, 1).ToUpper() + _edit.DateEdité.ToLongDateString().Substring(1);

                                    RejetéBorder.Background = _white;
                                    RejetéLine.Background = RejetéBorder.BorderBrush = RejetéIcon.Foreground = RejetéText.Foreground = _lightGray;
                                    RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                                    RejetéDescription.Text = "";

                                    CorrigéBorder.Background = _white;
                                    CorrigéLine.Background = CorrigéBorder.BorderBrush = CorrigéIcon.Foreground = CorrigéText.Foreground = _lightGray;
                                    CorrigéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                                    CorrigéDescription.Text = "";

                                    EnvoyéLine.Background = EnvoyéBorder.Background = EnvoyéBorder.BorderBrush = EnvoyéText.Foreground = _blue;
                                    EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    EnvoyéIcon.Foreground = _white;
                                    var _EnvoyeDate = App.Ds.Envoyé.Where(o => o.StagiaireGroupe.Equals(_get.First()) && DateTime.Compare(o.DateEnvoyé, _etat.ÉtatDate) < 0).OrderByDescending(o => o.DateEnvoyé).FirstOrDefault();
                                    if (_EnvoyeDate != null)
                                        EnvoyéDescription.Text = _EnvoyeDate.DateEnvoyé.ToLongDateString().Substring(0, 1).ToUpper() + _EnvoyeDate.DateEnvoyé.ToLongDateString().Substring(1);

                                    SignéLine.Background = SignéBorder.Background = SignéBorder.BorderBrush = SignéText.Foreground = _blue;
                                    SignéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckBold;
                                    SignéIcon.Foreground = _white;
                                    SignéDescription.Text = _etat.ÉtatDate.ToLongDateString().Substring(0, 1).ToUpper() + _etat.ÉtatDate.ToLongDateString().Substring(1);
                                }
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

                        RejetéDescription.Text = CorrigéDescription.Text = EnvoyéDescription.Text = SignéDescription.Text = "";

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
                if (SearchByBox.SelectedItem != null || SelectedTap.SelectedItem != null || e == null)
                {
                    ListViewModel._Data.Clear();
                    _full = SearchByBox.Text.Split('~');
                    for (int i = 0; i < _full.Length; i++) { _full[i] = _full[i].Trim(); }
                    InitialStyle();

                    if (_full[0].Length >= 2)
                    {
                        if (SearchBy.SelectedIndex == 0)
                        {
                            var _get = App.Ds.StagiaireGroupe.Where(o =>
                                o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.NomPrénom.Equals(_full[1], StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.Cin.Equals(_full[2], StringComparison.InvariantCultureIgnoreCase)
                                && o.Stagiaire.Equals(_full[3], StringComparison.InvariantCultureIgnoreCase)
                                ).Select(o => o.Id).ToList();
                            Refill(_get);
                        }
                        else if (SearchBy.SelectedIndex == 1)
                        {
                            var _get = App.Ds.StagiaireGroupe.Where(o =>
                                    o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                    && o.GroupeRow.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                    && o.GroupeRow.FilièreAnnéeRow.Établissement.Equals(_full[1].Substring(1, _full[1].Length - 2), StringComparison.InvariantCultureIgnoreCase)
                                    && SelectedTap.SelectedItem != null ? (
                                    SelectedTap.SelectedIndex == 0 ? o.GetStagiaireÉtatSignatureRows().Count() == 0 : true
                                    && SelectedTap.SelectedIndex == 1 ? o.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Edité", StringComparison.InvariantCultureIgnoreCase)).Any() : true
                                    && SelectedTap.SelectedIndex == 2 ? o.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase)).Any() : true
                                    ) : false
                                    ).ToList();

                            if (_get.Any())
                            {
                                foreach (var _stg in _get.Select(o => o.StagiaireRow))
                                {
                                    ListViewModel._Data.Add(new SelectableViewModel
                                    {
                                        IsSelected = true,
                                        Cef = _stg.Cef,
                                        NomPrénom = _stg.NomPrénom
                                    });
                                }
                                ListData.DataContext = new ListViewModel();
                                SelectedCount.Text = ListViewModel._Data.Where(o => o.IsSelected).Count().ToString();
                                ListItemsPanel.Visibility = Visibility.Visible;
                            }
                            if (SelectedTap.IsEnabled && SelectedTap.SelectedItem != null)
                                Refill(_get.Select(o => o.Id).ToList());
                            else SelectedTap.IsEnabled = true;
                        }


                        else if (SearchBy.SelectedIndex == 2)
                        {

                            var _get = App.Ds.StagiaireGroupe.Where(o =>
                                    o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0], StringComparison.InvariantCultureIgnoreCase)
                                    && o.GroupeRow.FilièreAnnéeRow.Établissement.Equals(_full[1].Substring(1, _full[1].Length - 2), StringComparison.InvariantCultureIgnoreCase)
                                    && SelectedTap.SelectedItem != null ? (
                                    SelectedTap.SelectedIndex == 0 ? o.GetStagiaireÉtatSignatureRows().Count() == 0 : true
                                    && SelectedTap.SelectedIndex == 1 ? o.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Edité", StringComparison.InvariantCultureIgnoreCase)).Any() : true
                                    && SelectedTap.SelectedIndex == 2 ? o.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase)).Any() : true
                                    ) : false
                                    ).ToList();

                            if (_get.Any())
                            {
                                foreach (var _stg in _get.Select(o => o.StagiaireRow))
                                {
                                    ListViewModel._Data.Add(new SelectableViewModel
                                    {
                                        IsSelected = true,
                                        Cef = _stg.Cef,
                                        NomPrénom = _stg.NomPrénom
                                    });
                                }
                                ListData.DataContext = new ListViewModel();
                                SelectedCount.Text = ListViewModel._Data.Where(o => o.IsSelected).Count().ToString();
                                ListItemsPanel.Visibility = Visibility.Visible;

                            }
                            if (SelectedTap.IsEnabled && SelectedTap.SelectedItem != null)
                                Refill(_get.Select(o => o.Id).ToList());
                            else SelectedTap.IsEnabled = true;
                        }
                    }
                }
                else
                {
                    if (SearchByBox.Text.Length <= 0)
                    {
                        ListItemsPanel.Visibility = Visibility.Collapsed;
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
            if (SearchByBox.Text.Length <= 0 && !ListViewModel._Data.Where(o => o.IsSelected).Any())
            {
                SelectedTap.IsEnabled = false;
                InitialStyle();
            }
        }

        private void SupprimerStagiaires_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var _tempData = ListViewModel._Data.Where(o => o.IsSelected).ToList();
                foreach (var item in _tempData)
                {
                    ListViewModel._Data.Remove(item);
                }
                SelectedCount.Text = ListViewModel._Data.Where(o => o.IsSelected).Count().ToString();
                if (ListData.Items.Count < 1)
                {
                    ListItemsPanel.Visibility = Visibility.Collapsed;
                    InitialStyle();
                    Valider.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void SelectedTap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchByBox_SelectionChanged(sender, e);
        }

        private void SearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillAutoCompleteBox();
            if (SelectedTap != null)
                SelectedTap.SelectedItem = null;
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SearchBy.SelectedIndex == 0)
                {
                    var _get = App.Ds.StagiaireGroupe.Where(o =>
                            o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                            && o.GroupeRow.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                            && o.StagiaireRow.NomPrénom.Equals(_full[1], StringComparison.InvariantCultureIgnoreCase)
                            && o.StagiaireRow.Cin.Equals(_full[2], StringComparison.InvariantCultureIgnoreCase)
                            && o.Stagiaire.Equals(_full[3], StringComparison.InvariantCultureIgnoreCase)
                            ).Select(o => o.Id).ToList();
                    if (_get.Any())
                    {
                        if (_step.Equals("Edité", StringComparison.InvariantCultureIgnoreCase))
                            DbContext.EditéAdapter.Insert(_get.First(), Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                        else
                        {


                            if (DateTime.Compare(App.Ds.StagiaireÉtatSignature.Where(o => o.StagiaireGroupe == _get.First()).Select(o => o.ÉtatDate).First(), Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now) < 0)
                            {
                                if (_step.Equals("Rejeté", StringComparison.InvariantCultureIgnoreCase))
                                    DbContext.RejetéAdapter.Insert(_get.First(), Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now, Motif.Text);
                                else if (_step.Equals("Corrigé", StringComparison.InvariantCultureIgnoreCase))
                                    DbContext.CorrigéAdapter.Insert(_get.First(), Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                                else if (_step.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase))
                                    DbContext.EnvoyéAdapter.Insert(_get.First(), Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                                else if (_step.Equals("Signé", StringComparison.InvariantCultureIgnoreCase))
                                    DbContext.SignéAdapter.Insert(_get.First(), Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                            }
                        }


                    }
                }
                else if (SearchBy.SelectedIndex == 1)
                {
                    var _get = App.Ds.StagiaireGroupe.Where(o =>
                                o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.FilièreAnnéeRow.Établissement.Equals(_full[1].Substring(1, _full[1].Length - 2), StringComparison.InvariantCultureIgnoreCase)
                                && SelectedTap.SelectedItem != null ? (
                                SelectedTap.SelectedIndex == 0 ? o.GetStagiaireÉtatSignatureRows().Count() == 0 : true
                                && SelectedTap.SelectedIndex == 1 ? o.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Edité", StringComparison.InvariantCultureIgnoreCase)).Any() : true
                                && SelectedTap.SelectedIndex == 2 ? o.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase)).Any() : true
                                ) : false
                                ).ToList();
                    if (_get.Any())
                    {
                        {
                            if (_step.Equals("Edité", StringComparison.InvariantCultureIgnoreCase))
                            {
                                foreach (int i in _get.Where(o => ListViewModel._Data.Where(o => o.IsSelected).Select(o => o.Cef).Contains(o.Stagiaire)).Select(o => o.Id))
                                    DbContext.EditéAdapter.Insert(i, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);

                            }
                            else
                            {
                                var _getDate = App.Ds.StagiaireÉtatSignature.Where(o => _get.Select(o => o.Id).Contains(o.StagiaireGroupe)).OrderByDescending(o => o.ÉtatDate).Select(o => o.ÉtatDate).First();
                                if (DateTime.Compare(_getDate, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now) < 0)
                                {
                                    foreach (int i in _get.Where(o => ListViewModel._Data.Where(o => o.IsSelected).Select(o => o.Cef).Contains(o.Stagiaire)).Select(o => o.Id))

                                        if (_step.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase))
                                            DbContext.EnvoyéAdapter.Insert(i, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                                        else if (_step.Equals("Signé", StringComparison.InvariantCultureIgnoreCase))
                                            DbContext.SignéAdapter.Insert(i, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                                }
                            }
                        }
                    }
                }
                else if (SearchBy.SelectedIndex == 2)
                {
                    var _get = App.Ds.StagiaireGroupe.Where(o =>
                                o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0], StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.FilièreAnnéeRow.Établissement.Equals(_full[1].Substring(1, _full[1].Length - 2), StringComparison.InvariantCultureIgnoreCase)
                                && SelectedTap.SelectedItem != null ? (
                                SelectedTap.SelectedIndex == 0 ? o.GetStagiaireÉtatSignatureRows().Count() == 0 : true
                                && SelectedTap.SelectedIndex == 1 ? o.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Edité", StringComparison.InvariantCultureIgnoreCase)).Any() : true
                                && SelectedTap.SelectedIndex == 2 ? o.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase)).Any() : true
                                ) : false
                                ).ToList();
                    if (_get.Any())
                    {
                        if (_step.Equals("Edité", StringComparison.InvariantCultureIgnoreCase))
                        {
                            foreach (int i in _get.Where(o => ListViewModel._Data.Where(o => o.IsSelected).Select(o => o.Cef).Contains(o.Stagiaire)).Select(o => o.Id))
                                DbContext.EditéAdapter.Insert(i, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);

                        }
                        else
                        {
                            var _getDate = App.Ds.StagiaireÉtatSignature.Where(o => _get.Select(o => o.Id).Contains(o.StagiaireGroupe)).OrderByDescending(o => o.ÉtatDate).Select(o => o.ÉtatDate).First();
                            if (DateTime.Compare(_getDate, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now) < 0)
                            {
                                foreach (int i in _get.Where(o => ListViewModel._Data.Where(o => o.IsSelected).Select(o => o.Cef).Contains(o.Stagiaire)).Select(o => o.Id))

                                    if (_step.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase))
                                        DbContext.EnvoyéAdapter.Insert(i, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                                    else if (_step.Equals("Signé", StringComparison.InvariantCultureIgnoreCase))
                                        DbContext.SignéAdapter.Insert(i, Calendar.SelectedDate.HasValue ? DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);
                            }
                        }
                    }
                }
                App.Ds.StagiaireÉtatSignature.Clear();
                App.Ds.Edité.Clear();
                App.Ds.Rejeté.Clear();
                App.Ds.Corrigé.Clear();
                App.Ds.Envoyé.Clear();
                App.Ds.Signé.Clear();

                DbContext.StagiaireÉtatSignatureAdapter.Fill(App.Ds.StagiaireÉtatSignature);
                DbContext.EditéAdapter.Fill(App.Ds.Edité);
                DbContext.RejetéAdapter.Fill(App.Ds.Rejeté);
                DbContext.CorrigéAdapter.Fill(App.Ds.Corrigé);
                DbContext.EnvoyéAdapter.Fill(App.Ds.Envoyé);
                DbContext.SignéAdapter.Fill(App.Ds.Signé);

                SearchByBox_SelectionChanged(sender, null);
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void RejetéBorder_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (SearchBy.SelectedIndex == 0 && _step.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase) && RejetéText.Foreground == _lightGray)
                {
                    // Remove Envoye style
                    EnvoyéBorder.Background = _white;
                    EnvoyéLine.Background = EnvoyéBorder.BorderBrush = EnvoyéIcon.Foreground = EnvoyéText.Foreground = _lightGray;
                    EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                    EnvoyéDescription.Text = "";

                    // Set Rejete Style
                    RejetéLine.Background = RejetéBorder.Background = RejetéBorder.BorderBrush = RejetéIcon.Foreground = RejetéText.Foreground = _gray;
                    RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                    RejetéIcon.Foreground = _white;
                    RejetéDescription.Text = "Sélectionnez une date";

                    Motif.Visibility = Visibility.Visible;

                    _step = "Rejeté";
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void RejetéBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (SearchBy.SelectedIndex == 0 && _step.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase) && RejetéText.Foreground == _lightGray)
                RejetéBorder.BorderBrush = RejetéIcon.Foreground = _gray;
        }

        private void RejetéBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (SearchBy.SelectedIndex == 0 && _step.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase) && RejetéText.Foreground == _lightGray)
                RejetéBorder.BorderBrush = RejetéIcon.Foreground = _lightGray;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReEditerDateWarning.Visibility = Visibility.Collapsed;
                ReEditerDateWarningText.Text = "";
                if (ReEditerDate.SelectedDate == null)
                {
                    ReEditerDateWarning.Visibility = Visibility.Visible;
                    ReEditerDateWarningText.Text = "Veuillez sélectionner une date";
                    return;
                }
                else if (!ReEditerDate.SelectedDate.HasValue)
                {
                    ReEditerDateWarning.Visibility = Visibility.Visible;
                    ReEditerDateWarningText.Text = "S'il vous plaît sélectionnez une date valide";
                    return;
                }
                var _get = App.Ds.StagiaireGroupe.Where(o =>
                                o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.NomPrénom.Equals(_full[1], StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.Cin.Equals(_full[2], StringComparison.InvariantCultureIgnoreCase)
                                && o.Stagiaire.Equals(_full[3], StringComparison.InvariantCultureIgnoreCase)
                                ).Select(o => o.Id).ToList();
                if (_get.Any())
                {
                    var _getDate = App.Ds.Envoyé.Where(o => o.StagiaireGroupe == _get.First()).OrderByDescending(o => o.DateEnvoyé).Select(o => o.DateEnvoyé).FirstOrDefault();
                    if (_getDate != null)
                    {
                        if (DateTime.Compare(_getDate, ReEditerDate.SelectedDate.HasValue ? DateTime.Parse(ReEditerDate.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now) < 0)
                        {
                            DbContext.EditéAdapter.Insert(_get.First(), ReEditerDate.SelectedDate.HasValue ? DateTime.Parse(ReEditerDate.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay) : DateTime.Now);

                            App.Ds.Edité.Clear();
                            DbContext.EditéAdapter.Fill(App.Ds.Edité);
                            //DbContext.StagiaireÉtatSignatureAdapter.Insert()

                            App.Ds.StagiaireÉtatSignature.Clear();
                            DbContext.StagiaireÉtatSignatureAdapter.Fill(App.Ds.StagiaireÉtatSignature);

                            SearchByBox_SelectionChanged(sender, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

        private void Calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            //MessageBox.Show(Calendar.SelectedDate.Value./*ToFileTime*/().ToString());
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(Calendar.SelectedDate.Value.ToLocalTime().ToString());
            //MessageBox.Show(Calendar.SelectedDate.Value.ToUniversalTime().ToString());
            //MessageBox.Show(Calendar.SelectedDate.Value.ToLongTimeString().ToString());
            //MessageBox.Show(Calendar.SelectedDate.Value.ToLongDateString().ToString());
            //MessageBox.Show(Calendar.SelectedDate.Value.ToShortTimeString().ToString());
            //MessageBox.Show(Calendar.SelectedDate.Value.TimeOfDay.ToString());

            //MessageBox.Show(DateTime.Parse(Calendar.SelectedDate.Value.ToShortDateString() + " " + DateTime.Now.TimeOfDay).ToString());
        }

        private void EnvoyéBorder_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (SearchBy.SelectedIndex == 0 && _step.Equals("Rejeté", StringComparison.InvariantCultureIgnoreCase) && EnvoyéText.Foreground == _lightGray)
                {
                    // Remove Envoye Rejete
                    RejetéBorder.Background = _white;
                    RejetéLine.Background = RejetéBorder.BorderBrush = RejetéIcon.Foreground = RejetéText.Foreground = _lightGray;
                    RejetéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                    RejetéDescription.Text = "";

                    // Set Rejete Style
                    EnvoyéLine.Background = EnvoyéBorder.Background = EnvoyéBorder.BorderBrush = EnvoyéIcon.Foreground = EnvoyéText.Foreground = _gray;
                    EnvoyéIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle;
                    EnvoyéIcon.Foreground = _white;
                    EnvoyéDescription.Text = "Sélectionnez une date";

                    Motif.Visibility = Visibility.Collapsed;

                    _step = "Envoyé";
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void EnvoyéBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (SearchBy.SelectedIndex == 0 && _step.Equals("Rejeté", StringComparison.InvariantCultureIgnoreCase) && EnvoyéText.Foreground == _lightGray)
                EnvoyéBorder.BorderBrush = EnvoyéIcon.Foreground = _gray;

        }

        private void EnvoyéBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (SearchBy.SelectedIndex == 0 && _step.Equals("Rejeté", StringComparison.InvariantCultureIgnoreCase) && EnvoyéText.Foreground == _lightGray)
                EnvoyéBorder.BorderBrush = EnvoyéIcon.Foreground = _lightGray;
        }
    }
}
