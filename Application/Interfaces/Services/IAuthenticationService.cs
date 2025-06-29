using Domain.Entities;
using Domain.HelperClasses;

namespace Application.Interfaces;
public interface IAuthenticationService
{
    Task<JwtAuthResult> GetJwtAuthForuser(User User);
}
