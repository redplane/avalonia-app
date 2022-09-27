using System;
using System.Threading.Tasks;

namespace EagleEye.Apps.Models;

public class PlatformFeature
{
    #region Methods

    public void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }

    public Task<int> GetPlatformAsync()
    {
        return Task.FromResult(1);
    }
    
    #endregion
}