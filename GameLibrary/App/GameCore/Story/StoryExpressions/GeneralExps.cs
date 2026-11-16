using System;
using System.Collections;
using System.Collections.Generic;
using StoryScript;
using StoryScript.DslExpression;
using UnityEngine;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// preload(dslfile1, dslfile2, ...) - preload story DSL files
    /// </summary>
    internal sealed class PreloadExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: preload(dslfile1, dslfile2, ...)");

            List<string> dslFiles = new List<string>();
            for (int i = 0; i < operands.Count; ++i)
            {
                dslFiles.Add(operands[i].ToString());
            }
            ClientStorySystem.Instance.LoadSceneStories(dslFiles.ToArray());
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// suspend_all_message_handler(msgid1, msgid2, ...) - suspend message handlers
    /// </summary>
    internal sealed class SuspendAllMessageHandlerExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: suspend_all_message_handler(msgid1, msgid2, ...)");

            for (int i = 0; i < operands.Count; ++i)
            {
                string msgId = operands[i].ToString();
                ClientStorySystem.Instance.SuspendMessageHandler(msgId, true);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// resume_all_message_handler(msgid1, msgid2, ...) - resume message handlers
    /// </summary>
    internal sealed class ResumeAllMessageHandlerExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: resume_all_message_handler(msgid1, msgid2, ...)");

            for (int i = 0; i < operands.Count; ++i)
            {
                string msgId = operands[i].ToString();
                ClientStorySystem.Instance.SuspendMessageHandler(msgId, false);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// send_ai_message(objid, msg, arg1, arg2, ...) - send AI message to object
    /// </summary>
    internal sealed class SendAiMessageExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: send_ai_message(objid, msg, ...)");

            int objId = operands[0].GetInt();
            string msg = operands[1].ToString();

            var entity = SceneSystem.Instance.GetEntityById(objId);
            if (null != entity) {
                BoxedValueList args = entity.GetAiStateInfo().NewBoxedValueList();
                for (int i = 2; i < operands.Count; ++i) {
                    args.Add(operands[i]);
                }
                entity.GetAiStateInfo().SendMessage(msg, args);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// send_ai_concurrent_message(objid, msg, arg1, arg2, ...) - send concurrent AI message to object
    /// </summary>
    internal sealed class SendAiConcurrentMessageExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: send_ai_concurrent_message(objid, msg, ...)");

            int objId = operands[0].GetInt();
            string msg = operands[1].ToString();

            var entity = SceneSystem.Instance.GetEntityById(objId);
            if (null != entity) {
                BoxedValueList args = entity.GetAiStateInfo().NewBoxedValueList();
                for (int i = 2; i < operands.Count; ++i) {
                    args.Add(operands[i]);
                }
                entity.GetAiStateInfo().SendConcurrentMessage(msg, args);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// send_ai_namespaced_message(objid, msg, arg1, arg2, ...) - send namespaced AI message to object
    /// </summary>
    internal sealed class SendAiNamespacedMessageExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: send_ai_namespaced_message(objid, msg, ...)");

            int objId = operands[0].GetInt();
            string msg = operands[1].ToString();

            var entity = SceneSystem.Instance.GetEntityById(objId);
            if (null != entity)
            {
                BoxedValueList args = entity.GetAiStateInfo().NewBoxedValueList();
                for (int i = 2; i < operands.Count; ++i)
                {
                    args.Add(operands[i]);
                }
                entity.GetAiStateInfo().SendNamespacedMessage(msg, args);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// send_ai_concurrent_namespaced_message(objid, msg, arg1, arg2, ...) - send concurrent namespaced AI message to object
    /// </summary>
    internal sealed class SendAiConcurrentNamespacedMessageExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: send_ai_concurrent_namespaced_message(objid, msg, ...)");

            int objId = operands[0].GetInt();
            string msg = operands[1].ToString();

            var entity = SceneSystem.Instance.GetEntityById(objId);
            if (null != entity) {
                BoxedValueList args = entity.GetAiStateInfo().NewBoxedValueList();
                for (int i = 2; i < operands.Count; ++i) {
                    args.Add(operands[i]);
                }
                entity.GetAiStateInfo().SendConcurrentNamespacedMessage(msg, args);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// send_script_message(msg, arg1, arg2, ...) - send a script message
    /// </summary>
    internal sealed class SendScriptMessageExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: send_script_message(msg, ...)");

            string msg = operands[0].ToString();
            ArrayList arglist = new ArrayList();
            for (int i = 1; i < operands.Count; ++i)
            {
                arglist.Add(operands[i].GetObject());
            }
            object[] args = arglist.ToArray();
            if (args.Length == 0)
                Utility.SendScriptMessage(msg, null);
            else if (args.Length == 1)
                Utility.SendScriptMessage(msg, args[0]);
            else
                Utility.SendScriptMessage(msg, args);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// fire_message(msg, arg1, arg2, ...) - send a sequential message to the story system
    /// </summary>
    internal sealed class FireMessageExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: fire_message(msg, ...)");

            string msgId = operands[0].ToString();
            BoxedValueList args = ClientStorySystem.Instance.NewBoxedValueList();
            for (int i = 1; i < operands.Count; ++i)
            {
                args.Add(operands[i]);
            }
            ClientStorySystem.Instance.SendMessage(msgId, args);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// fire_concurrent_message(msg, arg1, arg2, ...) - send a concurrent message to the story system
    /// </summary>
    internal sealed class FireConcurrentMessageExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: fire_concurrent_message(msg, ...)");

            string msgId = operands[0].ToString();
            BoxedValueList args = ClientStorySystem.Instance.NewBoxedValueList();
            for (int i = 1; i < operands.Count; ++i)
            {
                args.Add(operands[i]);
            }
            ClientStorySystem.Instance.SendConcurrentMessage(msgId, args);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// publish_event(ev_name, group, arg1, arg2, ...) - publish an event
    /// </summary>
    internal sealed class PublishEventExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: publish_event(ev_name, group, ...)");

            string evName = operands[0].ToString();
            string group = operands[1].ToString();

            ArrayList arglist = new ArrayList();
            for (int i = 2; i < operands.Count; ++i)
            {
                arglist.Add(operands[i].GetObject());
            }
            object[] args = arglist.ToArray();
            Utility.EventSystem.Publish(evName, group, args);

            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// get_time() - get Unity game time
    /// </summary>
    internal sealed class GetTimeExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            return UnityEngine.Time.time;
        }
    }

    /// <summary>
    /// get_time_scale() - get Unity time scale
    /// </summary>
    internal sealed class GetTimeScaleExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            return UnityEngine.Time.timeScale;
        }
    }

    /// <summary>
    /// is_active(obj) - check if GameObject is active (activeSelf)
    /// </summary>
    internal sealed class IsActiveExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: is_active(obj)");

            GameObject obj = GeneralExpsHelper.ResolveGameObject(operands[0]);
            if (obj != null)
                return obj.activeSelf ? 1 : 0;
            return 0;
        }
    }

    /// <summary>
    /// is_really_active(obj) - check if GameObject is active in hierarchy
    /// </summary>
    internal sealed class IsReallyActiveExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: is_really_active(obj)");

            GameObject obj = GeneralExpsHelper.ResolveGameObject(operands[0]);
            if (obj != null)
                return obj.activeInHierarchy ? 1 : 0;
            return 0;
        }
    }

    /// <summary>
    /// is_visible(obj) - check if object's renderer is visible
    /// </summary>
    internal sealed class IsVisibleExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: is_visible(obj)");

            GameObject obj = GeneralExpsHelper.ResolveGameObject(operands[0]);
            if (obj != null)
            {
                var renderer = obj.GetComponent<UnityEngine.Renderer>();
                if (renderer != null)
                    return renderer.isVisible ? 1 : 0;
            }
            return 0;
        }
    }

    /// <summary>
    /// get_parent(obj) - get parent GameObject
    /// </summary>
    internal sealed class GetParentExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: get_parent(obj)");

            GameObject obj = GeneralExpsHelper.ResolveGameObject(operands[0]);
            if (obj != null && obj.transform.parent != null)
                return BoxedValue.FromObject(obj.transform.parent.gameObject);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// get_unity_ui_type(name) - get UnityEngine.UI type by name
    /// </summary>
    internal sealed class GetUnityUiTypeExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: get_unity_ui_type(name)");

            string name = operands[0].ToString();
            Type type = Type.GetType("UnityEngine.UI." + name + ",UnityEngine.UI");
            if (type != null)
                return BoxedValue.FromObject(type);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// get_user_type(name) - get user type by name
    /// </summary>
    internal sealed class GetUserTypeExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: get_user_type(name)");

            string name = operands[0].ToString();
            Type type = Type.GetType(name + ",Assembly-CSharp");
            if (type != null)
                return BoxedValue.FromObject(type);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// get_entity_info(objid) - get entity info by object ID
    /// </summary>
    internal sealed class GetEntityInfoExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: get_entity_info(objid)");

            int objId = operands[0].GetInt();
            var entity = SceneSystem.Instance.GetEntityById(objId);
            return entity != null ? BoxedValue.FromObject(entity) : BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// get_entity_view(viewid) - get entity view by view ID
    /// </summary>
    internal sealed class GetEntityViewExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: get_entity_view(viewid)");

            int viewId = operands[0].GetInt();
            var view = SceneSystem.Instance.GetEntityViewById(viewId);
            return view != null ? BoxedValue.FromObject(view) : BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// global() - get GlobalVariables instance
    /// </summary>
    internal sealed class GlobalExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            return BoxedValue.FromObject(GlobalVariables.Instance);
        }
    }

    /// <summary>
    /// scene() - get SceneSystem instance
    /// </summary>
    internal sealed class SceneExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            return BoxedValue.FromObject(SceneSystem.Instance);
        }
    }

    /// <summary>
    /// resource_system() - get ResourceSystem instance
    /// </summary>
    internal sealed class ResourceSystemExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            return BoxedValue.FromObject(ResourceSystem.Instance);
        }
    }

    /// <summary>
    /// get_story(story_id[, namespace]) - get story instance
    /// </summary>
    internal sealed class GetStoryExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: get_story(story_id[, namespace])");

            string storyId = operands[0].ToString();
            string ns = operands.Count > 1 ? operands[1].ToString() : "";
            var story = ClientStorySystem.Instance.GetStory(storyId, ns);
            return story != null ? BoxedValue.FromObject(story) : BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// get_story_variable(story_id, var_name[, namespace]) - get variable from another story
    /// </summary>
    internal sealed class GetStoryVariableExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: get_story_variable(story_id, var_name[, namespace])");

            string storyId = operands[0].ToString();
            string varName = operands[1].ToString();
            string ns = operands.Count > 2 ? operands[2].ToString() : "";
            var story = ClientStorySystem.Instance.GetStory(storyId, ns);
            if (story != null && story.TryGetVariable(varName, out BoxedValue val))
                return val;
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// deg_2_rad(degree) - convert degrees to radians
    /// </summary>
    internal sealed class Deg2RadExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: deg_2_rad(degree)");

            float degree = operands[0].GetFloat();
            return Geometry.DegreeToRadian(degree);
        }
    }

    /// <summary>
    /// rad_2_deg(radian) - convert radians to degrees
    /// </summary>
    internal sealed class Rad2DegExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: rad_2_deg(radian)");

            float radian = operands[0].GetFloat();
            return Geometry.RadianToDegree(radian);
        }
    }

    /// <summary>
    /// get_unity_type(typeName) - get Unity type by name
    /// </summary>
    internal sealed class GetUnityTypeExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: get_unity_type(typeName)");

            string typeName = operands[0].ToString();
            Type type = Type.GetType("UnityEngine." + typeName + ",UnityEngine");
            if (type != null)
                return BoxedValue.FromObject(type);
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// get_persistent_path() - get persistent data path
    /// </summary>
    internal sealed class GetPersistentPathExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            return UnityEngine.Application.persistentDataPath;
        }
    }

    /// <summary>
    /// get_streaming_assets() - get streaming assets path
    /// </summary>
    internal sealed class GetStreamingAssetsExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            return UnityEngine.Application.streamingAssetsPath;
        }
    }

    /// <summary>
    /// read_file(path) - read file content
    /// </summary>
    internal sealed class ReadFileExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: read_file(path)");

            string path = operands[0].ToString();
            if (System.IO.File.Exists(path))
            {
                string content = System.IO.File.ReadAllText(path);
                return BoxedValue.FromObject(content);
            }
            return BoxedValue.FromObject(string.Empty);
        }
    }

    /// <summary>
    /// is_story_skipped() - check if story is skipped
    /// </summary>
    internal sealed class IsStorySkippedExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            return StoryConfigManager.Instance.IsStorySkipped;
        }
    }

    /// <summary>
    /// get_child(obj, path) - get child GameObject by path
    /// </summary>
    internal sealed class GetChildExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: get_child(obj, path)");

            GameObject obj = null;
            var objVal = operands[0];
            if (objVal.IsObject)
                obj = objVal.ObjectVal as GameObject;
            else if (objVal.IsString)
                obj = GameObject.Find(objVal.StringVal);

            string path = operands[1].ToString();
            if (obj != null && !string.IsNullOrEmpty(path))
            {
                Transform child = obj.transform.Find(path);
                if (child != null)
                    return BoxedValue.FromObject(child.gameObject);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// count_npc(campId) - count NPCs by camp ID
    /// </summary>
    internal sealed class CountNpcExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calculator, IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: count_npc(campId)");

            int campId = operands[0].GetInt();
            if (operands.Count > 1) {
                CharacterRelation relation = (CharacterRelation)operands[1].GetInt();
                return SceneSystem.Instance.GetNpcCountByRelationWithCamp(campId, relation);
            }
            return SceneSystem.Instance.GetNpcCountByCamp(campId);
        }
    }

    internal static class GeneralExpsHelper
    {
        public static GameObject ResolveGameObject(BoxedValue val)
        {
            if (val.IsObject)
                return val.ObjectVal as GameObject;
            if (val.IsInteger)
            {
                int objId = val.GetInt();
                var entity = SceneSystem.Instance.GetEntityById(objId);
                if (entity != null)
                    return entity.View?.Actor;
            }
            if (val.IsString)
                return GameObject.Find(val.StringVal);
            return null;
        }
    }
}
