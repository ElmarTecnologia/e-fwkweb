using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELMAR.DevHtmlHelper.Models
{
    [Table("fwk_usr_perfil", Schema = "public")]
    public class UsuarioPerfil
    {
        private readonly FwkContexto _contexto = new FwkContexto();

        [Key]
        [Column("per_titulo")]        
        public string Titulo { get; set; }
        private string _RedirectTo;        
        [Column("per_redirect_to")]        
        [Required(ErrorMessage = "Campo obrigatório")]
        public string RedirectTo {
            get
            {
                //Verfica se existe o parâmetro global para redirect padrão do perfil
                string redirectTo = ELMAR.DevHtmlHelper.Models.FwkConfig.GetSettingValue(this.Titulo, this.currentContext);
                return !string.IsNullOrEmpty(redirectTo) ? redirectTo : _RedirectTo; 
            }
            set
            {
                _RedirectTo = value;
            }
        }
        [Column("per_descricao")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Descricao { get; set; }
        [Column("per_ativo")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public bool Ativo { get; set; }
        //public virtual ICollection<Usuario> Usuarios { get; set; }
        [NotMapped]
        public string currentContext { get; set; }
        [NotMapped]
        public string gridKey
        {
            get
            {
                return this.Titulo;
            }
        }

        public virtual List<Usuario> getUsuariosList()
        {
            List<Usuario> usuariosList = new List<Usuario>();
            var alcList = _contexto.UsuarioAlcadas.Where(usuAlc => usuAlc.UsuarioPerfil.Equals(this.Titulo));
            foreach (var item in alcList)
            {
                usuariosList.Add(item.Usuario);
            }
            return usuariosList;
        }

        public virtual List<Usuario> getUsuariosList(string Contexto)
        {
            List<Usuario> usuariosList = new List<Usuario>();
            var alcList = _contexto.UsuarioAlcadas.Where(usuAlc => usuAlc.UsuarioPerfil.Equals(this.Titulo) && usuAlc.Contexto.Equals(Contexto));
            foreach (var item in alcList)
            {
                Usuario usu = _contexto.Usuarios.Find(item.CodigoUsuario);
                usuariosList.Add(usu);                
            }
            return usuariosList;
        }

        public IList GetPerfis()
        {
            return (from p in _contexto.UsuarioPerfis select p).ToList<UsuarioPerfil>();
        }
    }   
}