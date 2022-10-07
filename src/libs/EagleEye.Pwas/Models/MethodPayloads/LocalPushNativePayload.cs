using System;

namespace EagleEye.Pwas.Models.MethodPayloads
{
    public class LocalPushNativePayload
    {
        #region Properties

        public DateTime ShowTime { get; set; }
        
        public string Title { get; set; }
        
        public string Body { get; set; }
        
        #endregion
    }
}