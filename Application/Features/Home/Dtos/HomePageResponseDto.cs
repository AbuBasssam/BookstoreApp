namespace Application.Features.Home;
public record HomePageResponseDto(List<CategoryDto> Categories, List<HomePageBookDto> Books);
