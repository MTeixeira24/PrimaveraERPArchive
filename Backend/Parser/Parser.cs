using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Interop.ErpBS900;
using Interop.StdPlatBS900;
using Interop.StdBE900;
using Interop.GcpBE900;
using ADODB;
using System.Data;
using System.Xml;
using System.Text;
using System.IO;

namespace DashboardIntegration.Parser
{
    public class Parser
    {
        
        #region parser

        static SqlConnection sc = new SqlConnection();
        static Dictionary<String, Double> StkArtigo;
        static Dictionary<String, Double> PriceArtigo;
        static string EmpresaId;

        public static void Parse (string path, DashboardIntegration.Lib_Primavera.Model.Login creds)
        {
            StkArtigo = new Dictionary<String, Double>();
            /*Carregar Artigos do Primavera*/
            List<Lib_Primavera.Model.Artigo> arts;
            arts = Lib_Primavera.PriIntegration.ListaArtigos();
            Lib_Primavera.Model.Artigo aux = Lib_Primavera.PriIntegration.GetArtigo("Especial");
            foreach(Lib_Primavera.Model.Artigo a in arts){
                StkArtigo[a.CodArtigo] = a.STKAtual;
            } 

            sc.Close();
            sc.ConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + System.AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Staging.mdf;Integrated Security=True"; 
            sc.Open();
            /*Código para limpar a BD para facilitar debugging*/
            //SqlCommand clean = new SqlCommand("DELETE FROM LinePurchases; Delete FROM EncomendasFornecedor;Delete FROM Purchases; DELETE FROM Supplier; DELETE FROM Line; DELETE FROM Invoice; DELETE FROM Cliente;DELETE FROM Supplier;DELETE FROM BillingAddress; DELETE FROM Artigo ; DELETE FROM Venda; DELETE FROM SalesInvoices; DELETE FROM Empresa", sc);
            //clean.ExecuteScalar();

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(path);
            }
            catch (Exception)
            {
                /** TODO */
                return;
            }

            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            //xmlnsManager.AddNamespace("ns", "urn:OECD:StandardAuditFile-Tax:PT_1.04_01");
            xmlnsManager.AddNamespace("ns", "urn:OECD:StandardAuditFile-Tax:PT_1.01_01");

            XmlNode root = xmlDoc.DocumentElement;

            XmlNode header = xmlDoc.SelectSingleNode("/ns:AuditFile/ns:Header", xmlnsManager);
            EmpresaId = creds.company;
            StagingAPI.empresa = EmpresaId;
            string Nome = header["CompanyName"].InnerText;
            string NumContrib = header["TaxRegistrationNumber"].InnerText;
            XmlNode moradaNode = header.SelectSingleNode("descendant::ns:CompanyAddress", xmlnsManager);
            string Morada = moradaNode["StreetName"].InnerText;
            string Local = moradaNode["City"].InnerText;
            string Cp = moradaNode["PostalCode"].InnerText;


            string empresaQuery = "IF (SELECT COUNT(Id) FROM Empresa WHERE NumContrib='" + NumContrib + "') > 0 ";
            empresaQuery += "UPDATE Empresa SET Nome='" + Nome + "', Morada='"  + Morada + "', Local='" + Local + "', Cp='" + Cp + "' WHERE NumContrib='" + NumContrib + "';";
            empresaQuery += "ELSE ";
            empresaQuery += "INSERT INTO Empresa VALUES('" + EmpresaId + "','" + Nome + "', '" + NumContrib + "', '" + Morada + "', '" + Local + "', '" + Cp + "')";

            SqlCommand empresaCom = new SqlCommand(empresaQuery, sc);
            empresaCom.ExecuteScalar();

            //Carregar fornecedores

