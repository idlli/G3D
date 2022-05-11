﻿using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Windows.Controls;
using System.Linq;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for RegistreRetrait.xaml
    /// </summary>
    public partial class Registre : UserControl
    {
        public Registre()
        {
            InitializeComponent();
        }
        private void _ReportViewerLoad(object sender, System.EventArgs e)
        {
            try
            {
                _ReportViewer.Reset();

                ReportDataSource _RptSource = new ReportDataSource("DataSet1", (DataTable)DbContext.DiplomeRegistreRetraitAdapter.GetData());
                _ReportViewer.LocalReport.DataSources.Add(_RptSource);
                _ReportViewer.LocalReport.ReportEmbeddedResource = "G3D.Diplôme.Reports.Registre.rdlc";
                ReportParameterCollection _params = new ReportParameterCollection();
                if (Properties.Settings.Default.Établissement != "") _params.Add(new ReportParameter("Établissement", $"{App.Ds.Établissement.Where(o => o.Code == Properties.Settings.Default.Établissement).Select(o => o.Nom).FirstOrDefault()}"));
                if (Main._fAnnee != null) _params.Add(new ReportParameter("AnnéeFormation", $"{Main._fAnnee} - {Main._sAnnee}"));
                if (_params.Count > 0) _ReportViewer.LocalReport.SetParameters(_params);
                _ReportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void ToggleButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                _ReportViewer.Reset();
                ReportDataSource _RptSource = new ReportDataSource("DataSet1", (DataTable)DbContext.DiplomeRegistreRetraitFulldapter.GetData());
                _ReportViewer.LocalReport.DataSources.Add(_RptSource);
                _ReportViewer.LocalReport.ReportEmbeddedResource = "G3D.Diplôme.Reports.Registre.rdlc";
                ReportParameterCollection _params = new ReportParameterCollection();
                if (Properties.Settings.Default.Établissement != "") _params.Add(new ReportParameter("Établissement", $"{App.Ds.Établissement.Where(o => o.Code == Properties.Settings.Default.Établissement).Select(o => o.Nom).FirstOrDefault()}"));
                if (Main._fAnnee != null) _params.Add(new ReportParameter("AnnéeFormation", $"{Main._fAnnee} - {Main._sAnnee}"));
                if (_params.Count > 0) _ReportViewer.LocalReport.SetParameters(_params);
                _ReportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        private void ToggleButton_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            _ReportViewerLoad(sender, null);
        }
    }
}
