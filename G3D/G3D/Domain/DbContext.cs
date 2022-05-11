using System.Configuration;
using System.Data.SqlClient;
using G3D.Properties;

namespace G3D
{
    internal class DbContext
    {
        public static SqlConnection SqlConnection { get; set; }
        public static SqlCommand SqlCommand { get; set; }
        public static SqlCommandBuilder SqlCommandBuilder { get; set; }
        public static SqlDataAdapter SqlDataAdapter { get; set; }
        public static SqlDataReader SqlDataReader { get; }
        public static string SqlConnectionString = Settings.Default.G3DConnectionString;

        public static DataSet1TableAdapters.UtilisateurTableAdapter UtilisateurAdapter = new DataSet1TableAdapters.UtilisateurTableAdapter();
        public static DataSet1TableAdapters.UtilisateurConnectéTableAdapter UtilisateurConnectéAdapter = new DataSet1TableAdapters.UtilisateurConnectéTableAdapter();
        public static DataSet1TableAdapters.CléProduitTableAdapter CléProduitAdapter = new DataSet1TableAdapters.CléProduitTableAdapter();

        public static DataSet1TableAdapters.FilièreTableAdapter FilièreAdapter = new DataSet1TableAdapters.FilièreTableAdapter();
        public static DataSet1TableAdapters.FilièreAnnéeTableAdapter FilièreAnnéeAdapter = new DataSet1TableAdapters.FilièreAnnéeTableAdapter();
        public static DataSet1TableAdapters.GroupeTableAdapter GroupeAdapter = new DataSet1TableAdapters.GroupeTableAdapter();
        ////public static DataSet1TableAdapters.AnnéeÉtudeTableAdapter AnnéeÉtudeAdapter = new DataSet1TableAdapters.AnnéeÉtudeTableAdapter();
        public static DataSet1TableAdapters.StagiaireTableAdapter StagiaireAdapter = new DataSet1TableAdapters.StagiaireTableAdapter();
        public static DataSet1TableAdapters.StagiaireGroupeTableAdapter StagiaireGroupeAdapter = new DataSet1TableAdapters.StagiaireGroupeTableAdapter();

        public static DataSet1TableAdapters.NiveauTableAdapter NiveauAdapter = new DataSet1TableAdapters.NiveauTableAdapter();
        public static DataSet1TableAdapters.ÉtablissementTableAdapter ÉtablissementAdapter = new DataSet1TableAdapters.ÉtablissementTableAdapter();
        public static DataSet1TableAdapters.AnnéeÉtudeTableAdapter AnnéeÉtudeAdapter = new DataSet1TableAdapters.AnnéeÉtudeTableAdapter();
        public static DataSet1TableAdapters.TypeStagiairesTableAdapter TypeStagiairesAdapter = new DataSet1TableAdapters.TypeStagiairesTableAdapter();

        //public static DataSet1TableAdapters.GetAdmisResultTableAdapter GetAdmisResultAdapter = new DataSet1TableAdapters.GetAdmisResultTableAdapter();

        public static DataSet1TableAdapters.StagiaireEmbaucheTableAdapter StagiaireEmbaucheAdapter = new DataSet1TableAdapters.StagiaireEmbaucheTableAdapter();
        public static DataSet1TableAdapters.EditéTableAdapter EditéAdapter = new DataSet1TableAdapters.EditéTableAdapter();
        public static DataSet1TableAdapters.RejetéTableAdapter RejetéAdapter = new DataSet1TableAdapters.RejetéTableAdapter();
        public static DataSet1TableAdapters.CorrigéTableAdapter CorrigéAdapter = new DataSet1TableAdapters.CorrigéTableAdapter();
        public static DataSet1TableAdapters.EnvoyéTableAdapter EnvoyéAdapter = new DataSet1TableAdapters.EnvoyéTableAdapter();
        public static DataSet1TableAdapters.SignéTableAdapter SignéAdapter = new DataSet1TableAdapters.SignéTableAdapter();

