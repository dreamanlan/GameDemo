using System;
using System.Collections.Generic;
using StoryScript;

namespace GameLibrary.GmCommands
{
    //---------------------------------------------------------------------------------------------------------------
    internal class SetMaxEffectCommand : SimpleStoryCommandBase<SetMaxEffectCommand, StoryValueParam<int, int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int, int> _params, long delta)
        {
            int v1 = _params.Param1Value;
            int v2 = _params.Param2Value;
            if (v2 > 0) {
                if (v1 < 0) {
                    for (int i = (int)PredefinedResourceGroup.PlayerSkillEffect; i < (int)PredefinedResourceGroup.MaxCount; ++i) {
                        ResourceSystem.Instance.SetGroupMaxCount(i, v2);
                    }
                } else {
                    ResourceSystem.Instance.SetGroupMaxCount(v1, v2);
                }
            }
            return false;
        }
    }
    internal class SetDebugCommand : SimpleStoryCommandBase<SetDebugCommand, StoryValueParam<int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int> _params, long delta)
        {
            int val = _params.Param1Value;
            GlobalVariables.Instance.IsDebug = val != 0;
            return false;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal class AllocMemoryCommand : SimpleStoryCommandBase<AllocMemoryCommand, StoryValueParam<string, int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, int> _params, long delta)
        {
            string key = _params.Param1Value;
            int size = _params.Param2Value;
            BoxedValue m = BoxedValue.From(new byte[size]);
            if (instance.GlobalVariables.ContainsKey(key)) {
                instance.GlobalVariables[key] = m;
            } else {
                instance.GlobalVariables.Add(key, m);
            }
            return false;
        }
    }
    internal class FreeMemoryCommand : SimpleStoryCommandBase<FreeMemoryCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            string key = _params.Param1Value;
            if (instance.GlobalVariables.ContainsKey(key)) {
                instance.GlobalVariables.Remove(key);
                GC.Collect();
            } else {
                GC.Collect();
            }
            return false;
        }
    }
    internal class ConsumeCpuCommand : SimpleStoryCommandBase<ConsumeCpuCommand, StoryValueParam<int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int> _params, long delta)
        {
            int time = _params.Param1Value;
            long startTime = TimeUtility.GetElapsedTimeUs();
            while (startTime + time > TimeUtility.GetElapsedTimeUs()) {
            }
            return false;
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------
}
