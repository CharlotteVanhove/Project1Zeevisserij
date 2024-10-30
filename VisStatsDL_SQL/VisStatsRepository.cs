using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using VisStatsBL.Enum;
using VisStatsBL.Exceptions;
using VisStatsBL.Interfaces;
using VisStatsBL.Model;

namespace VisStatsDL_SQL
{
    public class VisStatsRepository : IVisStatsRepository
    {
        private string _connectionString; //geen _ ??
        //voor data de package installeren microsoft.data.sqlclient
        public VisStatsRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool HeeftHaven(Haven haven)
        {

            string SQL = "SELECT Count(*) FROM Haven WHERE naam=@naam";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.Add(new SqlParameter("@naam", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters["@naam"].Value = haven.Naam;
                    int n = (int)cmd.ExecuteScalar();
                    if (n > 0) return true; else return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("HeeftHaven", ex);
                }
            }
        }

        public bool HeeftVissoort(Vissoort vis)
        {
            string SQL = "SELECT Count(*) FROM Soort WHERE naam=@naam";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.Add(new SqlParameter("@naam", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters["@naam"].Value = vis.Naam;
                    int n = (int)cmd.ExecuteScalar(); //geeft inhoud van 1ste rij en eerste kolom weer
                    if (n > 0) return true; else return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("HeeftVissoort", ex);
                }
            }
        }

        public void SchrijfHaven(Haven haven)
        {

            string SQL = "INSERT INTO Haven(naam) VALUES(@naam)";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.Add(new SqlParameter("@naam", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters["@naam"].Value = haven.Naam;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SchrijfHaven", ex);
                }
            }
        }

        public void SchrijfSoort(Vissoort vis)
        {
            string SQL = "INSERT INTO Soort(naam) VALUES(@naam)"; //altijd met parameters werken, voor sql server is het @
            using (SqlConnection conn = new SqlConnection(_connectionString)) //using wil zeggen dat hij gaat opkuisen, kan maar een beperkt aantal connecties leggen naar de databank.
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.Add(new SqlParameter("@naam", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters["@naam"].Value = vis.Naam;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SchrijfSoorten", ex);
                }
            }
        }

