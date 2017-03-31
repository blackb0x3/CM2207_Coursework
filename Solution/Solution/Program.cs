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
			switch (args.Length)
			{
			case 1:
				TaskTwo (args[0]);
				break;
			case 2:
				TaskThree (args);
				break;
			default:
				Console.WriteLine ("Usage: mono Solution.exe CFG_FILENAME STRING_TO_DERIVE");
				Console.WriteLine ("Optional Arguments: STRING_TO_DERIVE");
				break;
			}
		}

		private static void TaskTwo(String fileName)
		{
			Console.WriteLine ("Original Form");
			CFG theCFG = GenerateCFGFromFile (fileName);
			Console.WriteLine (theCFG.ToString());
			Console.WriteLine ("Chomsky Normal Form");
			theCFG = theCFG.ConvertToChomsky ();
			Console.WriteLine (theCFG.ToString());
		}

		private static void TaskThree(String[] args)
		{
			CFG theCFG = GenerateCFGFromFile (args [0]);
			theCFG = theCFG.ConvertToChomsky ();

			String theStringToDerive = args [1];
			int derivationLength = theStringToDerive.Split (" ".ToCharArray ()).Length;
			List<String> generatedStrings = DeriveTerminal(theStringToDerive, derivationLength, theCFG);

			if (generatedStrings.Contains(theStringToDerive))
			{
				Console.WriteLine ("Derived '{0}'\n\nGenerated strings:", theStringToDerive);

				foreach (String derivation in generatedStrings)
				{
					Console.WriteLine (derivation);
				}
			}

			else
			{
				Console.WriteLine ("Couldn't derive '{0}'", theStringToDerive);
			}
		}

		private static List<String> DeriveTerminal(String stringToDerive, int n, CFG cfg) // Returns the strings generated from the input string
		{
			List<String> derivationsList = cfg.GetRulesForVariable (cfg.GetStartState ());
			List<String> updatedDerivations = new List<String> ();
			int productionCount = -1;

			do
			{
				productionCount++;
				foreach (String theseDerivations in derivationsList)
				{
					StringBuilder newString = new StringBuilder(theseDerivations);

					foreach (String derivation in theseDerivations.Split(" ".ToCharArray()))
					{
						if (cfg.GetNonTerminalStates().Contains(derivation))
						{
							foreach (String terminal in cfg.GetRulesForVariable (derivation))
							{
								newString = newString.Replace(derivation, terminal);
							}

							updatedDerivations.Insert (0, newString.ToString ());
						}
					}
				}

				derivationsList = cfg.CloneList(updatedDerivations);
			}
			while (!(derivationsList.Contains(stringToDerive)) && 2 * productionCount - 1 < n);

			return derivationsList;
		}

		public static CFG GenerateCFGFromFile(String fileName)
		{
			String[] lines = new String[4]; // Makes room for a grammar that contains no rules
			String[] rules;

			try
			{
				// CFG encodings should be located in the Solution/bin/Debug folder
				lines = File.ReadAllLines (fileName, Encoding.UTF8);
			}

			catch (IOException e)
			{
				Console.WriteLine (e.Message);
				Console.ReadLine ();
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
