using System;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ELMAR.DevHtmlHelper.Models
{
    public class Fwk_XmlResult : ActionResult
    {
        public Fwk_XmlResult() { }
        public Fwk_XmlResult(object data) { this.Data = data; }

        public string ContentType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public object Data { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
                response.ContentType = this.ContentType;
            else
                response.ContentType = "text/plain";

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null)
            {
                if (this.Data is XmlNode)
                    response.Write(((XmlNode)this.Data).OuterXml);
                else if (this.Data is XNode)
                    response.Write(((XNode)this.Data).ToString());
                else
                {
                    //if (this.Data is DataView)
                    //    this.Data = ((DataView)this.Data).ToTable();
                    var dataType = this.Data.GetType();
                    // OMAR: For generic types, use DataContractSerializer because 
                    // XMLSerializer cannot serialize generic interface lists or types.
                    if (dataType.IsGenericType ||
                      dataType.GetCustomAttributes(typeof(DataContractAttribute),
                      true).FirstOrDefault() != null)
                    {
                        var dSer = new DataContractSerializer(dataType);
                        dSer.WriteObject(response.OutputStream, this.Data);
                    }
                    else
                    {
                        if (this.Data is DataView || this.Data is DataTable)
                        {
                            //Método 1 (System.Data)
                            object xml = this.Data is DataView ? Core.DataViewToXML((DataView)this.Data) : Core.DataTableToXML((DataTable)this.Data);
                            response.Write(xml);
                        }
                        else
                        {
                            //Método 2 (Objetos)
                            var xSer = new XmlSerializer(dataType);
                            xSer.Serialize(response.OutputStream, this.Data);
                        }
                    }
                }
            }
        }
    }
}