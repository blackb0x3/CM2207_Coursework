using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Solution
{
	class MainClass
	{
		public static void Main (string[] args)
		{
		}

		public static void ReadCFGFile(String fileName)
		{
			try
			{
				// CFG encodings should be located in the bin folder, unless the path is specified in the command line
				String[] lines = File.ReadAllLines (fileName);
			}

			catch (IOException e)
			{
				Console.WriteLine (e.Message);
			}

			finally
			{
				
			}
		}
	}
}
