using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


namespace JpegSync
{
    internal static class Program
	{


        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        const int SW_MINIMISED= 2;


        private static void Main(string[] args)
		{
			var argPointer = 0;

		    var handle = GetConsoleWindow();
           // ShowWindow(handle, SW_SHOW);

           if (args.Contains("-hide"))
			{
                ShowWindow(handle, SW_MINIMISED);
            }

            while (argPointer < args.Length)
				if (args[argPointer].Equals("-i"))
				{
					argPointer++;
					try
					{
						// This needs work, the check if the path is valid etc is not robust
						var expanded = Environment.ExpandEnvironmentVariables(args[argPointer]);
						argPointer++;
                        //pathInfo = Path.GetDirectoryName(expanded);
                        var pathInfo = expanded;
						PrintPathInfo(pathInfo);
#if DEBUG
                        Console.ReadKey();
#endif
                        return;
					}
					catch (Exception e)
					{
						Console.WriteLine(args[argPointer] + " is not a valid path");
                        Console.WriteLine(e.Message);
						PrintHelpMessage();
						return;
					}
				}
                else if (args[argPointer].Equals("-s"))
                {
                    argPointer++;
                    try
                    {
                        var pathFrom = Environment.ExpandEnvironmentVariables(args[argPointer]).Trim();
                        if (!Directory.Exists(pathFrom))
                            throw new DirectoryNotFoundException();
                        argPointer++;

                        var pathTo = Environment.ExpandEnvironmentVariables(args[argPointer]).Trim();
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

			PrintHelpMessage();
		}

        private static void PrintHelpMessage()
		{
			Console.WriteLine("Syntax:");
			Console.WriteLine("JpegSync");
			Console.WriteLine(" -i path                 Shows info on all JPEG files at the specified location");
			Console.WriteLine(" -s path_from path_to    Syncs JPEGs from first path to the second");
            Console.WriteLine(" -hide                   Hide console window");
			Console.WriteLine("");
			Console.WriteLine("");
#if DEBUG
            Console.ReadKey();
#endif
        }

        private static void PrintPathInfo(string pathInfo)
		{
			Console.WriteLine("Reading from " + pathInfo);

            var files = JpegInfo.GetFiles(pathInfo);

			foreach (var file in files)
                Console.WriteLine(file.ToString);
            }

        private static void SyncPaths(string pathFrom, string pathTo)
        {
            Console.WriteLine("Syncing");
            Console.WriteLine($"  {pathFrom.Abbreviate(70, 4)}");
            Console.WriteLine("  to");
            Console.WriteLine($"  {pathTo.Abbreviate(70, 4)}");
            Console.WriteLine();

            
            var filesFrom = JpegInfo.GetFiles(pathFrom).Where(x => x.Jpeg && x.Width == 1920 && x.Height == 1080);
            var filesTo = JpegInfo.GetFiles(pathTo).Where(x => x.Jpeg);

            //#if DEBUG
            //            Stopwatch sw = Stopwatch.StartNew();
            //#endif
            //            var filesNew = filesFrom.Where(x => filesTo.All(y => !x.Similar(y)));
            //#if DEBUG
            //            sw.Stop();
            //            Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
            //Stopwatch sw2 = Stopwatch.StartNew();
        
            //var filesNew2 = filesFrom.Where(x => filesTo.All(y => y.Hash != x.Hash));
            //sw2.Stop();
            //#if DEBUG
            //            
           // Stopwatch sw3 = Stopwatch.StartNew();
            var filesNew3 = filesFrom.Where(x => filesTo.All(y => y != x));
           // sw3.Stop();

            //Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
            //Console.WriteLine("Time taken: {0}ms", sw2.Elapsed.TotalMilliseconds);
           // Console.WriteLine("Time taken: {0}ms", sw3.Elapsed.TotalMilliseconds);
           // Console.WriteLine("filenew: " + filesNew.Count());
           // Console.WriteLine("filenew2: " + filesNew2.Count());
            Console.WriteLine("filenew3: " + filesNew3.Count());
// #endif
            if (filesNew3.Any())
            {
                Console.WriteLine("Copying:");
                foreach (var file in filesNew3)
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
            var left = (length - dots) / 2;
            var right = length - dots - left;

            return
                str.Length > length
                    ? str.Substring(0, left) + new string('.', dots) + str.Substring(str.Length - right)
                    : str;
        }
    }
}