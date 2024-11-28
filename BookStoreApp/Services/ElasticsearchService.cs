using BookStoreApp.Data;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace BookStoreApp.Services
{
    public class ElasticsearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly BookStoreAppContext _context;
        
        public ElasticsearchService(IElasticClient elasticClient, BookStoreAppContext context) {
            _elasticClient = elasticClient;
            _context = context;
        }

        public async Task IndexBooksAsync()
        {
            var books = await _context.Books.ToListAsync();

            foreach (var book in books)
            {
                await _elasticClient.IndexDocumentAsync(book);
            }
        }
    }
}
