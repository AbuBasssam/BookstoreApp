namespace Domain.Entities;
public class Language : IEntity<int>
{
    public int Id { get; set; }
    public required string LanguageCode { get; set; }
    public required string NameEN { get; set; }
    public required string NameAR { get; set; }
}
