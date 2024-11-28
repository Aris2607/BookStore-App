namespace BookStoreApp.Models;

public class TransactionItem {
    public int BookId {get;set;}
    public string Title {get;set;}
    public decimal Price {get;set;}
    public string Image { get; set; }
    public int Qty { get; set; }
    public Transaction Transaction;
}