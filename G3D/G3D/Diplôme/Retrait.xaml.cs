using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for RetraitDuDiplôme.xaml
    /// </summary>

    public partial class RetraitView
    {
        string Pièce_Retirée { get; set; }
        string Num { get; set; }
        string Pièce_Présente { get; set; }
        string Date_Retrait { get; set; }

        public string PièceRetirée
        {
            get => Pièce_Retirée;
        }
        public string NumR
        {
            get => Num;
        }
        public string PiècePrésente
        {
            get => Pièce_Présente;
        }
        public string DateRetrait
        {
            get => Date_Retrait;
        }
        public RetraitView(string pièce_Retirée, string num, string pièce_Présente, string date_Retrait)
        {
            Pièce_Retirée = pièce_Retirée;
            Num = num;
            Pièce_Présente = pièce_Présente;
            Date_Retrait = date_Retrait;
        }
    }
    public partial class Retrait : UserControl
    {
        // the proncuction file not saved in archive folder

        List<RetraitView> _Data = new List<RetraitView>();
        public Retrait()
        {
            InitializeComponent();
            try
            {
                SearchByBox.ItemsSource = App.Ds.StagiaireGroupe.Select(o => $"{o.GroupeRow.FilièreAnnéeRow.Filière}{o.GroupeRow.Numéro} ~ {o.StagiaireRow.NomPrénom} ~ {o.StagiaireRow.Cin} ~ {o.Stagiaire}");
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
                    NomPrenom.Text = Cin.Text = Cef.Text = Groupe.Text = Admis.Text = État.Text = Tel1.Text = Tel2.Text = RaisonSociale.Text = "";
                    DateDebut.SelectedDate = null;
                    Embauche.IsChecked = true;

                    _full = SearchByBox.Text.Split('~');
                    for (int i = 0; i < _full.Length; i++) { _full[i] = _full[i].Trim(); }
                    var _stg = App.Ds.StagiaireGroupe.Where(o =>
                                o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.NomPrénom.Equals(_full[1], StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.Cin.Equals(_full[2], StringComparison.InvariantCultureIgnoreCase)
                                && o.Stagiaire.Equals(_full[3], StringComparison.InvariantCultureIgnoreCase)
                                ).FirstOrDefault();
                    if (_stg != null)
                    {
                        Groupe.Text = _full[0];
                        NomPrenom.Text = _full[1];
                        Cin.Text = _full[2];
                        Cef.Text = _full[3];
                        Admis.Text = _stg.Admis;
                        État.Text = _stg.GetStagiaireÉtatSignatureRows().Select(o => o.État).FirstOrDefault() == null ? "Non Edité" : _stg.GetStagiaireÉtatSignatureRows().Select(o => o.État).FirstOrDefault();
                        string _tel1, _tel2, _sit;
                        try { _tel1 = _stg.StagiaireRow.NuméroTéléphonePremier; } catch { _tel1 = ""; }
                        try { _tel2 = _stg.StagiaireRow.NuméroTéléphoneDeuxième; } catch { _tel2 = ""; }
                        try { _sit = _stg.StagiaireRow.SituationActuel; } catch { _sit = ""; }
                        Tel1.Text = _tel1;
                        Tel2.Text = _tel2;
                        if (_sit != "")
                        {
                            if (_sit.Equals("Embauche"))
                            {
                                Embauche.IsChecked = true;
                                var _emb = App.Ds.StagiaireEmbauche.Where(o => o.Stagiaire.Equals(_full[3], StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(o => o.DateInsertion).FirstOrDefault();
                                if (_emb != null)
                                {
                                    RaisonSociale.Text = _emb.RaisonSociale;
                                    try { DateDebut.SelectedDate = _emb.DateDébut; } catch { DateDebut.SelectedDate = null; }
                                }
                            }
                            else if (_sit.Equals("Recherche d'emploi")) RechercheEmploi.IsChecked = true;
                            else PoursuiteFormation.IsChecked = true;
                        }
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
            DataGridData.ItemsSource = null;
            _Data.Clear();
            Releve.IsChecked = false;
            Bac.IsChecked = false;
            DiplômeNumero.Text = "";
            DiplômeCab.Text = "";
            BacNumero.Text = "";
            Diplôme.IsEnabled = true;
            Attestation.IsEnabled = true;
            Releve.IsEnabled = true;
            Bac.IsEnabled = true;
            BacNumero.IsEnabled = true;
            DiplômeNumero.IsEnabled = true;
            DiplômeCab.IsEnabled = true;


            try
            {
                if (_stg.Admis.Equals("Oui"))
                {
                    Diplôme.IsChecked = false;
                    Attestation.IsChecked = false;
                    Diplôme.IsEnabled = true;
                    Attestation.IsEnabled = true;
                    DiplômeInfos.IsEnabled = true;
                }
                else if (_stg.Admis.Equals("Non"))
                {
                    Diplôme.IsChecked = null;
                    Attestation.IsChecked = null;
                    Diplôme.IsEnabled = false;
                    Attestation.IsEnabled = false;
                    DiplômeInfos.IsEnabled = false;
                }
                if (_stg.GetStagiaireÉtatSignatureRows().Where(o => o.État.Equals("Signé", StringComparison.InvariantCultureIgnoreCase)).Any())
                {
                    Diplôme.IsChecked = false;
                    Diplôme.IsEnabled = true;
                    DiplômeInfos.IsEnabled = true;
                }
                else
                {
                    Diplôme.IsChecked = null;
                    Diplôme.IsEnabled = false;
                    DiplômeInfos.IsEnabled = false;
                }
                if (_stg.GroupeRow.FilièreAnnéeRow.FilièreRow.Niveau.Equals("T", StringComparison.InvariantCultureIgnoreCase)
                    || _stg.GroupeRow.FilièreAnnéeRow.FilièreRow.Niveau.Equals("S", StringComparison.InvariantCultureIgnoreCase)
                    || _stg.GroupeRow.FilièreAnnéeRow.FilièreRow.Niveau.Equals("Q", StringComparison.InvariantCultureIgnoreCase))
                {
                    Bac.IsChecked = null;
                    Bac.IsEnabled = false;
                    BacNumero.IsEnabled = false;
                }
                else
                {
                    Bac.IsChecked = false;
                    Bac.IsEnabled = true;
                    BacNumero.IsEnabled = true;
                }
                if (_stg.GroupeRow.AnnéeÉtude.Equals("1A", StringComparison.InvariantCultureIgnoreCase))
                {
                    Diplôme.IsChecked = null;
                    Attestation.IsChecked = null;
                    Diplôme.IsEnabled = false;
                    Attestation.IsEnabled = false;
                    DiplômeInfos.IsEnabled = false;
                    Bac.IsChecked = null;
                    Bac.IsEnabled = false;
                    BacNumero.IsEnabled = false;
                }

                var _retraitDocuments = App.Ds.StagiaireRetrait.Where(o => o.StagiaireGroupe.Equals(_stg.Id));
                if (_retraitDocuments.Any())
                {
                    foreach (DataSet1.StagiaireRetraitRow _rt in _retraitDocuments)
                    {
                        if (_rt.TypeDocument.Equals("Diplôme") && _stg.Admis.Equals("Oui"))
                        {
                            Diplôme.IsChecked = true;
                            Diplôme.IsEnabled = false;
                            DiplômeInfos.IsEnabled = false;
                            var _rtD = App.Ds.RetraitDiplôme.Where(o => o.StagiaireRetrait.Equals(_rt.Id)).FirstOrDefault();
                            if (_rtD != null)
                            {
                                DiplômeNumero.Text = _rtD.NuméroSérie;
                                DiplômeCab.Text = _rtD.Cab;
                                DiplômeNumero.IsEnabled = false;
                                DiplômeCab.IsEnabled = false;
                                if (_rtD.DocumentVérification.Equals("Cin")) CinRadio.IsChecked = true;
                                else ProcurationRadio.IsChecked = true;
                                _Data.Add(new RetraitView(_rt.TypeDocument, _rtD.NuméroSérie, _rtD.Cab, _rt.DateRetrait.ToShortDateString()));
                            }
                        }
                        else if (_rt.TypeDocument.Equals("Attestation de Réussite") && _stg.Admis.Equals("Oui"))
                        {
                            Attestation.IsChecked = true;
                            Attestation.IsEnabled = false;
                            _Data.Add(new RetraitView(_rt.TypeDocument, "", "", _rt.DateRetrait.ToShortDateString()));
                        }
                        else if (_rt.TypeDocument.Equals("Relevé de Notes"))
                        {
                            Releve.IsChecked = true;
                            Releve.IsEnabled = false;
                            _Data.Add(new RetraitView(_rt.TypeDocument, "", "", _rt.DateRetrait.ToShortDateString()));
                        }
                        else if (_rt.TypeDocument.Equals("Baccalauréat"))
                        {
                            Bac.IsChecked = true;
                            Bac.IsEnabled = false;
                            BacNumero.IsEnabled = false;
                            var _rtB = App.Ds.RetraitBaccalauréat.Where(o => o.StagiaireRetrait.Equals(_rt.Id)).FirstOrDefault();
                            if (_rtB != null)
                            {
                                BacNumero.Text = _rtB.NuméroSérie;
                                _Data.Add(new RetraitView(_rt.TypeDocument, _rtB.NuméroSérie, "", _rt.DateRetrait.ToShortDateString()));
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
            Tel1Warning.Visibility = Tel2Warning.Visibility = RaisonSocialeWarning.Visibility = Visibility.Collapsed;
            Tel1Text.Text = Tel2Text.Text = RaisonSocialeWarningText.Text = "";
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
                    if (Embauche.IsChecked == null ? false : Embauche.IsChecked.Value && RaisonSociale.Text == "")
                    {
                        RaisonSocialeWarning.Visibility = Visibility.Visible;
                        RaisonSocialeWarningText.Text = "Veuillez remplir le champ raison social";
                        _isDirty = true;
                    }
                    if (_isDirty) return _isDirty;
                    try
                    {
                        DbContext.StagiaireAdapter.UpdateStagiaire(_full[3], Tel1.Text, Tel2.Text, Embauche.IsChecked == null ? (RechercheEmploi.IsChecked == null ? "Poursuite de Formation" : (RechercheEmploi.IsChecked.Value ? "Recherche d'emploi" : "Poursuite de Formation")) : (Embauche.IsChecked.Value ? "Embauche" : (RechercheEmploi.IsChecked == null ? "Poursuite de Formation" : (RechercheEmploi.IsChecked.Value ? "Recherche d'emploi" : "Poursuite de Formation"))));
                    }
                    catch
                    {
                        Main.Mad.MyErrorSnackbarIcon.MessageQueue.Enqueue("Veuillez valider tous les champs avant de continuer");
                        return true;
                    }
                    if (Embauche.IsChecked == null ? false : Embauche.IsChecked.Value)
                    {
                        var _emb = App.Ds.StagiaireEmbauche.Where(o => o.Stagiaire.Equals(Cef.Text, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(o => o.DateInsertion).FirstOrDefault();
                        if (_emb == null || !RaisonSociale.Text.Equals(_emb.RaisonSociale))
                        {
                            DbContext.StagiaireEmbaucheAdapter.Insert(Cef.Text, RaisonSociale.Text, DateDebut.SelectedDate, DateTime.Now);
                            App.Ds.StagiaireEmbauche.Clear();
                            DbContext.StagiaireEmbaucheAdapter.Fill(App.Ds.StagiaireEmbauche);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
                return true;
            }
            return false;
        }

        bool _isDirty;
        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            Tel1Warning.Visibility = Tel2Warning.Visibility = RaisonSocialeWarning.Visibility = Visibility.Collapsed;
            Tel1Text.Text = Tel2Text.Text = RaisonSocialeWarningText.Text = "";
            DiplômeNumeroWarning.Visibility = DiplômeCabWarning.Visibility = PathWarning.Visibility = BacNumeroWarning.Visibility = DateRetraitWarning.Visibility = Visibility.Collapsed;
            DiplômeNumeroWarningText.Text = DiplômeCabWarningText.Text = PathWarningText.Text = BacNumeroWarningText.Text = DateRetraitWarningText.Text = "";
            _isDirty = false;
            try
            {
                if (Diplôme.IsChecked == null ? false : Diplôme.IsChecked.Value && Diplôme.IsEnabled)
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
                if (Bac.IsChecked == null ? false : Bac.IsChecked.Value && Bac.IsEnabled)
                {
                    if (BacNumero.Text.Length < 1)
                    {
                        BacNumeroWarning.Visibility = Visibility.Visible;
                        BacNumeroWarningText.Text = "Veuillez insérer un numéro de série";
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
                var _stg = App.Ds.StagiaireGroupe.Where(o =>
                                o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.NomPrénom.Equals(_full[1], StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.Cin.Equals(_full[2], StringComparison.InvariantCultureIgnoreCase)
                                && o.Stagiaire.Equals(_full[3], StringComparison.InvariantCultureIgnoreCase)
                                ).FirstOrDefault();
                if (_stg != null)
                {
                    if (Diplôme.IsChecked == null ? false : Diplôme.IsChecked.Value && Diplôme.IsEnabled)
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
                    if (Attestation.IsChecked == null ? false : Attestation.IsChecked.Value && Attestation.IsEnabled)
                    {
                        DbContext.StagiaireRetraitAdapter.Insert(_stg.Id, "Attestation de Réussite", DateRetrait.SelectedDate.Value);
                    }
                    if (Releve.IsChecked == null ? false : Releve.IsChecked.Value && Releve.IsEnabled)
                    {
                        DbContext.StagiaireRetraitAdapter.Insert(_stg.Id, "Relevé de Notes", DateRetrait.SelectedDate.Value);
                    }
                    if (Bac.IsChecked == null ? false : Bac.IsChecked.Value && Bac.IsEnabled)
                    {
                        DbContext.StagiaireRetraitAdapter.Insert(_stg.Id, "Baccalauréat", DateRetrait.SelectedDate.Value);
                        App.Ds.StagiaireRetrait.Clear();
                        DbContext.StagiaireRetraitAdapter.Fill(App.Ds.StagiaireRetrait);
                        var _stgR = App.Ds.StagiaireRetrait.Where(o =>
                            o.StagiaireGroupe == _stg.Id
                            && o.TypeDocument == "Baccalauréat"
                            && o.DateRetrait == DateRetrait.SelectedDate.Value).FirstOrDefault();
                        if (_stgR != null)
                        {
                            DbContext.RetraitBaccalauréatAdapter.Insert(_stgR.Id, BacNumero.Text);
                        }
                    }
                    DbContext.ReFillRetrait();

                    _stg = App.Ds.StagiaireGroupe.Where(o =>
                                o.GroupeRow.FilièreAnnéeRow.Filière.Equals(_full[0].Substring(0, _full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.GroupeRow.Numéro.Equals(_full[0].Substring(_full[0].Length - 3), StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.NomPrénom.Equals(_full[1], StringComparison.InvariantCultureIgnoreCase)
                                && o.StagiaireRow.Cin.Equals(_full[2], StringComparison.InvariantCultureIgnoreCase)
                                && o.Stagiaire.Equals(_full[3], StringComparison.InvariantCultureIgnoreCase)
                                ).FirstOrDefault();
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

        private void Tel1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (((TextBox)sender).Text.Length > 0)
            //{
            //    Modifier.IsEnabled = true;
            //    Valider.IsEnabled = true;
            //}

        }

        private void Embauche_Checked(object sender, RoutedEventArgs e)
        {
            if (RaisonSociale != null)
            {
                RaisonSocialeStack.Visibility = DateDebut.Visibility = Visibility.Visible;
                //Modifier.IsEnabled = true;
            }
        }

        private void Embauche_Unchecked(object sender, RoutedEventArgs e)
        {
            RaisonSocialeStack.Visibility = DateDebut.Visibility = Visibility.Collapsed;
            //Modifier.IsEnabled = true;
        }

        private void SearchByBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (SearchByBox.Text.Length < 1)
            {
                Modifier.IsEnabled = false;
                Valider.IsEnabled = false;

                NomPrenom.Text = Cin.Text = Cef.Text = Groupe.Text = Admis.Text = État.Text = Tel1.Text = Tel2.Text = RaisonSociale.Text = "";
                DateDebut.SelectedDate = null;
                Embauche.IsChecked = true;


                DataGridData.ItemsSource = null;
                Releve.IsChecked = false;
                Bac.IsChecked = false;
                DiplômeNumero.Text = "";
                DiplômeCab.Text = "";
                BacNumero.Text = "";
                Diplôme.IsEnabled = true;
                Attestation.IsEnabled = true;
                Releve.IsEnabled = true;
                Bac.IsEnabled = true;
                BacNumero.IsEnabled = true;
                DiplômeNumero.IsEnabled = true;
                DiplômeCab.IsEnabled = true;
                Path.Text = "";
            }
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
