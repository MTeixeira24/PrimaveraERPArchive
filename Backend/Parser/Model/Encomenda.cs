using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Parser.Model
{
    public class Encomenda
    {
        public string CodEncomenda
        {
            get;
            set;
        }

        public string Entidade
        {
            get;
            set;
        }
        public List<Model.LinhaDocCompra> LinhasEnc
        {
            get;
            set;
        }

        public string Origem { get; set; }

        public double TotalDesc { get; set; }

        public double TotalIva { get; set; }

        public double TotalMerc { get; set; }

        public string ModoPag { get; set; }

        public DateTime DataDoc { get; set; }
    }
}