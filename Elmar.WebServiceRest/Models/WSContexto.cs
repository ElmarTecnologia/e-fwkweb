using Elmar.WebServiceRest;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Elmar.WebServiceRest
{
    public class WSContexto : DbContext
    {
        private static WSContexto AppContextoDAO;

        public WSContexto(string ConnectionString = "ConexaoPglWS")
            : base(ConnectionString){}

        public static WSContexto getAppContexto
        {
            get
            {
                AppContextoDAO = new WSContexto();
                if (AppContextoDAO == null)
                {
                    AppContextoDAO = new WSContexto();
                }
                return AppContextoDAO;
            }
        }

        //Objetos FWK mapeados
        public DbSet<SmsMobile> Sms { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<StoreGeneratedIdentityKeyConvention>();
        }

        public IEnumerable SelectionQuery(string query, string tableName = "")
        {
            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }
            
            IDbDataAdapter dtAdapater = this.setDataAdapter(query);
            var dsPg = new DataSet();
            var dtPg = new DataView();

            try
            {
                dtAdapater.Fill(dsPg);
                dsPg.Tables[0].TableName = !string.IsNullOrEmpty(tableName) ? tableName : "Table";
                dtPg = dsPg.Tables[0].AsDataView();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("22P05")) //Erro de conversão WIN1252 -> UTF8
                {
                    string result = string.Empty;
                    this.ExecuteQuery("set client_encoding = 'WIN1252'", out result);
                    dtAdapater.Fill(dsPg);
                }
                throw e;
            }

            Database.Connection.Close();
            return dtPg;
        }

        public bool ExecuteQuery(string query, out string result)
        {
            result = string.Empty;
            var res = 0;

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            IDbCommand command = this.setCommandSql(query);
            try
            {
                res = command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                result = ex.Message;
                return false;
            }
        }

        private IDbDataAdapter setDataAdapter(string query)
        {
            if (Database.Connection is MySql.Data.MySqlClient.MySqlConnection)
                return new MySql.Data.MySqlClient.MySqlDataAdapter(query, (MySql.Data.MySqlClient.MySqlConnection)Database.Connection);
            else
                return new Npgsql.NpgsqlDataAdapter(query, (Npgsql.NpgsqlConnection)Database.Connection);
        }

        private IDbCommand setCommandSql(string query)
        {
            if (Database.Connection is MySql.Data.MySqlClient.MySqlConnection)
                return new MySql.Data.MySqlClient.MySqlCommand(query, (MySql.Data.MySqlClient.MySqlConnection)Database.Connection);
            else
                return new NpgsqlCommand(query, (Npgsql.NpgsqlConnection)Database.Connection);
        }
    }
}
