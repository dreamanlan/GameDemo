using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// destroygameobject(path_or_obj) - destroy a GameObject by path or reference
    /// </summary>
    internal sealed class DestroyGameObjectExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 1)
                throw new Exception("Expected: destroygameobject(path_or_obj)");

            GameObject obj = GameObjectExpHelpers.GetGameObject(operands[0]);
            if (null != obj)
            {
                obj.transform.SetParent(null);
                if (!ResourceSystem.Instance.RecycleObject(obj))
                {
                    SceneSystem.Instance.GameObjectsFromDsl.Remove(obj);
                    Utility.DestroyObject(obj);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// creategameobject(name, prefab[, parent])[obj("varname")]{
    ///     position(vector3(x,y,z));
    ///     rotation(vector3(x,y,z));
    ///     scale(vector3(x,y,z));
    ///     disable("typename", "typename", ...);
    ///     remove("typename", "typename", ...);
    /// };
    /// </summary>
    internal sealed class CreateGameObjectExp : AbstractExpression
    {
        public override bool IsAsync { get { return true; } }

        protected override IEnumerator DoCalc(AsyncCalcResult result)
        {
            var name = m_Name.Calc().ToString();
            var prefab = m_Prefab.Calc().ToString();

            var obj = ResourceSystem.Instance.NewObject(prefab) as GameObject;
            if (null != obj)
            {
                obj.name = name;

                // Wait one frame before setting parent and manipulating components
                yield return null;

                // parent
                if (m_HaveParent)
                {
                    var parentVal = m_Parent.Calc();
                    GameObject parent = GameObjectExpHelpers.GetGameObject(parentVal);
                    if (null != parent)
                    {
                        obj.transform.SetParent(parent.transform, false);
                    }
                }

                // position
                if (null != m_Position)
                {
                    Vector3Obj posObj = m_Position.Calc();
                    obj.transform.localPosition = posObj;
                }

                // rotation
                if (null != m_Rotation)
                {
                    Vector3Obj rotObj = m_Rotation.Calc();
                    obj.transform.localEulerAngles = rotObj;
                }

                // scale
                if (null != m_Scale)
                {
                    Vector3Obj scaleObj = m_Scale.Calc();
                    obj.transform.localScale = scaleObj;
                }

                // disable components
                if (m_DisableComponents != null)
                {
                    for (int i = 0; i < m_DisableComponents.Count; ++i)
                    {
                        var typeName = m_DisableComponents[i].Calc().ToString();
                        var type = Utility.GetType(typeName);
                        if (null != type)
                        {
                            var comps = obj.GetComponentsInChildren(type);
                            for (int j = 0; j < comps.Length; ++j)
                            {
                                var t = comps[j].GetType();
                                t.InvokeMember("enabled", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, comps[j], new object[] { false });
                            }
                        }
                    }
                }

                // remove components
                if (m_RemoveComponents != null)
                {
                    for (int i = 0; i < m_RemoveComponents.Count; ++i)
                    {
                        var typeName = m_RemoveComponents[i].Calc().ToString();
                        var type = Utility.GetType(typeName);
                        if (null != type)
                        {
                            var comps = obj.GetComponentsInChildren(type);
                            for (int j = 0; j < comps.Length; ++j)
                            {
                                Utility.DestroyObject(comps[j]);
                            }
                        }
                    }
                }

                // set obj variable
                if (m_HaveObj)
                {
                    string varName = m_ObjVarName.Calc().ToString();
                    var storyInst = Calculator.GetFuncContext<StoryInstance>();
                    if (null != storyInst)
                    {
                        storyInst.SetVariable(varName, obj);
                    }
                }

                result.Value = BoxedValue.FromObject(obj);
                yield break;
            }

            result.Value = BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            // Handle: creategameobject(name, prefab[, parent]) { ... }
            // Also handle: creategameobject(name, prefab[, parent])[obj("varname")] { ... }
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
            {
                callData = funcData.LowerOrderFunction;
                // Load obj("varname") from high-order function
                LoadVarName(funcData.LowerOrderFunction);
            }

            if (null != callData && callData.HaveParam())
            {
                int num = callData.GetParamNum();
                if (num > 1)
                {
                    m_Name = Calculator.Load(callData.GetParam(0));
                    m_Prefab = Calculator.Load(callData.GetParam(1));
                    if (num > 2)
                    {
                        m_HaveParent = true;
                        m_Parent = Calculator.Load(callData.GetParam(2));
                    }
                }
            }

            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd)
                    {
                        LoadOptional(cd);
                    }
                }
            }
            return true;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            // Handle: creategameobject(name, prefab)[obj("varname")] { ... }
            if (statementData.Functions.Count == 2)
            {
                Dsl.FunctionData first = statementData.First.AsFunction;
                Dsl.FunctionData second = statementData.Second.AsFunction;
                if (null != first && null != second)
                {
                    Load(first);
                    if (second.IsHighOrder)
                    {
                        LoadVarName(second.LowerOrderFunction);
                    }
                    else if (second.HaveParam())
                    {
                        LoadVarName(second);
                    }
                }
                if (null != second && second.HaveStatement())
                {
                    for (int i = 0; i < second.GetParamNum(); ++i)
                    {
                        Dsl.FunctionData cd = second.GetParam(i) as Dsl.FunctionData;
                        if (null != cd)
                        {
                            LoadOptional(cd);
                        }
                    }
                }
            }
            return true;
        }

        private void LoadVarName(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "obj" && callData.GetParamNum() == 1)
            {
                m_ObjVarName = Calculator.Load(callData.GetParam(0));
                m_HaveObj = true;
            }
        }

        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            int num = callData.GetParamNum();
            if (id == "position")
            {
                if (num == 3)
                    m_Position = Calculator.Load(callData);
                else if (num > 0)
                    m_Position = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "rotation")
            {
                if (num == 3)
                    m_Rotation = Calculator.Load(callData);
                else if (num > 0)
                    m_Rotation = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "scale")
            {
                if (num == 3)
                    m_Scale = Calculator.Load(callData);
                else if (num > 0)
                    m_Scale = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "disable")
            {
                if (m_DisableComponents == null)
                    m_DisableComponents = new List<IExpression>();
                for (int i = 0; i < num; ++i)
                {
                    m_DisableComponents.Add(Calculator.Load(callData.GetParam(i)));
                }
            }
            else if (id == "remove")
            {
                if (m_RemoveComponents == null)
                    m_RemoveComponents = new List<IExpression>();
                for (int i = 0; i < num; ++i)
                {
                    m_RemoveComponents.Add(Calculator.Load(callData.GetParam(i)));
                }
            }
        }

        private IExpression m_Name;
        private IExpression m_Prefab;
        private bool m_HaveParent = false;
        private IExpression m_Parent;
        private bool m_HaveObj = false;
        private IExpression m_ObjVarName;
        private IExpression m_Position;
        private IExpression m_Rotation;
        private IExpression m_Scale;
        private List<IExpression> m_DisableComponents;
        private List<IExpression> m_RemoveComponents;
    }

    /// <summary>
    /// setactive(path_or_obj, active) - set GameObject active state
    /// </summary>
    internal sealed class SetActiveExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setactive(path_or_obj, active)");

            var objVal = operands[0];
            int active = operands[1].GetInt();
            GameObject obj = GameObjectExpHelpers.GetGameObject(objVal);
            if (null == obj)
            {
                // Try as entity ID
                try
                {
                    int id = objVal.GetInt();
                    var view = SceneSystem.Instance.GetEntityViewById(id);
                    if (null != view)
                    {
                        view.Active = active != 0;
                    }
                }
                catch { }
            }
            if (null != obj)
            {
                obj.SetActive(active != 0);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// setvisible(path_or_obj, visible) - set GameObject visibility
    /// </summary>
    internal sealed class SetVisibleExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setvisible(path_or_obj, visible)");

            GameObject obj = GameObjectExpHelpers.GetGameObject(operands[0]);
            int visible = operands[1].GetInt();
            if (null != obj)
            {
                Renderer renderer = obj.GetComponentInChildren<Renderer>();
                if (null != renderer)
                {
                    renderer.enabled = visible != 0;
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// setparent(child, parent) - set GameObject parent
    /// </summary>
    internal sealed class SetParentExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setparent(child, parent)");

            GameObject child = GameObjectExpHelpers.GetGameObject(operands[0]);
            GameObject parent = GameObjectExpHelpers.GetGameObject(operands[1]);
            if (null != child && null != parent)
            {
                child.transform.SetParent(parent.transform, false);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// settransform(obj, local_or_world){
    ///     position(vector3(x,y,z));
    ///     rotation(vector3(x,y,z));
    ///     scale(vector3(x,y,z));
    /// };
    /// </summary>
    internal sealed class SetTransformExp : AbstractExpression
    {
        protected override BoxedValue DoCalc()
        {
            var objVal = m_Obj.Calc();
            int worldOrLocal = m_WorldOrLocal.Calc().GetInt();
            GameObject obj = GameObjectExpHelpers.GetGameObject(objVal);
            if (null == obj) return BoxedValue.NullObject;

            var entityInfo = SceneSystem.Instance.GetEntityByGameObject(obj);

            if (null != m_Position)
            {
                Vector3Obj position = m_Position.Calc();
                if (0 == worldOrLocal)
                {
                    obj.transform.localPosition = position;
                }
                else
                {
                    if (null != entityInfo)
                    {
                        entityInfo.GetMovementStateInfo().SetPosition(position);
                    }
                    else
                    {
                        obj.transform.position = position;
                    }
                }
            }
            if (null != m_Rotation)
            {
                Vector3Obj rotation = m_Rotation.Calc();
                if (0 == worldOrLocal)
                    obj.transform.localEulerAngles = rotation;
                else
                    obj.transform.eulerAngles = rotation;
            }
            if (null != m_Scale)
            {
                Vector3Obj scale = m_Scale.Calc();
                obj.transform.localScale = scale;
            }
            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            // Handle: settransform(obj, local_or_world) { position(); rotation(); scale(); }
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
                callData = funcData.LowerOrderFunction;

            if (null != callData && callData.HaveParam())
            {
                int num = callData.GetParamNum();
                if (num > 1)
                {
                    m_Obj = Calculator.Load(callData.GetParam(0));
                    m_WorldOrLocal = Calculator.Load(callData.GetParam(1));
                }
            }

            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd)
                    {
                        LoadOptional(cd);
                    }
                }
            }
            return true;
        }

        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            int num = callData.GetParamNum();
            if (id == "position")
            {
                if (num == 3)
                    m_Position = Calculator.Load(callData);
                else if (num > 0)
                    m_Position = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "rotation")
            {
                if (num == 3)
                    m_Rotation = Calculator.Load(callData);
                else if (num > 0)
                    m_Rotation = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "scale")
            {
                if (num == 3)
                    m_Scale = Calculator.Load(callData);
                else if (num > 0)
                    m_Scale = Calculator.Load(callData.GetParam(0));
            }
        }

        private IExpression m_Obj;
        private IExpression m_WorldOrLocal;
        private IExpression m_Position;
        private IExpression m_Rotation;
        private IExpression m_Scale;
    }

    /// <summary>
    /// setactorscale(objid, vector3(x,y,z)) - set actor scale
    /// </summary>
    internal sealed class SetActorScaleExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: setactorscale(objid, scale)");

            int objId = operands[0].GetInt();
            Vector3Obj scaleObj = operands[1];
            Vector3 scale = scaleObj;

            GameObject obj = SceneSystem.Instance.GetGameObject(objId);
            if (null != obj)
            {
                obj.transform.localScale = new UnityEngine.Vector3(scale.x, scale.y, scale.z);
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// sendmessage(obj, msg, arg1, arg2, ...) - send Unity message to GameObject
    /// </summary>
    internal sealed class SendMessageExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: sendmessage(obj, msg, ...)");

            GameObject obj = GameObjectExpHelpers.GetGameObject(operands[0]);
            string msg = operands[1].ToString();

            if (null != obj) {
                if (operands.Count > 2) {
                    object[] args = new object[operands.Count - 2];
                    for (int i = 0; i < args.Length; ++i) {
                        args[i] = operands[i + 2].GetObject();
                    }
                    if (args.Length == 1)
                        obj.SendMessage(msg, args[0], UnityEngine.SendMessageOptions.DontRequireReceiver);
                    else
                        obj.SendMessage(msg, args, UnityEngine.SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    obj.SendMessage(msg, UnityEngine.SendMessageOptions.DontRequireReceiver);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// sendmessagewithtag(tag, msg, arg1, arg2, ...) - send Unity message to GameObjects by tag
    /// </summary>
    internal sealed class SendMessageWithTagExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: sendmessagewithtag(tag, msg, ...)");

            string tag = operands[0].ToString();
            string msg = operands[1].ToString();

            var objs = GameObject.FindGameObjectsWithTag(tag);
            if (operands.Count > 2) {
                object[] args = new object[operands.Count - 2];
                for (int i = 0; i < args.Length; ++i) {
                    args[i] = operands[i + 2].GetObject();
                }
                if (args.Length == 1) {
                    foreach (var obj in objs) {
                        obj.SendMessage(msg, args[0], UnityEngine.SendMessageOptions.DontRequireReceiver);
                    }
                }
                else {
                    foreach (var obj in objs) {
                        obj.SendMessage(msg, args, UnityEngine.SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            else
            {
                foreach (var obj in objs)
                {
                    obj.SendMessage(msg, UnityEngine.SendMessageOptions.DontRequireReceiver);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// sendmessagewithgameobject(obj, msg, args...) - send Unity message to GameObject (supports both GameObject and object ID)
    /// </summary>
    internal sealed class SendMessageWithGameObjectExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: sendmessagewithgameobject(obj, msg, ...)");

            var objVal = operands[0];
            string msg = operands[1].ToString();

            UnityEngine.GameObject uobj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
            if (null == uobj) {
                try {
                    int objId = objVal.IsInteger ? objVal.GetInt() : -1;
                    uobj = SceneSystem.Instance.GetGameObject(objId);
                }
                catch {
                    uobj = null;
                }
            }

            if (null != uobj) {
                if (operands.Count > 2) {
                    object[] args = new object[operands.Count - 2];
                    for (int i = 0; i < args.Length; ++i) {
                        args[i] = operands[i + 2].GetObject();
                    }
                    if (args.Length == 1)
                        uobj.SendMessage(msg, args[0], UnityEngine.SendMessageOptions.DontRequireReceiver);
                    else
                        uobj.SendMessage(msg, args, UnityEngine.SendMessageOptions.DontRequireReceiver);
                }
                else {
                    uobj.SendMessage(msg, UnityEngine.SendMessageOptions.DontRequireReceiver);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// auturecycle(path_or_obj, enable) - add/remove GameObject from auto-recycle list
    /// </summary>
    internal sealed class AutoRecycleExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: auturecycle(path_or_obj, enable)");

            var objVal = operands[0];
            int enable = operands[1].GetInt();
            string objPath = objVal.IsString ? objVal.StringVal : null;
            GameObject obj = null;

            if (null != objPath)
            {
                obj = GameObject.Find(objPath);
            }
            else
            {
                obj = objVal.ObjectVal as GameObject;
                if (null == obj)
                {
                    try
                    {
                        int id = objVal.GetInt();
                        obj = SceneSystem.Instance.GetGameObject(id);
                    }
                    catch
                    {
                        obj = null;
                    }
                }
            }

            if (null != obj)
            {
                if (enable != 0)
                {
                    SceneSystem.Instance.GameObjectsFromDsl.Add(obj);
                }
                else
                {
                    SceneSystem.Instance.GameObjectsFromDsl.Remove(obj);
                }
            }
            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// addcomponent(obj, component_type) - add component to GameObject, returns the component
    /// </summary>
    internal sealed class AddComponentExp : AbstractExpression
    {
        protected override BoxedValue DoCalc()
        {
            var objVal = m_ObjPath.Calc();
            var componentTypeVal = m_ComponentType.Calc();

            GameObject obj = GameObjectExpHelpers.GetGameObject(objVal);
            if (null == obj)
                return BoxedValue.NullObject;

            Type componentType = componentTypeVal.ObjectVal as Type;
            if (null == componentType)
            {
                string typeName = componentTypeVal.ToString();
                if (!string.IsNullOrEmpty(typeName))
                {
                    componentType = Type.GetType(typeName);
                }
            }

            if (null != componentType)
            {
                var component = obj.AddComponent(componentType);
                if (m_HaveObj && null != component)
                {
                    string varName = m_ObjVarName.Calc().ToString();
                    var storyInst = Calculator.GetFuncContext<StoryInstance>();
                    if (null != storyInst)
                    {
                        storyInst.SetVariable(varName, component);
                    }
                }
                return BoxedValue.FromObject(component);
            }

            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
            {
                callData = funcData.LowerOrderFunction;
                LoadObj(funcData.LowerOrderFunction);
            }
            if (null != callData && callData.HaveParam())
            {
                m_ObjPath = Calculator.Load(callData.GetParam(0));
                m_ComponentType = Calculator.Load(callData.GetParam(1));
            }
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd)
                    {
                        LoadObj(cd);
                    }
                }
            }
            return true;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2)
            {
                Dsl.FunctionData first = statementData.First.AsFunction;
                Dsl.FunctionData second = statementData.Second.AsFunction;
                if (null != first && null != second)
                {
                    Load(first);
                    LoadObj(second);
                }
            }
            return true;
        }

        private void LoadObj(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "obj" && callData.GetParamNum() == 1)
            {
                m_ObjVarName = Calculator.Load(callData.GetParam(0));
                m_HaveObj = true;
            }
        }

        private IExpression m_ObjPath;
        private IExpression m_ComponentType;
        private bool m_HaveObj = false;
        private IExpression m_ObjVarName;
    }

    /// <summary>
    /// removecomponent(obj, component_type) - remove component from GameObject
    /// </summary>
    internal sealed class RemoveComponentExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count < 2)
                throw new Exception("Expected: removecomponent(obj, component_type)");

            GameObject obj = GameObjectExpHelpers.GetGameObject(operands[0]);
            if (null == obj)
                return BoxedValue.NullObject;

            Type componentType = operands[1].ObjectVal as Type;
            if (null == componentType)
            {
                string typeName = operands[1].ToString();
                if (!string.IsNullOrEmpty(typeName))
                {
                    componentType = Type.GetType(typeName);
                }
            }

            if (null != componentType)
            {
                var component = obj.GetComponent(componentType);
                if (null != component)
                {
                    UnityEngine.Object.Destroy(component);
                }
            }

            return BoxedValue.NullObject;
        }
    }

    /// <summary>
    /// addtransform(obj, local_or_world){position(v3);rotation(v3);scale(v3)} - add transform values to GameObject
    /// addtransform(obj, local_or_world){
    ///     position(vector3(x,y,z));
    ///     rotation(vector3(x,y,z));
    ///     scale(vector3(x,y,z));
    /// };
    /// </summary>
    internal sealed class AddTransformExp : AbstractExpression
    {
        protected override BoxedValue DoCalc()
        {
            var objVal = m_ObjPath.Calc();
            int worldOrLocal = m_LocalOrWorld.Calc().GetInt();

            GameObject obj = GameObjectExpHelpers.GetGameObject(objVal);
            if (null == obj) return BoxedValue.NullObject;

            var entityInfo = SceneSystem.Instance.GetEntityByGameObject(obj);
            if (m_Position != null)
            {
                var v = m_Position.Calc();
                Vector3Obj positionObj = v;
                Vector3 position = positionObj;
                if (0 == worldOrLocal)
                {
                    obj.transform.localPosition += new UnityEngine.Vector3(position.x, position.y, position.z);
                }
                else
                {
                    if (null != entityInfo)
                    {
                        var movementInfo = entityInfo.GetMovementStateInfo();
                        movementInfo.SetPosition(movementInfo.GetPosition3D() + new UnityEngine.Vector3(position.x, position.y, position.z));
                    }
                    else
                    {
                        obj.transform.position += new UnityEngine.Vector3(position.x, position.y, position.z);
                    }
                }
            }
            if (m_Rotation != null)
            {
                var v = m_Rotation.Calc();
                Vector3Obj rotationObj = v;
                Vector3 rotation = rotationObj;
                if (0 == worldOrLocal)
                    obj.transform.localEulerAngles += new UnityEngine.Vector3(rotation.x, rotation.y, rotation.z);
                else
                    obj.transform.eulerAngles += new UnityEngine.Vector3(rotation.x, rotation.y, rotation.z);
            }
            if (m_Scale != null)
            {
                var v = m_Scale.Calc();
                Vector3Obj scaleObj = v;
                Vector3 scale = scaleObj;
                obj.transform.localScale += new UnityEngine.Vector3(scale.x, scale.y, scale.z);
            }
            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
            {
                callData = funcData.LowerOrderFunction;
            }
            if (null != callData && callData.HaveParam())
            {
                m_ObjPath = Calculator.Load(callData.GetParam(0));
                m_LocalOrWorld = Calculator.Load(callData.GetParam(1));
            }
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd)
                    {
                        LoadOptional(cd);
                    }
                }
            }
            return true;
        }

        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            int num = callData.GetParamNum();
            if (id == "position")
            {
                if (num == 3)
                    m_Position = Calculator.Load(callData);
                else if (num > 0)
                    m_Position = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "rotation")
            {
                if (num == 3)
                    m_Rotation = Calculator.Load(callData);
                else if (num > 0)
                    m_Rotation = Calculator.Load(callData.GetParam(0));
            }
            else if (id == "scale")
            {
                if (num == 3)
                    m_Scale = Calculator.Load(callData);
                else if (num > 0)
                    m_Scale = Calculator.Load(callData.GetParam(0));
            }
        }

        private IExpression m_ObjPath;
        private IExpression m_LocalOrWorld;
        private IExpression m_Position;
        private IExpression m_Rotation;
        private IExpression m_Scale;
    }

    /// <summary>
    /// getgameobject(objid) {
    ///     disable("typename", "typename", ...);
    ///     remove("typename", "typename", ...);
    /// };
    /// </summary>
    internal sealed class GetGameObjectExp : AbstractExpression
    {
        protected override BoxedValue DoCalc()
        {
            var objVal = m_ObjPath.Calc();
            string objPath = objVal.IsString ? objVal.ToString() : null;

            List<string> disables = new List<string>();
            for (int i = 0; i < m_DisableComponents.Count; ++i)
            {
                disables.Add(m_DisableComponents[i].Calc().ToString());
            }
            List<string> removes = new List<string>();
            for (int i = 0; i < m_RemoveComponents.Count; ++i)
            {
                removes.Add(m_RemoveComponents[i].Calc().ToString());
            }

            GameObject obj = null;
            if (null != objPath)
            {
                obj = UnityEngine.GameObject.Find(objPath);
            }
            else
            {
                try
                {
                    int objId = objVal.GetInt();
                    obj = SceneSystem.Instance.GetGameObject(objId);
                }
                catch
                {
                    obj = null;
                }
            }

            if (null != obj)
            {
                foreach (string disable in disables)
                {
                    var type = Utility.GetType(disable);
                    if (null != type)
                    {
                        var comps = obj.GetComponentsInChildren(type);
                        for (int i = 0; i < comps.Length; ++i)
                        {
                            var t = comps[i].GetType();
                            t.InvokeMember("enabled", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, comps[i], new object[] { false });
                        }
                    }
                }
                foreach (string remove in removes)
                {
                    var type = Utility.GetType(remove);
                    if (null != type)
                    {
                        var comps = obj.GetComponentsInChildren(type);
                        for (int i = 0; i < comps.Length; ++i)
                        {
                            Utility.DestroyObject(comps[i]);
                        }
                    }
                }
            }

            return BoxedValue.FromObject(obj);
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
            {
                callData = funcData.LowerOrderFunction;
            }

            if (null != callData && callData.HaveParam())
            {
                m_ObjPath = Calculator.Load(callData.GetParam(0));
            }

            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData cd = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != cd)
                    {
                        LoadOptional(cd);
                    }
                }
            }
            return true;
        }

        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            if (id == "disable")
            {
                for (int i = 0; i < callData.GetParamNum(); ++i)
                {
                    m_DisableComponents.Add(Calculator.Load(callData.GetParam(i)));
                }
            }
            else if (id == "remove")
            {
                for (int i = 0; i < callData.GetParamNum(); ++i)
                {
                    m_RemoveComponents.Add(Calculator.Load(callData.GetParam(i)));
                }
            }
        }

        private IExpression m_ObjPath;
        private List<IExpression> m_DisableComponents = new List<IExpression>();
        private List<IExpression> m_RemoveComponents = new List<IExpression>();
    }

    internal static class GameObjectExpHelpers
    {
        internal static GameObject GetGameObject(BoxedValue val)
        {
            string path = val.IsString ? val.StringVal : null;
            if (null != path)
            {
                return GameObject.Find(path);
            }
            else
            {
                var obj = val.ObjectVal as GameObject;
                if (null == obj)
                {
                    try
                    {
                        int id = val.GetInt();
                        obj = SceneSystem.Instance.GetGameObject(id);
                    }
                    catch { }
                }
                return obj;
            }
        }
    }
}
