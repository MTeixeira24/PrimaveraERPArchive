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
    public class ArtigosController : ApiController
    {
        //
        // GET: /Artigos/

        public IEnumerable<Lib_Primavera.Model.Artigo> Get()
        {
            //return Lib_Primavera.PriIntegration.ListaArtigos();
            return DashboardIntegration.Parser.StagingAPI.ListaArtigos();
        }


        // GET api/artigo/5    
        public Artigo Get(string id)
        {
            //Lib_Primavera.Model.Artigo artigo = Lib_Primavera.PriIntegration.GetArtigo(id);
            Lib_Primavera.Model.Artigo artigo = DashboardIntegration.Parser.StagingAPI.GetArtigo(id);
            if (artigo == null)
            {
                throw new HttpResponseException(
                  Request.CreateResponse(HttpStatusCode.NotFound));
            }
            else
            {
                return artigo;
            }
        }

        // POST api/artigos
        public IEnumerable<Lib_Primavera.Model.Artigo> Post([FromBody] Lib_Primavera.Model.Login login)
        {
            //return Lib_Primavera.PriIntegration.ListaArtigos();
            return DashboardIntegration.Parser.StagingAPI.ListaArtigos();
        }

        // POST api/artigo/5    
        public Artigo Post(string id, [FromBody] Lib_Primavera.Model.Login login)
        {
            //Lib_Primavera.Model.Artigo artigo = Lib_Primavera.PriIntegration.GetArtigo(id, login.company, login.user, login.password);

            Lib_Primavera.Model.Artigo artigo = DashboardIntegration.Parser.StagingAPI.GetArtigo(id);
            if (artigo == null)
            {
                throw new HttpResponseException(
                  Request.CreateResponse(HttpStatusCode.NotFound));
            }
            else
            {
                return artigo;
            }
        }
        public static Dictionary<String, int> topArtigosNumVendas(int type)
        {
            List<String> artigos = DashboardIntegration.Parser.StagingAPI.getIdArtigos();
            Dictionary<String, int> totalSales = new Dictionary<string, int>();
            foreach (string c in artigos)
            {
                int n = DashboardIntegration.Parser.StagingAPI.getTotalNumberProductSales(c);
                string s = DashboardIntegration.Parser.StagingAPI.getArtName(c);
                try
                {
                    totalSales.Add(s, n);
                }
                catch (System.ArgumentException e)
                {
                    totalSales.Add(s+s, n);
                }
                
            }
            var top5 = totalSales.OrderByDescending(pair => pair.Value).Take(type).ToDictionary(pair => pair.Key, pair => pair.Value);
            return top5;
        }
        public static int getNumArtigosVendidosMes(int m)
        {
            return DashboardIntegration.Parser.StagingAPI.getNumArtigosVendidosMes(m);
        }
        public static double getStkTotal()
        {
            return DashboardIntegration.Parser.StagingAPI.getStkTotal();
        }

    }
}

