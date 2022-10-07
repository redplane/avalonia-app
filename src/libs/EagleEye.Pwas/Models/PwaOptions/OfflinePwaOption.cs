using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EagleEye.Pwas.Models.PwaOptions
{
    public class OfflinePwaOption : PwaOption
    {
        #region Properties

        private readonly string _relativeUrl;

        private readonly Dictionary<string, string> _queryStringParams;

        private string _actualQueryString;

        #endregion

        #region Accessors

        public Func<Task<GetNextPwaVersionResult>> CheckNextUpdateHandler { get; }

        public Func<DownloadPwaContentRequest, Task<byte[]>> DownloadUpdateHandler { get; }

        #endregion

        #region Constructor

        public OfflinePwaOption(Func<Task<GetNextPwaVersionResult>> checkNextUpdateAsyncCallback = null,
            Func<DownloadPwaContentRequest, Task<byte[]>> downloadUpdateAsyncCallback = null,
            string relativeUrl = null)
        {
            CheckNextUpdateHandler = checkNextUpdateAsyncCallback;
            DownloadUpdateHandler = downloadUpdateAsyncCallback;
            _relativeUrl = relativeUrl ?? "";
            _queryStringParams = new Dictionary<string, string>();
        }

        #endregion

        #region Methods

        public virtual OfflinePwaOption WithQueryStringParams(string key, string value)
        {
            _queryStringParams.Add(key, value);

            var keyValuePairs = _queryStringParams.Keys
                .Select(actualKey => $"{actualKey}={_queryStringParams[key]}");

            _actualQueryString = string.Join("&", keyValuePairs);
            return this;
        }

        public string ToQueryString()
        {
            return _actualQueryString;
        }

        #endregion
    }
}