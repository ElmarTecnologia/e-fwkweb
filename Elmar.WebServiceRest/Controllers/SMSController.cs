using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Elmar.WebServiceRest
{
    public class SMSController : ApiController    {
        //Acesso ao banco
        private readonly WSContexto _contexto = WSContexto.getAppContexto;        
        
        /// <summary>
        /// GET api/sms/0 [ 0 = não enviados / 1 = enviados / -1 = todos] 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>      
        //[IFWAuthorizeFilter]
        public List<SmsMobile> Get(int id)
        {
            List<SmsMobile> smsMobile = new List<SmsMobile>();
            if (id == -1)
                smsMobile = (from s in _contexto.Sms select s).ToList<SmsMobile>();
            else
                smsMobile = (from s in _contexto.Sms where s.Status == id select s).ToList<SmsMobile>();
            return smsMobile;
        }
        
        // POST api/sms (Insert / Update dinâmico) - Inserção múltipla ( Padrão json => [{reg:1},{reg:2}] )
        // Json para testes:
        // [{"Codigo":0,"Numero":"83986224082","Titulo":"Teste","Conteudo":"Testando sistema de mensagens","Status":0}]
        // [{\"Codigo\":0,\"Numero\":\"83986224082\",\"Titulo\":\"Teste\",\"Conteudo\":\"Testando sistema de mensagens\",\"Status\":0}]
        public HttpResponseMessage PostSms(List<SmsMobile> smsList) {
            if(smsList == null)
                return Request.CreateResponse(HttpStatusCode.Created, "Lista Vazia ou Formato Inválido");

            foreach (var smsMobile in smsList)
            {
                if (string.IsNullOrEmpty(smsMobile.Conteudo))
                    continue;

                var encontrado = _contexto.Sms.Find(smsMobile.Codigo);
                if (encontrado == null || smsMobile.Codigo == 0)
                    _contexto.Sms.Add(smsMobile);
                else
                    _contexto.Entry(encontrado).CurrentValues.SetValues(smsMobile);
            }
            try
            {
                //Commit
                _contexto.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.Created, "Sucesso");                
            }
            catch (Exception ex)
            {
                throw ex;
            }                                       
        }

        /// <summary>
        /// DELETE api/sms/1, onde 1 = Código do registro 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage DeleteSms(string id)
        {
            SmsMobile smsMobile = _contexto.Sms.Find(int.Parse(id));
            if (smsMobile == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Registro não encontrado.");
            }

            _contexto.Sms.Remove(smsMobile);
 
            try
            {
                _contexto.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, smsMobile);
        }

        /// <summary>
        /// GET api/sms (Retorno em Json - Método alternativo) 
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetJson()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            List<SmsMobile> listSms = this.Get(-1);
            try
            {
                response.Content = new StringContent(JsonConvert.SerializeObject(listSms), Encoding.UTF8, "application/json");
            }
            catch { }
            return response;
        }

        /// <summary>
        /// Método para remover da memória após execução
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _contexto.Dispose();
            base.Dispose(disposing);
        }

        /*// POST api/sms (Insert / Update dinâmico) - Inserção individual - Método inativo    
        public HttpResponseMessage PostSms(SmsMobile smsMobile)
        {
            try
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(smsMobile.Conteudo))
                {
                    var encontrado = _contexto.Sms.Find(smsMobile.Codigo);
                    if(encontrado == null)
                        _contexto.Sms.Add(smsMobile);
                    else
                        _contexto.Entry(encontrado).CurrentValues.SetValues(smsMobile);
                    //Commit
                    _contexto.SaveChanges();
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, smsMobile);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = smsMobile.Codigo }));
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Formato Inválido");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }*/
    }
}