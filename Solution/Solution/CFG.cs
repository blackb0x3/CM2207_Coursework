using System;
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
		private Dictionary<String, List<String>> rules = new Dictionary<String, List<String>>();

		public CFG (String terminals, String nonTerminals, String start, String[] rules)
		{
		}

		private void AddRule(String variable, String terminal)
		{
			if (!this.rules.ContainsKey (variable))
			{
				this.rules.Add (variable, new List<String> ());
			}

			this.rules[variable].Add(terminal);
		}

		public List<String> GetRulesForVariable(String variable)
		{
			return this.rules [variable];
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

		public String ToString()
		{
			String toReturn = "";

			GetNonTerminalStates().ForEach(x => toReturn += x);
			GetTerminalStates().ForEach(x => toReturn += x);
			toReturn += GetStartState ();
			//this.rules.Values.
		}
	}
}

