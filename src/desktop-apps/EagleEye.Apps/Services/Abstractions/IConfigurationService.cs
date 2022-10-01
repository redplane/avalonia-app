using System;
using System.Threading;
using System.Threading.Tasks;
using EagleEye.Apps.Models;

namespace EagleEye.Apps.Services.Abstractions;

public interface IConfigurationService
{
    #region Methods

    Task<ApplicationConfiguration> GetAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(Func<ApplicationConfiguration, ApplicationConfiguration> updater,
        CancellationToken cancellationToken = default);

    #endregion
}