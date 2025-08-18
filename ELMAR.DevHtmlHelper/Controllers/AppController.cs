using ELMAR.DevHtmlHelper.Models;
using ELMAR.DevHtmlHelper.Models.CustomValidation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ELMAR.DevHtmlHelper.Controllers
{
    public class AppController : Controller
    {
        private readonly FwkContexto _contextoFwk = new FwkContexto();

        /// <summary>
        /// Exibe a página 'Index' da aplicação, utiliza o view 'IndexApp'.
        /// Este view deve ser implementado e adicionado: /Views/Shared/IndexApp ou /Views/App/IndexApp
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="ctx"></param>
        /// <param name="isPartial"></param>
        /// <param name="Tema"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string Result, string ctx = "", bool isPartial = false, string Tema = "")
        {
            ViewData["Titulo"] = ConfigurationManager.AppSettings["appName"];
            ViewBag.isPartial = isPartial;
            //Define o contexto
            Result += SetContexto(ctx);
            ViewBag.HideInfo = true;
            ViewData["Msg"] = Result;
            //TODO: Carregar o tema a partir do ConfigurationManager FWK (Override)
            ViewBag.Tema = string.IsNullOrEmpty(Tema) ? FwkConfig.GetSettingValue("DefaultTheme", Core.GetSetCTX(HttpContext)) : Tema;                
            return View("DevHelper/App/IndexApp");
        }

        /// <summary>
        /// Retorna o título da aplicação via Content
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetTitulo(string ctx = "")
        {
            string titulo = ConfigurationManager.AppSettings["appName"];
            titulo += !string.IsNullOrEmpty(FwkConfig.GetSettingValue("TituloContexto", Session)) ? " - " + FwkConfig.GetSettingValue("TituloContexto", Session) : string.Empty;
            SetContexto(ctx);
            return Content(titulo);
        }

        /// <summary>
        /// Retorna a logo da aplicação via Content
        /// </summary>
        /// <param name="ctx">Contexto</param>
        /// <returns></returns>
        public async Task<ActionResult> GetLogo(string ctx = "")
        {
            string url = !string.IsNullOrEmpty(FwkConfig.GetSettingValue("LogoContexto", Session)) ? "<img src=\""+FwkConfig.GetSettingValue("LogoContexto", Session)+"\"/>" : string.Empty;
            SetContexto(ctx);
            return Content(url);
        }

        /// <summary>
        /// Exibe conteudos em frames, utilizado para integração entre aplicações e módulos.
        /// Utiliza layout externo ao do E-Fwk (Adicionar view: /Views/Shared/LinkViewerFrame ou /Views/App/LinkViewerFrame
        /// </summary>
        /// <param name="LinkURL">URL link para ser carregado no frame</param>
        /// <param name="Result">Mensagem de resultado</param>
        /// <param name="ctx">Contexto da aplicação</param>
        /// <returns></returns>
        public async Task<ActionResult> LinkViewerFrame(string LinkURL, string Result = "", string ctx = "", string Titulo = "", bool isPartial = false, string Tema = "")
        {
            string viewFile = "DevHelper/App/LinkViewerFrame";
            Result += SetContexto(ctx);
            //ViewBag.Link = LinkURL.Split('?')[0] + "?" + HttpUtility.ParseQueryString(LinkURL.Split('?')[1]).ToString();
            ViewBag.Link = LinkURL;
            ViewBag.Tema = string.IsNullOrEmpty(Tema) ? FwkConfig.GetSettingValue("DefaultTheme", Core.GetSetCTX(HttpContext)) : Tema;
            ViewBag.Titulo = Titulo;
            if (isPartial)
                return PartialView(viewFile);
            return View(viewFile);
        }
        
        //[FwkAuthorize]
        /// <summary>
        /// Exibe a página inicial da aplicação.
        /// Este view deve ser implementado e adicionado: /Views/Shared/IndexApp ou /Views/App/IndexApp
        /// </summary>
        /// <param name="Result">Recebe mensagem de resultado para exibição</param>
        /// <param name="ctx">Contexto</param>
        /// <param name="isPartial">Método de exibição</param>
        /// <param name="Tema">Tema dev</param>
        /// <returns></returns>
        public async Task<ActionResult> IndexApp(string Result, string ctx = "", bool isPartial = false, string Tema = "")
        {
            Result += Environment.NewLine;
            Result += SetContexto(ctx);
            ViewBag.ctx = ctx;
            ViewBag.isPartial = isPartial;

            Session["PERFIL"] = Session["PERFIL"] == null ? "USER" : Session["PERFIL"];
            ViewData["Titulo"] = ConfigurationManager.AppSettings["appName"];
            ViewBag.Tema = string.IsNullOrEmpty(Tema) ? FwkConfig.GetSettingValue("DefaultTheme", Core.GetSetCTX(HttpContext)) : Tema;                

            ViewData["Msg"] = Result;
            return View();
        }

        /// <summary>
        /// Método para chamar o formulário de login da aplicação.
        /// Não possui view interna, utiliza o template /Shared/Login ou /App/Login da aplicação atual
        /// Ação: @Html.Action("Login", "Usuario", new { ViewFile = ViewBag.ViewFile, Tema = ViewBag.Tema, RedirectTo = ViewBag.RedirectTo, isPartial = ViewBag.isPartial })
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="ViewFile"></param>
        /// <param name="RedirectTo"></param>
        /// <param name="Perfil"></param>
        /// <param name="ctx"></param>
        /// <param name="isPartial"></param>
        /// <param name="Tema"></param>
        /// <returns></returns>
        public virtual ActionResult Login(string Login = "", string Email = "", string Perfil = "", string Result = "", string RedirectTo = "", bool ReenviaSenha = true, string ViewFile = "Login", string ClientID = "", string Codigo = "", string UITarget = "", bool PreLogin = false, string Tema = "", bool isPartial = false)
        {
            /*ViewData["Titulo"] = ConfigurationManager.AppSettings["appName"].ToString();
            ViewData["Msg"] = Result;
            ViewBag.ViewFile = ViewFile;
            ViewBag.isPartial = isPartial;
            ViewBag.RedirectTo = RedirectTo;
            Result += setContexto(ctx);
            ViewBag.HideInfo = (Request["PERFIL"] == null || !Request["PERFIL"].Equals("USER")) ? true : false;
            ViewData["Tema"] = string.IsNullOrEmpty(Tema) ? FwkConfig.GetSettingValue("DefaultTheme", Core.GetSetCTX(HttpContext)) : Tema;*/
            ViewBag.ViewFile = ViewFile;
            ViewData["Titulo"] = ConfigurationManager.AppSettings["appName"];
            ViewData["Acao"] = "SALVAR";
            ViewBag.Perfil = Perfil;
            ViewBag.Login = Login;
            ViewBag.Codigo = Codigo;
            ViewBag.Email = Email;
            ViewBag.Msg = Result;
            ViewBag.RedirectTo = RedirectTo;
            ViewBag.ReenviaSenha = ReenviaSenha;
            ViewBag.Tema = string.IsNullOrEmpty(Tema) ? FwkConfig.GetSettingValue("DefaultTheme", Core.GetSetCTX(HttpContext)) : Tema; 
            ViewBag.isPartial = isPartial;
            if (string.IsNullOrEmpty(Login))
                ViewBag.ReenviaSenha = false;
            ViewBag.PreLogin = PreLogin;
            return View();
        }

        /// <summary>
        /// Método para exibir o formulário de envio de mensagens. 
        /// Não possui template padrão, encapsular via chamada de template da aplicação para herdar o estilo do layout padrão.
        /// Criar a view em: /Views/Shared/EnviaMensagem ou /Views/App/EnviaMensagem 
        /// Ação: @Html.Action("MailSender", "Fwk", new { RedirectTo = ViewBag.RedirectTo, Tema = ViewBag.Tema } )
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="isPartial"></param>
        /// <returns></returns>
        [FwkAuthorize]
        public async Task<ActionResult> EnviaMensagem(string Result = "", bool isPartial = false, string RedirectTo = "")
        {
            ViewBag.Tema = FwkConfig.GetSettingValue("DefaultTheme", Core.GetSetCTX(HttpContext));
            ViewBag.isPartial = isPartial;
            ViewBag.Msg = Result;
            ViewBag.RedirectTo = string.IsNullOrEmpty(RedirectTo) ? FwkConfig.GetSettingValue("pathApp", Session) + HttpContext.Request.Url.AbsolutePath : RedirectTo;
            return View();
        }

        /// <summary>
        /// Método para forçar a definição de um contexto de execução para a aplicação
        /// </summary>
        /// <param name="ctx">Contexto</param>
        /// <returns></returns>
        public string SetContexto(string ctx)
        {
            string Result = string.Empty;
            Core.GetSetCTX(HttpContext, ctx);
            if (Core.GetSetCTX(HttpContext) == null)
                Core.GetSetCTX(HttpContext, ConfigurationManager.AppSettings["defaultCTX"]);
            if (Core.GetSetCTX(HttpContext) == null)
                Result = "Contexto da aplicação não definido." + Environment.NewLine;
            return Result;
        }

        /// <summary>
        /// Verificar de serviços do windows, utilizar o comando 'sc query'
        /// </summary>
        /// <param name="ip">Ip da máquina par aa conexão SSH</param>
        /// <param name="servico">Nome do serviço</param>
        /// <param name="user">Usuário para o acesso SSH</param>
        /// <param name="pass">Senha para o acesso SSH</param>
        /// <returns></returns>
        public async Task<ActionResult> VerificaServicoStatus(string ip, string servico, string user="dviana", string pass=@"dv#1982@")
        {
            string result = "";
            if (Core.ExecutarCmdSSH(out result, ip, user, pass, "sc query " + servico, 22))
            {
                if (result.Contains("RUNNING"))
                    result = "<span style=\"color:blue\">" + servico.ToUpper() + " => OK</span>";
                else
                    result = "<span style=\"color:red\">" + servico.ToUpper() + " => PARADO</span>";
            }
            else {
                return Content("<span style=\"color:red\">Erro na execução do comando SSH: " + result + "</span>");
            }
            return Content(result);
        }

        /// <summary>
        /// Exibe as informações de versionamento do E-Fwkweb
        /// </summary>
        /// <param name="versaoAPP">Informar para adicionar a informação de versionamento da aplicação</param>
        /// <param name="Tema">Tema Dev</param>
        /// <returns></returns>
        public async Task<ActionResult> About(string versaoAPP = "", string Tema = "", bool isPartial = true)
        {
            ViewBag.Titulo = "Sobre";            
            ViewBag.Mensagem = string.Empty;
            ViewBag.Tema = string.IsNullOrEmpty(Tema) ? FwkConfig.GetSettingValue("DefaultTheme", Core.GetSetCTX(HttpContext)) : Tema;                
            ViewBag.VersaoAPP = versaoAPP;
            ViewBag.VersaoFWK = typeof(ELMAR.DevHtmlHelper.Controllers.DevHelperController).Assembly.GetName().Version.ToString();
            ViewBag.lastUpdate = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime.ToShortDateString();
            //var NotasVersao = ((NotasVersao)_contexto.NotasVersao.Find(ViewBag.Versao));
            //ViewBag.NotasVersao = NotasVersao != null ? NotasVersao.Notas : string.Empty;
            if(isPartial)
                return PartialView();
            return View();
        }

        /// <summary>
        /// Método para testes do WSRest
        /// </summary>
        /// <param name="value">Valor</param>
        /// <param name="ctx">Contexto da aplicação</param>
        /// <param name="action">Ação: GET / POST / DELETE</param>
        /// <param name="url">URL do serviço</param>
        /// <returns></returns>
        [FwkAuthorize]
        public async Task<ActionResult> WSRestTest(string value, string ctx = "", string action = "GET", string url = "")
        {
            ViewData["Tema"] = FwkConfig.GetSettingValue("DefaultTheme", Core.GetSetCTX(HttpContext));
            url = string.IsNullOrEmpty(url) ? FwkConfig.GetSettingValue("pathApp", Session) + "/api/sms" : url;
            string result = string.Empty;
            if (value != null)
            {
                result = await Core.HttpRequestRest(url, value, action);
                return JavaScript(result);
            }
            //Exibe o formulário
            ViewBag.Url = url;
            return PartialView("DevHelper/App/WSRestTest", result);
        }

        public async Task<ActionResult> ConvertToSHA512(object file = null)
        {
            if (file is System.Web.HttpPostedFileBase[])
            {
                byte[] data;
                using (Stream inputStream = ((System.Web.HttpPostedFileBase[])file)[0].InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        await inputStream.CopyToAsync(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
                string content = Core.SHA512(data);
                return JavaScript(content);
            }
            else
            {
                return PartialView("DevHelper/App/ConvertToSHA512");
            }
        }

        public ActionResult SitemapXml(string MenuCSV = "MenuInscricaoDefault")
        {
            Fwk_MenuItem menuHelper = new Fwk_MenuItem();
            var sitemapNodes = menuHelper.GetSitemapNodes(this.Url, Session, System.Configuration.ConfigurationManager.AppSettings["pathFisico"] + "/Content/menus/" + MenuCSV + ".csv");
            string xml = menuHelper.GetSitemapDocument(sitemapNodes);
            return this.Content(xml, System.Net.Mime.MediaTypeNames.Text.Xml, Encoding.UTF8);
        }

        public ActionResult SessionSitemapXml(StringReader ItemsCSV)
        {
            Fwk_MenuItem menuHelper = new Fwk_MenuItem();
            ItemsCSV = ItemsCSV == null ? (StringReader)TempData["sessionMenu"] : ItemsCSV;
            var sitemapNodes = menuHelper.GetSitemapNodes(this.Url, Session, Fwk_MenuItem.CarregaMenusCSV(ItemsCSV));
            string xml = menuHelper.GetSitemapDocument(sitemapNodes);
            return this.Content(xml, System.Net.Mime.MediaTypeNames.Text.Xml, Encoding.UTF8);
        }

        public ActionResult ShowMetaTags(List<Fwk_MenuItem> Items, string Description = "", string Abstract = "")
        {
            Fwk_MenuItem menuHelper = new Fwk_MenuItem();
            var sitemapNodes = menuHelper.GetSitemapNodes(this.Url, Session, Items);
            StringBuilder metaTag = new StringBuilder();
            if (!string.IsNullOrEmpty(Description))
            {
                metaTag.AppendLine("<META NAME=\"DESCRIPTION\" CONTENT=\"" + Description + "\">");
            }
            if (!string.IsNullOrEmpty(Abstract))
            {
                metaTag.AppendLine("<META NAME=\"ABSTRACT\" CONTENT=\"" + Abstract + "\">");
            }
            string keywords = string.Empty;
            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                if (sitemapNode.Titulo == null)
                    continue;
                //Condição de saída especial do laço, limite de 255 caracteres
                if (keywords.Length + sitemapNode.Titulo.Length >= 256)
                    break; 
                keywords += sitemapNode.Titulo.Replace(".", " ") + ",";
            }
            if (keywords.Length > 0)
            {
                keywords = keywords.Substring(0, keywords.Length - 1);
                metaTag.AppendLine("<META NAME=\"KEYWORDS\" CONTENT=\"" + keywords + "\">");
            }
            return this.Content(metaTag.ToString().ToLower());
        }
        
        public ActionResult TestBrowserCapabilities(bool ShowBrowserInfo = false)
        {
            string alerta = string.Empty;
            System.Web.HttpBrowserCapabilitiesBase browser = Request.Browser;

            string s = "Browser Capabilities\n"
                + "Type = " + browser.Type + "\n"
                + "Name = " + browser.Browser + "\n"
                + "Version = " + browser.Version + "\n"
                + "Major Version = " + browser.MajorVersion + "\n"
                + "Minor Version = " + browser.MinorVersion + "\n"
                + "Platform = " + browser.Platform + "\n"
                + "Is Beta = " + browser.Beta + "\n"
                + "Is Crawler = " + browser.Crawler + "\n"
                + "Is AOL = " + browser.AOL + "\n"
                + "Is Win16 = " + browser.Win16 + "\n"
                + "Is Win32 = " + browser.Win32 + "\n"
                + "Supports Frames = " + browser.Frames + "\n"
                + "Supports Tables = " + browser.Tables + "\n"
                + "Supports Cookies = " + browser.Cookies + "\n"
                + "Supports VBScript = " + browser.VBScript + "\n"
                + "Supports JavaScript = " +
                    browser.EcmaScriptVersion.ToString() + "\n"
                + "Supports Java Applets = " + browser.JavaApplets + "\n"
                + "Supports ActiveX Controls = " + browser.ActiveXControls
                      + "\n";

            //Testa os pré-requisitos necessários para o browser
            if (browser.Type.Contains("CHROME") 
                || browser.Type.Contains("OPERA") 
                || browser.Type.Contains("EDGE") 
                || browser.Type.Contains("FIREFOX")) // replace with your check
            {
                if (browser.MajorVersion < 70)
                    alerta = "<span style=\"color:#ff0000\">Navegador compatível, mas desatualizado. " + browser.Browser + " Versão: " + browser.Version + "</span>";
            }
            else if (browser.Type.ToUpper().Contains("INTERNETEXPLORER")) // replace with your check
            {
                alerta = "<span style=\"color:#ff0000\">Este navegador não é compatível com essa aplicação. Compatíveis: Chrome, Firefox, Opera, Safari e Edge.</span>";
            }

            if (!browser.Cookies) { alerta += "<i>O browser não possui suporte a cookies, algumas funcionalidades poderão ficar instáveis.</i>"; }

            if (ShowBrowserInfo)
                alerta += "<br /><hr />"+s;

            return Content(alerta);
        }
    }
}