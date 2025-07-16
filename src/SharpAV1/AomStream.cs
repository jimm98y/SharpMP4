using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpAV1
{
    public class AomStream : IDisposable
    {
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AomStream()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ulong ReadLeb128(out uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteLeb128(uint size, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadFixed(int length, out uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadFixed(int length, out int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteFixed(int length, uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteFixed(int length, int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadVariable(long length, out int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteVariable(long length, int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadUvlc(out int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteUvlc(int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadSignedIntVar(int length, out int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteSignedIntVar(int length, int value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadUnsignedInt(uint length, out uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadUnsignedInt(int length, out uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteUnsignedInt(int length, uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong ReadLeVar(int length, out uint value, string name)
        {
            throw new NotImplementedException();
        }

        public ulong WriteLeVar(int length, uint value, string name)
        {
            throw new NotImplementedException();
        }

        public int GetPosition()
        {
            throw new NotImplementedException();
        }

        internal static int Clip3(int v, int limit, int feature_value)
        {
            throw new NotImplementedException();
        }

        internal static int GetQIndex(int v, uint segmentId)
        {
            throw new NotImplementedException();
        }
    }
}
