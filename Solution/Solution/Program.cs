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
			CFG theCFG = GenerateCFGFromFile (args [0]);
			Console.WriteLine (theCFG.ToString());
			Console.WriteLine ();
			CFG theChomsky = theCFG.ConvertToChomsky ();
			Console.WriteLine (theChomsky.ToString());

			/*String text1 = "helloworld!";
			String text2 = "world";

			char[] exceptions = text1.Except(text2).ToArray();

			foreach (char ex in exceptions)
			{
				Console.WriteLine (ex);
			}*/
		}

		public static CFG GenerateCFGFromFile(String fileName)
		{
			String[] lines = new String[4];
			String[] rules;

			try
			{
				// CFG encodings should be located in the Solution/bin/Debug folder, unless the path is specified in the command line
				lines = File.ReadAllLines (fileName, Encoding.UTF8);
			}

			catch (IOException e)
			{
				Console.WriteLine (e.Message);
				Environment.Exit (-1);
			}

			finally
			{
				rules = new String[lines.Length - 3];
				Array.Copy (lines, 3, rules, 0, rules.Length);
			}

			return new CFG(lines[1], lines[0], lines[2], rules);
		}
	}
}
