using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace JpegSync
{
    public class JpegInfo
    {
        private string filePath;

        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
                try
                {
                    size = (new FileInfo(filePath)).Length;
                }
                catch (Exception)
                {
                }
            }
        }

        public string FileName
        {
            get
            {
                return Path.GetFileName(filePath);
            }
        }

        public long size { get; set; }
        public bool jpeg { get; set; } = true;
        public bool error { get; set; } = false;
        public int width { get; set; }
        public int height { get; set; }

        public new string ToString
        {
            get
            {
                var filename = Path.GetFileName(FilePath);

                var fileFormatted =
                    filename.Length > 50 ?
                    filename.Substring(0, 23) + "...." + filename.Substring(filename.Length - 23) :
                    filename.PadLeft(50);

                string info;

                if (!jpeg)
                    info = "(not a JPEG)";
                else if (error)
                    info = "Error reading file!";
                else
                    info = String.Format("{0,10} {1,10} bytes", width + "x" + height, size);

                return String.Format("{0}: {1,27}", fileFormatted, info);
            }
        }

        public static List<JpegInfo> GetFiles(String path)
        {
            var files = new List<JpegInfo>();

            foreach (var file in Directory.EnumerateFiles(path))
            {
                JpegBitmapDecoder jpeg;

                try
                {
                    var filePath = Path.Combine(path, file);

                    jpeg = new JpegBitmapDecoder(
                        new FileStream(filePath, FileMode.Open),
                        System.Windows.Media.Imaging.BitmapCreateOptions.None,
                        System.Windows.Media.Imaging.BitmapCacheOption.None
                    );

                    var frame = jpeg.Frames[0];
                    files.Add(new JpegInfo() { FilePath = file, width = frame.PixelWidth, height = frame.PixelHeight });
                }
                catch (FileFormatException)
                {
                    files.Add(new JpegInfo() { FilePath = file, jpeg = false });
                    continue;
                }
                catch (Exception)
                {
                    files.Add(new JpegInfo() { FilePath = file, error = true });
                    continue;
                }
            }

            return files;
        }

        public static bool Similar(JpegInfo a, JpegInfo b)
        {
            return (
                a.jpeg == b.jpeg &&
                a.size == b.size &&
                a.width == b.width &&
                a.height == b.height);
        }

        public bool Similar(JpegInfo compare)
        {
            return JpegInfo.Similar(this, compare);
        }
    }
}