            if (DashboardIntegration.Lib_Primavera.PriEngine.InitializeCompany(creds.company, creds.user, creds.password) == true)
            {
                List<Lib_Primavera.Model.Fornecedor> listFornecedores = Lib_Primavera.PriIntegration.ListaFornecedores();
                foreach (Lib_Primavera.Model.Fornecedor f in listFornecedores)
                {
                    string query = "IF (SELECT COUNT(CodFornecedor) FROM Supplier WHERE CodFornecedor='" + f.CodFornecedor + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
                    query += "UPDATE Supplier SET Nome='" + sanitize(f.Nome) + "', Morada='" + sanitize(f.Morada) + "',Local='" + sanitize(f.Local) + "',Cp='" + f.Cp + "',Tel='" + f.Tel + "',Divida=" + f.Divida.ToString().Replace(",", ".") + ",NumContrib='" + f.NumContrib + "',ModoPag='" + f.ModoPag + "',EnderecoWeb='" + f.EnderecoWeb + "',EncomendasPendentes=" + f.EncomendasPendentes.ToString().Replace(",", ".") + " WHERE CodFornecedor='" + f.CodFornecedor + "' AND EmpresaId='"+EmpresaId+"';";
                    query += "ELSE ";
                    query += "INSERT INTO Supplier VALUES('" + f.CodFornecedor + "', '" + sanitize(f.Nome) + "', '" + sanitize(f.Morada) + "','" + sanitize(f.Local) + "','" + f.Cp + "','" + f.Tel + "'," + f.Divida.ToString().Replace(",", ".") + ",'" + f.NumContrib + "','" + f.ModoPag + "','" + f.EnderecoWeb + "'," + f.EncomendasPendentes.ToString().Replace(",", ".") + ",'" + EmpresaId + "')";
                    SqlCommand com = new SqlCommand(query, sc);
                    com.ExecuteScalar();
                }
                //Carregar documentos de compra
                List<Lib_Primavera.Model.DocCompra> compras = Lib_Primavera.PriIntegration.VGR_List();
                foreach (Lib_Primavera.Model.DocCompra c in compras)
                {
                    string parsedDate = c.Data.ToString("yyyy-MM-dd hh:mm:ss");
                    double absMerc = Math.Abs(c.TotalMerc);
                    string query = "IF (SELECT COUNT(Id) FROM Purchases WHERE Id='" + c.id + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
                    query += "UPDATE Purchases SET NumDocExterno=" + c.NumDocExterno + ", Entidade='" + sanitize(c.Entidade) + "',NumDoc=" + c.NumDoc + ",Data='" + parsedDate + "',TotalMerc=" + absMerc.ToString().Replace(",", ".") + ",Serie='" + c.Serie + "' WHERE Id='" + c.id + "' AND EmpresaId='" + EmpresaId+"'; "; 
                    query += "ELSE ";
                    query += "INSERT INTO Purchases VALUES('" + c.id + "', " + c.NumDocExterno + ", '" + sanitize(c.Entidade) + "'," + c.NumDoc + ",'" + parsedDate + "'," + absMerc.ToString().Replace(",", ".") + ",'" + c.Serie + "','" + EmpresaId + "')";
                    SqlCommand com = new SqlCommand(query, sc);
                    com.ExecuteScalar();
                    //Carregar linhas do doc
                    foreach (Lib_Primavera.Model.LinhaDocCompra l in c.LinhasDoc)
                    {

                        string queryl = "IF (SELECT COUNT(Id) FROM LinePurchases WHERE Id='" + l.Id + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
                        queryl += "UPDATE LinePurchases SET IdCabecDoc='" + l.IdCabecDoc + "', CodArtigo='" + l.CodArtigo + "',Descricao='" + sanitize(l.DescArtigo) + "',Quantidade=" + l.Quantidade + ",Unidade='" + l.Unidade + "',Desconto1=" + l.Desconto + ",PrecUnit=" + l.PrecoUnitario.ToString().Replace(",", ".") + ",TotalLiquido=" + l.TotalILiquido.ToString().Replace(",", ".") + ",PrecoLiquido=" + l.TotalLiquido.ToString().Replace(",", ".") + ",Armazem='" + l.Armazem + "',Lote='" + l.Lote + "',NumLinha=" + l.NumLinha + " WHERE Id='" + l.Id + "' AND EmpresaId='" + EmpresaId + "';";
                        queryl += "ELSE ";
                        queryl += "INSERT INTO LinePurchases VALUES('" + l.Id + "', '" + l.IdCabecDoc + "', '" + l.CodArtigo + "','" + sanitize(l.DescArtigo) + "'," + l.Quantidade + ",'" + l.Unidade + "'," + l.Desconto + "," + l.PrecoUnitario.ToString().Replace(",", ".") + "," + l.TotalILiquido.ToString().Replace(",", ".") + "," + l.TotalLiquido.ToString().Replace(",", ".") + ",'" + l.Armazem + "','" + l.Lote + "','" + EmpresaId + "'," + l.NumLinha + ");";
                        SqlCommand coml = new SqlCommand(queryl, sc);
                        coml.ExecuteScalar();
                    }
                }
                //Carregar encomendas feitas entregues
                List<Lib_Primavera.Model.Encomenda> encsEntregues = Lib_Primavera.PriIntegration.ListaEncomendasFiltrado('T'/*, DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()*/);
                foreach (Lib_Primavera.Model.Encomenda e in encsEntregues)
                {
                    string parsedDate = e.DataDoc.ToString("yyyy-MM-dd hh:mm:ss");

                    string query = "IF (SELECT COUNT(CodEncomenda) FROM EncomendasFornecedor WHERE CodEncomenda='" + e.CodEncomenda + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
                    query += "UPDATE EncomendasFornecedor SET Entidade='" + sanitize(e.Entidade) + "', Origem='" + sanitize(e.Origem) + "',TotalDesc=" + e.TotalDesc.ToString().Replace(",", ".") + ",TotalIva=" + e.TotalIva.ToString().Replace(",", ".") + ",TotalMerc=" + e.TotalMerc.ToString().Replace(",", ".") + ",ModoPag='" + e.ModoPag + "',DataDoc='" + parsedDate + "',Entregue = 1 WHERE CodEncomenda='"+e.CodEncomenda+"' AND EmpresaId='" + EmpresaId + "'; ";
                    query += "ELSE ";
                    query += "INSERT INTO EncomendasFornecedor VALUES('";
                    query += e.CodEncomenda + "', '" + sanitize(e.Entidade) + "', '" + sanitize(e.Origem) + "'," + e.TotalDesc.ToString().Replace(",", ".") + "," + e.TotalIva.ToString().Replace(",", ".") + "," + e.TotalMerc.ToString().Replace(",", ".") + ",'" + e.ModoPag + "','" + parsedDate + "', 1";
                    query += ",'" + EmpresaId + "');";
                    SqlCommand com = new SqlCommand(query, sc);
                    com.ExecuteScalar();
                    //Carregar linhas do doc
                    foreach (Lib_Primavera.Model.LinhaDocCompra l in e.LinhasEnc)
                    {
                        string queryl = "IF (SELECT COUNT(Id) FROM LinePurchases WHERE Id='" + l.Id + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
                        queryl += "UPDATE LinePurchases SET IdCabecDoc='" + l.IdCabecDoc + "', CodArtigo='" + l.CodArtigo + "',Descricao='" + sanitize(l.DescArtigo) + "',Quantidade=" + Math.Truncate(l.Quantidade) + ",Unidade='" + l.Unidade + "',Desconto1=" + l.Desconto + ",PrecUnit=" + l.PrecoUnitario.ToString().Replace(",", ".") + ",TotalLiquido=" + l.TotalILiquido.ToString().Replace(",", ".") + ",PrecoLiquido=" + l.TotalLiquido.ToString().Replace(",", ".") + ",Armazem='" + l.Armazem + "',Lote='" + l.Lote + "',NumLinha=" + l.NumLinha + " WHERE Id='" + l.Id + "' AND EmpresaId='" + EmpresaId + "';";
                        queryl += "ELSE ";
                        queryl += "INSERT INTO LinePurchases VALUES('" + l.Id + "', '" + l.IdCabecDoc + "', '" + l.CodArtigo + "','" + sanitize(l.DescArtigo) + "'," + Math.Truncate(l.Quantidade) + ",'" + l.Unidade + "'," + l.Desconto + "," + l.PrecoUnitario.ToString().Replace(",", ".") + "," + l.TotalILiquido.ToString().Replace(",", ".") + "," + l.TotalLiquido.ToString().Replace(",", ".") + ",'" + l.Armazem + "','" + l.Lote + "','" + EmpresaId + "'," + l.NumLinha + ");";
                        SqlCommand coml = new SqlCommand(queryl, sc);
                        coml.ExecuteScalar();
                    }
                }
                //Carregar encomendas feitas pendentes
                List<Lib_Primavera.Model.Encomenda> encsPendentes = Lib_Primavera.PriIntegration.ListaEncomendasFiltrado('P'/*, DashboardIntegration.Properties.Settings.Default.Company.Trim(), DashboardIntegration.Properties.Settings.Default.User.Trim(), DashboardIntegration.Properties.Settings.Default.Password.Trim()*/);
                foreach (Lib_Primavera.Model.Encomenda e in encsPendentes)
                {
                    string parsedDate = e.DataDoc.ToString("yyyy-MM-dd hh:mm:ss");
                    string query = "IF (SELECT COUNT(CodEncomenda) FROM EncomendasFornecedor WHERE CodEncomenda='" + e.CodEncomenda + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
                    query += "UPDATE EncomendasFornecedor SET Entidade='" + sanitize(e.Entidade) + "', Origem='" + sanitize(e.Origem) + "',TotalDesc=" + e.TotalDesc.ToString().Replace(",", ".") + ",TotalIva=" + e.TotalIva.ToString().Replace(",", ".") + ",TotalMerc=" + e.TotalMerc.ToString().Replace(",", ".") + ",ModoPag='" + e.ModoPag + "',DataDoc='" + parsedDate + "',Entregue = 0 WHERE CodEncomenda='" + e.CodEncomenda + "' AND EmpresaId='" + EmpresaId + "'; ";
                    query += "ELSE ";
                    query += "INSERT INTO EncomendasFornecedor VALUES('";
                    query += e.CodEncomenda + "', '" + sanitize(e.Entidade) + "', '" + sanitize(e.Origem) + "'," + e.TotalDesc.ToString().Replace(",", ".") + "," + e.TotalIva.ToString().Replace(",", ".") + "," + e.TotalMerc.ToString().Replace(",", ".") + ",'" + e.ModoPag + "','" + parsedDate + "', 0";
                    query += ",'" + EmpresaId + "');";
                    try
                    {
                        SqlCommand com = new SqlCommand(query, sc);
                        com.ExecuteScalar();
                        //Carregar linhas do doc
                        foreach (Lib_Primavera.Model.LinhaDocCompra l in e.LinhasEnc)
                        {
                            string queryl = "IF (SELECT COUNT(Id) FROM LinePurchases WHERE Id='" + l.Id + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
                            queryl += "UPDATE LinePurchases SET IdCabecDoc='" + l.IdCabecDoc + "', CodArtigo='" + l.CodArtigo + "',Descricao='" + sanitize(l.DescArtigo) + "',Quantidade=" + Math.Truncate(l.Quantidade) + ",Unidade='" + l.Unidade + "',Desconto1=" + l.Desconto + ",PrecUnit=" + l.PrecoUnitario.ToString().Replace(",", ".") + ",TotalLiquido=" + l.TotalILiquido.ToString().Replace(",", ".") + ",PrecoLiquido=" + l.TotalLiquido.ToString().Replace(",", ".") + ",Armazem='" + l.Armazem + "',Lote='" + l.Lote + "',NumLinha=" + l.NumLinha + " WHERE Id='" + l.Id + "' AND EmpresaId='" + EmpresaId + "';";
                            queryl += "ELSE ";
                            queryl += "INSERT INTO LinePurchases VALUES('" + l.Id + "', '" + l.IdCabecDoc + "', '" + l.CodArtigo + "','" + sanitize(l.DescArtigo) + "'," + Math.Truncate(l.Quantidade) + ",'" + l.Unidade + "'," + l.Desconto + "," + l.PrecoUnitario.ToString().Replace(",", ".") + "," + l.TotalILiquido.ToString().Replace(",", ".") + "," + l.TotalLiquido.ToString().Replace(",", ".") + ",'" + l.Armazem + "','" + l.Lote + "','" + EmpresaId + "'," + l.NumLinha + ");";
                            SqlCommand coml = new SqlCommand(queryl, sc);
                            coml.ExecuteScalar();
                        }
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        continue;
                    }
                }
            }
            

