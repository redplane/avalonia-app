using System.Collections.Generic;
using System.Collections.Specialized;
using EagleEye.Pwas.Interfaces;

namespace EagleEye.Pwas.Models.PwaOptions
{
    public abstract class PwaOption
    {
        #region Properties

        private readonly LinkedList<INativeMethod> _nativeMethods;

        #endregion
        
        #region Constructor

        protected PwaOption()
        {
            _nativeMethods = new LinkedList<INativeMethod>();
        }

        #endregion
        
        #region Methods

        public virtual PwaOption WithNativeMethod(INativeMethod nativeMethod)
        {
            _nativeMethods.AddLast(nativeMethod);
            return this;
        }

        #endregion
    }
}