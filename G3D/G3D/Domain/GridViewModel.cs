using G3D.Diplôme;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace G3D.Domain
{
    public class GridViewModel : ViewModelBase
    {
        public static ObservableCollection<SelectableViewModel> _Data;
        public GridViewModel()
        {
            GridItems = CreateData();

            foreach (var model in GridItems)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(SelectableViewModel.IsSelected))
                    {
                        OnPropertyChanged(nameof(IsAllGridItemsSelected));
                    }
                };
            }
        }

        public GridViewModel(ObservableCollection<SelectableViewModel> MyList)
        {
            GridItems = MyList;

            foreach (var model in GridItems)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(SelectableViewModel.IsSelected))
                    {
                        OnPropertyChanged(nameof(IsAllGridItemsSelected));
                    }
                };
            }
        }

        public bool? IsAllGridItemsSelected
        {
            get
            {
                var selected = GridItems.Select(item => item.IsSelected).Distinct().ToList();
                var selectedItems = GridItems.Where(o => o.IsSelected).Select(item => item.IsSelected).ToList();
                Importer.Imp.SupprimerStagiaires.IsEnabled = Importer.Imp.Export.IsEnabled = selected.Count == 1 ? selected.Single() : true;
                Importer.Imp.ModifierStagiaire.IsEnabled = selected.Count == 1 ? (GridItems.Count() == 1 ? selected.Single() : (selectedItems.Count == 1 ? true : false)) : selectedItems.Count == 1 ? true : false;
                Importer.Imp.CountSelectedItems.Text = selectedItems.Count.ToString();
                Importer.Imp.StagiairesCount.Text = GridItems.Count.ToString();
                return selected.Count == 1 ? selected.Single() : (bool?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, GridItems);
                    OnPropertyChanged();
                }
            }
        }

        //private void IsAnySelected
        //{
        //    //MessageBox.Show("I'n Here");
        ////var selected = GridItems.Select(item => item.IsSelected).ToList();
        ////if (selected.Any())
        ////{
        ////    ImporterDesStagiaires.Imp.SupprimerStagiaires.Visibility = Visibility.Visible;
        ////}
        ////else
        ////{
        ////    ImporterDesStagiaires.Imp.SupprimerStagiaires.Visibility = Visibility.Collapsed;
        ////}
        //    get
        //    {
        //        MessageBox.Show("I'n Here");

        //        var selected = GridItems.Select(item => item.IsSelected).Distinct().ToList();
        //    }
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            SelectAll(value.Value, GridItems);
        //            OnPropertyChanged();
        //        }
        //}

        private static void SelectAll(bool select, IEnumerable<SelectableViewModel> models)
        {
            foreach (var model in models.Where(o => Importer.Imp.RéguliersChecked.IsChecked.Value ? string.Equals(o.TypeStagiaires, "Candidat Régulier") : (Importer.Imp.LibresChecked.IsChecked.Value ? string.Equals(o.TypeStagiaires, "Candidat Libre") : true)))
            {
                model.IsSelected = select;
            }
        }

        private static ObservableCollection<SelectableViewModel> CreateData()
        {
            _Data = new ObservableCollection<SelectableViewModel>();
            foreach (DataSet1.StagiaireGroupeRow s in App.Ds.StagiaireGroupe)
            {
                _Data.Add(new SelectableViewModel
                {
                    Cef = s.StagiaireRow.Cef,
                    NomPrénom = s.StagiaireRow.NomPrénom,
                    Cin = s.StagiaireRow.Cin,
                    Niveau = s.GroupeRow.FilièreAnnéeRow.FilièreRow.NiveauRow.Nom,
                    TypeFormation = s.GroupeRow.TypeFormation,
                    AnnéeÉtude = s.GroupeRow.AnnéeÉtudeRow.Nom,
                    Classement = s.Classement,
                    Admis = s.Admis,
                    TypeStagiaires = s.GroupeRow.TypeStagiairesRow.Nom,
                    AnnéeFormation = s.GroupeRow.FilièreAnnéeRow.AnnéeFormation,
                    Groupe = s.GroupeRow.FilièreAnnéeRow.Filière + " " + s.GroupeRow.Numéro,
                    Établissement = s.GroupeRow.FilièreAnnéeRow.ÉtablissementRow.Nom,
                    Filière = s.GroupeRow.FilièreAnnéeRow.FilièreRow.Nom,
                    GroupeId = s.GroupeRow.Id
                });
            }
            return _Data;
        }

        public ObservableCollection<SelectableViewModel> GridItems { get; }

    }
}
