using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Diagnostics;

namespace NotasDeVendas_ExplosaoDeSabor.Entities
{
    internal class PDFGenerator
    {
        public static BaseFont fonteBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
        public static void GeneratePDF(Int64 orderNumber, string orderTotalValue, Int64 clientId)
        {
            // Configuração do PDF
            var pixelForMillimeter = 72 / 25.2f; // 72 pontos por polegada. 1 polegada = 25.2mm
            var pdf = new Document(PageSize.A4, 15 * pixelForMillimeter, 15 * pixelForMillimeter, 2 * pixelForMillimeter, 5 * pixelForMillimeter);
            var nomeArquivo = $@"C:\Vendas\Pedido_{orderNumber.ToString().PadLeft(10, '0')}.pdf";
            var arquivo = new FileStream(nomeArquivo, FileMode.Create);
            var escrever = PdfWriter.GetInstance(pdf, arquivo); // Associação do pdf com o arquivo
            pdf.Open(); // Inicializa o PDF para começar a receber contúdo

            // Título
            var fonteParagrafo = new iTextSharp.text.Font(fonteBase, 6, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLACK);
            var titulo = new Paragraph($"Pedido Nº{orderNumber.ToString().PadLeft(10, '0')}", fonteParagrafo);
            titulo.Alignment = Element.ALIGN_LEFT;
            pdf.Add(titulo);

            // Dados do cabeçalho
            var fonteDadosCabecalho = new iTextSharp.text.Font(fonteBase, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.Color.BLACK);

            // Dados da Empresa
            var cabecalho = new Paragraph("EXPLOSÃO DO SABOR", fonteDadosCabecalho);
            cabecalho.Alignment = Element.ALIGN_CENTER;
            cabecalho.IndentationLeft = 100;
            cabecalho.IndentationRight = 100;
            pdf.Add(cabecalho);
            cabecalho = new Paragraph("Fone: (62) 9 8519-8352", fonteDadosCabecalho);
            cabecalho.Alignment = Element.ALIGN_CENTER;
            cabecalho.IndentationLeft = 100;
            cabecalho.IndentationRight = 100;
            pdf.Add(cabecalho);

            fonteDadosCabecalho = new iTextSharp.text.Font(fonteBase, 8, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLACK);

            // Dados do Cliente da Venda

            // Busca os dados do Cliente da Venda na Base de Dados
            string sql = $"SELECT TB_Clients.Str_Name, TB_Clients.Str_Contact FROM TB_Clients WHERE TB_Clients.Int_ID = {clientId}";

            DataTable clientData = DataBaseHandle.ReadData(sql);

            if (clientData.Rows.Count > 0)
            {
                cabecalho = new Paragraph($"Cliente: {clientData.Rows[0].Field<string>("Str_Name")}", fonteDadosCabecalho);
                cabecalho.Alignment = Element.ALIGN_CENTER;
                cabecalho.IndentationLeft = 100;
                cabecalho.IndentationRight = 100;
                pdf.Add(cabecalho);
                cabecalho = new Paragraph($"Contato: {clientData.Rows[0].Field<string>("Str_Contact")}", fonteDadosCabecalho);
                cabecalho.Alignment = Element.ALIGN_CENTER;
                cabecalho.IndentationLeft = 100;
                cabecalho.IndentationRight = 100;
                pdf.Add(cabecalho);
            }
            else
            {
                cabecalho = new Paragraph($"", fonteDadosCabecalho);
                cabecalho.Alignment = Element.ALIGN_CENTER;
                cabecalho.IndentationLeft = 100;
                cabecalho.IndentationRight = 100;
                pdf.Add(cabecalho);
                cabecalho = new Paragraph($"", fonteDadosCabecalho);
                cabecalho.Alignment = Element.ALIGN_CENTER;
                cabecalho.IndentationLeft = 100;
                cabecalho.IndentationRight = 100;
                pdf.Add(cabecalho);
            }

            string fileName = "Logo.png"; // Nome do arquivo da imagem
            string caminhoImagem = Path.Combine(Path.GetFullPath(@"..\..\..\"), @"Images\", fileName);

            if (File.Exists(caminhoImagem))
            {
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(caminhoImagem);
                float razaoAlturaLargura = logo.Width / logo.Height;
                float alturaLogo = 45; // 70
                float larguraLogo = alturaLogo * razaoAlturaLargura;
                logo.ScaleToFit(larguraLogo, alturaLogo);

                var margemEsquerda = pdf.PageSize.Width - pdf.RightMargin - larguraLogo;
                var margemTopo = pdf.PageSize.Height - pdf.TopMargin - 54;
                logo.SetAbsolutePosition(margemEsquerda, margemTopo);
                escrever.DirectContent.AddImage(logo, false);
            }

            // Adição da tabela de dados
            var tabela = new PdfPTable(5);
            float[] larguraColunas = { 0.4f, 0.4f, 2f, 0.8f, 0.8f };
            tabela.SetWidths(larguraColunas);
            tabela.DefaultCell.BorderWidth = 0;
            tabela.WidthPercentage = 100;

            // Adição das células na tabela -- 1º Via
            CriarCelulaTexto(tabela, "Item", PdfPCell.ALIGN_CENTER, true, false, 7);
            CriarCelulaTexto(tabela, "Qtde.", PdfPCell.ALIGN_CENTER, true, false, 7);
            CriarCelulaTexto(tabela, "Produto", PdfPCell.ALIGN_LEFT, true, false, 7);
            CriarCelulaTexto(tabela, "Valor Un.", PdfPCell.ALIGN_CENTER, true, false, 7);
            CriarCelulaTexto(tabela, "Total Item", PdfPCell.ALIGN_CENTER, true, false, 7);

            SalesItemsReadHandle(tabela, orderNumber);

            // Tabela -- 1º Via
            pdf.Add(tabela);

            // Valor Total -- 1º Via
            var fonteValorTotal = new iTextSharp.text.Font(fonteBase, 8, iTextSharp.text.Font.BOLD, iTextSharp.text.Color.BLACK);
            var valorTotal = new Paragraph($"Valor Total: R$ {orderTotalValue}\n\n", fonteValorTotal);
            valorTotal.Alignment = Element.ALIGN_RIGHT;
            pdf.Add(valorTotal);

            // Dados do cliente -- 2º Via
            if (clientData.Rows.Count > 0)
            {
                cabecalho = new Paragraph($"\n2º Via", fonteDadosCabecalho);
                cabecalho.Alignment = Element.ALIGN_RIGHT;
                cabecalho.IndentationLeft = 100;
                cabecalho.IndentationRight = 0;
                pdf.Add(cabecalho);
                cabecalho = new Paragraph($"Cliente: {clientData.Rows[0].Field<string>("Str_Name")}", fonteDadosCabecalho);
                cabecalho.Alignment = Element.ALIGN_CENTER;
                cabecalho.IndentationLeft = 100;
                cabecalho.IndentationRight = 100;
                pdf.Add(cabecalho);
                cabecalho = new Paragraph($"Contato: {clientData.Rows[0].Field<string>("Str_Contact")}", fonteDadosCabecalho);
                cabecalho.Alignment = Element.ALIGN_CENTER;
                cabecalho.IndentationLeft = 100;
                cabecalho.IndentationRight = 100;
                pdf.Add(cabecalho);
            }
            else
            {
                cabecalho = new Paragraph($"", fonteDadosCabecalho);
                cabecalho.Alignment = Element.ALIGN_CENTER;
                cabecalho.IndentationLeft = 100;
                cabecalho.IndentationRight = 100;
                pdf.Add(cabecalho);
                cabecalho = new Paragraph($"", fonteDadosCabecalho);
                cabecalho.Alignment = Element.ALIGN_CENTER;
                cabecalho.IndentationLeft = 100;
                cabecalho.IndentationRight = 100;
                pdf.Add(cabecalho);
            }

            // Tabela -- 2° Via
            pdf.Add(tabela);

            // Valor Total -- 2º Via
            fonteValorTotal = new iTextSharp.text.Font(fonteBase, 8, iTextSharp.text.Font.BOLD, iTextSharp.text.Color.BLACK);
            valorTotal = new Paragraph($"Valor Total: R$ {orderTotalValue}", fonteValorTotal);
            valorTotal.Alignment = Element.ALIGN_RIGHT;
            pdf.Add(valorTotal);

            pdf.Close();

        }

        static void CriarCelulaTexto(PdfPTable tabela, string texto, int alinhamentoHorizontal = PdfPCell.ALIGN_LEFT, bool negrito = false, bool italico = false, int tamanhoFonte = 7,
            int alturaCelula = 12)
        {
            int estilo = iTextSharp.text.Font.NORMAL;

            if (negrito && italico)
            {
                estilo = iTextSharp.text.Font.BOLDITALIC;

            }
            else if (negrito)
            {
                estilo = iTextSharp.text.Font.BOLD;

            }
            else if (italico)
            {
                estilo = iTextSharp.text.Font.ITALIC;

            }

            var fontCelula = new iTextSharp.text.Font(fonteBase, tamanhoFonte, estilo, iTextSharp.text.Color.BLACK);
            var celula = new PdfPCell(new Phrase(texto, fontCelula));
            celula.HorizontalAlignment = alinhamentoHorizontal;
            celula.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            celula.Border = 2;
            celula.BorderWidthBottom = 1;
            celula.FixedHeight = alturaCelula;
            celula.PaddingBottom = 5;
            tabela.AddCell(celula);
        }

        // Obtém registros de dentro da base de vendas
        static void SalesItemsReadHandle(PdfPTable tabela, Int64 orderNumber)
        {

            string sql = $"SELECT TB_OrderItem.Int_ItemNumber, TB_OrderItem.Int_ProductQty, TB_OrderItem.Str_Product, TB_OrderItem.Real_ProdValue, TB_OrderItem.Real_ProdTotalValue FROM TB_OrderItem INNER JOIN TB_Orders ON TB_OrderItem.Int_OrderId = TB_Orders.Int_ID WHERE TB_Orders.Int_ID = {orderNumber}";

            DataTable orderItems = DataBaseHandle.ReadData(sql);

            foreach (DataRow row in orderItems.Rows)
            {
                CriarCelulaTexto(tabela, (row.Field<Int64>("Int_ItemNumber") + 1).ToString(), PdfPCell.ALIGN_CENTER, false);
                CriarCelulaTexto(tabela, row.Field<Int64>("Int_ProductQty").ToString(), PdfPCell.ALIGN_CENTER, false);
                CriarCelulaTexto(tabela, row.Field<string>("Str_Product"), PdfPCell.ALIGN_MIDDLE, false);
                CriarCelulaTexto(tabela, $"R$ {row.Field<double>("Real_ProdValue").ToString("F2")}", PdfPCell.ALIGN_CENTER, false);
                CriarCelulaTexto(tabela, $"R$ {row.Field<double>("Real_ProdTotalValue").ToString("F2")}", PdfPCell.ALIGN_CENTER, false);
            }
        }

        static public void OpenPdfFile(string orderNumber)
        {
            var path = $@"C:\Vendas\Pedido_{orderNumber.ToString().PadLeft(10, '0')}.pdf";

            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo()
                {
                    Arguments = $"/c start {path}",
                    FileName = "cmd.exe",
                    CreateNoWindow = true
                });
            }
        }
    }
}
