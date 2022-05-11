using G3D.Archive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace G3D.Domain
{
    public class ArchiveViewModel : ViewModelBase
    {
        public static ObservableCollection<ArchiveModel> _Data;
        public ArchiveViewModel(List<DataSet1.ArchiveRow> Arch)
        {
            GridItems = CreateData(Arch);

            foreach (var model in GridItems)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(ArchiveModel.IsSelected))
                    {
                        OnPropertyChanged(nameof(IsAllGridItemsSelected));
                        //OnPropertyChanged(nameof(IsMultipleSelected));
                    }
                };
            }
        }
        public ArchiveViewModel()
        {
            GridItems = _Data;

            foreach (var model in GridItems)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(ArchiveModel.IsSelected))
                    {
                        OnPropertyChanged(nameof(IsAllGridItemsSelected));
                        //OnPropertyChanged(nameof(IsMultipleSelected));
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
                Importer.Imp.SupprimerDocuments.IsEnabled = Importer.Imp.AfficherDocuments.IsEnabled = selected.Count == 1 ? selected.Single() : true;
                Importer.Imp.ModifierDocuments.IsEnabled = selected.Count == 1 ? (GridItems.Count() == 1 ? selected.Single() : (selectedItems.Count == 1 ? true : false)) : selectedItems.Count == 1 ? true : false;
                Importer.Imp.CountSelectedItems.Text = selectedItems.Count.ToString();
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

        private static void SelectAll(bool select, IEnumerable<ArchiveModel> models)
        {
            foreach (var model in models)
            {
                model.IsSelected = select;
            }
        }

        static SolidColorBrush _Pdf = new SolidColorBrush(Color.FromRgb(235, 59, 90));
        static SolidColorBrush _Doc = new SolidColorBrush(Color.FromRgb(56, 103, 214));
        static SolidColorBrush _Excel = new SolidColorBrush(Color.FromRgb(38, 222, 129));
        static SolidColorBrush _Other = new SolidColorBrush(Color.FromRgb(136, 84, 208));
        static SolidColorBrush _Default = new SolidColorBrush(Color.FromRgb(87, 96, 111));


        public static ObservableCollection<ArchiveModel> CreateData(List<DataSet1.ArchiveRow> Arch)
        {
            bool _errorFiles = false;
            _Data = new ObservableCollection<ArchiveModel>();
            foreach (DataSet1.ArchiveRow a in Arch)
            {
                try
                {
                    var f = new FileInfo(a.Path);
                    string ext = System.IO.Path.GetExtension(a.Path).ToLower();
                    DateTime? _dtM = null;
                    try { _dtM = a.DateModifiée; } catch { }
                    _Data.Add(new ArchiveModel
                    {
                        Type = System.IO.Path.GetExtension(a.Path),
                        Title = System.IO.Path.GetFileNameWithoutExtension(a.Path),
                        Size = string.Format("{0} Kb", (f.Length / 1024).ToString("N2")),
                        BackColor = ext.Equals(".txt") || ext.Equals(".rtf") ? _Default : (ext.Equals(".doc") || ext.Equals(".docx") ? _Doc : (ext.Equals(".htm") || ext.Equals(".html") || ext.Equals(".pdf") || ext.Equals(".ppt") || ext.Equals(".pptx") ? _Pdf : (ext.Equals(".xls") || ext.Equals(".xlsx")) ? _Excel : _Other)),
                        DateCréée = a.DateCréée,
                        DateModifiée = _dtM,
                        IdFile = a.Id,
                        Path = a.Path
                    });
                }
                catch
                {
                    _errorFiles = true;
                }

            }
            if (_errorFiles) Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue("There is Some Files was Not found In Main Directory Or There Is Some Other Issue", "Fermer", null);
            return _Data;
        }

        public ObservableCollection<ArchiveModel> GridItems { get; }

    }
}
