using BookStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace BookStoreApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly IElasticClient _elasticClient;

        public SearchController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public IActionResult Index() => View();

        // Elasticsearch 
        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("Index", new List<Book>());
            }

            // Search in Elasticsearch
            var searchResponse = await _elasticClient.SearchAsync<Book>(s => s
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(b => b.Title)
                            .Field(b => b.Author)
                            .Field(b => b.Summary))
                        .Query(query)
                    )
                )
            );

            // Pass results to the view
            var books = searchResponse.Documents.ToList();
            return View("Index", books);
        }
    }
}
