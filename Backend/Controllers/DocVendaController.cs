using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DashboardIntegration.Lib_Primavera.Model;


namespace DashboardIntegration.Controllers
{
    public class DocVendaController : ApiController
    {
        //
        // GET: /Clientes/

        public IEnumerable<Lib_Primavera.Model.DocVenda> Get()
        {
            //return Lib_Primavera.PriIntegration.Encomendas_List();
            return DashboardIntegration.Parser.StagingAPI.Encomendas_List();
        }


        // GET api/cliente/5    
        public Lib_Primavera.Model.DocVenda Get(string id)
        {
            //Lib_Primavera.Model.DocVenda docvenda = Lib_Primavera.PriIntegration.Encomenda_Get(id);
            Lib_Primavera.Model.DocVenda docvenda = DashboardIntegration.Parser.StagingAPI.Encomenda_Get(id);
            if (docvenda == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));

            }
            else
            {
                return docvenda;
            }
        }

        public IEnumerable<Lib_Primavera.Model.DocVenda> Post([FromBody] Lib_Primavera.Model.Login login)
        {
            //return Lib_Primavera.PriIntegration.Encomendas_List(login.company, login.user, login.password);
            return DashboardIntegration.Parser.StagingAPI.Encomendas_List();
        }
        public Lib_Primavera.Model.DocVenda Post(string id, [FromBody] Lib_Primavera.Model.Login login)
        {
            //Lib_Primavera.Model.DocVenda docvenda = Lib_Primavera.PriIntegration.Encomenda_Get(id,login.company, login.user, login.password);
            Lib_Primavera.Model.DocVenda docvenda = DashboardIntegration.Parser.StagingAPI.Encomenda_Get(id);
            if (docvenda == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));

            }
            else
            {
                return docvenda;
            }
        }
        public static double getTotalMensal(int type)
        {
            List<DocVenda> mensal = DashboardIntegration.Parser.StagingAPI.Encomendas_List(type);
            double total = 0;
            foreach (DocVenda c in mensal)
            {
                total += c.NetTotal;
            }
            return (total);
        }


        /*public HttpResponseMessage Post(Lib_Primavera.Model.DocVenda dv)
        {
            Lib_Primavera.Model.RespostaErro erro = new Lib_Primavera.Model.RespostaErro();
            erro = Lib_Primavera.PriIntegration.Encomendas_New(dv);

            if (erro.Erro == 0)
            {
                var response = Request.CreateResponse(
                   HttpStatusCode.Created, dv.id);
                string uri = Url.Link("DefaultApi", new {DocId = dv.id });
                response.Headers.Location = new Uri(uri);
                return response;
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }


        public HttpResponseMessage Put(int id, Lib_Primavera.Model.Cliente cliente)
        {

            Lib_Primavera.Model.RespostaErro erro = new Lib_Primavera.Model.RespostaErro();

            try
            {
                erro = Lib_Primavera.PriIntegration.UpdCliente(cliente);
                if (erro.Erro == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, erro.Descricao);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, erro.Descricao);
                }
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, erro.Descricao);
            }
        }



        public HttpResponseMessage Delete(string id)
        {


            Lib_Primavera.Model.RespostaErro erro = new Lib_Primavera.Model.RespostaErro();

            try
            {

                erro = Lib_Primavera.PriIntegration.DelCliente(id);

                if (erro.Erro == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, erro.Descricao);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, erro.Descricao);
                }

            }

            catch (Exception exc)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, erro.Descricao);

            }

        }*/
    }
}
