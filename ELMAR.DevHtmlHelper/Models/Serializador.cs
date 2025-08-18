using Newtonsoft.Json;
using System;
using System.IO;
using System.Web.Routing;
using System.Xml.Serialization;

namespace ELMAR.DevHtmlHelper.Models
{
    public class Serializador
    {
        /// <summary>
        /// Método para serializar um objeto em XML
        /// </summary>
        /// <param name="_Objeto">Objeto a ser serializado</param>
        /// <returns></returns>
        public static string Serializar(object _Objeto)
        {
            StringWriter writer = new StringWriter();
            Type type = _Objeto.GetType();
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                serializer.Serialize(writer, _Objeto);
            }
            catch (Exception e) { /*throw e;*/ }            

            return writer.ToString();
        }

        public static string SerializarToJson(object _Objeto)
        {
            return JsonConvert.SerializeObject(_Objeto);
        }

        public static object DeserializarJson(string json, JsonConvertTypes type)
        {
            switch (type)
	        {
		        case JsonConvertTypes.RouteValueDictionary:
                    return JsonConvert.DeserializeObject<RouteValueDictionary>(json);                
                default:
                    return JsonConvert.DeserializeObject(json);                
	        }            
        }

        /// <summary>
        /// Método para Deserializar um XML para um tipo de objeto
        /// </summary>
        /// <param name="xml">XML a ser deserializado</param>
        /// <param name="type">Tipo do objeto de retorno</param>
        /// <returns></returns>
        public static object Deserializar(string xml, Type type)
        {
            StringReader reader = new StringReader(xml);
            XmlSerializer serializer = new XmlSerializer(type);

            return serializer.Deserialize(reader);
        }
    }

    public enum JsonConvertTypes { RouteValueDictionary }
}