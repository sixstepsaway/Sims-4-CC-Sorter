/*

    Code taken and adjusted where applicable from S4PI.

*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimsCCManager.App.Images
{
    public class ImageTransformations
    {
        public Bitmap TransformToPNG(byte[] rawData)
        {
            using (MemoryStream ms = new MemoryStream(rawData))
            {
                BinaryReader r = new BinaryReader(ms);
                Bitmap colorImage;
                ms.Position = 0;
                r.ReadBytes(24);
                if (r.ReadUInt32() == 0x41464C41U)
                {
                    int length = r.ReadInt32();
                    length = (int)((length & 0xFF000000) >> 24) | (int)((length & 0x00FF0000) >> 8) | (int)((length & 0x0000FF00) << 8) | (int)((length & 0x000000FF) << 24);
                    using (MemoryStream alphaStream = new MemoryStream(r.ReadBytes(length)))
                    {
                        Bitmap alphaImage = new Bitmap(alphaStream);
                        colorImage = new Bitmap(ms);
                        if (colorImage.Width != alphaImage.Width || colorImage.Height != alphaImage.Height) throw new InvalidDataException("Not a proper TS4 Thumbnail image");
                        colorImage = UpdateAlpha(colorImage, alphaImage);

                        Bitmap bitp = colorImage;
                        return bitp;
                    }
                }
                ms.Position = 0;
                colorImage = new Bitmap(ms);
                Bitmap bp = colorImage;
                return bp;
            }
        }

        protected internal unsafe Bitmap UpdateAlpha(Bitmap source, Bitmap alpha)
        {
            int width = source.Width;
            int height = source.Height;
            Bitmap img = new Bitmap(width, height);

            BitmapData imgBitmapData = img.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            BitmapData sourceBitmapData = source.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            BitmapData alphaBitmapData = alpha.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            ColorARGB* imgStartingPosition = (ColorARGB*)imgBitmapData.Scan0;
            ColorARGB* sourceStartingPosition = (ColorARGB*)sourceBitmapData.Scan0;
            ColorARGB* alphaStartingPosition = (ColorARGB*)alphaBitmapData.Scan0;

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    ColorARGB* sourcePosition = sourceStartingPosition + j + i * width;
                    ColorARGB* alphaPosition = alphaStartingPosition + j + i * width;
                    ColorARGB* imgPosition = imgStartingPosition + j + i * width;
                    *imgPosition = new ColorARGB(sourcePosition, alphaPosition);
                }

            img.UnlockBits(imgBitmapData);
            source.UnlockBits(sourceBitmapData);
            alpha.UnlockBits(alphaBitmapData);
            return img;
        }        
        
        protected internal struct ColorARGB
        {
            public byte B;
            public byte G;
            public byte R;
            public byte A;
            public ColorARGB(Color color)
            {
                A = color.A;
                R = color.R;
                G = color.G;
                B = color.B;
            }

            public ColorARGB(ColorARGB color)
            {
                A = color.A;
                R = color.R;
                G = color.G;
                B = color.B;
            }

            public unsafe ColorARGB(ColorARGB* original, ColorARGB* alpha)
            {
                this.A = alpha->R;
                this.R = original->R;
                this.G = original->G;
                this.B = original->B;
            }

            public unsafe ColorARGB(ColorARGB* original)
            {
                this.A = 255;
                this.R = original->R;
                this.G = original->G;
                this.B = original->B;
            }

            public unsafe void UpdateAlha(ColorARGB* alpha)
            {
                this.A = alpha->R;
            }

            public ColorARGB(byte a, byte r, byte g, byte b)
            {
                A = a;
                R = r;
                G = g;
                B = b;
            }

            public ColorARGB(int color)
            {
                A = (byte)(color >> 24);
                R = (byte)((color & 0xFF0000) >> 16);
                G = (byte)((color & 0xFF00) >> 8);
                B = (byte)((color & 0XFF));
            }

            public Color ToColor()
            {
                return Color.FromArgb(A, R, G, B);
            }


        }
    }

    
}