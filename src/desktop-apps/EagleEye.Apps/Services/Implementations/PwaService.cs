using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EagleEye.Apps.Models.Pwas;
using EagleEye.Apps.Services.Abstractions;
using EagleEye.Pwas.Models;
using Newtonsoft.Json;

namespace EagleEye.Apps.Services.Implementations;

public class PwaService : IPwaService
{
    #region Properties

    private readonly string _apiUrl = "http://demo-web-content.azurewebsites.net";
    
    #endregion
    
    #region Methods

    public virtual async Task<GetNextPwaVersionResult?> GetVersionAsync(GetNextPwaVersionRequest request, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_apiUrl, UriKind.Absolute);
        
        var queryParams = new NameValueCollection();
        if (!string.IsNullOrWhiteSpace(request.Version))
            queryParams.Add(nameof(request.Version), request.Version);
        queryParams.Add(nameof(request.Environment), request.Environment);
        var szPath = $"api/web-content/next-update?" + string.Join("&", queryParams.AllKeys
            .Select(key => $"{key}={queryParams[key]}"));

        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        var httpResponse = await httpClient.GetAsync(new Uri(szPath, UriKind.Relative), cancellationToken);

        if (!httpResponse.IsSuccessStatusCode)
            return null;

        var szContent = await httpResponse.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GetNextPwaVersionResult>(szContent)!;
    }

    public virtual async Task<byte[]> DownloadAsync(string downloadUrl, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();
        var httpResponseMessage = await httpClient.GetAsync(downloadUrl, cancellationToken);
        var content = await httpResponseMessage.Content.ReadAsByteArrayAsync();
        return content;
    }
    
    #endregion

    
}