namespace EasyUiBackend.Domain.Models.UIComponent;

public class FilterUIComponentRequest
{
    public string? Framework { get; set; }
    public string? Type { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public List<Guid>? TagIds { get; set; }
    public string? SortBy { get; set; } // price_asc, price_desc, created_at_desc
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 