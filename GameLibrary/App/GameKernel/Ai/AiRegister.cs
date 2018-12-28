using UnityEngine;
using System.Collections.Generic;
using System.Text;
using GameLibrary;
using StorySystem;

internal static class AiRegister
{
    internal static void Register()
    {
        StoryCommandManager.Instance.RegisterCommandFactory("ai_chase", new StoryCommandFactoryHelper<AiChase>());
        StoryCommandManager.Instance.RegisterCommandFactory("ai_keep_away", new StoryCommandFactoryHelper<AiKeepAway>());
        StoryCommandManager.Instance.RegisterCommandFactory("ai_go_home", new StoryCommandFactoryHelper<AiGohome>());
        StoryCommandManager.Instance.RegisterCommandFactory("ai_rand_move", new StoryCommandFactoryHelper<AiRandMove>());

        StoryValueManager.Instance.RegisterValueFactory("ai_need_chase", new StoryValueFactoryHelper<AiNeedChase>());
        StoryValueManager.Instance.RegisterValueFactory("ai_need_keep_away", new StoryValueFactoryHelper<AiNeedKeepAway>());
        StoryValueManager.Instance.RegisterValueFactory("ai_select_target", new StoryValueFactoryHelper<AiSelectTarget>());
        StoryValueManager.Instance.RegisterValueFactory("ai_get_target", new StoryValueFactoryHelper<AiGetTarget>());
        StoryValueManager.Instance.RegisterValueFactory("select", new StoryValueFactoryHelper<AiQuery>());
    }
}
