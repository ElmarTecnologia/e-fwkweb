using ELMAR.DevHtmlHelper.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Elmar.WebServiceRest.Controllers
{
    public class SmsAndroidController : Controller
    {
        //
        // GET: /SmsAndroid/

        public async Task<ActionResult> Index(bool debug=false)
        {
            string urlRest = "http://intranet.elmartecnologia.com.br/efwk_wsrest/api/sms/0"; //Pega mensagens não lidas
            ViewBag.Json = Core.GetJSONString(urlRest);
            List<SmsMobile> smsList = Core.GetObjectFromJSONString<List<SmsMobile>>(ViewBag.Json);
            foreach (var item in smsList)
            {
                //Atualiza a mensagem para lida
                item.Status = debug ? 0 : 1;
                string json = "["+JsonConvert.SerializeObject(item)+"]";
                await Core.HttpRequestRest(urlRest, json, "POST");                
            }
            ViewBag.debug = debug;
            return View(smsList);
        }

        //Executa POST na aplicação atualizando a listagem de mensagens enviadas


    }
}