            /* AVG of each product */  
            PriceArtigo = new Dictionary<String, Double>();

            SqlCommand comp  = new SqlCommand("SELECT codArtigo, AVG(PrecUnit)  FROM LinePurchases GROUP BY codArtigo ", sc);
            SqlDataReader dr = comp.ExecuteReader();
            while (dr.Read())
            {
                IDataRecord rec = (IDataRecord)dr;
                String cod = (String)(rec[0]);
                double price = (double)(decimal)(rec[1]);

                PriceArtigo.Add(cod, price);
                
            }
            dr.Close();


           XmlNodeList masterFiles = xmlDoc.SelectNodes("/ns:AuditFile/ns:MasterFiles/*", xmlnsManager);
           XmlNodeList masterFiles2 = root.SelectNodes("/ns:MasterFiles/*", xmlnsManager);
            foreach (XmlNode node in masterFiles)
            {
                switch (node.Name)
                {
                    case "Customer":
                        parseCostumer(node, xmlnsManager);
                        break;
                    case "Supplier":
                        parseSupplier(node, xmlnsManager);
                        break;
                    case "Product":
                        parseProduct(node, xmlnsManager);
                        break;
                    default:
                        break;

                }
            }

            XmlNodeList SourceDocuments = xmlDoc.SelectNodes("/ns:AuditFile/ns:SourceDocuments/*", xmlnsManager);
            XmlNodeList auxNode = null;

