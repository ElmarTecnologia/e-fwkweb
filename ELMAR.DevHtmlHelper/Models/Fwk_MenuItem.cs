using ELMAR.DevHtmlHelper.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;

namespace ELMAR.DevHtmlHelper.Models
{
    [Table("fwk_menu", Schema = "public")]
    public class Fwk_MenuItem
    {
        //private readonly FwkContexto _contexto = new FwkContexto();

        [Column("men_ID")]
        public string ID
        {
            get;
            set;
        }

        [Column("men_contexto")]
        public string Contexto { get; set; }

        [Column("men_pai_id")]
        public string PaiID { get; set; }
        
        public Fwk_MenuItem Pai
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public IList<Fwk_MenuItem> MenusPai { get; set; }

        [Column("men_titulo")]
        public string Titulo
        {
            get;
            set;
        }

        [Column("men_controller")]
        public string Controller
        {
            get;
            set;
        }

        [Column("men_action")]
        public string Action
        {
            get;
            set;
        }

        private string _NavUrl;        
        [Column("men_navurl")]
        public string NavUrl
        {
            get{
                if (!string.IsNullOrEmpty(_NavUrl) && !_NavUrl.Equals("https"))
                {
                    //Corrige queryString
                    string _NavUrlCTX = _NavUrl.Contains("&ctx=" + Contexto) && !_NavUrl.Contains("?") ? _NavUrl.Replace("&", "?") : _NavUrl;
                    //Adiciona contexto a NavUrl
                    _NavUrlCTX = _NavUrlCTX.Contains("ctx=" + Contexto) ? _NavUrlCTX :
                        _NavUrlCTX.Contains("?") ? _NavUrlCTX + "&ctx=" + Contexto : _NavUrlCTX + "?ctx=" + Contexto;
                    return _NavUrlCTX;
                }
                else
                {
                    var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    if (this.Controller.Equals(string.Empty))
                        return string.Empty;
                    string urlLink = urlHelper.Action(this.Action, this.Controller, this.RouteValueDictionary);
                    //Converte o link para https
                    bool isHttps = false;
                    bool.TryParse(ELMAR.DevHtmlHelper.Models.FwkConfig.GetSettingValue("isHttps"), out isHttps);
                    //Uri uri = new Uri(ELMAR.DevHtmlHelper.Models.FwkConfig.GetSettingValue("pathApp") + urlLink);
                    //Evita a duplicação pathApp
                    Uri uri = new Uri(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + urlLink);
                    if (isHttps && !uri.Scheme.Equals("https"))
                        return uri.ToString().Replace("http:", "https:");
                    return uri.ToString();
                }
            }
            set
            {
                this._NavUrl = value;
            }

        }

        [Column("men_iconurl")]
        public string IconUrl
        {
            get;
            set;
        }

        [Column("men_ativo")]
        public bool Ativo
        {
            get;
            set;
        }

        [Column("men_visivel")]
        public bool Visivel
        {
            get;
            set;
        }

        [Column("men_routeparameters")]
        public string RouteParameters
        {
            get;
            set;
        }

        [Column("men_tooltip")]
        public string ToolTip
        {
            get;
            set;
        }

        [Column("men_group_image")]
        public string GroupImage
        {
            get;
            set;
        }

        [Column("men_order")]
        public int Order
        {
            get;
            set;
        }

        private RouteValueDictionary _RouteValueDictionary;
        [NotMapped]        
        public RouteValueDictionary RouteValueDictionary
        {
            get {
                if (_RouteValueDictionary == null && !string.IsNullOrEmpty(RouteParameters))
                    RouteValueDictionary = new RouteValueDictionary(Core.GetRouteParameters(RouteParameters));
                return _RouteValueDictionary;
            }
            set
            {
                _RouteValueDictionary = value;
            }
        }

        public bool PossuiFilhos()
        {
            throw new Exception("Not Implemented");
        }        

