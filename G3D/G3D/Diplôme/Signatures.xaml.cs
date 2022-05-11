using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using G3D.Domain;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for SuiviDesSignatures.xaml
    /// </summary>
    public partial class Signatures : UserControl
    {
        public Signatures()
        {
            InitializeComponent();
        }

        private void AfficherÉtatSignature(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                DataTable ÉtatSignatureTable;
                List<NewSuiviDesSignatures> Ls = null;
                Ls =
                    ÉtatSignature.SelectedIndex == -1 ?
                    App.Ds.StagiaireGroupe.Select(o => new NewSuiviDesSignatures(o.StagiaireRow.Cef, o.StagiaireRow.NomPrénom, o.GroupeRow.FilièreAnnéeRow.Filière, o.GroupeRow.Numéro, o.GetStagiaireÉtatSignatureRows().Select(o => o.État).FirstOrDefault() != null ? o.GetStagiaireÉtatSignatureRows().Select(o => o.État).FirstOrDefault() : "Non Edité", o.GetStagiaireÉtatSignatureRows().Select(o => o.ÉtatDate).Any() ? o.GetStagiaireÉtatSignatureRows().Select(o => o.ÉtatDate).FirstOrDefault().ToShortDateString() : "", o.GroupeRow.FilièreAnnéeRow.Promotion)).ToList()
                    :
                    (
                    ÉtatSignature.SelectedIndex == 0 ? App.Ds.StagiaireGroupe.Where(o => !App.Ds.StagiaireÉtatSignature.Select(o => o.StagiaireGroupe).Contains(o.Id)).Select(o => new NewSuiviDesSignatures(o.StagiaireRow.Cef, o.StagiaireRow.NomPrénom, o.GroupeRow.FilièreAnnéeRow.Filière, o.GroupeRow.Numéro, "Non Edité", "", o.GroupeRow.FilièreAnnéeRow.Promotion)).ToList()
                    :
                    App.Ds.StagiaireGroupe.Where(o => o.GetStagiaireÉtatSignatureRows().Select(o => o.État).FirstOrDefault() != null ? o.GetStagiaireÉtatSignatureRows().Select(o => o.État).FirstOrDefault().Equals(ÉtatSignature.Text, StringComparison.InvariantCultureIgnoreCase) : false).Select(o => new NewSuiviDesSignatures(o.StagiaireRow.Cef, o.StagiaireRow.NomPrénom, o.GroupeRow.FilièreAnnéeRow.Filière, o.GroupeRow.Numéro, o.GetStagiaireÉtatSignatureRows().Select(o => o.État).FirstOrDefault(), o.GetStagiaireÉtatSignatureRows().Select(o => o.ÉtatDate).FirstOrDefault().ToShortDateString(), o.GroupeRow.FilièreAnnéeRow.Promotion)).OrderByDescending(o => o.ÉtatDate).Distinct().ToList());
                _ReportViewer.Reset();
                if (Ls != null)
                {
                    ReportDataSource _RptSource = new ReportDataSource("DataSet1", Table.ToDataTable<NewSuiviDesSignatures>(Ls));
                    _ReportViewer.LocalReport.DataSources.Add(_RptSource);
                    _ReportViewer.LocalReport.ReportEmbeddedResource = "G3D.Diplôme.Reports.Signatures.rdlc";
                    _ReportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void _ReportViewerLoad(object sender, System.EventArgs e)
        {

            //_ReportViewer.Reset();
            //ReportDataSource _RptSource = new ReportDataSource("DataSet1", (DataTable)App.Ds.SuiviDesSignatures);
            //_ReportViewer.LocalReport.DataSources.Add(_RptSource);
            //_ReportViewer.LocalReport.ReportEmbeddedResource = "DDA.Diplôme.Reports.Signatures.rdlc";
            //_ReportViewer.RefreshReport();
        }
    }

    internal class NewSuiviDesSignatures
    {
        public string Cef { get; }
        public string NomPrénom { get; }
        public string Filière { get; }
        public string Numéro { get; }
        public string État { get; }
        public string ÉtatDate { get; }
        public string Promotion { get; }

        public NewSuiviDesSignatures(string cef, string nomPrénom, string filière, string numéro, string état, string étatDate, string promotion)
        {
            Cef = cef;
            NomPrénom = nomPrénom;
            Filière = filière;
            Numéro = numéro;
            État = état;
            ÉtatDate = étatDate;
            Promotion = promotion;
        }
    }
}
