
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace DigitalDash.Core.Classes
{
    public static class Utilities
    {
        public static bool IsGoodDivision(int a, int b)
        {
            return a % b == 0;
        }

        public static bool IsNullOrEmpty(this string inputString)
        {
            return string.IsNullOrEmpty(inputString);
        }

        public static T Cast<T>(T typeHolder, Object x)
        {
            // typeHolder above is just for compiler magic
            // to infer the type to cast x to
            return (T)x;
        }

        public static string Trim(this string inputString, int length)
        {
            if (inputString.Length > length)
            {
                return inputString.Substring(0, length-2) + "...";
            }

            return inputString;
        }

        public static Stream RotateStream(Stream stream, int angle)
        {
            stream.Position = 0;
            if (angle%90 != 0 || angle < 0) throw new ArgumentException();
            if (angle%360 == 0) return stream;

            var bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            var wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            if (angle%180 == 0)
            {
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            }
            else
            {
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
            }

            for (int x = 0; x < wbSource.PixelWidth; x++)
            {
                for (int y = 0; y < wbSource.PixelHeight; y++)
                {
                    switch (angle%360)
                    {
                        case 90:
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x*wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y*wbSource.PixelWidth];
                            break;
                        case 180:
                            wbTarget.Pixels[
                                (wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1)*wbSource.PixelWidth] =
                                wbSource.Pixels[x + y*wbSource.PixelWidth];
                            break;
                        case 270:
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1)*wbTarget.PixelWidth] =
                                wbSource.Pixels[x + y*wbSource.PixelWidth];
                            break;
                    }
                }
            }
            var targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
            return targetStream;
        }

        public static string FormatBytes(this long inputBytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };

            int order = 0;
            while (inputBytes >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                inputBytes = inputBytes / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return string.Format("{0:#,###,##0.##} {1}", inputBytes, sizes[order]);
        }

    }
}