        public static List<Fwk_MenuItem> CarregaMenusCSV(StringReader ContentCSV, string CTX = "")
        {
            Dictionary<string, Fwk_MenuItem> dictionary = new Dictionary<string, Fwk_MenuItem>();
            Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
            try
            {
                string text;
                int order = 0;
                while ((text = ContentCSV.ReadLine()) != null)
                {
                    if (text != string.Empty)
                    {
                        Fwk_MenuItem fwk_MenuItem = null;
                        string[] array = text.Split(';');
                        try
                        {
                            fwk_MenuItem = new Fwk_MenuItem
                            {
                                ID = array[0],
                                Contexto = CTX,
                                PaiID = array[1],
                                Titulo = array[2],
                                Controller = array[3],
                                Action = array[4],
                                NavUrl = array[5],
                                IconUrl = array[6],
                                Ativo = bool.Parse(array[7]),
                                Visivel = bool.Parse(array[8]),
                                ToolTip = array[9],
                                RouteParameters = ((array.Length > 10) ? array[10] : string.Empty),
                                GroupImage = ((array.Length > 11) ? array[11] : string.Empty),
                                Order = (array[0].Equals(array[1])) ? order++ : 9999
                            };                            
                        }
                        catch (Exception ex)
                        {
                            fwk_MenuItem = new Fwk_MenuItem
                            {
                                ID = array[0],
                                Titulo = "ERROR #" + array[0],
                                Controller = "",
                                Action = "",
                                Ativo = true,
                                Visivel = true,
                                ToolTip = ex.Message
                            };
                        }
                        dictionary2.Add(fwk_MenuItem.ID, array);
                        dictionary.Add(fwk_MenuItem.ID, fwk_MenuItem);
                    }
                }
                ContentCSV.Close();
                
                foreach (KeyValuePair<string, string[]> current in dictionary2)
                {
                    foreach (KeyValuePair<string, Fwk_MenuItem> current2 in dictionary)
                    {
                        if (!string.IsNullOrEmpty(current.Value[1]) && current2.Value.ID.Equals(current.Key))
                        {
                            current2.Value.Pai = (dictionary.ContainsKey(current.Value[1]) ? dictionary[current.Value[1]] : null);
                            if (current2.Value.Pai == null)
                            {
                                Fwk_MenuItem pai = new Fwk_MenuItem
                                {
                                    ID = current.Value[1],
                                    Titulo = current.Value[1],
                                    Controller = "",
                                    Action = "",
                                    Ativo = true,
                                    Visivel = true                                
                                };
                                current2.Value.Pai = pai;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (ContentCSV != null)
                {
                    ((IDisposable)ContentCSV).Dispose();
                }
            }
            ContentCSV.Close();
            return dictionary.Values.ToList<Fwk_MenuItem>();
        }

        public static List<Fwk_MenuItem> CarregaMenusCSV(string FilePathCSV, string saveMenuCtx = "")
        {            
            List<Fwk_MenuItem> result;
            if (FilePathCSV.ToLower().EndsWith(".csv") && File.Exists(FilePathCSV))
            {
                StreamReader streamReader = new StreamReader(FilePathCSV);
                result = Fwk_MenuItem.CarregaMenusCSV(new StringReader(streamReader.ReadToEnd()));
            }
            else
            {
                result = null;
            }

            //Atualiza ou salva o menu na base
            if (!string.IsNullOrEmpty(saveMenuCtx))
            {
                using (var _contextoFwk = new FwkContexto())
                {
                    var lstMenu =
                        _contextoFwk.Menus
                        .OrderBy(m => m.Pai.Order)
                        .ThenBy(m => m.Titulo)
                        .Where(m => m.Contexto.Equals(saveMenuCtx)).ToList();
                    if (lstMenu.Count != result.Count)
                    {
                        try
                        {
                            new FwkMenuController().PostFwkMenuItens(result);
                        }
                        catch { }
                    }
                }
            }
            return result;
        }

        public IReadOnlyCollection<SitemapNode> GetSitemapNodes(UrlHelper urlHelper, HttpSessionStateBase Session, string FileContentCSV)
        {
            List<Fwk_MenuItem>  menuItens = CarregaMenusCSV(FileContentCSV);
            return GetSitemapNodes(urlHelper, Session, menuItens);
        }

        public IReadOnlyCollection<SitemapNode> GetSitemapNodes(UrlHelper urlHelper, HttpSessionStateBase Session, List<Fwk_MenuItem> Items)
        {
            List<SitemapNode> nodes = new List<SitemapNode>
            {
                new SitemapNode()
                {
                    Url = FwkConfig.GetSettingValue("PathApp", Session),
                    Priority = 1
                }
            };

            foreach (Fwk_MenuItem menuItem in Items)
            {
                nodes.Add(
                   new SitemapNode()
                   {
                       Url = menuItem.NavUrl,
                       Titulo = menuItem.Titulo,
                       Frequency = SitemapFrequency.Weekly,
                       Priority = int.Parse(menuItem.ID)
                   });
            }

            return nodes;
        }

        public string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.Titulo == null ? null : new XElement(
                        xmlns + "title", sitemapNode.Titulo),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }
    }

    public class SitemapNode
    {
        public SitemapFrequency? Frequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Titulo { get; set; }
        public string Url { get; set; }
    }

    public enum SitemapFrequency
    {
        Never,
        Yearly,
        Monthly,
        Weekly,
        Daily,
        Hourly,
        Always
    }
}