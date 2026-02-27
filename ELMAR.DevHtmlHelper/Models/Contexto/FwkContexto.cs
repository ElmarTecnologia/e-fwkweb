using System;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Npgsql;
using Z.EntityFramework.Plus;

namespace ELMAR.DevHtmlHelper.Models
{
    [Serializable]
    public class FwkContexto : DbContext
    {
        public FwkContexto(string ConnectionString = "ConexaoPgl")
            : base(ConnectionString)
        {
        }

        //Objetos FWK mapeados
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioAlcada> UsuarioAlcadas { get; set; }
        public DbSet<UsuarioPerfil> UsuarioPerfis { get; set; }
        public DbSet<PerfilAutorizacao> PerfilAutorizacoes { get; set; }
        public DbSet<UsuarioAutorizacao> UsuarioAutorizacoes { get; set; }
        public DbSet<Chamada> Chamadas { get; set; }
        public DbSet<FwkConfig> Configs { get; set; }
        public DbSet<Fwk_MenuItem> Menus { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fwk_MenuItem>()
                .HasKey(i => new { i.ID, i.Contexto })
                .HasOptional(i => i.Pai)
                .WithMany(i => i.MenusPai).HasForeignKey(i => new { i.PaiID, i.Contexto });           

            modelBuilder.Conventions.Remove<StoreGeneratedIdentityKeyConvention>();
        }

        public IEnumerable SelectionQuery(string query)
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
                else
                    throw e;
            }

            Database.Connection.Close();
            return dtPg;
        }

        public bool ExecuteQuery(string query, out string result)
        {
            result = string.Empty;
            var res = 0;
            bool ok = false;

            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }

            IDbCommand command = this.setCommandSql(query);
            try
            {
                res = command.ExecuteNonQuery();
                ok = true;
            }
            catch (Exception ex)
            {
                result = ex.Message;
                ok = false;
            }
            finally { Database.Connection.Close(); }
            return ok;
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

        public static void UndoAll(DbContext context)
        {
            //detect all changes (probably not required if AutoDetectChanges is set to true)
            context.ChangeTracker.DetectChanges();

            //get all entries that are changed
            var entries = context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged).ToList();

            //somehow try to discard changes on every entry
            foreach (var dbEntityEntry in entries)
            {
                var entity = dbEntityEntry.Entity;

                if (entity == null) continue;

                if (dbEntityEntry.State == EntityState.Added)
                {
                    //if entity is in Added state, remove it. (there will be problems with Set methods if entity is of proxy type, in that case you need entity base type
                    var set = context.Set(entity.GetType());
                    set.Remove(entity);
                }
                else if (dbEntityEntry.State == EntityState.Modified)
                {
                    //entity is modified... you can set it to Unchanged or Reload it form Db??
                    dbEntityEntry.Reload();
                }
                else if (dbEntityEntry.State == EntityState.Deleted)
                    //entity is deleted... not sure what would be the right thing to do with it... set it to Modifed or Unchanged
                    dbEntityEntry.State = EntityState.Modified;
            }
        }
    }
}
