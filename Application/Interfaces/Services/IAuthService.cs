using Domain.Entities;
using Domain.HelperClasses;

namespace Application.Interfaces;
public interface IAuthService
{
    Task<JwtAuthResult> GetJwtAuthForuser(User User);
}
