using Avalonia.Web.Blazor;

namespace EagleEye.Apps.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<EagleEye.Apps.App>()
            .SetupWithSingleViewLifetime();
    }
}