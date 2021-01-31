namespace GameCreator.Stats
{
    using System;
    using System.Text.RegularExpressions;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Core;
    using GameCreator.Variables;
    using System.Text;
    using System.Globalization;

    [Serializable]
    public class Formula
    {
        public class Operation
        {
            public Func<FormulaData, string, string> action;
            public Regex regex;

            public Operation(string pattern, Func<FormulaData, string, string> action)
            {
                this.regex = new Regex(pattern);
                this.action = action;
            }
        }

        public class FormulaData
        {
            public float baseValue;
            public Table table;

            public Stats origin;
            public Stats target;

            public FormulaData(float baseValue, Table table, Stats origin, Stats target)
            {
                this.baseValue = baseValue;
                this.table = table;
                this.origin = origin;
                this.target = target;
            }
        }

        // DATA: ----------------------------------------------------------------------------------

        private static readonly CultureInfo CULTURE = new CultureInfo("en-US");

        /*
        public static Operation[] OPERATIONS = new Operation[]
        {
            new Operation(@"this\[value\]", OnMatch_ThisBaseValue),                    // this[value]
            new Operation(@"stat\[[a-zA-Z0-9\._-]+\]", OnMatch_StatName),              // stat[name]
            new Operation(@"attr\[[a-zA-Z0-9\._-]+\]", OnMatch_AttrName),              // attr[name]
            new Operation(@"stat:other\[[a-zA-Z0-9\._-]+\]", OnMatch_StatOtherName),   // stat:other[name]
            new Operation(@"attr:other\[[a-zA-Z0-9\._-]+\]", OnMatch_AttrOtherName),   // attr:other[name]
            new Operation(@"local\[[a-zA-Z0-9\._-]+\]", OnMatch_LocalName),            // local[name]
            new Operation(@"local:other\[[a-zA-Z0-9\._-]+\]", OnMatch_LocalOtherName), // local:other[name]
            new Operation(@"global\[[a-zA-Z0-9\._-]+\]", OnMatch_GlobalName),          // global[name]
            new Operation(@"rand\([a-zA-Z0-9\._-]+,[a-zA-Z0-9\._-]+\)", OnMatch_Rand), // rand(min,max)
            new Operation(@"dice\([a-zA-Z0-9\._-]+,[a-zA-Z0-9\._-]+\)", OnMatch_Dice), // dice(rolls,sides)
            new Operation(@"chance\([a-zA-Z0-9\._-]+\)", OnMatch_Chance),              // chance(value)
            new Operation(@"min\([a-zA-Z0-9\._-]+,[a-zA-Z0-9\._-]+\)", OnMatch_Min),   // min(a,b)
            new Operation(@"max\([a-zA-Z0-9\._-]+,[a-zA-Z0-9\._-]+\)", OnMatch_Max),   // max(a,b)
            new Operation(@"table:rise\([a-zA-Z0-9\._-]+\)", OnMatch_TableRise),       // table:rise(value)
            new Operation(@"table\([a-zA-Z0-9\._-]+\)", OnMatch_Table),                // table(value)
        };*/

        public static Operation[] OPERATIONS = new Operation[]
        {
            new Operation(@"this\[value\]", OnMatch_ThisBaseValue),       // this[value]
            new Operation(@"stat\[\S+\]", OnMatch_StatName),              // stat[name]
            new Operation(@"attr\[\S+\]", OnMatch_AttrName),              // attr[name]
            new Operation(@"stat:other\[\S+\]", OnMatch_StatOtherName),   // stat:other[name]
            new Operation(@"attr:other\[\S+\]", OnMatch_AttrOtherName),   // attr:other[name]
            new Operation(@"local\[\S+\]", OnMatch_LocalName),            // local[name]
            new Operation(@"local:other\[\S+\]", OnMatch_LocalOtherName), // local:other[name]
            new Operation(@"global\[\S+\]", OnMatch_GlobalName),          // global[name]
            new Operation(@"rand\(\S+,\S+\)", OnMatch_Rand),              // rand(min,max)
            new Operation(@"dice\(\S+,\S+\)", OnMatch_Dice),              // dice(rolls,sides)
            new Operation(@"chance\(\S+\)", OnMatch_Chance),              // chance(value)
            new Operation(@"min\(\S+,\S+\)", OnMatch_Min),                // min(a,b)
            new Operation(@"max\(\S+,\S+\)", OnMatch_Max),                // max(a,b)
            new Operation(@"table:rise\(\S+\)", OnMatch_TableRise),       // table:rise(value)
            new Operation(@"table\(\S+\)", OnMatch_Table),                // table(value)
        };

        private static string OnMatch_ThisBaseValue(FormulaData data, string clause)
        {
            return data.baseValue.ToString(CULTURE);
        }

        private static string OnMatch_StatName(FormulaData data, string clause)
        {
            string name = ClauseParseName(clause, data);
            return data.origin.GetStat(name, data.target).ToString(CULTURE);
        }

        private static string OnMatch_AttrName(FormulaData data, string clause)
        {
            string name = ClauseParseName(clause, data);
            return data.origin.GetAttrValue(name, data.target).ToString(CULTURE);
        }

        private static string OnMatch_StatOtherName(FormulaData data, string clause)
        {
            string name = ClauseParseName(clause, data);
            return data.target.GetStat(name, data.origin).ToString(CULTURE);
        }

        private static string OnMatch_AttrOtherName(FormulaData data, string clause)
        {
            string name = ClauseParseName(clause, data);
            return data.target.GetAttrValue(name, data.origin).ToString(CULTURE);
        }

        private static string OnMatch_LocalName(FormulaData data, string clause)
        {
            string name = ClauseParseName(clause, data);
            return Convert.ToSingle(VariablesManager.GetLocal(data.origin.gameObject, name, true)).ToString(CULTURE);
        }

        private static string OnMatch_LocalOtherName(FormulaData data, string clause)
        {
            string name = ClauseParseName(clause, data);
            return Convert.ToSingle(VariablesManager.GetLocal(data.target.gameObject, name, true)).ToString(CULTURE);
        }

        private static string OnMatch_GlobalName(FormulaData data, string clause)
        {
            string name = ClauseParseName(clause, data);
            return ((float)VariablesManager.GetGlobal(name)).ToString(CULTURE);
        }

        private static string OnMatch_Table(FormulaData data, string clause)
        {
            string value = ClauseParseInput(clause, data);
            return data.table.Tier(float.Parse(value)).ToString(CULTURE);
        }

        private static string OnMatch_TableRise(FormulaData data, string clause)
        {
            string value = ClauseParseInput(clause, data);
            return data.table.PercentNextTier(float.Parse(value)).ToString(CULTURE);
        }

        private static string OnMatch_Rand(FormulaData data, string clause)
        {
            string[] parse = ClauseParse2Inputs(clause, data);
            int min = int.Parse(parse[0]);
            int max = int.Parse(parse[1]);
            return UnityEngine.Random.Range(min, max).ToString(CULTURE);
        }

        private static string OnMatch_Dice(FormulaData data, string clause)
        {
            string[] parse = ClauseParse2Inputs(clause.ToLowerInvariant(), data);
            int rolls = int.Parse(parse[0]);
            int sides = int.Parse(parse[1]);

            float amount = 0.0f;
            for (int i = 0; i < rolls; ++i)
            {
                amount += (float)UnityEngine.Random.Range(1, sides + 1);
            }

            return amount.ToString(CULTURE);
        }

        private static string OnMatch_Chance(FormulaData data, string clause)
        {
            string value = ClauseParseInput(clause, data);
            float chance = float.Parse(value);

            float percent = UnityEngine.Random.Range(0f, 1f);
            return (chance <= percent ? 1 : 0).ToString(CULTURE);
        }

        private static string OnMatch_Min(FormulaData data, string clause)
        {
            string[] parse = ClauseParse2Inputs(clause, data);
            int a = int.Parse(parse[0]);
            int b = int.Parse(parse[1]);
            return Mathf.Min(a, b).ToString(CULTURE);
        }

        private static string OnMatch_Max(FormulaData data, string clause)
        {
            string[] parse = ClauseParse2Inputs(clause, data);
            int a = int.Parse(parse[0]);
            int b = int.Parse(parse[1]);
            return Mathf.Max(a, b).ToString(CULTURE);
        }

        // CLAUSES: -------------------------------------------------------------------------------

        private static string ClauseParseName(string clause, FormulaData data)
        {
            int sIndex = clause.IndexOf('[');
            int eIndex = clause.IndexOf(']');

            string content = clause.Substring(sIndex + 1, eIndex - sIndex - 1);
            return Formula.ParseFormula(new StringBuilder(content), data);
        }

        private static string ClauseParseInput(string clause, FormulaData data)
        {
            int sIndex = clause.IndexOf('(');
            int eIndex = clause.IndexOf(')');

            string content = clause.Substring(sIndex + 1, eIndex - sIndex - 1);
            string value = Formula.ParseFormula(new StringBuilder(content), data);
            return ExpressionEvaluator.Evaluate(value).ToString();
        }

        private static string ClauseParseTarget(string clause)
        {
            int sIndex = clause.IndexOf(':');
            int eIndex = clause.IndexOf('[');
            return clause.Substring(sIndex + 1, eIndex - sIndex - 1);
        }

        private static string[] ClauseParse2Inputs(string clause, FormulaData data)
        {
            int sIndex1 = clause.IndexOf('(');
            int eIndex1 = clause.IndexOf(',');

            int sIndex2 = clause.IndexOf(',');
            int eIndex2 = clause.IndexOf(')');

            string value1 = clause.Substring(sIndex1 + 1, eIndex1 - sIndex1 - 1);
            string value2 = clause.Substring(sIndex2 + 1, eIndex2 - sIndex2 - 1);

            value1 = Formula.ParseFormula(new StringBuilder(value1), data);
            value2 = Formula.ParseFormula(new StringBuilder(value2), data);

            value1 = ExpressionEvaluator.Evaluate(value1).ToString();
            value2 = ExpressionEvaluator.Evaluate(value2).ToString();

            return new string[] { value1, value2 };
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public string formula = "this[value]";
        public Table table = new Table();

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public float Calculate(float value, Stats origin, Stats target)
        {
            StringBuilder sbFormula = new StringBuilder(this.formula.Replace(" ", string.Empty));
            if (string.IsNullOrEmpty(sbFormula.ToString())) return 0f;

            FormulaData data = new FormulaData(value, this.table, origin, target);

            string formulaResult = Formula.ParseFormula(sbFormula, data);
            return ExpressionEvaluator.Evaluate(formulaResult);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static string ParseFormula(StringBuilder sbFormula, FormulaData data)
        {
            for (int i = 0; i < OPERATIONS.Length; ++i)
            {
                bool matchSuccess = true;
                while (matchSuccess)
                {
                    Match match = OPERATIONS[i].regex.Match(sbFormula.ToString());
                    if (matchSuccess = match.Success)
                    {
                        string result = OPERATIONS[i].action.Invoke(data, match.Value);
                        sbFormula.Remove(match.Index, match.Length);
                        sbFormula.Insert(match.Index, result);
                    }
                }
            }

            return sbFormula.ToString();
        }
    }
}
