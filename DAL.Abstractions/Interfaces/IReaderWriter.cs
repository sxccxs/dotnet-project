using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstractions.Interfaces
{
    public interface IReaderWriter
    {
        T Read<T>(string source);

        void Write<T>(string source, T value);
    }
}
