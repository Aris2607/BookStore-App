using BookStoreApp.Data;
using BookStoreApp.Data.Interfaces;
using BookStoreApp.Models;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly BookStoreAppContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly List<TransactionItem> _transaction = new();

        public TransactionService(BookStoreAppContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public List<TransactionItem> GetAllItems()
        {
            var sessionData = Session.GetString("Cart");
            return sessionData == null
                ? new List<TransactionItem>()
                : JsonSerializer.Deserialize<List<TransactionItem>>(sessionData);
        }

        public void AddItem(TransactionItem item)
        {
            var items = GetAllItems();
            var existingItem = items.FirstOrDefault(x => x.BookId == item.BookId);
            if (existingItem != null)
            {
                existingItem.Qty += 1;
            }
            else
            {
                items.Add(item);
            }
            Session.SetString("Cart", JsonSerializer.Serialize(items));
        }

        public async Task<List<Bag>> GetAllBagAsync(string Id)
        {
            var bags = await _context.Bags.Include(b => b.Book).Where(b => b.UserId == Id).ToListAsync();
            return bags;
        }

        public async Task<List<Transaction>> GetAllTransaction() {
            var transaction = await _context.Transactions.Include(t => t.User).ToListAsync();

            return transaction;
        }

        public async Task<List<Transaction>> GetAllTransactionByUserId(string Id) {
            return await _context.Transactions.Where(x => x.UserId == Id).Include(transaction => transaction.User).ToListAsync();
        }

        public async Task UpdateTransactionStatus(int Id, string status)
        {
            var existingTransaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == Id);

            existingTransaction.Status = status;

            _context.SaveChangesAsync();
        }

        public async Task CreateTransaction(Transaction transaction)
        {
            if (transaction.Bags != null)
            {
                StringBuilder bookList = new StringBuilder("");

                if (transaction.Bags.Count > 1)
                {
                    foreach (var item in transaction.Bags)
                    {
                        bookList.Append(item.Book.Title);
                        bookList.Append(", ");
                    }
                }
                else
                {
                    bookList.Append(transaction.Bags[0].Book.Title);
                }

                transaction.Book = bookList.ToString();
            }

            _context.Add(transaction);
            await _context.SaveChangesAsync();
            await DeleteItemFromBag(transaction.UserId);
            Session.Remove("Cart");
        }

        public async Task AddBagItem(Bag item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null.");
            }

            // Cari apakah ada item yang sama di keranjang pengguna
            List<Bag> userBag = await _context.Bags
                .Where(b => b.UserId == item.UserId && b.BookId == item.BookId)
                .ToListAsync();

            if (userBag.Count > 0) // Jika sudah ada item, update quantity-nya
            {
                userBag[0].Quantity += 1;  // Menambah quantity
                _context.Bags.Update(userBag[0]);  // Update item yang ada
            }
            else // Jika item belum ada di keranjang, tambahkan item baru
            {
                _context.Bags.Add(item);  // Menambahkan item baru ke context
            }

            // Simpan perubahan ke database
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemFromBag(string Id)
        {
            var userBagItems = await _context.Bags.Where(b => b.UserId == Id).ToListAsync();

            if(userBagItems.Any())
            {
                _context.RemoveRange(userBagItems);

                await _context.SaveChangesAsync();
            }

        }

    }
}
