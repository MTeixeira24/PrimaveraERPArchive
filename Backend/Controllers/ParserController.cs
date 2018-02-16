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
    public class ParserController : ApiController
    {        // POST api/parser
        public ActionResult Post([FromBody]Lib_Primavera.Model.Login login)
        {
            DashboardIntegration.Parser.Parser.Parse("saft.xml", login);
            return new HttpStatusCodeResult(200);
        }
        
        // POST api/parser/[saftPath]
        public ActionResult Post(string saftPath, [FromBody]Lib_Primavera.Model.Login login)
        {
            DashboardIntegration.Parser.Parser.Parse(saftPath, login);
            return new HttpStatusCodeResult(200);
        }


        // GET api/paser   
        public void Get()
        {
            
        }
    }
}
