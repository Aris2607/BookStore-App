using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class Error : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
