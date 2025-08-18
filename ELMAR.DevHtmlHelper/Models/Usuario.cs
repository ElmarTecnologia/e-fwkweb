using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace ELMAR.DevHtmlHelper.Models
{
    [Table("fwk_usuario", Schema = "public")]
    public class Usuario
    {
        private readonly FwkContexto _contexto = new FwkContexto();
        
        [Key]
        [Column("usu_codigo")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }
        [Column("usu_login")]
        public string Login { get; set; }
        [Column("usu_nome")]
        public string Nome { get; set; }
        private string _email;
        [Column("usu_email")]
        [Required(ErrorMessage = "O campo e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O e-mail informado é inválido")]
        public string Email
        {
            get { return this._email != null ? this._email.ToLower() : null; }
            set { this._email = value != null ? value.ToLower() : null; }
        }
        private string _senha;
        [Column("usu_senha")]
        public string Senha {
            get
            {
                return _senha;
            }
            set 
            {   //Verifica encryptação (value)
                if (value==null || Core.IsEncrypted(value))
                    _senha = value;
                else
                    _senha = Core.Crypto(value);
            }  
        }
        [Column("usu_ativo")]
        public bool Ativo { get; set; }
        [Column("usu_alterar_senha")]
        public bool DeveAlterarSenha { get; set; }
        [Column("usu_parametros")]
        public string ParametrosFixos { get; set; } //Parâmetro(s) Fixo(s)
        [Column("dt_ult_atualizacao")]
        public DateTime DtUltAtualizacao { get; set; }
        [NotMapped]
        public string gridKey
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        [XmlIgnoreAttribute]
        public virtual ICollection<UsuarioAlcada> Contextos { get; set; }

        [NotMapped]
        //[Compare("Senha", ErrorMessage = "A senha não foi confirmada corretamente")] //GLOBAL
        public virtual string ConfirmSenha { get; set; }
        [NotMapped]
        public virtual string ConfirmEmail{ get; set; }
        
        public Usuario GetUsuario(int Codigo)
        {
            return (from u in _contexto.Usuarios where u.Codigo == Codigo select u).FirstOrDefault();
        }

        public Usuario GetUsuario(string Login)
        {
            return (from u in _contexto.Usuarios where u.Login.Equals(Login) select u).FirstOrDefault();
        }

        public Usuario GetUsuariobyLoginEmail(string Login, string Email)
        {
            return (from u in _contexto.Usuarios where u.Login.Equals(Login) && u.Email.Equals(Email) select u).FirstOrDefault();
        }

        //TODO: Adicionar EF Join Queries
        public Usuario GetUsuariobyEmailPerfil(string Email, string Perfil, string Contexto)
        {
            Usuario usu = (from u in _contexto.Usuarios where u.Email.Equals(Email) select u).FirstOrDefault();
            //Verifica o perfil 
            if (usu != null && usu.Contextos.Where<UsuarioAlcada>(userAlc => userAlc.Contexto.Equals(Contexto) && userAlc.UsuarioPerfil.Equals(Perfil) && userAlc.CodigoUsuario == usu.Codigo).ToList().Count > 0)
                return usu;
            return null;
        }

        public Usuario GetUsuariobyEmailPerfil(string Email, string Perfil)
        {
            Usuario usu = (from u in _contexto.Usuarios where u.Email.Equals(Email) select u).FirstOrDefault();
            //Verifica o perfil 
            if (usu != null && usu.Contextos.Where<UsuarioAlcada>(userAlc => userAlc.UsuarioPerfil.Equals(Perfil) && userAlc.CodigoUsuario == usu.Codigo).ToList().Count > 0)
                return usu;
            return null;
        }

        public Usuario GetUsuariobyLoginPerfil(string Login, string Perfil, string Contexto)
        {
            Usuario usu = (from u in _contexto.Usuarios where u.Login.Equals(Login) select u).FirstOrDefault();
            if (usu != null && usu.Contextos.Where<UsuarioAlcada>(userAlc => userAlc.Contexto.Equals(Contexto) && userAlc.UsuarioPerfil.Equals(Perfil) && userAlc.CodigoUsuario == usu.Codigo).ToList().Count > 0)
                return usu;
            return null;
        }

        public Usuario GetUsuariobyLoginPerfil(string Login, string Perfil)
        {
            Usuario usu = (from u in _contexto.Usuarios where u.Login.Equals(Login) select u).FirstOrDefault();
            if (usu != null && usu.Contextos.Where<UsuarioAlcada>(userAlc => userAlc.UsuarioPerfil.Equals(Perfil) && userAlc.CodigoUsuario == usu.Codigo).ToList().Count > 0)
                return usu;
            return null;
        }

        public Usuario GetUsuariobyLoginCTX(string Login, string CTX)
        {
            Usuario usu = (from u in _contexto.Usuarios where u.Login.Equals(Login) select u).FirstOrDefault();
            if (usu != null && usu.Contextos.Where<UsuarioAlcada>(userAlc => userAlc.Contexto.Equals(CTX) && userAlc.CodigoUsuario == usu.Codigo).ToList().Count > 0)
                return usu;
            return null;
        }

        //Validação específica para prelogin
        public Usuario ValidaUsuario(string login, string senha, int codigo, bool canLoginWithEmail = false)
        {
            if (string.IsNullOrEmpty(senha))
                return null;
            senha = !Core.IsEncrypted(senha) ? Core.Crypto(senha) : senha;
            if (canLoginWithEmail)
                return (from u in _contexto.Usuarios where (u.Login.Equals(login) || u.Email.Equals(login)) && u.Senha.Equals(senha) && u.Codigo == codigo select u).FirstOrDefault();
            return (from u in _contexto.Usuarios where u.Login.Equals(login) && u.Senha.Equals(senha) && u.Codigo == codigo select u).FirstOrDefault();
        }

        //Sobrecarga de métodos para validação de usuários
        public Usuario ValidaUsuario(string login, string senha, bool canLoginWithEmail = false)
        {
            var usuarios = _contexto.Usuarios.Where(u => u.Login == "elmar").ToList();

            if (string.IsNullOrEmpty(senha))
            {
                return null;
            }
            senha = !Core.IsEncrypted(senha) ? Core.Crypto(senha) : senha;
            if(canLoginWithEmail)
            {
                return (from u in _contexto.Usuarios where (u.Login.Equals(login) || u.Email.Equals(login)) && u.Senha.Equals(senha) select u).FirstOrDefault();
            }
            return (from u in _contexto.Usuarios where u.Login.Equals(login) && u.Senha.Equals(senha) select u).FirstOrDefault();
        }
        
        public Usuario ValidaUsuario(string login, string senha, string Perfil, bool canLoginWithEmail = false)
        {
            if (string.IsNullOrEmpty(senha))
                return null;
            Usuario usu = this.ValidaUsuario(login, senha, canLoginWithEmail);
            if (usu != null && usu.Contextos.Where<UsuarioAlcada>(userAlc => userAlc.UsuarioPerfil.Equals(Perfil) && userAlc.CodigoUsuario == usu.Codigo).ToList().Count > 0)
                return usu;
            return null;
        }

        public Usuario ValidaUsuario(string login, string senha, string Perfil, string Contexto, bool canLoginWithEmail = true)
        {
            if (string.IsNullOrEmpty(senha))
                return null;
            Usuario usu = this.ValidaUsuario(login, senha, Perfil, canLoginWithEmail);
            if (usu != null && usu.Contextos.Where<UsuarioAlcada>(userAlc => userAlc.Contexto.Equals(Contexto)).ToList().Count > 0)
                return usu;
            return null;
        }

        //TODO: Implementar login by token
        public Usuario ValidaUsuario(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;
            //return (from u in _contexto.Usuarios where u.Senha.Equals(token) select u).FirstOrDefault();
            return null;
        }

        public virtual List<UsuarioAlcada> getContextos()
        {
            Dictionary<string, UsuarioAlcada> distinctContextos = new Dictionary<string, UsuarioAlcada>();  
            List<UsuarioAlcada> contextos = this.Contextos.OrderBy(x => x.ContextoDescricao).Distinct().ToList<UsuarioAlcada>();
            foreach (var item in contextos)
            {
                if (!distinctContextos.ContainsKey(item.Contexto))
                {
                    distinctContextos.Add(item.Contexto, item);     
                }
            }
            return distinctContextos.Values.ToList<UsuarioAlcada>();
        }

        public virtual List<UsuarioAlcada> getAlcadas()
        {
            return this.Contextos.OrderBy(x => x.ContextoDescricao).Distinct().ToList<UsuarioAlcada>();            
        }

        public List<string> GetContextosCodigos()
        {
            List<string> ContextoCodigos = new List<string>();
            foreach (var item in this.Contextos)
            {
                if(!ContextoCodigos.Contains(item.Contexto))
                    ContextoCodigos.Add(item.Contexto);
            }
            return ContextoCodigos;
        }

        public virtual bool possuiContexto(string contexto)
        {
            List<UsuarioAlcada> contextos = this.getContextos();
            foreach (var item in contextos)
            {
                if (item.Contexto.Equals(contexto))
                    return true;
            }
            return false;
        }

        public virtual bool possuiPerfil(string perfil)
        {
            List<UsuarioAlcada> contextos = this.getAlcadas();
            foreach (var item in contextos)
            {
                if (item.Perfil.Titulo.Equals(perfil))
                    return true;
            }
            return false;
        }

        public virtual UsuarioPerfil getCtxUsuPerfil(string ctx)
        {
            List<UsuarioAlcada> contextos = this.getAlcadas();
            foreach (var item in contextos)
            {
                if (item.Contexto.Equals(ctx) && item.Perfil != null)
                {
                    //Define o contexto atual no objeto UsuarioPerfil
                    item.Perfil.currentContext = ctx;
                    return item.Perfil;
                }               
            }
            return null;
        }

        public virtual UsuarioPerfil getCtxUsuPerfil(string ctx, string Perfil)
        {
            List<UsuarioPerfil> perfis = this.getCtxUsuPerfis(ctx);
            foreach (var item in perfis)
            {
                if (item.Titulo.Equals(Perfil))
                {
                    //Define o contexto atual no objeto UsuarioPerfil
                    item.currentContext = ctx;
                    return item;
                }
            }
            return null;
        }

        public virtual List<UsuarioPerfil> getCtxUsuPerfis(string ctx)
        {
            List<UsuarioPerfil> ctxPerfis = new List<UsuarioPerfil>();
            List<UsuarioAlcada> contextos = this.getAlcadas();
            foreach (var item in contextos)
            {
                if (item.Contexto.Equals(ctx) && item.Perfil != null && item.Perfil.Ativo)
                {
                    //Define o contexto atual no objeto UsuarioPerfil
                    item.Perfil.currentContext = ctx;
                    ctxPerfis.Add(item.Perfil);
                }
            }
            return ctxPerfis;
        }

        public static bool VerificaAutorizacao(out UsuarioAutorizacao userAutorizacao, string Chamada, HttpContextBase HttpContext, string absoluteUrl, string CustomLogin = "/Usuario/Logout/", string Contexto = "")
        {
            FwkContexto _contexto = new FwkContexto();
            bool permitido = true;
            HttpSessionStateBase Session = HttpContext.Session;

            //Define o contexto na sessão
            string CTX = Core.GetSetCTX(HttpContext, Contexto);

            userAutorizacao = new UsuarioAutorizacao();

            //Completa o endereçamento completo
            CustomLogin = !CustomLogin.Contains("http") ? FwkConfig.GetSettingValue("pathApp").ToString() + CustomLogin : CustomLogin; 

            #region Tabela de Autorizações Temporária - Session 
            //(Esvaziada ao perder sessão ou efetuar logoff)
            /*if (Session["AUTHORIZETABLE"] == null)
                Session["AUTHORIZETABLE"] = new List<string>();
            if (((List<string>)Session["AUTHORIZETABLE"]).Contains(Chamada))
                return true;*/
            #endregion

            Usuario sessionUser = (ELMAR.DevHtmlHelper.Models.Usuario)Session["USUARIO"];

            //Verifica a existência da Chamada
            //Chamada: "/Controller/Action"
            string Action = Chamada.Split('/')[2];
            string Controller = Chamada.Split('/')[1];
            Chamada call = _contexto.Chamadas.Where(x => x.Action == Action && x.Controller == Controller).FirstOrDefault();

            if (!string.IsNullOrEmpty(CTX))
            {
                License lic = License.getLicense;
                if (!lic.Licensed && !Action.Equals("Licensing"))
                {
                    userAutorizacao.Info = lic.Info;
                    userAutorizacao.Redirect = FwkConfig.GetSettingValue("pathApp")+"/Usuario/Login/" + "?ctx="+CTX+"&Alert=" + userAutorizacao.Info + "&RedirectTo=" + absoluteUrl + "&ViewFile=Licensing";
                    return false;
                }
            }

            if (call == null)
            {
                userAutorizacao.Info = "Chamada não cadastrada ou inválida...";
                userAutorizacao.Redirect = CustomLogin + "?ctx=" + CTX + "&Alert=" + userAutorizacao.Info + "&RedirectTo=" + absoluteUrl;
                return false;
            }

            if (!call.RequerAutorizacao)
            {
                //((List<string>)Session["AUTHORIZETABLE"]).Add(Chamada);
                return true;
            }

            if (CTX.Equals(string.Empty) || Session["PERFIL"] == null)
            {                
                userAutorizacao.Info = CTX.Equals(string.Empty) ? "Sessão perdida, por favor, efetue o login..." : string.Empty;
                userAutorizacao.Redirect = CustomLogin + "?ctx=" + CTX + "&Alert=" + userAutorizacao.Info + "&RedirectTo=" + absoluteUrl;
                return false;
            }

            if (sessionUser == null)
            {
                userAutorizacao.Info = "Usuário não definido";
                userAutorizacao.Redirect = CustomLogin + "?ctx=" + CTX + "&Alert=" + userAutorizacao.Info + "&RedirectTo=" + absoluteUrl;
                return false;
            }

            //Adiciona o Código do Usuário ao objeto UsuarioAutorizacao
            userAutorizacao.UsuAutUsuCodigo = sessionUser.Codigo;

            //Valida o contexto da aplicação
            if (!string.IsNullOrEmpty(Contexto) && !sessionUser.possuiContexto(Contexto))
            {
                userAutorizacao.Info = "O usuário não possui contexto autorizado para essa aplicação";
                userAutorizacao.Redirect = CustomLogin + "?ctx=" + CTX + "&Alert=" + userAutorizacao.Info + "&RedirectTo=" + absoluteUrl;
                return false;
            }

            //Perfis com acessos múltiplos (INFO: ADMIN REMOVIDO)
            if (Session["PERFIL"].ToString().Equals("SUPER") || Session["PERFIL"].ToString().Equals("MASTER"))
            {
                //((List<string>)Session["AUTHORIZETABLE"]).Add(Chamada);
                return true;
            }

            PerfilAutorizacao PerfilAut = null;
            if(sessionUser.getCtxUsuPerfil(CTX) != null)
                PerfilAut = new PerfilAutorizacao().getChamadaAutorizacao(Session["PERFIL"].ToString(), Chamada);
            if (PerfilAut == null || !PerfilAut.PerAutAtivo)
            {
                userAutorizacao.Info = "Perfil não autorizado";
                permitido = false;
            }

            //Possui prioridade à validação do perfil
            UsuarioAutorizacao baseUserAutorizacao = new UsuarioAutorizacao().getUsuAutorizacao(sessionUser.Codigo, Chamada);
            if (baseUserAutorizacao != null)
            {
                //Atualiza a instância do objeto UsuarioAutorizacao
                userAutorizacao = baseUserAutorizacao;
                if (!userAutorizacao.UsuAutAtivo)
                {
                    userAutorizacao.Info = "Usuário não autorizado";
                    permitido = false;
                }
                permitido = userAutorizacao.UsuAutAtivo;
            }

            //Definindo o redirect com base no Perfil
            if (!permitido)
            {
                var Perfil = _contexto.UsuarioPerfis.Find(Session["PERFIL"].ToString());
                userAutorizacao.Redirect = Perfil.RedirectTo + "?Alert=" + userAutorizacao.Info;
                if (Perfil.RedirectTo.Equals("/" + Controller + "/" + Action))
                    userAutorizacao.Redirect = CustomLogin + "?ctx=" + CTX + "&Alert=" + userAutorizacao.Info + "&RedirectTo=" + absoluteUrl;
            }
            //if(permitido)
            //    ((List<string>)Session["AUTHORIZETABLE"]).Add(Chamada);
            return permitido;
        }

        public static bool VerificaAutorizacao(out UsuarioAutorizacao userAutorizacao, string Chamada, HttpContext httpContext, string absoluteUrl = "", string CustomLogin = "/Usuario/Logout/", string Contexto = "")
        {
            return VerificaAutorizacao(out userAutorizacao, Chamada, new HttpContextWrapper(httpContext), absoluteUrl, CustomLogin, Contexto);
        }

        public static bool VerificaAutorizacao(out UsuarioAutorizacao userAutorizacao, HttpContextBase HttpContext, string CustomLogin = "/Usuario/Logout/", string Contexto = "", string CustomCall = "")
        {
            RouteValueDictionary values = HttpContext.Request.RequestContext.RouteData.Values;
            HttpSessionStateBase session = HttpContext.Session;
            string CTX = Core.GetSetCTX(HttpContext, Contexto);

            string absoluteUri = HttpContext.Request.Url.AbsoluteUri;
            string chamada = "/" + (string)values["controller"] + "/" + (string)values["action"];
            if (!string.IsNullOrEmpty(CustomCall))
            {
                chamada = CustomCall;
            }
            Contexto = ((!string.IsNullOrEmpty(HttpContext.Request.QueryString["CTX"])) ? HttpContext.Request.QueryString["CTX"] : string.Empty);
            /*if (string.IsNullOrEmpty(Contexto))
            {
                Contexto = CTX;
            }*/
            if (string.IsNullOrEmpty(Contexto))
            {
                Contexto = ((!string.IsNullOrEmpty(HttpContext.Request.QueryString["ecode"])) ? HttpContext.Request.QueryString["ecode"] : HttpContext.Request.QueryString["e"]);
            }
            if (string.IsNullOrEmpty(Contexto))
            {
                Contexto = ConfigurationManager.AppSettings["defaultCTX"];
            }
            return VerificaAutorizacao(out userAutorizacao, chamada, HttpContext, absoluteUri, CustomLogin, Contexto);
        }

        public static bool VerificaAutorizacao(out UsuarioAutorizacao userAutorizacao, HttpContext httpContext, string CustomLogin = "/Usuario/Logout/", string Contexto = "", string CustomCall = "")
        {
            return VerificaAutorizacao(out userAutorizacao, new HttpContextWrapper(httpContext), CustomLogin, Contexto, CustomCall);
        }

        public IDictionary<string, object> getRouteParameters()
        {
            return new UsuarioAutorizacao(){ UsuAutParametros = this.ParametrosFixos }.getRouteParameters();
        }
    }
}