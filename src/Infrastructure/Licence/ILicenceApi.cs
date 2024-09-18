using OCK.Core.Interfaces;

namespace Infrastructure.Licence;

public interface ILicenceApi : IExternalApi
{
    Task<object> GetModule(string taxNumber);
}