using BookStoreApp.Models;

namespace BookStoreApp.Controllers.Model
{
    public class BagViewModel
    {
        public List<Bag> Bags { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
    }
}
