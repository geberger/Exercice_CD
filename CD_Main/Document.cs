using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CD_Main
{
    class Document
    {
        public Document(int id, int opus, string number, bool discarded)
        {
            idDocument = id;
            idOpus = opus;
            numberDocument = number;
            discardedDocument = discarded;
        }
        public int idDocument;
        public int idOpus;
        public string numberDocument;
        public bool discardedDocument;
    }
    class Documents
    {
        string connexionString;
        public Documents(string connexionString)
        {
            this.connexionString = connexionString;
        }
        public Document this[int id]
        {
            get
            {
                DataTable tbl = new DataTable();
                using (SqlConnection connect = new SqlConnection(Properties.Settings.Default.ConnexionString))
                {
                    connect.Open();
                    using (SqlCommand sqlc = connect.CreateCommand())
                    {
                        sqlc.CommandType = CommandType.Text;
                        sqlc.CommandText = "SELECT idDocument, idOpus, numberDocument, discardedDocument FROM dbo.Document WHERE idDocument = " + id;
                        SqlDataAdapter sda = new SqlDataAdapter(sqlc);
                        sda.Fill(tbl);
                    }
                    connect.Close();
                }
                return new Document((int)tbl.Rows[0]["idDocument"], (int)tbl.Rows[0]["idOpus"], (string)tbl.Rows[0]["numberDocument"], (bool)tbl.Rows[0]["discardedDocument"]);
            }
            set
            {
                DataTable tbl = new DataTable();
                using (SqlConnection connect = new SqlConnection(connexionString))
                {
                    connect.Open();
                    using (SqlCommand sqlc = connect.CreateCommand())
                    {
                        sqlc.CommandType = CommandType.Text;
                        sqlc.CommandText = "UPDATE dbo.Document idOpus = " + value.idOpus
                            + " numberDocument = " + value.numberDocument
                            + " discardedDocument = " + value.numberDocument
                            + " WHERE idDocument = " + value.idDocument;
                    }
                    connect.Close();
                }
            }
        }
        public int Add(int idOpus, string numberDocument, bool discardedDocument)
        {
            DataTable tbl = new DataTable();
            int ret = 0;
            using (SqlConnection connect = new SqlConnection(connexionString))
            {
                connect.Open();
                using (SqlCommand sqlc = connect.CreateCommand())
                {
                    sqlc.CommandType = CommandType.Text;
                    sqlc.CommandText = "INSERT INTO dbo.Document (idDocument, idOpus, numberDocument, discardedDocument) VALUES ((SELECT MAX(idDocument) FROM dbo.Document) + 1, "
                        + idOpus + ", '" + numberDocument + "', " + (discardedDocument ? 1 : 0).ToString() + ")";
                    sqlc.ExecuteNonQuery();
                }
                connect.Close();
            }
            using(SqlConnection connect = new SqlConnection(connexionString))
            {
                connect.Open();
                using(SqlCommand sqlc = connect.CreateCommand())
                {
                    sqlc.CommandType = CommandType.Text;
                    sqlc.CommandText = "SELECT MAX(idDocument) FROM dbo.Document";
                    ret = Convert.ToInt32(sqlc.ExecuteScalar());
                }
                connect.Close();
            }
            return ret;
        }
    }
}
