using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StoryScript;
using StoryScript.DslExpression;

namespace GameLibrary.Story.StoryExpressions
{
    /// <summary>
    /// bindui(obj) { var(...); onevent(...); inputs(...); ... } - bind UI events and variables
    /// </summary>
    internal sealed class BindUiExp : AbstractExpression
    {
        public override bool IsAsync { get { return false; } }

        protected override BoxedValue DoCalc()
        {
            if (m_ObjExp == null)
                return BoxedValue.NullObject;

            GameObject obj = m_ObjExp.Calc().ObjectVal as GameObject;
            if (null == obj)
                return BoxedValue.NullObject;

            var storyInst = Calculator.GetFuncContext<StoryInstance>();
            if (null == storyInst)
                return BoxedValue.NullObject;

            UiStoryInitializer initer = obj.GetComponent<UiStoryInitializer>();
            if (null != initer)
            {
                ClientStorySystem.Instance.AddBindedStory(obj, storyInst);

                UiStoryEventHandler handler0 = obj.GetComponent<UiStoryEventHandler>();
                if (null == handler0)
                {
                    handler0 = obj.AddComponent<UiStoryEventHandler>();
                }
                if (null != handler0)
                {
                    // Set up variable bindings
                    for (int ix = 0; ix < m_VarInfos.Count; ++ix)
                    {
                        string name = m_VarInfos[ix].m_VarNameExp.Calc().ToString();
                        string path = m_VarInfos[ix].m_ControlPathExp.Calc().ToString();
                        GameObject ctrl = Utility.FindChildObjectByPath(obj, path);
                        if (null != ctrl)
                            AddVariable(storyInst, name, ctrl);
                    }

                    // Set window name
                    handler0.WindowName = initer.WindowName;

                    // Set up UI element references
                    SetupUiElements(obj, handler0);

                    // Set up event listeners
                    SetupEventListeners(obj, handler0);
                }
            }

            return BoxedValue.NullObject;
        }

        protected override bool Load(Dsl.StatementData statementData)
        {
            // Parse: bindui(obj) { ... }
            Dsl.FunctionData funcData = statementData.First.AsFunction;
            if (null != funcData)
            {
                Dsl.FunctionData callData = funcData;
                if (funcData.IsHighOrder)
                    callData = funcData.LowerOrderFunction;

                if (null != callData && callData.HaveParam())
                {
                    if (callData.GetParamNum() > 0)
                    {
                        m_ObjExp = Calculator.Load(callData.GetParam(0));
                    }
                }
            }

            // Parse block content
            for (int i = 1; i < statementData.GetFunctionNum(); ++i)
            {
                Dsl.FunctionData callData = statementData.Functions[i].AsFunction;
                if (null != callData)
                {
                    string id = callData.GetId();
                    if (id == "var")
                    {
                        LoadVar(callData);
                    }
                    else if (id == "onevent")
                    {
                        LoadEvent(callData);
                    }
                    else if (id == "inputs")
                    {
                        LoadPaths(m_Inputs, callData);
                    }
                    else if (id == "toggles")
                    {
                        LoadPaths(m_Toggles, callData);
                    }
                    else if (id == "sliders")
                    {
                        LoadPaths(m_Sliders, callData);
                    }
                    else if (id == "dropdowns")
                    {
                        LoadPaths(m_DropDowns, callData);
                    }
                }
            }

            return true;
        }

        protected override bool Load(Dsl.FunctionData funcData)
        {
            // Handle: bindui(obj) without block
            Dsl.FunctionData callData = funcData;
            if (funcData.IsHighOrder)
                callData = funcData.LowerOrderFunction;

            if (null != callData && callData.HaveParam())
            {
                if (callData.GetParamNum() > 0)
                {
                    m_ObjExp = Calculator.Load(callData.GetParam(0));
                }
            }

            // Also parse block content if present
            if (funcData.HaveStatement())
            {
                for (int i = 0; i < funcData.GetParamNum(); ++i)
                {
                    Dsl.FunctionData callData2 = funcData.GetParam(i) as Dsl.FunctionData;
                    if (null != callData2)
                    {
                        string id = callData2.GetId();
                        if (id == "var")
                        {
                            LoadVar(callData2);
                        }
                        else if (id == "onevent")
                        {
                            LoadEvent(callData2);
                        }
                        else if (id == "inputs")
                        {
                            LoadPaths(m_Inputs, callData2);
                        }
                        else if (id == "toggles")
                        {
                            LoadPaths(m_Toggles, callData2);
                        }
                        else if (id == "sliders")
                        {
                            LoadPaths(m_Sliders, callData2);
                        }
                        else if (id == "dropdowns")
                        {
                            LoadPaths(m_DropDowns, callData2);
                        }
                    }
                }
            }

            return true;
        }

        private void LoadVar(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2)
            {
                VarInfo info = new VarInfo();
                info.m_VarNameExp = Calculator.Load(callData.GetParam(0));
                info.m_ControlPathExp = Calculator.Load(callData.GetParam(1));
                m_VarInfos.Add(info);
            }
        }

