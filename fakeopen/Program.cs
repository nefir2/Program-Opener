using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace fakeopen
{
	internal class Program
	{
		[STAThread] private static void Main(string[] args)
		{
			bool isHidden = false;
			if (args.Length != 0)
			{
				Console.WriteLine("Args exists.");
				Hider(true);
				isHidden = true;
			}
			else Console.WriteLine("there's no args with start.");
			while (true)
			{
				try
				{
					if (!(args is null) && args.Length != 0)
					{
						Console.WriteLine($"Starting file \"{args[0]}\" with parameters {ConcatArgs(KillFirst(args))}");
						Start(GetFileAndArgs(args).ToTuple());
						break;
					}
					else
					{
						Console.WriteLine("starting OpenFileDialog...");
						OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true, CheckFileExists = false, CheckPathExists = false };
						if (ofd.ShowDialog() == DialogResult.OK)
						{
							foreach (var filename in ofd.SafeFileNames) Console.WriteLine($"choosed filenames: {filename}");
							List<string> s = new List<string>(ofd.SafeFileNames);
							s.RemoveAt(0);
							Console.WriteLine($"Starting file \"{ofd.FileNames[0]}\" with parameters {ConcatArgs(s.ToArray())}");
							Start(ofd.FileNames[0], ConcatArgs(s.ToArray()));
						}
						break;
					}
				}
				catch (Exception ex) 
				{
					if (isHidden) Hider(false);
					Console.WriteLine("OH NO! I GOT AN EXCEPTION!");
					Console.WriteLine("exception message: " + ex.Message);
					Console.WriteLine("\ndeleting args...");
					Console.Write($"program args before: ");
					ShowArray(args);
					args = null;
					Console.WriteLine($"program args after: {args}");
					continue;
				}
			}
		}
		/// <summary>
		/// запуск следующей программы, и закрытие этой.
		/// </summary>
		/// <param name="process"></param>
		private static void Start(string process)
		{
			Process.Start(process);
			ExitProcess(0);
		}
		/// <summary>
		/// запуск следующей программы, и закрытие этой.
		/// </summary>
		/// <param name="process"></param>
		private static void Start(string process, string args)
		{
			Process.Start(process, args);
			ExitProcess(0);
		}
		private static void Start(Tuple<string, string> files) => Start(files.Item1, files.Item2);
		private static (string process, string args) GetFileAndArgs(string[] FileNames)
		{
			string filename = FileNames[0];
			string fileargs = "";
			for (int i = 1; i < FileNames.Length; i++) fileargs += $" {FileNames[i]}";
			fileargs.Trim(' ');
			return (filename, fileargs);
		}
		private static string ConcatArgs(string[] args, char separator = ' ')
		{
			string ret = "";
			for (int i = 1; i < args.Length; i++) ret += $"{separator}{args[i]}";
			ret.Trim(separator);
			return ret;
		}
		private static T[] KillFirst<T>(T[] array)
		{
			List<T> list = new List<T>(array);
			list.RemoveAt(0);
			return list.ToArray();
		}
		private static void ShowArray<T>(T[] array, string sep = ", ", bool hasEndLine = true)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (i + 1 != array.Length) Console.Write($"{array[i]}{sep}");
				else Console.Write(array[i]);
			}
			if (hasEndLine) Console.WriteLine();
		}

		private static IntPtr hWnd;
		//https://habr.com/ru/company/otus/blog/598409/?ysclid=lacmcadxbe475507780
		[DllImport("user32.dll", SetLastError = true)][return: MarshalAs(UnmanagedType.Bool)] public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);
		[DllImport("user32.dll")] public static extern int MessageBox(IntPtr hwnd, String text, String caption, int options);
		private const int HWND_TOPMOST_ON = -1;
		private const int HWND_TOPMOST_OFF = -2; //добавлено собственноручно, методом тыка.
		private const int SWP_NOMOVE = 0x0002;
		private const int SWP_NOSIZE = 0x0001;
		[DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		const int SW_HIDE = 0;
		const int SW_SHOW = 5;
		[DllImport("kernel32.dll")] public static extern IntPtr GetConsoleWindow();
		[DllImport("kernel32.dll")] public static extern void ExitProcess([In] uint uExitCode);
		/// <summary>
		/// скрытие главного окна.
		/// </summary>
		/// <param name="isHidden"><see langword="true"/> чтобы скрыть, <br/><see langword="false"/> чтобы показать.</param>
		public static void Hider(bool isHidden)
		{
			hWnd = GetConsoleWindow();
			ShowWindow(hWnd, isHidden ? SW_HIDE : SW_SHOW);
		}
	}
}