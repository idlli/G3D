using ClosedXML.Excel;
using G3D.Domain;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace G3D.Duplicata
{
    /// <summary>
    /// Interaction logic for Duplicata.xaml
    /// </summary>
    public partial class Duplicatas : UserControl
    {
        public static Duplicatas Dup;

        public string ShortGroupe { get; set; }
        public Duplicatas()
        {
            InitializeComponent();
            Dup = this;
            
            try
            {
                CheckContent();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }


        }

        BackgroundWorker Bw = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        void CheckContent()
        {
            if (Bw.IsBusy)
            {
                return;
            }

            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();

            Bw.DoWork += (bwSender, bwArg) =>
            {

                sWatch.Start();
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Main.Mad.MainProgressBar.Visibility = Visibility.Visible;
                    DataGridData.DataContext = new DuplicataViewModel();
                }));
            };

            Bw.RunWorkerCompleted += (bwSender, bwArg) =>
            {
                sWatch.Stop();
                Bw.Dispose();
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Main.Mad.MainProgressBar.Visibility = Visibility.Collapsed;
                    if (App.Ds.Duplicata.Count > 0)
                    {
                        if (FirstPanel.Visibility == Visibility.Visible)
                        {
                            FirstPanel.Visibility = Visibility.Collapsed;
                            SecondPanel.Visibility = Visibility.Visible;
                        }
                    }
                }));
            };
            Bw.RunWorkerAsync();
        }

        private void ExcelPathLocate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        //string[] files = Directory.GetFiles(fbd.SelectedPath);
                        ExcelPath.Text = fbd.SelectedPath;
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        char[] invalidChars = Path.GetInvalidFileNameChars();
        bool _isDirty;

        private void ExcelExportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isDirty = false;

                if (string.IsNullOrWhiteSpace(ExcelPath.Text))
                {
                    ExcelPathLocateWarning.Visibility = Visibility.Visible;
                    ExcelPathLocateWarningText.Text = "Veuillez sélectionner un chemin de dossier pour enregistrer le fichier";
                    _isDirty = true;
                }
                else if (!Directory.Exists(ExcelPath.Text.Trim()))
                {
                    try
                    {
                        Directory.CreateDirectory(ExcelPath.Text.Trim());
                    }
                    catch
                    {
                        ExcelPathLocateWarning.Visibility = Visibility.Visible;
                        ExcelPathLocateWarningText.Text = "Le chemin du dossier que vous essayez d'utiliser n'est pas dans un format valide";
                        _isDirty = true;
                    }
                }
                if (_isDirty) return;
                if (!string.IsNullOrWhiteSpace(ExcelFileTitle.Text) && ExcelFileTitle.Text.IndexOfAny(invalidChars) > -1)
                {
                    ExcelFileTitleWarning.Visibility = Visibility.Visible;
                    ExcelFileTitleWarningText.Text = "Ce nom de fichier n'est pas valide";
                    return;
                }

                DataTable dt = new DataTable();

                foreach (DataGridColumn column in DataGridData.Columns)
                {
                    if (DataGridData.Columns[0].Equals(column)) continue;
                    dt.Columns.Add(column.Header.ToString().Trim());
                }

                foreach (DuplicataModel row in DuplicataViewModel._Data.Where(o => o.IsSelected))
                {
                    dt.Rows.Add(row.Cef, row.NomPrénom, row.Cin, row.Niveau, row.TypeFormation, row.ModeFormation, row.TypeStagiaires, row.AnnéeFormation, row.Filière, row.Groupe, row.Établissement);
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    try
                    {
                        wb.Worksheets.Add(dt, "G3D");
                        wb.Worksheet(1).Cells("A1:C1").Style.Font.Bold = true;
                        wb.Worksheet(1).Columns().AdjustToContents();
                        wb.SaveAs(Path.Combine(ExcelPath.Text.Trim(), $"{(string.IsNullOrWhiteSpace(ExcelFileTitle.Text) ? DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH'-'mm'-'ss") : ExcelFileTitle.Text)}.xlsx"));

                    }
                    catch (Exception ex)
                    {
                        Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
                        return;
                    }
                }
                Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue("Le fichier excel a été exporté avec succès");
                ExcelPathLocateWarning.Visibility = ExcelFileTitleWarning.Visibility = Visibility.Collapsed;
                ExcelPathLocateWarningText.Text = ExcelFileTitleWarningText.Text = "";
                ExcelPath.Text = ExcelFileTitle.Text = "";
                MainDialog.IsOpen = false;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                var _tempData = DuplicataViewModel._Data.Where(o => o.IsSelected).ToList();
                int i = 0;
                foreach (var item in _tempData)
                {
                    try
                    {
                        // Deleted Just Dupliacat Record And Not the Groupe if Added
                        DbContext.DuplicataAdapter.DeleteQuery(item.DupId);
                        DuplicataViewModel._Data.Remove(item);
                    }
                    catch
                    {
                        i++;
                    }
                }
                if (i > 0)
                {
                    Main.Mad.MyAlertSnackbarIcon.MessageQueue.Enqueue($"({_tempData.Count - i}) sur ({_tempData.Count}) duplicatas sélectionnés ont été supprimés car les duplicatas non supprimés sont en cours d'utilisation au niveau de la base de données");
                }
                else
                {
                    Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue($"{_tempData.Count} duplicatas ont été supprimés avec succès");
                    GridCheckBoxColumn.IsChecked = false;
                    MainDialog.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        ObservableCollection<DuplicataModel> _tempData;
        private void RechercherDuplicata_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string txt = RechercherDuplicata.Text.ToLower().Trim();
                if (txt.Length > 0)
                {
                    _tempData = new ObservableCollection<DuplicataModel>();
                    foreach (DuplicataModel item in DuplicataViewModel._Data)
                    {
                        if (item.Cef.Contains(txt)
                            || item.NomPrénom.ToLower().Contains(txt)
                            || item.Cin.ToLower().Contains(txt))
                        {
                            _tempData.Add(item);
                        }
                    }
                    if (_tempData.Count > 0)
                    {
                        DataGridData.ItemsSource = _tempData;
                    }
                }
                else
                {
                    if (DuplicataViewModel._Data.Count != DataGridData.Items.Count)
                    {
                        DataGridData.ItemsSource = DuplicataViewModel._Data;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        bool _inUsing;

        void AMS_Clear()
        {
            try
            {
                Etab.Text = ShortGroupe = Groupe.Text = SearchByCef.Text = Nom.Text = Cin.Text = Lieu.Text = NSerie.Text = Cab.Text = Session.Text = Tel1.Text = Tel2.Text = ModeF.Text = "";
                //EtabCode.ItemsSource = null;
                //Niveau.ItemsSource = null;
                //Filiere.ItemsSource = null;
                //Annee.ItemsSource = null;
                //TypeS.ItemsSource = null;
                //TypeF.ItemsSource = null;
                DateNaissance.SelectedDate = null;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }
        Hashtable MyHashtable = new Hashtable();

        private void AjouterStagiaire_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                AMDuplicata.Visibility = Visibility.Visible;
                DAButton.Visibility = Visibility.Visible;
                DMButton.Visibility = Visibility.Collapsed;

                ElementsRequired.Visibility = Visibility.Collapsed;
                DeleteWarningDialog.Visibility = Visibility.Collapsed;
                ExcelExport.Visibility = Visibility.Collapsed;


                _inUsing = true;


                AMS_Clear();



                //ShortGroupe = "ddd";

                SearchByCef.ItemsSource = App.Ds.Stagiaire.Select(o => o.Cef).Distinct();

                EtabCode.ItemsSource = App.Ds.Établissement;
                EtabCode.DisplayMemberPath = "Code";
                EtabCode.SelectedValuePath = "Code";

                Annee.ItemsSource = App.Ds.FilièreAnnée.Select(o => o.AnnéeFormation).Distinct();

                Niveau.ItemsSource = App.Ds.Niveau;
                Niveau.DisplayMemberPath = "Nom";
                Niveau.SelectedValuePath = "NomCourt";

                TypeS.ItemsSource = App.Ds.Groupe.Select(o => o.TypeStagiairesRow).Distinct();
                TypeS.DisplayMemberPath = "Nom";
                TypeS.SelectedValuePath = "NomCourt";

                TypeF.ItemsSource = App.Ds.Groupe.Select(o => o.TypeFormation).Distinct();

                Filiere.ItemsSource = App.Ds.Filière;
                Filiere.DisplayMemberPath = "Nom";
                Filiere.SelectedValuePath = "NomCourt";

                _inUsing = false;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void ModifierDuplicata_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AMDuplicata.Visibility = Visibility.Visible;
                DAButton.Visibility = Visibility.Collapsed;
                DMButton.Visibility = Visibility.Visible;

                ElementsRequired.Visibility = Visibility.Collapsed;
                DeleteWarningDialog.Visibility = Visibility.Collapsed;
                ExcelExport.Visibility = Visibility.Collapsed;

                _inUsing = true;

                AMS_Clear();

                SearchByCef.IsEnabled = EtabCode.IsEnabled = Annee.IsEnabled = false;

                var _stagiaire = DuplicataViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                if (_stagiaire != null)
                {
                    SearchByCef.Text = _stagiaire.Cef;



                    Cin.Text = _stagiaire.Cin;
                    Nom.Text = _stagiaire.NomPrénom;

                    var etab = App.Ds.Établissement.Where(o => o.Nom.Equals(_stagiaire.Établissement, StringComparison.InvariantCultureIgnoreCase));
                    EtabCode.Items.Add(etab.Select(o => o.Code).FirstOrDefault());
                    EtabCode.Text = etab.Select(o => o.Code).FirstOrDefault();
                    Etab.Text = _stagiaire.Établissement;

                    Annee.Items.Add(_stagiaire.AnnéeFormation);
                    Annee.SelectedItem = _stagiaire.AnnéeFormation;

                    Niveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreRow.NiveauRow).Distinct();

                    Niveau.DisplayMemberPath = "Nom";
                    Niveau.SelectedValuePath = "NomCourt";
                    Niveau.Text = _stagiaire.Niveau;

                    TypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeStagiairesRow).Distinct();
                    TypeS.DisplayMemberPath = "Nom";
                    TypeS.SelectedValuePath = "NomCourt";
                    TypeS.Text = _stagiaire.TypeStagiaires;

                    TypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeFormation).Distinct();
                    TypeF.SelectedItem = _stagiaire.TypeFormation;

                    Filiere.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreRow).Distinct();
                    Filiere.DisplayMemberPath = "Nom";
                    Filiere.SelectedValuePath = "NomCourt";
                    Filiere.Text = _stagiaire.Filière;

                    Groupe.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement) && string.Equals(o.FilièreAnnéeRow.Filière, Filiere.SelectedValue.ToString())).Select(o => o.Numéro).Distinct();
                    Groupe.Text = _stagiaire.Groupe;

                    DateNaissance.SelectedDate = _stagiaire.DateNaissance;
                    Lieu.Text = _stagiaire.Lieu;
                    NSerie.Text = _stagiaire.NSerie;
                    Cab.Text = _stagiaire.Cab;
                    Session.Text = _stagiaire.Session;
                    Tel1.Text = _stagiaire.Tel1;
                    Tel2.Text = _stagiaire.Tel2;
                    ModeF.Text = _stagiaire.ModeFormation;

                }
                _inUsing = false;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void ADuplicata_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isDirty = false;
                EtabCodeWarning.Visibility = NiveauWarning.Visibility = FiliereWarning.Visibility = GroupeWarning.Visibility = SearchByCefWarning.Visibility = AnneeWarning.Visibility = TypeSWarning.Visibility = NomWarning.Visibility = CinWarning.Visibility = DateNaissanceWarning.Visibility = LieuWarning.Visibility = NSerieWarning.Visibility = CabWarning.Visibility = SessionWarning.Visibility = ModeFWarning.Visibility = TypeFWarning.Visibility = Tel1Warning.Visibility = Visibility.Collapsed;
                EtabCodeWarningText.Text = NiveauWarningText.Text = FiliereWarningText.Text = GroupeWarningText.Text = SearchByCefWarningText.Text = AnneeWarningText.Text = TypeSWarningText.Text = NomWarningText.Text = CinWarningText.Text = DateNaissanceWarningText.Text = LieuWarningText.Text = NSerieWarningText.Text = CabWarningText.Text = SessionWarningText.Text = ModeFWarningText.Text = Tel1WarningText.Text = TypeFWarningText.Text = "";

                if (EtabCode.SelectedIndex == -1)
                {
                    EtabCodeWarning.Visibility = Visibility.Visible;
                    EtabCodeWarningText.Text = "Veuillez sélectionner une établissement";
                    _isDirty = true;
                }

                if (Annee.Text == "")
                {
                    AnneeWarning.Visibility = Visibility.Visible;
                    AnneeWarningText.Text = "Veuillez sélectionner une année de formation";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(SearchByCef.Text))
                {
                    SearchByCefWarning.Visibility = Visibility.Visible;
                    SearchByCefWarningText.Text = "Veuillez remplir un cef";
                    _isDirty = true;
                }

                if (Niveau.SelectedIndex == -1)
                {
                    NiveauWarning.Visibility = Visibility.Visible;
                    NiveauWarningText.Text = "Veuillez sélectionner un niveau";
                    _isDirty = true;
                }

                if (Filiere.Text == "")
                {
                    FiliereWarning.Visibility = Visibility.Visible;
                    FiliereWarningText.Text = "Veuillez sélectionner une filière";
                    _isDirty = true;
                }

                if (Groupe.Text == "")
                {
                    GroupeWarning.Visibility = Visibility.Visible;
                    GroupeWarningText.Text = "Veuillez sélectionner un groupe";
                    _isDirty = true;
                }

                if (TypeS.SelectedIndex == -1)
                {
                    TypeSWarning.Visibility = Visibility.Visible;
                    TypeSWarningText.Text = "Veuillez sélectionner un type de stagiaire";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Nom.Text) || Nom.Text.Trim().Length < 6)
                {
                    NomWarning.Visibility = Visibility.Visible;
                    NomWarningText.Text = "Veuillez définir un nom valide";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Cin.Text))
                {
                    CinWarning.Visibility = Visibility.Visible;
                    CinWarningText.Text = "Veuillez définir un cin valide";
                    _isDirty = true;
                }
                else if (App.Ds.Stagiaire.Where(o => string.Equals(o.Cin.ToLower(), Cin.Text.ToLower().Trim()) && !string.Equals(o.Cef, SearchByCef.Text)).Any())
                {
                    CinWarning.Visibility = Visibility.Visible;
                    CinWarningText.Text = "Ce cin déjà utilisé avec un autre stagiaire";
                    _isDirty = true;
                }

                if (DateNaissance.SelectedDate == null)
                {
                    DateNaissanceWarning.Visibility = Visibility.Visible;
                    DateNaissanceWarningText.Text = "Veuillez sélectionner une date de naissance";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Lieu.Text))
                {
                    LieuWarning.Visibility = Visibility.Visible;
                    LieuWarningText.Text = "Veuillez définir un lieu";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(NSerie.Text))
                {
                    NSerieWarning.Visibility = Visibility.Visible;
                    NSerieWarningText.Text = "Veuillez définir un numéro de diplôme";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Cab.Text))
                {
                    CabWarning.Visibility = Visibility.Visible;
                    CabWarningText.Text = "Veuillez définir un CAB";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Session.Text))
                {
                    SessionWarning.Visibility = Visibility.Visible;
                    SessionWarningText.Text = "Veuillez définir une session";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Tel1.Text))
                {
                    Tel1Warning.Visibility = Visibility.Visible;
                    Tel1WarningText.Text = "Veuillez définir un premier numéro de téléphone";
                    _isDirty = true;
                }

                if (ModeF.Text == "")
                {
                    ModeFWarning.Visibility = Visibility.Visible;
                    ModeFWarningText.Text = "Veuillez sélectionner un mode de formation";
                    _isDirty = true;
                }

                if (TypeF.Text == "")
                {
                    TypeFWarning.Visibility = Visibility.Visible;
                    TypeFWarningText.Text = "Veuillez sélectionner un type de formation";
                    _isDirty = true;
                }

                if (_isDirty) return;


                try
                {
                    DbContext.QueriesAdapter.InsertDuplicataRow
                        (
                            Filiere.SelectedValue.ToString(),
                            Filiere.Text,
                            Niveau.SelectedValue.ToString(),
                            EtabCode.SelectedValue.ToString(),
                            Annee.Text,
                            Session.Text,
                            Groupe.Text,
                            ModeF.Text,
                            TypeF.Text,
                            TypeS.SelectedValue.ToString(),
                            SearchByCef.Text,
                            Nom.Text,
                            Cin.Text,
                            Tel1.Text,
                            Tel2.Text,
                            DateNaissance.SelectedDate,
                            Lieu.Text,
                            NSerie.Text,
                            Cab.Text
                        );
                    DbContext.ReFillDuplicata();
                    Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue("Duplicata a été ajouté avec succès");
                    AMS_Clear();
                    //MainDialog.IsOpen = false;
                }
                catch
                {
                    Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue("Nous sommes désolés mais nous ne pouvions pas ajouter un duplicata pour le moment");
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void MDuplicata_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isDirty = false;
                EtabCodeWarning.Visibility = NiveauWarning.Visibility = FiliereWarning.Visibility = GroupeWarning.Visibility = SearchByCefWarning.Visibility = AnneeWarning.Visibility = TypeSWarning.Visibility = NomWarning.Visibility = CinWarning.Visibility = DateNaissanceWarning.Visibility = LieuWarning.Visibility = NSerieWarning.Visibility = CabWarning.Visibility = SessionWarning.Visibility = ModeFWarning.Visibility = Tel1Warning.Visibility = TypeFWarning.Visibility = Visibility.Collapsed;
                EtabCodeWarningText.Text = NiveauWarningText.Text = FiliereWarningText.Text = GroupeWarningText.Text = SearchByCefWarningText.Text = AnneeWarningText.Text = TypeSWarningText.Text = NomWarningText.Text = CinWarningText.Text = DateNaissanceWarningText.Text = LieuWarningText.Text = NSerieWarningText.Text = CabWarningText.Text = SessionWarningText.Text = ModeFWarningText.Text = Tel1WarningText.Text = TypeFWarningText.Text = "";

                if (Niveau.SelectedIndex == -1)
                {
                    NiveauWarning.Visibility = Visibility.Visible;
                    NiveauWarningText.Text = "Veuillez sélectionner un niveau";
                    _isDirty = true;
                }

                if (Filiere.Text == "")
                {
                    FiliereWarning.Visibility = Visibility.Visible;
                    FiliereWarningText.Text = "Veuillez sélectionner une filière";
                    _isDirty = true;
                }

                if (Groupe.Text == "")
                {
                    GroupeWarning.Visibility = Visibility.Visible;
                    GroupeWarningText.Text = "Veuillez sélectionner un groupe";
                    _isDirty = true;
                }

                if (TypeS.SelectedIndex == -1)
                {
                    TypeSWarning.Visibility = Visibility.Visible;
                    TypeSWarningText.Text = "Veuillez sélectionner un type de stagiaire";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Nom.Text) || Nom.Text.Trim().Length < 6)
                {
                    NomWarning.Visibility = Visibility.Visible;
                    NomWarningText.Text = "Veuillez définir un nom valide";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Cin.Text))
                {
                    CinWarning.Visibility = Visibility.Visible;
                    CinWarningText.Text = "Veuillez définir un cin valide";
                    _isDirty = true;
                }
                else if (App.Ds.Stagiaire.Where(o => string.Equals(o.Cin.ToLower(), Cin.Text.ToLower().Trim()) && !string.Equals(o.Cef, SearchByCef.Text)).Any())
                {
                    CinWarning.Visibility = Visibility.Visible;
                    CinWarningText.Text = "Ce cin déjà utilisé avec un autre trainer";
                    _isDirty = true;
                }

                if (DateNaissance.SelectedDate == null)
                {
                    DateNaissanceWarning.Visibility = Visibility.Visible;
                    DateNaissanceWarningText.Text = "Veuillez sélectionner une date de naissance";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Lieu.Text))
                {
                    LieuWarning.Visibility = Visibility.Visible;
                    LieuWarningText.Text = "Veuillez définir un lieu";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(NSerie.Text))
                {
                    NSerieWarning.Visibility = Visibility.Visible;
                    NSerieWarningText.Text = "Veuillez définir un numéro de diplôme";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Cab.Text))
                {
                    CabWarning.Visibility = Visibility.Visible;
                    CabWarningText.Text = "Veuillez définir un CAB";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Session.Text))
                {
                    SessionWarning.Visibility = Visibility.Visible;
                    SessionWarningText.Text = "Veuillez définir une session";
                    _isDirty = true;
                }

                if (string.IsNullOrWhiteSpace(Tel1.Text))
                {
                    Tel1Warning.Visibility = Visibility.Visible;
                    Tel1WarningText.Text = "Veuillez définir un premier numéro de téléphone";
                    _isDirty = true;
                }

                if (ModeF.Text == "")
                {
                    ModeFWarning.Visibility = Visibility.Visible;
                    ModeFWarningText.Text = "Veuillez sélectionner un mode de formation";
                    _isDirty = true;
                }

                if (TypeF.Text == "")
                {
                    TypeFWarning.Visibility = Visibility.Visible;
                    TypeFWarningText.Text = "Veuillez sélectionner un type de formation";
                    _isDirty = true;
                }

                if (_isDirty) return;

                var _stagiaire = DuplicataViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();

                if (_stagiaire.Cin.Equals(Cin.Text.Trim())
                    && _stagiaire.NomPrénom.Equals(Nom.Text.Trim())
                    && _stagiaire.Niveau.Equals(Niveau.Text)
                    && _stagiaire.TypeStagiaires.Equals(TypeS.Text)
                    && _stagiaire.Filière.Equals(Filiere.Text)
                    && _stagiaire.Groupe.Equals(Groupe.Text)
                    && _stagiaire.DateNaissance.Equals(DateNaissance.SelectedDate)
                    && _stagiaire.Lieu.Equals(Lieu.Text)
                    && _stagiaire.NSerie.Equals(NSerie.Text)
                    && _stagiaire.Cab.Equals(Cab.Text)
                    && _stagiaire.ModeFormation.Equals(ModeF.Text)
                    && _stagiaire.Tel1.Equals(Tel1.Text)
                    && _stagiaire.Tel2.Equals(Tel2.Text)
                    && _stagiaire.Session.Equals(Session.Text)
                    && _stagiaire.TypeFormation.Equals(TypeF.Text)
                    )

                {
                    Main.Mad.MyAlertSnackbarIcon.MessageQueue.Enqueue("Tu ne change rien");
                    return;
                }
                int? _filiereId = App.Ds.FilièreAnnée.Where(o => o.ÉtablissementRow.Code.Equals(EtabCode.Text) && o.AnnéeFormation.Equals(Annee.Text) && o.Filière.Equals(Filiere.SelectedValue.ToString(), StringComparison.InvariantCultureIgnoreCase) && o.FilièreRow.Niveau.Equals(Niveau.SelectedValue.ToString())).Select(o => o.Id).FirstOrDefault();
                if (_filiereId > 0)
                {
                    int? _groupeId = App.Ds.Groupe.Where(o => o.FilièreAnnée.Equals(_filiereId) && o.Numéro.Equals(Groupe.Text) && string.Equals(o.TypeStagiaires, TypeS.SelectedValue.ToString())).Select(o => o.Id).FirstOrDefault();
                    if (_groupeId > 0)
                    {
                        int? _StagaireGroupeId = App.Ds.StagiaireGroupe.Where(o => o.Groupe.Equals(_groupeId) && o.Stagiaire.Equals(SearchByCef.Text)).Select(o => o.Id).FirstOrDefault();
                        if (_StagaireGroupeId > 0)
                        {
                            try
                            {
                                DbContext.DuplicataAdapter.UpdateDuplicata(_stagiaire.DupId, _StagaireGroupeId, DateNaissance.SelectedDate, Lieu.Text, NSerie.Text, Cab.Text, SearchByCef.Text, Nom.Text, Cin.Text, Tel1.Text, Tel2.Text, ModeF.Text);

                                Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue("Duplicata a été modifié avec succès");

                                DbContext.ReFillDuplicata();
                                CheckContent();
                            }
                            catch
                            {
                                Main.Mad.MyErrorSnackbarIcon.MessageQueue.Enqueue("Désolé mais nous ne pouvons pas modifier ce stagiaire pour le moment");
                            }
                        }
                        else
                        {
                            try
                            {
                                DbContext.StagiaireGroupeAdapter.Insert(SearchByCef.Text, _groupeId, "Oui", null);
                                MDuplicata_Click(sender, e);
                            }
                            catch (Exception ex)
                            {
                                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            DbContext.GroupeAdapter.Insert(_filiereId, Groupe.Text, "2A", TypeF.Text, TypeS.Text);
                            MDuplicata_Click(sender, e);
                        }
                        catch (Exception ex)
                        {
                            Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
                        }
                    }
                }
                else
                {
                    try
                    {
                        DbContext.FilièreAnnéeAdapter.Insert(EtabCode.Text, Filiere.SelectedValue.ToString(), Annee.Text, Session.Text);
                        MDuplicata_Click(sender, e);
                    }
                    catch (Exception ex)
                    {
                        Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
                    }
                };
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AMDuplicata.Visibility = Visibility.Collapsed;
            DAButton.Visibility = Visibility.Collapsed;
            DMButton.Visibility = Visibility.Collapsed;
        }

        private void EtabCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && EtabCode.SelectedValue != null)
                {
                    var _Etab = App.Ds.Établissement.Where(o => o.Code.Equals(EtabCode.SelectedValue.ToString())).FirstOrDefault();
                    if (_Etab != null) Etab.Text = _Etab.Nom;
                    Niveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Établissement, EtabCode.SelectedValue.ToString())).Select(o => o.FilièreRow.NiveauRow).Distinct();
                    Filiere.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Établissement, EtabCode.SelectedValue.ToString())).Select(o => o.FilièreRow).Distinct();
                    Annee.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Établissement, EtabCode.SelectedValue.ToString())).Select(o => o.AnnéeFormation).Distinct();
                    TypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Établissement, EtabCode.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                    TypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Établissement, EtabCode.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                    if (Filiere.SelectedIndex == -1)
                    {
                        Groupe.ItemsSource = null;
                    }
                    else
                    {
                        Filiere_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Niveau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && Niveau.SelectedValue != null)
                {
                    if (DMButton.Visibility == Visibility.Visible)
                    {
                        var _duplicata = DuplicataViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                        TypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.FilièreRow.Niveau, Niveau.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.TypeStagiairesRow).Distinct();
                        TypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.FilièreRow.Niveau, Niveau.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.TypeFormation).Distinct();
                        Filiere.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.FilièreRow.Niveau, Niveau.SelectedValue.ToString()) && string.Equals(o.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.FilièreRow).Distinct();
                    }
                    else
                    {
                        EtabCode.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.FilièreRow.Niveau, Niveau.SelectedValue.ToString())).Select(o => o.ÉtablissementRow).Distinct();
                        Filiere.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.FilièreRow.Niveau, Niveau.SelectedValue.ToString())).Select(o => o.FilièreRow).Distinct();
                        SearchByCef.ItemsSource = App.Ds.StagiaireGroupe.Where(o => string.Equals(o.GroupeRow.FilièreAnnéeRow.FilièreRow.Niveau, Niveau.SelectedValue.ToString())).Select(o => o.Stagiaire).Distinct();
                        Annee.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.FilièreRow.Niveau, Niveau.SelectedValue.ToString())).Select(o => o.AnnéeFormation).Distinct();
                        TypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.FilièreRow.Niveau, Niveau.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                        TypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.FilièreRow.Niveau, Niveau.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                    }
                    if (Filiere.SelectedIndex == -1)
                    {
                        Groupe.ItemsSource = null;
                    }
                    else
                    {
                        Filiere_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Filiere_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && Filiere.SelectedValue != null)
                {
                    if (DMButton.Visibility == Visibility.Visible)
                    {
                        var _duplicata = DuplicataViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                        Niveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Filière, Filiere.SelectedValue.ToString()) && string.Equals(o.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.FilièreRow.NiveauRow).Distinct();
                        TypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, Filiere.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.TypeFormation).Distinct();
                        TypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, Filiere.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.TypeStagiairesRow).Distinct();
                        Groupe.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, Filiere.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.Numéro).Distinct();
                    }
                    else
                    {
                        EtabCode.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Filière, Filiere.SelectedValue.ToString())).Select(o => o.ÉtablissementRow).Distinct();
                        Niveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Filière, Filiere.SelectedValue.ToString())).Select(o => o.FilièreRow.NiveauRow).Distinct();
                        Groupe.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, Filiere.SelectedValue.ToString())).Select(o => o.Numéro).Distinct();
                        SearchByCef.ItemsSource = App.Ds.StagiaireGroupe.Where(o => string.Equals(o.GroupeRow.FilièreAnnéeRow.Filière, Filiere.SelectedValue.ToString())).Select(o => o.Stagiaire).Distinct();
                        Annee.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Filière, Filiere.SelectedValue.ToString())).Select(o => o.AnnéeFormation).Distinct();
                        TypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, Filiere.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                        TypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, Filiere.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void SearchByCef_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && SearchByCef.SelectedItem != null)
                {
                    var _stg = App.Ds.Stagiaire.Where(o => string.Equals(o.Cef, SearchByCef.SelectedItem.ToString().Trim())).FirstOrDefault();
                    if (_stg != null)
                    {
                        string _NuméroTéléphonePremier, _NuméroTéléphoneDeuxième;
                        try { _NuméroTéléphonePremier = _stg.NuméroTéléphonePremier; } catch { _NuméroTéléphonePremier = ""; }
                        try { _NuméroTéléphoneDeuxième = _stg.NuméroTéléphoneDeuxième; } catch { _NuméroTéléphoneDeuxième = ""; }
                        Cin.Text = _stg.Cin;
                        Nom.Text = _stg.NomPrénom;
                        Tel1.Text = _NuméroTéléphonePremier;
                        Tel2.Text = _NuméroTéléphoneDeuxième;
                    }
                    else
                    {
                        Cin.Text = "";
                        Nom.Text = "";
                        Tel1.Text = "";
                        Tel2.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Annee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && Annee.SelectedValue != null)
                {
                    EtabCode.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.AnnéeFormation, Annee.SelectedValue.ToString())).Select(o => o.ÉtablissementRow).Distinct();
                    Niveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.AnnéeFormation, Annee.SelectedValue.ToString())).Select(o => o.FilièreRow.NiveauRow).Distinct();
                    TypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.AnnéeFormation, Annee.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                    SearchByCef.ItemsSource = App.Ds.StagiaireGroupe.Where(o => string.Equals(o.GroupeRow.FilièreAnnéeRow.AnnéeFormation, Annee.SelectedValue.ToString())).Select(o => o.Stagiaire).Distinct();
                    TypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.AnnéeFormation, Annee.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                    Filiere.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.AnnéeFormation, Annee.SelectedValue.ToString())).Select(o => o.FilièreRow).Distinct();
                    if (Filiere.SelectedIndex == -1)
                    {
                        Groupe.ItemsSource = null;
                    }
                    else
                    {
                        Filiere_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void TypeS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && TypeS.SelectedValue != null)
                {
                    if (DMButton.Visibility == Visibility.Visible)
                    {
                        var _duplicata = DuplicataViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                        Niveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, TypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        TypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, TypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.TypeFormation).Distinct();
                        Filiere.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, TypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                    }
                    else
                    {
                        EtabCode.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, TypeS.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.ÉtablissementRow).Distinct();
                        Annee.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, TypeS.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.AnnéeFormation).Distinct();
                        Niveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, TypeS.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        TypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, TypeS.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                        Filiere.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, TypeS.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                        SearchByCef.ItemsSource = App.Ds.StagiaireGroupe.Where(o => string.Equals(o.GroupeRow.TypeStagiaires, TypeS.SelectedValue.ToString())).Select(o => o.Stagiaire).Distinct();
                    }
                    if (Filiere.SelectedIndex == -1)
                    {
                        Groupe.ItemsSource = null;
                    }
                    else
                    {
                        Filiere_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void TypeF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && TypeF.SelectedValue != null)
                {
                    if (DMButton.Visibility == Visibility.Visible)
                    {
                        var _duplicata = DuplicataViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                        Niveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, TypeF.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        TypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, TypeF.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.TypeStagiairesRow).Distinct();
                        Filiere.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, TypeF.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _duplicata.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _duplicata.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                    }
                    else
                    {
                        EtabCode.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, TypeF.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.ÉtablissementRow).Distinct();
                        Annee.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, TypeF.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.AnnéeFormation).Distinct();
                        Niveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, TypeF.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        TypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, TypeF.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                        Filiere.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, TypeF.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                        SearchByCef.ItemsSource = App.Ds.StagiaireGroupe.Where(o => string.Equals(o.GroupeRow.TypeFormation, TypeF.SelectedValue.ToString())).Select(o => o.Stagiaire).Distinct();
                    }
                    if (Filiere.SelectedIndex == -1)
                    {
                        Groupe.ItemsSource = null;
                    }
                    else
                    {
                        Filiere_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AMDuplicata.Visibility = Visibility.Collapsed;
            DAButton.Visibility = Visibility.Collapsed;
            DMButton.Visibility = Visibility.Collapsed;

            CheckContent();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            ElementsRequired.Visibility = Visibility.Collapsed;
            DeleteWarningDialog.Visibility = Visibility.Collapsed;
            ExcelExport.Visibility = Visibility.Visible;
        }

        private void Fiche_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _str = DuplicataViewModel._Data.Where(o => o.IsSelected).Select(o => $"{o.NomPrénom} ~ {o.Cef} ~ [{o.DupId}]").FirstOrDefault();
                if (_str != null) Menu.FicheText = _str;
                Menu.Med.Fiche.IsSelected = true;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void SupprimerDuplicatas_Click(object sender, RoutedEventArgs e)
        {
            ElementsRequired.Visibility = Visibility.Collapsed;
            DeleteWarningDialog.Visibility = Visibility.Visible;
            ExcelExport.Visibility = Visibility.Collapsed;

            try
            {
                DeleteWarningDialogCount.Text = DuplicataViewModel._Data.Where(o => o.IsSelected).Count().ToString();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ElementsRequired.Visibility = Visibility.Visible;
            DeleteWarningDialog.Visibility = Visibility.Collapsed;
            ExcelExport.Visibility = Visibility.Collapsed;
        }
    }
}
