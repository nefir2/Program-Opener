using System;
using System.Diagnostics;

namespace fakeopen
{
	internal class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				try
				{
					if (args.Length != 0)
					{
						Process.Start(args[0]);
						break;
					}
					else
					{
						Console.Write("Path to program: ");
						Process.Start(Console.ReadLine());
						break;
					}
				}
				catch (Exception ex) 
				{
					Console.WriteLine(ex.Message);
					args = null;
					continue;
				}
			}
		}
	}
}