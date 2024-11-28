namespace BookStoreApp.Models
{
    public abstract class BaseModel
    {
        public BaseModel()
        {
            createdBy = "System";
            createdTime = DateTime.Now;
        }
        public string createdBy { get; set; }
        public DateTime? createdTime { get; set; }
    }
}
