using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Parser.Model
{
    public class Cliente
    {
        public string Morada;

        /* Exemplo para POST e GET com valores específicos
         public string Morada
        {
            get
            {
                return "MORADA: " + _morada;
            }
            set
            {
                _morada = value;
            }
        }
    
*/       
        public string CodCliente
        {
            get;
            set;
        }

        public string NomeCliente
        {
            get;
            set;
        }

        public string NumContribuinte
        {
            get;
            set;
        }

        public string Telefone
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }


    }
}