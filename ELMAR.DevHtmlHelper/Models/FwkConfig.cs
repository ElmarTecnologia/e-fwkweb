using ELMAR.DevHtmlHelper.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ELMAR.DevHtmlHelper.Models
{
    [Table("fwk_config", Schema = "public")]
    public class FwkConfig
    {
        private readonly FwkContexto _contexto = new FwkContexto();

        [Key]
        [Column("cnf_codigo")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int? Codigo { get; set; }
        [Column("cnf_chave")]
        public string Chave { get; set; }
        [Column("cnf_valor")]
        public string Valor { get; set; }
        [Column("cnf_ctx")]
        public string Contexto { get; set; }
        [NotMapped]
        public string gridKey
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public static FwkConfig GetSetting(string Chave, string CTX)
        {
            using (FwkContexto _contexto = new FwkContexto())
            {
                return (from cnf in _contexto.Configs where cnf.Chave.Equals(Chave) && cnf.Contexto.Equals(CTX) select cnf).FirstOrDefault();
            }
        }

        public static string GetSettingValue(string Chave, string CTX = "0", HttpSessionStateBase Session = null, string CTXField = "CTX", bool update = false, string currentValue = "")
        {
            using (FwkContexto _contexto = new FwkContexto()) {

                //Tratamento específico para a variável pathApp
                if (Session != null && !string.IsNullOrEmpty(Chave) && Chave.Equals("pathApp"))
                {
                    string pathAppSubdir = !GetSettingValue("pathAppSubdir", CTX, Session, CTXField).Equals(string.Empty) ? "/" + GetSettingValue("pathAppSubdir", CTX, Session, CTXField) : string.Empty;
                    return HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + pathAppSubdir;
                }

                CTX = string.IsNullOrEmpty(CTX) && (Session != null && Session[CTXField] != null) ? Session[CTXField].ToString() : CTX;

                #region Trecho para inserção/atualização de parâmetros de configuração
                if (update)
                {
                    //Adiciona o valor do parâmetro às configurações
                    FwkConfig fwkCnf = GetSetting(Chave, CTX);
                    if (fwkCnf != null)
                    {
                        //Atualiza a configuração existente
                        var encontrado = _contexto.Configs.Find(fwkCnf.Codigo);
                        encontrado.Valor = currentValue;
                    }
                    else
                    {
                        //Insere a nova configuração
                        FwkConfig newFwkConfig = new FwkConfig()
                        {
                            Contexto = CTX,
                            Chave = Chave,
                            Valor = currentValue
                        };
                        _contexto.Configs.Add(newFwkConfig);
                    }
                    //DB Commit 
                    _contexto.SaveChanges();
                }
                #endregion

                if (Session != null && Session[Chave] != null)
                    return Session[Chave].ToString();

                string value = string.Empty;

                List<FwkConfig> fwkConfigs = MemoryCacheObject<List<FwkConfig>>.GetDataFromCache("_fwkConfigs");

                if (fwkConfigs == null)
                {
                    fwkConfigs = (from cnf in _contexto.Configs select cnf).ToList();
                    MemoryCacheObject<List<FwkConfig>>.StoreDataInCache("_fwkConfigs", fwkConfigs);
                }

                var config = (from cnf in fwkConfigs where cnf.Chave.Equals(Chave) && cnf.Contexto.Equals(CTX) select cnf).FirstOrDefault();
                if (config != null)
                    value = config.Valor;
                if (string.IsNullOrEmpty(value) && ConfigurationManager.AppSettings[Chave] != null)
                    value = ConfigurationManager.AppSettings[Chave].ToString();
                
                value = string.IsNullOrEmpty(value) ? currentValue : value;

                return value;
            }
        }

        public static string GetSettingValue(string Chave, HttpSessionStateBase Session, string CTXField = "CTX", bool update = false, string newValue = "")
        {
            return GetSettingValue(Chave, "", Session, CTXField, update, newValue);       
        }

        public static string GetSettingValue(string Chave, HttpContextBase Session, string CTXField = "CTX", bool update = false, string newValue = "")
        {
            return GetSettingValue(Chave, "", Session.Session, CTXField, update, newValue);
        }

        public static string GetSettingValue(string Chave, HttpContext Session, string CTXField = "CTX", bool update = false, string newValue = "")
        {
            HttpContextBase ctxBase = new HttpContextWrapper(Session);
            var CTX = Core.GetSetCTX(ctxBase);
            return GetSettingValue(Chave, CTX, ctxBase.Session, CTXField, update, newValue);
        }

    }
}