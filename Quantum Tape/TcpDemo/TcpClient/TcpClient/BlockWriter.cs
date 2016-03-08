using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TcpClient
{
    class BlockWriter
    {
        public static byte[] GetBytes(string message)
        {
            MemoryStream output = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(message);
            writer.Write(Environment.NewLine);
            writer.Close();
            output.Close();
            var data = output.ToArray();
            return data;
        }
    }
}
