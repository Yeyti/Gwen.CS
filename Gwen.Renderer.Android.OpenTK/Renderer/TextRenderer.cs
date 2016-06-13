using System;
using Android.Graphics;

namespace Gwen.Renderer.Android.OpenTK
{
    /// <summary>
    /// Uses System.Drawing for 2d text rendering.
    /// </summary>
    public sealed class TextRenderer : IDisposable
    {
        private readonly Bitmap m_Bitmap;
		private readonly Canvas m_Canvas;
		private readonly Texture m_Texture;
		private bool m_Disposed;

        public Texture Texture { get { return m_Texture; } }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="width">The width of the backing store in pixels.</param>
        /// <param name="height">The height of the backing store in pixels.</param>
        /// <param name="renderer">GWEN renderer.</param>
        public TextRenderer(int width, int height, OpenTK renderer)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            m_Bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
			m_Bitmap.EraseColor(0);
            m_Canvas = new Canvas(m_Bitmap);
			//m_Canvas.DrawColor(global::Android.Graphics.Color.Transparent);
			m_Texture = new Texture(renderer) { Width = width, Height = height };
        }

        /// <summary>
        /// Draws the specified string to the backing store.
        /// </summary>
        /// <param name="text">The <see cref="System.String"/> to draw.</param>
        /// <param name="font">The <see cref="System.Drawing.Font"/> that will be used.</param>
        /// <param name="brush">The <see cref="System.Drawing.Brush"/> that will be used.</param>
        /// <param name="point">The location of the text on the backing store, in 2d pixel coordinates.
        /// The origin (0, 0) lies at the top-left corner of the backing store.</param>
        public void DrawString(string text, Paint paint, Point point)
        {
			m_Canvas.DrawText(text, point.X, point.Y - paint.GetFontMetricsInt().Ascent, paint); // render text on the bitmap

			// Fix ugly looking anti-alias
			IntPtr data = m_Bitmap.LockPixels();
			unsafe
			{
				// Pointer to the current pixel
				uint* pPixel = (uint*)data;
				// Pointer value at which we terminate the loop (end of pixel data)
				var pLastPixel = pPixel + m_Bitmap.Width * m_Bitmap.Height;
				uint pixelValue, brightness;

				while (pPixel < pLastPixel)
				{
					// Get pixel data
					pixelValue = *pPixel;
					// Average RGB
					brightness = (((pixelValue & 0xff) + ((pixelValue >> 8) & 0xff) + ((pixelValue >> 16) & 0xff)) * 21845) >> 16; // Division by 3

					// Use brightness for alpha value, set R, G, and B 0xff (white)
					pixelValue = brightness << 24 | 0xffffff;

					// Copy back to image
					*pPixel = pixelValue;
					// Next pixel
					pPixel++;
				}
			}
			m_Bitmap.UnlockPixels();

			OpenTK.LoadTextureInternal(m_Texture, m_Bitmap); // copy bitmap to gl texture
        }

        void Dispose(bool manual)
        {
            if (!m_Disposed)
            {
                if (manual)
                {
                    m_Bitmap.Dispose();
                    m_Canvas.Dispose();
                    m_Texture.Dispose();
                }

                m_Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#if DEBUG
		~TextRenderer()
        {
			throw new InvalidOperationException(String.Format("[Warning] Resource leaked: {0}", typeof(TextRenderer)));
        }
#endif
	}
}
