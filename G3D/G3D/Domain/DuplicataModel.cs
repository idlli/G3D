using System;

namespace G3D.Domain
{
    public class DuplicataModel : ViewModelBase
    {
        bool _isSelected;
        string _Cef;
        string _NomPrénom;
        string _Cin;
        string _Niveau;
        string _ModeFormation;
        string _TypeFormation;
        string _TypeStagiaires;
        string _AnnéeFormation;
        string _Filière;
        string _Groupe;
        string _Établissement;
        string _Tel1;
        string _Tel2;
        DateTime _DateNaissance;
        string _Lieu;
        string _NSerie;
        string _Cab;
        string _Session;
        int _DupId;

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

        public string ModeFormation
        {
            get => _ModeFormation;
            set => SetProperty(ref _ModeFormation, value);
        }

        public string TypeFormation
        {
            get => _TypeFormation;
            set => SetProperty(ref _TypeFormation, value);
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

        public string Filière
        {
            get => _Filière;
            set => SetProperty(ref _Filière, value);
        }

        public string Groupe
        {
            get => _Groupe;
            set => SetProperty(ref _Groupe, value);
        }

        public string Établissement
        {
            get => _Établissement;
            set => SetProperty(ref _Établissement, value);
        }

        public string Tel1
        {
            get => _Tel1;
            set => SetProperty(ref _Tel1, value);
        }

        public string Tel2
        {
            get => _Tel2;
            set => SetProperty(ref _Tel2, value);
        }

        public DateTime DateNaissance
        {
            get => _DateNaissance;
            set => SetProperty(ref _DateNaissance, value);
        }

        public string Session
        {
            get => _Session;
            set => SetProperty(ref _Session, value);
        }

        public string Cab
        {
            get => _Cab;
            set => SetProperty(ref _Cab, value);
        }

        public string NSerie
        {
            get => _NSerie;
            set => SetProperty(ref _NSerie, value);
        }

        public string Lieu
        {
            get => _Lieu;
            set => SetProperty(ref _Lieu, value);
        }

        public int DupId
        {
            get => _DupId;
            set => SetProperty(ref _DupId, value);
        }
    }
}
