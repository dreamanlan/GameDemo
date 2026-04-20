using System;
using System.Collections.Generic;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// setmaxeffect(group_id, max_count) - set max effect count for resource system
    /// </summary>
    internal sealed class SetMaxEffectExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setmaxeffect(group_id, max_count)");

            int groupId = operands[0].GetInt();
            int maxCount = operands[1].GetInt();

            if (maxCount > 0)
            {
                if (groupId < 0)
                {
                    for (int i = (int)PredefinedResourceGroup.PlayerSkillEffect; i < (int)PredefinedResourceGroup.MaxCount; ++i)
                    {
                        ResourceSystem.Instance.SetGroupMaxCount(i, maxCount);
                    }
                }
                else
                {
                    ResourceSystem.Instance.SetGroupMaxCount(groupId, maxCount);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// setdebug(flag) - set debug mode
    /// </summary>
    internal sealed class SetDebugExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: setdebug(flag)");

            int val = operands[0].GetInt();
            GlobalVariables.Instance.IsDebug = val != 0;
            StoryConfigManager.Instance.IsDebug = val != 0;
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// editorbreak() - trigger Unity Editor break
    /// </summary>
    internal sealed class EditorBreakExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            UnityEngine.Debug.Break();
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// debugbreak() - trigger debug break
    /// </summary>
    internal sealed class DebugBreakExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            UnityEngine.Debug.DebugBreak();
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// allocmemory(key, size) - allocate memory and store in global variable
    /// </summary>
    internal sealed class AllocMemoryExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: allocmemory(key, size)");

            string key = operands[0].ToString();
            int size = operands[1].GetInt();

            var storyInst = Calculator.GetFuncContext<StoryInstance>();
            if (storyInst != null)
            {
                BoxedValue m = BoxedValue.FromObject(new byte[size]);
                var globals = storyInst.ContextVariables;
                if (globals.ContainsKey(key))
                {
                    globals[key] = m;
                }
                else
                {
                    globals.Add(key, m);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// freememory(key) - free memory from global variable
    /// </summary>
    internal sealed class FreeMemoryExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: freememory(key)");

            string key = operands[0].ToString();

            var storyInst = Calculator.GetFuncContext<StoryInstance>();
            if (storyInst != null)
            {
                var globals = storyInst.ContextVariables;
                if (globals.ContainsKey(key))
                {
                    globals.Remove(key);
                    GC.Collect();
                }
                else
                {
                    GC.Collect();
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// consumecpu(time_us) - consume CPU for testing
    /// </summary>
    internal sealed class ConsumeCpuExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: consumecpu(time_us)");

            int time = operands[0].GetInt();
            long startTime = TimeUtility.GetElapsedTimeUs();
            while (startTime + (long)time > TimeUtility.GetElapsedTimeUs())
            {
            }
            return BoxedValue.NullObject;
        }
    }
}
