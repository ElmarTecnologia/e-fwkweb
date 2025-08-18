using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ELMAR.DevHtmlHelper.Models;
using ELMAR.DevHtmlHelper.Models.CustomValidation;
using System.Collections;
using System.Reflection;
using DevExpress.Web.Mvc;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace ELMAR.DevHtmlHelper.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FwkController : Controller
    {                
        private readonly FwkContexto _contextoFwk = new FwkContexto();

        /// <summary>
        /// Método para exibição padronizada de mensagem
        /// </summary>
        /// <param name="Result">Mensagem de resultado</param>
        /// <param name="RedirectTo">Página para o redirecionamento</param>
        /// <param name="isPartial">Padrão de exibição</param>
        /// <returns></returns>
        public async Task<ActionResult> Info(string Result, string RedirectTo="", bool isPartial = false)
        {
            string viewFile = "DevHelper/_Info";
            ViewBag.Result = Result;
            ViewBag.Redirect = RedirectTo;
            ViewBag.isPartial = isPartial;
            return PartialView(viewFile);
        }

        /// <summary>
        /// Método para executar redirect
        /// </summary>
        /// <param name="Url">Url para o redirecionamento</param>
        /// <returns></returns>
        public async Task<ActionResult> RedirectTo(string Url)
        {
            return Redirect(Url);
        }

        /// <summary>
        /// Método para exibir o formulário de envio de mensagens
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="Tema">Tema dev</param>
        /// <param name="RedirectTo">Página de redirecionamento após o envio da mensagem</param>
        /// <returns></returns>
        [FwkAuthorize]
        public async Task<ActionResult> MailSender(string Result = "", string Tema = "", string RedirectTo = "")
        {
            ViewBag.Titulo = "Envio de Mensagens";
            ViewBag.Result = Result;
            ViewBag.RedirectTo = RedirectTo;
            ViewBag.Tema = Tema;
            ViewBag.Perfis = new UsuarioPerfil().GetPerfis();            
            return PartialView("DevHelper/MailSender");
        }

        /// <summary>
        /// Método de envio da mensagem, recebe apenas execuções via POST
        /// </summary>
        /// <param name="assunto">Assunto da mensagem</param>
        /// <param name="grupo">Grupo para envio, utiliza os usuários do sistema agrupados por perfis</param>
        /// <param name="emails">Email ou lista de emails separados por ';'</param>
        /// <param name="RedirectTo">Página de redirecionamento para a resposta da execução</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [FwkAuthorize]
        public async Task<ActionResult> MailSender(string assunto = "", string mensagem = "", string grupo = "", string emails = "", string RedirectTo = "")
        {            
            ViewBag.Titulo = "Envio de Mensagens";
            ViewBag.Perfis = new UsuarioPerfil().GetPerfis();
            #region POST Action (Form)
            if (!grupo.Equals(string.Empty))
            {
                UsuarioPerfil Perfil = _contextoFwk.UsuarioPerfis.Find(grupo);
                if (Perfil != null)
                {
                    if (!string.IsNullOrEmpty(emails))
                        emails += ";";

                    //Pega a lista de usuários do Perfil contextualizado
                    List<Usuario> usuariosList = Perfil.getUsuariosList(Core.GetSetCTX(HttpContext));

                    foreach (var item in usuariosList)
                    {
                        //Verifica se o usuário pertence ao contexto selecionado
                        if (!string.IsNullOrEmpty(item.Email) && Util.IsASCII(item.Email))
                            emails += item.Email + ";";
                    }
                }
            }

            mensagem = HtmlEditorExtension.GetHtml("mensagem_Html");
            var MailHelper = new MailHelper(FwkConfig.GetSettingValue("MailServer", Session), 
                FwkConfig.GetSettingValue("MailPort", Session), FwkConfig.GetSettingValue("MailAuthUser", Session),
                FwkConfig.GetSettingValue("MailAuthPass", Session), FwkConfig.GetSettingValue("MailEnableSSL", Session),
                FwkConfig.GetSettingValue("MailEmailFromAddress", Session), FwkConfig.GetSettingValue("MailDisplayName", Session))
            {
                Recipient = emails.Split(';')[0],
                RecipientCC = emails,
                Subject = assunto /*ConfigurationManager.AppSettings["appName"] + " - " + */,
                Body = mensagem
            };
            //Sem teste de envio
            string result = string.Empty;
            MailHelper.Send();
            result = MailHelper.Result;
            ViewBag.Result = result;

            if (!string.IsNullOrEmpty(RedirectTo))
            {
                ControllerContext.HttpContext.Response.Redirect(RedirectTo + "?Result=" + result);
                return null;
            }
            #endregion
            return PartialView("DevHelper/MailSender");
        }

        /// <summary>
        /// Método utilizado para envio de e-mails (11/2015)
        /// </summary>
        /// <param name="server">Servidor de E-mails</param>
        /// <param name="port">Porta para o serviço</param>
        /// <param name="user">Usuário para autenticação</param>
        /// <param name="pass">Senha para autenticação</param>
        /// <param name="ssl">Habilita certificado ssl (true/false)</param>
        /// <param name="sender">E-mail responsável pelo envio do e-mail</param>
        /// <param name="assunto">Assunto da mensagem</param>
        /// <param name="conteudo">Conteúdo da mensagem(e-mail)</param>
        /// <param name="destino">E-mail destino(Recipient)</param>
        /// <returns></returns>
        [HttpPost]   
        [FwkAuthorize]    
        public async Task<ActionResult> SendMail(string server, string port, string user, string pass, string ssl, string sender, string assunto, string conteudo, string destino, string redirect="")
        {
            MailHelper mailHelper = new MailHelper(server, port, user, pass, ssl, sender);
            mailHelper.Recipient = destino;
            mailHelper.RecipientCC = null;
            mailHelper.Subject = assunto;
            mailHelper.Body = conteudo;
            string result = mailHelper.Send().ToString();
            if (!string.IsNullOrEmpty(redirect))
                return Redirect(redirect+"?result="+result);
            return Content(result);
        }
        
        //TODO: Implementar sobrecarga obtendo o nome a partir de um repositório de arquivos do FWK
        /// <summary>
        /// Método para solicitar o download de um arquivo, utiliza encriptação para a exibição de caminhos e nomes de arquivos
        /// </summary>
        /// <param name="fileName">Nome do arquivo físico, deve conter a encriptação padrão do E-Fwkweb</param>
        /// <param name="fileDownloadName">Nome do arquivo para exibição</param>
        /// <returns></returns>
        public FileResult DownloadAction(string fileName, string fileDownloadName = "") {
            //Correção para o caractere '+' enviado via QueryString
            fileName = fileName.Replace(' ', '+');
            //Não aceita fileName sem encriptação 
            fileName = Core.IsEncrypted(fileName) ? Core.Decrypt(fileName) : string.Empty; 
            if (fileName.Equals(string.Empty))
                return null;
            string[] fileNameParts = fileName.Split('.');
            string contentType = "application/"+fileNameParts[fileNameParts.Length-1];            
            fileDownloadName = string.IsNullOrEmpty(fileDownloadName) ? fileName : fileDownloadName;
            //Acrescenta a extensão caso ausente
            fileDownloadName = !fileDownloadName.Contains('.') ? fileDownloadName + "." + fileNameParts[fileNameParts.Length-1] : fileDownloadName;
            return File(fileName, contentType, fileDownloadName);
        }

        [HttpGet]
        [DeleteFileAttribute] //Action Filter, it will auto delete the file after download, 
        public async Task<ActionResult> DownloadTemp(string file, object content = null)
        {
            if (content != null && ((string[])content)[0].StartsWith("fileContent_")){
                content = TempData[((string[])content)[0]];
            }

            //Cria o arquivo
            Util.FileWriter((string)content, file, "~/temp", true);

            //get the temp folder and file path in server
            string fullPath = Path.Combine(Server.MapPath("~/temp"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", file);
        }

        /// <summary>
        /// Método para geração da imagem Captcha
        /// </summary>
        /// <param name="prefix">Prefixo para identificação</param>
        /// <param name="noisy">Flag para ativação da geração utilizando "ruído" na imagem</param>
        /// <returns></returns>
        public async Task<ActionResult> CaptchaImage(string prefix, bool noisy = true) 
        { 
            var rand = new Random((int)DateTime.Now.Ticks); 
            //generate new question 
            int a = rand.Next(10, 99); 
            int b = rand.Next(0, 9); 
            var captcha = string.Format("{0} + {1} = ?", a, b); 
 
            //store answer 
            Session["Captcha" + prefix] = a + b; 
 
            //image stream 
            FileContentResult img = null; 
 
            using (var mem = new MemoryStream()) 
            using (var bmp = new Bitmap(130, 30)) 
            using (var gfx = Graphics.FromImage((Image)bmp)) 
            { 
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit; 
                gfx.SmoothingMode = SmoothingMode.AntiAlias; 
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height)); 
 
                //add noise 
                if (noisy) 
                { 
                    float i, r, x, y; 
                    var pen = new Pen(Color.Yellow); 
                    for (i = 1; i < 10; i++) 
                    { 
                        pen.Color = Color.FromArgb( 
                        (rand.Next(0, 255)), 
                        (rand.Next(0, 255)), 
                        (rand.Next(0, 255))); 
 
                        r = rand.Next(0, (130 / 3)); 
                        x = rand.Next(0, 130); 
                        y = rand.Next(0, 30); 
                        float v1 = x - r;
                        float v2 = y - r;
                        gfx.DrawEllipse(pen, v1,  v2, r, r);                         
                    } 
                } 
 
                //add question 
                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3); 
 
                //render as Jpeg 
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg); 
                img = this.File(mem.GetBuffer(), "image/Jpeg"); 
            } 
 
            return img; 
        }

        [FwkAuthorize(CustomCall = "/Fwk/SalvarChamada"), System.Web.Mvc.HttpPost]
        public async Task<ActionResult> SalvarChamada(Chamada Chamada, string CRUDAction = "", string RedirectTo = "", bool isPartial = false)
		{
            string viewName = "DevHelper/Fwk/SalvarChamada";
			ViewBag.isPartial = isPartial;
            if (CRUDAction.Equals("DELETE"))
            {
                Chamada.Action = Request["gridKey"].Replace("\"", "").Split('|')[0];
                Chamada.Controller = Request["gridKey"].Replace("\"", "").Split('|')[1];
            }
            Chamada chamada = this._contextoFwk.Chamadas.Find(new object[]
			{
				Chamada.Action,
				Chamada.Controller
			});
			if (!CRUDAction.Equals("SALVAR"))
			{
				base.ModelState.Clear();
			}
			System.Web.Mvc.ActionResult result;
			if (CRUDAction.Equals("SALVAR") || CRUDAction.Equals("DELETE"))
			{
				string arg = "Realizado";
				string retorno = string.Empty;
				if (!base.ModelState.IsValid)
				{
					ViewBag.Alert = "Ocorreram erros no preenchimento do formulário, por favor, revise e tente novamente.";
					return PartialView(viewName);
				}
				else
				{
					try
					{
						if (chamada == null)
						{
							this._contextoFwk.Chamadas.Add(Chamada);
						}
						else if (CRUDAction.Equals("SALVAR"))
						{
							this._contextoFwk.Entry<Chamada>(chamada).CurrentValues.SetValues(Chamada);
							arg = "Atualizado";
						}
						else if (CRUDAction.Equals("DELETE"))
						{
                            //string[] array = base.Request["Action;Controller"].Split(new char[]
                            //{
                            //    '|'
                            //});
                            //Chamada.Action = array[0];
                            //Chamada.Controller = array[1];
                            //chamada = this._contextoFwk.Chamadas.Find(new object[]
                            //{
                            //    Chamada.Action,
                            //    Chamada.Controller
                            //});
                            if(chamada != null)
							    this._contextoFwk.Chamadas.Remove(chamada);
							arg = "Removido";                            							
							this._contextoFwk.SaveChanges();
                            retorno = string.Format("Registro {0} com Sucesso.", arg);
                            ViewBag.Alert = retorno;
                            if (!RedirectTo.Contains("Alert"))
                                RedirectTo = (RedirectTo.Contains("?")) ? RedirectTo + "&Alert=" + retorno : RedirectTo + "?Alert=" + retorno;
							result = this.Redirect(RedirectTo);
							return result;
						}
                        //Commit
						this._contextoFwk.SaveChanges();
					}
					catch (Exception ex)
					{
						retorno = string.Format("Falha na Operação. Detalhes:{0}", ex.Message);
						ViewBag.Alert = retorno;
						result = base.PartialView(viewName);
						return result;
					}
					retorno = string.Format("Registro {0} com Sucesso.", arg);
					ViewBag.Alert = retorno;
					base.TempData["SessionModelChamadas" + Core.GetSetCTX(HttpContext)] = (from calls in this._contextoFwk.Chamadas
					select calls).ToList<Chamada>();
					if (!CRUDAction.Equals(string.Empty))
					{
						return JavaScript(retorno);
					}
					else
					{
						return PartialView(viewName);
					}
				}
			}
			else
			{
				base.ModelState.Clear();
            }

			if (chamada == null)
			{
				chamada = new Chamada();
			}
			
			return PartialView(viewName, chamada);
		}

        [FwkAuthorize(CustomCall = "/Fwk/SalvarParametro"), System.Web.Mvc.HttpPost]
        public async Task<ActionResult> SalvarParametro(FwkConfig FwkConfig, string CRUDAction = "", string RedirectTo = "", bool isPartial = false)
		{
			string viewName = "DevHelper/Fwk/SalvarParametro";
			ViewBag.isPartial = isPartial;
            if (CRUDAction.Equals("DELETE"))
            {
                FwkConfig.Codigo = int.Parse(Request["gridKey"].Replace("\"", ""));
            }
			FwkConfig fwkConfig = _contextoFwk.Configs.Find(new object[]
			{
				FwkConfig.Codigo
			});
			if (!CRUDAction.Equals("SALVAR"))
			{
				base.ModelState.Clear();
			}
			System.Web.Mvc.ActionResult result;
			if (CRUDAction.Equals("SALVAR"))
			{
				string arg = "Realizado";
				string arg2 = string.Empty;
				if (!base.ModelState.IsValid)
				{
					ViewBag.Alert= "Ocorreram erros no preenchimento do formulário, por favor, revise e tente novamente.";
					return PartialView(viewName);
				}
				else
				{
					try
					{
						if (fwkConfig == null || FwkConfig.Codigo == 0)
						{
							this._contextoFwk.Configs.Add(FwkConfig);
						}
						else
						{
							this._contextoFwk.Entry<FwkConfig>(fwkConfig).CurrentValues.SetValues(FwkConfig);
							arg = "Atualizado";
						}
						this._contextoFwk.SaveChanges();
					}
					catch (Exception ex)
					{
						arg2 = string.Format("Falha na Operação. Detalhes:{0}", ex.Message);
						ViewBag.Alert = arg2;
						result = base.PartialView(viewName);
						return result;
					}
					arg2 = string.Format("Registro {0} com Sucesso.", arg);
					ViewBag.Alert = arg2;
					base.TempData["SessionModelParametros" + Core.GetSetCTX(HttpContext)] = (from param in this._contextoFwk.Configs
					select param).ToList<FwkConfig>();
                    result = base.PartialView(viewName);
				}
			}
			else
			    base.ModelState.Clear();

			if (fwkConfig == null || fwkConfig.Codigo == 0)
			{
				fwkConfig = new FwkConfig();
			}

			return PartialView(viewName, fwkConfig);			
		}

        [FwkAuthorize(CustomCall = "/Fwk/DeleteParametro"), System.Web.Mvc.HttpPost]
        public async Task<ActionResult> DeleteParametro(FwkConfig FwkConfig, string CRUDAction = "", string RedirectTo = "", bool isPartial = false, string Tema = "", bool deleteAction = false)
        {
            //string[] array = base.Request["Codigo"].Split(new char[]
            string[] array = base.Request["gridKey"].Replace("\"", "").Split(new char[]
			{
				'|'
			});
            FwkConfig.Codigo = int.Parse(array[0]);
            FwkConfig fwkConfig = this._contextoFwk.Configs.Find(new object[]
			{
				FwkConfig.Codigo
			});
            string retorno = string.Empty;                
            if (CRUDAction.Equals("DELETE"))
            {
                string arg = "Realizado";
                try
                {
                    if (fwkConfig != null)
                    {                    
                        this._contextoFwk.Configs.Remove(fwkConfig);
                        arg = "Removido";
                    }
                    this._contextoFwk.SaveChanges();
                }
                catch (Exception ex)
                {
                    retorno = string.Format("Falha na Operação. Detalhes:{0}", ex.Message);
                    return JavaScript(retorno);
                }
                retorno = string.Format("Registro {0} com Sucesso.", arg);
                /*TempData["SessionModelParametros" + Core.GetSetCTX(HttpContext)] = (from param in this._contextoFwk.Configs
                                                                                           select param).ToList<FwkConfig>();*/
                //Adicionando o alerta de retorno
                ViewBag.Alert = retorno;
                if (!RedirectTo.Contains("Alert"))
                    RedirectTo = (RedirectTo.Contains("?")) ? RedirectTo + "&Alert=" + retorno : RedirectTo + "?Alert=" + retorno;
                //return Redirect(RedirectTo);
                return this.Redirect(RedirectTo);
            }

            return this.JavaScript(retorno);                        
        }

        [FwkAuthorize(CustomCall = "/Fwk/SalvarPerfilAutoriza"), System.Web.Mvc.HttpPost]
        public async Task<ActionResult> SalvarPerfilAutoriza(PerfilAutorizacao PerfilAutorizacao, string CRUDAction = "", string RedirectTo = "", bool isPartial = false, string Tema = "", bool deleteAction = false)
		{
			string viewName = "DevHelper/Fwk/SalvarPerfilAutoriza";
			ViewBag.isPartial = isPartial;
			ViewBag.deleteAction = deleteAction;
			ViewBag.Tema = Tema;
            if (CRUDAction.Equals("DELETE"))
            {
                PerfilAutorizacao.AutPerfil = Request["gridKey"].Replace("\"", "").Split('|')[0];
                PerfilAutorizacao.AutChamada = Request["gridKey"].Replace("\"", "").Split('|')[1];
            }
			PerfilAutorizacao perfilAutorizacao = _contextoFwk.PerfilAutorizacoes.Find(new object[]
			{
				PerfilAutorizacao.AutPerfil,
				PerfilAutorizacao.AutChamada
			});
			if (!CRUDAction.Equals("SALVAR"))
			{
				base.ModelState.Clear();
			}
            string retorno = string.Empty;				
			if (CRUDAction.Equals("SALVAR"))
			{
				string arg = "Realizado";
				if (!base.ModelState.IsValid)
				{
					ViewBag.Alert = "Ocorreram erros no preenchimento do formulário, por favor, revise e tente novamente.";
					return PartialView(viewName);
				}
				else
				{
					try
					{
						if (perfilAutorizacao == null)
						{
							this._contextoFwk.PerfilAutorizacoes.Add(PerfilAutorizacao);
						}
						else
						{
							this._contextoFwk.Entry<PerfilAutorizacao>(perfilAutorizacao).CurrentValues.SetValues(PerfilAutorizacao);
							arg = "Atualizado";
						}
						this._contextoFwk.SaveChanges();
					}
					catch (Exception ex)
					{
						retorno = string.Format("Falha na Operação. Detalhes:{0}", ex.Message);
						ViewBag.Alert = retorno;
						return PartialView(viewName);
					}
					retorno = string.Format("Registro {0} com Sucesso.", arg);
					ViewBag.Alert = retorno;
					base.TempData["SessionModelPerfilAutorizacoes" + Core.GetSetCTX(HttpContext)] = (from perAut in this._contextoFwk.PerfilAutorizacoes
					select perAut).ToList<PerfilAutorizacao>();
					return PartialView(viewName);
				}
			}
			else base.ModelState.Clear();
			
            if (perfilAutorizacao == null)
			{
				perfilAutorizacao = new PerfilAutorizacao();
			}
			
            return PartialView(viewName, perfilAutorizacao);						
		}

        [FwkAuthorize(CustomCall = "/Fwk/DeletePerfilAutoriza"), System.Web.Mvc.HttpPost]
        public async Task<ActionResult> DeletePerfilAutoriza(PerfilAutorizacao PerfilAutorizacao, string CRUDAction = "", string RedirectTo = "", bool isPartial = false, string Tema = "", bool deleteAction = false)
        {
            //string[] array = base.Request["AutPerfil;AutChamada"].Split(new char[]
            string[] array = base.Request["gridKey"].Replace("\"", "").Split(new char[]
			{
				'|'
			});
            PerfilAutorizacao.AutPerfil = array[0];
            PerfilAutorizacao.AutChamada = array[1];
            PerfilAutorizacao perfilAutorizacao = this._contextoFwk.PerfilAutorizacoes.Find(new object[]
			{
				PerfilAutorizacao.AutPerfil,
				PerfilAutorizacao.AutChamada
			});
            string retorno = string.Empty;                
            if (CRUDAction.Equals("DELETE"))
            {
                string arg = "Realizado";
                try
                {
                    if (perfilAutorizacao != null)
                    {
                        this._contextoFwk.PerfilAutorizacoes.Remove(perfilAutorizacao);
                        arg = "Removido";
                    }
                    this._contextoFwk.SaveChanges();
                }
                catch (Exception ex)
                {
                    retorno = string.Format("Falha na Operação. Detalhes:{0}", ex.Message);
                    return JavaScript(retorno);
                }
                retorno = string.Format("Registro {0} com Sucesso.", arg);
                //base.TempData["SessionModelPerfilAutorizacoes" + Core.GetSetCTX(HttpContext)] = (from perAut in this._contextoFwk.PerfilAutorizacoes
                //                                                                                  select perAut).ToList<PerfilAutorizacao>();                
                ViewBag.Alert = retorno;
                if (!RedirectTo.Contains("Alert"))
                    RedirectTo = (RedirectTo.Contains("?")) ? RedirectTo + "&Alert=" + retorno : RedirectTo + "?Alert=" + retorno;
                return Redirect(RedirectTo);
            }
            
            return this.JavaScript(retorno);                        
        }

        [FwkAuthorize(CustomCall = "/Fwk/SalvarUsuarioAutoriza"), System.Web.Mvc.HttpPost]
        public async Task<ActionResult> SalvarUsuarioAutoriza(UsuarioAutorizacao UsuarioAutorizacao, string CRUDAction = "", string RedirectTo = "", bool isPartial = false)
		{
            string viewName = "DevHelper/Fwk/SalvarUsuarioAutoriza";

			ViewBag.isPartial = isPartial;

            if (CRUDAction.Equals("DELETE"))
            {
                UsuarioAutorizacao.UsuAutUsuCodigo = int.Parse(Request["gridKey"].Replace("\"", "").Split('|')[0]);
                UsuarioAutorizacao.UsuAutChamada = Request["gridKey"].Replace("\"", "").Split('|')[1];
            }
			UsuarioAutorizacao usuarioAutorizacao = _contextoFwk.UsuarioAutorizacoes.Find(new object[]
			{
				UsuarioAutorizacao.UsuAutUsuCodigo,
				UsuarioAutorizacao.UsuAutChamada
			});
			if (!CRUDAction.Equals("SALVAR"))
			{
				base.ModelState.Clear();
			}
			
			if (CRUDAction.Equals("SALVAR"))
			{
				string arg = "Realizado";
				string arg2 = string.Empty;
				if (!base.ModelState.IsValid)
				{
					ViewBag.Alert = "Ocorreram erros no preenchimento do formulário, por favor, revise e tente novamente.";
					return PartialView(viewName);
				}
				else
				{
					try
					{
						if (usuarioAutorizacao == null)
						{
							this._contextoFwk.UsuarioAutorizacoes.Add(UsuarioAutorizacao);
						}
						else
						{
							_contextoFwk.Entry<UsuarioAutorizacao>(usuarioAutorizacao).CurrentValues.SetValues(UsuarioAutorizacao);
							arg = "Atualizado";
						}
						this._contextoFwk.SaveChanges();
					}
					catch (Exception ex)
					{
						arg2 = string.Format("Falha na Operação. Detalhes:{0}", ex.Message);
						ViewBag.Alert = arg2;
						return PartialView(viewName);
					}
					arg2 = string.Format("Registro {0} com Sucesso.", arg);
					ViewBag.Alert = arg2;
					base.TempData["SessionModelUsuarioAutorizacoes" + Core.GetSetCTX(HttpContext)] = (from usuAut in this._contextoFwk.UsuarioAutorizacoes
					select usuAut).ToList<UsuarioAutorizacao>();
					return PartialView(viewName);
				}
			}
			else
			    base.ModelState.Clear();
			
            if (usuarioAutorizacao == null)
			{
				usuarioAutorizacao = new UsuarioAutorizacao();
			}
			
            return PartialView(viewName, usuarioAutorizacao);
		}

        [FwkAuthorize(CustomCall = "/Fwk/DeleteUsuarioAutoriza"), System.Web.Mvc.HttpPost]
        public async Task<ActionResult> DeleteUsuarioAutoriza(UsuarioAutorizacao UsuarioAutorizacao, string CRUDAction = "", string RedirectTo = "", bool isPartial = false, string Tema = "", bool deleteAction = false)
        {
            //string[] array = base.Request["UsuAutUsuCodigo;UsuAutChamada"].Split(new char[]
            string[] array = base.Request["gridKey"].Replace("\"", "").Split(new char[]
			{
				'|'
			});
            UsuarioAutorizacao.UsuAutUsuCodigo = int.Parse(array[0]);
            UsuarioAutorizacao.UsuAutChamada = array[1];
            UsuarioAutorizacao usuarioAutorizacao = this._contextoFwk.UsuarioAutorizacoes.Find(new object[]
			{
				UsuarioAutorizacao.UsuAutUsuCodigo,
				UsuarioAutorizacao.UsuAutChamada
			});
            
            if (CRUDAction.Equals("DELETE"))
            {
                string arg = "Realizado";
                string script = string.Empty;
                try
                {
                    if (usuarioAutorizacao != null)
                    {
                        this._contextoFwk.UsuarioAutorizacoes.Remove(usuarioAutorizacao);
                        arg = "Removido";
                    }
                    this._contextoFwk.SaveChanges();
                }
                catch (Exception ex)
                {
                    script = string.Format("Falha na Operação. Detalhes:{0}", ex.Message);
                    return JavaScript(script);
                }
                script = string.Format("Registro {0} com Sucesso.", arg);
                base.TempData["SessionModelUsuarioAutorizacoes" + Core.GetSetCTX(HttpContext)] = (from usuAut in this._contextoFwk.UsuarioAutorizacoes
                                                                                                    select usuAut).ToList<UsuarioAutorizacao>();
                return Redirect(RedirectTo);
            }
            
            return this.JavaScript("");
        }

        /// <summary>
        /// Método para exportar dados (Em desenvolvimento - Utilizando o padrão do módulo desenvolvedor de E-Transparência)
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="module"></param>
        /// <param name="gridKey"></param>
        /// <param name="ctxField"></param>
        /// <param name="filters"></param>
        /// <param name="Titulo"></param>
        /// <param name="fields"></param>
        /// <param name="returnType"></param>
        /// <param name="tamPagina"></param>
        /// <param name="ColunasOcultas"></param>
        /// <param name="Totalizadores"></param>
        /// <param name="Agrupadores"></param>
        /// <param name="Expanded"></param>
        /// <param name="ExibeExportador"></param>
        /// <param name="ExibeAgrupador"></param>
        /// <param name="ExibeLinhaFiltro"></param>
        /// <param name="Info"></param>
        /// <param name="DetailAction"></param>
        /// <param name="ExportHeaderText"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ExportData(string contexto, string module, string gridKey, string ctxField, string filters, string Titulo = "", string fields = "", string returnType = "grid", int tamPagina = 30, string ColunasOcultas = "", string Totalizadores = "", string Agrupadores = "", bool Expanded = true, bool ExibeExportador = true, bool ExibeAgrupador = false, bool ExibeLinhaFiltro = false, string Info = "", string DetailAction = "", string ExportHeaderText = "")
        {
            string viewFile = "DevHelper/DataExport";

            string sqlCode = string.Empty;
            string ultAtualizacao = string.Empty;
            List<string> filterFields = string.IsNullOrEmpty(filters) ? null : filters.Split(';').ToList<string>();
            List<string> selectFields = string.IsNullOrEmpty(fields) ? null : fields.Split(';').ToList<string>();
            var model = ExportModel.getData(out sqlCode, gridKey, ctxField, contexto, selectFields, module, filterFields, Request, null);
            ViewBag.returnType = returnType;
            if (returnType.Equals("json"))
            {
                //return Json(Core.DataViewToJSON((DataView)model), JsonRequestBehavior.AllowGet);
                //TODO: Testar FWK Json 
                return new Fwk_JsonResult(model);
            }
            else
            {
                ViewData["gridTitulo_" + module] = Titulo;
                ViewData["gridKey_" + module] = gridKey;
                ViewData["sqlCode_" + module] = sqlCode;
                ViewData["tamPagina_" + ViewBag.module] = tamPagina;
                ViewData["ColunasOcultas_" + ViewBag.module] = ColunasOcultas;
                ViewData["Totalizadores_" + ViewBag.module] = Totalizadores;
                ViewData["Agrupadores_" + ViewBag.module] = Agrupadores;
                ViewData["Expanded_" + ViewBag.module] = Expanded;
                ViewData["ExibeExportador_" + ViewBag.module] = ExibeExportador;
                ViewData["ExibeAgrupador_" + ViewBag.module] = ExibeAgrupador;
                ViewData["ExibeLinhaFiltro_" + ViewBag.module] = ExibeLinhaFiltro;
                ViewData["DetailAction_" + ViewBag.module] = DetailAction;
                ViewData["Info_" + ViewBag.module] = Info;
                ViewData["ExportHeaderText_" + ViewBag.module] = ExportHeaderText;
                return PartialView(viewFile, model);
            }
        }

        /// <summary>
        /// Método para exportação genérica de dados (Em desenvolvimento)
        /// </summary>
        /// <param name="ctxCode"></param>
        /// <param name="module"></param>
        /// <param name="filters"></param>
        /// <param name="fields"></param>
        /// <param name="advSelect"></param>
        /// <param name="Titulo"></param>
        /// <param name="returnType"></param>
        /// <param name="tamPagina"></param>
        /// <param name="ColunasOcultas"></param>
        /// <param name="ColunasLinks"></param>
        /// <param name="Totalizadores"></param>
        /// <param name="Agrupadores"></param>
        /// <param name="Expanded"></param>
        /// <param name="ExibeExportador"></param>
        /// <param name="ExibeAgrupador"></param>
        /// <param name="ExibeLinhaFiltro"></param>
        /// <param name="Info"></param>
        /// <param name="DetailAction"></param>
        /// <param name="ColunasActions"></param>
        /// <param name="ExportHeaderText"></param>
        /// <param name="DistinctValues"></param>
        /// <param name="showUrl"></param>
        /// <param name="showAutoForm"></param>
        /// <param name="DXScript"></param>
        /// <returns></returns>
        public async Task<ActionResult> Data(string ctxCode, string module, string[] filters, string[] fields, string advSelect = "", string Titulo = "", string returnType = "grid", int tamPagina = 30, string ColunasOcultas = "", string ColunasLinks = "", string Totalizadores = "", string Agrupadores = "", bool Expanded = true, bool ExibeExportador = true, bool ExibeAgrupador = true, bool ExibeLinhaFiltro = false, string Info = "", string DetailAction = "", string ColunasActions = "", string ExportHeaderText = "", bool DistinctValues = true, bool showUrl = false, bool showAutoForm = false, string DXScript = "")
        {
            string viewFile = "DevHelper/DataExport";
            ViewBag.Titulo = module;

            IEnumerable EntidadeInfo = null;
            //ecode = ecode == "" ? e : ecode;
            if (string.IsNullOrEmpty(ctxCode))
            {
                return JavaScript("Aplicação sem contexto definido");
            }

            string gridKey = string.Empty;
            //string ctxField = string.Empty;
            string sqlCode = string.Empty;
            IEnumerable model = new DataView();

            #region Parser para campos/filtros passados com o separador ';' e para valores nulos
            //Adiciona a seleção avançada
            if (!string.IsNullOrEmpty(advSelect))
            {
                if (fields != null)
                    fields[0] += ";" + advSelect;
                else //Instancia o objeto fields com o valor do 'advSelect'
                    fields = new string[] { advSelect };
            }
            if (fields != null && fields[0].Contains(';'))
            {
                fields = fields[0].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (filters != null && filters[0].Contains(';'))
            {
                filters = filters[0].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            //Correção para parametrização vazia
            if (fields != null && fields.Length == 1 && string.IsNullOrEmpty(fields[0]))
                fields = null;
            if (filters != null && filters.Length == 1 && string.IsNullOrEmpty(filters[0]))
                filters = null;
            #endregion

            ViewBag.module = module;
            //List<string> filterFields = string.IsNullOrEmpty(fields) ? null : fields.Split(';').Select(valor => valor.Trim()).ToList<string>();
            //List<string> selectFields = string.IsNullOrEmpty(fields) /*|| returnType.Equals("grid")*/ ? null : fields.Split(';').Select(valor => valor.Trim()).ToList<string>();
            List<string> filterFields = filters != null && filters.Length > 0 ? filters.ToList<string>() : null;
            List<string> selectFields = fields != null && fields.Length > 0 ? fields.ToList<string>() : null;
            //TODO: Implementar a seperação da seleção entre os campos de Select e Filter

            //Instanciação do objeto ModuleModel (Reflection)   
            object instancia = null;
            string[] parametros = null;

            try
            {
                var classe = Assembly.GetExecutingAssembly().GetTypes().First(x => x.Name.ToLower() == module.ToLower());
                instancia = Activator.CreateInstance(classe);
                //((ModuleModel)instancia).Ecode = ecode;
                //((ModuleModel)instancia).Configure();
                gridKey = ((FwkModule)instancia).GridKey;
                #region Adicionando GridKey e ColunasOcultas - Campos obrigatórios para a exibição do tipo 'grid'
                if (returnType.Equals("grid") && selectFields != null && selectFields.Count > 0)
                {
                    if (!selectFields.Contains(gridKey.Trim()) && !selectFields.Contains("\"" + gridKey.Trim() + "\""))
                        selectFields.Add("\"" + gridKey.Trim() + "\"");
                    //Adicionando Colunas Ocultas, caso ausentes da seleção
                    foreach (var item in ColunasOcultas.Split(';'))
                    {
                        if (string.IsNullOrEmpty(item.Trim()))
                            continue;
                        if (!selectFields.Contains(item.Trim()) && !selectFields.Contains("\"" + item.Trim() + "\""))
                            selectFields.Add("\"" + item.Trim() + "\"");
                    }
                }
                #endregion
                #region Adiciona Parâmetros Obrigatórios
                if (((FwkModule)instancia).Parametros != null)
                {
                    parametros = ((FwkModule)instancia).Parametros.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < parametros.Length; i++)
                    {
                        if (filterFields == null)
                            filterFields = new List<string>();
                        filterFields.Add(parametros[i]);
                        if (Request[parametros[i]] == null || Request[parametros[i]] == "")
                            return JavaScript("Falha no carregamento das configurações do módulo (" + module + ") - Detalhes: Parâmetro obrigatório '" + parametros[i] + "' não informado.");
                        //Adicionando o valor ao parâmetro
                        parametros[i] += "|" + Request[parametros[i]];
                    }
                }
                #endregion
                //bool distinctValues = (returnType.Equals("json")) ? true : false;
                //Chamada Default: Parametrização padrão
                model = ((FwkModule)instancia).getData(out sqlCode, selectFields, filterFields, Request, ((FwkModule)instancia).Tabela_Descritor, 30, 0, null, true, DistinctValues);
                ColunasOcultas = string.IsNullOrEmpty(ColunasOcultas) ? ((FwkModule)instancia).Colunas_Ocultas : ColunasOcultas;
                Totalizadores = string.IsNullOrEmpty(Totalizadores) ? ((FwkModule)instancia).Totalizadores : Totalizadores;
                ColunasLinks = string.IsNullOrEmpty(ColunasLinks) ? ((FwkModule)instancia).Colunas_Links : ColunasLinks;
                Info = string.IsNullOrEmpty(Info) ? ((FwkModule)instancia).Info : Info;
                DetailAction = ((FwkModule)instancia).DetailAction;
                ColunasActions = ((FwkModule)instancia).Colunas_Actions;
            }
            catch (Exception ex) { return JavaScript("Falha no carregamento das configurações do módulo (" + module + ") - Detalhes: " + ex.Message); }

            ViewBag.Tema = FwkConfig.GetSettingValue("TEMA", ctxCode);
            ExportHeaderText = ((DataView)EntidadeInfo).ToTable().Rows[0]["NOME"].ToString();
            //Gera a URL de Integração
            ViewBag.showUrl = showUrl;
            //Gera formulário modelo para a filtragem
            ViewBag.showAutoForm = showAutoForm;
            ViewBag.DistinctValues = DistinctValues;
            //ViewBag.Campos = ((ModuleModel)instancia).Tabela_Fields_List;
            ViewBag.Fields = fields == null ? ((FwkModule)instancia).Tabela_Fields_Array : fields;
            ViewBag.Filters = filters;
            //Passando Parâmetros Obrigatórios
            ViewBag.Parametros = parametros;
            //Filtros de período padrão
            
            if (showUrl)
            {
                //ViewBag.URL_Integra = HttpContext.Request.Url.AbsoluteUri;                
                //Desabilita a exibição da Url de Integração e do auto-formulário de filtragem
                //string PathAndQuery = HttpContext.Request.Url.PathAndQuery.Replace("showUrl=True", "showUrl=False").Replace("showAutoForm=True", "showAutoForm=False");
                //ViewBag.URL_Integra = FwkConfig.GetSettingValue("pathIIS", Core.GetSetCTX(HttpContext)) + PathAndQuery;
                string PathAndQuery = HttpContext.Request.Url.PathAndQuery.Replace("showUrl=True", "showUrl=False").Replace("showAutoForm=True", "showAutoForm=False");
                Uri url = new Uri(FwkConfig.GetSettingValue("pathApp"));
                string SchemeAndHost = url.Scheme + "://" + url.Authority;
                ViewBag.URL_Integra = SchemeAndHost + PathAndQuery;
            }
            //Retorno            
            ViewBag.returnType = string.IsNullOrEmpty(returnType) ? "grid" : returnType;
            //Parametrização do GridView (DevHelper)
            ViewData["gridTitulo_" + module] = Titulo;
            ViewData["gridKey_" + module] = gridKey;
            ViewData["dtSource_" + module] = model;
            //ViewData["sqlCode_" + module] = sqlCode;
            ViewData["tamPagina_" + ViewBag.module] = tamPagina;
            ViewData["ColunasOcultas_" + ViewBag.module] = ColunasOcultas;
            ViewData["Totalizadores_" + ViewBag.module] = Totalizadores;
            ViewData["Agrupadores_" + ViewBag.module] = Agrupadores;
            ViewData["ColunasLinks_" + ViewBag.module] = ColunasLinks;
            ViewData["Expanded_" + ViewBag.module] = Expanded;
            ViewData["ExibeExportador_" + ViewBag.module] = ExibeExportador;
            ExibeAgrupador = string.IsNullOrEmpty(Agrupadores) ? false : ExibeAgrupador;
            ViewData["ExibeAgrupador_" + ViewBag.module] = ExibeAgrupador;
            ViewData["ExibeLinhaFiltro_" + ViewBag.module] = ExibeLinhaFiltro;
            ViewData["DetailAction_" + ViewBag.module] = DetailAction;
            ViewData["ColunasActions_" + ViewBag.module] = ColunasActions;
            ViewData["Info_" + ViewBag.module] = Info;
            ViewData["ExportHeaderText_" + ViewBag.module] = ExportHeaderText;

            if (ViewBag.returnType.Equals("json"))
            {
                if (showUrl)
                {
                    //Convertendo o model para o formato json (serializando)
                    string jsonResult = Core.DataViewToJSON((DataView)model, "Default", module).ToString();
                    return PartialView(viewFile, jsonResult);
                }
                else
                {
                    return new Fwk_JsonResult(((DataView)model).ToTable(module));
                    //return Json(CoreFunctions.DataViewToJSON((DataView)model), JsonRequestBehavior.AllowGet);
                }
            }
            else if (ViewBag.returnType.Equals("xml"))
            {
                if (showUrl)
                {
                    //Convertendo o model para o formato json (serializando)
                    string xmlResult = Core.DataViewToXML((DataView)model, module).ToString();
                    return PartialView(viewFile, xmlResult);
                }
                else
                {
                    return new Fwk_XmlResult(((DataView)model).ToTable(module));
                    //return Json(CoreFunctions.DataViewToJSON((DataView)model), JsonRequestBehavior.AllowGet);
                }
            }
            else if (ViewBag.returnType.Equals("grid"))
            {
                return PartialView(viewFile, model);
            }
            else { return JavaScript("Tipo de Retorno não implementado"); }
        }
        
        //TODO: Implementar modelo com reflexão de objetos para montar o formulário
        /// <summary>
        /// Método para geração de CRUD automático para as entidades mapeadas no sistema (Em desenvolvimento)
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Entity"></param>
        /// <param name="gridName"></param>
        /// <param name="CRUDAction"></param>
        /// <param name="RedirectTo"></param>
        /// <param name="isPartial"></param>
        /// <returns></returns>
        [FwkAuthorize(CustomCall = "/Fwk/SalvarGenerico"), System.Web.Mvc.HttpPost]
        public async Task<ActionResult> SalvarGenerico(object Model, System.Data.Entity.DbSet Entity, string gridName, string CRUDAction = "", string RedirectTo = "", bool isPartial = false)
        {
            string viewName = "DevHelper/Fwk/SalvarGenerico";

            ViewBag.isPartial = isPartial;

            var objType = Model.GetType();

            var encontrado = Entity.Find(new object[]
			{
				//GetKeys values from model
			});
            if (!CRUDAction.Equals("SALVAR"))
            {
                base.ModelState.Clear();
            }

            if (CRUDAction.Equals("SALVAR"))
            {
                string operation = "Realizado";
                string strResult = string.Empty;
                if (!base.ModelState.IsValid)
                {
                    ViewBag.Alert = "Ocorreram erros no preenchimento do formulário, por favor, revise e tente novamente.";
                    return PartialView(viewName);
                }
                else
                {
                    try
                    {
                        if (encontrado == null)
                        {
                            Entity.Add(Model);
                        }
                        else
                        {
                            _contextoFwk.Entry(encontrado).CurrentValues.SetValues(Model);
                            operation = "Atualizado";
                        }
                        _contextoFwk.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        strResult = string.Format("Falha na Operação. Detalhes:{0}", ex.Message);
                        ViewBag.Alert = strResult;
                        return PartialView(viewName);
                    }
                    strResult = string.Format("Registro {0} com Sucesso.", operation);
                    ViewBag.Alert = strResult;
                    base.TempData["SessionModel" + gridName + Core.GetSetCTX(HttpContext)] = null; //(from obj in Entity select obj).ToList<object>();
                    return PartialView(viewName);
                }
            }
            else
                base.ModelState.Clear();

            if (encontrado == null)
            {
                //TODO: Instancia tipo do objeto do Model
                encontrado = new object();
            }

            return PartialView(viewName, encontrado);
        }

        /// <summary>
        /// Converte o conteúdo de uma View para String
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="context"></param>
        /// <param name="ViewData"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string RenderRazorViewToString(string viewName, ControllerContext context, ViewDataDictionary ViewData, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Método para geração de texto (string) no formato QRCode
        /// </summary>
        /// <param name="valor">Valor textual para ser codificado no qrcode</param>
        /// <param name="filename">Nome do arquivo de imagem (bmp) a ser gerado</param>
        /// <returns>Retorna um FileContentResult (image)</returns>
        public async Task<ActionResult> GetQRCodeFromString(string valor, string filename = "qrcode.bmp", bool createFile = true)
        {
            return Util.CreateQRCodeImage(valor, filename, createFile);
        }

        public async Task<ActionResult> SetSelectedIDs(string selectedIDs, string gridKey)
        {
            Dictionary<int, RouteValueDictionary> selectedIDsDic = Core.ParserGridRouteValueDictionary(selectedIDs, gridKey);
            Session["sessSelectedIDs"] = selectedIDsDic;
            TempData["selectedIDs"] = selectedIDsDic;
            return null;
        }        

        [HttpGet]
        public JsonResult GetCidadesByUF(string UF)
        {
            string strJson = Util.FileReaderToStr(FwkConfig.GetSettingValue("pathFisico") + "\\Content\\json\\UFCidades.json");
            List<FwkDominio> list = Core.GetObjectFromJSONString<List<FwkDominio>>(strJson);
            //List<FwkDominio> list = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<FwkDominio>>(strJson);
            list = list.Where(c => c.valor.Equals(UF)).ToList<FwkDominio>();
            ICollection<string> cidades = list.Count > 0 ? list[0].dominio_itens : new List<string>();
            return this.Json(new { Result = cidades }, JsonRequestBehavior.AllowGet);
        }
    }
}