using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;


namespace ELMAR.DevHtmlHelper.Models
{
    public class ExportModel
    {
        public static IEnumerable getData(out string sqlCode, string gridKey, string ctxField, string ecode, List<string> selectFields, string tabela, List<string> filterFields, HttpRequestBase filtros, Dictionary<string, string> Colunas_Filtros, int pageSize = 30, int page = 0, IEnumerable dtSource = null, bool execQuery = true)
        {
            using (var con = new FwkContexto())
            {
                gridKey = string.Empty;
                /*Dictionary<string, List<string>> moduleFilters = new Dictionary<string, List<string>>(){
                    { tabela, filterFields}
                };*/

                var query = new StringBuilder();

                //Adiciona as colunas à query
                Core.AddQuerySelect(query, selectFields, Colunas_Filtros);

                query.Append(" from " + tabela);

                query.Append(String.Format(" where \"" + ctxField + "\" like '{0}' ", ecode));

                //Adiciona os filtros dinâmicos à query
                Core.AddQueryFilters(query, filterFields, filtros, Colunas_Filtros, tabela);

                if (page > 0)
                {
                    var offset = pageSize * (page - 1);
                    query.Append(String.Format(" limit {0} offset {1}", pageSize, offset));
                }

                sqlCode = query.ToString();

                return (dtSource != null && ((DataTable)dtSource).Rows.Count > 0 || !execQuery) ? dtSource : con.SelectionQuery(sqlCode);
            }
        }
    }
}