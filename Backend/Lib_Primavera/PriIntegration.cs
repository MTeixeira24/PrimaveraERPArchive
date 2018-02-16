using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interop.ErpBS900;
using Interop.StdPlatBS900;
using Interop.StdBE900;
using Interop.GcpBE900;
using ADODB;

namespace DashboardIntegration.Lib_Primavera
{
    public class PriIntegration
    {
        

        # region Cliente

        public static List<Model.Cliente> ListaClientes()
        {
            
            
            StdBELista objList;

            List<Model.Cliente> listClientes = new List<Model.Cliente>();

            if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
            {

                //objList = PriEngine.Engine.Comercial.Clientes.LstClientes();

                objList = PriEngine.Engine.Consulta("SELECT Cliente, Nome, Moeda, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo, CDU_Email as Email FROM  CLIENTES");

                
                while (!objList.NoFim())
                {
                    listClientes.Add(new Model.Cliente
                    {
                        CodCliente = objList.Valor("Cliente"),
                        NomeCliente = objList.Valor("Nome"),
                        Moeda = objList.Valor("Moeda"),
                        NumContribuinte = objList.Valor("NumContribuinte"),
                        Morada = objList.Valor("campo_exemplo"),
                        Email = objList.Valor("Email")
                    });
                    objList.Seguinte();

                }

                return listClientes;
            }
            else
                return null;
        }
        public static List<Model.Cliente> ListaClientes(string company, string user, string password)
        {

            StdBELista objList;

            Model.Cliente art = new Model.Cliente();
            List<Model.Cliente> listClientes = new List<Model.Cliente>();

            if (PriEngine.InitializeCompany(company, user, password) == true)
            {

                //objList = PriEngine.Engine.Comercial.Clientes.LstClientes();

                objList = PriEngine.Engine.Consulta("SELECT Cliente, Nome, Moeda, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo, CDU_Email as Email FROM  CLIENTES");


                while (!objList.NoFim())
                {
                    listClientes.Add(new Model.Cliente
                    {
                        CodCliente = objList.Valor("Cliente"),
                        NomeCliente = objList.Valor("Nome"),
                        Moeda = objList.Valor("Moeda"),
                        NumContribuinte = objList.Valor("NumContribuinte"),
                        Morada = objList.Valor("campo_exemplo"),
                        Email = objList.Valor("Email")
                    });
                    objList.Seguinte();

                }

                return listClientes;
            }
            else
                return null;
        }

        public static Lib_Primavera.Model.Cliente GetCliente(string codCliente)
        {
            

            GcpBECliente objCli = new GcpBECliente();


            Model.Cliente myCli = new Model.Cliente();

            if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
            {

                if (PriEngine.Engine.Comercial.Clientes.Existe(codCliente) == true)
                {
                    
                    objCli = PriEngine.Engine.Comercial.Clientes.Edita(codCliente);
                    myCli.CodCliente = objCli.get_Cliente();
                    myCli.NomeCliente = objCli.get_Nome();
                    myCli.Moeda = objCli.get_Moeda();
                    myCli.NumContribuinte = objCli.get_NumContribuinte();
                    myCli.Morada = objCli.get_Morada();
                    myCli.Email = PriEngine.Engine.Comercial.Clientes.DaValorAtributo(codCliente, "CDU_Email");

                    
                    return myCli;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;
        }
        public static Lib_Primavera.Model.Cliente GetCliente(string codCliente, string company, string user, string password)
        {

            GcpBECliente objCli = new GcpBECliente();


            Model.Cliente myCli = new Model.Cliente();

            if (PriEngine.InitializeCompany(company, user, password) == true)
            {

                if (PriEngine.Engine.Comercial.Clientes.Existe(codCliente) == true)
                {

                    objCli = PriEngine.Engine.Comercial.Clientes.Edita(codCliente);
                    myCli.CodCliente = objCli.get_Cliente();
                    myCli.NomeCliente = objCli.get_Nome();
                    myCli.Moeda = objCli.get_Moeda();
                    myCli.NumContribuinte = objCli.get_NumContribuinte();
                    myCli.Morada = objCli.get_Morada();
                    myCli.Email = PriEngine.Engine.Comercial.Clientes.DaValorAtributo(codCliente, "CDU_Email");


                    return myCli;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;
        }
        public static Lib_Primavera.Model.RespostaErro UpdCliente(Lib_Primavera.Model.Cliente cliente)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
           

            GcpBECliente objCli = new GcpBECliente();

            try
            {

                if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
                {

                    if (PriEngine.Engine.Comercial.Clientes.Existe(cliente.CodCliente) == false)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "O cliente não existe";
                        return erro;
                    }
                    else
                    {

                        objCli = PriEngine.Engine.Comercial.Clientes.Edita(cliente.CodCliente);
                        objCli.set_EmModoEdicao(true);

                        objCli.set_Nome(cliente.NomeCliente);
                        objCli.set_NumContribuinte(cliente.NumContribuinte);
                        objCli.set_Moeda(cliente.Moeda);
                        objCli.set_Morada(cliente.Morada);
                        
                        PriEngine.Engine.Comercial.Clientes.Actualiza(objCli);

                        erro.Erro = 0;
                        erro.Descricao = "Sucesso";
                        return erro;
                    }
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir a empresa";
                    return erro;

                }

            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }

        }


        public static Lib_Primavera.Model.RespostaErro DelCliente(string codCliente)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBECliente objCli = new GcpBECliente();


            try
            {

                if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
                {
                    if (PriEngine.Engine.Comercial.Clientes.Existe(codCliente) == false)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "O cliente não existe";
                        return erro;
                    }
                    else
                    {

                        PriEngine.Engine.Comercial.Clientes.Remove(codCliente);
                        erro.Erro = 0;
                        erro.Descricao = "Sucesso";
                        return erro;
                    }
                }

                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir a empresa";
                    return erro;
                }
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }

        }



