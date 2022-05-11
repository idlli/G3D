using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Windows.Controls;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for Bordereau.xaml
    /// </summary>
    public partial class Bordereau : UserControl
    {
        public Bordereau()
        {
            InitializeComponent();
            try
            {
                FilièreGroupeText.ItemsSource = App.Ds.Groupe.Select(o => $"{o.FilièreAnnéeRow.Filière} {o.Numéro}").Distinct().ToArray();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void BordereauClick(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                // can be multiplied
                var _r = App.Ds.DiplomeBordoreu.OrderByDescending(o => o.DateEnvoyé).Where(o =>
                    FilièreGroupeText.Text != "" ? string.Equals(FilièreGroupeText.Text, $"{o.Filière} {o.Numéro}") : true
                    &&
                    DateEnvoyé.SelectedDate != null ? SqlMethods.DateDiffDay(o.DateEnvoyé, DateEnvoyé.SelectedDate) == 0 : true);
                _ReportViewer.Reset();
                if (_r.Any())
                {
                    if (!_ReportViewer.Visible) _ReportViewer.Visible = true;

                    ReportDataSource _RptSource = new ReportDataSource("DataSet1", _r.CopyToDataTable());
                    _ReportViewer.LocalReport.DataSources.Add(_RptSource);
                    _ReportViewer.LocalReport.ReportEmbeddedResource = "G3D.Diplôme.Reports.Bordereau.rdlc";

                    ReportParameterCollection _params = new ReportParameterCollection();
                    if (Properties.Settings.Default.Établissement != "") _params.Add(new ReportParameter("Établissement", $"{App.Ds.Établissement.Where(o => o.Code == Properties.Settings.Default.Établissement).Select(o => o.Nom).FirstOrDefault()}"));
                    if (_params.Count > 0) _ReportViewer.LocalReport.SetParameters(_params);
                    _ReportViewer.LocalReport.SetParameters(_params);

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
