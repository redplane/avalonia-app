using System;

namespace EagleEye.Contents.Models
{
    public class NativeRequest
    {
        #region Properties
        
        public string Id { get; private set; }
        
        public string Namespace { get; set; }

        public string Method { get; set; }
        
        #endregion
        
        #region Constructor

        public NativeRequest(string id)
        {
            Id = id;
        }
        
        #endregion
    }
    
    public class NativeRequest<T> : NativeRequest
    {
        #region Properties
        
        public T Data { get; set; }

        #endregion
        
        #region Constructor

        public NativeRequest(string id) : base(id)
        {
        }

        #endregion
    }
}