using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Lib_Primavera.Model
{
    public class DocVenda
    {
        public string InvoiceNo
        {
            get;
            set;
        }

        public bool HashControl
        {
            get;
            set;
        }

        public string Hash
        {
            get;
            set;
        }

        public string InvoiceStatus
        {
            get;
            set;
        }
        public int Period
        {
            get;
            set;
        }
        public System.DateTime InvoiceDate
        {
            get;
            set;
        }
        public string InvoiceType
        {
            get;
            set;
        }
        public bool SelfBillingIndicator
        {
            get;
            set;
        }
        public System.DateTime SystemEntryDate
        {
            get;
            set;
        }
        public string CustomerID
        {
            get;
            set;
        }
        public double TaxPayable
        {
            get;
            set;
        }
        public double NetTotal
        {
            get;
            set;
        }
        public double GrossTotal
        {
            get;
            set;
        }
        public List<Model.LinhaDocVenda> LinhasDoc
        {
            get;
            set;
        }
        public Cliente dadosCliente
        {
            get;
            set;
        }
        public Empresa dadosEmpresa
        {
            get;
            set;
        }

        /*
        public string id
        {
            get;
            set;
        }

        public string Entidade
        {
            get;
            set;
        }

        public int NumDoc
        {
            get;
            set;
        }

        public DateTime Data
        {
            get;
            set;
        }

        public double TotalMerc
        {
            get;
            set;
        }

        public string Serie
        {
            get;
            set;
        }

        public List<Model.LinhaDocVenda> LinhasDoc

        {
            get;
            set;
        }*/
 

    }
}