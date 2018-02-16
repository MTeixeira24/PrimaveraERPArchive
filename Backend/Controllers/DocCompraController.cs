using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DashboardIntegration.Lib_Primavera.Model;
using DashboardIntegration.Lib_Primavera;

namespace DashboardIntegration.Controllers
{
    public class DocCompraController : ApiController
    {


        public IEnumerable<Lib_Primavera.Model.DocCompra> Get()
        {
            //return Lib_Primavera.PriIntegration.VGR_List();
            return DashboardIntegration.Parser.StagingAPI.VGR_List();
        }

        public IEnumerable<Lib_Primavera.Model.DocCompra> Post([FromBody] Lib_Primavera.Model.Login login)
        {
            //return Lib_Primavera.PriIntegration.VGR_List(login.company, login.user, login.password);
            return DashboardIntegration.Parser.StagingAPI.VGR_List();
        }

        
        public Lib_Primavera.Model.DocCompra Post(string id,[FromBody] Lib_Primavera.Model.Login login)
        {
            //Lib_Primavera.Model.DocCompra doccompra = Lib_Primavera.PriIntegration.GetCompra(id,login.company,login.user,login.password);
            Lib_Primavera.Model.DocCompra doccompra = DashboardIntegration.Parser.StagingAPI.GetCompra(id);
            if (doccompra == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));

            }
            else
            {
                return doccompra;
            }
        }

        public static double getTotalMensal(int type)
        {
            List<DocCompra> mensal = DashboardIntegration.Parser.StagingAPI.VGR_List(type);
            double total = 0;
            foreach (DocCompra c in mensal)
            {
                foreach (LinhaDocCompra l in c.LinhasDoc)
                {
                    total += l.TotalLiquido;
                }
            }
            return -1*(total);
        }
         /*
        public HttpResponseMessage Post(Lib_Primavera.Model.DocCompra dc)
        {
            Lib_Primavera.Model.RespostaErro erro = new Lib_Primavera.Model.RespostaErro();
            erro = Lib_Primavera.PriIntegration.VGR_New(dc);

            if (erro.Erro == 0)
            {
                var response = Request.CreateResponse(
                   HttpStatusCode.Created, dc.id);
                string uri = Url.Link("DefaultApi", new { DocId = dc.id });
                response.Headers.Location = new Uri(uri);
                return response;
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }*/

    }
}
