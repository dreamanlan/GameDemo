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
        public static void RegisterAiExpressions(DslCalculator calc)
        {
            if (calc == null)
                return;

            // Register AI need functions
            calc.Register("ai_need_chase", "ai_need_chase(objId, skillDist) - check if NPC needs to chase target",
                new ExpressionFactoryHelper<AiNeedChaseExp>());
            calc.Register("ai_need_keep_away", "ai_need_keep_away(objId, skillDist, ratio) - check if NPC needs to keep away",
                new ExpressionFactoryHelper<AiNeedKeepAwayExp>());
            calc.Register("ai_select_target", "ai_select_target(objId, dist) - select nearest enemy target",
                new ExpressionFactoryHelper<AiSelectTargetExp>());
            calc.Register("ai_get_target", "ai_get_target(objId) - get current target entity",
                new ExpressionFactoryHelper<AiGetTargetExp>());

            // Register AI command expressions (async)
            calc.Register("ai_chase", "ai_chase(objId, skillDist) - make NPC chase target",
                new ExpressionFactoryHelper<AiChaseExp>());
            calc.Register("ai_keep_away", "ai_keep_away(objId, skillDist, ratio) - make NPC keep away from target",
                new ExpressionFactoryHelper<AiKeepAwayExp>());
            calc.Register("ai_go_home", "ai_go_home(objId) - make NPC go back to home position",
                new ExpressionFactoryHelper<AiGohomeExp>());
            calc.Register("ai_rand_move", "ai_rand_move(objId, time, radius) - make NPC move randomly",
                new ExpressionFactoryHelper<AiRandMoveExp>());

            calc.Register("select", "select(...)from(...)[where(...)][orderby(...)][asc|desc] - LINQ-like query on collections",
                new ExpressionFactoryHelper<AiQueryExp>());
        }
    }
}
