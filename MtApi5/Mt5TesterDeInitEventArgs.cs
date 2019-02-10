using System;

namespace MtApi5
{
    public class Mt5TesterDeInitEventArgs : EventArgs
    {
        internal Mt5TesterDeInitEventArgs(int deinitCode)

        {
            DeInitCode = deinitCode;
        }

        public int DeInitCode { get; }
    }
}
