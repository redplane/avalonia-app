using System.Threading;
using System.Threading.Tasks;
using EagleEye.Contents.Models;

namespace EagleEye.Contents.Interfaces
{
    public interface INativeMethod
    {
        #region Methods

        Task<MethodExecutionResult> ExecuteAsync(NativeRequest request, CancellationToken cancellationToken = default);

        #endregion
    }
}