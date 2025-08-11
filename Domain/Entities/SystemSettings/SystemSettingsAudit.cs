namespace Domain.Entities;

public class SystemSettingsAudit:IEntity<int>
{
    public int Id { get; set; }
    public string SettingName { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public int ChangedBy { get; set; }
    public DateTime ChangeDate { get; set; }

    public virtual User User { get; set; }
}