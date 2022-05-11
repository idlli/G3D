using G3D.Authentication;
using MaterialDesignThemes.Wpf;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace G3D
{
    /// <summary>
    /// Interaction logic for Tableau_de_bord.xaml
    /// </summary>
    public partial class Main : Window
    {

        public static Main Mad;

        int? _userId;
        public static string _fAnnee = null;
        public static string _sAnnee;
        string _dipStr = "Diplômes";
        string _dupStr = "Duplicatas";
        string _arcStr = "Documents";
        private bool id;

        public Main()
        {
            InitializeComponent();

            try
            {
                DbContext.ReFillFirstUsing();

                if (Properties.Settings.Default.RestezConnecté || Properties.Settings.Default.JustStayConnected)
                {
                    Mad = this;

                    Properties.Settings.Default.JustStayConnected = false;
                    Properties.Settings.Default.Save();

                    DbContext.UtilisateurConnectéAdapter.FindQuery(Properties.Settings.Default.ChaîneConnexion, ref _userId);

                    YearSelected.Visibility = Visibility.Collapsed;
                    Menu.Visibility = Visibility.Visible;
                    Grid.SetColumn(ContentGrid, 1);
                    Grid.SetColumnSpan(ContentGrid, 1);
                    if (Properties.Settings.Default.Section.Equals(_dipStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        YearSelected.Visibility = Visibility.Visible;
                        if (Properties.Settings.Default.AnnéeFormation != "")
                        {
                            _fAnnee = Properties.Settings.Default.AnnéeFormation.Substring(0, 4);
                            _sAnnee = Properties.Settings.Default.AnnéeFormation.Substring(5);
                            YearSelectedContent.Text = $"{_fAnnee} - {_sAnnee}";
                        }
                        else
                        {
                            YearSelectedContent.Text = "Toutes les années";
                        }
                        if (Properties.Settings.Default.Établissement != "")
                        {
                            Etab.Text = App.Ds.Établissement.Where(o => o.Code.Equals(Properties.Settings.Default.Établissement)).Select(o => o.Nom).First();
                        }
                        else
                        {
                            Etab.Text = "Toutes les établissement";
                        }
                        Menu.Children.Add(new Diplôme.Menu());
                    }
                    else if (Properties.Settings.Default.Section.Equals(_dupStr, StringComparison.InvariantCultureIgnoreCase)) Menu.Children.Add(new Duplicata.Menu());
                    else
                    {
                        DbContext.ReFillArchive();

                        ContentGrid.Children.Clear();
                        SelectedItemTitle.Text = "Importer";
                        ContentGrid.Children.Add(new Archive.Importer());
                        Title = "Documents";

                        Menu.Visibility = Visibility.Collapsed;
                        Grid.SetColumn(ContentGrid, 0);
                        Grid.SetColumnSpan(ContentGrid, 2);
                    }

                    var _user = DbContext.UtilisateurAdapter.GetData().Where(o => int.Equals(o.Id, _userId)).First();
                    LName.Text = _user.NomFamille;
                    FName.Text = _user.Prénom;
                    try
                    {
                        Profile.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_user.PhotoProfil));
                    }
                    catch
                    {
                        Profile.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Path.GetFullPath(@".\Archive\Photos de profil\default.png"), UriKind.Absolute));
                    }

                    MyErrorSnackbarBorder.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(20));
                    MyAlertSnackbarBorder.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(20));
                    MyCheckSnackbarBorder.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(20));
                    MyInformationSnackbarBorder.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(20));

                }
                else
                {
                    Properties.Settings.Default.ChaîneConnexion = "";
                    Properties.Settings.Default.Section = "";
                    Properties.Settings.Default.AnnéeFormation = "";
                    Properties.Settings.Default.Établissement = "";
                    Properties.Settings.Default.Save();
                    Connexion _connexion = new Connexion();
                    _connexion.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

        private void PopupBoxContent_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Toggle_Icon.Kind == MaterialDesignThemes.Wpf.PackIconKind.ChevronUp)
                Toggle_Icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ChevronDown;
            else
                Toggle_Icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ChevronUp;
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            if (!PopupBox.IsPopupOpen)
                PopupBox.IsPopupOpen = true;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.Card PopupBoxCard = ((MaterialDesignThemes.Wpf.Card)PopupBoxContent.Parent);
            PopupBoxCard.Margin = new Thickness(0, 33, 0, 0);
        }



        private void ChangementClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Menu.Visibility = Visibility.Visible;
                Grid.SetColumn(ContentGrid, 1);
                Grid.SetColumnSpan(ContentGrid, 1);
                if (Diplôme.IsChecked.Value)
                {
                    YearSelected.Visibility = Visibility.Visible;
                    if (!(Properties.Settings.Default.Section == "Diplômes"))
                    {
                        Properties.Settings.Default.AnnéeFormation = OutlinedComboBox.SelectedValue == null ? "" : OutlinedComboBox.SelectedValue.ToString();
                        Properties.Settings.Default.Section = "Diplômes";
                        Properties.Settings.Default.Save();
                        this.Title = "Diplômes";
                        if (Properties.Settings.Default.AnnéeFormation != "")
                        {
                            _fAnnee = Properties.Settings.Default.AnnéeFormation.Substring(0, 4);
                            _sAnnee = Properties.Settings.Default.AnnéeFormation.Substring(5);
                            YearSelectedContent.Text = $"{_fAnnee} - {_sAnnee}";
                        }
                        else
                        {
                            _fAnnee = null;
                            YearSelectedContent.Text = "Toutes les années";
                        }
                        Menu.Children.Clear();
                        Menu.Children.Add(new Diplôme.Menu());
                    }
                    else if (OutlinedComboBox.Text != Properties.Settings.Default.AnnéeFormation)
                    {
                        Properties.Settings.Default.AnnéeFormation = OutlinedComboBox.SelectedValue == null ? "" : OutlinedComboBox.SelectedValue.ToString();
                        Properties.Settings.Default.Section = "Diplômes";
                        Properties.Settings.Default.Save();
                        this.Title = "Diplômes";
                        if (Properties.Settings.Default.AnnéeFormation != "")
                        {
                            _fAnnee = Properties.Settings.Default.AnnéeFormation.Substring(0, 4);
                            _sAnnee = Properties.Settings.Default.AnnéeFormation.Substring(5);
                            YearSelectedContent.Text = $"{_fAnnee} - {_sAnnee}";
                        }
                        else
                        {
                            _fAnnee = null;
                            YearSelectedContent.Text = "Toutes les années";
                        }
                        Menu.Children.Clear();
                        Menu.Children.Add(new Diplôme.Menu());
                    }
                }
                else if (Duplicata.IsChecked.Value)
                {
                    if (!(Properties.Settings.Default.Section == "Duplicatas"))
                    {
                        this.Title = "Duplicatas";
                        Properties.Settings.Default.Section = "Duplicatas";
                        Properties.Settings.Default.Save();
                        Menu.Children.Clear();
                        Menu.Children.Add(new Duplicata.Menu());

                        YearSelected.Visibility = Visibility.Collapsed;
                    }
                }
                else if (Archive.IsChecked.Value)
                {
                    if (!(Properties.Settings.Default.Section == "Documents"))
                    {
                        Menu.Children.Clear();

                        DbContext.ReFillArchive();


                        this.Title = "Documents";
                        Properties.Settings.Default.Section = "Documents";
                        Properties.Settings.Default.Save();
                        Main.Mad.ContentGrid.Children.Clear();
                        Main.Mad.SelectedItemTitle.Text = "Importer";
                        Main.Mad.ContentGrid.Children.Add(new Archive.Importer());

                        YearSelected.Visibility = Visibility.Collapsed;

                        Menu.Visibility = Visibility.Collapsed;
                        Grid.SetColumn(ContentGrid, 0);
                        Grid.SetColumnSpan(ContentGrid, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }

        }

        private void Diplôme_Checked(object sender, RoutedEventArgs e)
        {
            if (OutlinedComboBox != null)
                OutlinedComboBox.Visibility = Visibility.Visible;
        }

        private void Diplôme_Unchecked(object sender, RoutedEventArgs e)
        {
            OutlinedComboBox.Visibility = Visibility.Collapsed;
        }

        private void OutlinedComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BackUpFiles.Visibility = Visibility.Collapsed;
                DialogContent.Visibility = Visibility.Visible;
                if (!App.Ds.FilièreAnnée.Any())
                    DbContext.FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, string.Empty);
                OutlinedComboBox.ItemsSource = App.Ds.FilièreAnnée.Select(x => x.AnnéeFormation).Distinct();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BackUpFiles.Visibility = Visibility.Visible;
            DialogContent.Visibility = Visibility.Collapsed;
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

        bool _isDirty;
        char[] invalidChars = Path.GetInvalidFileNameChars();

        private void BackupDatabaseFull(Server myServer, Database myDatabase)
        {
            try
            {
                Backup bkpDBFull = new Backup();
                bkpDBFull.Action = BackupActionType.Database;
                bkpDBFull.Database = myDatabase.Name;
                string _path = Path.Combine(ExcelPath.Text, ExcelFileTitle.Text != "" ? $"{ExcelFileTitle.Text}" : $"{_DefaultFolderName}");
                System.IO.Directory.CreateDirectory(_path);
                if (System.IO.Directory.Exists(_path))
                {
                    bkpDBFull.Devices.AddDevice(Path.Combine(_path, $"{myDatabase.Name}.bak"), DeviceType.File);
                    bkpDBFull.BackupSetName = $"{myDatabase.Name} Sauvegarde de la base de données";
                    bkpDBFull.BackupSetDescription = $"{myDatabase.Name} Base de données - Sauvegarde complète";
                    bkpDBFull.Initialize = false;
                    bkpDBFull.SqlBackup(myServer);
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }
        private void StartingBackUp()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (DatabaseGroup.IsChecked.Value)
                {
                    Server myServer = new Server(@".\SQLEXPRESS");
                    try
                    {
                        myServer.ConnectionContext.LoginSecure = true;
                        myServer.ConnectionContext.Connect();
                        Database myDatabase = myServer.Databases["G3D"];
                        BackupDatabaseFull(myServer, myDatabase);
                    }
                    catch (Exception ex)
                    {
                        Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
                    }
                    finally
                    {
                        if (myServer.ConnectionContext.IsOpen)
                            myServer.ConnectionContext.Disconnect();
                    }

                }
                if (ArchiveGroup.IsChecked.Value)
                {
                    string TargrtDirectory = Path.Combine(Path.Combine(ExcelPath.Text, (ExcelFileTitle.Text != string.Empty ? ExcelFileTitle.Text : _DefaultFolderName)), @".\Archive");
                    copy_dir(Path.GetFullPath(@".\Archive"), TargrtDirectory);
                }
            }));
        }
        private void copy_dir(string source, string dest)
        {
            try
            {
                if (String.IsNullOrEmpty(source) || String.IsNullOrEmpty(dest)) return;
                Directory.CreateDirectory(dest);
                foreach (string fn in Directory.GetFiles(source))
                {
                    File.Copy(fn, Path.Combine(dest, Path.GetFileName(fn)), true);
                }
                foreach (string dir_fn in Directory.GetDirectories(source))
                {
                    copy_dir(dir_fn, Path.Combine(dest, Path.GetFileName(dir_fn)));
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }
        string TargrtDirectory;
        
        string _DefaultFolderName;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                _isDirty = false;
                _DefaultFolderName = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH'-'mm'-'ss");

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


                StatusInPercent.Visibility = Visibility.Visible;
                if (DatabaseGroup.IsChecked.Value && ArchiveGroup.IsChecked.Value) FullPercent.Text = "En cours de sauvegarder la base de données et copier les fichiers de l'archive";
                else if (DatabaseGroup.IsChecked.Value) FullPercent.Text = "En cours de sauvegarder la base de données";
                else FullPercent.Text = "En cours de copier les fichiers de l'archive";

                if (Bw.IsBusy)
                {
                    return;
                }

                System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();

                Bw.DoWork += (bwSender, bwArg) =>
                {
                    sWatch.Start();
                    StartingBackUp();
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
                        StatusInPercent.Visibility = Visibility.Collapsed;
                        MainDialog.IsOpen = false;
                        Main.Mad.MyCheckSnackbarIcon.MessageQueue.Enqueue("Sauvegarde terminée avec succès");
                    }));
                };

                Bw.RunWorkerAsync();
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

        private void DatabaseGroup_Checked(object sender, RoutedEventArgs e)
        {
            DirectoryInfos.IsEnabled = true;
            StartBackUp.IsEnabled = true;

            JustCheckedDataBase = true;
        }

        private void DatabaseGroup_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!DatabaseGroup.IsChecked.Value && !ArchiveGroup.IsChecked.Value)
            {
                DirectoryInfos.IsEnabled = false;
                StartBackUp.IsEnabled = false;
            }
        }

        public static string CurrentPage;
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            Menu.Children.Clear();
            CurrentPage = Title;
            this.Title = "Profil";
            YearSelected.Visibility = Visibility.Collapsed;

            Menu.Visibility = Visibility.Collapsed;
            Grid.SetColumn(ContentGrid, 0);
            Grid.SetColumnSpan(ContentGrid, 2);

            ContentGrid.Children.Clear();
            SelectedItemTitle.Text = "Mon profil";
            ContentGrid.Children.Add(new Profile());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.RestezConnecté = false;
                Properties.Settings.Default.JustStayConnected = false;
                Properties.Settings.Default.ChaîneConnexion = "";
                Properties.Settings.Default.Section = "";
                Properties.Settings.Default.AnnéeFormation = "";
                Properties.Settings.Default.Établissement = "";
                Properties.Settings.Default.Save();
                Connexion _connexion = new Connexion();
                _connexion.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }
        private bool JustCheckedDataBase, JustCheckedArchive;

        private void DatabaseGroup_Click(object sender, RoutedEventArgs e)
        {
            if (JustCheckedDataBase)
            {
                JustCheckedDataBase = false;
                e.Handled = true;
                return;
            }
            if (DatabaseGroup.IsChecked.Value)
                DatabaseGroup.IsChecked = false;
        }

        private void ArchiveGroup_Checked(object sender, RoutedEventArgs e)
        {
            DirectoryInfos.IsEnabled = true;
            StartBackUp.IsEnabled = true;

            JustCheckedArchive = true;
        }

        private void ArchiveGroup_Click(object sender, RoutedEventArgs e)
        {
            if (JustCheckedArchive)
            {
                JustCheckedArchive = false;
                e.Handled = true;
                return;
            }
            if (ArchiveGroup.IsChecked.Value)
                ArchiveGroup.IsChecked = false;
        }
    }
}
