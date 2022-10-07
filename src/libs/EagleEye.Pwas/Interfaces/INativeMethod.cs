using System.Threading;
using System.Threading.Tasks;
using EagleEye.Pwas.Models;

namespace EagleEye.Pwas.Interfaces
{
    public interface INativeMethod
    {
        #region Methods

        Task<MethodExecutionResult> ExecuteAsync(NativeRequest request, string rawData = default, CancellationToken cancellationToken = default);

        #endregion
    }
}