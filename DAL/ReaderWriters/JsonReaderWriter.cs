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
        public async Task<T> Read<T>(string source)
        {
            var data = await File.ReadAllTextAsync(source);
            return JsonConvert.DeserializeObject<T>(data);
        }

        public async Task Write<T>(string source, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            await File.WriteAllTextAsync(source, json);
        }
    }
}
