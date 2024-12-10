namespace ProductsAPI.Dto
 {
    public class ProductDto 
    {
        public int id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
    }
 }