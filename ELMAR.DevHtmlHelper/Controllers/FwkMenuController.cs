using ELMAR.DevHtmlHelper.Models;
using ELMAR.DevHtmlHelper.Models.CustomValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ELMAR.DevHtmlHelper.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FwkMenuController : ApiController
    {
        private readonly FwkContexto _contextoFwk = new FwkContexto();
        
        public List<Fwk_MenuItem> Get(string id = "") //Get by Contexto
        {
            //Decripta as variáveis informadas
            var lstMenu = _contextoFwk.Menus.AsNoTracking()
                .Where(m => m.Contexto.Equals(id)).ToList();            

            return lstMenu;
        }

        [IFWAuthorizeApi]
        public HttpResponseMessage PostFwkMenuItens(List<Fwk_MenuItem> fwk_MenuItemLst)
        {
            if (fwk_MenuItemLst == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Lista Vazia ou Formato Inválido");

            foreach (var fwk_menuitem in fwk_MenuItemLst)
            {
                var encontrado = _contextoFwk.Menus.Find(
                    new object[] { fwk_menuitem.ID, fwk_menuitem.Contexto });

                if (encontrado == null)
                {
                    fwk_menuitem.Pai = null;
                    _contextoFwk.Menus.Add(fwk_menuitem);
                }
                else
                {
                    _contextoFwk.Entry(encontrado).CurrentValues.SetValues(fwk_menuitem);
                }
            }
            try
            {
                //Commit
                _contextoFwk.SaveChanges();
                if(Request != null)
                    return Request.CreateResponse(HttpStatusCode.Created, "Sucesso");
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.Created, ReasonPhrase = "Sucesso" };
            }
            catch (Exception ex)
            {
                if(Request != null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = ex.Message };
            }
        }

        [IFWAuthorizeApi]
        public HttpResponseMessage DeleteFwkMenuItens(string ID, string Contexto, bool DeleteChild)
        {
            if (DeleteChild)
            {
                _contextoFwk.Menus.RemoveRange(_contextoFwk.Menus.Where(x => (x.ID == ID || x.PaiID == ID) && x.Contexto == Contexto));
            }
            else
            {
                var Fwk_MenuItem = _contextoFwk.Menus.Find(ID, Contexto);
                if (Fwk_MenuItem == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Item de Menu não localizado!");
                _contextoFwk.Menus.Remove(Fwk_MenuItem);
            }
            try
            {
                //Commit
                _contextoFwk.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Sucesso");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [IFWAuthorizeApi]
        public HttpResponseMessage DeleteFwkMenuContexto(string Contexto)
        {
            _contextoFwk.Menus.RemoveRange(_contextoFwk.Menus.Where(x => x.Contexto == Contexto));
            try
            {
                //Commit
                _contextoFwk.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Sucesso");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
