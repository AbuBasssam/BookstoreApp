namespace Domain.Entities;
public class Language : IEntity<int>
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string NameEN { get; set; }
    public string NameAR { get; set; }
}
