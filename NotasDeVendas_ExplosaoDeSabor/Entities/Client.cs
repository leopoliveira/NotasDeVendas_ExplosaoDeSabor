namespace NotasDeVendas_ExplosaoDeSabor.Entities
{
    public class Client
    {
        public Int64 Id { get; set; }
        public string? Name { get; set; }
        public string? Contact { get; set; }
        public string? AddressStreet { get; set; }
        public string? AddressNumber { get; set; }
        public string? AddressComplement { get; set; }
        public string? ZipCode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }
}
