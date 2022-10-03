using EagleEye.Contents.Enums;

namespace EagleEye.Contents.Models
{
    public class MethodExecutionResult
    {
        #region Properties

        public ExecutionResults Result { get; private set; }

        public object Payload { get; private set; }

        #endregion
        
        #region Constructor

        public MethodExecutionResult(ExecutionResults result)
        {
            Result = result;
        }

        public MethodExecutionResult(object payload)
        {
            Result = ExecutionResults.Successful;
            Payload = payload;
        }

        #endregion
    }
}