namespace EasyUiBackend.Domain.Models.UIComponent;

public class FilterUIComponentRequest
{
    public string? Framework { get; set; }
    public string? Type { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public List<Guid>? TagIds { get; set; }
    
    // Tùy chọn sắp xếp: 
    // - price_asc: Giá tăng dần
    // - price_desc: Giá giảm dần
    // - created_at_desc: Mới nhất
    // - likes_desc: Nhiều lượt thích nhất
    // - views_desc: Nhiều lượt xem nhất
    // - popular: Phổ biến nhất (kết hợp lượt xem và lượt thích)
    public string? SortBy { get; set; }
    
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 