namespace G3D.Domain
{
    public class SelectableViewModel : ViewModelBase
    {
        bool _isSelected;
        string _Cef;
        string _NomPrénom;
        string _Cin;
        string _Niveau;
        string _TypeFormation;
        string _AnnéeÉtude;
        int _Classement;
        string _Admis;
        string _TypeStagiaires;
        string _AnnéeFormation;
        string _Groupe;
        string _Efp;
        string _Filière;
        int _GroupeId;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string Cef
        {
            get => _Cef;
            set => SetProperty(ref _Cef, value);
        }

        public string NomPrénom
        {
            get => _NomPrénom;
            set => SetProperty(ref _NomPrénom, value);
        }

        public string Cin
        {
            get => _Cin;
            set => SetProperty(ref _Cin, value);
        }

        public string Niveau
        {
            get => _Niveau;
            set => SetProperty(ref _Niveau, value);
        }

        public string TypeFormation
        {
            get => _TypeFormation;
            set => SetProperty(ref _TypeFormation, value);
        }

        public string AnnéeÉtude
        {
            get => _AnnéeÉtude;
            set => SetProperty(ref _AnnéeÉtude, value);
        }

        public int Classement
        {
            get => _Classement;
            set => SetProperty(ref _Classement, value);
        }

        public string Admis
        {
            get => _Admis;
            set => SetProperty(ref _Admis, value);
        }

        public string TypeStagiaires
        {
            get => _TypeStagiaires;
            set => SetProperty(ref _TypeStagiaires, value);
        }

        public string AnnéeFormation
        {
            get => _AnnéeFormation;
            set => SetProperty(ref _AnnéeFormation, value);
        }

        public string Groupe
        {
            get => _Groupe;
            set => SetProperty(ref _Groupe, value);
        }

        public string Établissement
        {
            get => _Efp;
            set => SetProperty(ref _Efp, value);
        }

        public string Filière
        {
            get => _Filière;
            set => SetProperty(ref _Filière, value);
        }

        public int GroupeId
        {
            get => _GroupeId;
            set => SetProperty(ref _GroupeId, value);
        }
    }
}
