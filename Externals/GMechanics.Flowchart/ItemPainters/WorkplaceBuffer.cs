using System;
using System.Drawing;

namespace GMechanics.FlowchartControl.ItemPainters
{
    internal class WorkplaceBuffer : IDisposable
    {
        public Graphics Buffer;
        public Bitmap BufferBitmap;

        public void SetBuffer(int width, int height)
        {
            if (BufferBitmap == null || BufferBitmap.Width < width || BufferBitmap.Height < height)
            {
                Dispose();
                BufferBitmap = new Bitmap(width, height);
                Buffer = Graphics.FromImage(BufferBitmap);
            }
        }

        public void Dispose()
        {
            if (Buffer != null)
            {
                Buffer.Dispose();
                Buffer = null;
            }
            if (BufferBitmap != null)
            {
                BufferBitmap.Dispose();
                BufferBitmap = null;
            }
        }

        ~WorkplaceBuffer()
        {
            Dispose();
        }
    }
}
