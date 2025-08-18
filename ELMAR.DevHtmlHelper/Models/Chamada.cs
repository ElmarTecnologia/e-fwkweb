using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELMAR.DevHtmlHelper.Models
{
    [Table("fwk_chamada", Schema = "public")]
    public class Chamada
    {
        private readonly FwkContexto _contexto = new FwkContexto();

        [Key]
        [Column("action", Order = 0)]
        public string Action { get; set; }
        [Key]
        [Column("controller", Order = 1)]
        public string Controller { get; set; }
        [Column("autorizar")]
        public bool RequerAutorizacao { get; set; }
        [Column("titulo")]
        public string Titulo { get; set; }
        [Column("descricao")]
        public string Descricao { get; set; }
        [NotMapped]
        public string gridKey { 
            get{
                return this.Action+"|"+this.Controller;
            }
        }
    }
}