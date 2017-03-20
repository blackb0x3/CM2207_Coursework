# CM2207_Coursework

This coursework focusses on context-free grammars (CFGs). CFGs are used in modelling the syntax of programming languages. Compilers for programming languages will embody a parser, i.e., an algorithm to determine whether a given string belongs to the language generated by the CFG. If the string does belong, then the parser will produce a derivation of the string, which the compiler would then use to translate the string into assembly language. Your overall aim in this coursework is to implement a parser for CFGs. Specifically, you are required to:
<ol>
<li>Devise an encoding (i.e., a data structure) for CFGs that can be used to specify any given CFG in a .txt file, which can then be passed to and processed by the programs in parts 2,3 below.</li>
<li>Write a program that can take any given CFG as input (specified as a separate .txt file as in part 1) and output an equivalent CFG in Chomsky normal form.</li>
<li>Extend the program in part 2 so that, after the conversion to Chomsky normal form, it asks the user for an input string and then determines whether or not the string can be generated by the given CFG. If it can be generated, then it should output a derivation of the string.</li>
</ol>
