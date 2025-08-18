using System;
using System.Data;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ELMAR.DevHtmlHelper.Models
{
    public class Fwk_JsonResult : ActionResult
    {
        public Fwk_JsonResult() { }
        public Fwk_JsonResult(object data) { this.Data = data; }

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
                response.ContentType = "application/json";

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data is DataView || this.Data is DataTable)
            {
                //Método 1 (System.Data)
                object json = (this.Data is DataView) ? Core.DataViewToJSON((DataView)this.Data) : Core.DataTableToJSON((DataTable)this.Data);
                response.Write(json);
            }
            else
            {
                //Método 2 (Objetos)
                DataContractJsonSerializer serializer =
                  new DataContractJsonSerializer(this.Data.GetType());
                serializer.WriteObject(response.OutputStream, this.Data);
            }
        }
    }
}