using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

/* This package enables me to iterate over the possible permutations over production outputs that generate the empty string
 * Version: 1.0.3.2
 * Author/s: Adrian Akison, Eoin Campbell
 * URL: https://www.nuget.org/packages/Combinatorics
 * Date Published: 30-08-2012
 * Date Accessed: 28-03-2017
 * License: https://github.com/eoincampbell/combinatorics/blob/master/LICENCE
 * Used in: CFG.ModifyRulesForEmptyString(), Program.Main()
 * 
 */
using Combinatorics;
using Combinatorics.Collections;

namespace Solution
{
	public class CFG
	{
		private List<String> terminalStates;
		private List<String> nonTerminalStates;
		private String startState;
		private Dictionary<String, List<String>> rules;

		public CFG (String terminals, String nonTerminals, String start, String[] rules)
		{
			this.terminalStates = new List<String> ();
			this.nonTerminalStates = new List<String> ();
			this.startState = start;
			this.rules = new Dictionary<String, List<String>>();

			foreach (String symbol in nonTerminals.Split(",".ToCharArray()))
			{
				this.AddNonTerminal(symbol);
			}

			foreach (String symbol in terminals.Split(" ".ToCharArray()))
			{
				this.AddTerminal (symbol);
			}

			foreach (String rule in rules)
			{
				String theVariable = rule.Split (":".ToCharArray())[0];
				String theTerminal = rule.Split (":".ToCharArray())[1];

				if (UTF8Encoding.Equals (theTerminal, "0xE2") && !GetTerminalStates().Contains(theTerminal))
				{
					this.AddRule (theVariable, "ε");
				}

				else
				{
					this.AddRule (theVariable, theTerminal);
				}
			}
		}

		private void AddRule(String variable, String terminal)
		{
			if (!this.rules.ContainsKey (variable))
			{
				this.rules.Add (variable, new List<String> ());
			}

			this.rules[variable].Add(terminal);
		}

		private void AddTerminal(String terminal)
		{
			this.terminalStates.Add (terminal);
		}

		private void AddNonTerminal(String nonTerminal)
		{
			if (nonTerminal == "S0")
			{
				this.nonTerminalStates.Insert (0, nonTerminal);
			}

			else
			{
				this.nonTerminalStates.Add (nonTerminal);
			}
		}

		public List<String> GetRulesForVariable(String variable)
		{
			return this.rules [variable];
		}

		public String PrintRulesForVariable(String variable)
		{
			String toReturn = "";

			foreach (String rule in GetRulesForVariable(variable))
			{
				toReturn += variable + " -> " + rule + "\n";
			}

			return toReturn;
		}

		public List<String> GetTerminalStates()
		{
			return this.terminalStates;
		}

		public List<String> GetNonTerminalStates()
		{
			return this.nonTerminalStates;
		}

		public String GetStartState()
		{
			return this.startState;
		}

		public Dictionary<String, List<String>> GetAllRules()
		{
			return this.rules;
		}

		public String PrintAllRules()
		{
			String toReturn = "";

			foreach (String key in GetAllRules().Keys)
			{
				toReturn += PrintRulesForVariable(key);
			}

			return toReturn;
		}

		public CFG ConvertToChomsky()
		{
			CFG chomskyCFG = this;

			// 1) Add a new start variable.
			chomskyCFG.AddRule("S0", this.startState);
			this.AddNonTerminal ("S0");
			chomskyCFG.startState = "S0";

			// 2) Remove ε rules, i.e. rules of the form "A -> ε".
			chomskyCFG.RemoveEmptyStringRules();
			chomskyCFG.RemoveDuplicateRules ();

			// 3) Remove unit rules, i.e. rules of the form A → B, where B is a variable.
			chomskyCFG.RemoveUnitRules();
			chomskyCFG.RemoveDuplicateRules ();

			// 4) Convert all remaining rules into the proper form.
			chomskyCFG.ConvertRemainingRules();
			chomskyCFG.RemoveDuplicateRules ();

			return chomskyCFG;
		}

		private void RemoveDuplicateRules()
		{
			List<List<String>> trackedTerminals = new List<List<String>>();
			Dictionary<String, List<String>> newRules = new Dictionary<String, List<String>>();

			foreach (String key in this.rules.Keys)
			{
				newRules.Add(key, this.rules[key].Distinct().ToList());
				trackedTerminals.Add(this.rules[key]);
			}

			this.rules = CreateCopyOfRules (newRules);

			foreach (String key in this.rules.Keys)
			{
				bool variableIsUsed = false;

				foreach (List<String> productions in this.rules.Values)
				{
					foreach (String production in productions)
					{
						if (production.Contains(key))
						{
							variableIsUsed = true;
							break;
						}
					}

					if (variableIsUsed)
					{
						break;
					}
				}

				if (variableIsUsed || key == GetStartState())
				{
					continue;
				}

				else
				{
					newRules.Remove (key);
				}
			}

			this.rules = CreateCopyOfRules (newRules);
		}

