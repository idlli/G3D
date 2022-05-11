using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Windows.Controls;

namespace G3D.Duplicata
{
    /// <summary>
    /// Interaction logic for BordereauD.xaml
    /// </summary>
    public partial class Bordereau : UserControl
    {
        public Bordereau()
        {
            InitializeComponent();
            try
            {
                FilièreGroupeText.ItemsSource = App.Ds.DuplicataFiche.Select(o => $"{o.FilièreCourt} {o.Numéro}").Distinct().ToArray();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        string[] _full;
        private void BordereauClick(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                _ReportViewer.Reset();
                //_full = FilièreGroupeText.Text.Split(' ');
                var _dups = App.Ds.DuplicataBordoreu.OrderByDescending(o => o.DateEnvoyé).Where(o =>
                        FilièreGroupeText.Text != "" ? string.Equals(FilièreGroupeText.Text, $"{o.Filière} {o.Numéro}") : true
                        &&
                        DateEnvoyé.SelectedDate != null ? SqlMethods.DateDiffDay(o.DateEnvoyé, DateEnvoyé.SelectedDate) == 0 : true);
                _ReportViewer.Reset();
                if (_dups.Any())
                {
                    //var _StagiaireGroupeIds = App.Ds.Duplicata.Where(o => _dupIds.Contains(o.Id)).Select(o => o.StagiaireGroupe);
                    //if (_StagiaireGroupeIds.Any())
                    //{
                        //var _r = App.Ds.Envoyé.Where(o =>
                            //_StagiaireGroupeIds.Contains(o.StagiaireGroupe)
                            //&& DateEnvoyé.SelectedDate != null ? SqlMethods.DateDiffDay(o.DateEnvoyé, DateEnvoyé.SelectedDate) == 0 : true).Select(o => o.StagiaireGroupe);
                        //if (_r.Any())
                        //{
                        if (!_ReportViewer.Visible) _ReportViewer.Visible = true;
                        //var _dt = App.Ds.DuplicataFiche.Where(o => _r.Contains(o.StagiaireGroupe));
                        //if (_dt.Any())
                        //{
                        ReportDataSource _RptSource = new ReportDataSource("DataSet1", _dups.CopyToDataTable());
                        _ReportViewer.LocalReport.DataSources.Add(_RptSource);
                        _ReportViewer.LocalReport.ReportEmbeddedResource = "G3D.Duplicata.Reports.Bordereau.rdlc";
                        ReportParameterCollection _params = new ReportParameterCollection();
                        if (Properties.Settings.Default.Établissement != "") _params.Add(new ReportParameter("Établissement", $"{App.Ds.Établissement.Where(o => o.Code == Properties.Settings.Default.Établissement).Select(o => o.Nom).FirstOrDefault()}"));
                        if (_params.Count > 0) _ReportViewer.LocalReport.SetParameters(_params);
                        _ReportViewer.RefreshReport();
                            }
                        //}
                    //}
                }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }
    }
}
