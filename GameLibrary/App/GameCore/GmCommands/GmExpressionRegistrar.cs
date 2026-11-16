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
            registry.Register("set_max_effect", "set_max_effect(group_id, max_count) - set max effect count for resource system",
                new ExpressionFactoryHelper<SetMaxEffectExp>());
            registry.Register("set_debug", "set_debug(flag) - set debug mode",
                new ExpressionFactoryHelper<SetDebugExp>());
            registry.Register("editor_break", "editor_break() - trigger Unity Editor break",
                new ExpressionFactoryHelper<EditorBreakExp>());
            registry.Register("debug_break", "debug_break() - trigger debug break",
                new ExpressionFactoryHelper<DebugBreakExp>());
            registry.Register("alloc_memory", "alloc_memory(key, size) - allocate memory and store in global variable",
                new ExpressionFactoryHelper<AllocMemoryExp>());
            registry.Register("free_memory", "free_memory(key) - free memory from global variable",
                new ExpressionFactoryHelper<FreeMemoryExp>());
            registry.Register("consume_cpu", "consume_cpu(time_us) - consume CPU for testing",
                new ExpressionFactoryHelper<ConsumeCpuExp>());
        }
    }
}
