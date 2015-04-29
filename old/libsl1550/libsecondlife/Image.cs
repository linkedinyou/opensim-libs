using System;

namespace libsecondlife
{
    [Flags]
    public enum ImageChannels
    {
        Color = 1,
        Bump = 2,
        Alpha = 4,
    };

    public enum ImageResizeAlgorithm
    {
        NearestNeighbor
    }

    public class Image
    {
        /// <summary>
        /// Image width
        /// </summary>
        public int Width;

        /// <summary>
        /// Image height
        /// </summary>
        public int Height;
        
        /// <summary>
        /// Image channel flags
        /// </summary>
        public ImageChannels Channels;

        /// <summary>
        /// Red channel data
        /// </summary>
        public byte[] Red;
        
        /// <summary>
        /// Green channel data
        /// </summary>
        public byte[] Green;
        
        /// <summary>
        /// Blue channel data
        /// </summary>
        public byte[] Blue;

        /// <summary>
        /// Alpha channel data
        /// </summary>
        public byte[] Alpha;
        
        /// <summary>
        /// Bump channel data
        /// </summary>
        public byte[] Bump;

        /// <summary>
        /// Create a new blank image
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <param name="channels">channel flags</param>
        public Image(int width, int height, ImageChannels channels)
        {
            Width = width;
            Height = height;
            Channels = channels;

            int n = width * height;

            if ((channels & ImageChannels.Color) != 0)
            {
                Red = new byte[n];
                Green = new byte[n];
                Blue = new byte[n];
            }

            if ((channels & ImageChannels.Bump) != 0) Bump = new byte[n];

            if ((channels & ImageChannels.Alpha) != 0)
                Alpha = new byte[n];
        }

        /// <summary>
        /// Convert the channels in the image. Channels are created or destroyed as required.
        /// </summary>
        /// <param name="channels">new channel flags</param>
        public void ConvertChannels(ImageChannels channels)
        {
            if (Channels == channels)
                return;

            int n = Width * Height;
            ImageChannels add = Channels ^ channels & channels;
            ImageChannels del = Channels ^ channels & Channels;

            if ((add & ImageChannels.Color) != 0)
            {
                Red = new byte[n];
                Green = new byte[n];
                Blue = new byte[n];
            }
            else if ((del & ImageChannels.Color) != 0)
            {
                Red = null;
                Green = null;
                Blue = null;
            }

            if ((add & ImageChannels.Alpha) != 0)
            {
                Alpha = new byte[n];
                Fill(Alpha, 255);
            }
            else if ((del & ImageChannels.Alpha) != 0)
                Alpha = null;

            if ((add & ImageChannels.Bump) != 0)
                Bump = new byte[n];
            else if ((del & ImageChannels.Bump) != 0)
                Bump = null;

            Channels = channels;
        }

        /// <summary>
        /// Resize or stretch the image using nearest neighbor (ugly) resampling
        /// </summary>
        /// <param name="width">new width</param>
        /// <param name="height">new height</param>
        public void ResizeNearestNeighbor(int width, int height)
        {
            if (width == Width && height == Height)
                return;

            byte[]
                red = null, 
                green = null, 
                blue = null, 
                alpha = null, 
                bump = null;
            int n = width * height;
            int di = 0, si;

            if (Red != null) red = new byte[n];
            if (Green != null) green = new byte[n];
            if (Blue != null) blue = new byte[n];
            if (Alpha != null) alpha = new byte[n];
            if (Bump != null) bump = new byte[n];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    si = (y * Height / height) * Width + (x * Width / width);
                    if (Red != null) red[di] = Red[si];
                    if (Green != null) green[di] = Green[si];
                    if (Blue != null) blue[di] = Blue[si];
                    if (Alpha != null) alpha[di] = Alpha[si];
                    if (Bump != null) bump[di] = Bump[si];
                    di++;
                }
            }

            Width = width;
            Height = height;
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
            Bump = bump;
        }

        public byte[] ExportTGA()
        {
            byte[] tga = new byte[Width * Height * 4 + 32];
            int di = 0;
            tga[di++] = 0; // idlength
            tga[di++] = 0; // colormaptype = 0: no colormap
            tga[di++] = 2; // image type = 2: uncompressed RGB
            tga[di++] = 0; // color map spec is five zeroes for no color map
            tga[di++] = 0; // color map spec is five zeroes for no color map
            tga[di++] = 0; // color map spec is five zeroes for no color map
            tga[di++] = 0; // color map spec is five zeroes for no color map
            tga[di++] = 0; // color map spec is five zeroes for no color map
            tga[di++] = 0; // x origin = two bytes
            tga[di++] = 0; // x origin = two bytes
            tga[di++] = 0; // y origin = two bytes
            tga[di++] = 0; // y origin = two bytes
            tga[di++] = (byte)(Width & 0xFF); // width - low byte
            tga[di++] = (byte)(Width >> 8); // width - hi byte
            tga[di++] = (byte)(Height & 0xFF); // height - low byte
            tga[di++] = (byte)(Height >> 8); // height - hi byte
            tga[di++] = (byte)((Channels & ImageChannels.Alpha) == 0 ? 24 : 32); // 24/32 bits per pixel
            tga[di++] = (byte)((Channels & ImageChannels.Alpha) == 0 ? 32 : 40); // image descriptor byte

            int n = Width * Height;

            if ((Channels & ImageChannels.Alpha) != 0)
            {
                if ((Channels & ImageChannels.Color) != 0)
                {
                    for (int i = 0; i < n; i++)
                    {
                        tga[di++] = Blue[i];
                        tga[di++] = Green[i];
                        tga[di++] = Red[i];
                        tga[di++] = Alpha[i];
                    }
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        tga[di++] = Alpha[i];
                        tga[di++] = Alpha[i];
                        tga[di++] = Alpha[i];
                        tga[di++] = Alpha[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    tga[di++] = Blue[i];
                    tga[di++] = Green[i];
                    tga[di++] = Red[i];
                }
            }

            return tga;
        }

        private void Fill(byte[] array, byte value)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                    array[i] = value;
            }
        }

        public void Clear()
        {
            Fill(Red, 0);
            Fill(Green, 0);
            Fill(Blue, 0);
            Fill(Alpha, 0);
            Fill(Bump, 0);
        }

        public Image Clone()
        {
            Image image = new Image(Width, Height, Channels);
            if (Red != null) image.Red = (byte[])Red.Clone();
            if (Green != null) image.Green = (byte[])Green.Clone();
            if (Blue != null) image.Blue = (byte[])Blue.Clone();
            if (Alpha != null) image.Alpha = (byte[])Alpha.Clone();
            if (Bump != null) image.Bump = (byte[])Bump.Clone();
            return image;
        }
    }
}
