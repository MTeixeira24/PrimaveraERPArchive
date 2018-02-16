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
    public class LoginController : ApiController
    {
        // GET api/login
        /*public bool Get()
        {
            return Lib_Primavera.PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim());
        }*/

        // GET api/login/5
        public bool Get([FromBody]string company, [FromBody]string user, [FromBody]string password)
        {
            return Lib_Primavera.PriEngine.InitializeCompany(company, user, password);
        }

        // POST api/login
        public bool Post([FromBody] Lib_Primavera.Model.Login login)
        {
            try
            {
                DashboardIntegration.Parser.StagingAPI.empresa = login.company;
                return Lib_Primavera.PriEngine.InitializeCompany(login.company, login.user, login.password);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // PUT api/login/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/login/5
        public void Delete(int id)
        {
        }
    }
}
