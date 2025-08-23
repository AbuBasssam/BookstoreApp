namespace Domain.Entities;

public class Publisher : IEntity<int>
{
    public int Id { get; set; }
    public string NameEN { get; set; }
    public string NameAR { get; set; }
}