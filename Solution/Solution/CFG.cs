using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

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

				if (UTF8Encoding.Equals (theTerminal, "0xE2")) {
					this.AddRule (theVariable, "e");
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
			this.nonTerminalStates.Add (nonTerminal);
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
			chomskyCFG.startState = "S0";

			// 2) Remove ε rules, i.e. rules of the form "A -> ε".



			// 3) Remove unit rules, i.e. rules of the form A → B, where B is a variable.



			// 4) Convert all remaining rules into the proper form.


			return chomskyCFG;
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

