namespace Domain.Entities;

public class Publisher : IEntity<int>
{
    public int Id { get; set; }
    public required string NameEN { get; set; }
    public required string NameAR { get; set; }
}