            foreach (XmlNode node in SourceDocuments)
            {
                switch (node.Name)
                {
                    case "SalesInvoices":

                        string TotalDebit = node.SelectSingleNode("descendant::ns:TotalDebit", xmlnsManager).InnerText;
                        string TotalCredit = node.SelectSingleNode("descendant::ns:TotalCredit", xmlnsManager).InnerText;
                        string entriesS = node.SelectSingleNode("descendant::ns:NumberOfEntries", xmlnsManager).InnerText;
                        int stringSize = entriesS.Length;
                        double entries = 0;
                        for (int i = 0; i < stringSize; i++ )
                        {
                            entries += (int)Char.GetNumericValue(entriesS[i]) * Math.Pow(10, (stringSize - i - 1));
                        }
                        int iEntries = (int)entries;
                        string query = "INSERT INTO SalesInvoices VALUES('" + TotalDebit + "', '" + TotalDebit + "', " + iEntries + ",'"+ EmpresaId +"')";
                        SqlCommand com = new SqlCommand(query, sc);
                        com.ExecuteScalar();
                        auxNode = node.SelectNodes("descendant::ns:Invoice", xmlnsManager);
                        
                        foreach (XmlNode Invoice in auxNode)
                            parseInvoice(Invoice, xmlnsManager);
                        break;

                    case "MovementOfGoods":
                        auxNode = node.SelectNodes("descendant::StockMovement", xmlnsManager);
                        break;
                    case "WorkingDocuments":
                        break;
                    case "Payments":
                        break;


                }


            }

 
            sc.Close();

        }

