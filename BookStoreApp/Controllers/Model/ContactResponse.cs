namespace BookStoreApp.Controllers.Model {
    public class ContactResponse {
        public int BookId { get; set; }
        public string Name {get;set;}
        public string Mobile {get;set;}
        public string? Address {get;set;}
        public int? Number { get; set; }

        public ContactResponse(){}

        public ContactResponse(string Name, string Mobile, string Address) {
            this.Name = Name;
            this.Mobile = Mobile;
            this.Address = Address;
        }

        public ContactResponse(string Name, string Mobile, string Address, int CardNumber, int CVV) {
            this.Name = Name;
            this.Mobile = Mobile;
            this.Address = Address;
            this.Number = Number;
        }
    }
}