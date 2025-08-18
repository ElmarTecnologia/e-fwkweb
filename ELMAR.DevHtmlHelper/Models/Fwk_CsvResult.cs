using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ELMAR.DevHtmlHelper.Models
{
    public class Fwk_CsvResult : ActionResult
    {
        public Fwk_CsvResult() { }
        public Fwk_CsvResult(object data) { this.Data = data; }

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

            if (this.Data is DataView || this.Data is DataTable)
            {
                //Método 1 (System.Data)
                object csv = (this.Data is DataView) ? Core.DataTableToCsv((DataView)this.Data) : Core.DataTableToCsv(((DataTable)this.Data).AsDataView());
                response.Write(csv);
            }
        }
    }
}