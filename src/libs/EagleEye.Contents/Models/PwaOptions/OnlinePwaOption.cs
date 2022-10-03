namespace EagleEye.Contents.Models.PwaOptions
{
    public class OnlinePwaOption : PwaOption
    {
        #region Accessors
        
        public string Endpoint { get; }
        
        #endregion
        
        #region Constructor

        public OnlinePwaOption(string endpoint)
        {
            Endpoint = endpoint;
        }

        #endregion
    }
}