        public bool IsOpgeladen(string fileName)
        {
            string SQL = "SELECT Count(*) FROM upload WHERE filename=@filename";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.Add(new SqlParameter("@filename", System.Data.SqlDbType.NVarChar));
                    //cmd.Parameters.AddWithValue("@filename", fileName); //hetzelfde als lijn hierboven
                    cmd.Parameters["@filename"].Value = fileName.Substring(fileName.LastIndexOf("\\") + 1); //filename, het pad veranderen naar enkel filename
                    int n = (int)cmd.ExecuteScalar();
                    if (n > 0) return true; else return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("IsOpgeladen", ex);
                }
            }
        }

        public List<Haven> LeesHavens()
        {
            string SQL = "SELECT * FROM Haven";
            List<Haven> havens = new List<Haven>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    IDataReader reader = cmd.ExecuteReader(); //om file te lezen
                    while (reader.Read())
                    {
                        havens.Add(new Haven((int)reader["id"], (string)reader["naam"]));
                    }
                    return havens;
                }
                catch (Exception ex)
                {
                    throw new Exception("LeesHavens", ex);
                }
            }
        }

        public List<Vissoort> LeesVissoorten()
        {
            string SQL = "SELECT * FROM Soort";
            List<Vissoort> soorten = new List<Vissoort>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        soorten.Add(new Vissoort((int)reader["id"], (string)reader["naam"]));
                    }
                    return soorten;
                }
                catch (Exception ex)
                {
                    throw new Exception("LeesSoorten", ex);
                }
            }
        }

        public void SchrijfStatistieken(List<VisStatsDataRecord> data, string fileName)
        {
            //met transactie werken. alle gegevens wegschrijven of niets. om te voorkomen dat er fouten gebeuren tijden uploaden. dat al data zit in 1 tabel en de andere nog niet  
            //gaat hij zeker vragen op het examen en eindproef 
            //transactie altijd wanneer je in meerdere tabellen schrijft
            string SQLdata = "INSERT INTO VisStats(jaar,maand,haven_id,soort_id,gewicht,waarde) VALUES (@jaar, @maand, @haven_id, @soort_id, @gewicht, @waarde)";
            string SQLupload = "INSERT INTO Upload(fileNAme, datum,pad) VALUES(@fileNAme, @datum, @pad)";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQLdata;
                    cmd.Transaction = conn.BeginTransaction(); // dit niet vergeten voor transactie 
                    //addwithvalue nu niet, want anders maakt hij iedere keer een paramater aan. dus niet als je meerdere keren moet gebeuren. 
                    cmd.Parameters.Add(new SqlParameter("@jaar", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@maand", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@haven_id", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@soort_id", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@gewicht", SqlDbType.Float)); //double in c# is float in sql
                    cmd.Parameters.Add(new SqlParameter("@waarde", SqlDbType.Float));
                    foreach (VisStatsDataRecord d in data)
                    {
                        cmd.Parameters["@jaar"].Value = d.Jaar;
                        cmd.Parameters["@maand"].Value = d.Maand;
                        cmd.Parameters["@haven_id"].Value = d.Haven.ID;
                        cmd.Parameters["@soort_id"].Value = d.Soort.ID;
                        cmd.Parameters["@gewicht"].Value = d.Gewicht;
                        cmd.Parameters["@waarde"].Value = d.Waarde;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = SQLupload;
                    cmd.Parameters.Clear();
                    // nu wel addwithvalue want is maar 1 lijntje schrijven 
                    cmd.Parameters.AddWithValue("@fileName", fileName.Substring(fileName.LastIndexOf("\\") + 1));
                    cmd.Parameters.AddWithValue("@pad", fileName.Substring(0, fileName.LastIndexOf("\\") + 1));
                    cmd.Parameters.AddWithValue("@datum", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit(); //dit zeker onthouden, wanneer je wegschrijft in meerdere tabbellen tegelijk 

                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback(); // als er iets fout loopt, moet je rollback doen, brengt terug in toestand wanneer je begonnen bent. meeste van ado.net
                    throw new Exception("SchrijfStatistieken", ex);
                }
            }
        }

        public List<int> LeesJaartallen()
        {
            string SQL = "SELECT distinct jaar FROM visstats";
            List<int> jaartallen = new List<int>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        jaartallen.Add((int)reader["jaar"]);
                    }
                    return jaartallen;
                }
                catch (Exception ex)
                {
                    throw new Exception("LeesJaartallen", ex);
                }
            }
        }

        public List<JaarVangst> LeesStatistieken(int jaar, Haven haven, List<Vissoort> vissoorten, Eenheid eenheid)
        {
            string kolom = "";
            switch (eenheid)
            {
                case Eenheid.kg: kolom = "gewicht"; break;
                case Eenheid.euro: kolom = "waarde"; break;
            }
            string paramSoorten = "";
            for (int i = 0; i < vissoorten.Count; i++) paramSoorten += $"@ps{i},"; // "@ps0, @ps1,"
            paramSoorten = paramSoorten.Remove(paramSoorten.Length - 1); // laatste ',' verwijderen
            string SQL = $"SELECT soort_id,t2.naam soortnaam, jaar, sum({kolom}) totaal, min ({kolom}) minimum, max ({kolom}) maximum, avg({kolom}) gemiddelde " +
                $"FROM VisStats t1 left join soort t2 on t1.soort_id=t2.id " +
                $"WHERE jaar = @jaar and soort_id in ({paramSoorten}) and haven_id=@haven_id " +
                $"GROUP BY soort_id, t2.naam,jaar";
            List<JaarVangst> vangst = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddWithValue("@haven_id", haven.ID);
                    cmd.Parameters.AddWithValue("@jaar", jaar);
                    for (int i = 0; i < vissoorten.Count; i++) cmd.Parameters.AddWithValue($"@ps{i}", vissoorten[i].ID);
                   IDataReader reader = cmd.ExecuteReader(); //omdat het er meerdere zijn 
                    while (reader.Read())
                    {
                        vangst.Add(new JaarVangst((string)reader["soortnaam"], (double)reader["totaal"], (double)reader["minimum"], (double)reader["maximum"], (double)reader["gemiddelde"]));
                    }
                    return vangst;
                }
                catch (Exception ex)
                {
                    throw new Exception("LeesStatistieken", ex);
                }
            }
        }

        public List<Maandvangst> LeesMaandStatistieken(List<int> jaren, List<Haven> haven, Vissoort visSoort, Eenheid eenheid)
        {
            string kolom = "";
            switch (eenheid)
            {
                case Eenheid.kg: kolom = "gewicht"; break;
                case Eenheid.euro: kolom = "waarde"; break;
            }
            string SQL = $"SELECT jaar, maand, SUM({kolom}) totaal, MIN({kolom}) minimum, MAX({kolom}) maximum, AVG({kolom}) gemiddelde FROM VisStats t1 left JOIN Havens t2 ON t1.haven_id = t2.id WHERE soort_id = @soort_id AND haven_id IN (SELECT id FROM Havens) GROUP BY jaar, maand";
            List<Maandvangst> vangsten = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddWithValue("@soort_id", visSoort.ID);
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        vangsten.Add(new Maandvangst((int)reader["jaar"], (int)reader["maand"], (double)reader["totaal"], (double)reader["minimum"], (double)reader["maximum"], (double)reader["gemiddelde"]));
                    }
                }
                catch (DomeinException ex)
                {
                    throw new DomeinException("LeesMaandStatistieken", ex);
                }
            }
            return vangsten;
        }
    }
}
