using G3D.Duplicata;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace G3D.Domain
{
    public class DuplicataViewModel : ViewModelBase
    {
        public static ObservableCollection<DuplicataModel> _Data;
        public DuplicataViewModel()
        {
            GridItems = CreateData();

            foreach (var model in GridItems)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(DuplicataModel.IsSelected))
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
                Duplicatas.Dup.SupprimerDuplicatas.IsEnabled = Duplicatas.Dup.Export.IsEnabled = selected.Count == 1 ? selected.Single() : true;
                Duplicatas.Dup.ModifierDuplicata.IsEnabled = Duplicatas.Dup.Fiche.IsEnabled = selected.Count == 1 ? (GridItems.Count() == 1 ? selected.Single() : (selectedItems.Count == 1 ? true : false)) : selectedItems.Count == 1 ? true : false;
                Duplicatas.Dup.CountSelectedItems.Text = selectedItems.Count.ToString();
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

        private static void SelectAll(bool select, IEnumerable<DuplicataModel> models)
        {
            foreach (var model in models)
            {
                model.IsSelected = select;
            }
        }

        private static ObservableCollection<DuplicataModel> CreateData()
        {
            _Data = new ObservableCollection<DuplicataModel>();
            string _Session, _Lieu, _NSerie, _Cab, _NuméroTéléphonePremier, _NuméroTéléphoneDeuxième;
            foreach (DataSet1.DuplicataFicheRow s in App.Ds.DuplicataFiche)
            {

                try { _Session = s.Session; } catch { _Session = ""; }
                try { _Lieu = s.Lieu; } catch { _Lieu = ""; }
                try { _NSerie = s.NuméroSérie; } catch { _NSerie = ""; }
                try { _Cab = s.Cab; } catch { _Cab = ""; }
                try { _NuméroTéléphonePremier = s.NuméroTéléphonePremier; } catch { _NuméroTéléphonePremier = ""; }
                try { _NuméroTéléphoneDeuxième = s.NuméroTéléphoneDeuxième; } catch { _NuméroTéléphoneDeuxième = ""; }
                _Data.Add(new DuplicataModel
                {
                    Cef = s.Cef,
                    NomPrénom = s.NomPrénom,
                    Cin = s.Cin,
                    Niveau = s.Niveau,
                    ModeFormation = s.ModeFormation,
                    TypeFormation = s.TypeFormation,
                    TypeStagiaires = s.TypeStagiaire,
                    AnnéeFormation = s.AnnéeFormation,
                    Filière = s.Filière,
                    Groupe = s.Numéro,
                    Établissement = s.Établissement,
                    Tel1 = _NuméroTéléphonePremier,
                    Tel2 = _NuméroTéléphoneDeuxième,
                    DateNaissance = s.DateNaissance,
                    Session = _Session,
                    Lieu = _Lieu,
                    NSerie = _NSerie,
                    Cab = _Cab,
                    DupId = s.DupId
                });
            }
            return _Data;
        }

        public ObservableCollection<DuplicataModel> GridItems { get; }

    }
}
