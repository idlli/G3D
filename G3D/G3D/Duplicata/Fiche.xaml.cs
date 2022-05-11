using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace G3D.Duplicata
{
    /// <summary>
    /// Interaction logic for FicheDuplicata.xaml
    /// </summary>
    public partial class Fiche : UserControl
    {
        public Fiche()
        {
            InitializeComponent();

            try
            {
                DuplicataRech.ItemsSource = App.Ds.DuplicataFiche.Select(o => $"{o.NomPrénom} ~ {o.Cef} ~ [{o.DupId}]").Distinct().ToArray();
                if (!string.IsNullOrEmpty(Menu.FicheText))
                {
                    DuplicataRech.Text = Menu.FicheText;
                    Button_Click(null, null);
                }
            }
           catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        string[] _full;
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                _full = DuplicataRech.Text.Split('~');
                for (int i = 0; i < _full.Length; i++) { _full[i] = _full[i].Trim(); }
                _full[2] = _full[2].Substring(1, _full[2].Length - 2);
                var _r = App.Ds.DuplicataFiche.Where(o =>
                    o.DupId.ToString().Equals(_full[2])
                );
                if (_r.Any())
                {
                    if (!_ReportViewer.Visible) _ReportViewer.Visible = true;
                    _ReportViewer.Reset();
                    ReportDataSource _RptSource = new ReportDataSource("DataSet1", _r.CopyToDataTable());
                    _ReportViewer.LocalReport.DataSources.Add(_RptSource);
                    _ReportViewer.LocalReport.ReportEmbeddedResource = "G3D.Duplicata.Reports.Fiche.rdlc";
                    _ReportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void DuplicataRech_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DuplicataRech.Text.Length < 1) Afficher.IsEnabled = false;
                else Afficher.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }
    }
}
