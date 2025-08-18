using System.Collections.Generic;

namespace ELMAR.DevHtmlHelper.Models
{
    public class FwkDominio
    {
        public string valor{get; set;}
        public string descricao{get; set;}
        public ICollection<string> dominio_itens{get; set;}
    }
}