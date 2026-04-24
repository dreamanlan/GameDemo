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
        public static void RegisterGmExpressions(DslCalculatorApiRegistry registry)
        {
            if (registry == null)
                return;

            // Register GM expressions
            registry.Register("setmaxeffect", "setmaxeffect(group_id, max_count) - set max effect count for resource system",
                new ExpressionFactoryHelper<SetMaxEffectExp>());
            registry.Register("setdebug", "setdebug(flag) - set debug mode",
                new ExpressionFactoryHelper<SetDebugExp>());
            registry.Register("editorbreak", "editorbreak() - trigger Unity Editor break",
                new ExpressionFactoryHelper<EditorBreakExp>());
            registry.Register("debugbreak", "debugbreak() - trigger debug break",
                new ExpressionFactoryHelper<DebugBreakExp>());
            registry.Register("allocmemory", "allocmemory(key, size) - allocate memory and store in global variable",
                new ExpressionFactoryHelper<AllocMemoryExp>());
            registry.Register("freememory", "freememory(key) - free memory from global variable",
                new ExpressionFactoryHelper<FreeMemoryExp>());
            registry.Register("consumecpu", "consumecpu(time_us) - consume CPU for testing",
                new ExpressionFactoryHelper<ConsumeCpuExp>());
        }
    }
}
