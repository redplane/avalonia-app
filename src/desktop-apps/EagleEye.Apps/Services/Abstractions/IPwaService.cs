using System.Threading;
using System.Threading.Tasks;
using EagleEye.Apps.Models.Pwas;
using EagleEye.Pwas.Models;

namespace EagleEye.Apps.Services.Abstractions;

public interface IPwaService
{
    #region Methods

    Task<GetNextPwaVersionResult?> GetVersionAsync(GetNextPwaVersionRequest request, CancellationToken cancellationToken = default);

    Task<byte[]> DownloadAsync(string downloadUrl, CancellationToken cancellationToken = default);

    #endregion
}