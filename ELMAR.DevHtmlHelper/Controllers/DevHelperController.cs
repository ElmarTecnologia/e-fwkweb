using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.XtraReports.UI;
using ELMAR.DevHtmlHelper.Models;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using ELMAR.DevHtmlHelper.Helpers;
using System.Runtime.Caching;
using System;

namespace ELMAR.DevHtmlHelper.Controllers
{    
    public class DevHelperController : Controller
    {
        private readonly MemoryCache _cache;

        public DevHelperController()
        {
            _cache = MemoryCache.Default;
        }

        /// <summary>
        /// Método para exibição do GridView DevExpress
        /// </summary>
        /// <param name="ViewDataAct">Informar os valores pré-existentes no DataView</param>
        /// <param name="dtSource">Objeto do tipo Enumerable (List, DataView)</param>
        /// <param name="sqlCode">Código SQL para ser executado no BD</param>
        /// <param name="bdConnection">Nome da conexão padrão com a base de dados</param>
        /// <param name="gridName">Nome do Grid (UI)</param>
        /// <param name="gridTitulo">Título do Grid</param>
        /// <param name="gridKey">Chave do grid, não deve conter valores repetidos</param>
        /// <param name="Tema">Tema Ex: Glass, Blue, DevEx, PlasticBlue</param>
        /// <param name="tamPagina">Qtde. de registros a exibir por página no grid</param>
        /// <param name="Controller">Controlador para o callback</param>
        /// <param name="Action">Acão para o callback</param>
        /// <param name="Filtros">Filtros de consulta: NOT IMPLEMENTED</param>
        /// <param name="Agrupadores">Colunas agrupadas</param>
        /// <param name="CustomAgrupadores">Agrupadores personalizados</param>
        /// <param name="Expanded">Exibe os grupos expandidos</param>
        /// <param name="Totalizadores">Colunas totalizadoras de valores</param>
        /// <param name="TotalizadoresGrp">Colunas totalizadoras de valores por grupo</param>
        /// <param name="ColunasOcultas">Colunas ocultas do grid</param>
        /// <param name="ColunasLinks">Colunas do tipo link, devem conter uma URL</param>
        /// <param name="ColunasActions">Colunas de ação (Parâmetros: Nome_da_coluna|Action|Controller|Titulo_do_popup|param1|param2|...)</param>
        /// <param name="DetailAction">Ação para detalhamento da linha do grid (Parâmetros: Action|Controller|param1|param2|...)</param>
        /// <param name="OutputFormat">Formato para renderização: Default = HTML (PDF, CSV, RTF, XLS, XLSX)</param>        
        /// <param name="ExibeLinhaFiltro">Exibe a funcionalidade de filtrar dados das linhas de registros</param>
        /// <param name="PersonalizarFiltro">Exibe a funcionalidade de personalizar o filtro de consulta</param>
        /// <param name="ExibeExportador">Exibe a funcionalidade de exportação dos dados</param>
        /// <param name="isCallBack">Utilizado para evitar o carregamento completo da estrutura após um PostBack</param>
        /// <returns></returns>
        public ActionResult GridViewPartial(ViewDataDictionary ViewDataAct = null, IEnumerable dtSource = null, string sqlCode = "", string bdConnection = "ConexaoPgl", string assemblyDll = "", string gridName = "", string gridTitulo = "", string gridKey = "", string Tema = "Default", int tamPagina = 30, string Controller = "DevHelper", string Action = "GridViewPartial", string Filtros = "", string Agrupadores = "", string CustomAgrupadores = "", bool Expanded = true, string Totalizadores = "", string TotalizadoresGrp = "", string ColunasOcultas = "", string ColunasDateTime = "", string ColunasCurrency = "", string ColunasImage = "", string ColunasHtml = "", string ColunasLinks = "", string ColunasActions = "", string DetailAction = "", string CRUDActions = "", string OutputFormat = "HTML", bool ExibeLinhaFiltro = true, bool PersonalizarFiltro = true, string Info = "", string Altura = "100%", string Largura = "100%", bool ExibeExportador = true, string ExportadorPosicao = "Rodape", string ExportHeaderText = "", bool ExibeAgrupador = true, bool isCallBack = false, bool refreshData = true, bool autoScroll = true, string CTX = "", bool ShowSelectCheckbox = false, string SelectRowMode = "Single", bool customGrouping = false, bool reloadData = false, string CallbackRoute = "", string InfoColunas = "", string selectedIDs = "", bool showFooter = true, bool adaptativeMode = false, int Tab = -1, int page = 1)
        {
            string viewFile = "DevHelper/GridViewPartial";
            string nameUI = (!string.IsNullOrEmpty(gridName)) ? Regex.Replace(gridName, "[^0-9a-zA-Z]+", "") : Regex.Replace(gridTitulo, "[^0-9a-zA-Z]+", "");

            /*if (isCallBack)
            {
                CTX = Core.GetSetCookie(HttpContext);
            }
            else
            {
                CTX = Core.GetSetCTX(HttpContext, CTX, "CTX", false);
            }*/

            ViewData = ViewDataAct != null ? ViewDataAct : ViewData;
            ViewData["sqlCode"] = sqlCode;
            ViewData["gridName"] = gridName.Equals(string.Empty) ? Util.ConvertToAlphaNumeric(gridTitulo) : Util.ConvertToAlphaNumeric(gridName);
            ViewData["gridTitulo"] = gridTitulo;
            ViewData["gridKey"] = gridKey;
            ViewData["gridTamPagina"] = tamPagina;
            ViewData["Tema"] = Tema;
            ViewData["Controller"] = Controller;
            ViewData["Action"] = Action;
            ViewData["refreshData"] = refreshData;
            ViewData["DetailAction"] = DetailAction;
            ViewData["Filtros"] = Filtros;
            ViewData["Agrupadores"] = Agrupadores;
            ViewData["CustomAgrupadores"] = CustomAgrupadores;
            ViewData["Totalizadores"] = Totalizadores;
            ViewData["TotalizadoresGrp"] = TotalizadoresGrp;
            ViewData["ColunasOcultas"] = ColunasOcultas;
            ViewData["ColunasLinks"] = ColunasLinks;
            ViewData["ColunasActions"] = ColunasActions;
            ViewData["ColunasDateTime"] = ColunasDateTime;
            ViewData["ColunasCurrency"] = ColunasCurrency;
            ViewData["ColunasImage"] = ColunasImage;
            ViewData["ColunasHtml"] = ColunasHtml;
            ViewData["CRUDActions"] = CRUDActions;
            ViewData["ExibeLinhaFiltro"] = ExibeLinhaFiltro;
            ViewData["PersonalizarFiltro"] = PersonalizarFiltro;
            ViewData["ExibeExportador"] = ExibeExportador;
            ViewData["ExportadorPosicao"] = ExportadorPosicao;
            ViewData["ExibeAgrupador"] = ExibeAgrupador;
            ViewData["Info"] = Info;
            ViewData["Altura"] = Altura;
            ViewData["Largura"] = Largura;
            ViewData["ExportHeaderText"] = ExportHeaderText;
            ViewData["bdConnection"] = bdConnection;
            ViewData["Expanded"] = Expanded;
            ViewData["autoScroll"] = autoScroll;
            ViewData["ShowSelectCheckbox"] = ShowSelectCheckbox;            
            ViewData["SelectRowMode"] = ShowSelectCheckbox ? "Multi" : SelectRowMode;
            ViewData["InfoColunas"] = InfoColunas;
            ViewData["CTX"] = CTX;
            ViewBag.showFooter = showFooter;
            ViewBag.adaptativeMode = adaptativeMode;
            ViewData["Tab"] = Tab;
            //Inicializando variável de sessão (Params Values)
            Session[ViewData["gridName"] + CTX + "_GridKeyParams"] = null;

            if (Util.IsMobileDevice(Request))
            {
                ViewData["Largura"] = ViewData["Altura"] = "100%";
                ViewData["autoScroll"] = true;
            }

            if(isCallBack)
            {
                if (!string.IsNullOrEmpty(Request["selectedIDs"]))
                {
                    if (!string.IsNullOrEmpty(Request["actionLink"]))
                    {
                        RouteValueDictionary routeValueDictionary = (RouteValueDictionary)Serializador.DeserializarJson(Request["routeValuesLink"], JsonConvertTypes.RouteValueDictionary);
                        //Adicionando parâmetro selectedIDs ao routeValue
                        if (!routeValueDictionary.ContainsKey("selectedIDs"))
                            routeValueDictionary.Add("selectedIDs", selectedIDs);
                        string actionLink = Url.Action(Request["actionLink"], Request["controllerLink"], routeValueDictionary);
                        return Redirect(actionLink);
                    }
                    if (!string.IsNullOrEmpty(Request["redirectLink"]))
                    {
                        return Redirect(Request["redirectLink"] + "?selectedID=" + Request["selectedIDs"]);
                    }
                }

                if (CallbackRoute.Equals("Default"))
                    ViewData = Session["ViewData" + nameUI + CTX] != null ? (ViewDataDictionary)Session["ViewData" + nameUI + CTX] : ViewData;

                ViewData["isCallBack"] = true;
                ViewData["customGrouping"] = customGrouping;

                //var dtSourceMemory = MemoryCacheObject<object>.GetDataFromCache("SessionModel" + nameUI + CTX);
                var dtSourceMemory = GetDataFromCache("SessionModel" + nameUI + CTX);

                if (dtSourceMemory != null)
                {
                    if (dtSourceMemory is DataView view)
                        dtSource = view;
                    else if (dtSourceMemory is IList list)
                        dtSource = list;
                    else
                        dtSource = Core.JsonToDataView(dtSourceMemory.ToString());
                }

                //Recarrega o model atualizando os dados
                else if (TempData["SessionModel" + nameUI + CTX] != null)
                {
                    if (TempData["SessionModel" + nameUI + CTX] is DataView)
                        dtSource = (DataView)TempData["SessionModel" + nameUI + CTX];
                    else if (TempData["SessionModel" + nameUI + CTX] is IList)
                        dtSource = (IList)TempData["SessionModel" + nameUI + CTX];
                    else
                        dtSource = Core.JsonToDataView(TempData["SessionModel" + nameUI + CTX].ToString());
                }
                else if (Session["SessionModel" + nameUI + CTX] != null)
                {
                    if (Session["SessionModel" + nameUI + CTX] is DataView)
                        dtSource = (DataView)Session["SessionModel" + nameUI + CTX];
                    else if (Session["SessionModel" + nameUI + CTX] is IList)
                        dtSource = (IList)Session["SessionModel" + nameUI + CTX];
                    else
                        dtSource = Core.JsonToDataView(Session["SessionModel" + nameUI + CTX].ToString());
                }
                else if (Session["SessionDataModelJson" + nameUI + CTX] != null)
                {
                    dtSource = Core.JsonToDataView(Session["SessionDataModelJson" + nameUI + CTX].ToString());
                }
                else
                {
                    dtSource = (IList)Session["SessionDataModelList" + nameUI + CTX];
                }

                sqlCode = Session["SessionSqlCode" + nameUI + CTX]?.ToString();

                dtSource = dtSource.PrepareDataSource(sqlCode, bdConnection, assemblyDll);

                // Export Handler
                if (!OutputFormat.ToUpper().Equals("HTML") && !OutputFormat.ToUpper().Equals("GRID"))
                {
                    return RedirectToAction("ExportTo", "DevHelper", new
                    {
                        OutputFormat,
                        dtSource, //ViewData["dtSource"],
                        gridTitulo = ViewData["gridTitulo"],
                        gridName = ViewData["gridName"],
                        gridKey = ViewData["gridKey"],
                        tamPagina = ViewData["tamPagina"],
                        Tema = ViewData["Tema"],
                        Controller = ViewData["Controller"],
                        Action = ViewData["Action"],
                        Filtros = ViewData["Filtros"],
                        Agrupadores = ViewData["Agrupadores"],
                        Expanded = ViewData["Expanded"],
                        Totalizadores = ViewData["Totalizadores"],
                        TotalizadoresGrp = ViewData["TotalizadoresGrp"],
                        ColunasOcultas = ViewData["ColunasOcultas"],
                        ColunasActions = (string)ViewData["ColunasActions"] + ";" + (string)ViewData["ColunasLinks"],
                        ColunasDateTime = ViewData["ColunasDateTime"],
                        ColunasCurrency = ViewData["ColunasCurrency"],
                        ColunasImage = ViewData["ColunasImage"],
                        ColunasHtml = ViewData["ColunasHtml"],
                        ExportHeaderText = ViewData["ExportHeaderText"],
                        CTX = ViewData["CTX"]
                    });
                }

                return PartialView(viewFile, dtSource);
            }
            else
            {
                if (dtSource is DataView view)
                {
                    Session["SessionDataModelJson" + nameUI + CTX] = JsonConvert.SerializeObject(view.ToTable());                    
                }
                else if (dtSource is IList)
                {
                    Session["SessionDataModelList" + nameUI + CTX] = dtSource;                    
                }
                if (!string.IsNullOrEmpty(sqlCode))
                {
                    Session["SessionSqlCode" + nameUI + CTX] = sqlCode;                    
                }                
            }            

            #region Ativação do módulo de exportação
            if (!OutputFormat.ToUpper().Equals("HTML") && !OutputFormat.ToUpper().Equals("GRID"))
            {
                return RedirectToAction("ExportTo", "DevHelper", new
                {
                    OutputFormat,
                    sqlCode = ViewData["sqlCode"],
                    dtSource,
                    gridTitulo = ViewData["gridTitulo"],
                    gridName = ViewData["gridName"],
                    gridKey = ViewData["gridKey"],
                    tamPagina = ViewData["tamPagina"],
                    Tema = ViewData["Tema"],
                    Controller = ViewData["Controller"],
                    Action = ViewData["Action"],
                    Filtros = ViewData["Filtros"],
                    Agrupadores = ViewData["Agrupadores"],
                    Expanded = ViewData["Expanded"],
                    TotalizadoresGrp = ViewData["TotalizadoresGrp"],
                    ColunasOcultas = ViewData["ColunasOcultas"],
                    ColunasActions = ViewData["ColunasActions"],
                    ColunasDateTime = ViewData["ColunasDateTime"],
                    ColunasCurrency = ViewData["ColunasCurrency"],
                    ColunasImage = ViewData["ColunasImage"],
                    ColunasHtml = ViewData["ColunasHtml"],
                    ExportHeaderText = ViewData["ExportHeaderText"],
                    CTX = ViewData["CTX"]
                });
            }
            #endregion

            dtSource = dtSource.PrepareDataSource(sqlCode, bdConnection, assemblyDll);

            #region Set DtSource Cache
            //MemoryCacheObject<DataView>.StoreDataInCache("SessionModel" + nameUI + CTX, (DataView)dtSource);
            while (!SetDataToCache("SessionModel" + nameUI + CTX, dtSource))
                SetDataToCache("SessionModel" + nameUI + CTX, dtSource);
            #endregion

            return PartialView(viewFile, dtSource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="sqlCode"></param>
        /// <param name="bdConnection"></param>
        /// <param name="gridName"></param>
        /// <param name="gridTitulo"></param>
        /// <param name="gridKey"></param>
        /// <param name="Tema"></param>
        /// <param name="tamPagina"></param>
        /// <param name="Controller"></param>
        /// <param name="Action"></param>
        /// <param name="Filtros"></param>
        /// <param name="Agrupadores"></param>
        /// <param name="Expanded"></param>
        /// <param name="Totalizadores"></param>
        /// <param name="ColunasOcultas"></param>
        /// <param name="ColunasLinks"></param>
        /// <param name="ColunasActions"></param>
        /// <param name="ColunasDateTime"></param>
        /// <param name="ColunasCurrency"></param>
        /// <param name="ColunasImage"></param>
        /// <param name="ColunasHtml"></param>
        /// <param name="DetailAction"></param>
        /// <param name="CRUDActions"></param>
        /// <param name="OutputFormat"></param>
        /// <param name="ExibeLinhaFiltro"></param>
        /// <param name="PersonalizarFiltro"></param>
        /// <param name="Info"></param>
        /// <param name="Altura"></param>
        /// <param name="Largura"></param>
        /// <param name="ExibeExportador"></param>
        /// <param name="ExportHeaderText"></param>
        /// <param name="ExibeAgrupador"></param>
        /// <param name="isCallBack"></param>
        /// <param name="refreshData"></param>
        /// <param name="CTX"></param>
        /// <returns></returns>
        public ActionResult ReportGridExporter(IEnumerable dtSource = null, string sqlCode = "", string bdConnection = "ConexaoPgl", string gridName = "", string gridTitulo = "", string gridKey = "", string Tema = "Default", int tamPagina = 30, string Controller = "DevHelper", string Action = "GridViewPartial", string Filtros = "", string Agrupadores = "", bool Expanded = true, string Totalizadores = "", string ColunasOcultas = "", string ColunasLinks = "", string ColunasActions = "", string ColunasDateTime = "", string ColunasCurrency = "", string ColunasImage = "", string ColunasHtml = "", string DetailAction = "", string CRUDActions = "", string OutputFormat = "HTML", bool ExibeLinhaFiltro = true, bool PersonalizarFiltro = true, string Info = "", string Altura = "100%", string Largura = "100%", bool ExibeExportador = true, string ExportHeaderText = "", bool ExibeAgrupador = true, bool isCallBack = false, bool refreshData = true, string CTX = "")
        {
            string viewFile = "DevHelper/_GridViewExporterPartial";
            ViewData["sqlCode"] = sqlCode;
            ViewData["gridName"] = gridName;
            ViewData["gridTitulo"] = gridTitulo;
            ViewData["gridKey"] = gridKey;
            ViewData["gridTamPagina"] = tamPagina;
            ViewData["Tema"] = Tema;
            ViewData["Controller"] = Controller;
            ViewData["Action"] = Action;
            ViewData["refreshData"] = refreshData;
            ViewData["DetailAction"] = DetailAction;
            ViewData["Filtros"] = Filtros;
            ViewData["Agrupadores"] = Agrupadores;
            ViewData["Totalizadores"] = Totalizadores;
            ViewData["ColunasOcultas"] = ColunasOcultas;
            ViewData["ColunasLinks"] = ColunasLinks;
            ViewData["ColunasActions"] = ColunasActions;
            ViewData["ColunasDateTime"] = ColunasDateTime;
            ViewData["ColunasCurrency"] = ColunasCurrency;
            ViewData["ColunasImage"] = ColunasImage;
            ViewData["ColunasHtml"] = ColunasHtml;
            ViewData["CRUDActions"] = CRUDActions;
            ViewData["ExibeLinhaFiltro"] = ExibeLinhaFiltro;
            ViewData["PersonalizarFiltro"] = PersonalizarFiltro;
            ViewData["ExibeExportador"] = ExibeExportador;
            ViewData["ExibeAgrupador"] = ExibeAgrupador;
            ViewData["Info"] = Info;
            ViewData["Altura"] = Altura;
            ViewData["Largura"] = Largura;
            ViewData["ExportHeaderText"] = ExportHeaderText;
            ViewData["bdConnection"] = bdConnection;
            ViewData["Expanded"] = Expanded;
            ViewData["CTX"] = CTX;            
            return PartialView(viewFile, dtSource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Titulo"></param>
        /// <param name="SubTitulo"></param>
        /// <param name="sqlCode"></param>
        /// <param name="bdConnection"></param>
        /// <param name="ColunasOcultas"></param>
        /// <param name="Agrupadores"></param>
        /// <param name="Model"></param>
        /// <param name="Tema"></param>
        /// <param name="Report"></param>
        /// <param name="Paisagem"></param>
        /// <param name="isCallback"></param>
        /// <param name="exibeBordasTabela"></param>
        /// <param name="InfoColunas"></param>
        /// <param name="WaterMark"></param>
        /// <param name="Logo"></param>
        /// <param name="HeaderInfo"></param>
        /// <param name="exibeTotalizadores"></param>
        /// <param name="subReports"></param>
        /// <returns></returns>
        public ActionResult ReportControlPartial(string Titulo = "", string SubTitulo = "", string sqlCode = "", string bdConnection = "ConexaoPgl", string ColunasOcultas = "", string Agrupadores = "", string Totalizadores = "", string ColunasCurrency = "", bool GroupPageBreak = true, object Model = null, string Tema = "Default", object Report = null, bool Paisagem = false, bool isCallback = false, bool exibeBordasTabela = true, string InfoColunas = "", string WaterMark = "", string Logo = "", string HeaderInfo = "", bool exibeTotalizadores = false, string SummaryOperation = "Count", Dictionary<string, object> subReports = null, string ViewControl = "ReportViewer" )
        {
            string viewFile = "DevHelper/ReportControlPartial";            
            XtraReport report = new XtraReport();
            ViewData["sqlCode"] = sqlCode;
            ViewData["Model"] = Model;
            ViewData["isCallback"] = isCallback;
            ViewData["Report"] = Report;
            ViewData["ColunasOcultas"] = ColunasOcultas;
            ViewData["Agrupadores"] = Agrupadores;
            ViewData["Totalizadores"] = Totalizadores;
            ViewData["Tema"] = Tema;
            ViewBag.exibeBordasTabela = exibeBordasTabela;
			ViewBag.exibeTotalizadores = exibeTotalizadores;
            ViewBag.SummaryOperation = SummaryOperation;
            ViewBag.InfoColunas = InfoColunas;
            ViewBag.ViewControl = ViewControl;
            ViewBag.GroupPageBreak = GroupPageBreak;
            ViewBag.ColunasCurrency = ColunasCurrency;
            //report = new DevHelper.Views.Reports.XtraReport1();
            if (isCallback)
            {
                report = (DevExpress.XtraReports.UI.XtraReport)Session["SessionReportdocumentViewer1"];
                //report.DataSource = Session["DataModel"];
            }
            else
            {
                if (Report is XtraReport)
                {
                    report = (XtraReport)Report;
                    report.PaperKind = System.Drawing.Printing.PaperKind.A4;
                    report.Landscape = Paisagem;
                    //Populando os DataSource dos SubReports
                    if (subReports != null)
                    {
                        foreach (var item in subReports)
                        {
                            //XRSubreport detailReport = report.Bands[BandKind.Detail].FindControl("xrSubreport" + item.Key, true) != null ? report.Bands[BandKind.Detail].FindControl("xrSubreport" + item.Key, true) as XRSubreport : report.Bands[BandKind.Detail].FindControl(item.Key, true) as XRSubreport; //Acesso ao controle xrSubreport
                            XRSubreport detailReport = report.FindControl("xrSubreport" + item.Key, true) != null ? report.FindControl("xrSubreport" + item.Key, true) as XRSubreport : report.FindControl(item.Key, true) as XRSubreport; //Acesso ao controle xrSubreport
                            if (detailReport != null)
                            {
                                //Referecia pelo nome da classe 
                                detailReport.ReportSource = XtraReport.FromFile(System.Configuration.ConfigurationManager.AppSettings["pathReports"].ToString() + "\\" + item.Key + ".repx", true);
                                detailReport.ReportSource.DataSource = item.Value;
                            }
                        }
                    }

                    //Popula dados (DataBindings)
                    if (Model is DataView)
                    {
                        report.DataSource = Model;
                        //report.FillDataSource();
                    }
                    //Utilizado em callback
                    //Session["DataModel"] = Model;
                    ViewData["Report"] = Session["SessionReportdocumentViewer1"] = report;
                }
                else
                    report = new ReportModel
                    {
                        Titulo = Titulo,
                        SubTitulo = SubTitulo,
                        Model = Model,
                        bdConnection = bdConnection,
                        sqlCode = sqlCode,
                        Paisagem = Paisagem,
                        ColunasOcultas = ColunasOcultas.Split(';'),
                        Agrupadores = Agrupadores.Split(';'),
                        Totalizadores = Totalizadores.Split(';'),
                        ColunasCurrency = ColunasCurrency.Split(';'),
                        showTableBorders = exibeBordasTabela,
                        InfoColunas = InfoColunas,
                        WaterMark = WaterMark,
                        Logo = Logo,
                        HeaderInfo = HeaderInfo,
                        showTotalizadores = exibeTotalizadores,
                        SummaryOperation = SummaryOperation,
                        GroupPageBreak = GroupPageBreak
                    }.CreateReportViewer();
                //Utilizado em callback
                //Session["DataModel"] = Model;
                ViewData["Report"] = Session["SessionReportdocumentViewer1"] = report;
            }            
            return PartialView(viewFile, report);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Titulo"></param>
        /// <param name="SubTitulo"></param>
        /// <param name="sqlCode"></param>
        /// <param name="bdConnection"></param>
        /// <param name="ColunasOcultas"></param>
        /// <param name="Agrupadores"></param>
        /// <param name="Model"></param>
        /// <param name="Report"></param>
        /// <param name="Paisagem"></param>
        /// <param name="isCallback"></param>
        /// <returns></returns>
        public ActionResult ReportControlExport(string Titulo = "", string SubTitulo = "", string sqlCode = "", string bdConnection = "ConexaoPgl", string ColunasOcultas = "", string Agrupadores = "", object Model = null, object Report = null, bool Paisagem = false, bool isCallback = false)
        {
            XtraReport report = new XtraReport();
            ViewData["sqlCode"] = sqlCode;
            if (isCallback)
                Model = Session["SessionModeldocumentViewer1"];            
            ViewData["Model"] = Model;
            ViewData["Report"] = Report;
            ViewData["ColunasOcultas"] = ColunasOcultas;
            ViewData["Agrupadores"] = Agrupadores;
            //report = new DevHelper.Views.Reports.XtraReport1();
            if (isCallback)
                report = (DevExpress.XtraReports.UI.XtraReport)Session["SessionReportdocumentViewer1"];
            else
            {
                if (Report is XtraReport)
                    report = (XtraReport)Report;
                else
                    report = new ReportModel() { Titulo = Titulo, SubTitulo = SubTitulo, Model = Model, bdConnection = bdConnection, sqlCode = sqlCode, Paisagem = Paisagem, ColunasOcultas = ColunasOcultas.Split(';'), Agrupadores = Agrupadores.Split(';') }.CreateReportViewer();
            }
            return ReportViewerExtension.ExportTo(report);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="sqlCode"></param>
        /// <param name="chartName"></param>
        /// <param name="chartTitulo"></param>
        /// <param name="chartSubTitulo"></param>
        /// <param name="Series"></param>
        /// <param name="SeriesData"></param>
        /// <param name="chartArgumento"></param>
        /// <param name="chartValor"></param>
        /// <param name="Altura"></param>
        /// <param name="Largura"></param>
        /// <param name="Tema"></param>
        /// <param name="isCallBack"></param>
        /// <param name="ExibeLegenda"></param>
        /// <returns></returns>
        public ActionResult ChartViewPartial(IEnumerable dtSource = null, string sqlCode = "", string chartName = "", string chartTitulo = "", string chartSubTitulo = "", string Series = "", string SeriesData = "", string chartArgumento = "", string chartValor = "", string Altura = "300", string Largura = "400", string Tema = "Default", string Tipo = "Bar", bool isCallBack = false, bool ExibeLegenda = true, string CTX = "")
        {
            string viewFile = "DevHelper/ChartViewPartial";
            
            #region CallBack Action
            if (isCallBack)
            {
                string nameUI = (!string.IsNullOrEmpty(chartName)) ? chartName : chartTitulo;

                ViewData = (ViewDataDictionary)TempData["ViewData" + nameUI + CTX];

                //var dtSourceMemory = MemoryCacheObject<DataView>.GetDataFromCache("SessionModel" + nameUI + CTX);
                var dtSourceMemory = GetDataFromCache("SessionModel" + nameUI + CTX);

                dtSource = dtSourceMemory != null ? (DataView)dtSourceMemory : (DataView)TempData["SessionModel" + nameUI + CTX];
                
                return PartialView(viewFile, dtSource);
            }
            #endregion

            //Configuração dinâmica
            ViewData["sqlCode"] = sqlCode;
            ViewData["chartName"] = chartName;
            ViewData["chartTitulo"] = chartTitulo;
            ViewData["chartSubTitulo"] = chartSubTitulo;
            ViewData["Altura"] = Altura;
            ViewData["Largura"] = Largura;
            ViewData["Series"] = Series;
            ViewData["SeriesData"] = SeriesData;
            ViewData["chartArgumento"] = chartArgumento;
            ViewData["chartValor"] = chartValor;
            ViewData["ExibeLegenda"] = ExibeLegenda;
            ViewBag.Tipo = Tipo;
            ViewData["CTX"] = CTX;
            ViewBag.Tema = Tema;

            if (dtSource is DataView)
            {
                return PartialView(viewFile, dtSource);
            }
            if (!string.IsNullOrEmpty(sqlCode))
            {
                using (var con = new FwkContexto())
                {
                    dtSource = con.SelectionQuery(sqlCode);
                }
            }

            return PartialView(viewFile, dtSource);
        }

        /// <summary>
        /// Método para exibição do PopupControl DevExpress a partir de uma Action / PartialView / Url
        /// </summary>
        /// <param name="Name">Nome do componente UI (User Interface)</param>
        /// <param name="Titulo">Título do popup</param>
        /// <param name="Action">Nome da Action a ser executada</param>
        /// <param name="Controller">Nome do Controller portador da Action</param>
        /// <param name="Params">Parâmetros necessários para execução da ação (RouteValueDictionary)</param>
        /// <returns></returns>
        public ActionResult PopupControlPartial(string Name, string Titulo, string Action = "", string Controller = "", RouteValueDictionary Params = null, string PViewName = "", string Url = "", string Tema = "Default", string HtmlContent = "", string Link = "", string TextLink = "", string Icon = "", string LinkTip = "", string Altura = "520px", string Largura = "940px", bool autoRefresh = false, bool isMessageBox = false, bool autoScroll = true, string parentComponent = "", bool OpenInFrame = true, bool ShowOnPageLoad = false, string CTX = "")
        {
            string viewName = "DevHelper/PopupControlPartial";
            //Aceita apenas alfa numéricos, para evitar javascript crash
            ViewData["Name"] = (string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Titulo)) ? 
                Util.ConvertToAlphaNumeric(Titulo) 
                : Util.ConvertToAlphaNumeric(Name); 
            ViewData["Titulo"] = Titulo;
            ViewData["Action"] = Action;
            ViewData["Controller"] = Controller;
            ViewData["Params"] = Params;
            ViewData["PViewName"] = PViewName;
            ViewData["Url"] = Url;
            ViewData["Link"] = Link;
            ViewData["TextLink"] = TextLink;
            ViewData["Icon"] = Icon;
            ViewData["LinkTip"] = (!string.IsNullOrEmpty(LinkTip) ? LinkTip : Titulo);
            ViewData["Altura"] = Altura;
            ViewData["Largura"] = Largura;
            ViewData["CTX"] = CTX;

            if (ELMAR.DevHtmlHelper.Models.Util.IsMobileDevice(Request))
            {
                ViewData["Altura"] = Request.Browser.ScreenPixelsWidth.ToString()+"px";
                ViewData["Largura"] = Request.Browser.ScreenPixelsHeight.ToString() + "px";
            }
            ViewData["autoRefresh"] = autoRefresh;
            ViewData["parentComponent"] = parentComponent;
            ViewData["HtmlContent"] = HtmlContent;
            ViewData["isMessageBox"] = isMessageBox;
            ViewData["autoScroll"] = autoScroll;
            ViewBag.OpenInFrame = OpenInFrame;
            ViewBag.ShowOnPageLoad = ShowOnPageLoad;
            ViewData["Tema"] = Tema;            

            return PartialView(viewName);
        }

        //TODO: Testar chamada via ParametersJson Attribute
        public ActionResult PopupControlAct(string ParametersJson)
        {
            //return RedirectToAction("PopupControlPartial","");
            JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            dynamic resultado = serializer.DeserializeObject(ParametersJson);
            string viewName = "DevHelper/PopupControlPartial";
            //Aceita apenas alfa numéricos, para evitar javascript crash
            ViewData["Name"] = (string.IsNullOrEmpty(resultado["Name"]) && !string.IsNullOrEmpty(resultado["Titulo"])) ?
                Util.ConvertToAlphaNumeric(resultado["Titulo"])
                : Util.ConvertToAlphaNumeric(resultado["Name"]);
            ViewData["Titulo"] = resultado["Titulo"];
            ViewData["Action"] = resultado["Action"];
            ViewData["Controller"] = resultado["Controller"];
            ViewData["Params"] = resultado["Params"];
            ViewData["PViewName"] = resultado["PViewName"];
            ViewData["Url"] = resultado["Url"];
            ViewData["Link"] = resultado["Link"];
            ViewData["TextLink"] = resultado["TextLink"];
            ViewData["Icon"] = resultado["Icon"];
            ViewData["LinkTip"] = (!string.IsNullOrEmpty(resultado["LinkTip"]) ? resultado["LinkTip"] : resultado["Titulo"]);
            ViewData["Altura"] = resultado["Altura"];
            ViewData["Largura"] = resultado["Largura"];
            ViewData["autoRefresh"] = resultado["autoRefresh"];
            ViewData["parentComponent"] = resultado["parentComponent"];
            ViewData["HtmlContent"] = resultado["HtmlContent"];
            ViewData["isMessageBox"] = resultado["isMessageBox"];
            ViewBag.OpenInFrame = resultado["OpenInFrame"];
            ViewData["Tema"] = resultado["Tema"];

            return PartialView(viewName);
        }

        public ActionResult DockPanelPartial(string Name, string Titulo, string Action = "", string Controller = "", string PViewName = "", string HtmlContent = "", string Tema = "Default", string Url = "", object Params = null, string Altura = "640", string Largura = "800", string CustomLogin = "", bool CloseButton = false, string CTX = "")
        {
            string viewFile = "DevHelper/DockPanelPartial";
            
            ViewData["Name"] = Name;
            ViewData["Titulo"] = Titulo;
            ViewData["Action"] = Action;
            ViewData["Controller"] = Controller;
            //Converte para o padrão (RouteValueDictionary)
            if (Params is ViewDataDictionary)
            {
                RouteValueDictionary routeDictionary = new RouteValueDictionary();
                foreach (var item in (ViewDataDictionary)Params)
                {
                    routeDictionary.Add(item.Key, item.Value);
                }
                Params = routeDictionary;
            }
            else if (Params is RouteValueDictionary)
                Params = (RouteValueDictionary)Params;
            else
                Params = new RouteValueDictionary();
            ViewData["Params"] = Params;
            ViewData["Altura_dockPanel"] = Altura;
            ViewData["Largura_dockPanel"] = Largura;
            ViewData["CloseButton"] = CloseButton;
            ViewData["Url"] = Url;
            ViewData["PViewName"] = PViewName;
            ViewData["HtmlContent"] = HtmlContent;
            ViewData["Tema"] = Tema;
            ViewData["LoginUrl"] = !string.IsNullOrEmpty(CustomLogin) ? CustomLogin : ConfigurationManager.AppSettings["loginUrl"];
            ViewBag.CTX = CTX;

            return PartialView(viewFile);
        }

        public ActionResult TabControlPartial(string Name, string Titulo, string Tabs, string CallbackController = "", string CallbackAction = "", object ActionTabsParams = null, ViewDataDictionary currentViewData = null, string Tema = "Default", string CTX = "")
        {
            string viewFile = "DevHelper/TabControlPartial";

            //Repassando o objeto ViewData atual
            ViewData = currentViewData;
            
            ViewData["Name"] = (Name != "") ? Name : Titulo;
            ViewData["Titulo"] = Titulo;
            ViewData["Tabs"] = Tabs;
            ViewData["ActionTabsParams"] = ActionTabsParams;
            ViewData["Tema"] = Tema;
            ViewBag.CallbackAction = CallbackAction;
            ViewBag.CallbackController = CallbackController;

            //CTX = string.IsNullOrEmpty(CTX) ? Core.GetSetCTX(HttpContext) : CTX;

            return PartialView(viewFile, CTX);
        }

        public ActionResult CustomFormPartial(string Name, string Titulo, string Action, string Campos, string Method = "POST", string OnSubmit = "", string SubmitText = "Enviar", string Tema = "Default", string Largura = "400", string Target = "_self", bool PerformCallback = true)
        {
            string viewFile = "DevHelper/CustomFormPartial";

            ViewData["Name"] = Name;
            ViewData["Titulo"] = Titulo;
            ViewData["Action"] = Action;
            ViewData["Method"] = Method;
            //ViewData["Altura"] = Altura;
            ViewData["OnSubmit"] = OnSubmit;
            ViewData["Largura"] = Largura;
            ViewData["Campos"] = Campos;
            ViewData["SubmitText"] = SubmitText;
            ViewData["Tema"] = Tema;
            ViewBag.Target = Target;
            ViewBag.PerformCallback = PerformCallback;
            
            return PartialView(viewFile);
        }

        public ActionResult ButtonControlPartial(string Name, string Titulo, string Tema = "Default", string Controller = "", string Action = "", RouteValueDictionary Params = null, string Url = "", bool VerificaAutorizacao = true, string Target = "_blank", string GridParent = "", string GridParams = "", int Largura = 110, bool RequerConfirmacao = false)
        {
            string viewFile = "DevHelper/ButtonControlPartial";

            ViewData["Name"] = Name.Equals(string.Empty) ? Regex.Replace(Titulo, "[^0-9a-zA-Z]+", "") : Regex.Replace(Name, "[^0-9a-zA-Z]+", "");
            ViewData["Titulo"] = Titulo;
            ViewData["Action"] = Action;
            ViewData["Controller"] = Controller;
            ViewData["Url"] = Url;
            ViewData["Tema"] = Tema;
            ViewData["Params"] = Params;
            ViewBag.Target = Target;
            ViewBag.GridParent = GridParent;
            ViewBag.GridParams = GridParams;
            ViewData["VerificaAutorizacao"] = VerificaAutorizacao;
            ViewBag.RequerConfirmacao = RequerConfirmacao;
            ViewBag.Largura = Largura;

            return PartialView(viewFile);
        }

        public ActionResult SeletorControlPartial(string Name, IEnumerable ObjItems, string Key, string Value, string SelectedItems = "", int QtdColunas = 4, string Tema = "Default", bool Enabled = true, string Template = "SeletorControlPartial")
        {
            string viewFile = "DevHelper/"+Template;

            ViewData["Name"] = Name;
            ViewData["Tema"] = Tema;
            //ViewData["ItemsList"] = new SelectList(ObjItems, Key, Value, null);
            ViewData["Key"] = Key;
            ViewData["Value"] = Value;
            ViewData["SelectedItems"] = SelectedItems;
            ViewData["QtdColunas"] = QtdColunas;
            ViewData["Enabled"] = Enabled;
            return PartialView(viewFile, ObjItems);
        }
        
        public ActionResult MenuControlPartial(string Name, List<Fwk_MenuItem> Menus, bool Visivel = true, string Largura = "100%", string Orientacao = "Horizontal", string Tema = "Default", bool Expanded = true, bool checkAuthorization = false, string Template = "MenuControl", string CTX = "")
        {            
            if (Util.IsMobileDevice(Request))
                Template = "MenuControl"; //MenuControl - Formato padrão para o ambiente mobile (Adaptativo)
            string viewFile = "DevHelper/"+Template+"Partial";
            ViewData["Name"] = Name;
            ViewData["Largura"] = Largura;
            ViewData["Tema"] = Tema;
            ViewData["Menus"] = Menus;
            ViewData["Visivel"] = Visivel;
            ViewData["Orientacao"] = Orientacao;
            ViewBag.CTX = CTX;
            ViewBag.Expanded = Expanded;
            ViewBag.CheckAuthorization = checkAuthorization;
            return PartialView(viewFile);
        }        

        public ActionResult HtmlEditorPartial(string Name, string Valor="", string Tema = "", string Controller = "", string Action = "", bool isCallBack = false, string Largura = "98%", string Altura = "300px", bool isAdvance = true)
        {
	        ViewBag.Name = Name;
	        ViewBag.Tema = Tema;
	        ViewBag.Controller = Controller;
	        ViewBag.Action = Action;
            ViewBag.Largura = Largura;
            ViewBag.Altura = Altura;
            ViewBag.isAdvance = isAdvance;
            ViewBag.Valor = Valor;
	        return PartialView("DevHelper/HtmlEditorPartial");
        }

        [HttpPost]
        public ActionResult UploadFormPartial(DocumentFile viewModel, string File = "file")
        {
            // Verifica o peso do arquivo
            if (Request.Files.Count != 1 || Request.Files[0].ContentLength == 0)
            {
                ModelState.AddModelError("uploadError", "Arquivo corrompido ou não encontrado");
                return PartialView(viewModel);
            }

            int maxLength = 20; //20MB            
            if (Request.Files[0].ContentLength > 1024 * 1024 * maxLength)
            {
                ModelState.AddModelError("uploadError", "O peso do arquivo não pode exceder "+maxLength+"MB");
                return PartialView(viewModel);
            }

            // Tamanho mínimo 100 bytes
            if (Request.Files[0].ContentLength < 100)
            {
                ModelState.AddModelError("uploadError", "O arquivo é muito pequeno");
                return PartialView(viewModel);
            }

            // Verificando a extensão do arquivo
            string extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
            if (extension != ".pdf" && extension != ".doc" && extension != ".docx" && extension != ".rtf" && extension != ".txt")
            {
                ModelState.AddModelError("uploadError", ": pdf, doc, docx, rtf, txt");
                return PartialView(viewModel);
            }

            // Extrai apenas o nome do arquivo
            var fileName = Path.GetFileName(Request.Files[0].FileName);

            // Salva o arquivo para o respectivo diretório de upload
            string uploadPath = ConfigurationManager.AppSettings["pathUploadsRef"] != null && ConfigurationManager.AppSettings["pathUploadsRef"].ToString().Equals(string.Empty) ? ConfigurationManager.AppSettings["pathUploadsRef"].ToString() : "~/App_Data/uploads";
            //var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            var path = Path.Combine(Server.MapPath(uploadPath), fileName);

            if (ModelState.IsValid)
            {
                if (Core.SaveUploadedFiles(Request))
                    PartialView("Sucesso");
                return PartialView(viewModel);                                                     
            }
            return PartialView(viewModel);                                     
        }

        public ActionResult LoadingPartial()
        {
            string viewFile = "DevHelper/_LoadingPanelPartial";

            if (DevExpressHelper.IsCallback)
            { }
            ViewBag.LoadingText = (Request["LoadingText"] != null) ? Request["LoadingText"] : string.Empty;
            
            return PartialView(viewFile);
        }

        //Método encapsulado para a funcionalidade de exportação
        public ActionResult ExportTo(object dtSource = null, string sqlCode = "", string gridName = "", string gridTitulo = "", string gridKey = "", string Tema = "Default", int tamPagina = 30, string Controller = "DevHelper", string Action = "GridViewPartial", string Filtros = "", string Agrupadores = "", bool Expanded = true, string Totalizadores = "", string ColunasOcultas = "", string ColunasActions = "", string ColunasDateTime = "", string ColunasCurrency = "", string ColunasImage = "", string ColunasHtml = "", string OutputFormat = "PDF", string ExportHeaderText = "", string CTX = "")
        {
            dtSource = MemoryCacheObject<DataView>.GetDataFromCache("SessionModel" + gridName + CTX);

            if(dtSource == null)
            {
                dtSource = TempData["SessionModel" + gridName + CTX];
                dtSource = dtSource ?? Session["SessionModel" + gridName + CTX];
                TempData["SessionModel" + gridName + CTX] = dtSource;
            }

            var settings = new GridViewSettingsHelper(dtSource, gridName, gridTitulo, sqlCode, gridKey, tamPagina, Tema, Controller, Action, Filtros, Agrupadores, Expanded, Totalizadores, ColunasOcultas, ColunasActions, ColunasDateTime, ColunasCurrency, ExportHeaderText).ExportGridViewSettings;                       

            switch (OutputFormat.ToUpper())
            {
                case "CSV":
                    return GridViewExtension.ExportToCsv(settings, dtSource);
                case "PDF":
                    settings.SettingsDetail.ExportMode = GridViewDetailExportMode.None;
                    return GridViewExtension.ExportToPdf(settings, dtSource);
                case "RTF":
                    settings.SettingsDetail.ExportMode = GridViewDetailExportMode.None;
                    return GridViewExtension.ExportToRtf(settings, dtSource);                
                case "DOCX":
                    settings.SettingsDetail.ExportMode = GridViewDetailExportMode.None;
                    return GridViewExtension.ExportToDocx(settings, dtSource);
                case "XLS":
                    return GridViewExtension.ExportToXls(settings, dtSource);
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(settings, dtSource);
                case "TXT":                    
                    //return GridViewExtension.ExportToCsv(settings, dtSource, gridName + ".txt", true, new DevExpress.XtraPrinting.CsvExportOptionsEx(){TextExportMode = DevExpress.XtraPrinting.TextExportMode.Text, QuoteStringsWithSeparators = false});
                    var result = (FileResult)GridViewExtension.ExportToCsv(settings, dtSource, gridName + ".txt", true, new DevExpress.XtraPrinting.CsvExportOptionsEx() { TextExportMode = DevExpress.XtraPrinting.TextExportMode.Text, QuoteStringsWithSeparators = false, Separator = "|" });
                    result.FileDownloadName = gridName+".txt";
                    return result;
                default:
                    return RedirectToAction(Action, Controller);
            }            
        }
        
        private bool SetDataToCache(string key, object value)
        {
            var cacheItemPolicy = new CacheItemPolicy()
            {
                //Set your Cache expiration.
                AbsoluteExpiration = DateTime.Now.AddHours(1)
            };

            if (_cache.Contains(key))
                _cache.Remove(key);

            //remember to use the above created object as third parameter.
            return _cache.Add(key, value, cacheItemPolicy);
        }

        private object GetDataFromCache(string key)
        {
            if (_cache.Contains(key))
                return _cache.Get(key);

            return null;
        }
    }

    //Classes auxiliares para configuração dos controles 
    public class PopupControlConfig
    {
        public static void SetUIParams(RouteValueDictionary parametros, string parametro)
        {
            //TODO: Implementar leitura de parâmetro via string formato JSON
            parametros.Add("Altura", 570);
            parametros.Add("Largura", 940);
            if (!string.IsNullOrEmpty(parametro))
            {
                parametros.Add("Titulo", parametro);
                parametros.Add("LinkTip", parametro);
                //Adicionar parâmetros visuais 'Largura x Altura'
                if (parametro.Contains('&'))
                {
                    parametro = parametro.Replace("PX", "").Replace("px", "").Replace("Px", "");
                    string[] paramsUI = parametro.Split('&');
                    parametros["Titulo"] = parametros["LinkTip"] = paramsUI[0];                    
                    parametros["Largura"] = int.Parse(paramsUI[1]);
                    parametros["Altura"] = int.Parse(paramsUI[2]);
                    if (paramsUI.Length >= 4)
                    {
                        parametros["OpenInFrame"] = bool.Parse(paramsUI[3]);
                    }
                }
            }
        }        
    }

    public static class DtSourceExtensions
    {
        public static IEnumerable PrepareDataSource(this IEnumerable dtSource, string sqlCode, string dbConnection, string assemblyDll = "")
        {
            if (!string.IsNullOrEmpty(sqlCode))
            {
                using (var con = new FwkContexto(dbConnection))
                {
                    dtSource = con.SelectionQuery(sqlCode);
                }
            }
            else if (dtSource is DataView == false && dtSource != null)
            {
                if (dtSource is IList)
                {
                    try
                    {
                        string className = Util.InBetween(dtSource.GetType().ToString(), "[", "]", 0);
                        assemblyDll = string.IsNullOrEmpty(assemblyDll) ? Core.GetAssemblyDllName(className) : assemblyDll;
                        dtSource = Core.ToDataView(assemblyDll, className, dtSource, false);
                    }
                    catch { }
                }
            }

            return dtSource;
        }
    }
}