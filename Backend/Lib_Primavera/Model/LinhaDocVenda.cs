using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Lib_Primavera.Model
{
    public class LinhaDocVenda
    {

        public int Id
        {
            get;
            set;
        }
        public int LineNumber
        {
            get;
            set;
        }
        public string ProductCode
        {
            get;
            set;
        }
        public int Quantity
        {
            get;
            set;
        }
        public double UnitPrice
        {
            get;
            set;
        }
        public System.DateTime TaxPointDate
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public double CreditAmount
        {
            get;
            set;
        }
        public string TaxType
        {
            get;
            set;
        }
        public string TaxCountryRegion
        {
            get;
            set;
        }
        public string TaxCode
        {
            get;
            set;
        }
        public double TaxPercentage
        {
            get;
            set;
        }
        public double SettlementAmount
        {
            get;
            set;
        }
        public string InvoiceNo
        {
            get;
            set;
        }


        /*
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
        }*/


    }
}