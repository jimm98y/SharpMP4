using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxGenerator2
{
    public abstract class Box
    {
        public abstract string FourCC { get; }

        public async virtual Task ReadAsync(Stream stream)
        {
            throw new NotImplementedException();
        }

        public async virtual Task<ulong> WriteAsync(Stream stream)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class FullBox : Box
    {
        public int version { get; set; }
        public int flags { get; set; }
    }

    public class VisualSampleEntry
    {

    }
}
