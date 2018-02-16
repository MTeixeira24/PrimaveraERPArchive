using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DashboardIntegration.Controllers
{
    public class HomepageController : ApiController
    {
        // POST api/homepage
        public Lib_Primavera.Model.Homepage Post([FromBody]string value)
        {
            DateTime date = new DateTime();
            int month = date.Month;
            List<double> vendas = new List<double>();
            List<double> compras = new List<double>();
            List<double> grossRevenues = new List<double>();
            List<double> netRevenues = new List<double>();
            for (int i = 1; i <= 12; i++)
            {
                vendas.Add(DocVendaController.getTotalMensal(i));
                compras.Add(DocCompraController.getTotalMensal(i));
                grossRevenues.Add(GanhosController.getGrossRevenueMensal(i));
                netRevenues.Add(GanhosController.getNetRevenueMensal(i));
            }

            Dictionary<String, int> clientes = ClientesController.topClientesNumVendas(5);
            Dictionary<String, int> artigos = ArtigosController.topArtigosNumVendas(5);
            Dictionary<String, double> fornecedores = FornecedorController.topFornecedoresFluxoDinheiro(5);
            double totalDivida = FornecedorController.getTotalDivida();
            int totalArtigosVendidosMes = ArtigosController.getNumArtigosVendidosMes(month);
            int totalClientesMes = ClientesController.getTotalClientesMes(month);
            double stktotal = ArtigosController.getStkTotal();
            int totalEncomendas = EncomendasController.getTotalEncomendas(month);

            return new Lib_Primavera.Model.Homepage
            {
                Vendas = vendas,
                Compras = compras,
                TopClientes = clientes,
                TopArtigos = artigos,
                TopFornecedores = fornecedores,
                totalDivida = totalDivida,
                totalArtigosVendidosMes = totalArtigosVendidosMes,
                totalClientesMes = totalClientesMes,
                stktotal = stktotal,
                totalEncomendas = totalEncomendas,
                GrossRevenues = grossRevenues,
                NetRevenues = netRevenues,
            };
        }
    }
}
