using ClosedXML.Excel;
using G3D.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for ImporterDesStagiaires.xaml
    /// </summary>
    public partial class ErrorGridView
    {
        public string Cef { get; set; }
        public string Message { get; set; }

        public ErrorGridView(string cef, string message)
        {
            Cef = cef;
            Message = message;
        }
    }
    public partial class Importer : UserControl
    {
        string _Path;
        public static Importer Imp;
        ObservableCollection<SelectableViewModel> _tempData;
        List<ErrorGridView> _errorGridView = new List<ErrorGridView>();

        public Importer()
        {
            InitializeComponent();

            Imp = this;

            try
            {
                if (Bw2.IsBusy)
                {
                    return;
                }

                System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();

                Bw2.DoWork += (bwSender, bwArg) =>
                {

                    sWatch.Start();
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        DataGridData.DataContext = new GridViewModel();
                    }));
                };

                Bw2.RunWorkerCompleted += (bwSender, bwArg) =>
                {
                    sWatch.Stop();
                    Bw2.Dispose();
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Main.Mad.MainProgressBar.Visibility = Visibility.Collapsed;
                        if (App.Ds.Stagiaire.Count > 0)
                        {
                            CheckContent();
                        }
                    }));
                };
                Bw2.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        void CheckContent()
        {
            if (WelcomePanel.Visibility == Visibility.Visible)
            {
                WelcomePanel.Visibility = Visibility.Collapsed;
                MainPanel.Visibility = Visibility.Visible;
            }
        }
        private void ImportBorder_MouseEnter(object sender, EventArgs e)
        {
            ((SolidColorBrush)ImportBorder.Resources["ImportBorderBrush"]).Color = Color.FromRgb(33, 150, 243);
        }

        private void ImportBorder_MouseLeave(object sender, EventArgs e)
        {
            ((SolidColorBrush)ImportBorder.Resources["ImportBorderBrush"]).Color = Color.FromRgb(209, 216, 224);
        }
        private void ImportBorder_Drop(object sender, DragEventArgs e)
        {
            try
            {
                _Path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (System.IO.Path.GetExtension(_Path).Equals(".xls", StringComparison.InvariantCultureIgnoreCase) || System.IO.Path.GetExtension(_Path).Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                {
                    FileDropped.Items.Add(new Imports(1, _Path));
                    ImportBorder.IsEnabled = false;
                    ImportBorder_MouseLeave(sender, e);
                    ImportButton.IsEnabled = true;
                }
                else
                {
                    Main.Mad.MyAlertSnackbarIcon.MessageQueue.Enqueue("Ce fichier est dans un format incorrect");
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImportBorder.IsEnabled = true;
            ImportButton.IsEnabled = false;
            FileDropped.Items.Clear();
        }

        private void ImportBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog Ofd = new System.Windows.Forms.OpenFileDialog();
                Ofd.Filter = "Excell|*.xls;*.xlsx;";
                Ofd.Multiselect = false;
                System.Windows.Forms.DialogResult Dr = Ofd.ShowDialog();
                if (Dr == System.Windows.Forms.DialogResult.Abort || Dr == System.Windows.Forms.DialogResult.Cancel) return;
                _Path = Ofd.FileName.ToString();
                FileDropped.Items.Add(new Imports(1, _Path));
                ImportBorder.IsEnabled = false;
                ImportButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        BackgroundWorker Bw2 = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        BackgroundWorker Bw = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        int i;
        int? IdFilièreAnnée = 0, IdGroupe = 0;
        string _grp, FilièreShortS, FilièreShortI;

        void AMS_Clear()
        {
            AMSCef.Text = AMSCin.Text = AMSNom.Text = AMSClassement.Text = "";
            AMSÉtablissement.ItemsSource = AMSAnnéeF.ItemsSource = AMSNiveau.ItemsSource = AMSTypeS.ItemsSource = AMSTypeF.ItemsSource = AMSFilière.ItemsSource = AMSAnnéeE.ItemsSource = AMSGroupe.ItemsSource = null;
            AMSÉtablissement.Items.Clear();
            AMSAnnéeF.Items.Clear();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ImportForm.Visibility = Visibility.Collapsed;

            AMStagiaire.Visibility = Visibility.Visible;
            SAButton.Visibility = Visibility.Visible;
            SMButton.Visibility = Visibility.Collapsed;

            DeleteWarningDialog.Visibility = Visibility.Collapsed;
            ErrorList.Visibility = Visibility.Collapsed;
            ExcelExport.Visibility = Visibility.Collapsed;


            _inUsing = true;

            AMSCef.IsEnabled = true;
            AMSÉtablissement.IsEnabled = true;
            AMSAnnéeF.IsEnabled = true;


            AMSTitle.Text = "Ajouter un stagiaire";
            AMSAdmisOui.Background = AMSAdmisNon.Background = new SolidColorBrush(Color.FromRgb(46, 134, 222));

            AMS_Clear();

            try
            {
                AMSCef.ItemsSource = App.Ds.Stagiaire.Select(o => o.Cef).Distinct();

                AMSÉtablissement.ItemsSource = App.Ds.Établissement;
                AMSÉtablissement.DisplayMemberPath = "Nom";
                AMSÉtablissement.SelectedValuePath = "Code";

                AMSAnnéeF.ItemsSource = App.Ds.FilièreAnnée.Select(o => o.AnnéeFormation).Distinct();

                AMSNiveau.ItemsSource = App.Ds.Niveau;
                AMSNiveau.DisplayMemberPath = "Nom";
                AMSNiveau.SelectedValuePath = "NomCourt";

                AMSTypeS.ItemsSource = App.Ds.Groupe.Select(o => o.TypeStagiairesRow).Distinct();
                AMSTypeS.DisplayMemberPath = "Nom";
                AMSTypeS.SelectedValuePath = "NomCourt";

                AMSTypeF.ItemsSource = App.Ds.Groupe.Select(o => o.TypeFormation).Distinct();

                AMSFilière.ItemsSource = App.Ds.Filière;
                AMSFilière.DisplayMemberPath = "Nom";
                AMSFilière.SelectedValuePath = "NomCourt";

                AMSAnnéeE.ItemsSource = App.Ds.AnnéeÉtude;
                AMSAnnéeE.DisplayMemberPath = "Nom";
                AMSAnnéeE.SelectedValuePath = "NomCourt";

                _inUsing = false;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ImportForm.Visibility = Visibility.Collapsed;
            AMStagiaire.Visibility = Visibility.Collapsed;
            DeleteWarningDialog.Visibility = Visibility.Visible;
            ErrorList.Visibility = Visibility.Collapsed;
            ExcelExport.Visibility = Visibility.Collapsed;

            DeleteWarningDialogCount.Text = GridViewModel._Data.Where(o => o.IsSelected).Count().ToString();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                var _tempData = GridViewModel._Data.Where(o => o.IsSelected).ToList();
                int i = 0;
                foreach (var item in _tempData)
                {
                    try
                    {
                        DbContext.StagiaireAdapter.DeleteQuery(item.Cef);
                        GridViewModel._Data.Remove(item);
                    }
                    catch
                    {
                        i++;
                    }
                }
                if (i > 0)
                {
                    Main.Mad.MyAlertSnackbarIcon.MessageQueue.Enqueue($"({_tempData.Count - i}) sur ({_tempData.Count}) stagiaires sélectionnés ont été supprimés car les stagiaires non supprimés sont en cours d'utilisation au niveau de la base de données");
                }
                else
                {
                    Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue($"{_tempData.Count} stagiaires ont été supprimés avec succès");
                    GridCheckBoxColumn.IsChecked = false;
                    MainDialog.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void RechercherStagiaire_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string txt = RechercherStagiaire.Text.ToLower().Trim();
                _tempData = new ObservableCollection<SelectableViewModel>();
                var _Data = GridViewModel._Data.Where(o => RéguliersChecked.IsChecked.Value ? string.Equals(o.TypeStagiaires, "Candidat Régulier") : (LibresChecked.IsChecked.Value ? string.Equals(o.TypeStagiaires, "Candidat Libre") : true));
                if (txt.Length > 0)
                {
                    foreach (SelectableViewModel item in _Data)
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
                    if (GridViewModel._Data.Count != DataGridData.Items.Count)
                    {
                        foreach (SelectableViewModel item in _Data)
                        {
                            _tempData.Add(item);
                        }
                        DataGridData.ItemsSource = _tempData;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (DataGridData != null)
            {
                DataGridData.ItemsSource = GridViewModel._Data;
                return;
            }
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            _tempData = new ObservableCollection<SelectableViewModel>();
            foreach (SelectableViewModel ab in GridViewModel._Data.Where(o => string.Equals(o.TypeStagiaires, "Candidat Régulier")))
            {
                _tempData.Add(ab);
            }
            DataGridData.ItemsSource = _tempData;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            _tempData = new ObservableCollection<SelectableViewModel>();
            foreach (SelectableViewModel ab in GridViewModel._Data.Where(o => string.Equals(o.TypeStagiaires, "Candidat Libre")))
            {
                _tempData.Add(ab);
            }
            DataGridData.ItemsSource = _tempData;
        }

        bool _inUsing;
        private void ModifierStagiaire_Click(object sender, RoutedEventArgs e)
        {
            ImportForm.Visibility = Visibility.Collapsed;

            AMStagiaire.Visibility = Visibility.Visible;
            SAButton.Visibility = Visibility.Collapsed;
            SMButton.Visibility = Visibility.Visible;

            DeleteWarningDialog.Visibility = Visibility.Collapsed;
            ErrorList.Visibility = Visibility.Collapsed;
            ExcelExport.Visibility = Visibility.Collapsed;

            _inUsing = true;
            AMSTitle.Text = "Modifier un stagiaire";
            AMSAdmisOui.Background = AMSAdmisNon.Background = new SolidColorBrush(Color.FromRgb(16, 172, 132));

            AMS_Clear();

            AMSCef.IsEnabled = AMSÉtablissement.IsEnabled = AMSAnnéeF.IsEnabled = false;

            AMSÉtablissement.DisplayMemberPath = "";
            AMSÉtablissement.SelectedValuePath = "";

            try
            {
                var _stagiaire = GridViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                if (_stagiaire != null)
                {
                    AMSCef.Text = _stagiaire.Cef;

                    AMSCin.Text = _stagiaire.Cin;
                    AMSNom.Text = _stagiaire.NomPrénom;

                    AMSÉtablissement.Items.Add(_stagiaire.Établissement);
                    AMSÉtablissement.SelectedItem = _stagiaire.Établissement;

                    AMSAnnéeF.Items.Add(_stagiaire.AnnéeFormation);
                    AMSAnnéeF.SelectedItem = _stagiaire.AnnéeFormation;

                    AMSNiveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreRow.NiveauRow).Distinct();
                    AMSNiveau.DisplayMemberPath = "Nom";
                    AMSNiveau.SelectedValuePath = "NomCourt";
                    AMSNiveau.Text = _stagiaire.Niveau;

                    AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => o.FilièreAnnéeRow.FilièreRow.Niveau.Equals(AMSNiveau.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeStagiairesRow).Distinct();
                    AMSTypeS.DisplayMemberPath = "Nom";
                    AMSTypeS.SelectedValuePath = "NomCourt";
                    AMSTypeS.Text = _stagiaire.TypeStagiaires;

                    AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => o.FilièreAnnéeRow.FilièreRow.Niveau.Equals(AMSNiveau.SelectedValue.ToString()) && o.TypeStagiaires.Equals(AMSTypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeFormation).Distinct();
                    AMSTypeF.SelectedItem = _stagiaire.TypeFormation;

                    AMSFilière.ItemsSource = App.Ds.Groupe.Where(o => o.TypeFormation.Equals(AMSTypeF.SelectedValue.ToString()) && o.FilièreAnnéeRow.FilièreRow.Niveau.Equals(AMSNiveau.SelectedValue.ToString()) && o.TypeStagiaires.Equals(AMSTypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                    AMSFilière.DisplayMemberPath = "Nom";
                    AMSFilière.SelectedValuePath = "NomCourt";
                    AMSFilière.Text = _stagiaire.Filière;

                    AMSAnnéeE.ItemsSource = App.Ds.Groupe.Where(o => o.FilièreAnnéeRow.Filière.Equals(AMSFilière.SelectedValue.ToString()) && o.TypeFormation.Equals(AMSTypeF.SelectedValue.ToString()) && o.FilièreAnnéeRow.FilièreRow.Niveau.Equals(AMSNiveau.SelectedValue.ToString()) && o.TypeStagiaires.Equals(AMSTypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.AnnéeÉtudeRow).Distinct();
                    AMSAnnéeE.DisplayMemberPath = "Nom";
                    AMSAnnéeE.SelectedValuePath = "NomCourt";
                    AMSAnnéeE.Text = _stagiaire.AnnéeÉtude;

                   

                    AMSGroupe.ItemsSource = App.Ds.Groupe.Where(o => o.TypeFormation.Equals(AMSTypeF.SelectedValue.ToString()) && o.FilièreAnnéeRow.FilièreRow.Niveau.Equals(AMSNiveau.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement) && string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString()) && string.Equals(o.AnnéeÉtudeRow.Nom, _stagiaire.AnnéeÉtude) && o.TypeStagiaires.Equals(AMSTypeS.SelectedValue.ToString())).Select(o => o.Numéro).Distinct();
                    AMSGroupe.SelectedItem = _stagiaire.Groupe.Substring(_stagiaire.Groupe.Length - 3);

                    AMSClassement.Text = _stagiaire.Classement.ToString();

                    if (string.Equals(_stagiaire.Admis.ToLower(), "oui")) AMSAdmisOui.IsChecked = true;
                    else AMSAdmisNon.IsChecked = true;
                }
                else
                {
                    Main.Mad.MyInformationSnackbarIcon.MessageQueue.Enqueue("Pouvez-vous sélectionner au moins un stagiaire");
                }
                _inUsing = false;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void AMSNiveau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && AMSNiveau.SelectedValue != null)
                {
                    if (SMButton.Visibility == Visibility.Visible)
                    {
                        var _stagiaire = GridViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                        AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.FilièreRow.Niveau, AMSNiveau.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeStagiairesRow).Distinct();
                        AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.FilièreRow.Niveau, AMSNiveau.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeFormation).Distinct();
                        AMSFilière.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.FilièreRow.Niveau, AMSNiveau.SelectedValue.ToString()) && string.Equals(o.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreRow).Distinct();
                    }
                    else
                    {
                        AMSÉtablissement.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.FilièreRow.Niveau, AMSNiveau.SelectedValue.ToString())).Select(o => o.ÉtablissementRow).Distinct();
                        AMSAnnéeF.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.FilièreRow.Niveau, AMSNiveau.SelectedValue.ToString())).Select(o => o.AnnéeFormation).Distinct();
                        AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.FilièreRow.Niveau, AMSNiveau.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                        AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.FilièreRow.Niveau, AMSNiveau.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                        AMSFilière.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.FilièreRow.Niveau, AMSNiveau.SelectedValue.ToString())).Select(o => o.FilièreRow).Distinct();
                        AMSAnnéeE.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.FilièreRow.Niveau, AMSNiveau.SelectedValue.ToString())).Select(o => o.AnnéeÉtudeRow).Distinct();
                    }
                    if (AMSFilière.SelectedIndex == -1)
                    {
                        AMSGroupe.ItemsSource = null;
                        AMSAnnéeE.ItemsSource = null;
                    }
                    else
                    {
                        AMSFilière_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

        private void AMSTypeS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && AMSTypeS.SelectedValue != null)
                {
                    if (SMButton.Visibility == Visibility.Visible)
                    {
                        var _stagiaire = GridViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                        AMSNiveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeFormation).Distinct();
                        AMSFilière.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                    }
                    else
                    {
                        AMSÉtablissement.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.ÉtablissementRow).Distinct();
                        AMSAnnéeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.AnnéeFormation).Distinct();
                        AMSNiveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                        AMSFilière.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                        AMSAnnéeE.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString())).Select(o => o.AnnéeÉtudeRow).Distinct();
                    }
                    if (AMSFilière.SelectedIndex == -1)
                    {
                        AMSGroupe.ItemsSource = null;
                        AMSAnnéeE.ItemsSource = null;
                    }
                    else
                    {
                        AMSFilière_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

        private void AMSTypeF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && AMSTypeF.SelectedValue != null)
                {
                    if (SMButton.Visibility == Visibility.Visible)
                    {
                        var _stagiaire = GridViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                        AMSNiveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, AMSTypeF.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, AMSTypeF.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeStagiairesRow).Distinct();
                        AMSFilière.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, AMSTypeF.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                    }
                    else
                    {
                        AMSÉtablissement.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, AMSTypeF.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.ÉtablissementRow).Distinct();
                        AMSAnnéeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, AMSTypeF.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.AnnéeFormation).Distinct();
                        AMSNiveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, AMSTypeF.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, AMSTypeF.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                        AMSFilière.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, AMSTypeF.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                        AMSAnnéeE.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.TypeFormation, AMSTypeF.SelectedValue.ToString())).Select(o => o.AnnéeÉtudeRow).Distinct();
                    }
                    if (AMSFilière.SelectedIndex == -1)
                    {
                        AMSGroupe.ItemsSource = null;
                        AMSAnnéeE.ItemsSource = null;
                    }
                    else
                    {
                        AMSFilière_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

        private void AMSFilière_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && AMSFilière.SelectedValue != null)
                {
                    if (SMButton.Visibility == Visibility.Visible)
                    {
                        var _stagiaire = GridViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                        AMSNiveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Filière, AMSFilière.SelectedValue.ToString()) && string.Equals(o.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreRow.NiveauRow).Distinct();
                        AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeFormation).Distinct();
                        AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeStagiairesRow).Distinct();
                        AMSAnnéeE.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.AnnéeÉtudeRow).Distinct();
                        AMSGroupe.ItemsSource = App.Ds.Groupe.Where(o => AMSTypeS.SelectedItem == null ? true : o.TypeStagiaires.Equals(AMSTypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement) && AMSAnnéeE.SelectedValue != null ? string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString()) : false).Select(o => o.Numéro).Distinct();
                    }
                    else
                    {
                        AMSÉtablissement.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Filière, AMSFilière.SelectedValue.ToString())).Select(o => o.ÉtablissementRow).Distinct();
                        AMSAnnéeF.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Filière, AMSFilière.SelectedValue.ToString())).Select(o => o.AnnéeFormation).Distinct();
                        AMSNiveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Filière, AMSFilière.SelectedValue.ToString())).Select(o => o.FilièreRow.NiveauRow).Distinct();
                        AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                        AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                        AMSAnnéeE.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString())).Select(o => o.AnnéeÉtudeRow).Distinct();
                        AMSGroupe.ItemsSource = App.Ds.Groupe.Where(o => AMSTypeS.SelectedItem == null ? true : o.TypeStagiaires.Equals(AMSTypeS.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString())).Select(o => o.Numéro).Distinct();
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void AMSAnnéeE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && AMSAnnéeE.SelectedValue != null)
                {
                    if (SMButton.Visibility == Visibility.Visible)
                    {
                        var _stagiaire = GridViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                        AMSNiveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeFormation).Distinct();
                        AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.TypeStagiairesRow).Distinct();
                        AMSFilière.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement)).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                        AMSGroupe.ItemsSource = App.Ds.Groupe.Where(o => AMSTypeS.Text == "" ? true : o.TypeStagiaires.Equals(AMSTypeS.SelectedValue.ToString()) && string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString()) && string.Equals(o.FilièreAnnéeRow.AnnéeFormation, _stagiaire.AnnéeFormation) && string.Equals(o.FilièreAnnéeRow.ÉtablissementRow.Nom, _stagiaire.Établissement) && AMSFilière.SelectedValue != null ? string.Equals(o.FilièreAnnéeRow.Filière, AMSFilière.SelectedValue.ToString()) : false).Select(o => o.Numéro).Distinct();
                    }
                    else
                    {
                        AMSÉtablissement.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.ÉtablissementRow).Distinct();
                        AMSAnnéeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.AnnéeFormation).Distinct();
                        AMSNiveau.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow.NiveauRow).Distinct();
                        AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                        AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                        AMSFilière.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString())).Select(o => o.FilièreAnnéeRow.FilièreRow).Distinct();
                        AMSGroupe.ItemsSource = App.Ds.Groupe.Where(o => AMSTypeS.Text == "" ? true : o.TypeStagiaires.Equals(AMSTypeS.SelectedValue.ToString()) && string.Equals(o.AnnéeÉtude, AMSAnnéeE.SelectedValue.ToString())).Select(o => o.Numéro).Distinct();
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        bool _isDirty;
        private void SMButton_Click(object sender, RoutedEventArgs e)
        {
            _isDirty = false;
            int _AMSClassement;
            AMSCinWarning.Visibility = AMSCinWarning.Visibility = AMSNomWarning.Visibility = AMSNiveauWarning.Visibility = AMSTypeSWarning.Visibility = AMSTypeFWarning.Visibility = AMSFilièreWarning.Visibility = AMSAnnéeEWarning.Visibility = AMSGroupeWarning.Visibility = AMSClassementWarning.Visibility = Visibility.Collapsed;
            AMSCinWarningText.Text = AMSCinWarningText.Text = AMSNomWarningText.Text = AMSNiveauWarningText.Text = AMSTypeSWarningText.Text = AMSTypeFWarningText.Text = AMSFilièreWarningText.Text = AMSAnnéeEWarningText.Text = AMSGroupeWarningText.Text = AMSClassementWarningText.Text = "";

            try
            {
                if (string.IsNullOrWhiteSpace(AMSCin.Text))
                {
                    AMSCinWarning.Visibility = Visibility.Visible;
                    AMSCinWarningText.Text = "Veuillez définir un cin valide";
                    _isDirty = true;
                }
                else if (DbContext.StagiaireAdapter.GetData().Where(o => string.Equals(o.Cin.ToLower(), AMSCin.Text.ToLower().Trim()) && !string.Equals(o.Cef, AMSCef.Text)).Any())
                {
                    AMSCinWarning.Visibility = Visibility.Visible;
                    AMSCinWarningText.Text = "Ce cin déjà utilisé avec un autre stagiaire";
                    _isDirty = true;
                }
                if (string.IsNullOrWhiteSpace(AMSNom.Text) || AMSNom.Text.Trim().Length < 6)
                {
                    AMSNomWarning.Visibility = Visibility.Visible;
                    AMSNomWarningText.Text = "Veuillez définir un nom valide";
                    _isDirty = true;
                }
                if (AMSNiveau.SelectedIndex == -1)
                {
                    AMSNiveauWarning.Visibility = Visibility.Visible;
                    AMSNiveauWarningText.Text = "Veuillez sélectionner un niveau";
                    _isDirty = true;
                }
                if (AMSTypeS.SelectedIndex == -1)
                {
                    AMSTypeSWarning.Visibility = Visibility.Visible;
                    AMSTypeSWarningText.Text = "Veuillez sélectionner un type de stagiaire";
                    _isDirty = true;
                }
                if (AMSTypeF.SelectedIndex == -1)
                {
                    AMSTypeFWarning.Visibility = Visibility.Visible;
                    AMSTypeFWarningText.Text = "Veuillez sélectionner un type de formation";
                    _isDirty = true;
                }
                if (AMSFilière.SelectedIndex == -1)
                {
                    AMSFilièreWarning.Visibility = Visibility.Visible;
                    AMSFilièreWarningText.Text = "Veuillez sélectionner une filière";
                    _isDirty = true;
                }
                if (AMSAnnéeE.SelectedIndex == -1)
                {
                    AMSAnnéeEWarning.Visibility = Visibility.Visible;
                    AMSAnnéeEWarningText.Text = "Veuillez sélectionner une année d'étude";
                    _isDirty = true;
                }
                if (AMSGroupe.SelectedIndex == -1)
                {
                    AMSGroupeWarning.Visibility = Visibility.Visible;
                    AMSGroupeWarningText.Text = "Veuillez sélectionner un groupe";
                    _isDirty = true;
                }
                if (_isDirty) return;

                var _stagiaire = GridViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();

                if (_stagiaire.Cin.Equals(AMSCin.Text.Trim())
                    && _stagiaire.NomPrénom.Equals(AMSNom.Text.Trim())
                    && _stagiaire.Niveau.Equals(AMSNiveau.Text)
                    && _stagiaire.TypeStagiaires.Equals(AMSTypeS.Text)
                    && _stagiaire.TypeFormation.Equals(AMSTypeF.Text)
                    && _stagiaire.Filière.Equals(AMSFilière.Text)
                    && _stagiaire.AnnéeÉtude.Equals(AMSAnnéeE.Text)
                    && _stagiaire.Groupe.Equals(AMSFilière.SelectedValue.ToString() + " " + AMSGroupe.Text)
                    && _stagiaire.Classement.Equals(AMSClassement.Text.Trim())
                    && _stagiaire.Admis.Equals(AMSAdmisOui.IsChecked.Value ? "Oui" : "Non")
                    )
                {
                    Main.Mad.MyAlertSnackbarIcon.MessageQueue.Enqueue("Tu n'as rien changé");
                    return;
                }
                int? _filiereId = App.Ds.FilièreAnnée.Where(o => o.ÉtablissementRow.Nom.Equals(AMSÉtablissement.Text) && o.AnnéeFormation.Equals(AMSAnnéeF.Text) && o.Filière.Equals(AMSFilière.SelectedValue.ToString(), StringComparison.InvariantCultureIgnoreCase) && o.FilièreRow.Niveau.Equals(AMSNiveau.SelectedValue.ToString())).Select(o => o.Id).FirstOrDefault();
                if (_filiereId > 0)
                {
                    MessageBox.Show(AMSAnnéeE.SelectedValue.ToString());
                    MessageBox.Show(AMSTypeF.Text);
                    MessageBox.Show(AMSTypeS.SelectedValue.ToString());
                    int? _groupeId = App.Ds.Groupe.Where(o => 
                    o.FilièreAnnée.Equals(_filiereId) && 
                    o.Numéro.Equals(AMSGroupe.SelectedValue.ToString()) && 
                    o.AnnéeÉtude.Equals(AMSAnnéeE.SelectedValue.ToString()) && 
                    o.TypeFormation.Equals(AMSTypeF.Text) && 
                    o.TypeStagiaires.Equals(AMSTypeS.SelectedValue.ToString())
                    ).Select(o => o.Id).FirstOrDefault();
                    if (_groupeId > 0)
                    {
                        try
                        {
                            if (!int.TryParse(AMSClassement.Text.Trim(), out _AMSClassement))
                            {
                                AMSClassementWarning.Visibility = Visibility.Visible;
                                AMSClassementWarningText.Text = "S'il vous plait, entrez un nombre valide";
                                return;
                            }
                            else if (App.Ds.StagiaireGroupe.Where(o => o.Groupe.Equals(_groupeId) && !string.Equals(o.Stagiaire, _stagiaire.Cef) && int.Equals(o.Classement, _AMSClassement)).Any())
                            {
                                AMSClassementWarning.Visibility = Visibility.Visible;
                                AMSClassementWarningText.Text = "Ce classement est déjà utilisé";
                                return;
                            }
                            string _admis = AMSAdmisOui.IsChecked.Value ? "Oui" : "Non";
                            if (string.Equals(_stagiaire.Cin.ToLower(), AMSCin.Text.Trim().ToLower()) || string.Equals(_stagiaire.NomPrénom.ToLower(), AMSNom.Text.Trim().ToLower()))
                                DbContext.StagiaireAdapter.UpdateStagiaireGroupe(_stagiaire.GroupeId, _groupeId, _stagiaire.Cef, _AMSClassement, _admis, null, null);
                            else
                                DbContext.StagiaireAdapter.UpdateStagiaireGroupe(_stagiaire.GroupeId, _groupeId, _stagiaire.Cef, _AMSClassement, _admis, AMSCin.Text.Trim(), AMSNom.Text.Trim());
                            Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue("Les informations sur le stagiaire ont été modifiées avec succès");
                            //_stagiaire.Cin = AMSCin.Text.Trim();
                            //_stagiaire.NomPrénom = AMSNom.Text.Trim();
                            //_stagiaire.Niveau = AMSNiveau.Text;
                            //_stagiaire.TypeStagiaires = AMSTypeS.Text;
                            //_stagiaire.TypeFormation = AMSTypeF.Text;
                            //_stagiaire.Filière = AMSFilière.Text;
                            //_stagiaire.AnnéeÉtude = AMSAnnéeE.Text;
                            //_stagiaire.Groupe = AMSFilière.SelectedValue.ToString() + " " + AMSGroupe.Text;
                            //_stagiaire.Classement = _AMSClassement;
                            //_stagiaire.Admis = _admis;
                            //_stagiaire.GroupeId = (int)_groupeId;
                            //_stagiaire.IsSelected = false;
                            MainDialog.IsOpen = false;
                            DbContext.ReFillImporterDesStagiaire();
                            // PROB IM RE EDITING THE SAME ONE
                            DataGridData.DataContext = new GridViewModel();
                            DataGridData.ItemsSource = GridViewModel._Data;
                        }
                        catch
                        {
                            Main.Mad.MyErrorSnackbarIcon.MessageQueue.Enqueue("Désolé mais nous ne pouvons pas modifier ce stagiaire pour le moment");
                        }
                    }
                    else Main.Mad.MyErrorSnackbarIcon.MessageQueue.Enqueue("Le groupe que vous sélectionnez est introuvable");
                }
                else Main.Mad.MyErrorSnackbarIcon.MessageQueue.Enqueue("Le filière que vous sélectionnez n'est pas trouvée ou n'est pas utilisée actuellement");
            }
       
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void AMSÉtablissement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && AMSÉtablissement.SelectedValue != null)
                {
                    AMSAnnéeF.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Établissement, AMSÉtablissement.SelectedValue.ToString())).Select(o => o.AnnéeFormation).Distinct();
                    AMSNiveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Établissement, AMSÉtablissement.SelectedValue.ToString())).Select(o => o.FilièreRow.NiveauRow).Distinct();
                    AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Établissement, AMSÉtablissement.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                    AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Établissement, AMSÉtablissement.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                    AMSFilière.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.Établissement, AMSÉtablissement.SelectedValue.ToString())).Select(o => o.FilièreRow).Distinct();
                    AMSAnnéeE.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.Établissement, AMSÉtablissement.SelectedValue.ToString())).Select(o => o.AnnéeÉtudeRow).Distinct();
                    if (AMSFilière.SelectedIndex == -1)
                    {
                        AMSGroupe.ItemsSource = null;
                        AMSAnnéeE.ItemsSource = null;
                    }
                    else
                    {
                        AMSFilière_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void AMSAnnéeF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!_inUsing && AMSAnnéeF.SelectedValue != null)
                {
                    AMSÉtablissement.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.AnnéeFormation, AMSAnnéeF.SelectedValue.ToString())).Select(o => o.ÉtablissementRow).Distinct();
                    AMSNiveau.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.AnnéeFormation, AMSAnnéeF.SelectedValue.ToString())).Select(o => o.FilièreRow.NiveauRow).Distinct();
                    AMSTypeS.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.AnnéeFormation, AMSAnnéeF.SelectedValue.ToString())).Select(o => o.TypeStagiairesRow).Distinct();
                    AMSTypeF.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.AnnéeFormation, AMSAnnéeF.SelectedValue.ToString())).Select(o => o.TypeFormation).Distinct();
                    AMSFilière.ItemsSource = App.Ds.FilièreAnnée.Where(o => string.Equals(o.AnnéeFormation, AMSAnnéeF.SelectedValue.ToString())).Select(o => o.FilièreRow).Distinct();
                    AMSAnnéeE.ItemsSource = App.Ds.Groupe.Where(o => string.Equals(o.FilièreAnnéeRow.AnnéeFormation, AMSAnnéeF.SelectedValue.ToString())).Select(o => o.AnnéeÉtudeRow).Distinct();
                    if (AMSFilière.SelectedIndex == -1)
                    {
                        AMSGroupe.ItemsSource = null;
                        AMSAnnéeE.ItemsSource = null;
                    }
                    else
                    {
                        AMSFilière_SelectionChanged(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void AMSCef_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AMSCef.SelectedItem != null)
                {
                    var _stg = App.Ds.Stagiaire.Where(o => string.Equals(o.Cef, AMSCef.SelectedItem.ToString().Trim())).FirstOrDefault();
                    if (_stg != null)
                    {
                        AMSCin.Text = _stg.Cin;
                        AMSNom.Text = _stg.NomPrénom;
                    }
                    else
                    {
                        AMSCin.Text = "";
                        AMSNom.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void SAButton_Click(object sender, RoutedEventArgs e)
        {
            _isDirty = false;
            int _AMSClassement;
            AMSCefWarning.Visibility = AMSÉtablissementWarning.Visibility = AMSAnnéeFWarning.Visibility = AMSCinWarning.Visibility = AMSCinWarning.Visibility = AMSNomWarning.Visibility = AMSNiveauWarning.Visibility = AMSTypeSWarning.Visibility = AMSTypeFWarning.Visibility = AMSFilièreWarning.Visibility = AMSAnnéeEWarning.Visibility = AMSGroupeWarning.Visibility = AMSClassementWarning.Visibility = Visibility.Collapsed;
            AMSCefWarningText.Text = AMSÉtablissementWarningText.Text = AMSAnnéeFWarningText.Text = AMSCinWarningText.Text = AMSCinWarningText.Text = AMSNomWarningText.Text = AMSNiveauWarningText.Text = AMSTypeSWarningText.Text = AMSTypeFWarningText.Text = AMSFilièreWarningText.Text = AMSAnnéeEWarningText.Text = AMSGroupeWarningText.Text = AMSClassementWarningText.Text = "";

            try
            {
                if (string.IsNullOrWhiteSpace(AMSCef.Text))
                {
                    AMSCefWarning.Visibility = Visibility.Visible;
                    AMSCefWarningText.Text = "Veuillez définir un cef valide";
                    _isDirty = true;
                }
                if (string.IsNullOrWhiteSpace(AMSCin.Text))
                {
                    AMSCinWarning.Visibility = Visibility.Visible;
                    AMSCinWarningText.Text = "Veuillez définir un cin valide";
                    _isDirty = true;
                }
                else if (DbContext.StagiaireAdapter.GetData().Where(o => string.Equals(o.Cin.ToLower(), AMSCin.Text.ToLower().Trim()) && !string.Equals(o.Cef, AMSCef.Text)).Any())
                {
                    MessageBox.Show(AMSCef.Text);
                    AMSCinWarning.Visibility = Visibility.Visible;
                    AMSCinWarningText.Text = "Ce cin déjà utilisé avec un autre stagiaire";
                    _isDirty = true;
                }
                if (string.IsNullOrWhiteSpace(AMSNom.Text) || AMSNom.Text.Trim().Length < 6)
                {
                    AMSNomWarning.Visibility = Visibility.Visible;
                    AMSNomWarningText.Text = "Veuillez définir un nom valide";
                    _isDirty = true;
                }
                if (AMSÉtablissement.SelectedIndex == -1)
                {
                    AMSÉtablissementWarning.Visibility = Visibility.Visible;
                    AMSÉtablissementWarningText.Text = "Veuillez définir une établissement valide";
                    _isDirty = true;
                }
                if (AMSAnnéeF.SelectedIndex == -1)
                {
                    AMSAnnéeFWarning.Visibility = Visibility.Visible;
                    AMSAnnéeFWarningText.Text = "Veuillez définir une année de formation valide";
                    _isDirty = true;
                }
                if (AMSNiveau.SelectedIndex == -1)
                {
                    AMSNiveauWarning.Visibility = Visibility.Visible;
                    AMSNiveauWarningText.Text = "Veuillez sélectionner un niveau";
                    _isDirty = true;
                }
                if (AMSTypeS.SelectedIndex == -1)
                {
                    AMSTypeSWarning.Visibility = Visibility.Visible;
                    AMSTypeSWarningText.Text = "Veuillez sélectionner un type de stagiaire";
                    _isDirty = true;
                }
                if (AMSTypeF.SelectedIndex == -1)
                {
                    AMSTypeFWarning.Visibility = Visibility.Visible;
                    AMSTypeFWarningText.Text = "Veuillez sélectionner un type de formation";
                    _isDirty = true;
                }
                if (AMSFilière.SelectedIndex == -1)
                {
                    AMSFilièreWarning.Visibility = Visibility.Visible;
                    AMSFilièreWarningText.Text = "Veuillez sélectionner une filière";
                    _isDirty = true;
                }
                if (AMSAnnéeE.SelectedIndex == -1)
                {
                    AMSAnnéeEWarning.Visibility = Visibility.Visible;
                    AMSAnnéeEWarningText.Text = "Veuillez sélectionner une année d'étude";
                    _isDirty = true;
                }
                if (AMSGroupe.SelectedIndex == -1)
                {
                    AMSGroupeWarning.Visibility = Visibility.Visible;
                    AMSGroupeWarningText.Text = "Veuillez sélectionner un groupe";
                    _isDirty = true;
                }
                if (!int.TryParse(AMSClassement.Text.Trim(), out _AMSClassement))
                {
                    AMSClassementWarning.Visibility = Visibility.Visible;
                    AMSClassementWarningText.Text = "S'il vous plait, entrez un nombre valide";
                    _isDirty = true;
                }
                if (_isDirty) return;
                int? _filiereId = App.Ds.FilièreAnnée.Where(o => o.ÉtablissementRow.Nom.Equals(AMSÉtablissement.Text) && o.AnnéeFormation.Equals(AMSAnnéeF.Text) && o.Filière.Equals(AMSFilière.Text, StringComparison.InvariantCultureIgnoreCase) && o.FilièreRow.Niveau.Equals(AMSNiveau.SelectedValue.ToString())).Select(o => o.Id).FirstOrDefault();
                if (_filiereId > 0)
                {
                    int? _groupeId = App.Ds.Groupe.Where(o => o.FilièreAnnée.Equals(_filiereId) && o.Numéro.Equals(AMSGroupe.Text) && o.AnnéeÉtude.Equals(AMSAnnéeE.SelectedValue.ToString()) && o.TypeFormation.Equals(AMSTypeF.Text) && string.Equals(o.TypeStagiaires, AMSTypeS.SelectedValue.ToString())).Select(o => o.Id).FirstOrDefault();
                    if (_groupeId > 0 && App.Ds.StagiaireGroupe.Where(o => o.Groupe.Equals(_groupeId) && !string.Equals(o.Stagiaire, AMSCef.Text.Trim()) && int.Equals(o.Classement, _AMSClassement)).Any())
                    {
                        AMSClassementWarning.Visibility = Visibility.Visible;
                        AMSClassementWarningText.Text = "Ce classement est déjà utilisé";
                        return;
                    }
                }
                string _admis = AMSAdmisOui.IsChecked.Value ? "Oui" : "Non";
                try
                {
                    DbContext.QueriesAdapter.InsertFullRow(AMSFilière.SelectedValue.ToString(), AMSFilière.Text, AMSNiveau.SelectedValue.ToString(), AMSÉtablissement.SelectedValue.ToString(), AMSAnnéeF.Text, "Promotion", AMSGroupe.Text, AMSAnnéeE.SelectedValue.ToString(), AMSTypeF.Text, AMSTypeS.SelectedValue.ToString(), AMSCef.Text, AMSNom.Text, AMSCin.Text, _admis, _AMSClassement);
                    Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue("Stagiaire ajouté avec succès");
                    MainDialog.IsOpen = false;
                    DbContext.ReFillImporterDesStagiaire();
                    DataGridData.DataContext = new GridViewModel();
                    DataGridData.ItemsSource = GridViewModel._Data;
                    CheckContent();
                }
                catch
                {
                    Main.Mad.MyErrorSnackbarIcon.MessageQueue.Enqueue("Désolé mais nous ne pouvons pas ajouter ce stagiaire pour le moment");
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ImportForm.Visibility = Visibility.Visible;
            AMStagiaire.Visibility = Visibility.Collapsed;
            DeleteWarningDialog.Visibility = Visibility.Collapsed;
            ErrorList.Visibility = Visibility.Collapsed;
            ExcelExport.Visibility = Visibility.Collapsed;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ImportForm.Visibility = Visibility.Collapsed;
            AMStagiaire.Visibility = Visibility.Collapsed;
            DeleteWarningDialog.Visibility = Visibility.Collapsed;
            ErrorList.Visibility = Visibility.Visible;
            ExcelExport.Visibility = Visibility.Collapsed;

            try
            {
                ErrorListView.ItemsSource = _errorGridView;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ImportForm.Visibility = Visibility.Collapsed;
            AMStagiaire.Visibility = Visibility.Collapsed;
            DeleteWarningDialog.Visibility = Visibility.Collapsed;
            ErrorList.Visibility = Visibility.Collapsed;
            ExcelExport.Visibility = Visibility.Visible;


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
        private void ExcelExportBtn_Click(object sender, RoutedEventArgs e)
        {
            _isDirty = false;

            try
            {
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

                foreach (SelectableViewModel row in GridViewModel._Data.Where(o => o.IsSelected))
                {
                    dt.Rows.Add(row.Cef, row.NomPrénom, row.Cin, row.Niveau, row.TypeFormation, row.AnnéeÉtude, row.Classement, row.Admis, row.TypeStagiaires, row.AnnéeFormation, row.Groupe, row.Établissement, row.Filière);
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

        int b;
        string _AnneeFormation;
        private void InsertExcelRecords()
        {
            i = 0;
            b = 0;
            try
            {
                OleDbConnection MyCon = new OleDbConnection($"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={_Path};Extended properties=\"Excel 8.0; HDR = Yes;\";");
                MyCon.Open();
                try
                {
                    // we consider thart the excel file has just one sheet table if else we need to seet a for loop to manipulate all data

                    var sheets = MyCon.GetSchema("TABLES").AsEnumerable().Select(x => x.Field<string>("TABLE_NAME")).ToList();
                    OleDbCommand OCmd = new OleDbCommand($"SELECT * FROM [{sheets[0]}]", MyCon);
                    OleDbDataReader ORead = OCmd.ExecuteReader();


                    while (ORead.Read())
                    {
                        try
                        {
                            _grp = ORead[12].ToString().Trim();
                            FilièreShortS = _grp.Substring(0, _grp.Count() - 3);
                            FilièreShortI = _grp.Substring(_grp.Count() - 3);
                            _AnneeFormation = $"{ORead[9].ToString().Trim().Substring(0, 4)}-{ORead[9].ToString().Trim().Substring(5)}";
                            DbContext.QueriesAdapter.InsertFullRow(FilièreShortS.ToUpper(), ORead[11].ToString().Trim(), ORead[3].ToString().Trim().ToUpper(), ORead[13].ToString().Trim(), _AnneeFormation, ORead[10].ToString().Trim(), FilièreShortI, ORead[5].ToString().Trim().ToUpper(), ORead[4].ToString().Trim(), ORead[8].ToString().Trim().ToUpper(), ORead[0].ToString().Trim(), ORead[1].ToString().Trim(), ORead[2].ToString().Trim(), char.ToUpper(ORead[7].ToString().Trim()[0]) + ORead[7].ToString().Trim().Substring(1).ToLower(), int.Parse(ORead[6].ToString().Trim()));
                            i++;
                        }
                        catch (Exception ex)
                        {
                            _errorGridView.Add(new ErrorGridView(ORead[0].ToString().Trim(), ex.Message));
                            b++;
                        }
                    }
                    MyCon.Close();
                }
                catch
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Main.Mad.MyErrorSnackbarIcon.MessageQueue.Enqueue("Veuillez vérifier votre fichier excel avant de continuer");
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
                }));
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            DialogClose.Visibility = Visibility.Collapsed;
            DialogLoading.Visibility = Visibility.Visible;
            ImportButton.IsEnabled = false;

            try
            {
                if (Bw.IsBusy)
                {
                    return;
                }

                System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();

                Bw.DoWork += (bwSender, bwArg) =>
                {
                    sWatch.Start();
                    InsertExcelRecords();
                };

                Bw.ProgressChanged += (bwSender, bwArg) =>
                {
                    //update progress bars here
                };

                Bw.RunWorkerCompleted += (bwSender, bwArg) =>
                {
                    sWatch.Stop();
                    Bw.Dispose();
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        DialogClose.Visibility = Visibility.Visible;
                        DialogLoading.Visibility = Visibility.Collapsed;
                        Button_Click(sender, e);
                        DbContext.ReFillImporterDesStagiaire();
                        if (App.Ds.Stagiaire.Count > 0)
                        {
                            CheckContent();
                            DataGridData.DataContext = new GridViewModel();
                            DataGridData.ItemsSource = GridViewModel._Data;
                        }
                        if (b > 0)
                        {
                            Main.Mad.MyAlertSnackbarIcon.MessageQueue.Enqueue($"{i} stagiaires ont été ajoutés avec succès");
                            WarningBadge.Visibility = Visibility.Visible;
                            WarningBadge.Badge = _errorGridView.Count;
                        }
                        else
                        {
                            Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue($"{i} stagiaires ont été ajoutés avec succès");
                        }
                        MainDialog.IsOpen = false;
                    }));
                };

                Bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }   
    }
}
