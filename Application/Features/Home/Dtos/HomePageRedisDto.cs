using Domain.Entities;

namespace Application.Features.Home;

internal class HomePageRedisDto
{
    public List<Category> CategoriesData { get; set; }
    public List<BookRedisDto> PageData { get; set; }
    public HomePageMetaDto MetaData { get; set; }
}