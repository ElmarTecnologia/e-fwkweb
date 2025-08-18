using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELMAR.DevHtmlHelper.Models
{
    [Table("fwk_usr_perfil_autoriza", Schema = "public")]
    public class PerfilAutorizacao
    {
        private readonly FwkContexto _contexto = new FwkContexto();

        [Key]
        [Column("per_aut_perfil", Order = 0)]
        public string AutPerfil { get; set; }
        [ForeignKey("AutPerfil")]
        public virtual UsuarioPerfil Perfil { get; set; }
        [Key]
        [Column("per_aut_chamada", Order = 1)]
        public string AutChamada { get; set; }
        [Column("per_aut_ativo")]
        public bool PerAutAtivo { get; set; }
        [NotMapped]
        public string gridKey
        {
            get
            {
                return this.AutPerfil + "|" + this.AutChamada;
            }
        }

        public virtual List<PerfilAutorizacao> getPerAutorizacoes()
        {
            return (from p_alt in _contexto.PerfilAutorizacoes where p_alt.AutPerfil.Equals(this.AutPerfil) select p_alt).ToList<PerfilAutorizacao>();
        }

        public virtual PerfilAutorizacao getChamadaAutorizacao(string Perfil, string Chamada)
        {
            if (Chamada.EndsWith("/"))
                Chamada = Chamada.Substring(0, Chamada.Length-1);
            return (from p_alt in _contexto.PerfilAutorizacoes where p_alt.AutPerfil.Equals(Perfil) && p_alt.AutChamada.Equals(Chamada) select p_alt).FirstOrDefault();
        }
    }    
}