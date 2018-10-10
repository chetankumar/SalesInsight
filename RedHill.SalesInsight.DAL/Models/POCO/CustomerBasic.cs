namespace RedHill.SalesInsight.DAL.Models.POCO
{
    public class CustomerBasic
    {
        public int CustomerId { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public bool Active { get; set; }
        public string Group { get; set; }
        public string GroupHandle { get; set; }
        public int? id { get; set; }
        public string value { get; set; }
    }
}
