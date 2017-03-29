using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Solution
{
	delegate void RecursiveAction();

	class MainClass
	{
		public static void Main (string[] args)
		{
			/* Code for testing functionality for task 2

			Console.WriteLine ("Original Form");
			CFG theCFG = GenerateCFGFromFile (args [0]);
			Console.WriteLine (theCFG.ToString());
			Console.WriteLine ("Chomsky Normal Form");
			theCFG = theCFG.ConvertToChomsky ();
			Console.WriteLine (theCFG.ToString());
			
			*/

			// Task 3
			CFG theCFG = GenerateCFGFromFile (args [0]);
			theCFG = theCFG.ConvertToChomsky ();

			String theStringToDerive = args [1];
			List<String> generatedStrings = DeriveTerminal(theStringToDerive, "", new List<String> (), theCFG);
		}

		private static List<String> DeriveTerminal(String stringToDerive, String currentString, List<String> previousDerivations, CFG cfg) // Returns the strings generated from the input string
		{
			RecursiveAction ra = delegate()
			{
				previousDerivations.AddRange (DeriveTerminal (stringToDerive, currentString, previousDerivations, cfg));
			};

			if (previousDerivations.Contains(stringToDerive) && currentString == stringToDerive)
			{
				Console.WriteLine ("Derived '{0}'", currentString);
				return previousDerivations;
			}

			else if (previousDerivations.Count == 0)
			{
				List<String> terminalsProduced = cfg.GetAllRules () [cfg.GetStartState ()];

				foreach (String terminal in terminalsProduced)
				{
					previousDerivations.Add (terminal);
					Console.WriteLine (terminal);

					Thread thisThread = new Thread(new ThreadStart(ra), 100);
					thisThread.Start ();
				}

				return previousDerivations;
			}

			else
			{
				foreach (String output in currentString.Split(" ".ToCharArray()))
				{
					if (cfg.GetNonTerminalStates().Contains(output))
					{
						foreach (String terminal in cfg.GetRulesForVariable(output))
						{
							String newString = currentString.Replace (output + " ", terminal + " ");
							previousDerivations.Add (newString);
							Console.WriteLine (newString);

							Thread thisThread = new Thread(new ThreadStart(ra), 100);
							thisThread.Start ();
						}
					}
				}

				return previousDerivations;
			}
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
