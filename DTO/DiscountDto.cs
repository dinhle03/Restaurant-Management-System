public class DiscountDto
{
    public string DiscountId { get; set; }
    public string DiscountName { get; set; }
    public string DiscountCategory { get; set; }
    public double DiscountPrice { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public bool DiscountStatus { get; set; }
    public int RequiredPoints { get; set; } // ✅ thêm dòng này

}
