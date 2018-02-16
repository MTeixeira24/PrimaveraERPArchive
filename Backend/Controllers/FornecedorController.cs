using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DashboardIntegration.Lib_Primavera.Model;

namespace DashboardIntegration.Controllers
{
    public class FornecedorController : ApiController
    {

        //GET: /api/fornecedor/
        public IEnumerable<Lib_Primavera.Model.Fornecedor> Get()
        {
            //return Lib_Primavera.PriIntegration.ListaFornecedores();
            return DashboardIntegration.Parser.StagingAPI.ListaFornecedores();
        }

        // GET api/fornecedor/5    
        public Fornecedor Get(string id)
        {
            //Lib_Primavera.Model.Fornecedor fornecedor = Lib_Primavera.PriIntegration.GetFornecedor(id, DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim());
            Lib_Primavera.Model.Fornecedor fornecedor = DashboardIntegration.Parser.StagingAPI.GetFornecedor(id);
            if (fornecedor == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));
            }
            else
            {
                return fornecedor;
            }
        }

        //POST: /api/fornecedores/
        public IEnumerable<Lib_Primavera.Model.Fornecedor> Post([FromBody] Lib_Primavera.Model.Login login)
        {
            //return Lib_Primavera.PriIntegration.ListaFornecedores(login.company, login.user, login.password);
            return DashboardIntegration.Parser.StagingAPI.ListaFornecedores();
        }

        // POST api/fornecedor/5    
        public Fornecedor Post(string id, [FromBody] Lib_Primavera.Model.Login login)
        {
            //Lib_Primavera.Model.Fornecedor fornecedor = Lib_Primavera.PriIntegration.GetFornecedor(id, login.company, login.user, login.password);
            Lib_Primavera.Model.Fornecedor fornecedor = DashboardIntegration.Parser.StagingAPI.GetFornecedor(id);
            if (fornecedor == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));
            }
            else
            {
                return fornecedor;
            }
        }
        public static Dictionary<String, double> topFornecedoresFluxoDinheiro(int type)
        {
            List<String> forn = DashboardIntegration.Parser.StagingAPI.getIdFornecedores();
            Dictionary<String, double> totalCompras = new Dictionary<string, double>();
            foreach (string f in forn)
            {
                double n = DashboardIntegration.Parser.StagingAPI.getTotalNumberPurchasesValue(f);
                string s = DashboardIntegration.Parser.StagingAPI.getNomeFornecedores(f);
                totalCompras.Add(s, n);
            }
            var top5 = totalCompras.OrderByDescending(pair => pair.Value).Take(type).ToDictionary(pair => pair.Key, pair => pair.Value);
            return top5;
        }
        public static Dictionary<String, int> topFornecedoresNumCompras(int type)
        {
            List<String> forn = DashboardIntegration.Parser.StagingAPI.getIdFornecedores();
            Dictionary<String, int> totalCompras = new Dictionary<string, int>();
            foreach (string f in forn)
            {
                int n = DashboardIntegration.Parser.StagingAPI.getTotalNumberPurchases(f);
                string s = DashboardIntegration.Parser.StagingAPI.getNomeFornecedores(f);
                totalCompras.Add(f, n);
            }
            var top5 = totalCompras.OrderByDescending(pair => pair.Value).Take(type).ToDictionary(pair => pair.Key, pair => pair.Value);
            return top5;
        }
        public static double getTotalDivida()
        {
            return DashboardIntegration.Parser.StagingAPI.getTotalDividaFornecedor();
        }
    }
}
