using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DashboardIntegration.Lib_Primavera.Model;

namespace DashboardIntegration.Controllers
{
    public class ClientesController : ApiController
    {
        //
        // GET: /Clientes/

        public IEnumerable<Lib_Primavera.Model.Cliente> Get()
        {
                //return Lib_Primavera.PriIntegration.ListaClientes();
            return Parser.StagingAPI.ListaClientes();
        }


        // GET api/cliente/5    
        public Cliente Get(string id)
        {
            //Lib_Primavera.Model.Cliente cliente = Lib_Primavera.PriIntegration.GetCliente(id);
            Lib_Primavera.Model.Cliente cliente = Parser.StagingAPI.GetCliente(id);
            if (cliente == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));

            }
            else
            {
                return cliente;
            }
        }
        // POST api/clientes
        public IEnumerable<Lib_Primavera.Model.Cliente> Post([FromBody] Lib_Primavera.Model.Login login)
        {
            //return Lib_Primavera.PriIntegration.ListaClientes(login.company, login.user, login.password);
            return Parser.StagingAPI.ListaClientes();
        }

        // POST api/clientes/5    
        public Cliente Post(string id, [FromBody] Lib_Primavera.Model.Login login)
        {
            //Lib_Primavera.Model.Cliente cliente = Lib_Primavera.PriIntegration.GetCliente(id, login.company, login.user, login.password);
            Lib_Primavera.Model.Cliente cliente = Parser.StagingAPI.GetCliente(id);
            if (cliente == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));

            }
            else
            {
                return cliente;
            }
        }

        public string topClientesFluxoDinheiro(int type)
        {
            List<String> clientes = DashboardIntegration.Parser.StagingAPI.getNomeClientes();
            return "Works";
        }
        public static Dictionary<String, int> topClientesNumVendas(int type)
        {
            List<String> clientes = DashboardIntegration.Parser.StagingAPI.getNomeClientes();
            Dictionary<String, int> totalCompras = new Dictionary<string, int>();
            foreach (string c in clientes)
            {
                int n = DashboardIntegration.Parser.StagingAPI.getTotalNumberSales(c);
                totalCompras.Add(c,n);
            }
            var top5 = totalCompras.OrderByDescending(pair => pair.Value).Take(type).ToDictionary(pair => pair.Key, pair => pair.Value);
            return top5;
        }
        public static int getTotalClientesMes(int m)
        {
            return DashboardIntegration.Parser.StagingAPI.getTotalClientesMes(m);
        }

        /*
        public HttpResponseMessage Post(Lib_Primavera.Model.Cliente cliente)
        {
            Lib_Primavera.Model.RespostaErro erro = new Lib_Primavera.Model.RespostaErro();
            erro = Lib_Primavera.PriIntegration.InsereClienteObj(cliente);

            if (erro.Erro == 0)
            {
                var response = Request.CreateResponse(
                   HttpStatusCode.Created, cliente);
                string uri = Url.Link("DefaultApi", new { CodCliente = cliente.CodCliente });
                response.Headers.Location = new Uri(uri);
                return response;
            }

            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }
        public HttpResponseMessage Put(string id, Lib_Primavera.Model.Cliente cliente)
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

        }

        */
    }
}
