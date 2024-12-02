using BookStoreApp.Data;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace BookStoreApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly IElasticClient _elasticClient;
        private readonly BookStoreAppContext _context;

        public SearchController(IElasticClient elasticClient, BookStoreAppContext context)
        {
            _elasticClient = elasticClient;
            _context = context;
        }

        public IActionResult Index() => View();

        // Elasticsearch 
        //[HttpPost]
        //public async Task<IActionResult> Search(string query)
        //{
        //    if (string.IsNullOrEmpty(query))
        //    {
        //        return View("Index", new List<Book>());
        //    }

        //    // Search in Elasticsearch
        //    var searchResponse = await _elasticClient.SearchAsync<Book>(s => s
        //        .Query(q => q
        //            .MultiMatch(m => m
        //                .Fields(f => f
        //                    .Field(b => b.Title)
        //                    .Field(b => b.Author)
        //                    .Field(b => b.Summary))
        //                .Query(query)
        //            )
        //        )
        //    );

        //    // Pass results to the view
        //    var books = searchResponse.Documents.ToList();
        //    return View("Index", books);
        //}

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("Index", new List<Book>());
            }

            var searchResponse = await _context.Books.Where(b => b.Title.Contains(query) ||
                        b.Author.Contains(query) ||
                        b.Summary.Contains(query))
            .ToListAsync();

            return View("Index", searchResponse);
        }
    }
}
