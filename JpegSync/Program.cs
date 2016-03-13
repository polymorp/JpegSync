using JpegSync;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace JpegSync
{
	static class Program
	{
		static void Main(string[] args)
		{
			var argPointer = 0;
			String pathInfo;

			while (argPointer < args.Length)
			{
				if (args[argPointer].Equals("-i"))
				{
					argPointer++;
					try
					{
						// This needs work, the check if the path is valid etc is not robust
						var expanded = Environment.ExpandEnvironmentVariables(args[argPointer]);
						argPointer++;
                        //pathInfo = Path.GetDirectoryName(expanded);
                        pathInfo = expanded;
						PrintPathInfo(pathInfo);
#if DEBUG
                        Console.ReadKey();
#endif
                        return;
					}
					catch (Exception e)
					{
						Console.WriteLine(args[argPointer] + " is not a valid path");
						Console.WriteLine();
						PrintHelpMessage();
						return;
					}
				}
                else if (args[argPointer].Equals("-s"))
                {
                    argPointer++;
                    try
                    {
                        var pathFrom = Environment.ExpandEnvironmentVariables(args[argPointer]);
                        if (!Directory.Exists(pathFrom))
                            throw new DirectoryNotFoundException();
                        argPointer++;

                        var pathTo = Environment.ExpandEnvironmentVariables(args[argPointer]);
                        if (!Directory.Exists(pathTo))
                            throw new DirectoryNotFoundException();
                        argPointer++;

                        SyncPaths(pathFrom, pathTo);
#if DEBUG
                        Console.ReadKey();
#endif
                        return;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        Console.WriteLine(args[argPointer] + " is not a valid path");
                        Console.WriteLine();
                        PrintHelpMessage();
                        return;
                    }
                }
				else
				{
					PrintHelpMessage();
					return;
				}
			}

			PrintHelpMessage();
		}

		static void PrintHelpMessage()
		{
			Console.WriteLine("Syntax:");
			Console.WriteLine("JpegSync");
			Console.WriteLine(" -i path                 Shows info on all JPEG files at the specified location");
			Console.WriteLine(" -s path_from path_to    Syncs JPEGs from first path to the second");
			Console.WriteLine("");
			Console.WriteLine("");
			Console.WriteLine("");
#if DEBUG
            Console.ReadKey();
#endif
        }

        static void PrintPathInfo(String pathInfo)
		{
			Console.WriteLine("Reading from " + pathInfo);

            var files = JpegInfo.GetFiles(pathInfo);

			foreach (var file in files)
			{
                Console.WriteLine(file.ToString);
            }
		}

        static void SyncPaths(string pathFrom, string pathTo)
        {
            Console.WriteLine("Syncing");
            Console.WriteLine(String.Format("  {0}", pathFrom.Abbreviate(70, 4)));
            Console.WriteLine("  to");
            Console.WriteLine(String.Format("  {0}", pathTo.Abbreviate(70, 4)));
            Console.WriteLine();

            var filesFrom = JpegInfo.GetFiles(pathFrom).Where(x => x.jpeg && x.width == 1920 && x.height == 1080);
            var filesTo = JpegInfo.GetFiles(pathTo).Where(x => x.jpeg);

            var filesNew = filesFrom.Where(x => filesTo.All(y => !x.Similar(y)));

            if (filesNew.Count() > 0)
            {
                Console.WriteLine("Copying:");
                foreach (var file in filesNew)
                {
                    Console.WriteLine(file.ToString);
                    var filePathFrom = Path.Combine(pathFrom, file.FileName);
                    var filePathTo = Path.Combine(pathTo, file.FileName + ".jpg");
                    File.Copy(filePathFrom, filePathTo);
                }
            }
            else
            {
                Console.WriteLine("No new files");
            }
            Console.WriteLine();
        }

        public static string Abbreviate(this string str, int length, int dots)
        {
            int left = (length - dots) / 2;
            int right = (length - dots) - left;

            return
                str.Length > length ?
                str.Substring(0, left) + (new string('.', dots)) + str.Substring(str.Length - right) :
                str;
        }
    }
}
