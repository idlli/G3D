using G3D.Diplôme;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace G3D.Domain
{
    public class RetraitViewModel : ViewModelBase
    {
        public static ObservableCollection<SelectableViewModel> _Data = new ObservableCollection<SelectableViewModel>();
        public RetraitViewModel()
        {
            ListItems = _Data;

            foreach (var model in ListItems)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(SelectableViewModel.IsSelected))
                    {
                        OnPropertyChanged(nameof(IsAllListItemsSelected));
                    }
                };
            }
        }

        public bool? IsAllListItemsSelected
        {
            get
            {
                var selected = ListItems.Select(item => item.IsSelected).Distinct().ToList();
                if (Checklist.Ckl != null && Checklist.Ckl.IsVisible)
                {
                    Checklist.Ckl.SupprimerStagiaires.IsEnabled = Checklist.Ckl.ShowBtn.IsEnabled = selected.Count == 1 ? selected.Single() : true;
                    Checklist.Ckl.SelectedCount.Text = ListItems.Where(o => o.IsSelected).ToList().Count.ToString();
                }
                else if (Étapes.Etp != null && Étapes.Etp.IsVisible)
                {
                    Étapes.Etp.SupprimerStagiaires.IsEnabled = Étapes.Etp.Valider.IsEnabled = selected.Count == 1 ? selected.Single() : true;
                    Étapes.Etp.SelectedCount.Text = ListItems.Where(o => o.IsSelected).ToList().Count.ToString();
                }

                return selected.Count == 1 ? selected.Single() : (bool?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, ListItems);
                    OnPropertyChanged();
                }
            }
        }

        private static void SelectAll(bool select, IEnumerable<SelectableViewModel> models)
        {
            foreach (var model in models)
            {
                model.IsSelected = select;
            }
        }

        //private static ObservableCollection<SelectableViewModel> CreateData()
        //{
        //    _Data = new ObservableCollection<SelectableViewModel>();
        //    foreach (DataSet1.StagiaireRow s in App.Ds.Stagiaire)
        //    {
        //        _Data.Add(new SelectableViewModel
        //        {
        //            Cef = s.Cef,
        //            NomPrénom = s.NomPrénom
        //        });
        //    }
        //    return _Data;
        //}

        public ObservableCollection<SelectableViewModel> ListItems { get; }
    }
}
