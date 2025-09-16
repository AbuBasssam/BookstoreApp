namespace Application.Features.Home;
public record HomePageResponseDto(int notificationCount, List<CategoryDto> Categories, List<HomePageBookDto> Books);
