using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Abstractions.Interfaces;
using Newtonsoft.Json;

namespace DAL.ReaderWriters
{
    internal class JsonReaderWriter : IReaderWriter
    {
        public T Read<T>(string source)
        {
            var data = File.ReadAllText(source);
            return JsonConvert.DeserializeObject<T>(data);
        }

        public void Write<T>(string source, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            File.WriteAllText(source, json);
        }
    }
}
