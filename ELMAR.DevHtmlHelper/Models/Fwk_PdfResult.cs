using ELMAR.DevHtmlHelper.Controllers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ELMAR.DevHtmlHelper.Models
{
    public class Fwk_PdfResult : ViewResult
    {
        public Fwk_PdfResult() { }
        public Fwk_PdfResult(object data) { this.Data = data; }
        public Fwk_PdfResult(string viewName, ViewDataDictionary viewData, object Model)
        {
            this.ViewName = viewName;
            this.ViewData = viewData;
            this.Data = Model;
        }

        public string ContentType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public object Data { get; set; }
        //Utiliza a ViewName da classe base
        //public string ViewName { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
                response.ContentType = this.ContentType;
            else
                response.ContentType = "application/pdf";

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            String htmlText = new FwkController().RenderRazorViewToString(ViewName, context, ViewData, this.Data);

            Document document = new Document();

            string filePath = HostingEnvironment.MapPath("~/Uploads/");

            PdfWriter.GetInstance(document, new FileStream(filePath + "\\pdf-" + ViewName.ToLower() + ".pdf",
            FileMode.Create));

            document.Open();

            //TODO: Atualizar para  XmlWorkerHelper DLL
            iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

            hw.Parse(new StringReader(htmlText));

            document.Close();

            response.Write(document);
        }        
    }
}
