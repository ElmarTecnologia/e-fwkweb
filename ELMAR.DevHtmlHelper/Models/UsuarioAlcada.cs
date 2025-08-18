using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELMAR.DevHtmlHelper.Models
{
    [Table("fwk_usr_alcada", Schema = "public")]
    public class UsuarioAlcada
    {
        private readonly FwkContexto _contexto = new FwkContexto();

        [Key]
        [Column("alc_usu_codigo", Order=0) ]
        public int CodigoUsuario { get; set; }
        [ForeignKey("CodigoUsuario")]
        public virtual Usuario Usuario {get; set;}
        [Key]
        [Column("alc_usu_contexto", Order=1)]
        public string Contexto { get; set; }
        private string _ContextoDescricao;        
        [Column("alc_usu_contexto_desc")]
        public string ContextoDescricao {
            get
            {
                return (this._ContextoDescricao + " (" + this.Contexto + ")").ToUpper();
            }
            set
            {
                this._ContextoDescricao = value;
            }
        }
        [Key]
        [Column("alc_usu_perfil", Order=2)]
        public string UsuarioPerfil { get; set; }
        [ForeignKey("UsuarioPerfil")]
        public virtual UsuarioPerfil Perfil { get; set; }
        [NotMapped]
        public string gridKey
        {
            get
            {
                return this.CodigoUsuario + "|" + this.Contexto + "|" + this.UsuarioPerfil;
            }
        }
        
        public bool Exists()
        {
            return _contexto.UsuarioAlcadas.Find(new object[] { this.CodigoUsuario, this.Contexto, this.UsuarioPerfil }) != null;
        }
    }
}