        private static void parseInvoice(XmlNode Invoice, XmlNamespaceManager ns)
        {

            string InvoiceNo = Invoice.SelectSingleNode("descendant::ns:InvoiceNo", ns).InnerText.Replace("/","_");
            string InvoiceDate = Invoice.SelectSingleNode("descendant::ns:InvoiceDate", ns).InnerText;
            string CustomerID = Invoice.SelectSingleNode("descendant::ns:CustomerID", ns).InnerText.Replace(".","");
            string InvoiceStatus = Invoice.SelectSingleNode("descendant::ns:InvoiceStatus", ns).InnerText;
            string Hash = Invoice.SelectSingleNode("descendant::ns:Hash", ns).InnerText;
            string HashControl = Invoice.SelectSingleNode("descendant::ns:HashControl", ns).InnerText;
            string Period = Invoice.SelectSingleNode("descendant::ns:Period", ns).InnerText;
            string InvoiceType = Invoice.SelectSingleNode("descendant::ns:InvoiceType", ns).InnerText;
            string SelfBillingIndicator = Invoice.SelectSingleNode("descendant::ns:SelfBillingIndicator", ns).InnerText;
            string SystemEntryDate = Invoice.SelectSingleNode("descendant::ns:SystemEntryDate", ns).InnerText;

            //Totals
            XmlNode DocumentTotals = Invoice.SelectSingleNode("descendant::ns:DocumentTotals", ns);
            string TaxPayable = DocumentTotals.SelectSingleNode("descendant::ns:TaxPayable", ns).InnerText;
            string NetTotal = DocumentTotals.SelectSingleNode("descendant::ns:NetTotal", ns).InnerText;
            string GrossTotal = DocumentTotals.SelectSingleNode("descendant::ns:GrossTotal", ns).InnerText;
            if (!InvoiceStatus.Equals("A"))
            {
                string query = "IF (SELECT COUNT(InvoiceNo) FROM Invoice WHERE InvoiceNo='" + InvoiceNo + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
                query += "UPDATE Invoice SET InvoiceStatus='" + InvoiceStatus + "', HashControl=" + HashControl + ", Hash='" + Hash + "', Period=" + Period + ",InvoiceDate='" + InvoiceDate + "',InvoiceType='" + InvoiceType + "',SelfBillingIndicator=" + SelfBillingIndicator + ",SystemEntryDate='" + SystemEntryDate + "',CustomerID='" + CustomerID + "',TaxPayable=" + TaxPayable + ",NetTotal=" + NetTotal + ",GrossTotal=" + GrossTotal + " WHERE InvoiceNo='" + InvoiceNo + "' AND EmpresaId='" + EmpresaId + "';";
                query += "ELSE ";
                query += "INSERT INTO Invoice VALUES('" + InvoiceNo + "', '" + InvoiceStatus + "', " + HashControl + ",'" + Hash + "'," + Period + ",'" + InvoiceDate + "','" + InvoiceType + "'," + SelfBillingIndicator + ",'" + SystemEntryDate + "','" + CustomerID + "'," + TaxPayable + "," + NetTotal + "," + GrossTotal + ",'" + EmpresaId + "');";
                SqlCommand com = new SqlCommand(query, sc);
                com.ExecuteScalar();

                XmlNodeList Lines = Invoice.SelectNodes("descendant::ns:Line", ns);
                List<DashboardIntegration.Lib_Primavera.Model.LinhaDocVenda> listEncs = new List<DashboardIntegration.Lib_Primavera.Model.LinhaDocVenda>();

                foreach (XmlNode Line in Lines)
                    parseLine(Line, ns, InvoiceNo);
            }
            

        }

