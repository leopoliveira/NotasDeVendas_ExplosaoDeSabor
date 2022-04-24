using NotasDeVendas_ExplosaoDeSabor.Entities;
using System.Data;

namespace NotasDeVendas_ExplosaoDeSabor
{
    public partial class InitialScreen : Form
    {
        public DataTable productDataTable = new DataTable();
        public DataTable clientsDataTable = new DataTable();
        public DataTable brazilianStatesDataTable = new DataTable();

        public InitialScreen()
        {
            InitializeComponent();

            SalesDateTxtBox.Text = DateTime.Now.ToString();
            DeliveryDateTxtBox.Text = DateTime.Now.ToString();

            // Deixa alguns botões invisíveis
            DeleteProductItemBtn.Visible = false;
            UpdateProductBtn.Visible = false;
            DeleteProductBtn.Visible = false;
            UpdateClientBtn.Visible = false;
            DeleteClientBtn.Visible = false;
            NewOrderBtn.Visible = false;

            OrderNumberTxtBoxValueHandle(); // Preenche o número do pedido na tela de vendas

            ProductReadHandle(); // Preenche ComboBox do Produto na tela de vendas
            // Preenche o ComboBox do Cliente na tela de vendas e a lista de Clientes na tela de Clientes
            ClientReadHandle();
            ClientAddressStateReadHandle(); // Preenche o ComboBox do Estado do Cliente na tela de cadastro
        }

        //****************Tela de Vendas****************

        // Menus
        private void SalesMenuTxt_Click(object sender, EventArgs e)
        {
            // Vai para tela de Vendas
            ContainerScreen.SelectTab(0);
        }

        private void ProductMenuTxt_Click(object sender, EventArgs e)
        {
            // Vai para tela de Produtos
            ContainerScreen.SelectTab(1);
        }

        private void ClientsMenuTxt_Click(object sender, EventArgs e)
        {
            // Vai para tela de Clientes
            ContainerScreen.SelectTab(2);
        }

        private void DashboardMenuTxt_Click(object sender, EventArgs e)
        {
            // Vai para tela do Dashboard
        }

        private void LogoImg_Click(object sender, EventArgs e)
        {
            ContainerScreen.SelectTab(0);
        }

