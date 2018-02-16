using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Lib_Primavera.Model
{
    public class Homepage
    {
        public List<double> Vendas
        {
            get;
            set;
        }

        public List<double> Compras
        {
            get;
            set;
        }

        public Dictionary<String, int> TopClientes
        {
            get;
            set;
        }

        public Dictionary<String, int> TopArtigos
        {
            get;
            set;
        }

        public Dictionary<String, double> TopFornecedores
        {
            get;
            set;
        }

        public List<double> GrossRevenues
        {
            get;
            set;
        }
        public List<double> NetRevenues
        {
            get;
            set;
        }

        public double totalDivida
        {
            get;
            set;
        }
        public int totalArtigosVendidosMes
        {
            get;
            set;
        }
        public int totalClientesMes
        {
            get;
            set;
        }
        public double stktotal
        {
            get;
            set;
        }
        public int totalEncomendas
        {
            get;
            set;
        }
    }
}