		private void RemoveEmptyStringRules()
		{
			bool emptyStringExists;

			do
			{
				emptyStringExists = false;

				foreach (String variable in GetNonTerminalStates())
				{
					foreach (String terminal in GetRulesForVariable(variable))
					{
						if (terminal == "ε" && variable != GetStartState ())
						{
							emptyStringExists = true;
							this.rules = ModifyRulesForEmptyString(GetAllRules(), variable, terminal);
						}
					}
				}
			}
			while (emptyStringExists == true);
		}

		private void RemoveUnitRules() // i.e. rules of the form A → B, where B is a variable.
		{
			bool unitRuleExists = false;

			do
			{
				unitRuleExists = false;
				Dictionary<String, List<String>> newRules = CreateCopyOfRules (GetAllRules ());

				foreach (String variable in GetNonTerminalStates())
				{
					foreach (String terminal in GetRulesForVariable(variable))
					{
						if (variable == terminal)
						{
							unitRuleExists = true;
							newRules [variable].Remove (terminal);
						}
					}
				}

				this.rules = CreateCopyOfRules (newRules);

				foreach (String variable in GetNonTerminalStates())
				{
					foreach (String terminal in GetRulesForVariable(variable))
					{
						if (terminal != "ε" && GetNonTerminalStates ().Contains (terminal))
						{
							unitRuleExists = true;
							newRules [variable].Remove (terminal);
							newRules [variable].AddRange (newRules [terminal]);
						}
					}
				}

				this.rules = CreateCopyOfRules (newRules);

			}
			while (unitRuleExists);
		}

		private void ConvertRemainingRules()
		{
			bool thereAreRulesToChange;
			do {
				thereAreRulesToChange = false;
				Dictionary<String, List<String>> newRules = CreateCopyOfRules (GetAllRules ());
				List<String> newVariables = new List<String> ();
				List<String> terminalsAlreadyTracked = new List<String> ();

				int newVariableCount = 0;
				int newTerminalCount = 0;

				foreach (String variable in GetNonTerminalStates()) {
					foreach (String terminal in GetRulesForVariable(variable)) {
						String[] terminals = terminal.Split (" ".ToCharArray ());

						// >= 3 because whitespaces are used to separate terminal outputs, e.g. "A -> one two three" produces 3 terminal symbols,
						// assuming one two and three are in the language of the CFG
						if (terminals.Length >= 3 && !GetTerminalStates ().Any (x => terminals.Contains (x))) {
							thereAreRulesToChange = true;
							String[] terminalsToModify = terminals.Skip (1).Take (terminals.Length - 1).ToArray ();

							for (int i = 0; i < terminalsToModify.Length; i += 2) {
								if (!newVariables.Contains ("V" + newVariableCount)) {
									String newVariable = "V" + newVariableCount;
									newVariableCount++;
									String newTerminal = String.Join (" ", new String[] { terminalsToModify [i], terminalsToModify [i + 1] });

									newVariables.Add (newVariable);
									if (!newRules.Keys.Contains(newVariable))
									{
										newRules.Add (newVariable, new List<String> ());
									}

									newRules [newVariable].Add (newTerminal);
								}
							}

							this.rules = CreateCopyOfRules (newRules);
							thereAreRulesToChange = false;

							foreach (String newVariable in newVariables) {
								foreach (String variable2 in GetNonTerminalStates()) {
									foreach (String terminal2 in GetRulesForVariable(variable2)) {
										// If the new resulting terminal pair is contained in any of the original terminal outputs
										if (newRules [newVariable].ElementAt (0) != terminal2 && terminal2.Contains (newRules [newVariable].ElementAt (0))) {
											StringBuilder theModifiedTerminal = new StringBuilder (terminal2);
											int theIndex = terminal2.IndexOf (newRules [newVariable].ElementAt (0));
											theModifiedTerminal.Remove (theIndex, newRules [newVariable].ElementAt (0).Length);
											theModifiedTerminal.Insert (theIndex, newVariable);

											newRules [variable2].Remove (terminal2);
											newRules [variable2].Add (theModifiedTerminal.ToString ());
										}
									}
								}
							}

							this.rules = CreateCopyOfRules (newRules);
						}

						// If there are terminal symbols in the current production's output, create a rule for it and replace it's symbol in all other production rules 
						if (terminals.Length >= 2 && GetTerminalStates ().Any (x => terminals.Contains (x))) {
							thereAreRulesToChange = true;
							List<String> theTerminalsToConvert = terminals.Where (x => GetTerminalStates ().Contains (x)).ToList ();

							foreach (String aTerminal in theTerminalsToConvert) {
								if (!newVariables.Contains ("U" + newTerminalCount) && !terminalsAlreadyTracked.Contains (aTerminal)) {
									String theNewVariable = "U" + newTerminalCount;
									newTerminalCount++;
									String theNewTerminal = aTerminal;

									newVariables.Add (theNewVariable);
									newRules.Add (theNewVariable, new List<String> ());
									newRules [theNewVariable].Add (theNewTerminal);
									terminalsAlreadyTracked.Add (theNewTerminal);
								}
							}

							this.rules = CreateCopyOfRules (newRules);

							foreach (String newVariable in newVariables) {
								foreach (String variable2 in GetNonTerminalStates()) {
									foreach (String terminal2 in GetRulesForVariable(variable2)) {
										// If the new resulting terminal pair is contained in any of the original terminal outputs
										if (newRules [newVariable].ElementAt (0) != terminal2 && terminal2.Contains (newRules [newVariable].ElementAt (0))) {
											StringBuilder theModifiedTerminal = new StringBuilder (terminal2);
											int theIndex = terminal2.IndexOf (newRules [newVariable].ElementAt (0));
											theModifiedTerminal.Remove (theIndex, newRules [newVariable].ElementAt (0).Length);
											theModifiedTerminal.Insert (theIndex, newVariable);

											newRules [variable2].Remove (terminal2);
											newRules [variable2].Add (theModifiedTerminal.ToString ());
										}
									}
								}
							}
						}
					}
				}

			} while (thereAreRulesToChange == true);
		}

