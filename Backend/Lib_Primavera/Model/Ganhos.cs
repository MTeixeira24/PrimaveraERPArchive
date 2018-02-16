using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Lib_Primavera.Model
{
    public class Ganhos
    {
        public int month
        {
            get;
            set;
        }

        public double grossRevenue
        {
            get;
            set;
        }

        public double netRevenue
        {
            get;
            set;
        }

        public double grossMargin
        {
            get;
            set;
        }

        public int totalClients
        {
            get;
            set;
        }

        public List<GanhosDeCliente> ganhosDeClientes
        {
            get;
            set;
        }


        public class GanhosDeCliente
        {
            public String nome
            {
                get;
                set;
            }

            public double GrossMargin
            {
                get;
                set;
            }
        }





    }
}