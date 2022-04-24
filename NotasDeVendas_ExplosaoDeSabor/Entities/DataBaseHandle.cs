using System.Data;
using System.Data.SQLite;
using NotasDeVendas_ExplosaoDeSabor.Entities;

// Tables Names:
    //TB_BrazilianStates -> Estados do Brasil
    //TB_Clients -> Clientes Cadastrados
    //TB_Product -> Produtos Cadastrados
    //TB_Orders -> Registro de Vendas
    //TB_OrderItem -> Itens das Vendas

namespace NotasDeVendas_ExplosaoDeSabor.Entities
{
    class DataBaseHandle
    {
        private static SQLiteConnection _connection;

        //private static string DataBasePath = @"G:\My Drive\28 - Back End\1 - CSharp\3 - Projetos\NotasDeVendas_ExplosaoDeSabor\NotasDeVendas_ExplosaoDeSabor\DB\DataBase.db";
        private static string fileName = "DataBase.db"; // Nome da base dados
        private static string DataBasePath = Path.Combine(Path.GetFullPath(@"..\..\..\"), @"DB\", fileName);


        private static SQLiteConnection DataBaseConnection()
        {
            _connection = new SQLiteConnection($"Data Source={DataBasePath}");
            _connection.Open();

            return _connection;
        }

        //Método para Leitura de Valores
        public static DataTable ReadData(string sql)
        {
            SQLiteDataAdapter dataAdapter = null;
            DataTable table = new DataTable();

            try {
                using (SQLiteCommand command = DataBaseConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    dataAdapter = new SQLiteDataAdapter(command.CommandText, DataBaseConnection());
                    dataAdapter.Fill(table);
                    DataBaseConnection().Close();
                    return table;
                }
            
            }catch(Exception)
            {
                throw;
            }
        }

        //****************TB_Orders****************
        // Método para Escrita de Valores - TB_Orders
        public static void WriteDataIntoTB_Orders(Int64 orderNumber, double orderValue, DateTime orderDate, Int64 orderClient)
        {
            try
            {
                // ' ' para strings e datetime - Estou escrevendo uma Query!!!
                SQLiteCommand command = DataBaseConnection().CreateCommand();
                command.CommandText = $"INSERT INTO TB_Orders (Int_OrderNumber, Real_OrderValue, Date_OrderDate, Int_ClientId) " +
                    $"VALUES ({orderNumber}, {orderValue}, '{orderDate}', {orderClient})";
                command.ExecuteNonQuery();

                MessageBox.Show("Dados gravados com sucesso!");

                DataBaseConnection().Close();

            } catch(Exception e)
            {
                MessageBox.Show($"Erro ao gravar dados: {e.Message}");
            }
        }

        //****************TB_OrderItem****************
        // Método para Escrita de Valores - TB_OrderItem
        public static void WriteDataIntoTB_OrderItem(Int64 orderId, Int64 orderItemNumber, Int64 productQty, string product, double productValue, double productTotalValue)
        {
            try
            {
                // ' ' para strings e datetime - Estou escrevendo uma Query!!!
                SQLiteCommand command = DataBaseConnection().CreateCommand();
                command.CommandText = $"INSERT INTO TB_OrderItem (Int_OrderId, Int_ItemNumber, Int_ProductQty, Str_Product, Real_ProdValue, Real_ProdTotalValue) " +
                    $"VALUES ({orderId}, {orderItemNumber}, {productQty}, '{product}', {productValue}, {productTotalValue})";
                command.ExecuteNonQuery();

                DataBaseConnection().Close();

            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao gravar dados: {e.Message}");
            }
        }

        //****************TB_Product****************
        // Método para Escrita de Valores - TB_Product
        public static void WriteDataIntoTB_Product(Product product)
        {
            try
            {
                // ' ' para strings e datetime - Estou escrevendo uma Query!!!
                SQLiteCommand command = DataBaseConnection().CreateCommand();
                command.CommandText = $"INSERT INTO TB_Product (Str_Product, Real_ProdValue) " +
                    $"VALUES ('{product.ProductName}', {product.Price})";
                command.ExecuteNonQuery();

                MessageBox.Show("Produto criado com sucesso!");

                DataBaseConnection().Close();

            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao criar produto: {e.Message}");
            }
        }

        // Método para Atualziação de Valores - TB_Product
        public static void UpdateDataIntoTB_Product(Product product)
        {
            try
            {
                // ' ' para strings e datetime - Estou escrevendo uma Query!!!
                SQLiteCommand command = DataBaseConnection().CreateCommand();
                command.CommandText = $"UPDATE TB_Product " +
                    $"SET Str_Product = '{product.ProductName}', Real_ProdValue = {product.Price} " +
                    $"WHERE Int_ID = {product.Id}";
                command.ExecuteNonQuery();

                MessageBox.Show("Produto atualizado com sucesso!");

                DataBaseConnection().Close();

            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao atualizar produto: {e.Message}");
            }
        }

        // Método para Remoção de Valores - TB_Product
        public static void DeteleDataIntoTB_Product(Product product)
        {
            try
            {
                SQLiteCommand command = DataBaseConnection().CreateCommand();
                command.CommandText = $"DELETE FROM TB_Product WHERE TB_Product.Int_ID = {product.Id}";

                command.ExecuteNonQuery();

                MessageBox.Show("Produto apagados com sucesso!");

                DataBaseConnection().Close();

            }catch(Exception e)
            {
                MessageBox.Show($"Falha ao deletar produto: {e.Message}");
            }
        }

        //****************TB_Clients****************
        // Método para Escrita de Valores - TB_Clients
        public static void WriteDataIntoTB_Clients(Client client)
        {
            try
            {
                // ' ' para strings e datetime - Estou escrevendo uma Query!!!
                SQLiteCommand command = DataBaseConnection().CreateCommand();
                command.CommandText = $"INSERT INTO TB_Clients (Str_Name, Str_Contact, Str_AddressStreet, Str_AddressNumber, Str_AddressComplement, Str_ZipCode, Str_City, Str_State) " +
                    $"VALUES ('{client.Name}', '{client.Contact}', '{client.AddressStreet}', '{client.AddressNumber}', '{client.AddressComplement}', '{client.ZipCode}', '{client.City}', '{client.State}')";
                command.ExecuteNonQuery();

                MessageBox.Show("Cliente cadastrado com sucesso!");

                DataBaseConnection().Close();

            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao cadastrar cliente: {e.Message}");
            }
        }

        // Método para Atualziação de Valores - TB_Clients
        public static void UpdateDataIntoTB_Clients(Client client)
        {
            try
            {
                // ' ' para strings e datetime - Estou escrevendo uma Query!!!
                SQLiteCommand command = DataBaseConnection().CreateCommand();
                command.CommandText = $"UPDATE TB_Clients " +
                    $"SET Str_Name = '{client.Name}', Str_Contact = '{client.Contact}', Str_AddressStreet = '{client.AddressStreet}', Str_AddressNumber = '{client.AddressNumber}', Str_AddressComplement = '{client.AddressComplement}', Str_ZipCode = '{client.ZipCode}', Str_City = '{client.City}', Str_State = '{client.State}' " +
                    $"WHERE Int_ID = {client.Id}";
                command.ExecuteNonQuery();

                MessageBox.Show("Dados do cliente atualizados com sucesso!");

                DataBaseConnection().Close();

            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao atualizar dados do cliente: {e.Message}");
            }
        }

        // Método para Remoção de Valores - TB_Clients
        public static void DeteleDataIntoTB_Clients(Client client)
        {
            try
            {
                SQLiteCommand command = DataBaseConnection().CreateCommand();
                command.CommandText = $"DELETE FROM TB_Clients WHERE TB_Clients.Int_ID = {client.Id}";

                command.ExecuteNonQuery();

                MessageBox.Show("Cliente apagado com sucesso!");

                DataBaseConnection().Close();

            }
            catch (Exception e)
            {
                MessageBox.Show($"Falha ao deletar cliente: {e.Message}");
            }
        }
    }
}
