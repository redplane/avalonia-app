using System.Collections.Generic;
using System.Linq;

namespace EagleEye.Pwas.Models.PwaOptions
{
    public class OnlinePwaOption : PwaOption
    {
        #region Properties

        private readonly string _endpoint;

        private string _actualEndpoint;
        
        private readonly Dictionary<string, string> _queryStringParams;
        
        #endregion
        
        #region Accessors

        public string Endpoint => _actualEndpoint;
        
        #endregion
        
        #region Constructor

        public OnlinePwaOption() : this("http://localhost:4200")
        {
        }

        public OnlinePwaOption(string endpoint)
        {
            _queryStringParams = new Dictionary<string, string>();
            _endpoint = endpoint;
            _actualEndpoint = endpoint;
        }

        #endregion
        
        #region Methods

        public virtual PwaOption WithQueryStringParams(string key, string value)
        {
            _queryStringParams.Add(key, value);
            
            var keyValuePairs = _queryStringParams.Keys
                .Select(actualKey => $"{actualKey}={_queryStringParams[key]}");

            var queryStrings = string.Join("&", keyValuePairs);
            _actualEndpoint = string.Join("&", _endpoint, queryStrings);
            return this;
        }
        
        #endregion
    }
}