        private static /*Model.LinhaDocVenda*/ void parseLine(XmlNode Line, XmlNamespaceManager ns, string FK)
        {
            string LineNumber = Line.SelectSingleNode("descendant::ns:LineNumber", ns).InnerText;
            string ProductCode = Line.SelectSingleNode("descendant::ns:ProductCode", ns).InnerText;
            string Quantity = Line.SelectSingleNode("descendant::ns:Quantity", ns).InnerText;
            string UnitOfMeasure = Line.SelectSingleNode("descendant::ns:UnitOfMeasure", ns).InnerText;
            string UnitPrice = Line.SelectSingleNode("descendant::ns:UnitPrice", ns).InnerText;
            string TaxPointDate = Line.SelectSingleNode("descendant::ns:TaxPointDate", ns).InnerText;
            string Description = sanitize(Line.SelectSingleNode("descendant::ns:Description", ns).InnerText);
            string CreditAmount = Line.SelectSingleNode("descendant::ns:CreditAmount", ns).InnerText;
            string SettlementAmount = Line.SelectSingleNode("descendant::ns:SettlementAmount", ns).InnerText;

            //Tax
            XmlNode Tax = Line.SelectSingleNode("descendant::ns:Tax", ns);
            string TaxType = Tax.SelectSingleNode("descendant::ns:TaxType", ns).InnerText;
            string TaxCountryRegion = Tax.SelectSingleNode("descendant::ns:TaxCountryRegion", ns).InnerText;
            string TaxCode = Tax.SelectSingleNode("descendant::ns:TaxCode", ns).InnerText;
            string TaxPercentage = Tax.SelectSingleNode("descendant::ns:TaxPercentage", ns).InnerText;

            string query = "IF (SELECT COUNT(InvoiceNo) FROM Line WHERE InvoiceNo='" + FK + "' AND LineNumber="+LineNumber+" AND EmpresaId='" + EmpresaId + "') > 0 ";
            query += "UPDATE Line SET ProductCode='" + ProductCode + "', Quantity=" + Quantity + ",UnitPrice=" + UnitPrice + ",TaxPointDate='" + TaxPointDate + "',Description='" + Description + "',CreditAmount=" + CreditAmount + ",TaxType='" + TaxType + "',TaxCountryRegion='" + TaxCountryRegion + "',TaxCode='" + TaxCode + "',TaxPercentage=" + TaxPercentage + ",SettlementAmount=" + SettlementAmount + " WHERE InvoiceNo='" + FK + "' AND LineNumber ="+LineNumber +" AND EmpresaId='" + EmpresaId + "';";
            query += "ELSE ";
            query += "INSERT INTO Line(LineNumber,ProductCode,Quantity,UnitPrice,TaxPointDate,Description,CreditAmount,TaxType,TaxCountryRegion,TaxCode,TaxPercentage,SettlementAmount,InvoiceNo,EmpresaId) VALUES(" + LineNumber + ", '" + ProductCode + "', " + Quantity + "," + UnitPrice + ",'" + TaxPointDate + "','" + Description + "'," + CreditAmount + ",'" + TaxType + "','" + TaxCountryRegion + "','" + TaxCode + "'," + TaxPercentage + "," + SettlementAmount + ",'" + FK + "','" + EmpresaId + "')";

            SqlCommand com = new SqlCommand(query, sc);
            com.ExecuteScalar();

            /*
            //SettlementAmount
            XmlNode SettlementAmount = Line.SelectSingleNode("descendant::ns:SettlementAmount", ns);
            double settlementAmount = 0.0;
            if (SettlementAmount != null)
                settlementAmount = XmlConvert.ToDouble(SettlementAmount.InnerText);

            Model.LinhaDocVenda Linha = new Model.LinhaDocVenda();
            Linha.CodArtigo = ProductCode;
            Linha.Quantidade = Quantity;
            Linha.Unidade = UnitOfMeasure;
            Linha.PrecoUnitario = UnitPrice;
            Linha.Desconto = settlementAmount;*/


            //return Linha;

        }