		public List<String> CloneList(List<String> oldList)
		{
			List<String> newList = new List<String> ();

			foreach (String val in oldList)
			{
				newList.Add (val);
			}

			return newList;
		}

		private Dictionary<String, List<String>> CreateCopyOfRules(Dictionary<String, List<String>> oldRules)
		{
			Dictionary<String, List<String>> newRules = new Dictionary<String, List<String>> ();

			foreach (String key in oldRules.Keys)
			{
				newRules.Add (key, new List<String> ());

				foreach (String val in oldRules[key])
				{
					newRules [key].Add (val);
				}
			}

			return newRules;
		}

		private Dictionary<String, List<String>> ModifyRulesForEmptyString(Dictionary<String, List<String>> oldRules, String theVariableToModify, String theTerminal)
		{
			Dictionary<String, List<String>> newRules = CreateCopyOfRules (oldRules);
			newRules[theVariableToModify].Remove (theTerminal);

			foreach (String terminalVariable in GetNonTerminalStates()) // Go back through the variables
			{
				foreach (String variableThatIsTerminal in oldRules[terminalVariable]) // Go back through the rules and their output strings
				{
					if (variableThatIsTerminal.Contains (theVariableToModify)) // Find any new occurences of the variable on the right hand side
					{
						if (variableThatIsTerminal == theVariableToModify)
						{
							newRules[terminalVariable].Add(theTerminal); // Add empty string iff only the current variable is produced
						}

						else // If other terminals are produced other than the current variable, permutate through them and add them as rules
						{
							String variableToChange = variableThatIsTerminal;

							while (variableToChange.IndexOf(theVariableToModify) != -1)
							{
								variableToChange = variableToChange.Remove(variableToChange.IndexOf(theVariableToModify), 1);
								String[] terminalsInvolved = variableToChange.Split(" ".ToCharArray());

								var perms = new Permutations<String>(terminalsInvolved, GenerateOption.WithoutRepetition);
								//var combs = new Combinations<String> (terminalsInvolved, terminalsInvolved.Length);
								var variations = new Variations<String> (terminalsInvolved, terminalsInvolved.Length);

								//var exceptions = perms.Except (variations);

								foreach (List<String> permutation in perms.Except(variations))
								{
									permutation.RemoveAll (x => x.Equals (""));

									if (!(newRules[terminalVariable].Contains(String.Join("", permutation)) || newRules[terminalVariable].Contains(String.Join(" ", permutation))))
									{
										if (String.Join("", permutation).Length > 1)
										{
											newRules[terminalVariable].Add(String.Join(" ", permutation));
										}

										else
										{
											newRules[terminalVariable].Add(String.Join("", permutation));
										}
									}
								}
							}
						}
					}
				}
			}

			return newRules;
		}

		public override String ToString()
		{
			String toReturn = "V: {";

			GetNonTerminalStates ().ForEach (x => toReturn += x + ",");
			toReturn = toReturn.Remove (toReturn.Length - 1, 1);
			toReturn += "}\n\u03A3: {"; // \u03A3 is the unicode value for the Σ symbol - to represent the language of the CFG
			GetTerminalStates ().ForEach (x => toReturn += x + ",");
			toReturn = toReturn.Remove (toReturn.Length - 1, 1);
			toReturn += "}\nS: " + GetStartState () + "\n";
			toReturn += "Rules:\n" + PrintAllRules ();

			return toReturn;
		}
	}
}

