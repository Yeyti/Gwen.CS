using System;
using System.Drawing;
using System.Drawing.Text;
using OpenTK.Graphics;
using OpenTK;
using Bitmap = System.Drawing.Bitmap;

namespace Gwen.Renderer.OpenTK
{
    /// <summary>
    /// Uses System.Drawing for 2d text rendering.
    /// </summary>
    public sealed class TextRenderer : IDisposable
    {
        private readonly Bitmap m_Bitmap;
		private readonly System.Drawing.Graphics m_Graphics;
		private readonly Texture m_Texture;
		private bool m_Disposed;

        public Texture Texture { get { return m_Texture; } }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="width">The width of the backing store in pixels.</param>
        /// <param name="height">The height of the backing store in pixels.</param>
        /// <param name="renderer">GWEN renderer.</param>
        public TextRenderer(int width, int height, OpenTKBase renderer)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");
            if (GraphicsContext.CurrentContext == null)
                throw new InvalidOperationException("No GraphicsContext is current on the calling thread.");

            m_Bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            m_Graphics = System.Drawing.Graphics.FromImage(m_Bitmap);

            // NOTE:    TextRenderingHint.AntiAliasGridFit looks sharper and in most cases better
            //          but it comes with a some problems.
            //
            //          1.  Graphic.MeasureString and format.MeasureCharacterRanges 
            //              seem to return wrong values because of this.
            //
            //          2.  While typing the kerning changes in random places in the sentence.
            // 
            //          Until 1st problem is fixed we should use TextRenderingHint.AntiAlias...  :-(

            m_Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			if (Configuration.RunningOnMono)
				m_Graphics.Clear(System.Drawing.Color.Black);
			else
				m_Graphics.Clear(System.Drawing.Color.Transparent);
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
        public void DrawString(string text, System.Drawing.Font font, Brush brush, Point point, StringFormat format)
        {
			if (Configuration.RunningOnMono)
			{
				// from https://stackoverflow.com/questions/5167937/ugly-looking-text-problem
				m_Graphics.DrawString(text, font, Brushes.White, new System.Drawing.Point(point.X, point.Y), format); // render text on the bitmap
				var lockData = m_Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				unsafe
				{
					// Pointer to the current pixel
					uint* pPixel = (uint*)lockData.Scan0;
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
				m_Bitmap.UnlockBits(lockData);
			}
			else
			{
				m_Graphics.DrawString(text, font, brush, new System.Drawing.Point(point.X, point.Y), format); // render text on the bitmap
			}

			OpenTKBase.LoadTextureInternal(m_Texture, m_Bitmap); // copy bitmap to gl texture
        }

        void Dispose(bool manual)
        {
            if (!m_Disposed)
            {
                if (manual)
                {
                    m_Bitmap.Dispose();
                    m_Graphics.Dispose();
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
