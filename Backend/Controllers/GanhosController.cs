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
    public class GanhosController : ApiController
    {
        //
        // GET: /Ganhos/

        public IEnumerable<Lib_Primavera.Model.Ganhos> Get()
        {

            return Parser.StagingAPI.listaGanhos();
        }



        public static double getGrossRevenueMensal(int month)
        {
            return DashboardIntegration.Parser.StagingAPI.GetGrossRevenue(month.ToString());
        }
        public static double getNetRevenueMensal(int month)
        {
            return DashboardIntegration.Parser.StagingAPI.GetNetRevenue(month.ToString());
        }
    }
}