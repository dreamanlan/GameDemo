using System;
using StoryScript;
using StoryScript.DslExpression;
using GameLibrary.Story.StoryExpressions;

namespace GameLibrary.GmCommands
{
    /// <summary>
    /// Registers GM-specific story expressions with DslCalculator
    /// </summary>
    public static class GmExpressionRegistrar
    {
        public static void RegisterGmExpressions(DslCalculator calc)
        {
            if (calc == null)
                return;

            // Register GM expressions
            calc.Register("setmaxeffect", "setmaxeffect(group_id, max_count) - set max effect count for resource system",
                new ExpressionFactoryHelper<SetMaxEffectExp>());
            calc.Register("setdebug", "setdebug(flag) - set debug mode",
                new ExpressionFactoryHelper<SetDebugExp>());
            calc.Register("editorbreak", "editorbreak() - trigger Unity Editor break",
                new ExpressionFactoryHelper<EditorBreakExp>());
            calc.Register("debugbreak", "debugbreak() - trigger debug break",
                new ExpressionFactoryHelper<DebugBreakExp>());
            calc.Register("allocmemory", "allocmemory(key, size) - allocate memory and store in global variable",
                new ExpressionFactoryHelper<AllocMemoryExp>());
            calc.Register("freememory", "freememory(key) - free memory from global variable",
                new ExpressionFactoryHelper<FreeMemoryExp>());
            calc.Register("consumecpu", "consumecpu(time_us) - consume CPU for testing",
                new ExpressionFactoryHelper<ConsumeCpuExp>());
        }
    }
}
