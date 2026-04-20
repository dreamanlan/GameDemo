using System;
using System.Collections;
using System.Collections.Generic;
using GameLibrary;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// select(...) from(...) where(...) orderby(...) - SQL-like query expression
    /// Example: select($x) from($entities) where($x.hp > 0) orderby($x.dist)
    /// </summary>
    internal sealed class AiQueryExp : AbstractExpression
    {
        public override bool IsAsync { get { return false; } }

        protected override BoxedValue DoCalc()
        {
            if (m_Select == null || m_From == null)
                return BoxedValue.NullObject;

            BoxedValue fromVal = m_From.Calc();
            IEnumerable enumer = fromVal.ObjectVal as IEnumerable;

            if (enumer == null)
                return BoxedValue.NullObject;

            ArrayList coll = new ArrayList();

            var enumerator = enumer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var v = BoxedValue.FromObject(enumerator.Current);
                // Set iterator variable $$ so sub-expressions can reference it
                Calculator.SetVariable("$$", v);

                if (m_Where != null)
                {
                    BoxedValue whereVal = m_Where.Calc();
                    if (whereVal.GetInt() != 0)
                    {
                        AddRow(coll, v);
                    }
                }
                else
                {
                    AddRow(coll, v);
                }
            }

            // Sort with ORDERBY clause
            int ct = m_OrderBy.Count;
            if (ct > 0)
            {
                coll.Sort(new AiQueryComparer(m_Desc, ct));
            }

            // Get results
            ArrayList result = new ArrayList();
            for (int i = 0; i < coll.Count; ++i)
            {
                var ao = coll[i] as BoxedValueList;
                result.Add(ao[0]);
            }

            return BoxedValue.FromObject(result);
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            for (int i = 0; i < statementData.Functions.Count; ++i)
            {
                var vorf = statementData.Functions[i];
                var funcData = vorf.AsFunction;
                if (funcData != null)
                {
                    LoadCallData(funcData);
                }
                else {
                    var valueData = vorf.AsValue;
                    if (valueData != null) {
                        LoadValueData(valueData);
                    }
                }
            }
            return true;
        }

        private void LoadValueData(Dsl.ValueData valueData)
        {
            string id = valueData.GetId();
            if (id == "asc") {
                m_Desc = false;
            }
            else if (id == "desc") {
                m_Desc = true;
            }
        }
        private void LoadCallData(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            if (id == "select")
            {
                m_Select = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "from")
            {
                m_From = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "where")
            {
                m_Where = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "orderby")
            {
                for (int i = 0; i < callData.GetParamNum(); ++i)
                {
                    IExpression v = Calculator.Load(callData.GetParam(i));
                    m_OrderBy.Add(v);
                }
            }
            else if (id == "asc")
            {
                m_Desc = false;
            }
            else if (id == "desc")
            {
                m_Desc = true;
            }
        }

        private void AddRow(ArrayList coll, BoxedValue v)
        {
            BoxedValueList row = new BoxedValueList();
            coll.Add(row);

            // Evaluate SELECT with iterator set to v
            Calculator.SetVariable("$$", v);
            row.Add(m_Select.Calc());

            // Evaluate ORDERBY with iterator set to v
            for (int i = 0; i < m_OrderBy.Count; ++i)
            {
                Calculator.SetVariable("$$", v);
                row.Add(m_OrderBy[i].Calc());
            }
        }

        private IExpression m_Select = null;
        private IExpression m_From = null;
        private IExpression m_Where = null;
        private List<IExpression> m_OrderBy = new List<IExpression>();
        private bool m_Desc = false;
    }

    internal sealed class AiQueryComparer : IComparer
    {
        private bool m_Desc;
        private int m_ColumnCount;

        public AiQueryComparer(bool desc, int columnCount)
        {
            m_Desc = desc;
            m_ColumnCount = columnCount;
        }

        public int Compare(object x, object y)
        {
            var a = x as BoxedValueList;
            var b = y as BoxedValueList;
            if (a == null || b == null) return 0;

            // Column 0 is the SELECT result, columns 1..N are ORDERBY keys
            for (int i = 1; i < m_ColumnCount + 1 && i < a.Count && i < b.Count; ++i)
            {
                var va = a[i];
                var vb = b[i];
                int cmp = Comparer.Default.Compare(va.GetObject(), vb.GetObject());
                if (cmp != 0)
                {
                    return m_Desc ? -cmp : cmp;
                }
            }
            return 0;
        }
    }
}
