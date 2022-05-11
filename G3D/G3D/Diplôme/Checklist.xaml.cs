using G3D.Domain;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for Checklist.xaml
    /// </summary>
    public partial class Checklist : UserControl
    {
        public static Checklist Ckl;

        public Checklist()
        {
            InitializeComponent();

            Ckl = this;

            FillAutoCompleteBox();
        }

        BackgroundWorker Bw2 = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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
                    _ReportViewer.Reset();
                    if (SearchBy.SelectedIndex == 0)
                    {
                        SearchByBox.ItemsSource = App.Ds.StagiaireGroupe.Select(o => $"{o.GroupeRow.FilièreAnnéeRow.Filière}{o.GroupeRow.Numéro} ~ {o.StagiaireRow.NomPrénom} ~ {o.StagiaireRow.Cin} ~ {o.Stagiaire}");
                        SelectedType.IsEnabled = false;
                    }
                    else if (SearchBy.SelectedIndex == 1)
                    {
                        SearchByBox.ItemsSource = App.Ds.Groupe.Select(o => $"{o.FilièreAnnéeRow.Filière}{o.Numéro} ~ [{o.FilièreAnnéeRow.Établissement}]");
                        SelectedType.IsEnabled = true;
                    }
                    else
                    {
                        SearchByBox.ItemsSource = App.Ds.FilièreAnnée.Select(o => $"{o.Filière} ~ [{o.Établissement}]");
                        SelectedType.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

        private void SearchByBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (SearchByBox.SelectedItem != null)
                {
                    ListViewModel._Data.Clear();
                    ShowBtn.IsEnabled = true;
                    string[] _full = SearchByBox.Text.Split('~');
                    for (int i = 0; i < _full.Length; i++) { _full[i] = _full[i].Trim(); }
                    if (_full[0].Length >= 2)
                    {
                        if (SearchBy.SelectedIndex == 1)
                        {
                            var _get = App.Ds.StagiaireGroupe.Where(o =>
                                o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.FilièreAnnéeRow.Établissement.Equals(_full[1].Substring(1, _full[1].Length - 2), StringComparison.InvariantCultureIgnoreCase)
                                ).Select(o => o.StagiaireRow);
                            if (_get.Any())
                            {
                                foreach (var _stg in _get)
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
                        }
                        else if (SearchBy.SelectedIndex == 2)
                        {

                            var _get = App.Ds.StagiaireGroupe.Where(o =>
                                o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0], StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.FilièreAnnéeRow.Établissement.Equals(_full[1].Substring(1, _full[1].Length - 2), StringComparison.InvariantCultureIgnoreCase)
                                ).Select(o => o.StagiaireRow);

                            if (_get.Any())
                            {
                                foreach (var _stg in _get)
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
                        }
                    }
                }
                else
                {
                    if (SearchBy.Text.Length <= 0)
                    {
                        ListItemsPanel.Visibility = Visibility.Collapsed;
                        ShowBtn.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void SearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            FillAutoCompleteBox();
        }

        bool _isDirty;
        private void ShowBtn_Click(object sender, RoutedEventArgs e)
        {
            _isDirty = false;
            DateControleWarning.Visibility = DateVerificationWarning.Visibility = DirectionWarning.Visibility = PromotionWarning.Visibility = Visibility.Collapsed;
            DateControleWarningText.Text = DateVerificationWarningText.Text = DirectionWarningText.Text = PromotionWarningText.Text = "";

            try
            {
                if (string.IsNullOrEmpty(DateControle.Text))
                {
                    DateControleWarning.Visibility = Visibility.Visible;
                    DateControleWarningText.Text = "Veuillez sélectionner une date";
                    _isDirty = true;
                }
                if (string.IsNullOrEmpty(DateVerification.Text))
                {
                    DateVerificationWarning.Visibility = Visibility.Visible;
                    DateVerificationWarningText.Text = "Veuillez sélectionner une date";
                    _isDirty = true;
                }
                if (!_isDirty)
                {

                    if (DateTime.Compare(DateControle.SelectedDate.Value.Date, DateVerification.SelectedDate.Value.Date) > 0)
                    {
                        DateControleWarning.Visibility = Visibility.Visible;
                        DateControleWarningText.Text = "Cette date est supérieure à la date de vérification";
                        DateVerificationWarning.Visibility = Visibility.Visible;
                        DateVerificationWarningText.Text = "Cette date est inférieure à la date de contrôle";
                        _isDirty = true;
                    }
                }
                if (string.IsNullOrEmpty(Direction.Text))
                {
                    DirectionWarning.Visibility = Visibility.Visible;
                    DirectionWarningText.Text = "Veuillez insérer une direction";
                    _isDirty = true;
                }
                if (string.IsNullOrEmpty(Promotion.Text))
                {
                    PromotionWarning.Visibility = Visibility.Visible;
                    PromotionWarningText.Text = "Veuillez insérer une promotion";
                    _isDirty = true;
                }

                if (_isDirty) return;

                _ReportViewer.Reset();

                string[] _full = SearchByBox.Text.Split('~');
                for (int i = 0; i < _full.Length; i++) { _full[i] = _full[i].Trim(); }

                List<NewClass> dt = null;


                if (SearchBy.SelectedIndex == 0)
                {
                    dt = App.Ds.StagiaireGroupe.Select(o => new NewClass(o.Stagiaire, o.StagiaireRow.NomPrénom, o.StagiaireRow.Cin, o.GroupeRow.Numéro, o.GroupeRow.TypeStagiaires, o.GroupeRow.FilièreAnnéeRow.Filière, o.GroupeRow.FilièreAnnéeRow.FilièreRow.Nom, o.GroupeRow.FilièreAnnéeRow.FilièreRow.NiveauRow.Nom, o.GroupeRow.FilièreAnnéeRow.Établissement, o.GroupeRow.FilièreAnnéeRow.ÉtablissementRow.Nom, o.GroupeRow.FilièreAnnéeRow.AnnéeFormation)).Where(o =>
                        o.FilièreNomCourt.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                        && o.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                        && o.NomPrénom.Equals(_full[1], StringComparison.InvariantCultureIgnoreCase)
                        && o.Cin.Equals(_full[2], StringComparison.InvariantCultureIgnoreCase)
                        && o.Cef.Equals(_full[3], StringComparison.InvariantCultureIgnoreCase)
                    ).ToList();
                }
                else if (SearchBy.SelectedIndex == 1)
                {
                    dt = App.Ds.StagiaireGroupe.Select(o => new NewClass(o.Stagiaire, o.StagiaireRow.NomPrénom, o.StagiaireRow.Cin, o.GroupeRow.Numéro, o.GroupeRow.TypeStagiaires, o.GroupeRow.FilièreAnnéeRow.Filière, o.GroupeRow.FilièreAnnéeRow.FilièreRow.Nom, o.GroupeRow.FilièreAnnéeRow.FilièreRow.NiveauRow.Nom, o.GroupeRow.FilièreAnnéeRow.Établissement, o.GroupeRow.FilièreAnnéeRow.ÉtablissementRow.Nom, o.GroupeRow.FilièreAnnéeRow.AnnéeFormation)).Where(o =>
                        o.FilièreNomCourt.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                        && o.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                        && o.ÉtablissementCode.Equals(_full[1].Substring(1, _full[1].Length - 2), StringComparison.InvariantCultureIgnoreCase)
                    ).ToList();
                }
                else if (SearchBy.SelectedIndex == 2)
                {
                    dt = App.Ds.StagiaireGroupe.Select(o => new NewClass(o.Stagiaire, o.StagiaireRow.NomPrénom, o.StagiaireRow.Cin, o.GroupeRow.Numéro, o.GroupeRow.TypeStagiaires, o.GroupeRow.FilièreAnnéeRow.Filière, o.GroupeRow.FilièreAnnéeRow.FilièreRow.Nom, o.GroupeRow.FilièreAnnéeRow.FilièreRow.NiveauRow.Nom, o.GroupeRow.FilièreAnnéeRow.Établissement, o.GroupeRow.FilièreAnnéeRow.ÉtablissementRow.Nom, o.GroupeRow.FilièreAnnéeRow.AnnéeFormation)).Where(o =>
                        o.FilièreNomCourt.Equals(_full[0], StringComparison.InvariantCultureIgnoreCase)
                        && o.ÉtablissementCode.Equals(_full[1].Substring(1, _full[1].Length - 2), StringComparison.InvariantCultureIgnoreCase)
                    ).ToList();
                }

                if (dt == null)
                {
                    Main.Mad.MyErrorSnackbarIcon.MessageQueue.Enqueue("Aucun stagiaire trouvé");
                    return;
                }

                if (SearchBy.SelectedIndex == 0)
                {
                    if (dt[0].TypeStagiaires.Equals("CR", StringComparison.InvariantCultureIgnoreCase)) Réguliers.IsChecked = true;
                    else Libres.IsChecked = true;
                }


                ReportDataSource _RptSource = new ReportDataSource("DataSet1", Table.ToDataTable<NewClass>(SearchBy.SelectedIndex == 0 ? dt : dt.Where(o => ListViewModel._Data.Where(o => o.IsSelected).Select(o => o.Cef).Contains(o.Cef)).ToList()));
                _ReportViewer.LocalReport.DataSources.Add(_RptSource);
                _ReportViewer.LocalReport.ReportEmbeddedResource = $"G3D.Diplôme.Reports.{(Vide.IsChecked.Value ? (Réguliers.IsChecked.Value ? "ChecklistCRV" : "ChecklistCLV") : (Réguliers.IsChecked.Value ? "ChecklistCR" : "ChecklistCL"))}.rdlc";
                ReportParameterCollection _params = new ReportParameterCollection();
                _params.Add(new ReportParameter("DateControle", DateControle.SelectedDate.Value.Date.ToShortDateString()));
                _params.Add(new ReportParameter("DateVerification", DateVerification.SelectedDate.Value.Date.ToShortDateString()));
                _params.Add(new ReportParameter("Direction", Direction.Text));
                _params.Add(new ReportParameter("Promotion", Promotion.Text));
                _ReportViewer.LocalReport.SetParameters(_params);
                _ReportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }


        }

        private void _ReportViewerLoad(object sender, System.EventArgs e)
        {
            _ReportViewer.Reset();
        }

        private void SupprimerStagiaires_Click(object sender, RoutedEventArgs e)
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
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void SearchByBox_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SearchByBox.Text.Length <= 0 && !ListViewModel._Data.Where(o => o.IsSelected).Any())
                {
                    ShowBtn.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }
    }

    internal class NewClass
    {
        public string Cef { get; }
        public string NomPrénom { get; }
        public string Cin { get; }
        public string Numéro { get; }
        public string TypeStagiaires { get; }
        public string FilièreNomCourt { get; }
        public string FilièreNom { get; }
        public string NiveauNom { get; }
        public string ÉtablissementCode { get; }
        public string ÉtablissementNom { get; }
        public string AnnéeFormation { get; }

        public NewClass(string cef, string nomPrénom, string cin, string numéro, string typeStagiaires, string filièreNomCourt, string filièreNom, string niveauNom, string établissementCode, string établissementNom, string annéeFormation)
        {
            Cef = cef;
            NomPrénom = nomPrénom;
            Cin = cin;
            Numéro = numéro;
            TypeStagiaires = typeStagiaires;
            FilièreNomCourt = filièreNomCourt;
            FilièreNom = filièreNom;
            NiveauNom = niveauNom;
            ÉtablissementCode = établissementCode;
            ÉtablissementNom = établissementNom;
            AnnéeFormation = annéeFormation;
        }
    }
}
