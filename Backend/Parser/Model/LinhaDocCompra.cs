using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Parser.Model
{

    public class LinhaDocCompra
    {

        public string CodArtigo
        {
            get;
            set;
        }

        public string DescArtigo
        {
            get;
            set;
        }

        public string IdCabecDoc
        {
            get;
            set;
        }

        public int NumLinha
        {
            get;
            set;
        }


        public double Quantidade
        {
            get;
            set;
        }

        public string Unidade
        {
            get;
            set;
        }

        public double Desconto
        {
            get;
            set;
        }

        public double PrecoUnitario
        {
            get;
            set;
        }

        public double TotalILiquido
        {
            get;
            set;
        }

        public double TotalLiquido
        {
            get;
            set;
        }

        public string Armazem
        {
            get;
            set;
        }

        public string Lote
        {
            get;
            set;
        }
    }
}