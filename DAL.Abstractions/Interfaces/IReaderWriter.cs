using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstractions.Interfaces
{
    public interface IReaderWriter
    {
        Task<T> Read<T>(string source);

        Task Write<T>(string source, T value);
    }
}
