using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CD_Main
{
    class Utilities
    {
        public static DataTable AuthorTable
        {
            get
            {
                DataTable ret = new DataTable();
                using (SqlConnection connect = new SqlConnection(Properties.Settings.Default.ConnexionString))
                {
                    connect.Open();
                    using (SqlCommand sqlc = connect.CreateCommand())
                    {
                        sqlc.CommandType = CommandType.Text;
                        sqlc.CommandText = "SELECT * FROM dbo.Author ORDER BY nameAuthor";
                        SqlDataAdapter sda = new SqlDataAdapter(sqlc);
                        sda.Fill(ret);
                    }
                    connect.Close();
                }
                return ret;
            }
        }
        public static DataTable OpusTable
        {
            get
            {
                DataTable ret = new DataTable();
                using (SqlConnection connect = new SqlConnection(Properties.Settings.Default.ConnexionString))
                {
                    connect.Open();
                    using (SqlCommand sqlc = connect.CreateCommand())
                    {
                        sqlc.CommandType = CommandType.Text;
                        sqlc.CommandText = "SELECT * FROM dbo.Opus ORDER BY titleOpus";
                        SqlDataAdapter sda = new SqlDataAdapter(sqlc);
                        sda.Fill(ret);
                    }
                    connect.Close();
                }
                return ret;
            }
        }
        public static DataTable FindByAuthorName(string name)
        {
            DataTable ret = new DataTable();
            using (SqlConnection connect = new SqlConnection(Properties.Settings.Default.ConnexionString))
            {
                connect.Open();
                using (SqlCommand sqlc = connect.CreateCommand())
                {
                    string cmd = "SELECT o.titleOpus AS 'Titre', d.numberDocument AS 'Côte', g.denominationGenre AS 'Genre', d.discardedDocument AS 'Perdu'";
                    cmd += " FROM dbo.Document AS d, dbo.Author AS a, dbo.Genre AS g, dbo.Author_creates_Opus AS aco, dbo.Opus AS o";
                    cmd += " WHERE a.nameAuthor='" + name + "'";
                    cmd += " AND a.idAuthor = aco.idAuthor";
                    cmd += " AND o.idOpus = aco.idOpus";
                    cmd += " AND d.idOpus = o.idOpus";
                    cmd += " AND g.idGenre = o.idGenre";
                    sqlc.CommandType = CommandType.Text;
                    sqlc.CommandText = cmd;
                    SqlDataAdapter sda = new SqlDataAdapter(sqlc);
                    sda.Fill(ret);
                }
                connect.Close();
            }
            CompleteTable(ref ret);
            return ret;
        }
        public static DataTable FindByOpusTitle(string title)
        {
            DataTable ret = new DataTable();
            using (SqlConnection connect = new SqlConnection(Properties.Settings.Default.ConnexionString))
            {
                connect.Open();
                using (SqlCommand sqlc = connect.CreateCommand())
                {
                    string cmd = "SELECT o.titleOpus AS 'Titre', d.numberDocument AS 'Côte', g.denominationGenre AS 'Genre', d.discardedDocument AS 'Perdu'";
                    cmd += " FROM dbo.Document AS d, dbo.Genre AS g, dbo.Opus AS o";
                    cmd += " WHERE o.titleOpus = '" + title + "'";
                    cmd += " AND d.idOpus = o.idOpus";
                    cmd += " AND g.idGenre = o.idGenre";
                    sqlc.CommandType = CommandType.Text;
                    sqlc.CommandText = cmd;
                    SqlDataAdapter sda = new SqlDataAdapter(sqlc);
                    sda.Fill(ret);
                }
                connect.Close();
            }
            CompleteTable(ref ret);
            return ret;
        }
        static void CompleteTable(ref DataTable table)
        {
            table.Columns.Add(new DataColumn("Auteurs", typeof(string)));
            table.Columns.Add(new DataColumn("Disponible", typeof(bool)));
            using (SqlConnection connect = new SqlConnection(Properties.Settings.Default.ConnexionString))
            {
                connect.Open();
                using (SqlCommand sqlc = connect.CreateCommand())
                {
                    sqlc.CommandType = CommandType.Text;
                    string cmd;
                    foreach (DataRow row in table.Rows)
                    {
                        DataTable tbl = new DataTable();
                        cmd = "SELECT a.nameAuthor FROM dbo.Author AS a, dbo.Author_Creates_Opus AS aco, dbo.Document AS d, dbo.Opus AS o";
                        cmd += " WHERE d.numberDocument = '" + row["Côte"] + "'";
                        cmd += " AND d.idOpus = o.idOpus";
                        cmd += " AND aco.idOpus = o.idOpus";
                        cmd += " AND a.idAuthor = aco.idAuthor";
                        sqlc.CommandText = cmd;
                        SqlDataAdapter sda = new SqlDataAdapter(sqlc);
                        sda.Fill(tbl);
                        List<string> Auth = new List<string>();
                        foreach (DataRow r in tbl.Rows)
                        {
                            Auth.Add((string)r["nameAuthor"]);
                        }
                        row["Auteurs"] = string.Join(",", Auth.ToArray());
                        tbl = new DataTable();
                        cmd = "SELECT s.denominationLoanStatus AS dls FROM dbo.Document AS d, dbo.Loan AS l, dbo.LoanStatus AS s";
                        cmd += " WHERE d.numberDocument = '" + row["Côte"] + "'";
                        cmd += " AND l.idDocument = d.idDocument";
                        cmd += " AND l.idLoanStatus = s.idLoanStatus";
                        cmd += " ORDER BY l.dateLoan";
                        sqlc.CommandText = cmd;
                        sda = new SqlDataAdapter(sqlc);
                        sda.Fill(tbl);
                        row["Disponible"] = !(bool)row["Perdu"] && tbl.Rows.Count == 0 || (string)tbl.Rows[tbl.Rows.Count - 1]["dls"] == "Restitué";
                    }
                }
                connect.Close();
            }
        }
    }
}
