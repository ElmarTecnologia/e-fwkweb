using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Elmar.WebServiceRest
{
    [Table("sms", Schema = "public")]
    public class SmsMobile
    {            
        [Key]
        [Column("sms_codigo")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }
        [Column("sms_numero")]
        public string Numero { get; set; }
        [Column("sms_titulo")]
        public string Titulo { get; set; }
        [Column("sms_conteudo")]
        public string Conteudo { get; set; }
        [Column("sms_status")]
        public int Status { get; set; }                
    }

}