using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using DashboardIntegration.Lib_Primavera.Model;

namespace DashboardIntegration.Parser
{
    public class StagingAPI
    {

        public static string empresa = "DEMOSINF";
        static string connectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + System.AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Staging.mdf;Integrated Security=True;MultipleActiveResultSets=true";

        public static Empresa getEmpresa(string id)
        {
            Empresa e;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Empresa WHERE Id='" + id + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;
                e = new Empresa
                {
                    Nome = (string)rec[1],
                    NumContrib = (string)rec[2],
                    Morada = (string)rec[3],
                    Local = (string)rec[4],
                    Cp = (string)rec[5]
                };
            }
            return e;
        }

        #region Cliente
        //Listar clientes
        public static IEnumerable<Cliente> ListaClientes()
        {
            List<Cliente> listClientes = new List<Cliente>();
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Cliente WHERE EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    SqlCommand fk = new SqlCommand("Select * FROM BillingAddress WHERE Id=" + (Int32)rec[3] + " AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    fkr.Read();
                    IDataRecord ba = (IDataRecord)fkr;
                    Cliente cliente = new Cliente
                    {
                        CodCliente = (string)rec[0],
                        NumContribuinte = (string)rec[1],
                        NomeCliente = (string)rec[2],
                        Morada = (string)ba[1] + " " + (string)ba[2] + " " + (string)ba[3] + " " + (string)ba[4]
                    };
                    listClientes.Add(cliente);
                    fkr.Close();
                }
                dr.Close();
            }
            return listClientes;
        }
        //Obter clientes com os quais fizemos negócio neste mês
        //Obter número de artigos vendidos em um mês
        public static int getTotalClientesMes(int m)
        {
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                int n = 0;
                SqlCommand c = new SqlCommand("SELECT COUNT(DISTINCT CustomerID) FROM Invoice WHERE Month(InvoiceDate) =" + m + "AND Year(InvoiceDate) = 2016 AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                try
                {
                    dr.Read();
                    IDataRecord rec = (IDataRecord)dr;
                    n = (int)rec[0];
                    dr.Close();
                }
                catch (Exception e)
                {
                    dr.Close();
                    return -1;
                }
                return n;
            }
        }
        public static List<String> getNomeClientes()
        {
            List<String> clientes = new List<String>();
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT CustomerID FROM Cliente WHERE EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    clientes.Add((string)rec[0]);
                }
                dr.Close();
            }
            return clientes;
        }
        public static int getTotalNumberSales(string cliente)
        {
            int t = 0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT COUNT(*) FROM Invoice WHERE CustomerID='" + cliente + "' AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    t += (int)rec[0];
                }
                dr.Close();
            }
            return t;
        }
        //Obter um cliente
        public static Cliente GetCliente(string id)
        {
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Cliente WHERE CustomerID='" + id + "' AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;

                try
                {
                    SqlCommand fk = new SqlCommand("Select * FROM BillingAddress WHERE Id=" + (Int32)rec[3] + " AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    fkr.Read();
                    IDataRecord fkrdr = (IDataRecord)fkr;

                    List<DocVenda> listdv = new List<DocVenda>();
                    List<LinhaDocVenda> listlindv;
                    c = new SqlCommand("SELECT * FROM Invoice WHERE CustomerID='" + id + "' AND EmpresaId='" + empresa + "'", sc);
                    dr = c.ExecuteReader();
                    while (dr.Read())
                    {
                        IDataRecord rec2 = (IDataRecord)dr;
                        fk = new SqlCommand("Select * FROM Line WHERE InvoiceNo='" + rec2[0] + "' AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader fkr2 = fk.ExecuteReader();
                        listlindv = new List<LinhaDocVenda>();
                        while (fkr2.Read())
                        {
                            IDataReader ldcvr = (IDataReader)fkr2;
                            listlindv.Add(new LinhaDocVenda
                            {
                                Id = (int)ldcvr[0],
                                LineNumber = (int)ldcvr[1],
                                ProductCode = (string)ldcvr[2],
                                Quantity = (int)ldcvr[3],
                                UnitPrice = (double)(decimal)ldcvr[4],
                                TaxPointDate = (System.DateTime)ldcvr[5],
                                Description = (string)ldcvr[6],
                                CreditAmount = (double)(decimal)ldcvr[7],
                                TaxType = (string)ldcvr[8],
                                TaxCountryRegion = (string)ldcvr[9],
                                TaxCode = (string)ldcvr[10],
                                TaxPercentage = (double)ldcvr[11],
                                SettlementAmount = (double)ldcvr[12],
                                InvoiceNo = (string)ldcvr[13]
                            });
                        }
                        DocVenda venda = new DocVenda
                        {
                            InvoiceNo = (string)rec2[0],
                            InvoiceStatus = (string)rec2[1],
                            HashControl = (bool)rec2[2],
                            Hash = (string)rec2[3],
                            Period = (int)rec2[4],
                            InvoiceDate = (System.DateTime)rec2[5],
                            InvoiceType = (string)rec2[6],
                            SelfBillingIndicator = (bool)rec2[7],
                            SystemEntryDate = (System.DateTime)rec2[8],
                            CustomerID = (string)rec2[9],
                            TaxPayable = (double)(decimal)rec2[10],
                            NetTotal = (double)(decimal)rec2[11],
                            GrossTotal = (double)(decimal)rec2[12],
                            LinhasDoc = listlindv
                        };
                        listdv.Add(venda);
                        fkr2.Close();
                    }
                    Cliente cliente = new Cliente
                    {
                        CodCliente = (string)rec[0],
                        NumContribuinte = (string)rec[1],
                        NomeCliente = (string)rec[2],
                        Morada = (string)fkrdr[1] + " " + (string)fkrdr[2] + " " + (string)fkrdr[3] + " " + (string)fkrdr[4],
                        Vendas = listdv
                    };
                    fkr.Close();
                    dr.Close();
                    return cliente;
                }
                catch (Exception e)
                {
                    dr.Close();
                    return null;
                }

            }
        }
        #endregion
        #region Artigo
        //Lista de artigos
        public static IEnumerable<Artigo> ListaArtigos()
        {
            List<Artigo> listArts = new List<Artigo>();
            List<TransaccaoArtigo> lt;
            TransaccaoArtigo t;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Artigo WHERE EmpresaId='"+empresa+"'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    lt = new List<TransaccaoArtigo>();
                    SqlCommand ct = new SqlCommand("SELECT Quantidade, NumLinha, IdCabecDoc FROM LinePurchases WHERE CodArtigo='"+(string)rec[0]+"'AND EmpresaId='"+empresa+"';", sc);
                    SqlDataReader drt = ct.ExecuteReader();
                    while (drt.Read())
                    {
                        IDataRecord rect = (IDataRecord)drt;
                        SqlCommand ctl = new SqlCommand("SELECT Entidade, Data FROM Purchases WHERE Id='" + (string)rect[2] + "' AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader drtl = ctl.ExecuteReader();
                        if (!drtl.Read())
                        {
                            ctl = new SqlCommand("SELECT Entidade, DataDoc FROM EncomendasFornecedor WHERE CodEncomenda='" + (string)rect[2] + "' AND EmpresaId='"+empresa+"'", sc);
                            drtl = ctl.ExecuteReader();
                            drtl.Read();
                        }

                        IDataRecord rectl = (IDataRecord)drtl;
                        t = new TransaccaoArtigo
                        {
                            entidade = (string)rectl[0],
                            iddoc = (string)rect[2],
                            date = (DateTime)rectl[1],
                            quantity = (int)rect[0],
                            linha = (int)rect[1],
                            tipo = false
                        };
                        lt.Add(t);
                    }
                    ct = new SqlCommand("SELECT Quantity, LineNumber, InvoiceNo FROM Line WHERE ProductCode='" + (string)rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    drt = ct.ExecuteReader();
                    while (drt.Read())
                    {
                        IDataRecord rect = (IDataRecord)drt;
                        SqlCommand ctl = new SqlCommand("SELECT CustomerID, InvoiceDate FROM Invoice WHERE InvoiceNo='" + (string)rect[2] + "' AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader drtl = ctl.ExecuteReader();
                        drtl.Read();
                        IDataRecord rectl = (IDataRecord)drtl;
                        string entidade = (string)rectl[0];
                        string iddoc = (string)rect[2];
                        DateTime date = (DateTime)rectl[1];
                        int quantity = (int)rect[0];
                        int linha = (int)rect[1];
                        t = new TransaccaoArtigo
                        {
                            entidade = (string)rectl[0],
                            iddoc = (string)rect[2],
                            date = (DateTime)rectl[1],
                            quantity = (int)rect[0],
                            linha = (int)rect[1],
                            tipo = true
                        };
                        lt.Add(t);
                    }
                    Artigo art = new Artigo
                    {
                        CodArtigo = (string)rec[0],
                        DescArtigo = (string)rec[1],
                        TipoArtigo = (string)rec[2],
                        STKAtual = (double)rec[3],
                        transaccoes = lt
                    };
                    listArts.Add(art);

                }
                dr.Close();
            }
            return listArts;
        }
        //Obter um artigo
        public static Artigo GetArtigo(string id)
        {
            List<TransaccaoArtigo> lt = new List<TransaccaoArtigo>();
            TransaccaoArtigo t;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Artigo WHERE CodArtigo='" + id + "'AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;

                try
                {
                    lt = new List<TransaccaoArtigo>();
                    SqlCommand ct = new SqlCommand("SELECT Quantidade, NumLinha, IdCabecDoc FROM LinePurchases WHERE CodArtigo='" + id + "'AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader drt = ct.ExecuteReader();
                    while (drt.Read())
                    {
                        IDataRecord rect = (IDataRecord)drt;
                        SqlCommand ctl = new SqlCommand("SELECT Entidade, Data FROM Purchases WHERE Id='" + (string)rect[2] + "'AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader drtl = ctl.ExecuteReader();
                        if (!drtl.Read())
                        {
                            ctl = new SqlCommand("SELECT Entidade, DataDoc FROM EncomendasFornecedor WHERE CodEncomenda='" + (string)rect[2] + "'AND EmpresaId='" + empresa + "'", sc);
                            drtl = ctl.ExecuteReader();
                            drtl.Read();
                        }
                        IDataRecord rectl = (IDataRecord)drtl;
                        t = new TransaccaoArtigo
                        {
                            entidade = (string)rectl[0],
                            iddoc = (string)rect[2],
                            date = (DateTime)rectl[1],
                            quantity = (int)rect[0],
                            linha = (int)rect[1],
                            tipo = false
                        };
                        lt.Add(t);
                    }
                    ct = new SqlCommand("SELECT Quantity, LineNumber, InvoiceNo FROM Line WHERE ProductCode='" + id + "'AND EmpresaId='" + empresa + "'", sc);
                    drt = ct.ExecuteReader();
                    while (drt.Read())
                    {
                        IDataRecord rect = (IDataRecord)drt;
                        SqlCommand ctl = new SqlCommand("SELECT CustomerID, InvoiceDate FROM Invoice WHERE InvoiceNo='" + (string)rect[2] + "'AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader drtl = ctl.ExecuteReader();
                        drtl.Read();
                        IDataRecord rectl = (IDataRecord)drtl;
                        t = new TransaccaoArtigo
                        {
                            entidade = (string)rectl[0],
                            iddoc = (string)rect[2],
                            date = (DateTime)rectl[1],
                            quantity = (int)rect[0],
                            linha = (int)rect[1],
                            tipo = true
                        };
                        lt.Add(t);
                    }
                    Artigo art = new Artigo
                    {
                        CodArtigo = (string)rec[0],
                        DescArtigo = (string)rec[1],
                        TipoArtigo = (string)rec[2],
                        STKAtual = (double)rec[3],
                        transaccoes = lt
                    };
                    dr.Close();
                    return art;
                }
                catch (Exception e)
                {
                    dr.Close();
                    return null;
                }

            }
        }
        //Obter todo o stock
        public static double getStkTotal()
        {
            double n = 0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();

                SqlCommand c = new SqlCommand("SELECT SUM(STKAtual) FROM Artigo WHERE EmpresaId='" + empresa + "';", sc);
                SqlDataReader dr = c.ExecuteReader();
                try
                {
                    dr.Read();
                    IDataRecord rec = (IDataRecord)dr;
                    n = (double)rec[0];
                    dr.Close();
                }
                catch (Exception e)
                {
                    dr.Close();
                    return -1;
                }
            }
            return n;
        }
        public static List<String> getIdArtigos( )
        {
            List<String> arts = new List<String>();
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT CodArtigo FROM Artigo WHERE EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    arts.Add((string)rec[0]);
                }
                dr.Close();
            }
            return arts;
        }
        public static string getArtName(string code )
        {
            string name = "";
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT DescArtigo FROM Artigo WHERE CodArtigo='" + code + "'AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;
                name = (string)rec[0];
                dr.Close();
            }
            return name;
        }
        public static int getTotalNumberProductSales(string artigo  )
        {
            int t = 0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT Quantity FROM Line WHERE ProductCode='" + artigo + "'AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    t += (int)rec[0];
                }
                dr.Close();
            }
            return t;
        }
        #endregion
        #region Fornecedores
        //Listar fornecedores
        public static IEnumerable<Fornecedor> ListaFornecedores( )
        {
            List<Fornecedor> listFc = new List<Fornecedor>();
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Supplier WHERE EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    Fornecedor f = new Fornecedor
                    {
                        CodFornecedor = (string)rec[0],
                        Nome = (string)rec[1],
                        Morada = (string)rec[2],
                        Local = (string)rec[3],
                        Cp = (string)rec[4],
                        Tel = (string)rec[5],
                        Divida = (double)(decimal)rec[6],
                        NumContrib = (string)rec[7],
                        ModoPag = (string)rec[8],
                        EnderecoWeb = (string)rec[9],
                        EncomendasPendentes = (double)(decimal)rec[10]
                    };
                    listFc.Add(f);

                }
                dr.Close();
            }
            return listFc;
        }
        public static List<String> getIdFornecedores( )
        {
            List<String> forn = new List<String>();
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT CodFornecedor FROM Supplier WHERE EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    forn.Add((string)rec[0]);
                }
                dr.Close();
            }
            return forn;
        }
        public static string getNomeFornecedores(string id)
        {
            string forn = "";
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT Nome FROM Supplier WHERE CodFornecedor='" + id + "'AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;
                forn = (string)rec[0];
                dr.Close();
            }
            return forn;
        }
        public static int getTotalNumberPurchases(string forn )
        {
            int t = 0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT COUNT(*) FROM Purchases WHERE Entidade='" + forn + "'AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    t += (int)rec[0];
                }
                dr.Close();
            }
            return t;
        }
        public static double getTotalNumberPurchasesValue(string forn)
        {
            double t = 0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT TotalMerc FROM Purchases WHERE Entidade='" + forn + "'AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    t += (double)(decimal)rec[0];
                }
                dr.Close();
            }
            return t;
        }
        public static double getTotalDividaFornecedor( )
        {
            double t = 0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT SUM(Divida) FROM Supplier WHERE EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    t += (double)(decimal)rec[0];
                }
                dr.Close();
            }
            return Math.Abs(t);
        }
        //Obter um fornecedor
        public static Fornecedor GetFornecedor(string id)
        {
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Supplier WHERE CodFornecedor='" + id + "'AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;

                try
                {
                    // get compras do fornecedor
                    List<DocCompra> listdc = new List<DocCompra>();
                    List<LinhaDocCompra> listlindc;
                    c = new SqlCommand("SELECT * FROM Purchases WHERE Entidade='" + id + "'AND EmpresaId='" + empresa + "'", sc);
                    dr = c.ExecuteReader();
                    while (dr.Read())
                    {
                        IDataRecord rec2 = (IDataRecord)dr;
                        SqlCommand fk = new SqlCommand("Select * FROM LinePurchases WHERE IdCabecDoc='" + rec2[0] + "'AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader fkr = fk.ExecuteReader();
                        listlindc = new List<LinhaDocCompra>();
                        while (fkr.Read())
                        {
                            IDataReader ldcdr = (IDataReader)fkr;
                            listlindc.Add(new LinhaDocCompra
                            {
                                Id = (string)ldcdr[0],
                                IdCabecDoc = (string)ldcdr[1],
                                CodArtigo = (string)ldcdr[2],
                                DescArtigo = (string)ldcdr[3],
                                Quantidade = (int)ldcdr[4],
                                Unidade = (string)ldcdr[5],
                                Desconto = (int)ldcdr[6],
                                PrecoUnitario = (double)(decimal)ldcdr[7],
                                TotalLiquido = (double)(decimal)ldcdr[8],
                                TotalILiquido = (double)(decimal)ldcdr[9],
                                Armazem = (string)ldcdr[10],
                                Lote = (string)ldcdr[11]
                            });
                        }
                        DocCompra compra = new DocCompra
                        {
                            id = (string)rec2[0],
                            NumDocExterno = ((int)rec2[1]).ToString(),
                            Entidade = (string)rec2[2],
                            NumDoc = (int)rec2[3],
                            Data = (System.DateTime)rec2[4],
                            TotalMerc = (double)(decimal)rec2[5],
                            Serie = (string)rec2[6],
                            LinhasDoc = listlindc
                        };
                        listdc.Add(compra);
                        fkr.Close();
                    }

                    //get encomendas fornecedor
                    List<Encomenda> liste = new List<Encomenda>();
                    c = new SqlCommand("SELECT * FROM EncomendasFornecedor WHERE Entidade='" + id + "'AND EmpresaId='" + empresa + "'", sc);
                    dr = c.ExecuteReader();
                    while (dr.Read())
                    {
                        IDataRecord rec2 = (IDataRecord)dr;
                        SqlCommand fk = new SqlCommand("Select * FROM LinePurchases WHERE IdCabecDoc='" + rec2[0] + "'AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader fkr = fk.ExecuteReader();
                        listlindc = new List<LinhaDocCompra>();
                        while (fkr.Read())
                        {
                            IDataReader ldcdr = (IDataReader)fkr;
                            listlindc.Add(new LinhaDocCompra
                            {
                                Id = (string)ldcdr[0],
                                IdCabecDoc = (string)ldcdr[1],
                                CodArtigo = (string)ldcdr[2],
                                DescArtigo = (string)ldcdr[3],
                                Quantidade = (int)ldcdr[4],
                                Unidade = (string)ldcdr[5],
                                Desconto = (int)ldcdr[6],
                                PrecoUnitario = (double)(decimal)ldcdr[7],
                                TotalLiquido = (double)(decimal)ldcdr[8],
                                TotalILiquido = (double)(decimal)ldcdr[9],
                                Armazem = (string)ldcdr[10],
                                Lote = (string)ldcdr[11]
                            });
                        }
                        Encomenda enc = new Encomenda
                        {
                            CodEncomenda = (string)rec2[0],
                            Entidade = (string)rec2[1],
                            Origem = (string)rec2[2],
                            TotalDesc = (double)(decimal)rec2[3],
                            TotalIva = (double)(decimal)rec2[4],
                            TotalMerc = (double)(decimal)rec2[5],
                            ModoPag = (string)rec2[6],
                            DataDoc = (System.DateTime)rec2[7],
                            Entregue = (bool)rec2[8],
                            LinhasEnc = listlindc
                        };
                        liste.Add(enc);
                        fkr.Close();
                    }

                    Fornecedor f = new Fornecedor
                    {
                        CodFornecedor = (string)rec[0],
                        Nome = (string)rec[1],
                        Morada = (string)rec[2],
                        Local = (string)rec[3],
                        Cp = (string)rec[4],
                        Tel = (string)rec[5],
                        Divida = (double)(decimal)rec[6],
                        NumContrib = (string)rec[7],
                        ModoPag = (string)rec[8],
                        EnderecoWeb = (string)rec[9],
                        EncomendasPendentes = (double)(decimal)rec[10],
                        Compras = listdc,
                        Encomendas = liste
                    };
                    dr.Close();
                    return f;
                }
                catch (Exception e)
                {
                    dr.Close();
                    return null;
                }

            }
        }
        #endregion
        #region DocCompra
        //Listar compras
        public static IEnumerable<DocCompra> VGR_List()
        {
            List<DocCompra> listdc = new List<DocCompra>();
            List<LinhaDocCompra> listlindc;
            Empresa e;
            Fornecedor f;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Purchases WHERE EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    SqlCommand fk = new SqlCommand("Select * FROM LinePurchases WHERE IdCabecDoc='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    listlindc = new List<LinhaDocCompra>();
                    while (fkr.Read())
                    {
                        IDataReader ldcdr = (IDataReader)fkr;
                        listlindc.Add(new LinhaDocCompra
                        {
                            Id = (string)ldcdr[0],
                            IdCabecDoc = (string)ldcdr[1],
                            CodArtigo = (string)ldcdr[2],
                            DescArtigo = (string)ldcdr[3],
                            Quantidade = (int)ldcdr[4],
                            Unidade = (string)ldcdr[5],
                            Desconto = (int)ldcdr[6],
                            PrecoUnitario = (double)(decimal)ldcdr[7],
                            TotalLiquido = (double)(decimal)ldcdr[8],
                            TotalILiquido = (double)(decimal)ldcdr[9],
                            Armazem = (string)ldcdr[10],
                            Lote = (string)ldcdr[11]
                        });
                    }
                    f = GetFornecedor((string)rec[2]);
                    e = getEmpresa((string)rec[7]);
                    DocCompra compra = new DocCompra
                    {
                        id = (string)rec[0],
                        NumDocExterno = ((int)rec[1]).ToString(),
                        Entidade = (string)rec[2],
                        NumDoc = (int)rec[3],
                        Data = (System.DateTime)rec[4],
                        TotalMerc = (double)(decimal)rec[5],
                        Serie = (string)rec[6],
                        LinhasDoc = listlindc,
                        dadosEmpresa = e,
                        dadosFornecedor = f
                    };
                    listdc.Add(compra);
                    fkr.Close();
                }
                dr.Close();
            }
            return listdc;
        }
        //Obter número de artigos vendidos em um mês
        public static int getNumArtigosVendidosMes(int m)
        {
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                int n = 0;
                SqlCommand c = new SqlCommand("SELECT InvoiceNo FROM Invoice WHERE Month(InvoiceDate) =" + m + "AND Year(InvoiceDate) = 2016 AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                try{
                    while (dr.Read())
                    {
                        IDataRecord rec = (IDataRecord)dr;
                        SqlCommand l = new SqlCommand("SELECT SUM(Quantity) FROM Line WHERE InvoiceNo ='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader drl = l.ExecuteReader();
                        drl.Read();
                        IDataRecord recl = (IDataRecord)drl;
                        n += (int)recl[0];
                    }
                }
                catch (Exception e)
                {
                    dr.Close();
                    return -1;
                }
                return n;

            }
        }
        //Listar compras por mês
        public static List<DocCompra> VGR_List(int month)
        {
            List<DocCompra> listdc = new List<DocCompra>();
            List<LinhaDocCompra> listlindc;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                Fornecedor f;
                Empresa e;
                SqlCommand c = new SqlCommand("SELECT * FROM Purchases WHERE Month(Data) =" + month + "AND Year(Data) = 2016 AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    SqlCommand fk = new SqlCommand("Select * FROM LinePurchases WHERE IdCabecDoc='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    listlindc = new List<LinhaDocCompra>();
                    while (fkr.Read())
                    {
                        IDataReader ldcdr = (IDataReader)fkr;
                        listlindc.Add(new LinhaDocCompra
                        {
                            Id = (string)ldcdr[0],
                            IdCabecDoc = (string)ldcdr[1],
                            CodArtigo = (string)ldcdr[2],
                            DescArtigo = (string)ldcdr[3],
                            Quantidade = (int)ldcdr[4],
                            Unidade = (string)ldcdr[5],
                            Desconto = (int)ldcdr[6],
                            PrecoUnitario = (double)(decimal)ldcdr[7],
                            TotalLiquido = (double)(decimal)ldcdr[8],
                            TotalILiquido = (double)(decimal)ldcdr[9],
                            Armazem = (string)ldcdr[10],
                            Lote = (string)ldcdr[11]
                        });
                    }
                    f = GetFornecedor((string)rec[2]);
                    e = getEmpresa((string)rec[7]);
                    DocCompra compra = new DocCompra
                    {
                        id = (string)rec[0],
                        NumDocExterno = ((int)rec[1]).ToString(),
                        Entidade = (string)rec[2],
                        NumDoc = (int)rec[3],
                        Data = (System.DateTime)rec[4],
                        TotalMerc = (double)(decimal)rec[5],
                        Serie = (string)rec[6],
                        LinhasDoc = listlindc,
                        dadosEmpresa = e,
                        dadosFornecedor = f
                    };
                    listdc.Add(compra);
                    fkr.Close();
                }
                dr.Close();
            }
            return listdc;
        }
        //Obter um doccompra
        public static DocCompra GetCompra(string id)
        {
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                Fornecedor f;
                Empresa e;
                List<LinhaDocCompra> listlindc;
                SqlCommand c = new SqlCommand("SELECT * FROM Purchases WHERE Id='" + id + "' AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;

                try
                {
                    SqlCommand fk = new SqlCommand("Select * FROM LinePurchases WHERE IdCabecDoc='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    listlindc = new List<LinhaDocCompra>();
                    while (fkr.Read())
                    {
                        IDataReader ldcdr = (IDataReader)fkr;
                        listlindc.Add(new LinhaDocCompra
                        {
                            Id = (string)ldcdr[0],
                            IdCabecDoc = (string)ldcdr[1],
                            CodArtigo = (string)ldcdr[2],
                            DescArtigo = (string)ldcdr[3],
                            Quantidade = (int)ldcdr[4],
                            Unidade = (string)ldcdr[5],
                            Desconto = (int)ldcdr[6],
                            PrecoUnitario = (double)(decimal)ldcdr[7],
                            TotalLiquido = (double)(decimal)ldcdr[8],
                            TotalILiquido = (double)(decimal)ldcdr[9],
                            Armazem = (string)ldcdr[10],
                            Lote = (string)ldcdr[11]
                        });
                    }
                    f = GetFornecedor((string)rec[2]);
                    e = getEmpresa((string)rec[7]);
                    DocCompra compra = new DocCompra
                    {
                        id = (string)rec[0],
                        NumDocExterno = ((int)rec[1]).ToString(),
                        Entidade = (string)rec[2],
                        NumDoc = (int)rec[3],
                        Data = (System.DateTime)rec[4],
                        TotalMerc = (double)(decimal)rec[5],
                        Serie = (string)rec[6],
                        LinhasDoc = listlindc,
                        dadosFornecedor = f,
                        dadosEmpresa = e
                    };
                    fkr.Close();
                    dr.Close();
                    return compra;
                }
                catch (Exception ex)
                {
                    dr.Close();
                    return null;
                }

            }
        }
        #endregion
        #region DocVenda
        //Listar vendas
        public static IEnumerable<DocVenda> Encomendas_List()
        {
            List<DocVenda> listdv = new List<DocVenda>();
            List<LinhaDocVenda> listlindv;
            Empresa e;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Invoice WHERE EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    SqlCommand fk = new SqlCommand("Select * FROM Line WHERE InvoiceNo='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    listlindv = new List<LinhaDocVenda>();
                    while (fkr.Read())
                    {
                        IDataReader ldcvr = (IDataReader)fkr;
                        listlindv.Add(new LinhaDocVenda
                        {
                            Id = (int)ldcvr[0],
                            LineNumber = (int)ldcvr[1],
                            ProductCode = (string)ldcvr[2],
                            Quantity = (int)ldcvr[3],
                            UnitPrice = (double)(decimal)ldcvr[4],
                            TaxPointDate = (System.DateTime)ldcvr[5],
                            Description = (string)ldcvr[6],
                            CreditAmount = (double)(decimal)ldcvr[7],
                            TaxType = (string)ldcvr[8],
                            TaxCountryRegion = (string)ldcvr[9],
                            TaxCode = (string)ldcvr[10],
                            TaxPercentage = (double)ldcvr[11],
                            SettlementAmount = (double)ldcvr[12],
                            InvoiceNo = (string)ldcvr[13]
                        });
                    }
                    e = getEmpresa((string)rec[13]);
                    DocVenda venda = new DocVenda
                    {
                        InvoiceNo = (string)rec[0],
                        InvoiceStatus = (string)rec[1],
                        HashControl = (bool)rec[2],
                        Hash = (string)rec[3],
                        Period = (int)rec[4],
                        InvoiceDate = (System.DateTime)rec[5],
                        InvoiceType = (string)rec[6],
                        SelfBillingIndicator = (bool)rec[7],
                        SystemEntryDate = (System.DateTime)rec[8],
                        CustomerID = (string)rec[9],
                        TaxPayable = (double)(decimal)rec[10],
                        NetTotal = (double)(decimal)rec[11],
                        GrossTotal = (double)(decimal)rec[12],
                        LinhasDoc = listlindv,
                        dadosEmpresa = e
                    };
                    listdv.Add(venda);
                    fkr.Close();
                }
                dr.Close();
            }
            return listdv;
        }
        public static List<DocVenda> Encomendas_List(int month)
        {
            List<DocVenda> listdv = new List<DocVenda>();
            List<LinhaDocVenda> listlindv;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM Invoice WHERE Month(InvoiceDate) =" + month + "AND Year(InvoiceDate) = 2016 AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                Empresa e;
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    SqlCommand fk = new SqlCommand("Select * FROM Line WHERE InvoiceNo='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    listlindv = new List<LinhaDocVenda>();
                    while (fkr.Read())
                    {
                        IDataReader ldcvr = (IDataReader)fkr;
                        listlindv.Add(new LinhaDocVenda
                        {
                            Id = (int)ldcvr[0],
                            LineNumber = (int)ldcvr[1],
                            ProductCode = (string)ldcvr[2],
                            Quantity = (int)ldcvr[3],
                            UnitPrice = (double)(decimal)ldcvr[4],
                            TaxPointDate = (System.DateTime)ldcvr[5],
                            Description = (string)ldcvr[6],
                            CreditAmount = (double)(decimal)ldcvr[7],
                            TaxType = (string)ldcvr[8],
                            TaxCountryRegion = (string)ldcvr[9],
                            TaxCode = (string)ldcvr[10],
                            TaxPercentage = (double)ldcvr[11],
                            SettlementAmount = (double)ldcvr[12],
                            InvoiceNo = (string)ldcvr[13]
                        });
                    }
                    e = getEmpresa((string)rec[13]);
                    DocVenda venda = new DocVenda
                    {
                        InvoiceNo = (string)rec[0],
                        InvoiceStatus = (string)rec[1],
                        HashControl = (bool)rec[2],
                        Hash = (string)rec[3],
                        Period = (int)rec[4],
                        InvoiceDate = (System.DateTime)rec[5],
                        InvoiceType = (string)rec[6],
                        SelfBillingIndicator = (bool)rec[7],
                        SystemEntryDate = (System.DateTime)rec[8],
                        CustomerID = (string)rec[9],
                        TaxPayable = (double)(decimal)rec[10],
                        NetTotal = (double)(decimal)rec[11],
                        GrossTotal = (double)(decimal)rec[12],
                        LinhasDoc = listlindv,
                        dadosEmpresa = e
                    };
                    listdv.Add(venda);
                    fkr.Close();
                }
                dr.Close();
            }
            return listdv;
        }

        public static DocVenda Encomenda_Get(string id)
        {

            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                Empresa e;
                List<LinhaDocVenda> listlindv;
                SqlCommand c = new SqlCommand("SELECT * FROM Invoice WHERE InvoiceNo='" + id + "' AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;

                try
                {
                    SqlCommand fk = new SqlCommand("Select * FROM Line WHERE InvoiceNo='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    listlindv = new List<LinhaDocVenda>();
                    while (fkr.Read())
                    {
                        IDataReader ldcvr = (IDataReader)fkr;
                        listlindv.Add(new LinhaDocVenda
                        {
                            Id = (int)ldcvr[0],
                            LineNumber = (int)ldcvr[1],
                            ProductCode = (string)ldcvr[2],
                            Quantity = (int)ldcvr[3],
                            UnitPrice = (double)(decimal)ldcvr[4],
                            TaxPointDate = (System.DateTime)ldcvr[5],
                            Description = (string)ldcvr[6],
                            CreditAmount = (double)(decimal)ldcvr[7],
                            TaxType = (string)ldcvr[8],
                            TaxCountryRegion = (string)ldcvr[9],
                            TaxCode = (string)ldcvr[10],
                            TaxPercentage = (double)ldcvr[11],
                            SettlementAmount = (double)ldcvr[12],
                            InvoiceNo = (string)ldcvr[13]
                        });
                    }
                    e = getEmpresa((string)rec[13]);
                    DocVenda venda = new DocVenda
                    {
                        InvoiceNo = (string)rec[0],
                        InvoiceStatus = (string)rec[1],
                        HashControl = (bool)rec[2],
                        Hash = (string)rec[3],
                        Period = (int)rec[4],
                        InvoiceDate = (System.DateTime)rec[5],
                        InvoiceType = (string)rec[6],
                        SelfBillingIndicator = (bool)rec[7],
                        SystemEntryDate = (System.DateTime)rec[8],
                        CustomerID = (string)rec[9],
                        TaxPayable = (double)(decimal)rec[10],
                        NetTotal = (double)(decimal)rec[11],
                        GrossTotal = (double)(decimal)rec[12],
                        LinhasDoc = listlindv,
                        dadosEmpresa = e
                    };
                    Cliente cl = GetCliente((string)rec[9]);
                    venda.dadosCliente = cl;
                    fkr.Close();
                    dr.Close();
                    return venda;
                }
                catch (Exception ex)
                {
                    dr.Close();
                    return null;
                }

            }
        }
        #endregion
        #region Encomenda
        //Obter encomendas
        public static IEnumerable<Encomenda> ListaEncomendas()
        {
            List<Encomenda> liste = new List<Encomenda>();
            List<LinhaDocCompra> listlindc;
            Fornecedor f;
            Empresa e;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM EncomendasFornecedor WHERE EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    SqlCommand fk = new SqlCommand("Select * FROM LinePurchases WHERE IdCabecDoc='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    listlindc = new List<LinhaDocCompra>();
                    while (fkr.Read())
                    {
                        IDataReader ldcdr = (IDataReader)fkr;
                        listlindc.Add(new LinhaDocCompra
                        {
                            Id = (string)ldcdr[0],
                            IdCabecDoc = (string)ldcdr[1],
                            CodArtigo = (string)ldcdr[2],
                            DescArtigo = (string)ldcdr[3],
                            Quantidade = (int)ldcdr[4],
                            Unidade = (string)ldcdr[5],
                            Desconto = (int)ldcdr[6],
                            PrecoUnitario = (double)(decimal)ldcdr[7],
                            TotalLiquido = (double)(decimal)ldcdr[8],
                            TotalILiquido = (double)(decimal)ldcdr[9],
                            Armazem = (string)ldcdr[10],
                            Lote = (string)ldcdr[11]
                        });
                    }
                    f = GetFornecedor((string)rec[1]);
                    e = getEmpresa((string)rec[9]);
                    Encomenda enc = new Encomenda
                    {
                        CodEncomenda = (string)rec[0],
                        Entidade = (string)rec[1],
                        Origem = (string)rec[2],
                        TotalDesc = (double)(decimal)rec[3],
                        TotalIva = (double)(decimal)rec[4],
                        TotalMerc = (double)(decimal)rec[5],
                        ModoPag = (string)rec[6],
                        DataDoc = (System.DateTime)rec[7],
                        Entregue = (bool)rec[8],
                        LinhasEnc = listlindc,
                        dadosFornecedor = f,
                        dadosEmpresa = e
                    };
                    liste.Add(enc);
                    fkr.Close();
                }
                dr.Close();
            }
            return liste;
        }
        //Obter total de encomendas
        public static int getTotalEncomendas(int m)
        {
            int n = 0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();

                SqlCommand c = new SqlCommand("SELECT COUNT(CodEncomenda) FROM EncomendasFornecedor WHERE MONTH(DataDoc)=" + m + " AND YEAR(DataDoc)=2016 AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                try
                {
                    dr.Read();
                    IDataRecord rec = (IDataRecord)dr;
                    n = (int)rec[0];
                    dr.Close();
                }
                catch (Exception e)
                {
                    dr.Close();
                    return -1;
                }
            }
            return n;
        }
        //Obter uma encomenda
        public static Encomenda GetEncomenda(string id)
        {
            Empresa e;
            Fornecedor f;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                List<LinhaDocCompra> listlindc;
                SqlCommand c = new SqlCommand("SELECT * FROM EncomendasFornecedor WHERE CodEncomenda='" + id + "' AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;

                try
                {
                    SqlCommand fk = new SqlCommand("Select * FROM LinePurchases WHERE IdCabecDoc='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    listlindc = new List<LinhaDocCompra>();
                    while (fkr.Read())
                    {
                        IDataReader ldcdr = (IDataReader)fkr;
                        listlindc.Add(new LinhaDocCompra
                        {
                            Id = (string)ldcdr[0],
                            IdCabecDoc = (string)ldcdr[1],
                            CodArtigo = (string)ldcdr[2],
                            DescArtigo = (string)ldcdr[3],
                            Quantidade = (int)ldcdr[4],
                            Unidade = (string)ldcdr[5],
                            Desconto = (int)ldcdr[6],
                            PrecoUnitario = (double)(decimal)ldcdr[7],
                            TotalLiquido = (double)(decimal)ldcdr[8],
                            TotalILiquido = (double)(decimal)ldcdr[9],
                            Armazem = (string)ldcdr[10],
                            Lote = (string)ldcdr[11]
                        });
                    }
                    f = GetFornecedor((string)rec[1]);
                    e = getEmpresa((string)rec[9]);
                    Encomenda enc = new Encomenda
                    {
                        CodEncomenda = (string)rec[0],
                        Entidade = (string)rec[1],
                        Origem = (string)rec[2],
                        TotalDesc = (double)(decimal)rec[3],
                        TotalIva = (double)(decimal)rec[4],
                        TotalMerc = (double)(decimal)rec[5],
                        ModoPag = (string)rec[6],
                        DataDoc = (System.DateTime)rec[7],
                        Entregue = (bool)rec[8],
                        LinhasEnc = listlindc,
                        dadosEmpresa = e,
                        dadosFornecedor = f
                    };
                    fkr.Close();
                    dr.Close();
                    return enc;
                }
                catch (Exception ex)
                {
                    dr.Close();
                    return null;
                }

            }
        }
        //Obter encomendas por Pendentes ou Entregues
        public static IEnumerable<Encomenda> ListaEncomendasFiltrado(char t)
        {
            int select = 0;
            if (t == 'T')
                select = 1;
            if (t == 'P')
                select = 0;
            List<Encomenda> liste = new List<Encomenda>();
            List<LinhaDocCompra> listlindc;
            Empresa e;
            Fornecedor f;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                SqlCommand c = new SqlCommand("SELECT * FROM EncomendasFornecedor WHERE Entregue=" + select + " AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                while (dr.Read())
                {
                    IDataRecord rec = (IDataRecord)dr;
                    SqlCommand fk = new SqlCommand("Select * FROM LinePurchases WHERE IdCabecDoc='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader fkr = fk.ExecuteReader();
                    listlindc = new List<LinhaDocCompra>();
                    while (fkr.Read())
                    {
                        IDataReader ldcdr = (IDataReader)fkr;
                        listlindc.Add(new LinhaDocCompra
                        {
                            Id = (string)ldcdr[0],
                            IdCabecDoc = (string)ldcdr[1],
                            CodArtigo = (string)ldcdr[2],
                            DescArtigo = (string)ldcdr[3],
                            Quantidade = (int)ldcdr[4],
                            Unidade = (string)ldcdr[5],
                            Desconto = (int)ldcdr[6],
                            PrecoUnitario = (double)(decimal)ldcdr[7],
                            TotalLiquido = (double)(decimal)ldcdr[8],
                            TotalILiquido = (double)(decimal)ldcdr[9],
                            Armazem = (string)ldcdr[10],
                            Lote = (string)ldcdr[11]
                        });
                    }
                    f = GetFornecedor((string)rec[1]);
                    e = getEmpresa((string)rec[9]);
                    Encomenda enc = new Encomenda
                    {
                        CodEncomenda = (string)rec[0],
                        Entidade = (string)rec[1],
                        Origem = (string)rec[2],
                        TotalDesc = (double)(decimal)rec[3],
                        TotalIva = (double)(decimal)rec[4],
                        TotalMerc = (double)(decimal)rec[5],
                        ModoPag = (string)rec[6],
                        DataDoc = (System.DateTime)rec[7],
                        Entregue = (bool)rec[8],
                        LinhasEnc = listlindc,
                        dadosEmpresa = e,
                        dadosFornecedor = f
                    };
                    liste.Add(enc);
                    fkr.Close();
                }
                dr.Close();
            }
            return liste;
        }

        #endregion
        #region Ganhos
        public static double GetGrossRevenue(String month)
        {
            double grossRevenue = 0.0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                String firstDayOfTheMonth = "2016-" + month + "-01";
                String lastDayOfTheMonth = "2016-" + month + "-" + DateTime.DaysInMonth(2016, Int32.Parse(month));

                SqlCommand c = new SqlCommand("SELECT SUM(GrossTotal) FROM Invoice WHERE InvoiceDate < '" + lastDayOfTheMonth + "' AND InvoiceDate > '" + firstDayOfTheMonth + "' AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;

                try
                {

                    grossRevenue = (double)(decimal)rec[0];

                    dr.Close();
                    return grossRevenue;
                }
                catch (Exception e)
                {
                    dr.Close();
                    return grossRevenue;
                }


            }
        }


        public static double GetNetRevenue(String month)
        {
            double netRevenue = 0.0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                sc.Open();
                String firstDayOfTheMonth = "2016-" + month + "-01";
                String lastDayOfTheMonth = "2016-" + month + "-" + DateTime.DaysInMonth(2016, Int32.Parse(month));

                SqlCommand c = new SqlCommand("SELECT SUM(NetTotal) FROM Invoice WHERE InvoiceDate < '" + lastDayOfTheMonth + "' AND InvoiceDate > '" + firstDayOfTheMonth + "' AND EmpresaId='" + empresa + "'", sc);
                SqlDataReader dr = c.ExecuteReader();
                dr.Read();
                IDataRecord rec = (IDataRecord)dr;

                try
                {

                    netRevenue = (double)(decimal)rec[0];

                    dr.Close();
                    return netRevenue;
                }
                catch (Exception e)
                {
                    dr.Close();
                    return netRevenue;
                }


            }
        }


        public static double GetGrossMargin(String month)
        {
            /* GrossMargin = (NetRevenue - Cost Of Goods Sold)/NetRevenue */

            double GrossMargin = 0.0;

            double NetRevenue = 0.0;
            double CostOfGoodsSold = 0.0;
            using (SqlConnection sc = new SqlConnection(connectionString))
            {

                try
                {
                    sc.Open();
                    String firstDayOfTheMonth = "2016-" + month + "-01";
                    String lastDayOfTheMonth = "2016-" + month + "-" + DateTime.DaysInMonth(2016, Int32.Parse(month));

                    SqlCommand c = new SqlCommand("SELECT * FROM Line WHERE TaxPointDate < '" + lastDayOfTheMonth + "' AND TaxPointDate > '" + firstDayOfTheMonth + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader dr = c.ExecuteReader();
                    while (dr.Read())
                    {
                        IDataRecord rec = (IDataRecord)dr;
                        SqlCommand fk = new SqlCommand("Select Price FROM Artigo WHERE CodArtigo='" + rec[2] + "' AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader fkr = fk.ExecuteReader();

                        while (fkr.Read())
                        {
                            IDataReader ldcdr = (IDataReader)fkr;
                            try
                            {
                                int Quantity = (int)rec[3];
                                double Price = (double)ldcdr[0];
                                double UnitPrice = (double)(decimal)rec[4];

                                NetRevenue += Quantity * UnitPrice;
                                CostOfGoodsSold += Quantity * Price;


                            }
                            catch (Exception e)
                            {
                                continue;
                            }


                        }

                    }

                }
                catch (Exception e)
                {
                    return 0.0;
                }

                if (NetRevenue > 0.0)
                {
                    GrossMargin = (NetRevenue - CostOfGoodsSold) / NetRevenue;
                }

                return GrossMargin;


            }


        }


        public static double GetGrossMarginClient(String Client)
        {
            double GrossMargin = 0.0;
            double NetRevenue = 0.0;
            double CostOfGoodsSold = 0.0;



            using (SqlConnection sc = new SqlConnection(connectionString))
            {

                try
                {
                    sc.Open();


                    SqlCommand c = new SqlCommand("SELECT InvoiceNo FROM Invoice WHERE CustomerID = '" + Client + "' AND EmpresaId='" + empresa + "'", sc);
                    SqlDataReader dr = c.ExecuteReader();
                    while (dr.Read())
                    {
                        IDataRecord rec = (IDataRecord)dr;
                        SqlCommand fk = new SqlCommand("Select * FROM Line WHERE InvoiceNo='" + rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader fkr = fk.ExecuteReader();

                        while (fkr.Read())
                        {
                            IDataReader ldcdr = (IDataReader)fkr;
                            try
                            {
                                int Quantity = (int)ldcdr[3];

                                double UnitPrice = (double)(decimal)ldcdr[4];

                                SqlCommand fk2 = new SqlCommand("Select Price From Artigo WHERE CodArtigo ='" + ldcdr[2] + "' AND EmpresaId='" + empresa + "'", sc);
                                SqlDataReader fkr2 = fk2.ExecuteReader();
                                fkr2.Read();

                                IDataReader ldcdr2 = (IDataReader)fkr2;

                                double Price = (double)ldcdr2[0];


                                NetRevenue += Quantity * UnitPrice;
                                CostOfGoodsSold += Quantity * Price;


                            }
                            catch (Exception e)
                            {
                                continue;
                            }


                        }

                    }

                }
                catch (Exception e)
                {
                    return 0.0;
                }

                if (NetRevenue > 0.0)
                {
                    GrossMargin = (NetRevenue - CostOfGoodsSold) / NetRevenue;
                }

                return GrossMargin;


            }


        }


        public static List<Cliente> getTopClientsFromNetRevenue()
        {
            List<Cliente> clients  = new List<Cliente>();

            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                 try
                {
                    sc.Open();
                    SqlCommand c = new SqlCommand("SELECT CustomerID,  SUM(NetTotal) AS Total  FROM Invoice GROUP BY CustomerID ORDER BY Total", sc);
                    SqlDataReader dr = c.ExecuteReader();
                    
                     
                     
                    while (dr.Read())
                    {
                        IDataRecord rec = (IDataRecord)dr;

                        SqlCommand fk = new SqlCommand("Select * FROM Cliente WHERE CustomerID='" + (string)rec[0] + "' AND EmpresaId='" + empresa + "'", sc);
                        SqlDataReader fkr = fk.ExecuteReader();
                        fkr.Read();
                        IDataRecord ba = (IDataRecord)fkr;


                        Cliente cliente = new Cliente
                        {
                            CodCliente = (string)rec[0],
                            NomeCliente = (string)ba[2]
                           
                        };
                        clients.Add(cliente);
                        fkr.Close();
                            
                    }

                    dr.Close();
                }
                 catch (Exception e)
                 {
                     return null;
                 }

                
            }

            return clients; 
        }




        public static List<Ganhos> listaGanhos()
        {
            List<Ganhos> ganhos = new List<Ganhos>();
            String[] months = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };

            

            List<Ganhos.GanhosDeCliente> tmp = new List<Ganhos.GanhosDeCliente>();


            List<Cliente> topClients = getTopClientsFromNetRevenue();
            int i = 0;
            foreach(Cliente cli in topClients){
                double aux = GetGrossMarginClient(cli.CodCliente);
                if (i > 8)
                    break;
                Ganhos.GanhosDeCliente tmp2 = new Ganhos.GanhosDeCliente
                {
                    nome = cli.NomeCliente, 
                    GrossMargin = aux

                };
                
                
                tmp.Add(tmp2);

            }


            for (i = 0; i < months.Length; i++)
            {



                ganhos.Add(new Ganhos
                {
                    month = Int32.Parse(months[i]),
                    grossRevenue = GetGrossRevenue(months[i]),
                    netRevenue = GetNetRevenue(months[i]),
                    grossMargin = GetGrossMargin(months[i]),
                    totalClients = getTotalClientesMes(i + 1),
                    ganhosDeClientes = tmp

                });
            }         

            



                return ganhos;
        }



        #endregion


    }
}