        public static Lib_Primavera.Model.RespostaErro InsereClienteObj(Model.Cliente cli)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            

            GcpBECliente myCli = new GcpBECliente();

            try
            {
                if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
                {

                    myCli.set_Cliente(cli.CodCliente);
                    myCli.set_Nome(cli.NomeCliente);
                    myCli.set_NumContribuinte(cli.NumContribuinte);
                    myCli.set_Moeda(cli.Moeda);
                    myCli.set_Morada(cli.Morada);

                    PriEngine.Engine.Comercial.Clientes.Actualiza(myCli);

                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir empresa";
                    return erro;
                }
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }


        }

       

        #endregion Cliente;   // -----------------------------  END   CLIENTE    -----------------------

        #region Fornecedores

        public static List<Model.Fornecedor> ListaFornecedores()
        {
            StdBELista objList;
            List<Model.Fornecedor> listFornecedores = new List<Model.Fornecedor>();
            //if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
            //{
                objList = PriEngine.Engine.Consulta("SELECT Fornecedor, Nome ,Morada, Local, Cp, Tel, TotalDeb as Divida, NumContrib, ModoPag, EnderecoWeb, EncomendasPendentes FROM Fornecedores");
                while (!objList.NoFim())
                {
                    listFornecedores.Add(new Model.Fornecedor
                    {
                        CodFornecedor = objList.Valor("Fornecedor"),
                        Nome = objList.Valor("Nome"),
                        Morada = objList.Valor("Morada"),
                        Local = objList.Valor("Local"),
                        Cp = objList.Valor("Cp"),
                        Tel = objList.Valor("Tel"),
                        Divida = objList.Valor("Divida"),
                        NumContrib = objList.Valor("NumContrib"),
                        ModoPag = objList.Valor("ModoPag"),
                        EnderecoWeb = objList.Valor("EnderecoWeb"),
                        EncomendasPendentes = objList.Valor("EncomendasPendentes")
                    });
                    objList.Seguinte();
                }
                return listFornecedores;
            //}
            //else
                //return null;
        }
        public static List<Model.Fornecedor> ListaFornecedores(string company, string user, string password)
        {
            StdBELista objList;
            List<Model.Fornecedor> listFornecedores = new List<Model.Fornecedor>();
            if (PriEngine.InitializeCompany(company, user, password) == true)
            {
                objList = PriEngine.Engine.Consulta("SELECT Fornecedor, Morada, Local, Cp, Tel, TotalDeb as Divida, NumContrib, ModoPag, EnderecoWeb, EncomendasPendentes FROM Fornecedores");
                while (!objList.NoFim())
                {
                    listFornecedores.Add(new Model.Fornecedor
                    {
                        CodFornecedor = objList.Valor("Fornecedor"),
                        Morada = objList.Valor("Morada"),
                        Local = objList.Valor("Local"),
                        Cp = objList.Valor("Cp"),
                        Tel = objList.Valor("Tel"),
                        Divida = objList.Valor("Divida"),
                        NumContrib = objList.Valor("NumContrib"),
                        ModoPag = objList.Valor("ModoPag"),
                        EnderecoWeb = objList.Valor("EnderecoWeb"),
                        EncomendasPendentes = objList.Valor("EncomendasPendentes")
                    });
                    objList.Seguinte();
                }
                return listFornecedores;
            }
            else
                return null;
        }

        public static Lib_Primavera.Model.Fornecedor GetFornecedor(string codFornecedor, string company, string user, string password)
        {

            GcpBEFornecedor objFor = new GcpBEFornecedor();


            Model.Fornecedor myFor = new Model.Fornecedor();

            if (PriEngine.InitializeCompany(company, user, password) == true)
            {

                if (PriEngine.Engine.Comercial.Fornecedores.Existe(codFornecedor) == true)
                {
                    objFor = PriEngine.Engine.Comercial.Fornecedores.Edita(codFornecedor);
                    myFor.CodFornecedor = objFor.get_Fornecedor();
                    myFor.Morada = objFor.get_Morada();
                        myFor.Local = objFor.get_Localidade();
                        myFor.Cp = objFor.get_LocalidadeCodigoPostal();
                        myFor.Tel = objFor.get_Telefone();
                        myFor.Divida = objFor.get_DebitoContaCorrente();
                        myFor.NumContrib = objFor.get_NumContribuinte();
                        myFor.ModoPag = objFor.get_ModoPag();
                        myFor.EnderecoWeb = objFor.get_EnderecoWeb();
                        //myFor.EncomendasPendentes = objFor.get_EncomendasPendentes();

                    return myFor;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;
        }

        #endregion


        #region Artigo

        public static Lib_Primavera.Model.Artigo GetArtigo(string codArtigo)
        {
            
            GcpBEArtigo objArtigo = new GcpBEArtigo();
            Model.Artigo myArt = new Model.Artigo();

            if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
            {

                if (PriEngine.Engine.Comercial.Artigos.Existe(codArtigo) == false)
                {
                    return null;
                }
                else
                {
                    objArtigo = PriEngine.Engine.Comercial.Artigos.Edita(codArtigo);
                    myArt.CodArtigo = objArtigo.get_Artigo();
                    myArt.DescArtigo = objArtigo.get_Descricao();
                    myArt.STKAtual = objArtigo.get_StkActual(); 

                    return myArt;
                }
                
            }
            else
            {
                return null;
            }

        }

        public static List<Model.Artigo> ListaArtigos()
        {

            StdBELista objList;

            Model.Artigo art = new Model.Artigo();
            List<Model.Artigo> listArts = new List<Model.Artigo>();

            if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
            {

                objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

                while (!objList.NoFim())
                {
                    art = new Model.Artigo();
                    art.CodArtigo = objList.Valor("artigo");
                    art.DescArtigo = objList.Valor("descricao");
                    //                  art.STKAtual = objList.Valor("stkatual");


                    listArts.Add(art);
                    objList.Seguinte();
                }

                return listArts;

            }
            else
            {
                return null;

            }

        }

        public static Lib_Primavera.Model.Artigo GetArtigo(string codArtigo, string company, string user, string password)
        {

            GcpBEArtigo objArtigo = new GcpBEArtigo();
            Model.Artigo myArt = new Model.Artigo();

            if (PriEngine.InitializeCompany(company, user, password) == true)
            {

                if (PriEngine.Engine.Comercial.Artigos.Existe(codArtigo) == false)
                {
                    return null;
                }
                else
                {
                    objArtigo = PriEngine.Engine.Comercial.Artigos.Edita(codArtigo);
                    myArt.CodArtigo = objArtigo.get_Artigo();
                    myArt.DescArtigo = objArtigo.get_Descricao();
                    myArt.STKAtual = objArtigo.get_StkActual();

                    return myArt;
                }

            }
            else
            {
                return null;
            }

        }

        public static List<Model.Artigo> ListaArtigos(string company, string user, string password)
        {
                        
            StdBELista objList;

            Model.Artigo art = new Model.Artigo();
            List<Model.Artigo> listArts = new List<Model.Artigo>();

            if (PriEngine.InitializeCompany(company, user, password) == true)
            {

                objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

                while (!objList.NoFim())
                {
                    art = new Model.Artigo();
                    art.CodArtigo = objList.Valor("artigo");
                    art.DescArtigo = objList.Valor("descricao");
  //                  art.STKAtual = objList.Valor("stkatual");
                  
                    
                    listArts.Add(art);
                    objList.Seguinte();
                }

                return listArts;

            }
            else
            {
                return null;

            }

        }

        #endregion Artigo


        #region DocCompra
        

        public static List<Model.DocCompra> VGR_List()
        {
                
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocCompra dc = new Model.DocCompra();
            List<Model.DocCompra> listdc = new List<Model.DocCompra>();
            Model.LinhaDocCompra lindc = new Model.LinhaDocCompra();
            List<Model.LinhaDocCompra> listlindc = new List<Model.LinhaDocCompra>();

            //if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
            //{
                objListCab = PriEngine.Engine.Consulta("SELECT id, NumDocExterno, Entidade, DataDoc, NumDoc, TotalMerc, Serie From CabecCompras where TipoDoc='VGR'");
                while (!objListCab.NoFim())
                {
                    dc = new Model.DocCompra();
                    dc.id = objListCab.Valor("id");
                    dc.NumDocExterno = objListCab.Valor("NumDocExterno");
                    dc.Entidade = objListCab.Valor("Entidade");
                    dc.NumDoc = objListCab.Valor("NumDoc");
                    dc.Data = objListCab.Valor("DataDoc");
                    dc.TotalMerc = objListCab.Valor("TotalMerc");
                    dc.Serie = objListCab.Valor("Serie");
                    objListLin = PriEngine.Engine.Consulta("SELECT id, idCabecCompras, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Armazem, Lote from LinhasCompras where IdCabecCompras='" + dc.id + "' order By NumLinha");
                    listlindc = new List<Model.LinhaDocCompra>();

                    while (!objListLin.NoFim())
                    {
                        lindc = new Model.LinhaDocCompra();
                        lindc.Id = objListLin.Valor("id");
                        lindc.IdCabecDoc = objListLin.Valor("idCabecCompras");
                        lindc.CodArtigo = objListLin.Valor("Artigo");
                        lindc.DescArtigo = objListLin.Valor("Descricao");
                        lindc.Quantidade = objListLin.Valor("Quantidade");
                        lindc.Unidade = objListLin.Valor("Unidade");
                        lindc.Desconto = objListLin.Valor("Desconto1");
                        lindc.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindc.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindc.TotalLiquido = objListLin.Valor("PrecoLiquido");
                        lindc.Armazem = objListLin.Valor("Armazem");
                        lindc.Lote = objListLin.Valor("Lote");

                        listlindc.Add(lindc);
                        objListLin.Seguinte();
                    }

                    dc.LinhasDoc = listlindc;
                    
                    listdc.Add(dc);
                    objListCab.Seguinte();
                }
            //}
            return listdc;
        }

        public static List<Model.DocCompra> VGR_List(string company, string user, string password)
        {

            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocCompra dc = new Model.DocCompra();
            List<Model.DocCompra> listdc = new List<Model.DocCompra>();
            Model.LinhaDocCompra lindc = new Model.LinhaDocCompra();
            List<Model.LinhaDocCompra> listlindc = new List<Model.LinhaDocCompra>();

            if (PriEngine.InitializeCompany(company, user, password) == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT id, NumDocExterno, Entidade, DataDoc, NumDoc, TotalMerc, Serie From CabecCompras where TipoDoc='VGR'");
                while (!objListCab.NoFim())
                {
                    dc = new Model.DocCompra();
                    dc.id = objListCab.Valor("id");
                    dc.NumDocExterno = objListCab.Valor("NumDocExterno");
                    dc.Entidade = objListCab.Valor("Entidade");
                    dc.NumDoc = objListCab.Valor("NumDoc");
                    dc.Data = objListCab.Valor("DataDoc");
                    dc.TotalMerc = objListCab.Valor("TotalMerc");
                    dc.Serie = objListCab.Valor("Serie");
                    objListLin = PriEngine.Engine.Consulta("SELECT id, idCabecCompras, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Armazem, Lote from LinhasCompras where IdCabecCompras='" + dc.id + "' order By NumLinha");
                    listlindc = new List<Model.LinhaDocCompra>();

                    while (!objListLin.NoFim())
                    {
                        lindc = new Model.LinhaDocCompra();
                        lindc.Id = objListLin.Valor("id");
                        lindc.IdCabecDoc = objListLin.Valor("idCabecCompras");
                        lindc.CodArtigo = objListLin.Valor("Artigo");
                        lindc.DescArtigo = objListLin.Valor("Descricao");
                        lindc.Quantidade = objListLin.Valor("Quantidade");
                        lindc.Unidade = objListLin.Valor("Unidade");
                        lindc.Desconto = objListLin.Valor("Desconto1");
                        lindc.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindc.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindc.TotalLiquido = objListLin.Valor("PrecoLiquido");
                        lindc.Armazem = objListLin.Valor("Armazem");
                        lindc.Lote = objListLin.Valor("Lote");

                        listlindc.Add(lindc);
                        objListLin.Seguinte();
                    }

                    dc.LinhasDoc = listlindc;

                    listdc.Add(dc);
                    objListCab.Seguinte();
                }
            }
            return listdc;
        }

        public static Model.DocCompra GetCompra(string id, string company, string user, string password)
        {
            Model.DocCompra dc = new Model.DocCompra();
            StdBELista objListCab;
            StdBELista objListLin;
            List<Model.LinhaDocCompra> listlindc = new List<Model.LinhaDocCompra>();
            Model.LinhaDocCompra lindc;
            if (PriEngine.InitializeCompany(company, user, password) == true)
            {
                string st = "SELECT id, NumDocExterno, Entidade, DataDoc, NumDoc, TotalMerc, Serie From CabecCompras where TipoDoc='VGR' and NumDoc='" + id + "'";
                objListCab = PriEngine.Engine.Consulta(st);
                dc.id = objListCab.Valor("id");
                dc.NumDocExterno = objListCab.Valor("NumDocExterno");
                dc.Entidade = objListCab.Valor("Entidade");
                dc.NumDoc = objListCab.Valor("NumDoc");
                dc.Data = objListCab.Valor("DataDoc");
                dc.TotalMerc = objListCab.Valor("TotalMerc");
                dc.Serie = objListCab.Valor("Serie");
                objListLin = PriEngine.Engine.Consulta("SELECT id, idCabecCompras, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Armazem, Lote from LinhasCompras where IdCabecCompras='" + dc.id + "' order By NumLinha");
                listlindc = new List<Model.LinhaDocCompra>();

                while (!objListLin.NoFim())
                {
                   lindc = new Model.LinhaDocCompra();
                   lindc.Id = objListLin.Valor("id");
                   lindc.IdCabecDoc = objListLin.Valor("idCabecCompras");
                   lindc.CodArtigo = objListLin.Valor("Artigo");
                   lindc.DescArtigo = objListLin.Valor("Descricao");
                   lindc.Quantidade = objListLin.Valor("Quantidade");
                   lindc.Unidade = objListLin.Valor("Unidade");
                   lindc.Desconto = objListLin.Valor("Desconto1");
                   lindc.PrecoUnitario = objListLin.Valor("PrecUnit");
                   lindc.TotalILiquido = objListLin.Valor("TotalILiquido");
                   lindc.TotalLiquido = objListLin.Valor("PrecoLiquido");
                   lindc.Armazem = objListLin.Valor("Armazem");
                   lindc.Lote = objListLin.Valor("Lote");
                   listlindc.Add(lindc);
                   objListLin.Seguinte();
                }

                dc.LinhasDoc = listlindc;
                return dc;
            }
            else
            {
                return null;
            }
        }
                
        public static Model.RespostaErro VGR_New(Model.DocCompra dc)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            

            GcpBEDocumentoCompra myGR = new GcpBEDocumentoCompra();
            GcpBELinhaDocumentoCompra myLin = new GcpBELinhaDocumentoCompra();
            GcpBELinhasDocumentoCompra myLinhas = new GcpBELinhasDocumentoCompra();

            PreencheRelacaoCompras rl = new PreencheRelacaoCompras();
            List<Model.LinhaDocCompra> lstlindv = new List<Model.LinhaDocCompra>();

            try
            {
                if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
                {
                    // Atribui valores ao cabecalho do doc
                    //myEnc.set_DataDoc(dv.Data);
                    myGR.set_Entidade(dc.Entidade);
                    myGR.set_NumDocExterno(dc.NumDocExterno);
                    myGR.set_Serie(dc.Serie);
                    myGR.set_Tipodoc("VGR");
                    myGR.set_TipoEntidade("F");
                    // Linhas do documento para a lista de linhas
                    lstlindv = dc.LinhasDoc;
                    //PriEngine.Engine.Comercial.Compras.PreencheDadosRelacionados(myGR,rl);
                    PriEngine.Engine.Comercial.Compras.PreencheDadosRelacionados(myGR);
                    foreach (Model.LinhaDocCompra lin in lstlindv)
                    {
                        PriEngine.Engine.Comercial.Compras.AdicionaLinha(myGR, lin.CodArtigo, lin.Quantidade, lin.Armazem, "", lin.PrecoUnitario, lin.Desconto);
                    }


                    PriEngine.Engine.IniciaTransaccao();
                    PriEngine.Engine.Comercial.Compras.Actualiza(myGR, "Teste");
                    PriEngine.Engine.TerminaTransaccao();
                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir empresa";
                    return erro;

                }

            }
            catch (Exception ex)
            {
                PriEngine.Engine.DesfazTransaccao();
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }


        #endregion DocCompra


        #region DocsVenda
        /*
        public static Model.RespostaErro Encomendas_New(Model.DocVenda dv)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBEDocumentoVenda myEnc = new GcpBEDocumentoVenda();
             
            GcpBELinhaDocumentoVenda myLin = new GcpBELinhaDocumentoVenda();

            GcpBELinhasDocumentoVenda myLinhas = new GcpBELinhasDocumentoVenda();
             
            PreencheRelacaoVendas rl = new PreencheRelacaoVendas();
            List<Model.LinhaDocVenda> lstlindv = new List<Model.LinhaDocVenda>();
            
            try
            {
                if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
                {
                    // Atribui valores ao cabecalho do doc
                    //myEnc.set_DataDoc(dv.Data);
                    myEnc.set_Entidade(dv.Entidade);
                    myEnc.set_Serie(dv.Serie);
                    myEnc.set_Tipodoc("ECL");
                    myEnc.set_TipoEntidade("C");
                    // Linhas do documento para a lista de linhas
                    lstlindv = dv.LinhasDoc;
                    //PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc, rl);
                    PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc);
                    foreach (Model.LinhaDocVenda lin in lstlindv)
                    {
                        PriEngine.Engine.Comercial.Vendas.AdicionaLinha(myEnc, lin.CodArtigo, lin.Quantidade, "", "", lin.PrecoUnitario, lin.Desconto);
                    }


                   // PriEngine.Engine.Comercial.Compras.TransformaDocumento(

                    PriEngine.Engine.IniciaTransaccao();
                    //PriEngine.Engine.Comercial.Vendas.Edita Actualiza(myEnc, "Teste");
                    PriEngine.Engine.Comercial.Vendas.Actualiza(myEnc, "Teste");
                    PriEngine.Engine.TerminaTransaccao();
                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir empresa";
                    return erro;

                }

            }
            catch (Exception ex)
            {
                PriEngine.Engine.DesfazTransaccao();
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

     

        public static List<Model.DocVenda> Encomendas_List()
        {
            
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocVenda dv = new Model.DocVenda();
            List<Model.DocVenda> listdv = new List<Model.DocVenda>();
            Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
            List<Model.LinhaDocVenda> listlindv = new
            List<Model.LinhaDocVenda>();

            if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL'");
                while (!objListCab.NoFim())
                {
                    dv = new Model.DocVenda();
                    dv.id = objListCab.Valor("id");
                    dv.Entidade = objListCab.Valor("Entidade");
                    dv.NumDoc = objListCab.Valor("NumDoc");
                    dv.Data = objListCab.Valor("Data");
                    dv.TotalMerc = objListCab.Valor("TotalMerc");
                    dv.Serie = objListCab.Valor("Serie");
                    objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                    listlindv = new List<Model.LinhaDocVenda>();

                    while (!objListLin.NoFim())
                    {
                        lindv = new Model.LinhaDocVenda();
                        lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                        lindv.CodArtigo = objListLin.Valor("Artigo");
                        lindv.DescArtigo = objListLin.Valor("Descricao");
                        lindv.Quantidade = objListLin.Valor("Quantidade");
                        lindv.Unidade = objListLin.Valor("Unidade");
                        lindv.Desconto = objListLin.Valor("Desconto1");
                        lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");

                        listlindv.Add(lindv);
                        objListLin.Seguinte();
                    }

                    dv.LinhasDoc = listlindv;
                    listdv.Add(dv);
                    objListCab.Seguinte();
                }
            }
            return listdv;
        }*/

        /*  public static List<Model.DocVenda> Encomendas_List(string empresa, string login, string password)
          {

              StdBELista objListCab;
              StdBELista objListLin;
              Model.DocVenda dv = new Model.DocVenda();
              List<Model.DocVenda> listdv = new List<Model.DocVenda>();
              Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
              List<Model.LinhaDocVenda> listlindv = new
              List<Model.LinhaDocVenda>();

              if (PriEngine.InitializeCompany(empresa, login, password) == true)
              {
                  objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL'");
                  while (!objListCab.NoFim())
                  {
                      dv = new Model.DocVenda();
                      dv.id = objListCab.Valor("id");
                      dv.Entidade = objListCab.Valor("Entidade");
                      dv.NumDoc = objListCab.Valor("NumDoc");
                      dv.Data = objListCab.Valor("Data");
                      dv.TotalMerc = objListCab.Valor("TotalMerc");
                      dv.Serie = objListCab.Valor("Serie");
                      objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                      listlindv = new List<Model.LinhaDocVenda>();

                      while (!objListLin.NoFim())
                      {
                          lindv = new Model.LinhaDocVenda();
                          lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                          lindv.CodArtigo = objListLin.Valor("Artigo");
                          lindv.DescArtigo = objListLin.Valor("Descricao");
                          lindv.Quantidade = objListLin.Valor("Quantidade");
                          lindv.Unidade = objListLin.Valor("Unidade");
                          lindv.Desconto = objListLin.Valor("Desconto1");
                          lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                          lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                          lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");

                          listlindv.Add(lindv);
                          objListLin.Seguinte();
                      }

                      dv.LinhasDoc = listlindv;
                      listdv.Add(dv);
                      objListCab.Seguinte();
                  }
              }
              return listdv;
          }
       

          public static Model.DocVenda Encomenda_Get(string numdoc)
          {
            
            
              StdBELista objListCab;
              StdBELista objListLin;
              Model.DocVenda dv = new Model.DocVenda();
              Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
              List<Model.LinhaDocVenda> listlindv = new List<Model.LinhaDocVenda>();

              if (PriEngine.InitializeCompany(DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()) == true)
              {
                

                  string st = "SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL' and NumDoc='" + numdoc + "'";
                  objListCab = PriEngine.Engine.Consulta(st);
                  dv = new Model.DocVenda();
                  dv.id = objListCab.Valor("id");
                  dv.Entidade = objListCab.Valor("Entidade");
                  dv.NumDoc = objListCab.Valor("NumDoc");
                  dv.Data = objListCab.Valor("Data");
                  dv.TotalMerc = objListCab.Valor("TotalMerc");
                  dv.Serie = objListCab.Valor("Serie");
                  objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                  listlindv = new List<Model.LinhaDocVenda>();

                  while (!objListLin.NoFim())
                  {
                      lindv = new Model.LinhaDocVenda();
                      lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                      lindv.CodArtigo = objListLin.Valor("Artigo");
                      lindv.DescArtigo = objListLin.Valor("Descricao");
                      lindv.Quantidade = objListLin.Valor("Quantidade");
                      lindv.Unidade = objListLin.Valor("Unidade");
                      lindv.Desconto = objListLin.Valor("Desconto1");
                      lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                      lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                      lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");
                      listlindv.Add(lindv);
                      objListLin.Seguinte();
                  }

                  dv.LinhasDoc = listlindv;
                  return dv;
              }
              return null;
          }
          public static Model.DocVenda Encomenda_Get(string numdoc, string empresa, string user, string password)
          {
              StdBELista objListCab;
              StdBELista objListLin;
              Model.DocVenda dv = new Model.DocVenda();
              Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
              List<Model.LinhaDocVenda> listlindv = new List<Model.LinhaDocVenda>();

              if (PriEngine.InitializeCompany(empresa,user,password) == true)
              {


                  string st = "SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL' and id='" + numdoc + "'";
                  objListCab = PriEngine.Engine.Consulta(st);
                  dv = new Model.DocVenda();
                  dv.id = objListCab.Valor("id");
                  dv.Entidade = objListCab.Valor("Entidade");
                  dv.NumDoc = objListCab.Valor("NumDoc");
                  dv.Data = objListCab.Valor("Data");
                  dv.TotalMerc = objListCab.Valor("TotalMerc");
                  dv.Serie = objListCab.Valor("Serie");
                  objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                  listlindv = new List<Model.LinhaDocVenda>();

                  while (!objListLin.NoFim())
                  {
                      lindv = new Model.LinhaDocVenda();
                      lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                      lindv.CodArtigo = objListLin.Valor("Artigo");
                      lindv.DescArtigo = objListLin.Valor("Descricao");
                      lindv.Quantidade = objListLin.Valor("Quantidade");
                      lindv.Unidade = objListLin.Valor("Unidade");
                      lindv.Desconto = objListLin.Valor("Desconto1");
                      lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                      lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                      lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");
                      listlindv.Add(lindv);
                      objListLin.Seguinte();
                  }

                  dv.LinhasDoc = listlindv;
                  return dv;
              }
              return null;
          }
          */
        #endregion DocsVenda

        #region encomendas


        public static List<Model.Encomenda> ListaEncomendas(string company, string user, string password)
        {

            StdBELista objList;
            StdBELista objListLin;
            List<Model.LinhaDocCompra> listlindc;
            Model.Encomenda enc;
            Model.LinhaDocCompra lindc;
            List<Model.Encomenda> listEncs = new List<Model.Encomenda>();

            //if (PriEngine.InitializeCompany(company, user, password) == true)
            //{

                objList = PriEngine.Engine.Consulta("SELECT * FROM CabecCompras WHERE TipoDoc='ECF'");

                while (!objList.NoFim())
                {
                    enc = new Model.Encomenda();
                    enc.CodEncomenda = objList.Valor("Id");
                    enc.Entidade = objList.Valor("Entidade");
                    enc.DataDoc = objList.Valor("DataDoc");
                    enc.ModoPag = objList.Valor("ModoPag");
                    enc.TotalMerc = objList.Valor("TotalMerc");
                    enc.TotalIva = objList.Valor("TotalIva");
                    enc.TotalDesc = objList.Valor("TotalDesc");
                    enc.Origem = objList.Valor("Morada");

                    objListLin = PriEngine.Engine.Consulta("SELECT id, idCabecCompras, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Armazem, Lote from LinhasCompras where IdCabecCompras='" + enc.CodEncomenda + "' order By NumLinha");
                    listlindc = new List<Model.LinhaDocCompra>();

                    while (!objListLin.NoFim())
                    {
                        lindc = new Model.LinhaDocCompra();
                        lindc.Id = objListLin.Valor("id");
                        lindc.IdCabecDoc = objListLin.Valor("idCabecCompras");
                        lindc.CodArtigo = objListLin.Valor("Artigo");
                        lindc.DescArtigo = objListLin.Valor("Descricao");
                        lindc.Quantidade = objListLin.Valor("Quantidade");
                        lindc.Unidade = objListLin.Valor("Unidade");
                        lindc.Desconto = objListLin.Valor("Desconto1");
                        lindc.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindc.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindc.TotalLiquido = objListLin.Valor("PrecoLiquido");
                        lindc.Armazem = objListLin.Valor("Armazem");
                        lindc.Lote = objListLin.Valor("Lote");

                        listlindc.Add(lindc);
                        objListLin.Seguinte();
                    }

                    enc.LinhasEnc = listlindc;

                    listEncs.Add(enc);
                    objList.Seguinte();
                }

                return listEncs;

            //}
            /*else
            {
                return null;
            }*/

        }
        public static Model.Encomenda GetEncomenda(string id,string company, string user, string password)
        {
            StdBELista objList;
            StdBELista objListLin;
            List<Model.LinhaDocCompra> listlindc;
            Model.Encomenda enc;
            Model.LinhaDocCompra lindc;
            if (PriEngine.InitializeCompany(company, user, password) == true)
            {
                objList = PriEngine.Engine.Consulta("SELECT * FROM CabecCompras WHERE Id='"+id+"' AND TipoDoc='ECF'");

                enc = new Model.Encomenda();
                enc.CodEncomenda = objList.Valor("Id");
                enc.Entidade = objList.Valor("Entidade");
                enc.DataDoc = objList.Valor("DataDoc");
                enc.ModoPag = objList.Valor("ModoPag");
                enc.TotalMerc = objList.Valor("TotalMerc");
                enc.TotalIva = objList.Valor("TotalIva");
                enc.TotalDesc = objList.Valor("TotalDesc");
                enc.Origem = objList.Valor("Morada");

                objListLin = PriEngine.Engine.Consulta("SELECT idCabecCompras, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Armazem, Lote from LinhasCompras where IdCabecCompras='" + enc.CodEncomenda + "' order By NumLinha");
                listlindc = new List<Model.LinhaDocCompra>();

                while (!objListLin.NoFim())
                {
                    lindc = new Model.LinhaDocCompra();
                    lindc.IdCabecDoc = objListLin.Valor("idCabecCompras");
                    lindc.CodArtigo = objListLin.Valor("Artigo");
                    lindc.DescArtigo = objListLin.Valor("Descricao");
                    lindc.Quantidade = objListLin.Valor("Quantidade");
                    lindc.Unidade = objListLin.Valor("Unidade");
                    lindc.Desconto = objListLin.Valor("Desconto1");
                    lindc.PrecoUnitario = objListLin.Valor("PrecUnit");
                    lindc.TotalILiquido = objListLin.Valor("TotalILiquido");
                    lindc.TotalLiquido = objListLin.Valor("PrecoLiquido");
                    lindc.Armazem = objListLin.Valor("Armazem");
                    lindc.Lote = objListLin.Valor("Lote");

                    listlindc.Add(lindc);
                    objListLin.Seguinte();
                }
                enc.LinhasEnc = listlindc;
                return enc;
            }
            else{
                return null;
            }
        }
        public static List<Model.Encomenda> ListaEncomendasFiltrado(char type/*,string company, string user, string password*/)
        {

            StdBELista objList;
            StdBELista objListLin;
            List<Model.LinhaDocCompra> listlindc;
            Model.Encomenda enc;
            Model.LinhaDocCompra lindc;
            List<Model.Encomenda> listEncs = new List<Model.Encomenda>();
            string test;
            string query = "";

           // if (PriEngine.InitializeCompany(company, user, password) == true)
            //{
                query += "SELECT * FROM CabecCompras WHERE Id IN(";
                query += "SELECT IdCabecCompras FROM LinhasCompras WHERE IdCabecCompras IN (";
                query += "SELECT Id FROM CabecCompras WHERE TipoDoc='ECF') AND ";
                query += "Id IN (Select IdLinhasCompras FROM LinhasComprasStatus WHERE EstadoTrans='"+type+"')) ";
                query += "ORDER BY Entidade;";
                objList = PriEngine.Engine.Consulta(query);

                while (!objList.NoFim())
                {
                    
                        enc = new Model.Encomenda();
                        enc.CodEncomenda = objList.Valor("Id");
                        enc.Entidade = objList.Valor("Entidade");
                        enc.DataDoc = objList.Valor("DataDoc");
                        enc.ModoPag = objList.Valor("ModoPag");
                        enc.TotalMerc = objList.Valor("TotalMerc");
                        enc.TotalIva = objList.Valor("TotalIva");
                        enc.TotalDesc = objList.Valor("TotalDesc");
                        enc.Origem = objList.Valor("Morada");

                        objListLin = PriEngine.Engine.Consulta("SELECT id, idCabecCompras, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Armazem, Lote, IntrastatPautal from LinhasCompras where IdCabecCompras='" + enc.CodEncomenda + "' AND Artigo IS NOT NULL order By NumLinha");
                        listlindc = new List<Model.LinhaDocCompra>();

                        while (!objListLin.NoFim())
                        {
                            test = objListLin.Valor("Artigo");
                            test = objListLin.Valor("IntrastatPautal");
                                lindc = new Model.LinhaDocCompra();
                                lindc.Id = objListLin.Valor("id");
                                lindc.IdCabecDoc = objListLin.Valor("idCabecCompras");
                                lindc.CodArtigo = objListLin.Valor("Artigo");
                                lindc.DescArtigo = objListLin.Valor("Descricao");
                                lindc.Quantidade = objListLin.Valor("Quantidade");
                                lindc.Unidade = objListLin.Valor("Unidade");
                                lindc.Desconto = objListLin.Valor("Desconto1");
                                lindc.PrecoUnitario = objListLin.Valor("PrecUnit");
                                lindc.TotalILiquido = objListLin.Valor("TotalILiquido");
                                lindc.TotalLiquido = objListLin.Valor("PrecoLiquido");
                                lindc.Armazem = objListLin.Valor("Armazem");
                                lindc.Lote = objListLin.Valor("Lote");

                                listlindc.Add(lindc);
                            objListLin.Seguinte();
                        }
                        enc.LinhasEnc = listlindc;
                        listEncs.Add(enc);
                        objList.Seguinte();
                }

                return listEncs;

            /*}
            else
            {
                return null;
            }*/

        }

        #endregion

    }
}