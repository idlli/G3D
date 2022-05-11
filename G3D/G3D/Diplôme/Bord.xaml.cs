using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace G3D.Diplôme
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 

    public partial class _stagiaireGroupeBy
    {
        public string NomCourt { get; set; }
        public string Admis { get; set; }
        public int CountBy { get; set; }

        public _stagiaireGroupeBy(string nomCourt, string admis, int countBy)
        {
            NomCourt = nomCourt;
            Admis = admis;
            CountBy = countBy;
        }
    }
    public partial class Bord : UserControl
    {
        public int totalAdmis { get; set; }
        public Bord()
        {
            InitializeComponent();

            try
            {
                totalAdmis = App.Ds.StagiaireGroupe.Where(o => o.Admis.Equals("Oui", StringComparison.InvariantCultureIgnoreCase)).Count();
                RéguliersAdmis.Content = App.Ds.StagiaireGroupe.Where(o => o.Admis.Equals("Oui", StringComparison.InvariantCultureIgnoreCase) && o.GroupeRow.TypeStagiaires.Equals("CR", StringComparison.InvariantCultureIgnoreCase)).Count().ToString();
                LibresAdmis.Content = (totalAdmis - int.Parse(RéguliersAdmis.Content.ToString())).ToString();
                TotalAdmis.Content = totalAdmis.ToString();
                TotalNonAdmis.Content = App.Ds.StagiaireGroupe.Where(o => o.Admis.Equals("Non", StringComparison.InvariantCultureIgnoreCase)).Count().ToString();




                ChartValues<int> AdmisCount = new ChartValues<int>();
                ChartValues<int> NonAdmisCount = new ChartValues<int>();
                List<string> Reguliers = new List<string>();

                List<_stagiaireGroupeBy> _stagiaireGroupeBy = new List<_stagiaireGroupeBy>();
                var _stagiaireGroupeByLocal = App.Ds.StagiaireGroupe.Where(o => o.GroupeRow.TypeStagiaires.Equals("CR", StringComparison.InvariantCultureIgnoreCase)).GroupBy(o => new { o.GroupeRow.FilièreAnnéeRow.Filière, o.Admis }).Select(o => new { NomCourt = o.Key.Filière, Admis = o.Key.Admis, CountBy = o.Count() });

                foreach (var stg in _stagiaireGroupeByLocal)
                {
                    _stagiaireGroupeBy.Add(new _stagiaireGroupeBy(stg.NomCourt, stg.Admis, stg.CountBy));
                    if (_stagiaireGroupeByLocal.Where(o => o.NomCourt.Equals(stg.NomCourt, StringComparison.InvariantCultureIgnoreCase)).Count() < 2)
                    {
                        _stagiaireGroupeBy.Add(new _stagiaireGroupeBy(stg.NomCourt, stg.Admis.Equals("Oui", StringComparison.InvariantCultureIgnoreCase) ? "Non" : "Oui", 0));
                    }
                }
                _stagiaireGroupeBy = _stagiaireGroupeBy.OrderBy(o => o.NomCourt).ToList();
                Reguliers.AddRange(_stagiaireGroupeBy.Select(o => o.NomCourt).Distinct());
                AdmisCount.AddRange(_stagiaireGroupeBy.Where(o => o.Admis.Equals("Oui", StringComparison.InvariantCultureIgnoreCase)).Select(o => o.CountBy));
                NonAdmisCount.AddRange(_stagiaireGroupeBy.Where(o => o.Admis.Equals("Non", StringComparison.InvariantCultureIgnoreCase)).Select(o => o.CountBy));

                ReguliersLabels = Reguliers.ToArray();
                ReguliersLabelsValue.MaxValue = AdmisCount.Count == 0 ? 1 : AdmisCount.Count;
                ReguliersSeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title = "Admis",
                    Values = AdmisCount,
                    DataLabels = true
                },
                new StackedColumnSeries
                {
                    Title = "Non Admis",
                    Values = NonAdmisCount,
                    DataLabels = true
                }
            };
                ChartValues<int> LAdmisCount = new ChartValues<int>();
                ChartValues<int> LNonAdmisCount = new ChartValues<int>();
                List<string> Libres = new List<string>();
                _stagiaireGroupeBy.Clear();
                _stagiaireGroupeByLocal = App.Ds.StagiaireGroupe.Where(o => o.GroupeRow.TypeStagiaires.Equals("CL", StringComparison.InvariantCultureIgnoreCase)).GroupBy(o => new { o.GroupeRow.FilièreAnnéeRow.Filière, o.Admis }).Select(o => new { NomCourt = o.Key.Filière, Admis = o.Key.Admis, CountBy = o.Count() });
                foreach (var stg in _stagiaireGroupeByLocal)
                {
                    _stagiaireGroupeBy.Add(new _stagiaireGroupeBy(stg.NomCourt, stg.Admis, stg.CountBy));
                    if (_stagiaireGroupeByLocal.Where(o => o.NomCourt.Equals(stg.NomCourt, StringComparison.InvariantCultureIgnoreCase)).Count() < 2)
                    {
                        _stagiaireGroupeBy.Add(new _stagiaireGroupeBy(stg.NomCourt, stg.Admis.Equals("Oui", StringComparison.InvariantCultureIgnoreCase) ? "Non" : "Oui", 0));
                    }
                }
                _stagiaireGroupeBy = _stagiaireGroupeBy.OrderBy(o => o.NomCourt).ToList();
                Libres.AddRange(_stagiaireGroupeBy.Select(o => o.NomCourt).Distinct());
                LAdmisCount.AddRange(_stagiaireGroupeBy.Where(o => o.Admis.Equals("Oui", StringComparison.InvariantCultureIgnoreCase)).Select(o => o.CountBy));
                LNonAdmisCount.AddRange(_stagiaireGroupeBy.Where(o => o.Admis.Equals("Non", StringComparison.InvariantCultureIgnoreCase)).Select(o => o.CountBy));

                LibresLabels = Libres.ToArray();
                LibresLabelsValue.MaxValue = LAdmisCount.Count == 0 ? 1 : LAdmisCount.Count;
                LibresSeriesCollection = new SeriesCollection
                {
                    new StackedColumnSeries
                    {
                        Title = "Admis",
                        Values = LAdmisCount,
                        DataLabels = true
                    },
                    new StackedColumnSeries
                    {
                        Title = "Non Admis",
                        Values = LNonAdmisCount,
                        DataLabels = true
                    }
                };


                int embauchesCount = App.Ds.Stagiaire.Where(o => o.IsSituationActuelNull() ? false : o.SituationActuel.Equals("Embauche", StringComparison.InvariantCultureIgnoreCase)).Count();
                int EditesCount = App.Ds.Edité.Count();
                int CorrigeeCount = App.Ds.Corrigé.Count();
                int RejeteeCount = App.Ds.Rejeté.Count();
                int SigneeCount = App.Ds.Signé.Count();
                int EnvoyeCount = App.Ds.Envoyé.Count();

                EmbauchesCount.Text = embauchesCount.ToString();


                EditeCount.Text = EditesCount.ToString();
                EnvoyCount.Text = EnvoyeCount.ToString();
                SigneCount.Text = SigneeCount.ToString();
                CorrigeCount.Text = CorrigeeCount.ToString();
                RejeteCount.Text = RejeteeCount.ToString();

                totalAdmis = totalAdmis == 0 ? 1 : totalAdmis;

                double EditeValue = ((double)EditesCount / totalAdmis) * 100;
                if (EditeValue < 50)
                {
                    EditeArrowTop.Visibility = Visibility.Collapsed;
                    EditeArrowBottom.Visibility = Visibility.Visible;
                }
                else
                {
                    EditeArrowTop.Visibility = Visibility.Visible;
                    EditeArrowBottom.Visibility = Visibility.Collapsed;
                }
                GaugeEdites.Value = EditeValue;

                double EnvoyeValue = ((double)EnvoyeCount / totalAdmis) * 100;
                if (EnvoyeValue < 50)
                {
                    EnvoyeArrowTop.Visibility = Visibility.Collapsed;
                    EnvoyArrowBottom.Visibility = Visibility.Visible;
                }
                else
                {
                    EnvoyeArrowTop.Visibility = Visibility.Visible;
                    EnvoyArrowBottom.Visibility = Visibility.Collapsed;
                }
                GaugeEnvoye.Value = EnvoyeValue;

                double SigneValue = ((double)SigneeCount / totalAdmis) * 100;
                if (SigneValue < 50)
                {
                    SigneArrowTop.Visibility = Visibility.Collapsed;
                    SigneArrowBottom.Visibility = Visibility.Visible;
                }
                else
                {
                    SigneArrowTop.Visibility = Visibility.Visible;
                    SigneArrowBottom.Visibility = Visibility.Collapsed;
                }
                GaugeSigne.Value = SigneValue;

                RejeteeCount = RejeteeCount == 0 ? 1 : RejeteeCount;
                double CorrigeValue = ((double)CorrigeeCount / RejeteeCount) * 100;
                if (CorrigeValue < 50)
                {
                    CorrigeArrowTop.Visibility = Visibility.Collapsed;
                    CorrigeArrowBottom.Visibility = Visibility.Visible;
                }
                else
                {
                    CorrigeArrowTop.Visibility = Visibility.Visible;
                    CorrigeArrowBottom.Visibility = Visibility.Collapsed;
                }
                GaugeCorriges.Value = CorrigeValue;

                double embauchesValue = ((double)embauchesCount / totalAdmis) * 100;
                if (embauchesValue < 50)
                {
                    EmbauchesArrowTop.Visibility = Visibility.Collapsed;
                    EmbauchesArrowBottom.Visibility = Visibility.Visible;
                }
                else
                {
                    EmbauchesArrowTop.Visibility = Visibility.Visible;
                    EmbauchesArrowBottom.Visibility = Visibility.Collapsed;
                }
                Embauches.Value = embauchesValue;

                totalAdmis = totalAdmis == 1 ? 0 : totalAdmis;

                DataContext = this;

                GaugeEdites.LabelFormatter = GaugeCorriges.LabelFormatter = GaugeSigne.LabelFormatter = Embauches.LabelFormatter = GaugeEnvoye.LabelFormatter = val => Math.Round(val, 2) + " %";
            }
            catch (Exception ex)
            {
                Main.Mad.MyErrorSnackbarBorder.MessageQueue.Enqueue(ex.Message, "Fermer", null);
            }
        }

        public SeriesCollection ReguliersSeriesCollection { get; set; }
        public SeriesCollection LibresSeriesCollection { get; set; }
        public string[] ReguliersLabels { get; set; }
        public string[] LibresLabels { get; set; }

    }
}
