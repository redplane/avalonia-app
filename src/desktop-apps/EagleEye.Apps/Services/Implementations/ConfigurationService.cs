using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EagleEye.Apps.Models;
using EagleEye.Apps.Services.Abstractions;
using Newtonsoft.Json;

namespace EagleEye.Apps.Services.Implementations;

public class ConfigurationService : IConfigurationService
{
    #region Properties

    private readonly string _configurationFilePath;
    
    #endregion
    
    #region Constructor

    public ConfigurationService()
    {
        var directory = Directory.GetCurrentDirectory();
        _configurationFilePath = Path.Combine(directory, "wwwroot", "configuration.json");
    }

    #endregion
    
    #region Methods
    
    public virtual async Task<ApplicationConfiguration> GetAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_configurationFilePath))
            return new ApplicationConfiguration();

        var szContent = File.ReadAllText(_configurationFilePath);
        var instance = JsonConvert
            .DeserializeObject<ApplicationConfiguration>(szContent) ?? new ApplicationConfiguration();
        return await Task.FromResult(instance);
    }

    public virtual async Task UpdateAsync(Func<ApplicationConfiguration, ApplicationConfiguration> updater, CancellationToken cancellationToken = default)
    {
        var originalConfiguration = await GetAsync(cancellationToken);

        var updatedConfiguration = updater(originalConfiguration);
        if (updatedConfiguration == null)
            updatedConfiguration = new ApplicationConfiguration();

        var szConfiguration = JsonConvert.SerializeObject(updatedConfiguration);
        File.WriteAllText(_configurationFilePath, szConfiguration);
    }

    #endregion
}