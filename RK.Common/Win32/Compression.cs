using System.IO;
using System.IO.Compression;

namespace RK.Common.Win32
{
    public static class Compression
    {
        public static byte[] Compress(byte[] data, int startPos)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionLevel.Fastest))
                {
                    ds.Write(data, startPos, data.Length - startPos);
                }
                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data, int startPos)
        {
            using (MemoryStream os = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data, startPos, data.Length - startPos))
                {
                    using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        ds.CopyTo(os);
                    }
                }
                return os.ToArray();
            }
        }
    }
}
