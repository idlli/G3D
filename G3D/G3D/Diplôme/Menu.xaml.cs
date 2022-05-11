using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for MenuDiplome.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void ListViewItem1_Unselected(object sender, RoutedEventArgs e)
        {
            TreeViewItemHeader.Foreground = new SolidColorBrush(Color.FromRgb(95, 99, 104));
            TreeName.FontWeight = FontWeights.Regular;
            TreeViewItemHeader.IsExpanded = false;
        }

        private void ListViewItem1_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItemHeader.Foreground = new SolidColorBrush(Color.FromRgb(57, 156, 244));
            TreeName.FontWeight = FontWeights.Medium;

        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TreeViewItemHeader.IsExpanded)
                TreeViewItemHeader.IsExpanded = false;
            else
            {
                TreeViewItemHeader.IsExpanded = true;
                TreeName.FontWeight = FontWeights.Medium;
            }
        }

        private void TreeViewItemHeader_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItemWithTreeView.IsSelected = true;
            TreeName.FontWeight = FontWeights.Medium;
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillRegistre();
            ((TreeViewItem)sender).Foreground = new SolidColorBrush(Color.FromRgb(57, 156, 244));
            if (TreeViewItemHeader.Items[0].Equals((TreeViewItem)sender))
            {
                
                Main.Mad.ContentGrid.Children.Clear();
                Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = Main.Mad.Title = "Registre des signatures";
                Main.Mad.ContentGrid.Children.Add(new Cahier());
            }
            else if (TreeViewItemHeader.Items[1].Equals((TreeViewItem)sender))
            {
                Main.Mad.ContentGrid.Children.Clear();
                Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = Main.Mad.Title = "Registre retrait du diplômes";
                Main.Mad.ContentGrid.Children.Add(new Registre());
            }
        }

        private void TreeViewItem_Unselected(object sender, RoutedEventArgs e)
        {
            ((TreeViewItem)sender).Foreground = new SolidColorBrush(Color.FromRgb(95, 99, 104));
        }

        private void TreeViewItemHeader_MouseEnter(object sender, MouseEventArgs e)
        {
            ListViewItemWithTreeView.IsSelected = true;
            TreeName.FontWeight = FontWeights.Medium;
        }


        private void TableauDeBordSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillTableauDeBord();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Tableau de bord";
            Main.Mad.ContentGrid.Children.Add(new Bord());
        }

        private void ImporterDesStagiairesSelected(object sender, RoutedEventArgs e)
        {
            Main.Mad.MainProgressBar.Visibility = Visibility.Visible;

            DbContext.ReFillImporterDesStagiaire();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Importer des stagiaires";
            Main.Mad.ContentGrid.Children.Add(new Importer());
        }

        private void ChecklistSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillCheckList();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Checklist";
            Main.Mad.ContentGrid.Children.Add(new Checklist());
        }

        private void RetraitDuDiplômeSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillRetrait();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Retrait du diplôme";
            Main.Mad.ContentGrid.Children.Add(new Retrait());
        }
        private void ÉtapesDeSignatureSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillEtapes();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Étapes de signature";
            Main.Mad.ContentGrid.Children.Add(new Étapes());
        }
        private void SuiviDesSignaturesSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillSuiviDesSignatures();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Suivi des signatures";
            Main.Mad.ContentGrid.Children.Add(new Signatures());
        }
        private void BordereauSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillBordereau();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Bordereau";
            Main.Mad.ContentGrid.Children.Add(new Bordereau());
        }
        private void ContactLauréatsSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillContact();
            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Contact lauréats";
            Main.Mad.ContentGrid.Children.Add(new Contact());
        }
    }
}