        public static DataSet1TableAdapters.StagiaireRetraitTableAdapter StagiaireRetraitAdapter = new DataSet1TableAdapters.StagiaireRetraitTableAdapter();
        public static DataSet1TableAdapters.RetraitDiplômeTableAdapter RetraitDiplômeAdapter = new DataSet1TableAdapters.RetraitDiplômeTableAdapter();
        public static DataSet1TableAdapters.RetraitBaccalauréatTableAdapter RetraitBaccalauréatAdapter = new DataSet1TableAdapters.RetraitBaccalauréatTableAdapter();

        //public static DataSet1TableAdapters.GetStagiaireCountTableAdapter GetStagiaireCountAdapter = new DataSet1TableAdapters.GetStagiaireCountTableAdapter();

        public static DataSet1TableAdapters.DiplomeRegistreRetraitTableAdapter DiplomeRegistreRetraitAdapter = new DataSet1TableAdapters.DiplomeRegistreRetraitTableAdapter();
        public static DataSet1TableAdapters.DiplomeRegistreRetraitFullTableAdapter DiplomeRegistreRetraitFulldapter = new DataSet1TableAdapters.DiplomeRegistreRetraitFullTableAdapter();


        public static DataSet1TableAdapters.SuiviDesSignaturesTableAdapter SuiviDesSignaturesAdapter = new DataSet1TableAdapters.SuiviDesSignaturesTableAdapter();
        public static DataSet1TableAdapters.DiplomeBordoreuTableAdapter DiplomeBordoreuAdapter = new DataSet1TableAdapters.DiplomeBordoreuTableAdapter();

        //public static DataSet1TableAdapters.ContactTableAdapter ContactAdapter = new DataSet1TableAdapters.ContactTableAdapter();
        public static DataSet1TableAdapters.ContactSelectQueryTableAdapter ContactSelectQueryAdapter = new DataSet1TableAdapters.ContactSelectQueryTableAdapter();


        public static DataSet1TableAdapters.QueriesTableAdapter QueriesAdapter = new DataSet1TableAdapters.QueriesTableAdapter();

        public static DataSet1TableAdapters.CheckListViewTableAdapter CheckListViewAdapter = new DataSet1TableAdapters.CheckListViewTableAdapter();



        public static DataSet1TableAdapters.StagiaireÉtatSignatureTableAdapter StagiaireÉtatSignatureAdapter = new DataSet1TableAdapters.StagiaireÉtatSignatureTableAdapter();



        // Duplicata
        public static DataSet1TableAdapters.DuplicataTableAdapter DuplicataAdapter = new DataSet1TableAdapters.DuplicataTableAdapter();
        public static DataSet1TableAdapters.DuplicataFicheTableAdapter DuplicataFicheAdapter = new DataSet1TableAdapters.DuplicataFicheTableAdapter();
        public static DataSet1TableAdapters.DuplicataBordoreuTableAdapter DuplicataBordoreuAdapter = new DataSet1TableAdapters.DuplicataBordoreuTableAdapter();


        // Archive
        public static DataSet1TableAdapters.DocumentTableAdapter DocumentAdapter = new DataSet1TableAdapters.DocumentTableAdapter();
        public static DataSet1TableAdapters.NoteTableAdapter NoteAdapter = new DataSet1TableAdapters.NoteTableAdapter();
        public static DataSet1TableAdapters.ArchiveTableAdapter ArchiveAdapter = new DataSet1TableAdapters.ArchiveTableAdapter();



        public static void ReFillFirstUsing()
        {
            App.Ds.Clear();

            UtilisateurAdapter.Fill(App.Ds.Utilisateur);
            CléProduitAdapter.Fill(App.Ds.CléProduit);
            FilièreAnnéeAdapter.Fill(App.Ds.FilièreAnnée);
            ÉtablissementAdapter.Fill(App.Ds.Établissement);
        }

