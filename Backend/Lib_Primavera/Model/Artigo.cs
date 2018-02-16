using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashboardIntegration.Lib_Primavera.Model
{
    public class Artigo
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
        public string TipoArtigo
        {
            get;
            set;
        }

        public double STKAtual
        {
            get;
            set;
        }

        public List<TransaccaoArtigo> transaccoes
        {
            get;
            set;
        }

    }
}