﻿using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;
public class User : IdentityUser<int>
{
    #region var/prop
    public int RoleID { get; set; }

    // Navigation properties
    public virtual Role Role { get; set; }

    public virtual ICollection<UserRefreshToken>? RefreshTokens { get; }
    public virtual ICollection<UserDevice>? Devices { get; }
    public virtual ICollection<Notification>? Notifications { get; }

    #endregion



}








