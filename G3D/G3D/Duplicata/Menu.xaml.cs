using System.Windows;
using System.Windows.Controls;

namespace G3D.Duplicata
{
    /// <summary>
    /// Interaction logic for MenuDuplicata.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public static Menu Med;
        public static string FicheText { get; set; }

        public Menu()
        {
            InitializeComponent();
            Med = this;
        }
        private void DuplicatasSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillDuplicata();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Duplicatas";
            Main.Mad.ContentGrid.Children.Add(new Duplicatas());
        }

        private void FicheSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillFiche();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Fiche Duplicata";
            Main.Mad.ContentGrid.Children.Add(new Fiche());
        }
        private void ÉtapesDeSignatureSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillDuplicataEtapes();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Étapes de signature";
            Main.Mad.ContentGrid.Children.Add(new Étapes());
        }
        private void BordereauSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillDuplicataBordereau();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Bordereau";
            Main.Mad.ContentGrid.Children.Add(new Bordereau());
        }
        private void RetraitDuDiplômeSelected(object sender, RoutedEventArgs e)
        {
            DbContext.ReFillDuplicataRetrait();

            Main.Mad.ContentGrid.Children.Clear();
            Main.Mad.SelectedItemTitle.Text = Main.Mad.Title = "Retrait du diplôme";
            Main.Mad.ContentGrid.Children.Add(new Retrait());
        }
    }
}
