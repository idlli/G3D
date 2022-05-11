using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for ContactLauréats.xaml
    /// </summary>
    public partial class Contact : UserControl
    {
        public Contact()
        {
            InitializeComponent();
            try
            {
                FilièreBox.ItemsSource = App.Ds.FilièreAnnée.Select(o => o.Filière).Distinct();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void ContactClick(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var _r = App.Ds.ContactSelectQuery.Where(o =>
                    (FilièreBox.SelectedItem != null ? string.Equals(FilièreBox.SelectedItem.ToString(), o.Filière) : true)
                    &&
                    (SituationBox.SelectedIndex != -1 ? (o.IsSituationActuelNull() ? false : o.SituationActuel.Equals(SituationBox.Text, StringComparison.InvariantCultureIgnoreCase)) : true));
                _ReportViewer.Reset();
                if (_r.Any())
                {
                    if (!_ReportViewer.Visible) _ReportViewer.Visible = true;
                    ReportDataSource _RptSource = new ReportDataSource("DataSet1", _r.CopyToDataTable());
                    _ReportViewer.LocalReport.DataSources.Add(_RptSource);
                    _ReportViewer.LocalReport.ReportEmbeddedResource = "G3D.Diplôme.Reports.Contact.rdlc";
                    ReportParameterCollection _params = new ReportParameterCollection();
                    if (Main._fAnnee != null) _params.Add(new ReportParameter("AnnéeFormation", $"{Main._fAnnee} - {Main._sAnnee}"));
                    if (_params.Count > 0) _ReportViewer.LocalReport.SetParameters(_params);
                    _ReportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }
    }
}
