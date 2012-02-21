using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Fluid.Drawing.GdiPlus
{
    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    internal struct HandleRef
    {
        internal object wrapper;
        internal IntPtr handle;
        public HandleRef(object wrapper, IntPtr handle)
        {
            this.wrapper = wrapper;
            this.handle = handle;
        }

        public object Wrapper
        {
            get { return wrapper; }
        }

        public IntPtr Handle
        {
            get {  return handle; }
        }

        public static explicit operator IntPtr(HandleRef value)
        {
            return value.handle;
        }

        public static IntPtr ToIntPtr(HandleRef value)
        {
            return value.handle;
        }
    }
}