        private static void parseProduct(XmlNode node, XmlNamespaceManager ns)
        {
            string ProductType = node.SelectSingleNode("descendant::ns:ProductType", ns).InnerText;
            string ProductCode = node.SelectSingleNode("descendant::ns:ProductCode", ns).InnerText;
            string ProductDescription = sanitize(node.SelectSingleNode("descendant::ns:ProductDescription", ns).InnerText);
            string ProductNumberCode = node.SelectSingleNode("descendant::ns:ProductNumberCode", ns).InnerText;
            double STKAtual;
            double inicial_price = 0;



            if (!ProductCode.Equals("Especial"))
            {
                STKAtual = StkArtigo[ProductCode];
                if (PriceArtigo.ContainsKey(ProductCode))
                    inicial_price = PriceArtigo[ProductCode];
            }
            else
            {
                STKAtual = 0;
            }

            String stkA = STKAtual.ToString().Replace(",", ".");
            String ip = inicial_price.ToString().Replace(",", ".");

            ProductCode = ProductCode.Replace(".", "");

            string query = "IF (SELECT COUNT(CodArtigo) FROM Artigo WHERE CodArtigo='" + ProductCode + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
            query += "UPDATE Artigo SET DescArtigo='" + ProductDescription + "', TipoArtigo='" + ProductType + "', STKAtual=" + stkA + " , Price=" + ip + " WHERE CodArtigo='" + ProductCode + "' AND EmpresaId='" + EmpresaId + "'; ";
            query += "ELSE ";
            query += "INSERT INTO Artigo (CodArtigo, DescArtigo, TipoArtigo, STKAtual, Price, EmpresaId) VALUES('" + ProductCode + "', '" + ProductDescription + "', '" + ProductType + "', " + stkA + " , " + ip + ",'"+EmpresaId+"');";
            SqlCommand com = new SqlCommand(query, sc);
            com.ExecuteScalar();
        }

