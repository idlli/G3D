using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace G3D.Duplicata
{
    /// <summary>
    /// Interaction logic for RetraitDuDiplômeD.xaml
    /// </summary>
    public partial class Retrait : UserControl
    {
        List<Diplôme.RetraitView> _Data = new List<Diplôme.RetraitView>();
        public Retrait()
        {
            InitializeComponent();
            try
            {
                SearchByBox.ItemsSource = App.Ds.Duplicata.Select(o => $"{o.StagiaireGroupeRow.StagiaireRow.NomPrénom} ~ {o.StagiaireGroupeRow.Stagiaire} ~ [{o.Id}]");
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        string _Path = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog Ofd = new System.Windows.Forms.OpenFileDialog();
                Ofd.Filter = "Image|*.JPG;*.PNG;*.PDF;";
                Ofd.Multiselect = false;
                System.Windows.Forms.DialogResult Dr = Ofd.ShowDialog();
                if (Dr == System.Windows.Forms.DialogResult.Abort || Dr == System.Windows.Forms.DialogResult.Cancel) return;
                _Path = Ofd.FileName.ToString();
                Path.Text = System.IO.Path.GetFileName(_Path);
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
                if (SearchByBox.SelectedItem != null)
                {
                    NomPrenom.Text = Cin.Text = Cef.Text = Groupe.Text = État.Text = Tel1.Text = Tel2.Text = "";

                    _full = SearchByBox.Text.Split('~');
                    for (int i = 0; i < _full.Length; i++) { _full[i] = _full[i].Trim(); }
                    _full[2] = _full[2].Substring(1, _full[2].Length - 2);
                    var _stg = App.Ds.Duplicata.Where(o =>
                                o.Id.ToString().Equals(_full[2])
                                ).Select(o => o.StagiaireGroupeRow).FirstOrDefault();
                    if (_stg != null)
                    {
                        Groupe.Text = _stg.GroupeRow.FilièreAnnéeRow.Filière + " " + _stg.GroupeRow.Numéro;
                        NomPrenom.Text = _full[0];
                        Cin.Text = _stg.StagiaireRow.Cin;
                        Cef.Text = _full[1];
                        État.Text = _stg.GetStagiaireÉtatSignatureRows().Select(o => o.État).FirstOrDefault() == null ? "Non Edité" : _stg.GetStagiaireÉtatSignatureRows().Select(o => o.État).FirstOrDefault();
                        string _tel1, _tel2;
                        try { _tel1 = _stg.StagiaireRow.NuméroTéléphonePremier; } catch { _tel1 = ""; }
                        try { _tel2 = _stg.StagiaireRow.NuméroTéléphoneDeuxième; } catch { _tel2 = ""; }
                        Tel1.Text = _tel1;
                        Tel2.Text = _tel2;
                        Modifier.IsEnabled = App.Ds.StagiaireRetrait.Where(o => o.StagiaireGroupe == _stg.Id).Any();
                        Valider.IsEnabled = true;
                        ReFillRetraitDocuments(_stg);
                    }
                }
            }
            catch (Exception ex) 
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void ReFillRetraitDocuments(DataSet1.StagiaireGroupeRow _stg)
        {
            try
            {
                DataGridData.ItemsSource = null;
                _Data.Clear();
                DiplômeNumero.Text = "";
                DiplômeCab.Text = "";
                DiplômeNumero.IsEnabled = true;
                DiplômeCab.IsEnabled = true;

                if (_stg.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Envoyé", StringComparison.InvariantCultureIgnoreCase)).Any())
                {
                    DiplômeInfos.IsEnabled = true;
                    Valider.IsEnabled = true;
                }
                else
                {
                    DiplômeInfos.IsEnabled = false;
                    Valider.IsEnabled = false;
                }

                var _retraitDocuments = App.Ds.StagiaireRetrait.Where(o => o.StagiaireGroupe.Equals(_stg.Id));
                if (_retraitDocuments.Any())
                {
                    foreach (DataSet1.StagiaireRetraitRow _rt in _retraitDocuments)
                    {
                        if (_rt.TypeDocument.Equals("Diplôme"))
                        {
                            DiplômeInfos.IsEnabled = false;
                            var _rtD = App.Ds.RetraitDiplôme.Where(o => o.StagiaireRetrait.Equals(_rt.Id)).FirstOrDefault();
                            if (_rtD != null)
                            {
                                DiplômeNumero.Text = _rtD.NuméroSérie;
                                DiplômeCab.Text = _rtD.Cab;
                                //Path.Text = _rtD.CheminDocumentVérification;
                                DiplômeNumero.IsEnabled = false;
                                DiplômeCab.IsEnabled = false;
                                if (_rtD.DocumentVérification.Equals("Cin")) CinRadio.IsChecked = true;
                                else ProcurationRadio.IsChecked = true;
                                _Data.Add(new Diplôme.RetraitView(_rt.TypeDocument, _rtD.NuméroSérie, _rtD.Cab, _rt.DateRetrait.ToShortDateString()));
                            }
                        }
                    }
                    DataGridData.ItemsSource = _Data;
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            if (!EditOnClick())
            {
                Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue("La modification s'est terminée avec succès");
            }
        }

        private bool EditOnClick()
        {
            Tel1Warning.Visibility = Tel2Warning.Visibility = Visibility.Collapsed;
            Tel1Text.Text = Tel2Text.Text = "";
            _isDirty = false;
            try
            {
                var _stg = App.Ds.Stagiaire.Where(o => o.Cef.Equals(Cef.Text, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (_stg != null)
                {
                    if (string.IsNullOrEmpty(Tel1.Text))
                    {
                        Tel1Warning.Visibility = Visibility.Visible;
                        Tel1Text.Text = "Veuillez insérer le premier numéro de téléphone";
                        _isDirty = true;
                    }
                    if (string.IsNullOrEmpty(Tel2.Text))
                    {
                        Tel2Warning.Visibility = Visibility.Visible;
                        Tel2Text.Text = "Veuillez insérer le deuxième numéro de téléphone";
                        _isDirty = true;
                    }
                    if (_isDirty) return _isDirty;
                    try
                    {
                        DbContext.StagiaireAdapter.UpdateStagiaire(_full[1], Tel1.Text, Tel2.Text, null);
                    }
                    catch
                    {
                        Main.Mad.MyErrorSnackbarIcon.MessageQueue.Enqueue("Veuillez valider tous les champs avant de continuer");
                        _isDirty = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
                _isDirty = true;
            }
            return _isDirty;
        }

        bool _isDirty;
        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tel1Warning.Visibility = Tel2Warning.Visibility = Visibility.Collapsed;
                Tel1Text.Text = Tel2Text.Text = "";
                DiplômeNumeroWarning.Visibility = DiplômeCabWarning.Visibility = PathWarning.Visibility = Visibility.Collapsed;
                DiplômeNumeroWarningText.Text = DiplômeCabWarningText.Text = PathWarningText.Text = "";
                _isDirty = false;
                if (DiplômeInfos.IsEnabled)
                {
                    if (DiplômeNumero.Text.Length < 1)
                    {
                        DiplômeNumeroWarning.Visibility = Visibility.Visible;
                        DiplômeNumeroWarningText.Text = "Veuillez insérer un numéro de série";
                        _isDirty = true;
                    }
                    if (DiplômeCab.Text.Length < 1)
                    {
                        DiplômeCabWarning.Visibility = Visibility.Visible;
                        DiplômeCabWarningText.Text = "Veuillez insérer un numéro de série";
                        _isDirty = true;
                    }
                    if (Path.Text.Length < 1 && ProcurationRadio.IsChecked.Value)
                    {
                        PathWarning.Visibility = Visibility.Visible;
                        PathWarningText.Text = "Veuillez sélectionner un fichier";
                        _isDirty = true;
                    }
                }
                if (DateRetrait.SelectedDate == null)
                {
                    DateRetraitWarning.Visibility = Visibility.Visible;
                    DateRetraitWarningText.Text = "Veuillez sélectionner une date";
                    _isDirty = true;
                }
                if (_isDirty) return;
                if (EditOnClick()) return;
                var _stg = App.Ds.Duplicata.Where(o =>
                                o.Id.ToString().Equals(_full[2])
                                ).Select(o => o.StagiaireGroupeRow).FirstOrDefault();
                if (_stg != null)
                {
                    if (DiplômeInfos.IsEnabled)
                    {
                        DbContext.StagiaireRetraitAdapter.Insert(_stg.Id, "Diplôme", DateRetrait.SelectedDate.Value);
                        App.Ds.StagiaireRetrait.Clear();
                        DbContext.StagiaireRetraitAdapter.Fill(App.Ds.StagiaireRetrait);
                        var _stgR = App.Ds.StagiaireRetrait.Where(o =>
                            o.StagiaireGroupe == _stg.Id
                            && o.TypeDocument == "Diplôme"
                            && o.DateRetrait == DateRetrait.SelectedDate.Value).FirstOrDefault();
                        if (_stgR != null)
                        {
                            DbContext.RetraitDiplômeAdapter.Insert(_stgR.Id, DiplômeNumero.Text, DiplômeCab.Text, CinRadio.IsChecked == null ? "Procuration" : (CinRadio.IsChecked.Value ? "Cin" : "Procuration"), _Path);
                        }
                    }

                    DbContext.ReFillDuplicataRetrait();

                    _stg = App.Ds.Duplicata.Where(o =>
                                o.Id.ToString().Equals(_full[2])
                                ).Select(o => o.StagiaireGroupeRow).FirstOrDefault();
                    if (_stg != null)
                        ReFillRetraitDocuments(_stg);

                    Modifier.IsEnabled = true;

                    Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue("Validation terminée avec succès");
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void SearchByBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (SearchByBox.Text.Length < 1)
            {
                Modifier.IsEnabled = false;
                Valider.IsEnabled = false;

                NomPrenom.Text = Cin.Text = Cef.Text = Groupe.Text = Tel1.Text = Tel2.Text = "";

                DataGridData.ItemsSource = null;
                DiplômeNumero.Text = "";
                DiplômeCab.Text = "";
                DiplômeNumero.IsEnabled = true;
                DiplômeCab.IsEnabled = true;
                Path.Text = "";
            }
        }

        private void Tel1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (((TextBox)sender).Text.Length > 0)
            //{
            //    Modifier.IsEnabled = true;
            //    Valider.IsEnabled = true;
            //}
        }

        private void CinRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (ScannedFile != null)
                ScannedFile.Visibility = Visibility.Collapsed;
        }

        private void CinRadio_Unchecked(object sender, RoutedEventArgs e)
        {
            ScannedFile.Visibility = Visibility.Visible;
        }
    }
}
