using BookStoreApp.Models;

namespace BookStoreApp.Controllers.Model {
    public class PaymentViewModel {
        public int? BookId { get; set; }
        public Book? Book {get;set;}
        public ContactResponse? ContactResponse {get;set;}
        public BagViewModel? BagViewModel { get; set; }
        public string? PromoCode {get;set;}
        public decimal DiscountAmount {get;set;} = 0;
    }
}