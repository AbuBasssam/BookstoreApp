using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Role : IdentityRole<int>
{

    #region Vars / Props

    public virtual ICollection<User>? Users { get; private set; }

    #endregion



}