        private static void parseSupplier(XmlNode node, XmlNamespaceManager ns)
        {
            string SupplierID = node.SelectSingleNode("descendant::ns:SupplierID", ns).InnerText;
            string AccountID = node.SelectSingleNode("descendant::ns:AccountID", ns).InnerText;
            string SupplierTaxID = node.SelectSingleNode("descendant::ns:SupplierTaxID", ns).InnerText;
            string CompanyName = sanitize(node.SelectSingleNode("descendant::ns:CompanyName", ns).InnerText);

            XmlNode contact = node.SelectSingleNode("descendant::ns:Contact", ns);
            string Contact = "";
            if (contact != null)
                Contact = contact.InnerText;

            XmlNode addressNode = node.SelectSingleNode("descendant::ns:BillingAddress", ns);
            int bk = parseBilling(addressNode, ns);


            string query = "INSERT INTO Supplier VALUES('" + SupplierID + "', '" + SupplierTaxID + "', '" + CompanyName + "'," + bk + ",'" + EmpresaId + "')";
            SqlCommand com = new SqlCommand(query, sc);
            com.ExecuteScalar();

        }

        private static void parseCostumer(XmlNode node, XmlNamespaceManager ns)
        {
            //XmlNode node1 = node.SelectSingleNode("/CustomerID", ns);                             null
            //XmlNode node3 = node.SelectSingleNode("/ns:CustomerID", ns);                          null
            //XmlNode node2 = node.ChildNodes[0];                                                   CustomerID
            //XmlNode node3 = node.SelectSingleNode("//ns:CustomerID", ns);                         Possible wrong CustomerID
            //XmlNode node4 = node.SelectSingleNode("descendant::ns:CustomerID", ns);               CustomerID
            //XmlNode node5 = node["CustomerID"];                                                   CustomerID

            string CostumerID = node["CustomerID"].InnerText.Replace(".","");
            string AccountID = node["AccountID"].InnerText;
            string CustomerTaxID = node["CustomerTaxID"].InnerText;
            string CompanyName = node["CompanyName"].InnerText;

            XmlNode addressNode = node.SelectSingleNode("descendant::ns:BillingAddress", ns);
            int bk = parseBilling(addressNode, ns);

            string query = "IF (SELECT COUNT(CustomerID) FROM Cliente WHERE CustomerID='" + CostumerID + "' AND EmpresaId='" + EmpresaId + "') > 0 ";
            query += "UPDATE Cliente SET CostumerTaxID='" + CustomerTaxID + "', CompanyName='" + CompanyName + "',BillingAddress=" + bk + " WHERE CustomerID='" + CostumerID + "' AND EmpresaId='" + EmpresaId + "';";
            query += "ELSE ";
            query += "INSERT INTO Cliente VALUES('" + CostumerID + "', '" + CustomerTaxID + "', '" + CompanyName + "'," + bk + ",'" + EmpresaId + "')";

            SqlCommand com = new SqlCommand(query, sc);
            com.ExecuteScalar();
        }

        private static int parseBilling(XmlNode addressNode, XmlNamespaceManager ns)
        {
            if (addressNode != null)
            {
                string AddressDetail = sanitize(addressNode.SelectSingleNode("descendant::ns:AddressDetail", ns).InnerText);
                string City = addressNode.SelectSingleNode("descendant::ns:City", ns).InnerText;
                string PostalCode = addressNode.SelectSingleNode("descendant::ns:PostalCode", ns).InnerText;
                string Country = addressNode.SelectSingleNode("descendant::ns:Country", ns).InnerText;

                string query = "INSERT INTO BillingAddress (AddressDetail,City,PostalCode,Country,EmpresaId) OUTPUT INSERTED.Id VALUES('" + AddressDetail + "','" + City + "','" + PostalCode + "','" + Country + "','" + EmpresaId + "')";
                SqlCommand com = new SqlCommand(query, sc);
                return (Int32)com.ExecuteScalar();

            }
    
            return -1;
        }

        private static string sanitize(string s)
        {
            string r;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].Equals('\''))
                {
                    sb.Append('\'');
                }
                sb.Append(s[i]);
            }
            r = sb.ToString();
            return r;
        }



        #endregion
    }
}