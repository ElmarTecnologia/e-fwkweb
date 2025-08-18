using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Web.Routing;
using System.Drawing;
using System.Drawing.Drawing2D;
using Renci.SshNet;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Hanssens.Net;
using System.Threading;
using System.Threading.Tasks;
using ELMAR.DevHtmlHelper.Models.Extensions;

namespace ELMAR.DevHtmlHelper.Models
{
    public class Core
    {
        private const string _key = "@hashkey_elmk17_GJHASD$%D*(*D*HDJSJNS";
        private static bool sshIsRunning;

        public static async Task<List<string>> FtpGetDirContentsAsync(string path)
        {
            string ftpUrl = ConfigurationManager.AppSettings["FTPHost"];
            string user = ConfigurationManager.AppSettings["FTPHostUser"];
            string pass = ConfigurationManager.AppSettings["FTPHostPass"];
            List<string> files = new List<string>();

            List<string> liArquivos = new List<string>();
            //Cria comunicação com o servidor
            //Definir o diretório a ser listado
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl + path);
            //Define que a ação vai ser de listar diretório
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            //Credenciais para o login (usuario, senha)
            request.Credentials = new NetworkCredential(user, pass);
            //modo passivo
            //request.UsePassive = true;
            //dados binarios
            request.UseBinary = true;
            //setar o KeepAlive para true
            request.KeepAlive = true;
            request.EnableSsl = true;

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    //Criando a Stream para pegar o retorno
                    Stream responseStream = response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        //Adicionar os arquivos na lista
                        liArquivos = (await reader.ReadToEndAsync()).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                    }
                }

                //Responder a lista dos arquivos
                foreach (string item in liArquivos)
                {
                    files.Add(item);
                }
            }
            catch { }

            return files;
        }

        public static List<string> FtpGetDirContents(string path)
        {
            string ftpUrl = ConfigurationManager.AppSettings["FTPHost"];
            string user = ConfigurationManager.AppSettings["FTPHostUser"];
            string pass = ConfigurationManager.AppSettings["FTPHostPass"];
            List<string> files = new List<string>();

            List<string> liArquivos = new List<string>();
            //Cria comunicação com o servidor
            //Definir o diretório a ser listado
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl + path);
            //Define que a ação vai ser de listar diretório

            request.Method = WebRequestMethods.Ftp.ListDirectory;

            //Credenciais para o login (usuario, senha)
            request.Credentials = new NetworkCredential(user, pass);
            //modo passivo
            //request.UsePassive = true;
            //dados binarios
            request.UseBinary = true;
            //setar o KeepAlive para true
            request.KeepAlive = true;
            //utilizar o ssl
            request.EnableSsl = true;

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    //Criando a Stream para pegar o retorno
                    Stream responseStream = response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        //Adicionar os arquivos na lista
                        liArquivos = (reader.ReadToEnd()).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                    }
                }

                //Responder a lista dos arquivos
                foreach (string item in liArquivos)
                {
                    files.Add(item);
                }
            }
            catch { }

            return files;
        }

        public static string ParserInfo(string type, string value)
        {
            switch (type)
            {
                case "Month":
                    CultureInfo br = new CultureInfo("pt-BR");
                    System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                    DateTime dtTime = new DateTime(2015, int.Parse(value) + 1, 1);
                    string strMonth = dtTime.ToString("MMMM", br).ToUpper();
                    //string strMonth = mfi.GetMonthName(dtTime.Month).ToString(br);
                    return strMonth;
                case "Tema":
                    switch (value)
                    {
                        //"Aqua", "BlackGlass", "DevEx", "Glass", "iOS",
                        //"Metropolis", "Office2003Blue", "Office2003Olive", "Office2003Silver", "Office2010Black",
                        //"Office2010Blue", "Office2010Silver", "PlasticBlue", "RedWine", "SoftOrange", "Youthful" 
                        case "Aqua":
                            return value + " (Azul Claro)";
                        case "BlackGlass":
                            return value + " (Preto e Azul Claro)";
                        case "Glass": //TEMA PADRÃO
                            return value + " (Prata e Azul Claro)";
                        case "iOS":
                            return value + " (Cinza e Azul - Fontes Grandes)";
                        case "MetropolisBlue":
                            return value + " (Cinza e Azul)";
                        case "Metropolis":
                            return value + " (Cinza e Laranja)";
                        case "Office2003Blue":
                            return value + " (Azul)";
                        case "Office2003Olive":
                            return value + " (Verde Oliva)";
                        case "Office2003Silver":
                            return value + " (Cinza Prata e Laranja)";
                        case "Office2010Black":
                            return value + " (Grafite e Cinza)";
                        case "Office2010Blue":
                            return value + " (Azul Claro)";
                        case "Office2010Silver":
                            return value + " (Prata)";
                        case "PlasticBlue":
                            return value + " (Azul Escuro e Cinza)";
                        case "RedWine":
                            return value + " (Lilás e Rosa)";
                        case "SoftOrange":
                            return value + " (Cinza Prata e Laranja)";
                        case "Youthful":
                            return value + " (Verde e Laranja)";
                        case "Moderno":
                            return value + " (Padrão Azul e Cinza)";
                        case "Material":
                        case "MaterialCompact":
                            return value + " (Verde e Cinza)";
                        case "Office365":
                            return value + " (Laranja e Cinza)";
                        case "Mulberry":
                            return value + " (Roxo e Cinza)";
                        case "Default":
                            return "Padrão" + " (Cinza Claro)";
                        default:
                            return value;
                    }
                case "Periodo": //Utilizando para os relatórios
                    if (string.IsNullOrEmpty(value) || value.Equals("%"))
                    {
                        return string.Empty;
                    }
                    if (value.Equals("ANUAL"))
                    {
                        return value;
                    }
                    if (value.StartsWith("QUADRIENAL"))
                    {
                        string[] values = value.Split(' ');
                        int ano = int.Parse(values[1]);
                        string saidaDesc = "QUADRIENAL " + values[1];
                        //Mostra apenas a descrição de anos com publicação do PPA
                        for (int i = 2002; i <= 2200; i = i + 4)
                        {
                            if (i <= ano && ano < i + 4)
                            {
                                int p = i + 3;
                                saidaDesc = "QUADRIENAL " + i + "-" + p;
                                break;
                            }
                        }
                        return saidaDesc;
                    }
                    var saida = int.Parse(value.Substring(0, 2)).ToString() + "º";
                    switch (value.Substring(2, 3))
                    {
                        case "BIM":
                            return saida + " Bimestre";
                        case "TRI":
                            return saida + " Trimestre";
                        case "SEM":
                            return saida + " Semestre";
                        case "QUA":
                            return saida + " Quadrimestre";
                        default:
                            return saida + " Período";
                    }
                case "DateTime":
                    return DateTime.Parse(value).ConvertToPortalUpdated();
                case "Date":
                    return DateTime.Parse(value).ToShortDateString();
                default:
                    return value;
            }
        }

        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public static bool SetViewDataAdapter(ViewDataDictionary ViewData, DataView dtView)
        {
            bool Encontrado = false;
            if (dtView.ToTable().Rows.Count > 0)
            {
                Encontrado = true;
                foreach (System.Data.DataRow row in dtView.ToTable().Rows)
                {
                    foreach (System.Data.DataColumn col in dtView.ToTable().Columns)
                    {
                        ViewData[col.Caption] = row[col.Caption].ToString();
                    }
                    break;
                }
            }
            return Encontrado;
        }

        public static bool SaveUploadedFiles(HttpRequestBase Request)
        {
            foreach (string file in Request.Files)
            {
                var hpf = Request.Files[file];
                if (hpf.ContentLength == 0)
                    continue;

                string savedFileName = Path.Combine(ConfigurationManager.AppSettings["pathUploads"], Path.GetFileName(hpf.FileName));

                hpf.SaveAs(savedFileName);
                return true;
            }
            return false;
        }

        public static string SaveUploadedFile(object file)
        {
            var hpf = ((HttpPostedFileBase[])file)[0];

            if (hpf == null || hpf.ContentLength == 0)
                return string.Empty;

            string savedFileName = Path.Combine(ConfigurationManager.AppSettings["pathUploads"], Path.GetFileName(hpf.FileName));

            hpf.SaveAs(savedFileName);
            
            return savedFileName;                        
        }

        public static string DownloadFile(string url, string localPath = "/Downloads")
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            string filename = "";
            string destinationpath = localPath;
            if (!Directory.Exists(destinationpath))
            {
                Directory.CreateDirectory(destinationpath);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result)
            {
                string path = response.Headers["Content-Disposition"];
                if (string.IsNullOrWhiteSpace(path))
                {
                    var uri = new Uri(url);
                    filename = Path.GetFileName(uri.LocalPath);
                }
                else
                {
                    ContentDisposition contentDisposition = new ContentDisposition(path);
                    filename = contentDisposition.FileName;

                }

                var responseStream = response.GetResponseStream();
                using (var fileStream = File.Create(System.IO.Path.Combine(destinationpath, filename)))
                {
                    responseStream.CopyTo(fileStream);
                }
            }

            return Path.Combine(destinationpath, filename);
        }

        public static IDictionary<string, object> GetRouteParameters(string Parametros, string ctxString = "", object ctxValue = null, char separator = ':')
        {
            if (!string.IsNullOrEmpty(Parametros) && Parametros.Contains('|'))
                separator = '|';
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(Parametros))
            {
                List<string> list = Parametros.Split(new char[]
                {
                    separator
                }).ToList<string>();
                try
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            dictionary.Add(list[i], list[i + 1]);
                            if (i + 2 == list.Count)
                            {
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
            if (!string.IsNullOrEmpty(ctxString))
            {
                dictionary.Add(ctxString, ctxValue);
            }
            return dictionary;
        }

        public static Dictionary<int, RouteValueDictionary> ParserGridRouteValueDictionary(string[] Parametros, string gridKey)
        {
            string paramsStr = string.Empty;
            foreach (var item in Parametros)
            {
                paramsStr += item + "|";
            }
            return ParserGridRouteValueDictionary(paramsStr, gridKey);
        }

        public static Dictionary<int, RouteValueDictionary> ParserGridRouteValueDictionary(string Parametros, string gridKey)
        {
            //Criando o dicionário de 'RouteValueDictionary'
            Dictionary<int, RouteValueDictionary> selectedIDs = new Dictionary<int, RouteValueDictionary>();
            string[] DParamsGrid = Parametros.Replace("\"", "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < DParamsGrid.Length; i++)
            {
                string[] DParams = gridKey.Split(';');
                string[] DParamsValues = null;
                DParamsValues = DParamsGrid[i].Split('|');
                RouteValueDictionary routeValues = new RouteValueDictionary();
                for (int u = 0; u < DParams.Length; u++)
                {
                    var pValue = DParamsValues != null ? DParamsValues[u] : null;
                    //Retorna Parâmetro Composto
                    if (DParamsValues.Length > 1 && DParams.Length == 1)
                    {
                        string[] pValueComposite = DParamsValues != null ? DParamsValues : null;
                        routeValues.Add(DParams[u], pValueComposite);
                    }
                    else
                        routeValues.Add(DParams[u], pValue);
                    //Session[ViewData["gridName"] + CTX + "_GridKeyParams"] = routeValues;
                }
                selectedIDs.Add(i, routeValues);
            }
            return selectedIDs;
        }

        public static Dictionary<int, RouteValueDictionary> ParserGridRouteValueDictionary(Dictionary<int, RouteValueDictionary> Parametros, string gridParams, string gridKey = "gridKey")
        {
            //Criando o dicionário de 'RouteValueDictionary'
            Dictionary<int, RouteValueDictionary> selectedIDs = new Dictionary<int, RouteValueDictionary>();
            //string[] DParamsGrid = Parametros.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Parametros.Count; i++)
            {
                string[] DParams = gridParams.Split(';');
                string[] DParamsValues = null;
                DParamsValues = Parametros[i][gridKey] is string ? new string[] { (string)Parametros[i][gridKey] } : (string[])Parametros[i][gridKey]; //Parametros[i].ToString().Split('|');
                RouteValueDictionary routeValues = new RouteValueDictionary();
                for (int u = 0; u < DParams.Length; u++)
                {
                    var pValue = DParamsValues != null ? DParamsValues[u] : null;
                    //Retorna Parâmetro Composto
                    if (DParamsValues.Length > 1 && DParams.Length == 1)
                    {
                        string[] pValueComposite = DParamsValues != null ? DParamsValues : null;
                        routeValues.Add(DParams[u], pValueComposite);
                    }
                    else
                        routeValues.Add(DParams[u], pValue);
                }
                selectedIDs.Add(i, routeValues);
            }
            return selectedIDs;
        }

        public static string GetAssemblyDllName(string className)
        {
            string dll = string.Empty;
            string[] objParamsParts = className.Split('.');
            foreach (string name in objParamsParts)
            {
                if (name.ToUpper().Contains("MODEL"))
                {
                    //Remove o ultimo ponto
                    dll = dll.Substring(0, dll.Length - 1);
                    break;
                }
                dll += name + ".";
            }
            return dll;
        }

        #region Métodos genéricos de conversão (Reflection)
        public static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            // Create the result table, and gather all properties of a T        
            DataTable table = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add the properties as columns to the datatable
            foreach (var prop in props)
            {
                Type propType = prop.PropertyType;

                // Is it a nullable type? Get the underlying type 
                if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    propType = new NullableConverter(propType).UnderlyingType;

                table.Columns.Add(prop.Name, propType);
            }

            // Add the property values per T as rows to the datatable
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);

                table.Rows.Add(values);
            }

            return table;
        }

        public static DataView ToDataView<T>(IEnumerable<T> items)
        {
            // Create the result table, and gather all properties of a T        
            DataTable table = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add the properties as columns to the datatable
            foreach (var prop in props)
            {
                Type propType = prop.PropertyType;

                // Is it a nullable type? Get the underlying type 
                if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    propType = new NullableConverter(propType).UnderlyingType;

                table.Columns.Add(prop.Name, propType);
            }

            // Add the property values per T as rows to the datatable
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);

                table.Rows.Add(values);
            }

            return table.AsDataView();
        }

        public static DataView ToDataView(string assemblyName, string objName, IEnumerable items, bool createGridKey = false)
        {
            DataTable dataTable = new DataTable(objName);
            Assembly assembly = Assembly.Load(assemblyName);
            Type type = assembly.GetType(objName);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<int> list = new List<int>();
            int num = 0;
            PropertyInfo[] array = properties;
            for (int i = 0; i < array.Length; i++)
            {
                PropertyInfo propertyInfo = array[i];
                bool flag = true;
                Type type2 = propertyInfo.PropertyType;
                if (type2.IsGenericType && type2.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    type2 = new NullableConverter(type2).UnderlyingType;
                }
                if (type2.FullName.Contains("ELMAR.DevHtmlHelper.Models"))
                {
                    list.Add(num);
                    flag = false;
                }
                if (flag)
                {
                    var columnName = propertyInfo.Name;
                    var propertyAttributes = propertyInfo.GetCustomAttribute<DisplayNameAttribute>(false);
                    if (propertyAttributes != null)
                    {
                        columnName = propertyAttributes.DisplayName;
                    }
                    dataTable.Columns.Add(columnName, type2);
                }
                num++;
            }
            if (createGridKey)
            {
                dataTable.Columns.Add("gridKey", typeof(int));
            }
            num = 1;
            foreach (object current in items)
            {
                object[] array2 = new object[properties.Length - list.Count];
                if (createGridKey)
                {
                    array2 = new object[array2.Length + 1];
                }
                int num2 = 0;
                for (int j = 0; j < properties.Length; j++)
                {
                    if (!list.Contains(j))
                    {
                        array2[num2++] = properties[j].GetValue(current, null);
                    }
                }
                if (createGridKey)
                {
                    array2[num2] = num++;
                }
                dataTable.Rows.Add(array2);
            }
            return dataTable.AsDataView();
        }

        public static void CopyObjProperties(object p, object c)
        {
            PropertyInfo[] props = p.GetType().GetProperties();

            foreach (PropertyInfo pi in props)
            {
                try
                {
                    pi.SetValue(c, pi.GetValue(p));
                }
                catch { }
            }
        }

        //DEPRECATED
        public static DataView ToDataViewOld(string assemblyName, string objName, IEnumerable items, bool createGridKey = false)
        {
            // Create the result table, and gather all properties of a T        
            DataTable table = new DataTable(objName);
            Assembly assembly = Assembly.Load(assemblyName);
            Type t = assembly.GetType(objName);
            PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add the properties as columns to the datatable
            foreach (var prop in props)
            {
                Type propType = prop.PropertyType;

                // Is it a nullable type? Get the underlying type 
                if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    propType = new NullableConverter(propType).UnderlyingType;

                table.Columns.Add(prop.Name, propType);
            }
            if (createGridKey)
                table.Columns.Add("gridKey", typeof(int));

            // Add the property values per T as rows to the datatable
            int index = 1;
            foreach (var item in items)
            {
                var values = new object[props.Length];
                if (createGridKey)
                    values = new object[props.Length + 1];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                if (createGridKey)
                    values[props.Length] = index++;

                table.Rows.Add(values);
            }

            return table.AsDataView();
        }
        #endregion

        #region Json / XML / CSV / TXT Parsers
        public static string GetJSONString(string url)
        {
            string result = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                result = reader.ReadToEnd();
            }
            result = result.Replace("[[", "[").Replace("]]", "]");
            return "[" + Util.GetAllBetween(result, "[", "]")[0] + "]";
        }

        public static string GetJSONString(string url, bool useIp = false)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (useIp)
            {
                string ip = System.Net.Dns.GetHostEntry(request.RequestUri.Host).AddressList[0].ToString();
                url = url.Replace(request.RequestUri.Host, ip);
            }
            return GetJSONString(url);
        }

        public static T GetObjectFromJSONString<T>(string json) where T : new()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        public static T[] GetArrayFromJSONString<T>(string json) where T : new()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T[]));
                return (T[])serializer.ReadObject(stream);
            }
        }

        public static DataTable JsonToDataTable(string jsonString)
        {
            return (DataTable)JsonConvert.DeserializeObject(jsonString, (typeof(DataTable)));
        }

        public static DataView JsonToDataView(string jsonString)
        {
            return JsonToDataTable(jsonString).AsDataView();
        }

        public static DataTable JsonParserToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(jsonString))
            {
                return dt;
            }
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                        //throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }

        public static DataView DataViewSetFilterOrder(DataView dtView, string filter, string order)
        {
            try
            {
                //dtView = new DataView(((DataView)dtView).Table, filter, order, DataViewRowState.OriginalRows);
                dtView.RowFilter = filter;
                dtView.Sort = order;
            }
            catch { }
            return dtView;
        }

        public static object DataTableToJSON(DataTable dtTable, string method = "Default")
        {
            //method = Newtonsoft.Json.JsonConvert (auxiliar)
            if (method.Equals("Newtonsoft.Json.JsonConvert"))
            {
                return DataViewToJSON(dtTable.AsDataView(), method);
            }
            else //Default
            {
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (DataRow row in dtTable.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    int countNull = 0;
                    foreach (DataColumn col in dtTable.Columns)
                    {
                        //Não adiciona valores nulos ou vazios
                        if (Convert.ToString(row[col]).ToLower().Trim().Equals("null") || Convert.ToString(row[col]).Trim().Equals(string.Empty))
                        {
                            countNull++;
                            continue;
                        }
                        dict[col.ColumnName] = (Convert.ToString(row[col]));
                    }
                    //Verifica se a linha é nula
                    if (countNull < dtTable.Columns.Count)
                        list.Add(dict);
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = 2147483644;
                return serializer.Serialize(list);
            }
        }

        public static object DataViewToJSON(DataView dtView, string method = "Default", string tableName = "")
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = "Table";
            if (method.Equals("Newtonsoft.Json.JsonConvert"))
                return JsonConvert.SerializeObject(dtView.ToTable(tableName));
            else
                return DataTableToJSON(dtView.ToTable(tableName), method);
        }

        public static object DataTableToXML(DataTable dtTable)
        {
            using (StringWriter writer = new StringWriter())
            {
                //dtTable.TableName = name;
                XmlSerializer xml = new XmlSerializer(typeof(DataTable));
                xml.Serialize(writer, dtTable);
                return writer;
            }
        }

        public static object DataViewToXML(DataView dtView, string tableName = "")
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = "Table";
            return DataTableToXML(dtView.ToTable(tableName));
        }

        public static DataSet CsvToDataSet(string path, char separator = ';')
        {
            //Arquivo CSV
            string strFile = HttpContext.Current.Server.MapPath(path);
            //Se a primeira linha contém o nome das colunas
            bool isRowOneHeader = true;

            DataTable csvDataTable = new DataTable();
            String[] csvData = File.ReadAllLines(strFile, Encoding.Default);

            //Se o arquivo .csv não está vazio
            if (csvData.Length > 0)
            {
                int index = 0;
                foreach (var item in csvData)
                {

                    if (!item.StartsWith("#") && !string.IsNullOrEmpty(item))
                    {
                        break;
                    }
                    index++;

                }

                String[] headings = csvData[index].Split(separator);
                int intRowIndex = index;

                //Se a primeira linha contém o nome das colunas
                if (isRowOneHeader)
                {
                    for (int i = 0; i < headings.Length; i++)
                    {
                        //Adiciona colunas ao DataTable
                        csvDataTable.Columns.Add(headings[i].ToString());
                    }

                    intRowIndex++;
                }
                //Se a primeira linha não contém o nome das colunas, 
                //adiciona colunas como "Coluna1", "Coluna2", etc.
                else
                {
                    for (int i = 0; i < headings.Length; i++)
                    {
                        csvDataTable.Columns.Add("Coluna" + (i + 1).ToString());
                    }
                }

                //Popula o DataTable
                for (int i = intRowIndex; i < csvData.Length; i++)
                {
                    //Cria uma nova linha
                    DataRow row = csvDataTable.NewRow();

                    for (int j = 0; j < headings.Length; j++)
                    {
                        //Adiciona os valores de cada coluna
                        if (string.IsNullOrEmpty(csvData[i]))
                            continue;
                        row[j] = csvData[i].Split(separator)[j];
                    }

                    //Adiciona a linha ao DataTable
                    csvDataTable.Rows.Add(row);
                }
            }

            //Cria o DataSet e adiciona o DataTable nele
            DataSet myDataSet = new DataSet();
            myDataSet.Tables.Add(csvDataTable);

            return myDataSet;
        }

        public static DataView CsvToChartDataView(string csvUrl, string colArgumento, string colValue, string parserType = "", string DataTableName = "ChartDataTable")
        {
            //Montando o DataView Dinamicamente
            //string parserType = "Month";
            DataTable dtTable = CsvToDataSet(csvUrl, ',').Tables[0];
            System.Data.DataView dtViewAnalytics = new System.Data.DataView();

            dtViewAnalytics.Table = new System.Data.DataTable(DataTableName);
            //string colArgumento = "Índice do mês";
            //string colValue = "Visualizações de página";

            foreach (System.Data.DataColumn col in dtTable.Columns)
            {
                if (col.Caption.Equals(colArgumento))
                {
                    dtViewAnalytics.Table.Columns.Add(col.Caption);
                }
                if (col.Caption.Equals(colValue))
                {
                    dtViewAnalytics.Table.Columns.Add(col.Caption, typeof(float));
                }
            }

            foreach (System.Data.DataRow row in dtTable.Rows)
            {
                string argumentoValor = string.Empty;
                if (row[colArgumento].ToString() == "")
                    continue;

                //Tratamento específico
                argumentoValor = ParserInfo(parserType, row[colArgumento].ToString());

                dtViewAnalytics.Table.Rows.Add(new object[] { argumentoValor, row[colValue] });
            }

            return dtViewAnalytics;
        }

        public static string DataTableToCsv(DataView tbl)
        {
            StringBuilder strb = new StringBuilder();

            //column headers
            strb.AppendLine(string.Join(",", tbl.Table.Columns.Cast<DataColumn>()
                .Select(s => "\"" + s.ColumnName + "\"")));

            //rows
            tbl.Table.AsEnumerable().Select(s => strb.AppendLine(
                string.Join(",", s.ItemArray.Select(
                    i => "\"" + i.ToString() + "\"")))).ToList();

            return strb.ToString();
        }

        public static string DataTabletoTxt(DataView datatable, string delimited = "|", bool exportcolumnsheader = true)
        {
            StringBuilder str = new StringBuilder();
            if (exportcolumnsheader)
            {
                string Columns = string.Empty;
                foreach (DataColumn column in datatable.Table.Columns)
                {
                    Columns += column.ColumnName + delimited;
                }
                str.AppendLine(Columns.Remove(Columns.Length - 1, 1));
            }

            foreach (DataRow datarow in datatable.Table.Rows)
            {
                string row = string.Empty;
                foreach (object items in datarow.ItemArray)
                {
                    row += items.ToString() + delimited;
                }
                str.AppendLine(row.Remove(row.Length - 1, 1));
            }
            return str.ToString();
        }
        #endregion

        #region QueryDataHelpers
        public static StringBuilder AddQuerySelect(StringBuilder strBuilder, List<string> selectFields, Dictionary<string, string> Tabela_Descritor, bool DistinctValuesOnly = false, bool normalizeStr = false, Dictionary<string, string> filterValues = null)
        {
            var useAlias = true;
            selectFields = selectFields ?? new List<string>();
            if (normalizeStr)
            {
                Dictionary<string, string> Tabela_DescritorNew = new Dictionary<string, string>();
                List<string> selectFieldsNew = new List<string>();
                foreach (var item in Tabela_Descritor)
                {
                    var chave = StringNormalize(item.Key,normalizeStr);
                    Tabela_DescritorNew.Add(chave, item.Value);
                }
                Tabela_Descritor = Tabela_DescritorNew;
                foreach (var item in selectFields)
                {
                    var chave = StringNormalize(item, normalizeStr);
                    selectFieldsNew.Add(chave);
                }
                selectFields = selectFieldsNew;
            }

            if (DistinctValuesOnly)
                strBuilder.Append("select distinct ");
            else
                strBuilder.Append("select ");
            for (int index = 0; index < selectFields.Count; index++)
            {
                useAlias = true;
                string campoFisico = selectFields[index].Trim();
                if (string.IsNullOrEmpty(campoFisico))
                    continue;
                if (Tabela_Descritor != null)
                {
                    //Verifica se a chave é esperada para seleção (Seleção simples - Nome do ALIAS na consulta)
                    if (Tabela_Descritor.ContainsKey(selectFields[index].Replace("\"", "").Trim()))
                        campoFisico = Tabela_Descritor[selectFields[index].Replace("\"", "").Trim()];
                    //Verifica se o campo existe fisicamente no respectivo módulo (Seleção avançada - Nome físico na tabela)                        
                    else if (selectFields[index].Trim().Contains(" as ") && Tabela_Descritor.ContainsKey(selectFields[index].Trim().Split(' ')[0])) //Seleção avançada (Utiliza o nome do alias do descritor)
                    {
                        campoFisico = Tabela_Descritor[selectFields[index].Trim().Split(' ')[0]];
                        for (int i = 1; i < selectFields[index].Trim().Split(' ').Length; i++)
                        {
                            campoFisico += " " + selectFields[index].Trim().Split(' ')[i];
                        }
                        useAlias = false;
                    }
                    else if (selectFields[index].Trim().Contains(" as ") && Tabela_Descritor.ContainsValue(selectFields[index].Trim().Split(' ')[0])) //Seleção avançada (Utiliza o nome físico da coluna na base de dados
                    {
                        campoFisico = selectFields[index].Trim().Split(' ')[0];
                        for (int i = 1; i < selectFields[index].Trim().Split(' ').Length; i++)
                        {
                            campoFisico += " " + selectFields[index].Trim().Split(' ')[i];
                        }
                        useAlias = false;
                    }
                    else if (selectFields[index].Trim().Contains(' ')) //Seleção avançada (Utiliza o nome físico da coluna na base de dados mais o alias)
                    {
                        //Seleção avançada direta
                        campoFisico = selectFields[index].Trim();
                        useAlias = false;
                    }
                }
                else
                    throw new Exception("Seleção '" + campoFisico + "' não permitida ou inválida.");

                if (useAlias)
                {
                    //Alias Parser Corretivo
                    string aliasName = selectFields[index].Replace("\"", "").Trim();
                    strBuilder.Append(campoFisico + " as \"" + aliasName + "\"");
                }
                else
                    strBuilder.Append(campoFisico);
                if (index < selectFields.Count - 1)
                    strBuilder.Append(", ");
                else strBuilder.Append(" ");
            }
            //return strBuilder.ToString().Trim().Substring(0, strBuilder.ToString().Trim().Length-1);
            return strBuilder;
        }

        public static StringBuilder AddQueryFilters(StringBuilder strBuilder, List<string> filterFields, HttpRequestBase filtros, Dictionary<string, string> Tabela_Descritor, string Tabela, Dictionary<string, string>  filterValues = null)
        {
            //DataTable tabelaInfo = ((DataView)Core.getTabelaInfo(Tabela)).ToTable();
            for (int i = 0; i < filterFields.Count; i++)
            {
                //Dicionário para obter o nome físico do campo na base de dados a partir de seu alias no select
                string campoFiltro = filterFields[i].Trim();
                if (string.IsNullOrEmpty(campoFiltro))
                    continue;
                if (Tabela_Descritor != null &&
                    (
                    //Seleção via ALIAS
                    Tabela_Descritor.ContainsKey(filterFields[i].Trim())
                    //Seleção via Nome do Campo na Tabela  
                    || Tabela_Descritor.ContainsValue(filterFields[i].Trim())
                    )
                    )
                    campoFiltro = Tabela_Descritor.ContainsKey(filterFields[i].Trim()) ? Tabela_Descritor[filterFields[i].Trim()].Trim() : filterFields[i].Trim();
                else
                    throw new Exception("Filtro '" + filterFields[i].Trim() + "' não permitido ou inválido.");

                filterValues = filterValues ?? new Dictionary<string, string>();
                if (filtros != null && filtros.Params.Count > 0)
                {
                    foreach (var item in filtros.Params.Keys)
                    {
                        filterValues.Add(item.ToString(), filtros.Params[item.ToString()]);
                    }                        
                }

                if (!string.IsNullOrEmpty(filterValues[filterFields[i].Trim().ToString()]) || !string.IsNullOrEmpty(filterValues[filterFields[i].Trim().ToString() + "#ini"]))
                {
                    string fieldType = "character";
                    try
                    {
                        //Tenta pegar o tipo da coluna
                        fieldType = Core.GetTabelaFieldInfo(Tabela, campoFiltro).Split(' ')[0];
                    }
                    catch { }
                    switch (fieldType)
                    {
                        case "integer":
                        case "bigint":
                        case "smallint":
                        case "double":
                            if (filterValues.ContainsKey(filterFields[i].Trim().ToString() + "#ini"))
                            {
                                strBuilder.Append(String.Format(" and \"" + campoFiltro + "\" >= {0} and \"" + campoFiltro + "\" <= {1}", filterValues[filterFields[i].Trim().ToString() + "#ini"].ToUpper(), filterValues[filterFields[i].Trim().ToString() + "#end"].ToUpper()));
                            }
                            else if (filterValues.ContainsKey(filterFields[i].Trim().ToString()))
                            {
                                strBuilder.Append(String.Format(" and \"" + campoFiltro + "\" = {0} ", filterValues[filterFields[i].Trim().ToString()].ToUpper()));
                            }
                            break;
                        case "timestamp":
                        case "time":
                        case "date":
                            if (filterValues.ContainsKey(filterFields[i].Trim().ToString() + "#ini"))
                            {
                                strBuilder.Append(String.Format(" and \"" + campoFiltro + "\" between '{0}' and '{1}'", filterValues[filterFields[i].Trim().ToString() + "#ini"].ToUpper(), filterValues[filterFields[i].Trim().ToString() + "#end"].ToUpper()));
                            }
                            else if (filterValues.ContainsKey(filterFields[i].Trim().ToString()))
                            {
                                strBuilder.Append(String.Format(" and \"" + campoFiltro + "\" = '{0}' ", filterValues[filterFields[i].Trim().ToString()].ToUpper()));
                            }
                            break;
                        default:
                            if (filterValues.ContainsKey(filterFields[i].Trim().ToString() + "#ini"))
                            {
                                strBuilder.Append(String.Format(" and \"" + campoFiltro + "\" between '{0}' and '{1}'", filterValues[filterFields[i].Trim().ToString() + "#ini"].ToUpper(), filterValues[filterFields[i].Trim().ToString() + "#end"].ToUpper()));
                            }
                            else if (filterValues.ContainsKey(filterFields[i].Trim().ToString()))
                            {
                                //INFO: SIMILAR TO aceita multiplos valores '%(a|b|c)%'
                                strBuilder.Append(String.Format(" and UPPER(" + campoFiltro + ") similar to UPPER('%{0}%') ", filterValues[filterFields[i].Trim().ToString()].ToUpper()));
                            }
                            break;
                    }
                }
            }
            return strBuilder;
        }

        public static StringBuilder AddQueryCustomFilters(StringBuilder strBuilder, Dictionary<string, string> customFilters)
        {
            if (customFilters != null)
            {
                foreach (var item in customFilters)
                {
                    strBuilder.Append(String.Format(" AND {0} {1}", item.Key, item.Value));
                }
            }
            return strBuilder;
        }

        public static IEnumerable GetTabelaInfo(string Tabela)
        {
            using (var con = new FwkContexto())
            {

                StringBuilder query = new StringBuilder();
                query.Append("SELECT ");
                query.Append("    rel.nspname, rel.relname, attrs.attname, \"Type\", \"Default\",   attrs.attnotnull ");
                query.Append("FROM ( ");
                query.Append("    SELECT c.oid, n.nspname, c.relname ");
                query.Append("    FROM pg_catalog.pg_class   c ");
                query.Append("    LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace ");
                query.Append("    WHERE pg_catalog.pg_table_is_visible(c.oid)   ) rel ");
                query.Append("JOIN ( ");
                query.Append("    SELECT a.attname, a.attrelid, pg_catalog.format_type(a.atttypid,   a.atttypmod) ");
                query.Append("as \"Type\", ");
                query.Append("    (SELECT substring(d.adsrc for 128) FROM pg_catalog.pg_attrdef   d ");
                query.Append("    WHERE d.adrelid = a.attrelid AND d.adnum = a.attnum AND a.atthasdef) ");
                query.Append("as \"Default\",   ");
                query.Append("a.attnotnull, a.attnum ");
                query.Append("    FROM pg_catalog.pg_attribute a WHERE a.attnum > 0   AND NOT a.attisdropped ) ");
                query.Append("attrs ");
                query.Append("    ON (attrs.attrelid = rel.oid ) ");
                query.Append("    WHERE relname = '" + Tabela + "' ORDER BY attrs.attnum; ");

                return con.SelectionQuery(query.ToString());
            }
        }

        public static string GetTabelaFieldInfo(string Tabela, string Campo)
        {
            using (var con = new FwkContexto())
            {

                StringBuilder query = new StringBuilder();
                query.Append("SELECT ");
                query.Append("    rel.nspname, rel.relname, attrs.attname, \"Type\", \"Default\",   attrs.attnotnull ");
                query.Append("FROM ( ");
                query.Append("    SELECT c.oid, n.nspname, c.relname ");
                query.Append("    FROM pg_catalog.pg_class   c ");
                query.Append("    LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace ");
                query.Append("    WHERE pg_catalog.pg_table_is_visible(c.oid)   ) rel ");
                query.Append("JOIN ( ");
                query.Append("    SELECT a.attname, a.attrelid, pg_catalog.format_type(a.atttypid,   a.atttypmod) ");
                query.Append("as \"Type\", ");
                query.Append("    (SELECT substring(d.adsrc for 128) FROM pg_catalog.pg_attrdef   d ");
                query.Append("    WHERE d.adrelid = a.attrelid AND d.adnum = a.attnum AND a.atthasdef) ");
                query.Append("as \"Default\",   ");
                query.Append("a.attnotnull, a.attnum ");
                query.Append("    FROM pg_catalog.pg_attribute a WHERE a.attnum > 0 and a.attname = '" + Campo + "'  AND NOT a.attisdropped ) ");
                query.Append("attrs ");
                query.Append("    ON (attrs.attrelid = rel.oid ) ");
                query.Append("    WHERE relname = '" + Tabela + "' ORDER BY attrs.attnum; ");

                return ((DataView)con.SelectionQuery(query.ToString())).ToTable().Rows[0]["Type"].ToString();
            }
        }
        #endregion

        #region Hash MD5 / Crypto
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return (0 == comparer.Compare(hashOfInput, hash));
        }

        public static string Crypto(string Message, string privateKey = "")
        {
            if (!string.IsNullOrEmpty(privateKey))
            {
                string key = privateKey; // 16 bytes key
                string iv = "abcdef9876543210"; // 16 bytes IV
                string encrypted = AesEncryption.Encrypt(Message, key, iv);
                return encrypted;
            }

            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(string.IsNullOrEmpty(privateKey) ? _key : privateKey));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            try
            {
                byte[] DataToEncrypt = UTF8.GetBytes(Message);
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return Convert.ToBase64String(Results);
        }

        public static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Core.SHA512(bytes);
        }

        public static string SHA512(byte[] bytes)
        {
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        public static string Decrypt(string Message)
        {
            if (string.IsNullOrEmpty(Message))
                return string.Empty;
            //Ação corretiva para símbolo '+' passado por QueryString ' '
            Message = Message.Replace(" ", "+").Replace(".","/");
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(_key));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            try
            {
                byte[] DataToDecrypt = Convert.FromBase64String(Message);
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            catch
            {
                //Retorna o mesmo valor
                return Message;
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return UTF8.GetString(Results);
        }

        public static bool IsEncrypted(string Message)
        {
            //Ação corretiva para símbolo '+' passado por QueryString ' '
            Message = Message.Replace(" ", "+");
            return !Core.Decrypt(Message).Equals(Message) ? true : false;
        }
        #endregion                       

        /// <summary>
        /// CMD - ShellExec Module
        /// </summary>
        /// <param name="comando"></param>
        /// <returns></returns>
        public static string ExecutarCmd(string comando, string execProgram = "cmd.exe")
        {
            string saida = string.Empty;

            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = execProgram,
                    UseShellExecute = false,
                    //startInfo.Verb = "runas";
                    Arguments = comando
                };
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception)
            {
                throw;
            }

            return saida;
        }

        public static bool ExecutarCmdSSH(out string result, string server, string user, string pass, string comando, int port = 22, string privateKeyPath = "", string privateKeyPass = "")
        {
            return ExecutarCmdSSH(out result, server, user, pass, new string[] { comando }, port, privateKeyPath, privateKeyPass);
        }

        public static bool ExecutarCmdSSH(out string result, string server, string user, string pass, string[] comandos, int port = 22, string privateKeyPath = "", string privateKeyPass = "")
        {
            result = string.Empty;
            bool sucesso = true;

            /*string privateKey = System.IO.File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["pathFisico"] + @"\bin\"+privateKeyPath);
            SSH.Client.SSHClient sshCli = new SSH.Client.SSHClient(server, user, privateKey, 22);
            sshCli.Connection();
            sshCli.CommandAsync(comandos[0]);*/

            for (int i = 1; i <= 20; i++)
            {
                if (sshIsRunning)
                {
                    Thread.Sleep(3000); //Wait for 3s for execution ends
                }
                else
                {
                    try
                    {
                        //Definindo o método de autenticação
                        AuthenticationMethod[] authMethod;
                        if (!string.IsNullOrEmpty(privateKeyPath))
                        {
                            string keyStr = privateKeyPath;
                            if (!privateKeyPath.Contains("PRIVATE KEY"))
                                keyStr = File.ReadAllText(privateKeyPath);
                            var keyStream = new MemoryStream(Encoding.UTF8.GetBytes(keyStr));
                            var _key = new PrivateKeyFile(keyStream, privateKeyPass);
                            authMethod = new AuthenticationMethod[]{
                                new PasswordAuthenticationMethod(user, pass),
                                new PrivateKeyAuthenticationMethod(user, _key)                                
                            };
                        }
                        else
                        {
                            authMethod = new AuthenticationMethod[]{
                                new PasswordAuthenticationMethod(user, pass),                                
                            };
                        }
                        
                        using (var client = new SshClient(new ConnectionInfo(server, port, user, authMethod)))
                        {
                            //var allowedAuthentications = authMethod[0].AllowedAuthentications;                            
                            client.Connect();
                            sshIsRunning = true;
                            foreach (string comando in comandos)
                            {                                
                                var output = client.RunCommand(comando);
                                result += output.Result + Environment.NewLine;
                                sucesso = true;                  
                            }
                            client.Disconnect();
                            sshIsRunning = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message; //Variável de retorno OUT
                        sucesso = false;     //Flag para indicar a falha na execução
                                             //throw ex;
                        sshIsRunning = false;
                    }
                    break; //Executado
                } //If Not Running
            } //For Retry 20 times

            return sucesso;
        }

        /// <summary>
        /// Método utilizado para consumir serviços Rest
        /// </summary>
        /// <param name="url">Url do serviço</param>
        /// <param name="value">Valor</param>
        /// <param name="action">Ação: GET POST PUT DELETE</param>
        /// <returns></returns>
        public static HttpResponseMessage HttpRequestRest(out string result, string url, string value, string action, string token = "", bool isJson = true)
        {
            result = string.Empty;
            using (var http = new HttpClient())
            {
                var content = new StringContent(value);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                if (isJson)
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (!string.IsNullOrEmpty(token))
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var request = http.GetAsync(url);

                if (action.Equals("GET"))
                {
                    url += "/" + value; //Get Action
                    request = http.GetAsync(url);
                }
                if (action.Equals("DELETE"))
                {
                    url += "/" + value; //Delete Action
                    request = http.DeleteAsync(url);
                }
                else if (action.Equals("POST"))
                {
                    request = http.PostAsync(url, content);
                }
                else if (action.Equals("PUT"))
                {
                    request = http.PutAsync(url, content);
                }
                result = request.Result.Content.ReadAsStringAsync().Result;
                return request.Result;
            }            
        }

        public static async Task<string> HttpRequestRest(string url, string value, string action, string token = "", bool isJson = true)
        {
            string result = string.Empty;
            using (var http = new HttpClient())
            {
                var content = new StringContent(value);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                if (isJson)
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (!string.IsNullOrEmpty(token))
                {
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var request = await http.GetAsync(url);

                if (action.Equals("GET"))
                {
                    url += "/" + value; //Get Action
                    request = await http.GetAsync(url);
                }
                if (action.Equals("DELETE"))
                {
                    url += "/" + value; //Delete Action
                    request = await http.DeleteAsync(url);
                }
                else if (action.Equals("POST"))
                {
                    request = await http.PostAsync(url, content);
                }
                else if (action.Equals("PUT"))
                {
                    request = await http.PutAsync(url, content);
                }
                result = request.Content.ReadAsStringAsync().Result;
            }

            return result;
        }

        public static string HttpRequestRestForm(string baseApiUrl, string values, string action, string token = "")
        {
            string Result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseApiUrl);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers["Authorization"] = "Bearer " + token;
                request.PreAuthenticate = true;
            }
            request.Method = action;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(values);

            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = byte1.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                Result = reader.ReadToEnd();
            }
            return Result;
        }

        public static void SaveLog(string msg, string id = "")
        {
            id = ((!string.IsNullOrEmpty(id)) ? ("-" + id) : string.Empty);
            string path = System.Web.HttpContext.Current.Server.MapPath(string.Concat(new string[]
            {
                "~/App_Data/log",
                id,
                "-",
                DateTime.Now.ToString("ddMMyyyy"),
                ".txt"
            }));
            if (!File.Exists(path))
            {
                using (File.Create(path))
                {
                }
            }
            using (StreamWriter streamWriter = File.AppendText(path))
            {
                string text = string.Empty;
                string str = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToString();
                text = "#############################################################\r\n";
                text = text + "Data:" + DateTime.Now.ToString("yyyyMMdd-HH:mm:ss") + "\r\n";
                text = text + "URL:" + str + "\r\n";
                text += msg;
                text += "\r\n\r\n";
                streamWriter.Write(text);
                streamWriter.Close();
            }
        }

        public static Image ResizeImage(Image sourceImage, int maxWidth, int maxHeight)
        {
            //Calculam-se as dimensões novas para a imagem
            var ratioX = 0.0;
            var ratioY = 0.0;
            if (sourceImage.Height / sourceImage.Width >= 1) //Imagem Retrato - Inverte atributos
            {
                ratioX = (double)maxWidth / sourceImage.Width;
                ratioY = (double)maxHeight / sourceImage.Height;
            }
            else
            { //Inverte os atributos para imedir a rotação da imagem
                ratioX = (double)maxWidth / sourceImage.Height;
                ratioY = (double)maxHeight / sourceImage.Width;
            }
            var ratio = Math.Min(ratioX, ratioY);
            var newWidth = (int)(sourceImage.Width * ratio);
            var newHeight = (int)(sourceImage.Height * ratio);

            //Cria-se um bitmap com as dimensões otimizadas
            var newImage = new Bitmap(newWidth, newHeight);
            //Desenha-se a imagem no bitmap
            using (var grafico = Graphics.FromImage(newImage))
            {
                grafico.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grafico.SmoothingMode = SmoothingMode.HighQuality;
                grafico.PixelOffsetMode = PixelOffsetMode.HighQuality;
                grafico.CompositingQuality = CompositingQuality.HighQuality;
                grafico.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
            };
            return newImage;
        }

        public static string SaveImage(object file, HttpServerUtilityBase Server, string name, int Largura = 150, int Altura = 200, bool deleteOld = true, string pathImages = "~/Images", bool createThumbnail = true, int LarguraThumb = 50, int AlturaThumb = 60)
        {
            bool directoryOk = CreateDirectory(pathImages);
            if (((HttpPostedFileBase[])file)[0] != null && ((HttpPostedFileBase[])file)[0].ContentLength > 0 && directoryOk)
            {
                var fileName = Path.GetFileName(((HttpPostedFileBase[])file)[0].FileName);
                string[] fileParts = ((HttpPostedFileBase[])file)[0].FileName.Split('.');
                var path = Path.Combine(Server.MapPath(pathImages), fileName);
                ((HttpPostedFileBase[])file)[0].SaveAs(path);
                Image image = Image.FromFile(path);
                //Tenta remover a imagem enviada
                if (deleteOld)
                {
                    try
                    {
                        System.IO.File.Delete(path);
                    }
                    catch { }
                }
                //Salva a imagem criada
                Image newImage = Core.ResizeImage(image, Largura, Altura);
                Image newImageThumb = Core.ResizeImage(image, LarguraThumb, AlturaThumb);
                try
                {
                    newImage.Save(Path.Combine(Server.MapPath(pathImages), name + "." + fileParts[fileParts.Length - 1]));
                    if (createThumbnail)
                    {
                        newImageThumb.Save(Path.Combine(Server.MapPath(pathImages), name + "_thumb." + fileParts[fileParts.Length - 1]));
                    }
                }
                catch { }
                image.Dispose(); //Remove o objeto da memória
                return pathImages.Replace("~", "") + "/" + name + "." + fileParts[fileParts.Length - 1];
            }
            return string.Empty;
        }        

        public static bool CreateDirectory(string path)
        {
            path = path.Replace("~", FwkConfig.GetSettingValue("pathFisico")).Replace("/", "\\");
            try
            {
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                    return true;
                }
                return true;
            }
            catch { return false; }
        }

        public static string CreateCookie(HttpContextBase Context, string value, string key = "CTX")
        {
            var mobile = Util.IsMobileDevice(Context.Request) ? "mobile" : "";
            string cookieName = Util.ConvertToAlphaNumeric(ConfigurationManager.AppSettings["appName"]) + "_Cookie" + mobile;
            //key = !(key.Contains(cookieName)) ? cookieName + "_" + key : key;            

            HttpCookie myCookie = null;
            bool exists = false;

            if (Context.Request.Cookies[cookieName] != null)
            { //Cria o cookie
                myCookie = Context.Request.Cookies[cookieName];
                exists = true;
            }
            else
                myCookie = new HttpCookie(cookieName);

            // setting cookie
            myCookie.SameSite = SameSiteMode.None;
            myCookie.HttpOnly = false;
            myCookie.Secure = true;
            myCookie.Expires = DateTime.Now.AddMinutes(30);
            //Add key-values in the cookie
            myCookie.Values.Remove(key);
            myCookie.Values.Add(key, value);

            if(!exists)
                Context.Response.Cookies.Add(myCookie);
            else
                Context.Response.Cookies.Set(myCookie);

            return value;
        }

        public static string CreateCookie(HttpContext HttpContext, string value, string key = "CTX")
        {
            return CreateCookie(new HttpContextWrapper(HttpContext), value, key);
        }

        public static string GetSetCookie(HttpContext HttpContext, string value = "", string key = "CTX", bool useDefaultValue = false)
        {
            return GetSetCookie(new HttpContextWrapper(HttpContext), value, key, useDefaultValue);
        }

        public static string GetSetCookie(HttpContextBase Context, string Valor = "", string key = "CTX", bool useDefaultValue = false)
        {
            //return GetSetCTX_LocalStorage(Valor, key, useDefaultValue);
            Valor = string.IsNullOrEmpty(Valor) ?
                Context.Request["ctx"] : Valor;

            Valor = string.IsNullOrEmpty(Valor) ?
                Context.Request["e"] : Valor;

            Context.Session["CTX"] = Valor;

            return Valor;

            // Código em desuso 
            /*string currentValor = string.Empty;
            try
            {
                var mobile = Util.IsMobileDevice(Context.Request) ? "mobile" : "";

                string cookieName = Util.ConvertToAlphaNumeric(ConfigurationManager.AppSettings["appName"]) + "_Cookie" + mobile;
                //key = !key.Contains(cookieName) ? cookieName + "_" + key : key;

                currentValor = Context.Request.Cookies[cookieName] != null
                    && Context.Request.Cookies[cookieName].Values[key] != null ?
                    Context.Request.Cookies[cookieName].Values[key] : string.Empty;

                if (!string.IsNullOrEmpty(Valor) && !Valor.Equals(currentValor))
                {
                    // store any object
                    currentValor = CreateCookie(Context, Valor, key);
                }

                Context.Session[key] = currentValor;
            }
            finally {
                if (!string.IsNullOrEmpty(Valor))
                {
                    // store any object
                    Context.Session[key] = Valor;
                    currentValor = Valor;
                }
            }

            return currentValor;
            */
        }

        public static string GetSetCTX_LocalStorage(String ctx = null, string Key = "CTX", bool useDefaultCTX = false)
        {
            string clientID = Util.ConvertToAlphaNumeric(Util.GetIPAddress());
            Key = !Key.Contains(clientID) ? clientID + Key : Key;
            
            var storage = new LocalStorage();

            string currentCTX = storage.Exists(Key) ? storage.Get<string>(Key) : string.Empty;

            if (!String.IsNullOrEmpty(ctx) && !ctx.Equals(currentCTX))
            {
                // store any object
                storage.Store<string>(Key, ctx);
                currentCTX = ctx;
                // finally, persist the stored objects to disk (.localstorage file)
                storage.Persist();
            }
            
            string defaultCTX = (useDefaultCTX) ? FwkConfig.GetSettingValue("defaultCTX") : string.Empty;
            if (string.IsNullOrEmpty(ctx) && !string.IsNullOrEmpty(defaultCTX))
            {
                return GetSetCTX_LocalStorage(defaultCTX, Key, false);
            }

            return currentCTX;             
        }

        public static object GetSetLocalStorage(string Key, object Valor = null, bool global = false)
        {
            if (!global) {
                string clientID = Util.ConvertToAlphaNumeric(Util.GetIPAddress());
                Key = !Key.Contains(clientID) ? clientID + Key : Key;
            }

            var storage = new LocalStorage();

            var currentValor = storage.Exists(Key) ? storage.Get(Key) : null;

            if (Valor != null && !Valor.Equals(currentValor))
            {
                // store any object
                var json = JsonConvert.SerializeObject(Valor, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                storage.Store(Key, json);
                currentValor = Valor;
                // finally, persist the stored objects to disk (.localstorage file)
                storage.Persist();
            }

            return currentValor;
            
        }

        public static string GetSetCTX(HttpContextBase Context, String ctx = "", string key = "CTX", bool useDefaultCTX = false)
        {
            //Prioridade ao contexto definido com base no parâmetro hostCTX, 
            //com está configuração ativa não é possível a troca de contexto para este host            
            string hostCTX = FwkConfig.GetSettingValue(Context.Request.Url.Host);
            if (!string.IsNullOrEmpty(hostCTX))
            {
                ctx = hostCTX;
            }            
            
            string defaultCTX = (useDefaultCTX) ? FwkConfig.GetSettingValue("defaultCTX") : string.Empty;
            if (string.IsNullOrEmpty(ctx) && !string.IsNullOrEmpty(defaultCTX))
            {
                ctx = defaultCTX;
            }

            return Core.GetSetCookie(Context, ctx, key, useDefaultCTX);
        }

        public static string GetSetCTX(HttpContext HttpContext, String ctx = "", string key = "CTX", bool useDefaultCTX = true)
        {
            return GetSetCTX(new HttpContextWrapper(HttpContext), ctx, key, useDefaultCTX);
        }

        //Método auxiliar para pegar os IDs selecionados a partir do GridView
        public static Dictionary<int, RouteValueDictionary> GetSelectedIDs(HttpSessionStateBase Session, TempDataDictionary TempData = null, string gridKey = "")
        {
            Dictionary<int, RouteValueDictionary> SelectedIDs = TempData != null && TempData["selectedIDs"] != null ? (Dictionary<int, RouteValueDictionary>)TempData["selectedIDs"] : null;
            return (SelectedIDs == null && (Session != null && Session["sessSelectedIDs"] != null)) ? (Dictionary<int, RouteValueDictionary>)Session["sessSelectedIDs"] : SelectedIDs;
        }

        public static string StringNormalize(string str, bool normalize = false)
        {
            if (!normalize) return str;

            /** Troca os caracteres acentuados por não acentuados **/
            string[] acentos = new string[] { "ç", "Ç", "á", "é", "í", "ó", "ú", "ý", "Á", "É", "Í", "Ó", "Ú", "Ý", "à", "è", "ì", "ò", "ù", "À", "È", "Ì", "Ò", "Ù", "ã", "õ", "ñ", "ä", "ë", "ï", "ö", "ü", "ÿ", "Ä", "Ë", "Ï", "Ö", "Ü", "Ã", "Õ", "Ñ", "â", "ê", "î", "ô", "û", "Â", "Ê", "Î", "Ô", "Û" };
            string[] semAcento = new string[] { "c", "C", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "Y", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U", "a", "o", "n", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "A", "O", "N", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U" };

            for (int i = 0; i < acentos.Length; i++)
            {
                str = str.Replace(acentos[i], semAcento[i]);
            }
            /** Troca os caracteres especiais da string por "" **/
            string[] caracteresEspeciais = { "¹", "²", "³", "£", "¢", "¬", "º", "¨", "\"", "'", ".", ",", "-", ":", "(", ")", "ª", "|", "\\\\", "°", "@", "#", "!", "$", "%", "&", "*", ";", "/", "<", ">", "?", "[", "]", "{", "}", "=", "+", "§", "´", "`", "^", "~" };
            //Removido '_'

            for (int i = 0; i < caracteresEspeciais.Length; i++)
            {
                str = str.Replace(caracteresEspeciais[i], "");
            }

            /** Troca os caracteres especiais da string por " " **/
            str = Regex.Replace(str, @"[^\w\.@-]", " ",
                                RegexOptions.None, TimeSpan.FromSeconds(1.5));

            str = str.Replace(" ", "_");

            return str.ToLower().Trim();
        }
    }

    public class AesEncryption
    {
        public static string Encrypt(string plainText, string key, string iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }
    }
}