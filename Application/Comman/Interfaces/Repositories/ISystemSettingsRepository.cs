using Domain.Entities;

namespace Application.Interfaces;

public interface ISystemSettingsRepository
{
    Task<SystemSettings> GetSettingsAsync();
}