        // Verifica a aba selecionada atualmente do Container de páginas
        private void ContainerScreen_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ContainerScreen.SelectedIndex)
            {
                case 0:
                    ClientComboBox.Focus();
                    GeneratePdfBtn.Visible = true;
                    break;
                case 1:
                    ProductNameTxtBox.Focus();
                    GeneratePdfBtn.Visible = false;
                    break;
                case 2:
                    ClientNameSignTxtBox.Focus();
                    GeneratePdfBtn.Visible = false;
                    break;
            }
        }

        // Pega registros dentro da base de produtos
        private void ProductReadHandle()
        {

            string sql = "SELECT * FROM TB_Product";

            productDataTable = DataBaseHandle.ReadData(sql);

            if (productDataTable.Rows.Count > 1)
            {
                ProductsListView.Items.Clear();

                foreach (DataRow row in productDataTable.Rows)
                {
                    ProductComboBox.Items.Add(row.Field<string>("Str_Product"));

                    ListViewItem item = new ListViewItem(row.Field<Int64>("Int_ID").ToString());
                    item.SubItems.Add(row.Field<string>("Str_Product"));
                    item.SubItems.Add($"R$ {row.Field<double>("Real_ProdValue").ToString("F2")}");
                    ProductsListView.Items.Add(item);
                }
            }
            else
            {
                MessageBox.Show("Não foram encontrados produtos cadastrados!\nFavor cadastrar na aba Produtos");
            }
        }

        // Pega registros de clientes na base
        private void ClientReadHandle()
        {

            string sql = "SELECT * FROM TB_Clients";

            clientsDataTable = DataBaseHandle.ReadData(sql);

            if (clientsDataTable.Rows.Count > 1)
            {
                ClientListView.Items.Clear();

                foreach (DataRow row in clientsDataTable.Rows)
                {
                    ClientComboBox.Items.Add($"{row.Field<string>("Str_Name")}, {row.Field<string>("Str_City")}, {row.Field<string>("Str_State")}");

                    ListViewItem item = new ListViewItem(row.Field<Int64>("Int_ID").ToString());
                    item.SubItems.Add(row.Field<string>("Str_Name"));
                    item.SubItems.Add(row.Field<string>("Str_Contact"));
                    item.SubItems.Add(row.Field<string>("Str_AddressStreet"));
                    item.SubItems.Add(row.Field<string>("Str_AddressNumber"));
                    item.SubItems.Add(row.Field<string>("Str_AddressComplement"));
                    item.SubItems.Add(row.Field<string>("Str_ZipCode"));
                    item.SubItems.Add(row.Field<string>("Str_City"));
                    item.SubItems.Add(row.Field<string>("Str_State"));

                    ClientListView.Items.Add(item);
                }
            }
            else
            {
                MessageBox.Show("Não foram encontrados clientes cadastrados!\nFavor cadastrar na aba clientes");
            }
        }

        // Pega os registro dos estados brasileiros na base
        private void ClientAddressStateReadHandle()
        {

            string sql = "SELECT * FROM TB_BrazilianStates";

            brazilianStatesDataTable = DataBaseHandle.ReadData(sql);

            if (brazilianStatesDataTable.Rows.Count > 0)
            {
                foreach (DataRow row in brazilianStatesDataTable.Rows)
                {
                    ClientStateSignComboBox.Items.Add(row.Field<string>("Str_State"));
                }

            }
            else
            {
                MessageBox.Show("Ocorreu um erro ao buscar registros em 'BrazilianStates Data Base'");
            }
        }

        // Reseta tela de vendas para ficar pronto para uma nova venda
        public void NewOrder()
        {
            OrderNumberTxtBox.Clear();
            OrderNumberTxtBoxValueHandle();

            OrderItemsListView.Items.Clear();
            SalesScreenRegistersReset();

            NewOrderBtn.Visible = false;

            MessageBox.Show("Olá");
        }

        // Procura o valor do produto selecionado na tela de vendas e preenche o campo ProductSingleValueTxtBox
        private void FindSelectedProductSinglePrice(string? selectedProduct)
        {

            string sql = $"SELECT TB_Product.Real_ProdValue FROM TB_Product WHERE TB_Product.Str_Product = '{selectedProduct}'";

            DataTable selectedProductPrice = new DataTable();

            selectedProductPrice = DataBaseHandle.ReadData(sql);

            if (selectedProductPrice.Rows.Count > 0)
            {
                ProductSingleValueTxtBox.Clear();
                ProductSingleValueTxtBox.Text = $"{selectedProductPrice.Rows[0].Field<double>("Real_ProdValue").ToString("F2")}";
            }
            else
            {
                MessageBox.Show("O produto selecionado não tem um valor cadastrado.\nFavor atualizar o produto na tela de cadastro do produto.");
            }


        }

        // Preenche o valor do número da venda baseado na venda anterior
        private void OrderNumberTxtBoxValueHandle()
        {
            string sql = "SELECT TB_Orders.Int_ID FROM TB_Orders ORDER BY TB_Orders.Int_ID DESC";

            DataTable salesIdDataTable = DataBaseHandle.ReadData(sql);

            long salesNumber = 1;

            OrderNumberTxtBox.Clear();
            if (salesIdDataTable.Rows.Count > 0)
            {
                salesNumber = salesIdDataTable.Rows[0].Field<Int64>("Int_ID");
                salesNumber += 1;
                OrderNumberTxtBox.Text = salesNumber.ToString().PadLeft(10, '0');
            }
            else
            {
                OrderNumberTxtBox.Text = salesNumber.ToString().PadLeft(10, '0');
            }
        }

        // Apaga alguns campos da tela de vendas
        private void SalesScreenRegistersReset()
        {
            ProductSingleValueTxtBox.Clear();
            ProductQtyTxtBox.Text = "1";
            ProductComboBox.Text = "Selecione o Produto...";
            ClientComboBox.Text = "Selecione o Cliente...";
        }

        // Calcula valor total da venda
        private void SumSalesScreenItemsValues()
        {

            double sum = 0;

            if (OrderItemsListView.Items.Count > 0)
            {
                for (int i = 0; i < OrderItemsListView.Items.Count; i++)
                {
                    if (OrderItemsListView.Items[i].SubItems[4].Text != null)
                    {
                        sum += double.Parse(OrderItemsListView.Items[i].SubItems[4].Text);
                    }
                }

                SalesTotalValueTxtBox.Clear();
                SalesTotalValueTxtBox.Text = sum.ToString("F2");
            }
            else
            {
                SalesTotalValueTxtBox.Text = sum.ToString("F2");
            }
        }

        // Busca o valor do produto selecionado no ComboBox
        private void ProductComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FindSelectedProductSinglePrice(ProductComboBox.SelectedItem.ToString());
        }

        // Preenche os campos da tela de vendas ao selecionar um item da lista
        private void OrderItemsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OrderItemsListView.SelectedItems.Count > 0)
            {
                // Aparece com o botão
                EditProductItemBtn.Visible = true;
                DeleteProductItemBtn.Visible = true;
                // Some com o botão
                AddProductItemBtn.Visible = false;

                ProductQtyTxtBox.Text = OrderItemsListView.SelectedItems[0].SubItems[1].Text;
                ProductComboBox.Text = OrderItemsListView.SelectedItems[0].SubItems[2].Text;
                ProductSingleValueTxtBox.Text = OrderItemsListView.SelectedItems[0].SubItems[3].Text;
            }
        }

        // Permite apenas caracteres numéricos e um . no campo Valor Unitário
        private void ProductSingleValueTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        // Permite apenas caracteres numérico no campo Quantidade
        private void ProductQtyTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }
        
        // Adiciona produto a lista de itens da venda
        private void AddProductItemBtn_Click(object sender, EventArgs e)
        {
            // Adiciona itens na lista de produtos da venda
            ListViewItem item = new ListViewItem(OrderItemsListView.Items.Count.ToString());
            item.SubItems.Add(ProductQtyTxtBox.Text);
            item.SubItems.Add(ProductComboBox.Text);
            if (ProductSingleValueTxtBox.Text.Contains(','))
            {
                ProductSingleValueTxtBox.Text = ProductSingleValueTxtBox.Text.Replace(',', '.');
            }
            item.SubItems.Add(ProductSingleValueTxtBox.Text);

            double totalPrice = int.Parse(ProductQtyTxtBox.Text) * double.Parse(ProductSingleValueTxtBox.Text);

            item.SubItems.Add(totalPrice.ToString());

            OrderItemsListView.Items.Add(item);

            // Calcula valor total do item
            SumSalesScreenItemsValues();

            // Apaga alguns campos
            SalesScreenRegistersReset();

            // Aparece com botão
            NewOrderBtn.Visible = true;
        }

        // Edita produto da lista de itens de venda
        private void EditProductItemBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < OrderItemsListView.Items.Count; i++)
            {
                for (int k = 0; k < OrderItemsListView.Items[i].SubItems.Count; k++)
                {
                    if (OrderItemsListView.Items[i].SubItems[0].Text == OrderItemsListView.SelectedItems[0].SubItems[0].Text)
                    {
                        OrderItemsListView.SelectedItems[0].SubItems[1].Text = ProductQtyTxtBox.Text;
                        OrderItemsListView.SelectedItems[0].SubItems[2].Text = ProductComboBox.Text;
                        if (ProductSingleValueTxtBox.Text.Contains(','))
                        {
                            ProductSingleValueTxtBox.Text = ProductSingleValueTxtBox.Text.Replace(',', '.');
                        }
                        OrderItemsListView.SelectedItems[0].SubItems[3].Text = ProductSingleValueTxtBox.Text;

                        double totalPrice = int.Parse(ProductQtyTxtBox.Text) * double.Parse(ProductSingleValueTxtBox.Text);

                        OrderItemsListView.SelectedItems[0].SubItems[4].Text = totalPrice.ToString("F2");

                        // Calcula valor total do item
                        SumSalesScreenItemsValues();

                        // Apaga alguns campos
                        SalesScreenRegistersReset();

                        // Some com o botão
                        EditProductItemBtn.Visible = false;
                        DeleteProductItemBtn.Visible = false;

                        // Aparece com o botão
                        AddProductItemBtn.Visible = true;

                        return;
                    }
                }
            }
        }

        // Deleta produto da lista de itens de venda
        private void DeleteProductItemBtn_Click(object sender, EventArgs e)
        {
            OrderItemsListView.Items.RemoveAt(OrderItemsListView.SelectedIndices[0]);

            // Soma valores dos itens do pedido
            SumSalesScreenItemsValues();

            // Some com o botão
            DeleteProductItemBtn.Visible = false;

            // Aparece com o botão
            AddProductItemBtn.Visible = true;
        }

        // Grava no Banco de Dados e gera um PDF dos itens do pedido
        private void GeneratePdfBtn_Click(object sender, EventArgs e)
        {

            // Guarda itens de venda no banco de dados
            if (OrderItemsListView.Items.Count > 0)
            {
                if (ClientComboBox.Text == "Selecione o Cliente...")
                {
                    MessageBox.Show("Favor selecionar o Cliente que receberá o pedido!");
                    ClientComboBox.Focus();
                    return;
                }
                else
                {
                    // Procura ID do Cliente da Venda
                    string[] clientData = ClientComboBox.Text.Split(',');

                    string sql = $"SELECT TB_Clients.Int_ID FROM TB_Clients WHERE (TB_Clients.Str_Name = '{clientData[0].Trim()}') AND (TB_Clients.Str_City = '{clientData[1].Trim()}') AND (TB_Clients.Str_State = '{clientData[2].Trim()}')";

                    DataTable clientDataTable = DataBaseHandle.ReadData(sql);

                    Int64 clientId = 0;

                    if (clientDataTable.Rows.Count > 0)
                    {
                        clientId = clientDataTable.Rows[0].Field<Int64>("Int_ID");
                    }

                    // Grava registros no Banco de Dados
                    // Grava N° da venda, valor total da venda e data atual
                    DataBaseHandle.WriteDataIntoTB_Orders(
                        Convert.ToInt64(OrderNumberTxtBox.Text), // N° da Venda
                        Convert.ToDouble(SalesTotalValueTxtBox.Text), // Valor total da Venda
                                                                      // Data da Venda
                        Convert.ToDateTime(SalesDateTxtBox.Value.ToUniversalTime()),
                        clientId // Id do Cliente da Venda
                        );

                    // Grava items da venda
                    for (int i = 0; i < OrderItemsListView.Items.Count; i++)
                    {

                        DataBaseHandle.WriteDataIntoTB_OrderItem(
                            Convert.ToInt64(OrderNumberTxtBox.Text),
                            Convert.ToInt64(OrderItemsListView.Items[i].SubItems[0].Text), // N° Item
                            Convert.ToInt64(OrderItemsListView.Items[i].SubItems[1].Text), // Quantidade do Produto
                            OrderItemsListView.Items[i].SubItems[2].Text, // Produto
                            Convert.ToDouble(OrderItemsListView.Items[i].SubItems[3].Text), // Valor Un. Produto
                            Convert.ToDouble(OrderItemsListView.Items[i].SubItems[4].Text) // Valor total Produto
                            );
                    }

                    // Gera o PDF
                    PDFGenerator.GeneratePDF(Convert.ToInt64(OrderNumberTxtBox.Text), SalesTotalValueTxtBox.Text, clientId);

                    // Pergunta se quer abrir o arquivo
                    DialogResult openPdf = MessageBox.Show("Nota de Venda gerada com sucesso!", "Deseja abrir a nota gerada?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (openPdf == DialogResult.Yes)
                    {
                        PDFGenerator.OpenPdfFile(OrderNumberTxtBox.Text);
                        NewOrder();
                    }
                    else
                    {
                        NewOrder();
                    }
                }
            }
            else
            {
                MessageBox.Show("Não é possível fechar um pedido sem produtos adicionados!\nPor favor, adicione ao menos um produto na lista e tente novamente...");
                ProductComboBox.Focus();
            }
        }

        // Limpa a tela de vendas para receber um novo pedido
        private void NewOrderBtn_Click(object sender, EventArgs e)
        {
            NewOrder();
        }

        //****************Tela de Produtos****************

        // Permite apenas caracteres numéricos e um . no campo Valor Unitário
        private void ProductValueTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        // Preenche os campos da tela de produtos ao selecionar um item da lista
        private void ProductsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProductsListView.SelectedItems.Count > 0)
            {
                // Aparece com o botão
                UpdateProductBtn.Visible = true;
                DeleteProductBtn.Visible = true;
                // Some com o botão
                CreateProductBtn.Visible = false;

                ProductCodeTxtBox.Text = ProductsListView.SelectedItems[0].SubItems[0].Text;
                ProductNameTxtBox.Text = ProductsListView.SelectedItems[0].SubItems[1].Text;
                ProductValueTxtBox.Text = ProductsListView.SelectedItems[0].SubItems[2].Text.Replace("R$ ", "");
            }
        }

        // Cria um novo produto
        private void CreateProductBtn_Click(object sender, EventArgs e)
        {
            if (ProductNameTxtBox.Text == "")
            {
                MessageBox.Show("O campo Produto não pode ficar em branco");

            }
            else if (ProductValueTxtBox.Text == "")
            {
                MessageBox.Show("O campo Valor Unitário (Un.) não pode ficar em branco");

            }
            else
            {
                // Grava dados dentro do Banco de Dados
                Product product = new Product();

                product.ProductName = ProductNameTxtBox.Text;
                product.Price = Convert.ToDouble(ProductValueTxtBox.Text);

                DataBaseHandle.WriteDataIntoTB_Product(product);

                // Atualiza lista de produtos
                ProductReadHandle();

                // Limpa caixas de texto
                ProductNameTxtBox.Clear();
                ProductValueTxtBox.Clear();
            }
        }

        // Edita um produto existente
        private void UpdateProductBtn_Click(object sender, EventArgs e)
        {
            if (ProductNameTxtBox.Text == "")
            {
                MessageBox.Show("O campo Produto não pode ficar em branco");
            }
            else if (ProductValueTxtBox.Text == "")
            {
                MessageBox.Show("O campo Valor Un. não pode ficar em branco");
            }
            else
            {
                // Grava dados dentro do Banco de Dados

                if (ProductValueTxtBox.Text.Contains(','))
                {
                    ProductValueTxtBox.Text = ProductValueTxtBox.Text.Replace(',', '.');
                }

                Product product = new Product();

                product.Id = Convert.ToInt64(ProductCodeTxtBox.Text);
                product.ProductName = ProductNameTxtBox.Text;
                product.Price = Convert.ToDouble(ProductValueTxtBox.Text);

                DataBaseHandle.UpdateDataIntoTB_Product(product);

                // Atualiza lista de produtos
                ProductReadHandle();

                // Some com o botão
                UpdateProductBtn.Visible = false;
                DeleteProductBtn.Visible = false;

                // Limpa caixas de texto
                ProductCodeTxtBox.Clear();
                ProductNameTxtBox.Clear();
                ProductValueTxtBox.Clear();

                // Aparece com o botão
                CreateProductBtn.Visible = true;
            }
        }

        // Deleta um produto existente
        private void DeleteProductBtn_Click(object sender, EventArgs e)
        {
            // Deleta Produto do Banco de Dados
            Product product = new Product();

            product.Id = Convert.ToInt64(ProductsListView.SelectedItems[0].SubItems[0].Text);

            DataBaseHandle.DeteleDataIntoTB_Product(product);

            // Atualiza lista de produtos
            ProductReadHandle();

            // Some com o botão
            UpdateProductBtn.Visible = false;
            DeleteProductBtn.Visible = false;

            // Limpa caixas de texto
            ProductCodeTxtBox.Clear();
            ProductNameTxtBox.Clear();
            ProductValueTxtBox.Clear();

            // Aparece com o botão
            CreateProductBtn.Visible = true;

        }

        //****************Tela de Clientes****************

        // Permite apenas caracteres numéricos nos campos de Contato e CEP
        private void ClientContactSignTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void ClientZIPCodeSignTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        // Preenche os campos da tela de clientes ao selecionar um item da lista
        private void ClientListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ClientListView.SelectedItems.Count > 0)
            {
                // Aparece com o botão
                UpdateClientBtn.Visible = true;
                DeleteClientBtn.Visible = true;
                // Some com o botão
                CreateClientBtn.Visible = false;

                ClientCodSignTxtBox.Text = ClientListView.SelectedItems[0].SubItems[0].Text;
                ClientNameSignTxtBox.Text = ClientListView.SelectedItems[0].SubItems[1].Text;
                ClientContactSignTxtBox.Text = ClientListView.SelectedItems[0].SubItems[2].Text;
                ClientAdressSignTxtBox.Text = ClientListView.SelectedItems[0].SubItems[3].Text;
                ClientAdrsNumberTxtBox.Text = ClientListView.SelectedItems[0].SubItems[4].Text;
                ClientComplementSignTxtBox.Text = ClientListView.SelectedItems[0].SubItems[5].Text;
                ClientZIPCodeSignTxtBox.Text = ClientListView.SelectedItems[0].SubItems[6].Text;
                ClientCitySignTxtBox.Text = ClientListView.SelectedItems[0].SubItems[7].Text;
                ClientStateSignComboBox.Text = ClientListView.SelectedItems[0].SubItems[8].Text;
            }
        }

        // Cria um novo cliente
        private void CreateClientBtn_Click(object sender, EventArgs e)
        {
            if (ClientNameSignTxtBox.Text == "")
            {
                MessageBox.Show("O campo Nome não pode ficar em branco");

            }
            else if (!ClientContactSignTxtBox.MaskCompleted)
            {
                MessageBox.Show("O campo Contato não pode ficar em branco");

            }
            else
            {
                // Grava dados dentro do Banco de Dados
                Client client = new Client();

                client.Name = ClientNameSignTxtBox.Text;
                client.Contact = ClientContactSignTxtBox.Text;
                client.AddressStreet = ClientAdressSignTxtBox.Text;
                client.AddressNumber = ClientAdrsNumberTxtBox.Text;
                client.AddressComplement = ClientComplementSignTxtBox.Text;
                client.ZipCode = ClientZIPCodeSignTxtBox.Text;
                client.City = ClientCitySignTxtBox.Text;
                client.State = ClientStateSignComboBox.Text;

                DataBaseHandle.WriteDataIntoTB_Clients(client);

                // Atualiza lista de clientes
                ClientReadHandle();

                // Limpa caixas de texto
                ClientCodSignTxtBox.Clear();
                ClientNameSignTxtBox.Clear();
                ClientContactSignTxtBox.Clear();
                ClientAdressSignTxtBox.Clear();
                ClientAdrsNumberTxtBox.Clear();
                ClientComplementSignTxtBox.Clear();
                ClientZIPCodeSignTxtBox.Clear();
                ClientCitySignTxtBox.Clear();
                ClientStateSignComboBox.Text = "Selecione o Estado";
            }
        }

        // Edita um cliente existente
        private void EditClientBtn_Click(object sender, EventArgs e)
        {
            if (ClientNameSignTxtBox.Text == "")
            {
                MessageBox.Show("O campo Nome não pode ficar em branco");

            }
            else if (ClientContactSignTxtBox.Text == "")
            {
                MessageBox.Show("O campo Contato não pode ficar em branco");

            }
            else
            {
                // Grava dados dentro do Banco de Dados
                Client client = new Client();

                client.Id = Convert.ToInt64(ClientCodSignTxtBox.Text);
                client.Name = ClientNameSignTxtBox.Text;
                client.Contact = ClientContactSignTxtBox.Text;
                client.AddressStreet = ClientAdressSignTxtBox.Text;
                client.AddressNumber = ClientAdrsNumberTxtBox.Text;
                client.AddressComplement = ClientComplementSignTxtBox.Text;
                client.ZipCode = ClientZIPCodeSignTxtBox.Text;
                client.City = ClientCitySignTxtBox.Text;
                client.State = ClientStateSignComboBox.Text;

                DataBaseHandle.UpdateDataIntoTB_Clients(client);

                // Atualiza lista de produtos
                ClientReadHandle();

                // Some com o botão
                UpdateClientBtn.Visible = false;
                DeleteClientBtn.Visible = false;

                // Limpa caixas de texto
                ClientCodSignTxtBox.Clear();
                ClientNameSignTxtBox.Clear();
                ClientContactSignTxtBox.Clear();
                ClientAdressSignTxtBox.Clear();
                ClientAdrsNumberTxtBox.Clear();
                ClientComplementSignTxtBox.Clear();
                ClientZIPCodeSignTxtBox.Clear();
                ClientCitySignTxtBox.Clear();
                ClientStateSignComboBox.Text = "Selecione o Estado";

                // Aparece com o botão
                CreateClientBtn.Visible = true;
            }
        }

        // Deleta um cliente existente
        private void DeleteClientBtn_Click(object sender, EventArgs e)
        {
            // Deleta Produto do Banco de Dados
            Client client = new Client();

            client.Id = Convert.ToInt64(ClientListView.SelectedItems[0].SubItems[0].Text);

            DataBaseHandle.DeteleDataIntoTB_Clients(client);

            // Atualiza lista de produtos
            ClientReadHandle();

            // Some com o botão
            UpdateClientBtn.Visible = false;
            DeleteClientBtn.Visible = false;

            // Limpa caixas de texto
            ClientCodSignTxtBox.Clear();
            ClientNameSignTxtBox.Clear();
            ClientContactSignTxtBox.Clear();
            ClientAdressSignTxtBox.Clear();
            ClientAdrsNumberTxtBox.Clear();
            ClientComplementSignTxtBox.Clear();
            ClientZIPCodeSignTxtBox.Clear();
            ClientCitySignTxtBox.Clear();
            ClientStateSignComboBox.Text = "Selecione o Estado";

            // Aparece com o botão
            CreateClientBtn.Visible = true;

        }

    }
}