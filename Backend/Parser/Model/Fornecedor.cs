using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Parser.Model
{
    public class Fornecedor
    {
        public string CodFornecedor{
            get;
            set;
        }
        public string Morada{
            get;
            set;
        }
        public string Local{
            get;
            set;
        }
        public string Cp{
            get;
            set;
        }
        public string Tel{
            get;
            set;
        }
        public double Divida{
            get;
            set;
        }
        public string NumContrib{
            get;
            set;
        }
        public string ModoPag{
            get;
            set;
        }
        public string EnderecoWeb{
            get;
            set;
        }
        public double EncomendasPendentes
        {
            get;
            set;
        }
    }
}