        public static void ReFillTableauDeBord()
        {
            App.Ds.Clear();

            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            GroupeAdapter.FillBy(App.Ds.Groupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireGroupeAdapter.FillBy(App.Ds.StagiaireGroupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireAdapter.FillBy(App.Ds.Stagiaire, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            EditéAdapter.FillBy(App.Ds.Edité, Properties.Settings.Default.Établissement, Main._fAnnee);
            RejetéAdapter.FillBy(App.Ds.Rejeté, Properties.Settings.Default.Établissement, Main._fAnnee);
            CorrigéAdapter.FillBy(App.Ds.Corrigé, Properties.Settings.Default.Établissement, Main._fAnnee);
            EnvoyéAdapter.FillBy(App.Ds.Envoyé, Properties.Settings.Default.Établissement, Main._fAnnee);
            SignéAdapter.FillBy(App.Ds.Signé, Properties.Settings.Default.Établissement, Main._fAnnee);
        }

        public static void ReFillImporterDesStagiaire()
        {
            App.Ds.Clear();

            ÉtablissementAdapter.Fill(App.Ds.Établissement);
            NiveauAdapter.Fill(App.Ds.Niveau);
            FilièreAdapter.Fill(App.Ds.Filière);
            AnnéeÉtudeAdapter.Fill(App.Ds.AnnéeÉtude);
            TypeStagiairesAdapter.Fill(App.Ds.TypeStagiaires);
            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, null);
            GroupeAdapter.FillBy(App.Ds.Groupe, Properties.Settings.Default.Établissement, Main._fAnnee, null);
            StagiaireGroupeAdapter.FillBy(App.Ds.StagiaireGroupe, Properties.Settings.Default.Établissement, Main._fAnnee, null);
            StagiaireAdapter.FillBy(App.Ds.Stagiaire, Properties.Settings.Default.Établissement, Main._fAnnee, null);
        }

        public static void ReFillCheckList()
        {
            App.Ds.Clear();

            ÉtablissementAdapter.Fill(App.Ds.Établissement);
            NiveauAdapter.Fill(App.Ds.Niveau);
            FilièreAdapter.Fill(App.Ds.Filière);
            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            GroupeAdapter.FillBy(App.Ds.Groupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireGroupeAdapter.FillBy(App.Ds.StagiaireGroupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireAdapter.FillBy(App.Ds.Stagiaire, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
        }

        public static void ReFillEtapes()
        {
            App.Ds.Clear();

            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            GroupeAdapter.FillBy(App.Ds.Groupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireGroupeAdapter.FillBy(App.Ds.StagiaireGroupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireAdapter.FillBy(App.Ds.Stagiaire, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");


            // needs (Properties.Settings.Default.Établissement, Main._fAnnee, "2A")
            StagiaireÉtatSignatureAdapter.Fill(App.Ds.StagiaireÉtatSignature);
            EditéAdapter.Fill(App.Ds.Edité);
            RejetéAdapter.Fill(App.Ds.Rejeté);
            CorrigéAdapter.Fill(App.Ds.Corrigé);
            EnvoyéAdapter.Fill(App.Ds.Envoyé);
            SignéAdapter.Fill(App.Ds.Signé);
        }

        public static void ReFillRegistre()
        {
            App.Ds.Clear();

            ÉtablissementAdapter.Fill(App.Ds.Établissement);
        }
        public static void ReFillSuiviDesSignatures()
        {
            App.Ds.Clear();

            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            GroupeAdapter.FillBy(App.Ds.Groupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireGroupeAdapter.FillBy(App.Ds.StagiaireGroupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireAdapter.FillBy(App.Ds.Stagiaire, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");

            // needs (Properties.Settings.Default.Établissement, Main._fAnnee, "2A")
            StagiaireÉtatSignatureAdapter.Fill(App.Ds.StagiaireÉtatSignature);
        }

        public static void ReFillBordereau()
        {
            App.Ds.Clear();

            ÉtablissementAdapter.Fill(App.Ds.Établissement);
            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            GroupeAdapter.FillBy(App.Ds.Groupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");

            // needs (Properties.Settings.Default.Établissement, Main._fAnnee, "2A")
            DiplomeBordoreuAdapter.Fill(App.Ds.DiplomeBordoreu);
        }
        public static void ReFillContact()
        {
            App.Ds.Clear();

            // 2A ??
            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            ContactSelectQueryAdapter.Fill(App.Ds.ContactSelectQuery, Properties.Settings.Default.Établissement, Main._fAnnee);
            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
        }

        //... Using ReClear Here ??
        public static void DiplômeReFill(string _annee, string _etab)
        {
            if (_annee != "")
            {
                Main._fAnnee = _annee.Substring(0, 4);
                Main._sAnnee = _annee.Substring(4);
                if (_etab != "")
                {
                    ////NiveauAdapter.Fill(App.Ds.Niveau);
                    FilièreAdapter.Fill(App.Ds.Filière);
                    GroupeAdapter.Fill(App.Ds.Groupe);
                    ////AnnéeÉtudeAdapter.Fill(App.Ds.AnnéeÉtude);
                    StagiaireAdapter.Fill(App.Ds.Stagiaire);
                    StagiaireGroupeAdapter.Fill(App.Ds.StagiaireGroupe);

                    StagiaireEmbaucheAdapter.Fill(App.Ds.StagiaireEmbauche);
                    EditéAdapter.Fill(App.Ds.Edité);
                    RejetéAdapter.Fill(App.Ds.Rejeté);
                    CorrigéAdapter.Fill(App.Ds.Corrigé);
                    EnvoyéAdapter.Fill(App.Ds.Envoyé);
                    SignéAdapter.Fill(App.Ds.Signé);

                    StagiaireRetraitAdapter.Fill(App.Ds.StagiaireRetrait);
                    RetraitDiplômeAdapter.Fill(App.Ds.RetraitDiplôme);
                    RetraitBaccalauréatAdapter.Fill(App.Ds.RetraitBaccalauréat);
                }
                else
                {
                    FilièreAdapter.Fill(App.Ds.Filière);
                }
            }
        }
        public static void ReFillDataSet()
        {

            //UtilisateurAdapter.Fill(App.Ds.Utilisateur);
            //CléProduitAdapter.Fill(App.Ds.CléProduit);

            ////NiveauAdapter.Fill(Main.Ds.Niveau);
            ////ÉtablissementAdapter.Fill(Main.Ds.Établissement);
            ////FilièreAdapter.Fill(Main.Ds.Filière);
            ////FilièreAnnéeAdapter.Fill(Main.Ds.FilièreAnnée);
            ////GroupeAdapter.Fill(Main.Ds.Groupe);
            ////StagiaireAdapter.Fill(Main.Ds.Stagiaire);
            ////StagiaireGroupeAdapter.Fill(Main.Ds.StagiaireGroupe);
        }


        public static void ReFillRetrait()
        {
            App.Ds.Clear();

            FilièreAdapter.Fill(App.Ds.Filière);
            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, null);
            GroupeAdapter.FillBy(App.Ds.Groupe, Properties.Settings.Default.Établissement, Main._fAnnee, null);
            StagiaireGroupeAdapter.FillBy(App.Ds.StagiaireGroupe, Properties.Settings.Default.Établissement, Main._fAnnee, null);
            StagiaireAdapter.FillBy(App.Ds.Stagiaire, Properties.Settings.Default.Établissement, Main._fAnnee, null);

            StagiaireRetraitAdapter.Fill(App.Ds.StagiaireRetrait);
            RetraitDiplômeAdapter.Fill(App.Ds.RetraitDiplôme);
            RetraitBaccalauréatAdapter.Fill(App.Ds.RetraitBaccalauréat);

            StagiaireEmbaucheAdapter.Fill(App.Ds.StagiaireEmbauche);

            StagiaireÉtatSignatureAdapter.Fill(App.Ds.StagiaireÉtatSignature);
        }


        public static void ReFillDuplicata()
        {
            App.Ds.Clear();

            ÉtablissementAdapter.Fill(App.Ds.Établissement);
            FilièreAdapter.Fill(App.Ds.Filière);
            NiveauAdapter.Fill(App.Ds.Niveau);
            FilièreAnnéeAdapter.Fill(App.Ds.FilièreAnnée);
            GroupeAdapter.Fill(App.Ds.Groupe);
            TypeStagiairesAdapter.Fill(App.Ds.TypeStagiaires);
            StagiaireGroupeAdapter.Fill(App.Ds.StagiaireGroupe);
            StagiaireAdapter.Fill(App.Ds.Stagiaire);
            DuplicataAdapter.Fill(App.Ds.Duplicata);
            DuplicataFicheAdapter.Fill(App.Ds.DuplicataFiche);

        }

        public static void ReFillFiche()
        {
            App.Ds.Clear();

            DuplicataFicheAdapter.Fill(App.Ds.DuplicataFiche);
        }

        public static void ReFillDuplicataEtapes()
        {
            App.Ds.Clear();

            DuplicataFicheAdapter.Fill(App.Ds.DuplicataFiche);
            StagiaireÉtatSignatureAdapter.Fill(App.Ds.StagiaireÉtatSignature);
            EditéAdapter.FillBy(App.Ds.Edité, Properties.Settings.Default.Établissement, Main._fAnnee);
            DuplicataAdapter.Fill(App.Ds.Duplicata);
        }

        public static void ReFillDuplicataBordereau()
        {
            App.Ds.Clear();

            ÉtablissementAdapter.Fill(App.Ds.Établissement);
            DuplicataBordoreuAdapter.Fill(App.Ds.DuplicataBordoreu);
            DuplicataFicheAdapter.Fill(App.Ds.DuplicataFiche);
        }

        public static void ReFillDuplicataRetrait()
        {
            App.Ds.Clear();

            DuplicataAdapter.Fill(App.Ds.Duplicata);
            StagiaireRetraitAdapter.Fill(App.Ds.StagiaireRetrait);
            RetraitDiplômeAdapter.Fill(App.Ds.RetraitDiplôme);

            FilièreAnnéeAdapter.FillBy(App.Ds.FilièreAnnée, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            GroupeAdapter.FillBy(App.Ds.Groupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireGroupeAdapter.FillBy(App.Ds.StagiaireGroupe, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");
            StagiaireAdapter.FillBy(App.Ds.Stagiaire, Properties.Settings.Default.Établissement, Main._fAnnee, "2A");

            StagiaireRetraitAdapter.Fill(App.Ds.StagiaireRetrait);
            RetraitDiplômeAdapter.Fill(App.Ds.RetraitDiplôme);

            DuplicataBordoreuAdapter.Fill(App.Ds.DuplicataBordoreu);
            StagiaireÉtatSignatureAdapter.Fill(App.Ds.StagiaireÉtatSignature);
        }

        public static void ReFillArchive()
        {
            ReClear();

            FilièreAdapter.Fill(App.Ds.Filière);
            NiveauAdapter.Fill(App.Ds.Niveau);
            FilièreAnnéeAdapter.Fill(App.Ds.FilièreAnnée);
            AnnéeÉtudeAdapter.Fill(App.Ds.AnnéeÉtude);
            GroupeAdapter.Fill(App.Ds.Groupe);
            ArchiveAdapter.Fill(App.Ds.Archive);
            DocumentAdapter.Fill(App.Ds.Document);
            NoteAdapter.Fill(App.Ds.Note);
        }

        public static void ReClear()
        {
            


            //ÉtablissementAdapter.Fill(App.Ds.Établissement);
            //FilièreAdapter.Fill(App.Ds.Filière);
            //NiveauAdapter.Fill(App.Ds.Niveau);
            //AnnéeÉtudeAdapter.Fill(App.Ds.AnnéeÉtude);
            //TypeStagiairesAdapter.Fill(App.Ds.TypeStagiaires);

        }
    }
}