        private void LoadEvent(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 3)
            {
                EventInfo info = new EventInfo();
                info.m_EventExp = Calculator.Load(callData.GetParam(0));
                info.m_TagExp = Calculator.Load(callData.GetParam(1));
                info.m_PathExp = Calculator.Load(callData.GetParam(2));
                m_EventInfos.Add(info);
            }
        }

        private void LoadPaths(List<IExpression> list, Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            for (int i = 0; i < num; ++i)
            {
                list.Add(Calculator.Load(callData.GetParam(i)));
            }
        }

        private void SetupUiElements(GameObject obj, UiStoryEventHandler handler0)
        {
            // Input fields
            for (int k = 0; k < m_Inputs.Count; ++k)
            {
                string path = m_Inputs[k].Calc().ToString();
                var comp = Utility.FindComponentInChildren<InputField>(obj, path);
                if (null != comp)
                    handler0.InputLabels.Add(comp);
            }

            // Toggles
            for (int k = 0; k < m_Toggles.Count; ++k)
            {
                string path = m_Toggles[k].Calc().ToString();
                var comp = Utility.FindComponentInChildren<Toggle>(obj, path);
                if (null != comp)
                    handler0.InputToggles.Add(comp);
            }

            // Sliders
            for (int k = 0; k < m_Sliders.Count; ++k)
            {
                string path = m_Sliders[k].Calc().ToString();
                var comp = Utility.FindComponentInChildren<Slider>(obj, path);
                if (null != comp)
                    handler0.InputSliders.Add(comp);
            }

            // Dropdowns
            for (int k = 0; k < m_DropDowns.Count; ++k)
            {
                string path = m_DropDowns[k].Calc().ToString();
                var comp = Utility.FindComponentInChildren<Dropdown>(obj, path);
                if (null != comp)
                    handler0.InputDropdowns.Add(comp);
            }
        }

        private void SetupEventListeners(GameObject obj, UiStoryEventHandler handler0)
        {
            for (int ix = 0; ix < m_EventInfos.Count; ++ix)
            {
                string evt = m_EventInfos[ix].m_EventExp.Calc().ToString();
                string tag = m_EventInfos[ix].m_TagExp.Calc().ToString();
                string path = m_EventInfos[ix].m_PathExp.Calc().ToString();

                if (evt == "button")
                {
                    Button button = Utility.FindComponentInChildren<Button>(obj, path);
                    if (null != button)
                        button.onClick.AddListener(() => { handler0.OnClickHandler(tag); });
                }
                else if (evt == "toggle")
                {
                    Toggle toggle = Utility.FindComponentInChildren<Toggle>(obj, path);
                    if (null != toggle)
                        toggle.onValueChanged.AddListener((bool val) => { handler0.OnToggleHandler(tag, val); });
                }
                else if (evt == "dropdown")
                {
                    Dropdown dropdown = Utility.FindComponentInChildren<Dropdown>(obj, path);
                    if (null != dropdown)
                        dropdown.onValueChanged.AddListener((int val) => { handler0.OnDropdownHandler(tag, val); });
                }
                else if (evt == "slider")
                {
                    Slider slider = Utility.FindComponentInChildren<Slider>(obj, path);
                    if (null != slider)
                        slider.onValueChanged.AddListener((float val) => { handler0.OnSliderHandler(tag, val); });
                }
                else if (evt == "input")
                {
                    InputField input = Utility.FindComponentInChildren<InputField>(obj, path);
                    if (null != input)
                        input.onEndEdit.AddListener((string val) => { handler0.OnInputHandler(tag, val); });
                }
            }
        }

        private void AddVariable(StoryInstance instance, string name, GameObject ctrl)
        {
            instance.SetVariable(name, BoxedValue.FromObject(ctrl));
            Text text = ctrl.GetComponent<Text>();
            if (null != text)
            {
                instance.SetVariable(string.Format("{0}_Text", name), BoxedValue.FromObject(text));
            }
            Image image = ctrl.GetComponent<Image>();
            if (null != image)
            {
                instance.SetVariable(string.Format("{0}_Image", name), BoxedValue.FromObject(image));
            }
            RawImage rawImage = ctrl.GetComponent<RawImage>();
            if (null != rawImage)
            {
                instance.SetVariable(string.Format("{0}_RawImage", name), BoxedValue.FromObject(rawImage));
            }
            Button button = ctrl.GetComponent<Button>();
            if (null != button)
            {
                instance.SetVariable(string.Format("{0}_Button", name), BoxedValue.FromObject(button));
            }
            Dropdown dropdown = ctrl.GetComponent<Dropdown>();
            if (null != dropdown)
            {
                instance.SetVariable(string.Format("{0}_Dropdown", name), BoxedValue.FromObject(dropdown));
            }
            InputField inputField = ctrl.GetComponent<InputField>();
            if (null != inputField)
            {
                instance.SetVariable(string.Format("{0}_Input", name), BoxedValue.FromObject(inputField));
            }
            Slider slider = ctrl.GetComponent<Slider>();
            if (null != slider)
            {
                instance.SetVariable(string.Format("{0}_Slider", name), BoxedValue.FromObject(slider));
            }
        }

        private class VarInfo
        {
            internal IExpression m_VarNameExp;
            internal IExpression m_ControlPathExp;
        }

        private class EventInfo
        {
            internal IExpression m_EventExp;
            internal IExpression m_TagExp;
            internal IExpression m_PathExp;
        }

        private IExpression m_ObjExp = null;
        private List<VarInfo> m_VarInfos = new List<VarInfo>();
        private List<EventInfo> m_EventInfos = new List<EventInfo>();
        private List<IExpression> m_Inputs = new List<IExpression>();
        private List<IExpression> m_Toggles = new List<IExpression>();
        private List<IExpression> m_Sliders = new List<IExpression>();
        private List<IExpression> m_DropDowns = new List<IExpression>();
    }
}
