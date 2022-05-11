using G3D.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace G3D.Archive
{
    /// <summary>
    /// Interaction logic for ImporterArchive.xaml
    /// </summary>
    /// 
    public partial class Importer : UserControl
    {
        SolidColorBrush pdf = new SolidColorBrush(Color.FromRgb(252, 92, 101));
        SolidColorBrush word = new SolidColorBrush(Color.FromRgb(75, 123, 236));
        SolidColorBrush excel = new SolidColorBrush(Color.FromRgb(38, 222, 129));

        public static Importer Imp;


        public Importer()
        {
            InitializeComponent();



            Imp = this;

        }

        List<DataSet1.ArchiveRow> Arch = new List<DataSet1.ArchiveRow>();

        private void Bran_Selected(object sender, RoutedEventArgs e)
        {
            Nive.Visibility = AnnéE.Visibility = Fili.Visibility = Grou.Visibility = Visibility.Collapsed;
            Path.Text = "";
            if (Bran.SelectedItem != Note)
            {
                AnnéF.Visibility = Visibility.Visible;
                AnnéFCombo.ItemsSource = App.Ds.FilièreAnnée.Select(o => o.AnnéeFormation).Distinct();
                NoteBorder.Visibility = Visibility.Collapsed;
                Import.IsEnabled = false;
                DataGridData.ItemsSource = null;
                Arch.Clear();
            }
            else
            {
                AnnéF.Visibility = Visibility.Collapsed;
                NoteBorder.Visibility = Visibility.Visible;
                Import.IsEnabled = true;
                Arch.AddRange(App.Ds.Note.Select(o => o.ArchiveRow).Distinct());
                DataGridData.DataContext = new ArchiveViewModel(Arch);
                DataGridData.ItemsSource = ArchiveViewModel._Data;
            }
        }

        private void AnnéFCombo_Selected(object sender, RoutedEventArgs e)
        {
            if (AnnéFCombo.SelectedItem != null || AnnéFCombo.Text != "")
            {
                Nive.Visibility = Visibility.Visible;
                NiveCombo.ItemsSource = App.Ds.Niveau.Select(o => o.Nom);
            }
            else
            {
                Nive.Visibility = Visibility.Collapsed;
            }
            DataGridData.ItemsSource = null;
            Fili.Visibility = Visibility.Collapsed;
            Grou.Visibility = Visibility.Collapsed;
        }

        private void NiveCombo_Selected(object sender, RoutedEventArgs e)
        {
            AnnéE.Visibility = Visibility.Visible;

            AnnéECombo.SelectedItem = null;
            Fili.Visibility = Visibility.Collapsed;
            Grou.Visibility = Visibility.Collapsed;
            DataGridData.ItemsSource = null;

            if (Bran.SelectedItem == Dipl)
            {
                AnnéECombo.ItemsSource = App.Ds.AnnéeÉtude.Where(o => !o.Nom.StartsWith("1")).Select(o => o.Nom);
            }
            else
            {
                AnnéECombo.ItemsSource = App.Ds.AnnéeÉtude.Select(o => o.Nom);
            }
        }

        private void AnnéECombo_Selected(object sender, RoutedEventArgs e)
        {
            Fili.Visibility = Visibility.Visible;
            FiliCombo.ItemsSource = App.Ds.Filière.Select(o => o.NomCourt);
            DataGridData.ItemsSource = null;
        }

        private void FiliCombo_Selected(object sender, RoutedEventArgs e)
        {
            if (FiliCombo.SelectedItem != null || FiliCombo.Text != "")
            {
                Grou.Visibility = Visibility.Visible;
                GrouCombo.ItemsSource = App.Ds.Groupe.Where(o => o.AnnéeÉtudeRow.Nom.Equals(AnnéECombo.SelectedItem.ToString())).Select(o => o.Numéro).Distinct();
                if (GrouCombo.Text != "")
                {
                    Path.Text = FiliCombo.Text.ToString() + " " + GrouCombo.Text.ToString();
                    Arch.AddRange(App.Ds.Document.Where(o =>
                    o.Document.Equals(Bran.Text)
                    && o.AnnéeFormation.Equals(AnnéFCombo.Text)
                    && o.Niveau.Equals(NiveCombo.Text)
                    && o.Filière.Equals(FiliCombo.Text)
                    && o.Groupe.Equals(GrouCombo.Text)).Select(o => o.ArchiveRow).Distinct());
                    DataGridData.DataContext = new ArchiveViewModel(Arch);
                    DataGridData.ItemsSource = ArchiveViewModel._Data;
                }
                else
                {
                    DataGridData.ItemsSource = null;
                    Path.Text = "";
                    Arch.Clear();
                }
            }
            else
            {
                DataGridData.ItemsSource = null;
                Path.Text = "";
                Arch.Clear();
            }
        }

        private void GrouCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GrouCombo.SelectedItem != null)
            {
                if (AnnéFCombo.Text != "" && NiveCombo.SelectedItem != null && AnnéECombo.SelectedItem != null && FiliCombo.Text != "")
                {
                    Import.IsEnabled = true;
                    Path.Text = FiliCombo.Text.ToString() + " " + GrouCombo.Text.ToString();
                    Arch.AddRange(App.Ds.Document.Where(o =>
                    o.Document.Equals(Bran.Text)
                    && o.AnnéeFormation.Equals(AnnéFCombo.Text)
                    && o.Niveau.Equals(NiveCombo.Text)
                    && o.Filière.Equals(FiliCombo.Text)
                    && o.Groupe.Equals(GrouCombo.SelectedItem.ToString())).Select(o => o.ArchiveRow).Distinct());
                    //MessageBox.Show(Arch.Count().ToString());
                    DataGridData.DataContext = new ArchiveViewModel(Arch);
                    DataGridData.ItemsSource = ArchiveViewModel._Data;
                }
                else
                {
                    Import.IsEnabled = false;
                    DataGridData.ItemsSource = null;
                    Arch.Clear();
                }
            }
            else if (GrouCombo.Text == "")
            {
                Path.Text = "";
                Import.IsEnabled = false;
                DataGridData.ItemsSource = null;
                Arch.Clear();
            }
        }

        //private void FiliCombo_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == System.Windows.Input.Key.Enter && FiliCombo.Text != "")
        //    {
        //        Grou.Visibility = Visibility.Visible;
        //        GrouCombo.ItemsSource = App.Ds.Groupe.Select(o => o.Numéro).Distinct();
        //        if (GrouCombo.Text != "")
        //            Path.Text = FiliCombo.SelectedItem.ToString() + " " + GrouCombo.SelectedItem.ToString();
        //    }
        //    if ((FiliCombo.Text + e.Key) == "")
        //    {
        //        Path.Text = "";
        //        DataGridData.ItemsSource = null;
        //        Grou.Visibility = Visibility.Collapsed;
        //    }
        //}


        private void ImportBorder_MouseEnter(object sender, EventArgs e)
        {
            ((SolidColorBrush)ImportBorder.Resources["ImportBorderBrush"]).Color = Color.FromRgb(33, 150, 243);
        }

        private void ImportBorder_MouseLeave(object sender, EventArgs e)
        {
            ((SolidColorBrush)ImportBorder.Resources["ImportBorderBrush"]).Color = Color.FromRgb(209, 216, 224);
        }

        string[] _Paths;
        private void ImportBorder_Drop(object sender, DragEventArgs e)
        {

            int i = 0;
            _Paths = ((string[])e.Data.GetData(DataFormats.FileDrop));
            foreach (string path in _Paths)
            {
                Impo.Add(new Imports(i, path));
                i++;
                FileDropped.ItemsSource = null;
                FileDropped.ItemsSource = Impo;
                //ImportBorder.IsEnabled = false;
                ImportBorder_MouseLeave(sender, e);
                ImportButton.IsEnabled = true;
            }
            //if (System.IO.Path.GetExtension(_Path).Equals(".xls", StringComparison.InvariantCultureIgnoreCase) || System.IO.Path.GetExtension(_Path).Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase))
            //{

            //}
            //else
            //{
            //    //Main.Mad.MyAlertSnackbar.MessageQueue.Enqueue("Ce fichier est dans un format incorrect");
            //}
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ImportBorder.IsEnabled = true;
            //ImportButton.IsEnabled = false;
            //FileDropped.Items.Clear();
        }

        List<Imports> Impo = new List<Imports>();
        private void ImportBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            int i = 0;
            System.Windows.Forms.OpenFileDialog Ofd = new System.Windows.Forms.OpenFileDialog();
            //Ofd.Filter = "Excell|*.xls;*.xlsx;";
            Ofd.Multiselect = true;
            System.Windows.Forms.DialogResult Dr = Ofd.ShowDialog();
            if (Dr == System.Windows.Forms.DialogResult.Abort || Dr == System.Windows.Forms.DialogResult.Cancel) return;
            _Paths = Ofd.FileNames;
            //AddNewDroppedFile(od.FileNames.Count());
            foreach (string path in _Paths)
            {
                Impo.Add(new Imports(i, path));
                i++;
            }
            FileDropped.ItemsSource = null;
            FileDropped.ItemsSource = Impo;
            //ImportBorder.IsEnabled = false;
            ImportButton.IsEnabled = true;
        }

        BackgroundWorker Bw = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        public void InsertRecords()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                string fileName, destFile;
                string TargrtDirectory;
                if (Bran.SelectedItem != Note)
                {
                    TargrtDirectory = @$".\Archive\{Bran.Text}\{AnnéFCombo.Text}\{NiveCombo.Text}\{AnnéECombo.Text}\{FiliCombo.Text}\{GrouCombo.Text}";

                    System.IO.Directory.CreateDirectory(TargrtDirectory);
                    if (System.IO.Directory.Exists(TargrtDirectory))
                    {
                        //string[] files = System.IO.Directory.GetFiles(sourcePath);

                        // Copy the files and overwrite destination files if they already exist.
                        foreach (string path in _Paths)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            fileName = System.IO.Path.GetFileName(path);
                            destFile = System.IO.Path.Combine(TargrtDirectory, fileName);
                            System.IO.File.Copy(path, destFile, true);

                            int? IdArchive = null;
                            DbContext.ArchiveAdapter.ArchiveInsertQuery(DateTime.Now, null, destFile, ref IdArchive);
                            DbContext.DocumentAdapter.Insert(IdArchive, Bran.Text, AnnéFCombo.Text, NiveCombo.Text, AnnéECombo.Text, FiliCombo.Text, GrouCombo.Text);
                        }
                    }
                }
                else
                {
                    TargrtDirectory = @$".\Archive\{Bran.Text}";
                    //MessageBox.Show(TargrtDirectory);
                    System.IO.Directory.CreateDirectory(TargrtDirectory);
                    if (System.IO.Directory.Exists(TargrtDirectory))
                    {
                        //string[] files = System.IO.Directory.GetFiles(sourcePath);

                        // Copy the files and overwrite destination files if they already exist.
                        foreach (string path in _Paths)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            fileName = System.IO.Path.GetFileName(path);
                            destFile = System.IO.Path.Combine(TargrtDirectory, fileName);
                            System.IO.File.Copy(path, destFile, true);


                            int? IdArchive = null;
                            DbContext.ArchiveAdapter.ArchiveInsertQuery(DateTime.Now, null, destFile, ref IdArchive);
                            DbContext.NoteAdapter.Insert(IdArchive, Ref.Text, Obj.Text, Date.SelectedDate == null ? DateTime.Now : Date.SelectedDate);
                        }
                    }
                }

            }));
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            DialogClose.Visibility = Visibility.Collapsed;
            DialogLoading.Visibility = Visibility.Visible;
            ImportButton.IsEnabled = false;

            RefWarning.Visibility = Visibility.Collapsed;
            RefWarningText.Text = "";
            Arch.Clear();

            if (Bw.IsBusy) return;

            if (SearchByRef.SelectedItem == Note && Ref.Text == "")
            {
                RefWarning.Visibility = Visibility.Visible;
                RefWarningText.Text = "Please set a ref";
                DialogClose.Visibility = Visibility.Visible;
                DialogLoading.Visibility = Visibility.Collapsed;
                ImportButton.IsEnabled = true;
                return;
            }


            System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();

            Bw.DoWork += (bwSender, bwArg) =>
            {
                sWatch.Start();
                InsertRecords();
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
                    //Button_Click(sender, e);
                    //DbContext.ReFillDataSet();
                    /////////////////if (Main.Ds.Stagiaire.Count > 0)
                    /////////////////{
                    /////////////////CheckContent();
                    /////////////////}
                    DbContext.ReFillArchive();
                    if (Bran.SelectedItem != Note)
                    {
                        Arch.AddRange(App.Ds.Document.Where(o =>
                            o.Document.Equals(Bran.Text)
                            && o.AnnéeFormation.Equals(AnnéFCombo.Text)
                            && o.Niveau.Equals(NiveCombo.Text)
                            && o.Filière.Equals(FiliCombo.Text)
                            && o.Groupe.Equals(GrouCombo.Text)).Select(o => o.ArchiveRow).Distinct());
                    }
                    else
                    {
                        Arch.AddRange(App.Ds.Note.Select(o => o.ArchiveRow).Distinct());
                    }
                    DataGridData.DataContext = new ArchiveViewModel(Arch);
                    DataGridData.ItemsSource = ArchiveViewModel._Data;
                    FileDropped.ItemsSource = null;
                    Impo.Clear();
                    /////////////////StagiairesCount.Text = Main.Ds.Stagiaire.Count.ToString();
                    //Main.Mad.MyCheckSnackbar.MessageQueue.Enqueue($"{i} stagiaires ont été ajoutés avec succès");
                    //else Main.Mad.MyAlertSnackbar.MessageQueue.Enqueue($"{i} stagiaires ont été ajoutés avec succès");
                    //if (b > 0)
                    //{
                    //WarningBadge.Visibility = Visibility.Visible;
                    //WarningBadge.Badge = ErrorListView.Items.Count;
                    //ErrorListView.ItemsSource = _errorList;
                    //}
                    MainDialog.IsOpen = false;
                }));
            };

            Bw.RunWorkerAsync();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            ImportBorder.Visibility = Visibility.Visible;
            DeleteWarningDialog.Visibility = Visibility.Collapsed;
            FileEditForm.Visibility = Visibility.Collapsed;

            Impo.Clear();
            FileDropped.ItemsSource = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ImportBorder.Visibility = Visibility.Collapsed;
            DeleteWarningDialog.Visibility = Visibility.Visible;
            FileEditForm.Visibility = Visibility.Collapsed;

            DeleteWarningDialogCount.Text = ArchiveViewModel._Data.Where(o => o.IsSelected).Count().ToString();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var _SelectedFiles = ArchiveViewModel._Data.Where(o => o.IsSelected).Select(o => o.IdFile);
            //List<DataSet1.ArchiveRow> _TempList = new List<DataSet1.ArchiveRow>();
            if (_SelectedFiles.Any())
            {
                string? _Path;
                foreach (int id in _SelectedFiles)
                {
                    _Path = App.Ds.Archive.Where(o => o.Id == id).Select(o => o.Path).FirstOrDefault();
                    if (_Path != null)
                    {
                        if (System.IO.File.Exists(_Path))
                        {
                            try
                            {
                                System.IO.File.Delete(_Path);
                                DbContext.ArchiveAdapter.DeleteQuery(id);
                                Arch.Remove(Arch.Where(o => o.Id == id).FirstOrDefault());
                            }
                            catch (System.IO.IOException ex)
                            {
                                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
                                return;
                            }
                        }
                    }
                }
            }
            MainDialog.IsOpen = false;
            DataGridData.DataContext = new ArchiveViewModel(Arch);
            DataGridData.ItemsSource = ArchiveViewModel._Data;
        }

        string? _Path;
        private void FileEdit_Click(object sender, RoutedEventArgs e)
        {
            if (FilePath.Text != System.IO.Path.GetDirectoryName(_Path) || FileTitle.Text != System.IO.Path.GetFileNameWithoutExtension(_Path))
            {
                try
                {
                    System.IO.File.Move(_Path, System.IO.Path.Combine(FilePath.Text, $"{FileTitle.Text}{System.IO.Path.GetExtension(_Path)}"));
                    var _SelectedArch = ArchiveViewModel._Data.Where(o => o.IsSelected).FirstOrDefault();
                    DbContext.ArchiveAdapter.UpdateQuery(_SelectedArch.IdFile, System.IO.Path.Combine(FilePath.Text, $"{FileTitle.Text}{System.IO.Path.GetExtension(_Path)}"), DateTime.Now);
                    DbContext.ReFillArchive();
                    MainDialog.IsOpen = false;

                    Arch.Clear();
                    if (Bran.SelectedItem != Note)
                    {
                        Arch.AddRange(App.Ds.Document.Where(o =>
                            o.Document.Equals(Bran.Text)
                            && o.AnnéeFormation.Equals(AnnéFCombo.Text)
                            && o.Niveau.Equals(NiveCombo.Text)
                            && o.Filière.Equals(FiliCombo.Text)
                            && o.Groupe.Equals(GrouCombo.Text)).Select(o => o.ArchiveRow).Distinct());
                        DataGridData.DataContext = new ArchiveViewModel(Arch);
                        DataGridData.ItemsSource = ArchiveViewModel._Data;
                        Path.Text = "";
                    }
                    else
                    {
                        Arch.AddRange(App.Ds.Note.Select(o => o.ArchiveRow).Distinct());
                        DataGridData.DataContext = new ArchiveViewModel(Arch);
                        DataGridData.ItemsSource = ArchiveViewModel._Data;
                        Path.Text = Bran.Text.ToString();
                    }

                }
                catch (System.IO.IOException ex)
                {

                }
            }
        }

        private void FilePathLocate_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    //string[] files = Directory.GetFiles(fbd.SelectedPath);
                    FilePath.Text = fbd.SelectedPath;
                }
            }
        }

        private void ModifierDocuments_Click(object sender, RoutedEventArgs e)
        {
            ImportBorder.Visibility = Visibility.Collapsed;
            DeleteWarningDialog.Visibility = Visibility.Collapsed;
            FileEditForm.Visibility = Visibility.Visible;

            _Path = ArchiveViewModel._Data.Where(o => o.IsSelected).Select(o => o.Path).FirstOrDefault();
            if (_Path != null)
            {
                FilePath.Text = System.IO.Path.GetDirectoryName(_Path);
                FileTitle.Text = System.IO.Path.GetFileNameWithoutExtension(_Path);
            }
            else
            {
                FilePath.Text = "";
                FileTitle.Text = "";
            }
        }
        private void AfficherDocuments_Click(object sender, RoutedEventArgs e)
        {
            _Paths = ArchiveViewModel._Data.Where(o => o.IsSelected).Select(o => o.Path).ToArray();

            foreach (string path in _Paths)
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Chemin != null)
            {
                Chemin.Visibility = Visibility.Visible;
                Stagiaire.Visibility = Visibility.Collapsed;
                Administration.Visibility = Visibility.Collapsed;
                Import.Visibility = Visibility.Visible;
                DbContext.ReClear();
                DbContext.ReFillArchive();
                DataGridData.ItemsSource = null;
                Path.Text = "";
                Arch.Clear();
            }

        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            Chemin.Visibility = Visibility.Collapsed;
            Stagiaire.Visibility = Visibility.Visible;
            Administration.Visibility = Visibility.Collapsed;
            Import.Visibility = Visibility.Collapsed;
            if (SearchByBox.ItemsSource == null)
            {
                DbContext.ReFillArchive();
                DbContext.StagiaireGroupeAdapter.Fill(App.Ds.StagiaireGroupe);
                DbContext.StagiaireAdapter.Fill(App.Ds.Stagiaire);

                SearchByBox.ItemsSource = App.Ds.Stagiaire.Select(o => $"{o.NomPrénom} ~ {o.Cin} ~ {o.Cef}");
            }
            DataGridData.ItemsSource = null;
            Path.Text = "";
            Arch.Clear();
        }
        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            Chemin.Visibility = Visibility.Collapsed;
            Stagiaire.Visibility = Visibility.Collapsed;
            Administration.Visibility = Visibility.Visible;
            Import.Visibility = Visibility.Collapsed;
            //if (SearchByRef.ItemsSource == null)
            //{
            //    DbContext.ReClear();
            //}
            DbContext.ArchiveAdapter.Fill(App.Ds.Archive);
            DbContext.NoteAdapter.Fill(App.Ds.Note);
            SearchByRef.ItemsSource = App.Ds.Note.Select(o => $"{o.Référence} ~ {o.Objectif}");

            DataGridData.ItemsSource = null;
            Path.Text = "";
            Arch.Clear();
        }

        string[] _full;
        private void SearchByBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchByBox.SelectedItem != null)
            {
                _full = SearchByBox.Text.Split('~');
                for (int i = 0; i < _full.Length; i++) { _full[i] = _full[i].Trim(); }
                YearsOfStudent.IsEnabled = true;
                YearsOfStudent.ItemsSource = App.Ds.StagiaireGroupe.Where(o => o.Stagiaire.Equals(_full[2])).Select(o => o.GroupeRow.FilièreAnnéeRow.AnnéeFormation).Distinct();
                DataGridData.ItemsSource = null;
                Path.Text = "";
            }
        }

        private void SearchByBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (SearchByBox.Text.Length < 1 || !App.Ds.Stagiaire.Where(o => (_full != null && _full.Count() == 3) ? (o.NomPrénom.Equals(_full[0]) && o.Cin.Equals(_full[1]) && o.Cef.Equals(_full[2])) : false).Any())
            {
                YearsOfStudent.IsEnabled = false;
                YearsOfStudent.ItemsSource = null;
                Arch.Clear();
            }
        }

        private void SearchByRef_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchByRef.SelectedItem != null)
            {
                Arch.Clear();
                _full = SearchByRef.Text.Split('~');
                for (int i = 0; i < _full.Length; i++) { _full[i] = _full[i].Trim(); }
                Arch.AddRange(App.Ds.Note.Where(o => _full.Count() == 2 ? (o.Référence.Equals(_full[0]) && o.Objectif.Equals(_full[1])) : false).Select(o => o.ArchiveRow));
                DataGridData.DataContext = new ArchiveViewModel(Arch);
                DataGridData.ItemsSource = ArchiveViewModel._Data;
            }
        }

        private void SearchByRef_TextChanged(object sender, RoutedEventArgs e)
        {
            if (SearchByRef.Text.Length < 1 || !App.Ds.Note.Where(o => (_full != null && _full.Count() == 2) ? (o.Référence.Equals(_full[0]) && o.Objectif.Equals(_full[1])) : false).Any())
            {
                DataGridData.ItemsSource = null;
                Arch.Clear();
            }
        }

        private void YearsOfStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearsOfStudent.SelectedItem != null)
            {
                var _newGS = App.Ds.StagiaireGroupe.Where(o => o.Stagiaire.Equals(_full[2]) && o.GroupeRow.FilièreAnnéeRow.AnnéeFormation.Equals(YearsOfStudent.SelectedItem.ToString()));
                if (_newGS.Any())
                {
                    Groupe.ItemsSource = _newGS.Select(o => $"{o.GroupeRow.FilièreAnnéeRow.Filière} {o.GroupeRow.Numéro}").ToArray();
                    Groupe.IsEnabled = true;
                    DataGridData.ItemsSource = null;
                    Path.Text = "";
                    if (Categorie.SelectedIndex != -1 && Groupe.SelectedItem != null)
                    {
                        var _g = _newGS.Where(o => string.Equals($"{o.GroupeRow.FilièreAnnéeRow.Filière} {o.GroupeRow.Numéro}", Groupe.SelectedItem.ToString())).FirstOrDefault();
                        if (_g != null)
                        {
                            Arch.AddRange(App.Ds.Document.Where(o => o.Document.Equals(Categorie.Text) && o.AnnéeFormation.Equals(YearsOfStudent.SelectedItem.ToString()) && o.Niveau.Equals(_g.GroupeRow.FilièreAnnéeRow.FilièreRow.NiveauRow.Nom) && o.AnnéeÉtude.Equals(_g.GroupeRow.AnnéeÉtude) && o.Filière.Equals(_g.GroupeRow.FilièreAnnéeRow.Filière) && o.Groupe.Equals(_g.GroupeRow.Numéro)).Select(o => o.ArchiveRow));
                            DataGridData.DataContext = new ArchiveViewModel(Arch);
                            DataGridData.ItemsSource = ArchiveViewModel._Data;
                            Path.Text = $"{Categorie.Text} / {YearsOfStudent.Text} / {Groupe.Text}";
                        }
                    }
                    return;
                }
            }
            Groupe.Text = "";
            Path.Text = "";
            DataGridData.ItemsSource = null;
            Categorie.IsEnabled = false;
            Groupe.IsEnabled = false;
            Arch.Clear();
        }

        private void Categorie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Categorie.SelectedItem != null)
            {
                Arch.Clear();
                var _newGS = App.Ds.StagiaireGroupe.Where(o => o.Stagiaire.Equals(_full[2]) && o.GroupeRow.FilièreAnnéeRow.AnnéeFormation.Equals(YearsOfStudent.Text)
                && o.GroupeRow.Numéro.Equals(Groupe.SelectedItem.ToString().Substring(Groupe.SelectedItem.ToString().Length - 3))
                && o.GroupeRow.FilièreAnnéeRow.Filière.Equals(Groupe.SelectedItem.ToString().Substring(0, Groupe.SelectedItem.ToString().Length - 4))).FirstOrDefault();


                //MessageBox.Show(_newGS.Stagiaire);
                ////MessageBox.Show(YearsOfStudent.Text);
                ////MessageBox.Show(Groupe.SelectedItem.ToString().Substring(Groupe.SelectedItem.ToString().Length - 3));
                ////MessageBox.Show(Groupe.SelectedItem.ToString().Substring(0, Groupe.SelectedItem.ToString().Length - 4));


                //MessageBox.Show(((ComboBoxItem)Categorie.SelectedItem).Content.ToString());
                //MessageBox.Show(YearsOfStudent.Text);
                //MessageBox.Show(_newGS.GroupeRow.FilièreAnnéeRow.FilièreRow.NiveauRow.Nom);
                //MessageBox.Show(_newGS.GroupeRow.AnnéeÉtudeRow.Nom);
                //MessageBox.Show(_newGS.GroupeRow.FilièreAnnéeRow.Filière);
                //MessageBox.Show(_newGS.GroupeRow.Numéro);

                //MessageBox.Show(App.Ds.Document.Count().ToString());

                Arch.AddRange(App.Ds.Document.Where(o =>
                o.Document.Equals(((ComboBoxItem)Categorie.SelectedItem).Content.ToString())
                && o.AnnéeFormation.Equals(YearsOfStudent.Text)
                && o.Niveau.Equals(_newGS.GroupeRow.FilièreAnnéeRow.FilièreRow.NiveauRow.Nom)
                && o.AnnéeÉtude.Equals(_newGS.GroupeRow.AnnéeÉtudeRow.Nom)
                && o.Filière.Equals(_newGS.GroupeRow.FilièreAnnéeRow.Filière)
                && o.Groupe.Equals(_newGS.GroupeRow.Numéro)).Select(o => o.ArchiveRow));

                //MessageBox.Show(Arch.Count().ToString());

                DataGridData.DataContext = new ArchiveViewModel(Arch);
                DataGridData.ItemsSource = ArchiveViewModel._Data;
                Path.Text = $"{Categorie.Text} / {YearsOfStudent.Text} / {Groupe.Text}";
            }
            else
            {
                DataGridData.ItemsSource = null;
                Path.Text = "";
                Arch.Clear();
            }
        }

        private void GrouCombo_TextChanged(object sender, RoutedEventArgs e)
        {

        }

        private void GrouCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (GrouCombo.SelectedItem == null && GrouCombo.Text != "")
            {
                if (AnnéFCombo.Text != "" && NiveCombo.SelectedItem != null && AnnéECombo.SelectedItem != null && FiliCombo.Text != "")
                {
                    Import.IsEnabled = true;
                    if (GrouCombo.Text != "")
                        Path.Text = $"{Bran.Text} / {AnnéFCombo.Text} / {FiliCombo.Text + " " + GrouCombo.Text}";
                    Arch.AddRange(App.Ds.Document.Where(o =>
                    o.Document.Equals(Bran.Text)
                    && o.AnnéeFormation.Equals(AnnéFCombo.Text)
                    && o.Niveau.Equals(NiveCombo.Text)
                    && o.Filière.Equals(FiliCombo.Text)
                    && o.Groupe.Equals(GrouCombo.Text)).Select(o => o.ArchiveRow).Distinct());
                    //MessageBox.Show(Arch.Count().ToString());
                    DataGridData.DataContext = new ArchiveViewModel(Arch);
                    DataGridData.ItemsSource = ArchiveViewModel._Data;
                }
                else
                {
                    Import.IsEnabled = false;
                    DataGridData.ItemsSource = null;
                    Arch.Clear();
                }
            }
            else
            {
                Import.IsEnabled = false;
                DataGridData.ItemsSource = null;
                Arch.Clear();
            }
        }

        private void Groupe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Groupe.SelectedItem != null)
            {
                Categorie.IsEnabled = true;
                DataGridData.ItemsSource = null;
                Path.Text = "";
                if (Categorie.SelectedIndex != -1)
                {
                    var _g = App.Ds.StagiaireGroupe.Where(o => string.Equals($"{o.GroupeRow.FilièreAnnéeRow.Filière} {o.GroupeRow.Numéro}", GrouCombo.SelectedItem.ToString())).FirstOrDefault();
                    if (_g != null)
                    {
                        Arch.AddRange(App.Ds.Document.Where(o => o.Document.Equals(Categorie.Text) && o.AnnéeFormation.Equals(YearsOfStudent.SelectedItem.ToString()) && o.Niveau.Equals(_g.GroupeRow.FilièreAnnéeRow.FilièreRow.NiveauRow.Nom) && o.AnnéeÉtude.Equals(_g.GroupeRow.AnnéeÉtude) && o.Filière.Equals(_g.GroupeRow.FilièreAnnéeRow.Filière) && o.Groupe.Equals(_g.GroupeRow.Numéro)).Select(o => o.ArchiveRow));
                        DataGridData.DataContext = new ArchiveViewModel(Arch);
                        DataGridData.ItemsSource = ArchiveViewModel._Data;
                        Path.Text = $"{Categorie.Text} / {YearsOfStudent.Text} / {Groupe.Text}";
                    }
                }
                return;
            }
            Groupe.Text = "";
            Path.Text = "";
            DataGridData.ItemsSource = null;
            Categorie.IsEnabled = false;
            Arch.Clear();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var _impo = Impo.Where(o => o.Count == int.Parse(((Button)sender).Tag.ToString())).FirstOrDefault();
            if (_impo != null)
            {
                Impo.Remove(_impo);
                FileDropped.ItemsSource = null;
                FileDropped.ItemsSource = Impo;
            }
        }
    }

}