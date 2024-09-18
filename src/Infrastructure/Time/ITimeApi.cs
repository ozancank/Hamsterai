using OCK.Core.Interfaces;

namespace Infrastructure.Time;

public interface ITimeApi : IExternalApi
{
    Task<DateTime> GetTime();
}