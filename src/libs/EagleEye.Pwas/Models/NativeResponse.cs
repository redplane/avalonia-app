namespace EagleEye.Pwas.Models
{
    public class NativeResponse
    {
        #region Properties

        public string Id { get; private set; }
        
        public string Namespace { get; set; }
        
        public string Method { get; set; }

        #endregion
        
        #region Constructor

        public NativeResponse(string id)
        {
            Id = id;
        }

        #endregion
    }

    public class NativeResponse<T> : NativeResponse
    {
        #region Properties

        public T Data { get; set; }
        
        #endregion
        
        #region Constructor

        public NativeResponse(string id): base(id)
        {
        }

        #endregion
    }
}