using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using CoreText;

namespace Gwen.Renderer.iOS.OpenTK
{
    /// <summary>
    /// Uses System.Drawing for 2d text rendering.
    /// </summary>
    public sealed class TextRenderer : IDisposable
    {
		private readonly Texture m_Texture;

		private CGBitmapContext m_Bitmap;
		private IntPtr m_PixelData;

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

			m_PixelData = Marshal.AllocHGlobal(width * height * 4);
			m_Bitmap = new CGBitmapContext(m_PixelData, (nint)width, (nint)height, 8, 4 * (nint)width, CGColorSpace.CreateDeviceRGB(), CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);

			m_Bitmap.SetTextDrawingMode(CGTextDrawingMode.Fill);
			m_Bitmap.SetFillColor(new CGColor(1.0f, 1.0f, 1.0f, 1.0f));
			m_Bitmap.ClearRect(new CGRect(0, 0, (nint)width, (nint)height));

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
		public void DrawString(CTLine text, Point point)
        {
			if (m_Bitmap != null)
			{
				m_Bitmap.TextPosition = new CGPoint(point.X, point.Y);
				text.Draw(m_Bitmap);

				// Fix ugly looking anti-alias
				unsafe
				{
					// Pointer to the current pixel
					uint* pPixel = (uint*)m_PixelData;
					// Pointer value at which we terminate the loop (end of pixel data)
					var pLastPixel = pPixel + (int)m_Bitmap.Width * (int)m_Bitmap.Height;

					while (pPixel < pLastPixel)
					{
						// Get pixel data
						uint pixelValue = *pPixel;
						// Average RGB
						uint brightness = (((pixelValue & 0xFF) + ((pixelValue >> 8) & 0xFF) + ((pixelValue >> 16) & 0xFF)) * 21845) >> 16; // Division by 3

						// Use brightness for alpha value, set R, G, and B 0xff (white)
						pixelValue = brightness << 24 | 0xffffff;

						// Copy back to image
						*pPixel = pixelValue;
						// Next pixel
						pPixel++;
					}
				}

				OpenTK.LoadTextureInternal(m_Texture, m_Bitmap); // copy bitmap to gl texture

				// Freeze texture to save memory
				// Todo: Make optional
				m_Bitmap.Dispose();
				m_Bitmap = null;
				Marshal.FreeHGlobal(m_PixelData);
				m_PixelData = IntPtr.Zero;
			}
        }

        void Dispose(bool manual)
        {
            if (!m_Disposed)
            {
                if (manual)
                {
					if (m_Bitmap != null)
                    	m_Bitmap.Dispose();
					if (m_PixelData != IntPtr.Zero)
						Marshal.FreeHGlobal(m_PixelData);
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
