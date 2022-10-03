using System;
using System.Threading.Tasks;

namespace EagleEye.Contents.Models.PwaOptions
{
    public class OfflinePwaOption : PwaOption
    {
        #region Properties

        #endregion
        
        #region Accessors

        public Func<Task<GetNextPwaVersionResult>> CheckNextUpdateHandler { get; }

        public Func<DownloadPwaContentRequest, Task<byte[]>> DownloadUpdateHandler { get; }

        #endregion
        
        #region Constructor

        public OfflinePwaOption(Func<Task<GetNextPwaVersionResult>> checkNextUpdateAsyncCallback = null, 
            Func<DownloadPwaContentRequest, Task<byte[]>> downloadUpdateAsyncCallback = null)
        {
            CheckNextUpdateHandler = checkNextUpdateAsyncCallback;
            DownloadUpdateHandler = downloadUpdateAsyncCallback;
        }
        
        #endregion
    }
}