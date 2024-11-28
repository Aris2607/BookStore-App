using BookStoreApp.Models;

namespace BookStoreApp.Data.Interfaces
{
    public interface ITransactionService
    {
        public List<TransactionItem> GetAllItems();
        public void AddItem(TransactionItem Item);
        public Task CreateTransaction(Transaction transaction);
        public Task<List<Transaction>> GetAllTransaction();
        public Task<List<Transaction>> GetAllTransactionByUserId(string Id);
        public Task UpdateTransactionStatus(int Id, string status);
        public Task<List<Bag>> GetAllBagAsync(string Id);
        public Task AddBagItem(Bag Item);
    }
}