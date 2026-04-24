using System;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// Registers AI-specific story expressions with DslCalculator
    /// Call this method when initializing the AI story system
    /// </summary>
    public static class AiStoryExpressionRegistrar
    {
        public static void RegisterAiExpressions(DslCalculatorApiRegistry registry)
        {
            if (registry == null)
                return;

            // Register AI need functions
            registry.Register("ai_need_chase", "ai_need_chase(objId, skillDist) - check if NPC needs to chase target",
                new ExpressionFactoryHelper<AiNeedChaseExp>());
            registry.Register("ai_need_keep_away", "ai_need_keep_away(objId, skillDist, ratio) - check if NPC needs to keep away",
                new ExpressionFactoryHelper<AiNeedKeepAwayExp>());
            registry.Register("ai_select_target", "ai_select_target(objId, dist) - select nearest enemy target",
                new ExpressionFactoryHelper<AiSelectTargetExp>());
            registry.Register("ai_get_target", "ai_get_target(objId) - get current target entity",
                new ExpressionFactoryHelper<AiGetTargetExp>());

            // Register AI command expressions (async)
            registry.Register("ai_chase", "ai_chase(objId, skillDist) - make NPC chase target",
                new ExpressionFactoryHelper<AiChaseExp>());
            registry.Register("ai_keep_away", "ai_keep_away(objId, skillDist, ratio) - make NPC keep away from target",
                new ExpressionFactoryHelper<AiKeepAwayExp>());
            registry.Register("ai_go_home", "ai_go_home(objId) - make NPC go back to home position",
                new ExpressionFactoryHelper<AiGohomeExp>());
            registry.Register("ai_rand_move", "ai_rand_move(objId, time, radius) - make NPC move randomly",
                new ExpressionFactoryHelper<AiRandMoveExp>());

            registry.Register("select", "select(...)from(...)[where(...)][orderby(...)][asc|desc] - LINQ-like query on collections",
                new ExpressionFactoryHelper<AiQueryExp>());
        }
    }
}
