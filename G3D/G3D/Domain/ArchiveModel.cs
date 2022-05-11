using System;
using System.Windows.Media;

namespace G3D.Domain
{
    public class ArchiveModel : ViewModelBase
    {
        bool _isSelected;
        string _Type;
        string _Title;
        string _Size;
        DateTime _DateCréée;
        DateTime? _DateModifiée;
        SolidColorBrush _BackColor;
        int _IdFile;
        string _Path;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string Type
        {
            get => _Type;
            set => SetProperty(ref _Type, value);
        }

        public string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, value);
        }

        public string Size
        {
            get => _Size;
            set => SetProperty(ref _Size, value);
        }

        public DateTime DateCréée
        {
            get => _DateCréée;
            set => SetProperty(ref _DateCréée, value);
        }

        public DateTime? DateModifiée
        {
            get => _DateModifiée;
            set => SetProperty(ref _DateModifiée, value);
        }

        public int IdFile
        {
            get => _IdFile;
            set => SetProperty(ref _IdFile, value);
        }

        public string Path
        {
            get => _Path;
            set => SetProperty(ref _Path, value);
        }

        public SolidColorBrush BackColor
        {
            get => _BackColor;
            set => SetProperty(ref _BackColor, value);
        }

    }
}
