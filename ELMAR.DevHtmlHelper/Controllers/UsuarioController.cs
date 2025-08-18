using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using ELMAR.DevHtmlHelper.Models;
using System.Collections.Specialized;
using ELMAR.DevHtmlHelper.Models.CustomValidation;
using System.Threading.Tasks;

namespace ELMAR.DevHtmlHelper.Controllers
{
    public class UsuarioController : AppController
    {
        private readonly FwkContexto _contexto = new FwkContexto("201082");
        public string loginUrl = FwkConfig.GetSettingValue("pathApp") + ConfigurationManager.AppSettings["loginUrl"];

        /// <summary>
        /// Método genérico para encapsular funções do E-Fwk à páginas da aplicação herdando o layout desta.
        /// </summary>
        /// <param name="Login">Login</param>
        /// <param name="Email">Email</param>
        /// <param name="Perfil">Perfil</param>
        /// <param name="Result">Variável para armazenar o resultado</param>
        /// <param name="RedirectTo">Página de redirecionamento</param>
        /// <param name="ReenviaSenha">Flag para exibir a funcionalidade de reenvio de senhas</param>
        /// <param name="ViewFile">Nome do arquivo de template</param>
        /// <param name="ClientID">Utilizado pelo formulário de licenciamento da aplicação 'LicensingForm'</param>
        /// <param name="PreLogin">Flag para ativar o recurso de pré-login - Define variáveis de sessão antes de efetuar o login final)</param>
        /// <param name="Tema">Tema devexpress</param>
        /// <param name="isPartial">Executa de maneira parcial ou utilizando o layout base da aplicação</param>
        /// <returns></returns>
        public override ActionResult Login(string Login = "", string Email = "", string Perfil = "", string Result = "", string RedirectTo = "", bool ReenviaSenha = true, string ViewFile = "Login", string ClientID="", string Codigo = "", string UITarget = "", bool PreLogin = false, string Tema = "", bool isPartial = false)
        {
            string viewName = "DevHelper/Usuario/"+ViewFile+"";

            loginUrl = FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ConfigurationManager.AppSettings["loginUrl"];

            ViewData["Titulo"] = ConfigurationManager.AppSettings["appName"];
            ViewData["Acao"] = "SALVAR";
            ViewBag.Perfil = Perfil;
            ViewBag.Login = Login;
            ViewBag.Email = Email;
            ViewBag.Msg = Result;
            ViewBag.RedirectTo = RedirectTo;
            ViewBag.ReenviaSenha = ReenviaSenha;
            ViewBag.Tema = Tema;
            ViewBag.isPartial = isPartial;
            ViewBag.Codigo = Codigo;
            ViewBag.UITarget = UITarget;

            if (string.IsNullOrEmpty(Login)) 
                ViewBag.ReenviaSenha = false;
            ViewBag.PreLogin = PreLogin;

            #region Valida a autorização/parametrização a partir do FWK
            UsuarioAutorizacao userAut = null;
            if (!Usuario.VerificaAutorizacao(out userAut, "/Usuario/"+ViewFile, HttpContext, HttpContext.Request.Url.AbsoluteUri))
            {
                ViewBag.Msg = userAut.Info;
                ViewBag.RedirectTo = userAut.Redirect;
                return PartialView("DevHelper/Usuario/Login");
            }
            #endregion

            switch (ViewFile)
            {
                case "Login":
                    //Reinicia as variáveis de sessão
                    //Não reseta o CONTEXTO para permitir logar com outras credenciais posteriormente
                    Session["USER"] = Session["USER_ID"] = Session["PERFIL"] = null;
                    break;
                case "SelecionarContexto":
                    Usuario usuario = new Usuario().GetUsuario(base.Session["USER"].ToString());
					int num = new Random().Next(10000, 99999);
					Session["TOKEN"] = num.ToString();
					ViewBag.Contextos = usuario.getContextos();
					break;
                case "AlterarSenha":
                    ViewBag.Titulo = "Alterar Senha";
					ViewBag.Msg = Result;
					ViewBag.RedirectTo = RedirectTo;
                    ViewBag.DicaSenha = new ChecaForcaSenha().GetDicaSenha(HttpContext);
                    Usuario usuariobyLoginPerfil;
					if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Perfil) && (base.Session["PERFIL"].ToString().Equals("MASTER") || base.Session["PERFIL"].ToString().Equals("SUPER")))
					{
						usuariobyLoginPerfil = new Usuario().GetUsuariobyLoginPerfil(Login, Perfil, Core.GetSetCTX(HttpContext));
					}
					else
					{
						usuariobyLoginPerfil = new Usuario().GetUsuariobyLoginPerfil(base.Session["USER"].ToString(), base.Session["PERFIL"].ToString(), Core.GetSetCTX(HttpContext));
					}
					if (usuariobyLoginPerfil == null)
					{
						ViewBag.Msg = "Solicitação não permitida, usuário não encontrado";
						return PartialView("DevHelper/Usuario/Login");						
					}
                    ViewBag.Login = usuariobyLoginPerfil.Login;                    
					return PartialView(viewName, usuariobyLoginPerfil);					
                case "AdmAlterarSenha":
                    viewName = viewName.Replace("AdmAlterarSenha", "AlterarSenha");
					ViewBag.Titulo = "Alterar Senha";
                    ViewBag.DicaSenha = new ChecaForcaSenha().GetDicaSenha(HttpContext);
					base.ViewData["Msg"] = Result;
					ViewBag.RedirectTo = RedirectTo;
					Usuario usuariobyLoginPerfil2;
					if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Perfil))
					{
						usuariobyLoginPerfil2 = new Usuario().GetUsuariobyLoginPerfil(Login, Perfil);
					}
					else
					{
						usuariobyLoginPerfil2 = new Usuario().GetUsuariobyLoginPerfil(base.Session["USER"].ToString(), base.Session["PERFIL"].ToString(), Core.GetSetCTX(HttpContext));
					}
					if (usuariobyLoginPerfil2 == null)
					{
						ViewBag.Msg = "Solicitação não permitida, usuário não encontrado";
						return PartialView("DevHelper/Usuario/Login");
					}
                    return PartialView(viewName, usuariobyLoginPerfil2);				
                case "Perfis":
                    ViewBag.dtSource = (from uPerfils in _contexto.UsuarioPerfis select uPerfils).ToList<UsuarioPerfil>();
                    return PartialView(viewName);
                case "Contextos":
                    ViewBag.dtSource = (from uContexts in _contexto.UsuarioAlcadas select uContexts).ToList<UsuarioAlcada>();
                    return PartialView(viewName);
                case "Usuarios":
                    ViewBag.dtSource = (from users in _contexto.Usuarios select users).ToList<Usuario>();
                    return PartialView(viewName);
                case "Chamadas":
					ViewBag.dtSource = (from calls in this._contexto.Chamadas
					select calls).ToList<Chamada>();
					return PartialView(viewName);
				case "Parametros":
					ViewBag.dtSource = (from cnfs in this._contexto.Configs
					select cnfs).ToList<FwkConfig>();
					return PartialView(viewName);
				case "PerfilAutoriza":
					ViewBag.dtSource = (from p_aut in this._contexto.PerfilAutorizacoes
					select p_aut).ToList<PerfilAutorizacao>();
					return PartialView(viewName);
				case "UsuarioAutoriza":
					ViewBag.dtSource = (from u_aut in this._contexto.UsuarioAutorizacoes
					select u_aut).ToList<UsuarioAutorizacao>();
					return PartialView(viewName);
                case "Licensing":
                    ViewBag.ID = ConfigurationManager.AppSettings["appName"] + " - ID: " + Util.GetMACAddress();
                    return PartialView(viewName);
                case "LicensingForm":
                    if (!string.IsNullOrEmpty(ClientID))
                    {
                        string[] valores = Core.Decrypt(ClientID).Split('|');
                        if (valores.Length >= 2)
                        {
                            ViewBag.Mac = valores[0];
                            ViewBag.App = valores[1];
                        }
                    }
                    return PartialView(viewName);
            }

            return PartialView(viewName);
        }

        public ActionResult DoLogin(String Login, String inputSenha, string captcha = "", String Perfil="", String Email="", String RedirectTo = "", String Codigo = "", bool PreLogin=false, string ctx = "")
        {
            ViewData["Titulo"] = ConfigurationManager.AppSettings["appName"];

            loginUrl = FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ConfigurationManager.AppSettings["loginUrl"];

            //Não permite o login com senha em branco
            if (String.IsNullOrEmpty(inputSenha) || String.IsNullOrEmpty(Login))
                return Redirect(string.Format("{0}?Result=Os campos 'Usuário' e 'Senha' são obrigatórios&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));                            

            if (!string.IsNullOrEmpty(Perfil))
            {
                if (_contexto.UsuarioPerfis.Find(Perfil) == null)
                {
                    Perfil = "ADMIN"; //Define o Perfil padrão 
                    return Redirect(String.Format("{0}?Result=O Perfil informado é inválido!&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
                }
            }

            bool isDirectLogin = Core.IsEncrypted(inputSenha);
            //Login Direto sem ser a partir do formulário padrão será aceito apenas após a validação da senha encriptada            
            if (!Request.HttpMethod.Equals("POST") && !isDirectLogin)
            {
                return Redirect(string.Format("{0}?Result=Tentativa de login não validada&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
            }

            //Validando o captcha, caso ativo
            if (!isDirectLogin && (Session["LoginAttemps"] != null && (int)Session["LoginAttemps"] > 1) && (String.IsNullOrEmpty(captcha) || Session["Captcha"] == null || !captcha.Equals(Session["Captcha"].ToString())))
                return Redirect(string.Format("{0}?Result=O processo de validação (captcha) não foi verificado corretamente&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
                 
            //Valida o usuário
            inputSenha = isDirectLogin ? Core.Decrypt(inputSenha) : inputSenha;

            Usuario usuario = new Usuario().ValidaUsuario(Login, inputSenha);
            //Tentativa de login utilizando o email
            usuario = usuario ?? new Usuario().ValidaUsuario(Email, inputSenha);
            if (!string.IsNullOrEmpty(Codigo) && Core.IsEncrypted(Codigo))
            {
                int CodigoParse = int.Parse(Core.Decrypt(Codigo));
                usuario = new Usuario().ValidaUsuario(Login, inputSenha, CodigoParse, true);
                //Tentativa de login utilizando o email
                usuario = usuario ?? new Usuario().ValidaUsuario(Email, inputSenha, CodigoParse, true);
            }            

            //Definindo o redirecionamento
            if(Core.GetSetCTX(HttpContext) != null && !string.IsNullOrEmpty(Core.GetSetCTX(HttpContext)))
                RedirectTo = (string.IsNullOrEmpty(RedirectTo) && usuario != null && usuario.getCtxUsuPerfil(Core.GetSetCTX(HttpContext)) != null) ? usuario.getCtxUsuPerfil(Core.GetSetCTX(HttpContext)).RedirectTo : RedirectTo;                
            
            //Nâo permite que o RedirectTo permaneça vazio
            RedirectTo = string.IsNullOrEmpty(RedirectTo) ? loginUrl : RedirectTo;
            
            switch (Perfil)
            {
                case "USER":
                    //...
                default:
                    if (usuario != null)
                    {
                        //Inicializando variáveis de sessão auxiliares
                        Session["SELETOR_PERFIL"] = null;

                        // 1º Tenta definir o contexto informado diretamente 
                        if (isDirectLogin && !string.IsNullOrEmpty(ctx))
                        {
                            if (usuario.possuiContexto(ctx))
                            {
                                SetContexto(ctx);
                            }
                            else
                            {
                                return Redirect(string.Format("{0}?Result=USUÁRIO NÃO POSSUI CONTEXTO DEFINIDO PARA ESTA APLICAÇÃO&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
                            }
                        } 
                        // 2º Tenta definir o contexto Default 
                        else if ((Core.GetSetCTX(HttpContext).Equals(string.Empty) || Core.GetSetCTX(HttpContext).Equals(string.Empty))
                            && !ConfigurationManager.AppSettings["defaultCTX"].ToString().Equals(String.Empty))
                        {
                            if (usuario.possuiContexto(ConfigurationManager.AppSettings["defaultCTX"].ToString()))
                            {
                                 SetContexto(ConfigurationManager.AppSettings["defaultCTX"].ToString());
                            }
                            else
                            {
                                return Redirect(string.Format("{0}?Result=USUÁRIO NÃO POSSUI CONTEXTO DEFINIDO PARA ESTA APLICAÇÃO&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
                            }
                        }
                        // 3º Selecionar a partir do(s) contexto(s) existente(s) no usuário
                        else
                        { 
                            //Pega os contexto(s) do usuário (alcada), caso o contexto não tenha sido fornecido por FORÇA
                            if (Core.GetSetCTX(HttpContext).Equals(string.Empty) || Core.GetSetCTX(HttpContext).Equals(string.Empty))
                            {
                                if (usuario.getContextos().Count > 1)
                                {
                                    //TODO: Remover para seleção quando selecionado o CTX
                                    //Session["PERFIL"] = usuario.UsuarioPerfil; 
                                    Session["USER"] = usuario.Login;
                                    Session["USER_ID"] = usuario.Codigo;
                                    Session["USUARIO"] = usuario;
                                    //return RedirectToAction("SelecionarContexto", new { RedirectTo = Request["RedirectTo"] });
                                    return Redirect(string.Format("{0}?Result=SELECIONE O CONTEXTO&Login={1}&Email={2}&ViewFile={3}&RedirectTo={4}&PreLogin={5}&Perfil={6}&Codigo={6}", loginUrl, Login, Email, "SelecionarContexto", RedirectTo, PreLogin, Perfil, Codigo));
                                }

                                //Define o nível de acesso na sessão
                                if (usuario.getContextos().Count == 1)
                                    SetContexto(usuario.getContextos()[0].Contexto);
                                else
                                {
                                    return Redirect(string.Format("{0}?Result=O USUÁRIO NÃO POSSUI CONTEXTO DEFINIDO&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
                                }
                            }
                        }

                        //EFETUA O LOGIN NA SESSÃO, SOMENTE COM A DEFINIÇÃO DO CONTEXTO
                        if (Core.GetSetCTX(HttpContext) != null && !string.IsNullOrEmpty(Core.GetSetCTX(HttpContext)))
                        {
                            //Pega o primeiro PERFIL do CTX selecionado
                            List<UsuarioPerfil> usuCtxPerfis = usuario.getCtxUsuPerfis(Core.GetSetCTX(HttpContext));

                            UsuarioPerfil usuPer = null;
                            if (usuCtxPerfis.Count > 0)
                            {
                                usuPer = usuCtxPerfis[0]; //usuario.getCtxUsuPerfil(Core.GetSetCTX(HttpContext));
                                
                                Session["PERFIL"] = usuPer.Titulo;

                                //Ativa o seletor de perfis dentro do contexto ativo
                                if (usuCtxPerfis.Count > 1)
                                {
                                    Session["SELETOR_PERFIL"] = true;
                                }

                            }
                            else {
                                return Redirect(string.Format("{0}?Result=USUÁRIO NÃO POSSUI PERFIL ATIVO PARA ESTE CONTEXTO&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
                            }

                            //RedirectTo = ( string.IsNullOrEmpty(RedirectTo) || (RedirectTo.Contains(ConfigurationManager.AppSettings["loginUrl"]) ) && !RedirectTo.Contains("ViewFile")) ? usuPer.RedirectTo : RedirectTo;
                            RedirectTo = (string.IsNullOrEmpty(RedirectTo) || !RedirectTo.Contains("ViewFile")) ? usuPer.RedirectTo : RedirectTo;
                            Session["USER"] = usuario.Login;
                            Session["USER_ID"] = usuario.Codigo;
                            Session["USUARIO"] = usuario;

                            //Define o tempo de sessão
                            int sessionTimeout = 0;
                            int.TryParse(FwkConfig.GetSettingValue("sessionTimeout", Session), out sessionTimeout);
                            Session.Timeout = (sessionTimeout == 0) ? 15 : sessionTimeout;

                            //Carrega a solicitação para troca de senha
                            if (usuario.DeveAlterarSenha)
                            {
                                return Redirect(string.Format("{0}?Alert=É necessário alterar a senha...&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}&ViewFile=AlterarSenha", new object[]
						        {
							        loginUrl,
							        Login,
							        Email,
							        RedirectTo,
							        PreLogin,
							        Perfil,
                                    Codigo
						        }));
                            }
                                                        
                            //Sucesso no Login
                            if(!RedirectTo.StartsWith("http"))
                                return Redirect(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + RedirectTo);
                            return Redirect(RedirectTo);
                        }
                        else
                        {
                            //Remove caso já tenham sido definidos na seleção do contexto dinâmico                    
                            Session["PERFIL"] = Session["USER"] = Session["USER_ID"] = Session["USUARIO"] = null;
                            return Redirect(string.Format("{0}?Result=Impossível fazer Login. Sessão perdida, por favor, tentar novamente.&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
                        }                      
                    }                  
                        
                    //Remove caso já tenham sido definidos na seleção do contexto dinâmico
                    Session["PERFIL"] = Session["USER"] = Session["USER_ID"] = Session["USUARIO"] = null;
                    Session["LoginAttemps"] = Session["LoginAttemps"] != null ? (int)Session["LoginAttemps"] + 1 : 1;
                    if (isDirectLogin)
                        return Redirect(string.Format("{0}?Result=Falha no Login. Link Inválido ou Expirado&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
                    return Redirect(string.Format("{0}?Result=Falha no Login&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&Codigo={6}", loginUrl, Login, Email, RedirectTo, PreLogin, Perfil, Codigo));
            }
        }

        public ActionResult DoLoginToken(String token, String RedirectTo = "")
        {
            //Valida o usuário
            Usuario usuario = new Usuario().ValidaUsuario(token);
            UsuarioPerfil usuPer = usuario.getCtxUsuPerfil(Core.GetSetCTX(HttpContext));
            if (usuPer == null || !usuPer.Ativo)
                return Redirect(string.Format("{0}?Result=USUÁRIO NÃO POSSUI PERFIL ATIVO PARA ESTE CONTEXTO&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}", loginUrl, usuario.Login, string.Empty, RedirectTo, "false", string.Empty));
            //Session["PERFIL"] = usuario.UsuarioPerfil;
            Session["PERFIL"] = usuPer.Titulo;
            Session["USER"] = usuario.Login;
            Session["USER_ID"] = usuario.Codigo;
            Session["USUARIO"] = usuario;
            RedirectTo = string.IsNullOrEmpty(RedirectTo) ? loginUrl : RedirectTo;
            if (usuario.DeveAlterarSenha)
            {
                return Redirect(string.Format("{0}?Alert=É necessário alterar a senha...&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}&ViewFile=AlterarSenha", new object[]
						        {
							        loginUrl,
							        usuario.Login,
							        string.Empty,
							        RedirectTo,
							        false,
							        usuPer.Titulo
						        }));
            }
            return Redirect(RedirectTo);
        }

        public ActionResult Logout(string Result = "", string Alert = "", string RedirectTo = "")
        {
            string perfil = Session["PERFIL"] != null ? Session["PERFIL"].ToString() : string.Empty;
            string ctx = Core.GetSetCTX(HttpContext) ?? string.Empty; 

            ViewBag.Msg = Result;

            Session["USER"] = Session["USER_ID"] = Session["PERFIL"] = Session["USUARIO"] = null;

            //Default
            return Redirect(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ConfigurationManager.AppSettings["loginUrl"].ToString() + "?ctx=" + ctx + "&Alert=" + Alert + "&RedirectTo=" + RedirectTo);           
        }

        public ActionResult EnviarSenha(string Email, String Login, string Perfil, string RedirectTo, bool PreLogin, string Codigo = "", string Result = "")
        {
            string viewFile = "DevHelper/Usuario/EnviarSenha";
            ViewBag.Titulo = "Enviar Senha";
            ViewBag.Perfil = Perfil;
            ViewBag.Email = Email;
            ViewBag.RedirectTo = RedirectTo;
            ViewBag.Login = Login;
            ViewBag.PreLogin = PreLogin;
            ViewBag.Msg = Result;
            //return PartialView(viewFile);
            return View(viewFile);
        }

        [FwkAuthorize(CustomCall = "/Usuario/EnviarSenha"), System.Web.Mvc.HttpPost]
        public ActionResult EnviarSenha(String Login, String inputSenha, String Perfil="", String Email = "", String RedirectTo = "", bool PreLogin = false, String Codigo = "", String Sender = "")
        {
            //Verificar a existência do Usuário 
            //-> Default: Não checa o contexto. Quando o perfil for vazio, checa pelo login e CTX)
            Usuario usuario = null;
            if (Core.GetSetCTX(HttpContext) != null)            
                usuario = new Usuario().GetUsuariobyLoginPerfil(Login, Perfil, Core.GetSetCTX(HttpContext));                            
            else
                usuario = new Usuario().GetUsuariobyLoginPerfil(Login, Perfil);

            if (!string.IsNullOrEmpty(Codigo) && Core.IsEncrypted(Codigo))
            {
                usuario = new Usuario().GetUsuario(int.Parse(Core.Decrypt(Codigo)));
            }

            //Código auxiliar para buscar o usuário
            usuario = usuario == null ? new Usuario().GetUsuariobyLoginEmail(Login, Email) : usuario;
            //Condição para localizar o usuario utilizando o email
            usuario = usuario == null ? new Usuario().GetUsuariobyLoginEmail(Email, Email) : usuario;

            //Getting current CTX
            var CTX = Core.GetSetCTX(HttpContext);

            if (usuario == null)
                return RedirectToAction("EnviarSenha", new { Email = Email, Login = Login, Perfil = Perfil, RedirectTo = RedirectTo, PreLogin = PreLogin, Result = "Solicitação não permitida, usuário não encontrado" });
                //return Redirect(ConfigurationManager.AppSettings["loginUrl"].ToString() + "?Result=Solicitação não permitida, usuário não encontrado");                            
                //return RedirectToAction("Index", "Participante", new { Result = "Solicitação não permitida, e-mail não cadastrado." });

            if(string.IsNullOrEmpty(usuario.Email) || !Email.Trim().Equals(usuario.Email.Trim())){
                return RedirectToAction("EnviarSenha", new { Email = Email, Login = Login, Perfil = Perfil, RedirectTo = RedirectTo, PreLogin = PreLogin, Result = "Solicitação não permitida, e-mail não cadastrado" });
            }            

            if (FwkConfig.GetSettingValue("MailServer", CTX) == string.Empty)
                return JavaScript("Sistema para envio de mensagens não configurado.");

            //usuario.Senha = senha;
            var usuarioEdit = _contexto.Usuarios.Find(usuario.Codigo);
            //Gera senha 
            string senhaBkp = usuarioEdit.Senha;
            string senha;
            do
            {   //Info: Evita a ocorrência do caractere '+', pois causa problema no acesso ao link por QueryString
                senha = new Random().Next(100000, 999999).ToString();
                //Encriptando a senha
                senha = Core.Crypto(senha);
            } while (senha.Contains("+"));
            usuarioEdit.Senha = usuarioEdit.ConfirmSenha = senha;
            usuarioEdit.DeveAlterarSenha = true;
            usuarioEdit.DtUltAtualizacao = DateTime.Now;
            string urlNewPass = FwkConfig.GetSettingValue("pathApp", CTX) + "/Usuario/DoLogin/?inputSenha=" + Core.Crypto(senha) + "&Login=" + Login + "&Email=" + Email + "&RedirectTo=" + RedirectTo + "&PreLogin=" + PreLogin.ToString() + "&ctx=" + CTX;

            var MailHelper = new MailHelper(FwkConfig.GetSettingValue("MailServer", CTX), 
                FwkConfig.GetSettingValue("MailPort", CTX), FwkConfig.GetSettingValue("MailAuthUser", CTX),
                FwkConfig.GetSettingValue("MailAuthPass", CTX), FwkConfig.GetSettingValue("MailEnableSSL", CTX),
                FwkConfig.GetSettingValue("MailEmailFromAddress", CTX), FwkConfig.GetSettingValue("MailDisplayName", CTX))
            {
                //Sender = "EJC - Sistema de Cadastros",
                Recipient = usuario.Email,
                RecipientCC = null,
                Subject = "Envio de Senha",
                //Body = "Sua senha de acesso é: "+senha+Environment.NewLine+
                Body = "<br />- Acesse o link para alterar sua senha: "+
                "<a href='"+ urlNewPass + "' title='Clique neste link para recriar sua senha'>"+ urlNewPass + "</a>"+
                "<br />- Ao efetuar o login, favor alterar a senha."
            };

            bool sucesso;
            try
			{
                this._contexto.SaveChanges();
                sucesso = MailHelper.Send();
			}
			catch
			{
                return JavaScript("Erro no processo de alteração da senha");
			}

            if(sucesso)
                return JavaScript("O link para troca de senha foi enviado para seu e-mail, ao acessar o sistema será necessário alterá-la.");            
            else 
                return JavaScript("Falha no envio do email, tente novamente mais tarde...");            
        }        

        [FwkAuthorize(CustomCall = "/Usuario/AlterarSenha"), HttpPost]
        public ActionResult AlterarSenha(Usuario Usuario, string RedirectTo = "", string Login = "", string Perfil = "", bool isPartial = false)
        {
            ModelState.Clear();

            if(string.IsNullOrEmpty(Request["Senha"]))
                ModelState.AddModelError("Senha", "Uma senha deve ser informada");

            if (!Request["Senha"].Equals(Request["ConfirmSenha"]))
                ModelState.AddModelError("ConfirmSenha", "A senha não foi confirmada corretamente");

            Usuario.Codigo = Session["USUARIO"] != null && Usuario.Codigo == 0 ? ((Usuario)Session["USUARIO"]).Codigo : Usuario.Codigo;
            var userToEdit = _contexto.Usuarios.Find(new object[] { Usuario.Codigo });

            if (Request["Senha"].Equals(Core.Decrypt(userToEdit.Senha)))
                ModelState.AddModelError("Senha", "Por favor, informe uma nova senha");

            ChecaForcaSenha verifyPass = new ChecaForcaSenha();
            ViewBag.DicaSenha = verifyPass.GetDicaSenha(HttpContext);
            if (!verifyPass.VerificaForcaSenha(Request["Senha"], HttpContext))
                ModelState.AddModelError("Senha", verifyPass.InfoMessage);            

            if (!ModelState.IsValid)
            {
                String errorMessages = Util.GetModelStateErrors(ModelState);
                //TODO: remover variável de sessão
                return Redirect(String.Format(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ConfigurationManager.AppSettings["loginUrl"].ToString() + "?ctx=" + Session["CTX"] +"&Alert=Erro na execução da operação! (" + errorMessages + ")&ViewFile=AlterarSenha&isPartial={0}&Login={1}&Perfil={2}&RedirectTo={3}", Request["isPartial"], Request["Login"], Request["Perfil"], Request["RedirectTo"]));                            
            }

            //Altera a Senha do usuário      
            userToEdit.DtUltAtualizacao = DateTime.Now;
            userToEdit.Senha = Request["Senha"];
            userToEdit.ConfirmSenha = Request["ConfirmSenha"];
            userToEdit.DeveAlterarSenha = false;

            try
            {
                _contexto.SaveChanges();
                #region Envia e-mail com a notificação da alteração da senha
                //if (!string.IsNullOrEmpty(Usuario.Email))
                //{
                //    var MailHelper = new MailHelper
                //    {
                //        //Sender = "EJC - Sistema de Cadastros",
                //        Recipient = Request["Senha"],
                //        RecipientCC = null,
                //        Subject = "Alteração de Senha",
                //        Body = "Sua senha foi alterada com êxito."
                //    };
                //    MailHelper.Send();
                //}
                #endregion
            }
            catch {
                return Redirect(String.Format(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ConfigurationManager.AppSettings["loginUrl"].ToString() + "?ctx="+ Session["CTX"] + "&Alert=Erro na execução da operação!&ViewFile=AlterarSenha&isPartial={0}&Login={1}&Perfil={2}", Request["isPartial"], Request["Login"], Request["Perfil"]));                            
            }

            if (!string.IsNullOrEmpty(Request["RedirectTo"]))
            {
                UsuarioPerfil perUser = userToEdit.getCtxUsuPerfil(Core.GetSetCTX(HttpContext));
                RedirectTo = Request["RedirectTo"];
                //QueryString
                string qs = RedirectTo.Split('?').Length > 1 ? RedirectTo.Split('?')[1] : string.Empty;
                //Host
                RedirectTo = RedirectTo.Split('?')[0];
                //Adiciona valores à QueryString
                NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(qs);
                queryString.Add("Alert", "A senha foi alterada com êxito");
                queryString.Add("isPartial", Request["isPartial"]);
                queryString.Add("Login", Request["Login"]);
                queryString.Add("Perfil", Request["Perfil"]);
                if (!RedirectTo.Contains("Alert"))
                    RedirectTo += RedirectTo.Contains("?") ? "&" + queryString.ToString() : "?" + queryString.ToString();
                if (!RedirectTo.StartsWith("http"))
                    return Redirect(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + RedirectTo);
                else
                    return Redirect(RedirectTo);                
            }
            else
                return Redirect(String.Format(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ConfigurationManager.AppSettings["loginUrl"].ToString() + "?ctx="+Session["CTX"] +"&Alert=Sua senha foi alterada com êxito&ViewFile=AlterarSenha&isPartial={0}&Login={1}&Perfil={2}", Request["isPartial"], Request["Login"], Request["Perfil"] ));                            
        }

        public ActionResult SelecionarContexto(string RedirectTo = "")
        {
            string viewFile = "DevHelper/Usuario/SelecionarContexto";

            if (Session["PERFIL"] == null)
                return Redirect(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ConfigurationManager.AppSettings["loginUrl"].ToString() + "?Result=PERFIL NÃO DEFINIDO!");                                            

            Usuario usuario = new Usuario().GetUsuariobyLoginPerfil(Session["USER"].ToString(), Session["PERFIL"].ToString(), Core.GetSetCTX(HttpContext));

            Session["TOKEN"] = new Random().Next(10000, 99999).ToString();
            
            ViewBag.Contextos = usuario.getContextos();
            ViewBag.RedirectTo = RedirectTo;

            return PartialView(viewFile);
        }        

        [HttpPost]
        public ActionResult SelecionarContextoAction()
        {
            if (Session["TOKEN"] == null || string.IsNullOrEmpty(Session["TOKEN"].ToString()))
                return Redirect(ConfigurationManager.AppSettings["loginUrl"].ToString() + "?Result=Acesso por aplicações externas não permitido!");                                            

            string RedirectTo = Request["RedirectTo"];
            
            //DEFININDO O CTX
            SetContexto(Request["Contexto"]);

            //Pega o PERFIL do CTX selecionado            
            UsuarioPerfil usuPer = ((Usuario)Session["USUARIO"]).getCtxUsuPerfil(Core.GetSetCTX(HttpContext));
            if (usuPer == null || !usuPer.Ativo)
                return Redirect(string.Format("{0}?Result=USUÁRIO NÃO POSSUI PERFIL ATIVO PARA ESTE CONTEXTO&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}", FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ConfigurationManager.AppSettings["loginUrl"].ToString(), ((Usuario)Session["USUARIO"]).Login, ((Usuario)Session["USUARIO"]).Email, ConfigurationManager.AppSettings["loginUrl"].ToString(), true, string.Empty));
            
            //DEFININDO O PERFIL
            Session["PERFIL"] = usuPer.Titulo;

            //Redirect to:
            Session["TOKEN"] = null;

            RedirectTo = !RedirectTo.Contains("ViewFile") ? usuPer.RedirectTo : RedirectTo;
            if (!string.IsNullOrEmpty(RedirectTo))
            {
                if (!RedirectTo.StartsWith("http"))
                    return Redirect(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + RedirectTo);
                else
                    return Redirect(RedirectTo); 
            }

            return Redirect(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ((Usuario)Session["USUARIO"]).getCtxUsuPerfil(Core.GetSetCTX(HttpContext)).RedirectTo);
        }

        public ActionResult SelecionarPerfil(string RedirectTo = "")
        {
            string viewFile = "DevHelper/Usuario/SelecionarPerfil";

            Usuario usuario = new Usuario().GetUsuariobyLoginPerfil(Session["USER"].ToString(), Session["PERFIL"].ToString(), Core.GetSetCTX(HttpContext));

            Session["TOKEN"] = new Random().Next(10000, 99999).ToString();

            ViewBag.Perfis = usuario.getCtxUsuPerfis(Core.GetSetCTX(HttpContext));
            ViewBag.RedirectTo = RedirectTo;

            return PartialView(viewFile);
        }

        [HttpPost]
        public ActionResult SelecionarPerfilAction()
        {
            if (Session["TOKEN"] == null || string.IsNullOrEmpty(Session["TOKEN"].ToString()))
                return Redirect(ConfigurationManager.AppSettings["loginUrl"].ToString() + "?Result=Acesso por aplicações externas não permitido!");            

            string RedirectTo = Request["RedirectTo"];

            //DEFININDO O PERFIL
            Session["PERFIL"] = !string.IsNullOrEmpty(Request["Perfil"]) ? Request["Perfil"] : Session["PERFIL"];

            //Pega o PERFIL do CTX selecionado            
            UsuarioPerfil usuPer = ((Usuario)Session["USUARIO"]).getCtxUsuPerfil(Core.GetSetCTX(HttpContext), Session["PERFIL"].ToString());
            if (usuPer == null || !usuPer.Ativo)
                return Redirect(string.Format("{0}?Result=USUÁRIO NÃO POSSUI ESSE PERFIL ATIVO PARA ESTE CONTEXTO&Login={1}&Email={2}&RedirectTo={3}&PreLogin={4}&Perfil={5}", FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ConfigurationManager.AppSettings["loginUrl"].ToString(), ((Usuario)Session["USUARIO"]).Login, ((Usuario)Session["USUARIO"]).Email, ConfigurationManager.AppSettings["loginUrl"].ToString(), true, string.Empty));

            //Redirect to:
            Session["TOKEN"] = null;

            RedirectTo = RedirectTo == null || !RedirectTo.Contains("ViewFile") ? usuPer.RedirectTo : RedirectTo;
            if (!string.IsNullOrEmpty(RedirectTo))
            {
                if (!RedirectTo.StartsWith("http"))
                    return Redirect(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + RedirectTo);
                else
                    return Redirect(RedirectTo);
            }

            return Redirect(FwkConfig.GetSettingValue("pathApp", Core.GetSetCTX(HttpContext)) + ((Usuario)Session["USUARIO"]).getCtxUsuPerfil(Core.GetSetCTX(HttpContext)).RedirectTo);
        }

        //Acesso direto via Action
        [FwkAuthorize]
        public ActionResult Perfis(string Tema = "DevEx")
        {
            string viewFile = "DevHelper/Usuario/Perfis";
            ViewBag.Tema = Tema;
            ViewBag.dtSource = (from uPerfils in _contexto.UsuarioPerfis select uPerfils).ToList<UsuarioPerfil>();        
            return PartialView(viewFile);
        }

        [FwkAuthorize(CustomCall = "/Usuario/SalvarPerfil"), System.Web.Mvc.HttpPost]
        public ActionResult SalvarPerfil(UsuarioPerfil Perfil, string CRUDAction = "", string RedirectTo = "")
        {
            string viewFile = "DevHelper/Usuario/SalvarPerfil";

            if (CRUDAction.Equals("DELETE"))
            {
                Perfil.Titulo = Request["gridKey"].Replace("\"", "");
            }
            var encontrado = _contexto.UsuarioPerfis.Find(Perfil.Titulo);

            if (CRUDAction.Equals("SALVAR") || CRUDAction.Equals("DELETE") /*Request.HttpMethod == "POST"*/)
            {
                string action = "Realizado";

                if (!ModelState.IsValid && !CRUDAction.Equals("DELETE"))
                {
                    ViewBag.Msg = "Ocorreram erros no preenchimento do formulário, por favor, revise e tente novamente.";
                    return PartialView(viewFile);
                }

                try
                {
                    if ((encontrado == null || Perfil.Titulo.Equals(string.Empty)) && !CRUDAction.Equals("DELETE"))
                    {
                        _contexto.UsuarioPerfis.Add(Perfil);                        
                    }
                    else if (CRUDAction.Equals("SALVAR")) //EDIÇÃO DE REGISTRO
                    {
                        //Edita instância completa do objeto
                        _contexto.Entry(encontrado).CurrentValues.SetValues(Perfil);

                        action = "Atualizado";
                    }
                    else if (CRUDAction.Equals("DELETE"))
                    {
                        //string[] array = base.Request["Titulo"].Split(new char[]
                        //    {
                        //        '|'
                        //    });
                        //Perfil.Titulo = array[0];
                        encontrado = this._contexto.UsuarioPerfis.Find(new object[]
						{
							Perfil.Titulo
						});
                        this._contexto.UsuarioPerfis.Remove(encontrado);
                        action = "Removido";
                        this._contexto.SaveChanges();
                        ViewBag.Alert = string.Format("Registro {0} com Sucesso.", action);
                        if (!RedirectTo.Contains("Alert"))
                            RedirectTo = (RedirectTo.Contains("?")) ? RedirectTo + "&Alert=" + ViewBag.Alert : RedirectTo + "?ctx="+ Session["CTX"] + "&Alert=" + ViewBag.Alert;
                        return this.Redirect(RedirectTo);
                    }

                    _contexto.SaveChanges();

                }
                catch (Exception ex)
                {
                    ViewBag.Msg = String.Format("Falha na Operação. Detalhes:{0}", ex.Message);
                    return PartialView(viewFile);
                }

                ViewBag.Msg = String.Format("Cadastro {0} com Sucesso.", action);
                //Atualizando os dados do grid (CallBack)
                TempData["SessionModelPerfis" + Core.GetSetCTX(HttpContext)] = (from uPerfils in _contexto.UsuarioPerfis select uPerfils).ToList<UsuarioPerfil>();
                //Session["SessionModelPerfis" + Core.GetSetCTX(HttpContext)] = (from uPerfils in _contexto.UsuarioPerfis select uPerfils).ToList<UsuarioPerfil>();
                return PartialView(viewFile);

            }
            else
                ModelState.Clear();

            if (encontrado == null)
                encontrado = new UsuarioPerfil();

            return PartialView(viewFile, encontrado);
        }

        [FwkAuthorize(CustomCall = "/Usuario/SalvarContexto"), System.Web.Mvc.HttpPost]
        public ActionResult SalvarContexto(UsuarioAlcada UsuarioAlcada, string CodigoUsuario, string Contexto, string CRUDAction = "", string RedirectTo = "")
        {
            string viewFile = "DevHelper/Usuario/SalvarContexto";

            if (CRUDAction.Equals("DELETE"))
            {
                UsuarioAlcada.CodigoUsuario = int.Parse(Request["gridKey"].Replace("\"", "").Split('|')[0]);
                UsuarioAlcada.Contexto = Request["gridKey"].Replace("\"", "").Split('|')[1];
                UsuarioAlcada.UsuarioPerfil = Request["gridKey"].Replace("\"", "").Split('|')[2];
            }
            var encontrado = _contexto.UsuarioAlcadas.Find(new object[] { 
                UsuarioAlcada.CodigoUsuario, 
                UsuarioAlcada.Contexto,
                UsuarioAlcada.UsuarioPerfil
            });

            if (CRUDAction.Equals("SALVAR") || CRUDAction.Equals("DELETE")) 
            {
                string action = "Realizado";

                if (!ModelState.IsValid)
                {
                    ViewBag.Msg = "Ocorreram erros no preenchimento do formulário, por favor, revise e tente novamente.";
                    return PartialView(viewFile);
                }

                try
                {
                    if ((encontrado == null || UsuarioAlcada.CodigoUsuario == 0) && !CRUDAction.Equals("DELETE"))
                    {
                        _contexto.UsuarioAlcadas.Add(UsuarioAlcada);
                    }
                    else if (CRUDAction.Equals("SALVAR"))
				    {
                        //Edita instância completa do objeto
                        _contexto.Entry(encontrado).CurrentValues.SetValues(UsuarioAlcada);

                        action = "Atualizado";
                    }
                    else if (CRUDAction.Equals("DELETE"))
                    {
                        if(encontrado != null)
                            this._contexto.UsuarioAlcadas.Remove(encontrado);
                        action = "Removido";
                        this._contexto.SaveChanges();
                        ViewBag.Alert = string.Format("Registro {0} com Sucesso.", action); 
                        if (!RedirectTo.Contains("Alert"))
                            RedirectTo = (RedirectTo.Contains("?")) ? RedirectTo + "&Alert=" + ViewBag.Alert : RedirectTo + "?ctx="+Session["CTX"] +"&Alert=" + ViewBag.Alert;
                        return this.Redirect(RedirectTo);
                    }

                    _contexto.SaveChanges();

                }
                catch (Exception ex)
                {
                    ViewBag.Msg = String.Format("Falha na Operação. Detalhes:{0}", ex.Message);
                    return PartialView(viewFile);
                }

                ViewBag.Msg = String.Format("Cadastro {0} com Sucesso.", action);
                //Atualizando os dados do grid (CallBack)
                //Session["SessionModelContextos" + Core.GetSetCTX(HttpContext)] = (from uContextos in _contexto.UsuarioAlcadas select uContextos).ToList<UsuarioAlcada>();
                TempData["SessionModelContextos" + Core.GetSetCTX(HttpContext)] = (from uContextos in _contexto.UsuarioAlcadas select uContextos).ToList<UsuarioAlcada>();
                return PartialView(viewFile);

            }
            else
                ModelState.Clear();

            if (encontrado == null)
                encontrado = new UsuarioAlcada();

            return PartialView(viewFile, encontrado);
        }

        [FwkAuthorize(CustomCall = "/Usuario/SalvarUsuario"), System.Web.Mvc.HttpPost]
        public ActionResult SalvarUsuario(Usuario Usuario, string CRUDAction = "", string RedirectTo = "", bool isPartial = false)
        {
            string viewFile = "DevHelper/Usuario/SalvarUsuario";
            ViewBag.Perfis = new UsuarioPerfil().GetPerfis();
            ViewBag.isPartial = isPartial;
            if (CRUDAction.Equals("DELETE"))
            {
                Usuario.Codigo = int.Parse(Request["gridKey"].Replace("\"",""));
            }
            var encontrado = _contexto.Usuarios.Find(Usuario.Codigo);            

            if (CRUDAction.Equals("SALVAR") || CRUDAction.Equals("DELETE") /*Request.HttpMethod == "POST"*/)
            {
                string action = "Realizado";

                if (!CRUDAction.Equals("DELETE"))
                {
                    if (!Request["Senha"].Equals(Request["ConfirmSenha"]))
                        ModelState.AddModelError("ConfirmSenha", "A senha não foi confirmada corretamente");

                    //Não valida para edição de registro, apenas para NOVO
                    if (encontrado == null) 
                    {
                        ChecaForcaSenha verifyPass = new ChecaForcaSenha();
                        if (!verifyPass.VerificaForcaSenha(Request["Senha"], HttpContext))
                            ModelState.AddModelError("Senha", verifyPass.InfoMessage);
                    }

                    if (!ModelState.IsValid)
                    {
                        ViewBag.Msg = "Ocorreram erros no preenchimento do formulário, por favor, revise e tente novamente.";
                        return PartialView(viewFile, Usuario);
                    }
                }

                try
                {
                    //Encriptando a Senha 
                    Usuario.DtUltAtualizacao = DateTime.Now;
                    //Usuario.Senha = Usuario.Senha != null ? Core.Crypto(Usuario.Senha) : null;
                    if ((encontrado == null || Usuario.Codigo == 0) && !CRUDAction.Equals("DELETE"))
                    {
                        _contexto.Usuarios.Add(Usuario);
                    }
                    else if(CRUDAction.Equals("SALVAR")) //EDIÇÃO DE REGISTRO
                    {
                        //Mantem a senha atual caso uma nova não seja informada
                        if (string.IsNullOrEmpty(Usuario.Senha))
                        {
                            Usuario.Senha = encontrado.Senha;
                        }
                        //Edita instância completa do objeto                        
                        _contexto.Entry(encontrado).CurrentValues.SetValues(Usuario);

                        action = "Atualizado";
                    }
                    else if (CRUDAction.Equals("DELETE"))
                    {
                        encontrado = this._contexto.Usuarios.Find(new object[]
						{
							Usuario.Codigo
						});
                        this._contexto.Usuarios.Remove(encontrado);
                        action = "Removido";
                        this._contexto.SaveChanges();
                        ViewBag.Alert = string.Format("Registro {0} com Sucesso.", action); 
                        if (!RedirectTo.Contains("Alert"))
                            RedirectTo = (RedirectTo.Contains("?")) ? RedirectTo + "&Alert=" + ViewBag.Alert : RedirectTo + "?ctx="+Session["CTX"] +"&Alert=" + ViewBag.Alert;
                        return this.Redirect(RedirectTo);
                        //ViewBag.dtSource = (from users in _contexto.Usuarios select users).ToList<Usuario>();
                        //return PartialView(viewFile);
                    }
                    
                    _contexto.SaveChanges();

                }
                catch (Exception ex)
                {
                    ViewBag.Msg = String.Format("Falha na Operação. Detalhes:{0}", ex.Message);
                    return PartialView(viewFile);
                }

                ViewBag.Msg = String.Format("Cadastro {0} com Sucesso.", action);
                //Atualizando os dados do grid (CallBack)
                //Session["SessionModelUsuarios" + Core.GetSetCTX(HttpContext)] = (from users in _contexto.Usuarios select users).ToList<Usuario>();
                base.TempData["SessionModelUsuarios" + Core.GetSetCTX(HttpContext)] = (from users in _contexto.Usuarios select users).ToList<Usuario>();

                if (!string.IsNullOrEmpty(Request["RedirectTo"]))
                    return Redirect(Request["RedirectTo"]);

                return PartialView(viewFile);

            }
            else
                ModelState.Clear();

            if (encontrado == null || encontrado.Codigo == 0)
            {
                encontrado = new Usuario();
            }

            return PartialView(viewFile, encontrado);
        }
        
        /// <summary>
        /// LicensingForm = Gerador de licenças
        /// MAC|01/01/2200|APPNAME
        /// </summary>
        /// <returns></returns>
        [FwkAuthorize(CustomCall = "/Usuario/Encrypto")]
        public ActionResult LicensingForm(string ClientID)
        {
            if (!string.IsNullOrEmpty(ClientID))
            {
                string[] valores = Core.Decrypt(ClientID).Split('|');
                ViewBag.Mac = valores[0];
                ViewBag.App = valores[1];
            }
            //return PartialView("DevHelper/Usuario/LicensingForm");
            return View("DevHelper/Usuario/LicensingForm");
        }

        [FwkAuthorize(CustomCall = "/Usuario/Encrypto")]
        public async Task<ActionResult> Encrypto(string App, string Data, string Mac)
        {
            #region Utilizado para ajuste
            /*if(!string.IsNullOrEmpty(key) && key.Equals("elmar#2k17"))
            {
                int sucesso = 0;
                List<int> idErrors = new List<int>();
                string idErrorsStr = "-";
                List<Usuario> usuarios = (from users in _contexto.Usuarios select users).ToList<Usuario>();
                foreach (var item in usuarios)
                {
                    //Atualiza a Senha para o formato encriptado
                    var usuarioEdit = _contexto.Usuarios.Find(item.Codigo);
                    if (Core.IsEncrypted(usuarioEdit.Senha))
                        continue;
                    usuarioEdit.Senha = usuarioEdit.ConfirmSenha = Core.Crypto(item.Senha);
                    if (usuarioEdit.Email == null)
                        usuarioEdit.Email = "suporte@elmarinformatica.com.br";
                    //usuarioEdit.DeveAlterarSenha = true;
                    try
                    {
                        this._contexto.SaveChanges();
                        sucesso++;
                    }
                    catch
                    {
                        idErrors.Add(item.Codigo);
                        idErrorsStr += item.Codigo + ";";
                    }
                }
                return JavaScript(sucesso.ToString() + "/" + usuarios.Count + " usuários atualizados com sucesso. Id Errors: " + idErrorsStr);
            }*/
            #endregion
            if (!string.IsNullOrEmpty(App) && !string.IsNullOrEmpty(Mac)) 
            {
                //Gerador de Chave de Licença
                return JavaScript("License key: " + Core.Crypto(Mac+"|"+Data+"|"+App));
            }
            return JavaScript("Acesso negado.");
        }

        [FwkAuthorize(CustomCall = "/Usuario/RevealPass", CustomError = "")]
        public ActionResult RevealPass(string pass)
        {            
            if(string.IsNullOrEmpty(pass))
                return JavaScript(String.Empty);
            return JavaScript("<p>Senha Atual:  "+Core.Decrypt(pass)+"</p>");
        }
    }
}
