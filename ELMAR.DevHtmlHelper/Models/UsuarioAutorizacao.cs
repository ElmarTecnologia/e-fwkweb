using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELMAR.DevHtmlHelper.Models
{
    [Table("fwk_usr_chamada_autoriza", Schema = "public")]
    public class UsuarioAutorizacao
    {
        private readonly FwkContexto _contexto = new FwkContexto();

        [Key]
        [Column("usu_aut_codigo", Order = 0)]
        public int UsuAutUsuCodigo { get; set; }
        [ForeignKey("UsuAutUsuCodigo")]
        public virtual Usuario Usuario { get; set; }
        [Key]
        [Column("usu_aut_chamada", Order = 1)]
        public string UsuAutChamada { get; set; }
        [Column("usu_aut_parametros")]
        public string UsuAutParametros { get; set; }
        [Column("usu_aut_ativo")]
        public bool UsuAutAtivo { get; set; }
        [NotMapped]
        public string Info { get; set; }
        private string _Redirect;
        [NotMapped]
        public string Redirect {
            get
            {
                return this._Redirect.Replace("\r", "").Replace("\n", "");
            }
            set {
                this._Redirect = value;
            }
        }
        [NotMapped]
        public string gridKey
        {
            get
            {
                return this.UsuAutUsuCodigo + "|" + this.UsuAutChamada;
            }
        }

        public virtual UsuarioAutorizacao getUsuAutorizacao(int UsuCodigo, string Chamada)
        {
            return (from u_alt in _contexto.UsuarioAutorizacoes where u_alt.UsuAutUsuCodigo == UsuCodigo 
                       && u_alt.UsuAutChamada.Equals(Chamada) select u_alt).FirstOrDefault();
        }

        public virtual List<UsuarioAutorizacao> getUsuAutorizacoes()
        {
            return (from u_alt in _contexto.UsuarioAutorizacoes where u_alt.UsuAutUsuCodigo == this.UsuAutUsuCodigo select u_alt).ToList<UsuarioAutorizacao>();
        }

        public object getParameterValue(string key)
        {
            List<string> parametros = this.UsuAutParametros.Split(':').ToList<string>();
            foreach (var item in parametros)
            {
                if(item.Equals(key))
                    return parametros[item.IndexOf(key)+1];
            }
            return null;
        }

        public IDictionary<string, object> getRouteParameters()
        {
            return Core.GetRouteParameters(this.UsuAutParametros);
        }     
    }    
}