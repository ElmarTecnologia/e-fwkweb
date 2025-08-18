using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ELMAR.DevHtmlHelper.Models
{
    public abstract class FwkModule : Core
    {
        private string _Ecode;

        public virtual string Ecode
        {
            get { return _Ecode; }
            set { _Ecode = value; }
        }

        private string _tabela;

        public virtual string Tabela
        {
            get { return _tabela; }
            set { _tabela = value; }
        }
        private string _gridKey;

        public virtual string GridKey
        {
            get
            {
                return _gridKey;
            }
            set
            {
                _gridKey = value;
            }
        }

        private string _ctxField;

        public virtual string CtxField
        {
            get { return _ctxField; }
            set { _ctxField = value; }
        }

        private Dictionary<string, string> _tabela_Descritor;

        public virtual Dictionary<string, string> Tabela_Descritor
        {
            get { return _tabela_Descritor; }
            set { _tabela_Descritor = value; }
        }

        private string _colunas_Ocultas;

        public virtual string Colunas_Ocultas
        {
            get { return _colunas_Ocultas.Replace("\"", ""); }
            set { _colunas_Ocultas = value; }
        }

        public virtual List<string> ColunasOcultasList
        {
            get { return this.Colunas_Ocultas.Split(';').ToList<string>(); }
        }

        private string _colunas_currency;

        public virtual string ColunasCurrency
        {
            get { return _colunas_currency; }
            set { _colunas_currency = value; }
        }

        private string _totalizadores;

        public virtual string Totalizadores
        {
            get { return _totalizadores; }
            set { _totalizadores = value; }
        }

        private string _agrupadores;

        public virtual string Agrupadores
        {
            get { return _agrupadores; }
            set { _agrupadores = value; }
        }

        private string _detailAction;

        public virtual string DetailAction
        {
            get { return _detailAction; }
            set { _detailAction = value; }
        }

        private string _colunas_Actions;

        public virtual string Colunas_Actions
        {
            get { return _colunas_Actions; }
            set { _colunas_Actions = value; }
        }

        private string _colunas_Links;

        public virtual string Colunas_Links
        {
            get { return _colunas_Links; }
            set { _colunas_Links = value; }
        }

        private string _info_Colunas;

        public virtual string Info_Colunas
        {
            get { return _info_Colunas; }
            set { _info_Colunas = value; }
        }

        private string _parametros;

        public virtual string Parametros
        {
            get { return _parametros; }
            set { _parametros = value; }
        }

        private string _info;

        public virtual string Info
        {
            get { return _info; }
            set { _info = value; }
        }

        private bool _expanded;

        public virtual bool Expanded
        {
            get { return _expanded; }
            set { _expanded = value; }
        }

        private string _filterMeta;

        public virtual string FilterMeta
        {
            get { return _filterMeta; }
            set { _filterMeta = value; }
        }

        public virtual string Tabela_Fields
        {
            get
            {
                string tabelaFields = string.Empty;
                int index = 1;

                foreach (var item in this.Tabela_Descritor)
                {
                    tabelaFields += item.Key;
                    if (index < this.Tabela_Descritor.Count)
                        tabelaFields += "; ";
                    index++;
                }
                return tabelaFields;
            }
        }

        public virtual string[] Tabela_Fields_Array
        {
            get
            {
                string tabelaFields = string.Empty;
                int index = 1;

                foreach (var item in this.Tabela_Descritor)
                {
                    //Não adiciona as colunas ocultas aos campos
                    //if (this.ColunasOcultasList.Contains(item.Key))
                    //    continue;
                    tabelaFields += item.Key;
                    if (index < this.Tabela_Descritor.Count)
                        tabelaFields += "; ";
                    index++;
                }
                return tabelaFields.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public virtual List<string> Tabela_Fields_List
        {
            get
            {
                return Tabela_Fields_Array.Select(s => s.Trim()).ToList<string>();
            }
        }

        //Métodos Construtores
        public FwkModule() { }

        public FwkModule(string Ecode)
        {
            this.Ecode = Ecode;
        }

        public virtual List<SelectListItem> Tabela_Fields_SelectList
        {
            get
            {
                List<SelectListItem> tabelaFieldsList = new List<SelectListItem>();
                foreach (string item in this.Tabela_Fields.Split(';'))
                {
                    //Não adiciona as colunas vazias/ocultas aos campos de seleção
                    if (string.IsNullOrEmpty(item.Trim()) || this.ColunasOcultasList.Contains(item.Trim()))
                        continue;
                    tabelaFieldsList.Add(new SelectListItem { Text = item, Value = item.Trim() });
                }
                return tabelaFieldsList;
            }
        }

        //Filtros customizados fixos
        public Dictionary<string, string> customFilters;

        public virtual List<SelectListItem> Tabela_Filters_SelectList
        {
            get
            {
                List<SelectListItem> tabelaFieldsList = new List<SelectListItem>();
                foreach (string item in this.Tabela_Fields.Split(';'))
                {
                    if (string.IsNullOrEmpty(item.Trim()) || this.ColunasOcultasList.Contains(item.Trim()))
                        continue;
                    tabelaFieldsList.Add(new SelectListItem { Text = item, Value = item.Trim() });
                }
                return tabelaFieldsList;
            }
        }

        public virtual IEnumerable getData(out string sqlCode, List<string> selectFields = null, List<string> filterFields = null, HttpRequestBase filtros = null, Dictionary<string, string> Tabela_Descritor = null, int pageSize = 30, int page = 0, IEnumerable dtSource = null, bool execQuery = true, bool distinctValues = false)
        {
            sqlCode = string.Empty;
            return null;
        }        

        public virtual async Task Configure() { }
    }
}