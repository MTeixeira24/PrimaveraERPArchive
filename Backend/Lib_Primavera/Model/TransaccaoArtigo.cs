using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Lib_Primavera.Model
{
    public class TransaccaoArtigo
    {
        public string entidade
        {
            get;
            set;
        }
        public string iddoc
        {
            get;
            set;
        }
        public DateTime date
        {
            get;
            set;
        }
        public int quantity
        {
            get;
            set;
        }
        public int linha
        {
            get;
            set;
        }
        public bool tipo //0 = venda, 1 = compra
        {
            get;
            set;
        }
    }
}