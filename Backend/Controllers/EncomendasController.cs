using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DashboardIntegration.Lib_Primavera.Model;
namespace DashboardIntegration.Controllers
{
    public class EncomendasController : ApiController
    {
        // POST api/encomendas
        public IEnumerable<Lib_Primavera.Model.Encomenda> Post([FromBody] Lib_Primavera.Model.Login login)
        {
            //return Lib_Primavera.PriIntegration.ListaEncomendas(login.company, login.user, login.password);
            return DashboardIntegration.Parser.StagingAPI.ListaEncomendas();
        }

        // POST api/encomendas/5    
        public Encomenda Post(string id, [FromBody] Lib_Primavera.Model.Login login)
        {
            //Lib_Primavera.Model.Encomenda enc = Lib_Primavera.PriIntegration.GetEncomenda(id, login.company, login.user, login.password);
            Lib_Primavera.Model.Encomenda enc = DashboardIntegration.Parser.StagingAPI.GetEncomenda(id);
            if (enc == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));

            }
            else
            {
                return enc;
            }
        }

        //POST api/encomendas/filtrarEncomendas/<Pendentes || Entregues>
        public IEnumerable<Lib_Primavera.Model.Encomenda> filtrarEncomendas(string type, [FromBody] Lib_Primavera.Model.Login login)
        {
            if (type.Equals("Pendentes"))
            {
                //return Lib_Primavera.PriIntegration.ListaEncomendasFiltrado('P',login.company, login.user, login.password);
                return DashboardIntegration.Parser.StagingAPI.ListaEncomendasFiltrado('P');
            }
            else if(type.Equals("Entregues"))
            {
                //return Lib_Primavera.PriIntegration.ListaEncomendasFiltrado('T',login.company, login.user, login.password);
                return DashboardIntegration.Parser.StagingAPI.ListaEncomendasFiltrado('T');
            }
            else
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));
            }
        }
        public static int getTotalEncomendas(int m)
        {
            return DashboardIntegration.Parser.StagingAPI.getTotalEncomendas(m);
        }
    }
}
