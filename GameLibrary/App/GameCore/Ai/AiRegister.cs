using UnityEngine;
using System.Collections.Generic;
using System.Text;
using GameLibrary;
using StoryScript;

internal static class AiRegister
{
    internal static void Register()
    {
        StoryCommandManager.Instance.RegisterCommandFactory("ai_chase", "ai_chase command", new StoryCommandFactoryHelper<AiChase>());
        StoryCommandManager.Instance.RegisterCommandFactory("ai_keep_away", "ai_keep_away command", new StoryCommandFactoryHelper<AiKeepAway>());
        StoryCommandManager.Instance.RegisterCommandFactory("ai_go_home", "ai_go_home command", new StoryCommandFactoryHelper<AiGohome>());
        StoryCommandManager.Instance.RegisterCommandFactory("ai_rand_move", "ai_rand_move command", new StoryCommandFactoryHelper<AiRandMove>());

        StoryFunctionManager.Instance.RegisterFunctionFactory("ai_need_chase", "ai_need_chase function", new StoryFunctionFactoryHelper<AiNeedChase>());
        StoryFunctionManager.Instance.RegisterFunctionFactory("ai_need_keep_away", "ai_need_keep_away function", new StoryFunctionFactoryHelper<AiNeedKeepAway>());
        StoryFunctionManager.Instance.RegisterFunctionFactory("ai_select_target", "ai_select_target function", new StoryFunctionFactoryHelper<AiSelectTarget>());
        StoryFunctionManager.Instance.RegisterFunctionFactory("ai_get_target", "ai_get_target function", new StoryFunctionFactoryHelper<AiGetTarget>());
        StoryFunctionManager.Instance.RegisterFunctionFactory("select", "select function", new StoryFunctionFactoryHelper<AiQuery>());
    }
}
