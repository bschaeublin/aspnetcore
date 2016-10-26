﻿
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SocketsSample
{
    public class LineInvocationAdapter : IInvocationAdapter
    {
        public async Task<InvocationDescriptor> ReadInvocationDescriptor(Stream stream, Func<string, Type[]> getParams)
        {
            var streamReader = new StreamReader(stream);
            var line = await streamReader.ReadLineAsync();
            if (line == null)
            {
                return null;
            }

            var values = line.Split(',');

            var method = values[1].Substring(1);

            return new InvocationDescriptor
            {
                Id = values[0].Substring(2),
                Method = method,
                Arguments = values.Skip(2).Zip(getParams(method), (v, t) => Convert.ChangeType(v, t)).ToArray()
            };
        }

        public async Task WriteInvocationDescriptor(InvocationDescriptor invocationDescriptor, Stream stream)
        {
            var msg = $"CI{invocationDescriptor.Id},M{invocationDescriptor.Method},{string.Join(",", invocationDescriptor.Arguments.Select(a => a.ToString()))}\n";
            await WriteAsync(msg, stream);
        }

        public async Task WriteInvocationResult(InvocationResultDescriptor resultDescriptor, Stream stream)
        {
            if (string.IsNullOrEmpty(resultDescriptor.Error))
            {
                await WriteAsync($"RI{resultDescriptor.Id},E{resultDescriptor.Error}\n", stream);
            }
            else
            {
                await WriteAsync($"RI{resultDescriptor.Id},R{(resultDescriptor.Result != null ? resultDescriptor.Result.ToString() : string.Empty)}\n", stream);
            }
        }

        private async Task WriteAsync(string msg, Stream stream)
        {
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(msg);
            await writer.FlushAsync();
        }
    }
}
