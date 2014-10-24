using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Core;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;

namespace UnityTest
{
    public class TestLine : UnitTestRendererLine, IComparable<TestLine>
    {
        public static Func<string, UnitTestResult> GetUnitTestResult;

        protected static GUIContent s_GUIOpenInEditor = new GUIContent("Open in editor");
        private readonly string m_ResultId;
        private readonly IList<string> m_Categories;

        public TestLine(TestMethod test, string resultId) : base(test)
        {
            m_RenderedName = test.Parent is ParameterizedMethodSuite ? test.TestName.Name : test.MethodName;
            m_ResultId = resultId;
            var c = new List<string>();
            foreach (string category in test.Categories)
                c.Add(category);
            foreach (string category in test.Parent.Categories)
                c.Add(category);
            if (test.Parent is ParameterizedMethodSuite) 
                foreach (string category in test.Parent.Parent.Categories)
                    c.Add(category);
            m_Categories = c; 
        }

        public UnitTestResult result
        {
            get { return GetUnitTestResult(m_ResultId); }
        }

        public int CompareTo(TestLine other)
        {
            return result.Id.CompareTo(other.result.Id);
        }

        protected override void DrawLine(bool isSelected, RenderingOptions options)
        {
            if (!IsVisible(options)) return;

            var tempColor = GUI.color;
            if (result.Executed && result.Outdated) GUI.color = new Color(1, 1, 1, 0.7f);

            var icon = result.Executed || result.IsIgnored || result.ResultState == TestResultState.NotRunnable
                       ? GuiHelper.GetIconForResult(result.ResultState)
                       : Icons.UnknownImg;
            if (m_Test.RunState == RunState.Ignored)
                icon = GuiHelper.GetIconForResult(TestResultState.Ignored);

            var guiContent = new GUIContent(m_RenderedName, icon, m_FullName);

            GUILayout.Space(10);
            var rect = GUILayoutUtility.GetRect(guiContent, EditorStyles.label, GUILayout.ExpandWidth(true) /*, GUILayout.MaxHeight (18)*/);

            OnLeftMouseButtonClick(rect);
            OnContextClick(rect);

            EditorGUI.LabelField(rect, guiContent, isSelected ? Styles.selectedLabel : Styles.label);

            if (result.Outdated) GUI.color = tempColor;
        }

        protected internal override TestResultState ? GetResult()
        {
            return result.ResultState;
        }

        protected internal override bool IsVisible(RenderingOptions options)
        {
            if (!string.IsNullOrEmpty(options.nameFilter) && !m_FullName.ToLower().Contains(options.nameFilter.ToLower()))
                return false;
            if (options.categories != null && options.categories.Length > 0 && !options.categories.Any(c => m_Categories.Contains(c)))
                return false;
            if (!options.showIgnored && (m_Test.RunState == RunState.Ignored || m_Test.RunState == RunState.Skipped))
                return false;
            if (!options.showFailed && (result.IsFailure || result.IsError || result.IsInconclusive))
                return false;
            if (!options.showNotRunned && !result.Executed)
                return false;
            if (!options.showSucceeded && result.IsSuccess)
                return false;
            return true;
        }

        public override string GetResultText()
        {
            var tempTest = result;
            var text = tempTest.Name;
            if (tempTest.Executed)
                text += " (" + tempTest.Duration.ToString("##0.###") + "s)";
            if (!tempTest.IsSuccess)
            {
                text += "\n";
                if (!string.IsNullOrEmpty(tempTest.Message))
                {
                    text += "---\n";
                    text += tempTest.Message.Trim();
                }
                if (!string.IsNullOrEmpty(tempTest.StackTrace))
                {
                    var stackTrace = StackTraceFilter.Filter(tempTest.StackTrace).Trim();
                    text += "\n---EXCEPTION---\n" + stackTrace;
                }
            }
            return text.Trim();
        }

        private void OnContextClick(Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
            {
                Event.current.Use();
                PrintTestContextMenu();
            }
        }

        private void PrintTestContextMenu()
        {
            var m = new GenericMenu();
            var multilineSelection = SelectedLines.Count() > 1;
            if (multilineSelection)
            {
                m.AddItem(s_GUIRunSelected,
                          false,
                          data => RunTests(SelectedLines.Select(line => (object)line.m_Test.TestName).ToArray()),
                          "");
            }
            if (!string.IsNullOrEmpty(m_FullName))
            {
                m.AddItem(s_GUIRun,
                          false,
                          data => RunTests(new[] { (object)m_Test.TestName }),
                          "");
            }
            if (!multilineSelection)
            {
                m.AddSeparator("");

                m.AddItem(s_GUIOpenInEditor,
                          false,
                          data => GuiHelper.OpenInEditor(result, false),
                          "");
            }
            m.ShowAsContext();
        }

        private void OnLeftMouseButtonClick(Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                OnSelect();
                if (Event.current.clickCount == 2 && SelectedLines.Count == 1)
                {
                    GuiHelper.OpenInEditor(result, true);
                }
            }
        }
    }
}
