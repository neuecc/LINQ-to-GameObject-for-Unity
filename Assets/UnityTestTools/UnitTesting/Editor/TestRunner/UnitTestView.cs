using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    [Serializable]
    public partial class UnitTestView : EditorWindow
    {
        private static UnitTestView s_Instance;
        private static readonly IUnitTestEngine k_TestEngine = new NUnitTestEngine();

        [SerializeField] private List<UnitTestResult> m_ResultList = new List<UnitTestResult>();
        [SerializeField] private string[] m_AvailableCategories;
        [SerializeField] private List<string> m_FoldMarkers = new List<string>();
        [SerializeField] private List<UnitTestRendererLine> m_SelectedLines = new List<UnitTestRendererLine>();
        UnitTestRendererLine m_TestLines;

        #region runner steering vars
        private Vector2 m_TestListScroll, m_TestInfoScroll;
        private float m_HorizontalSplitBarPosition = 200;
        private float m_VerticalSplitBarPosition = 300;
        #endregion

        private UnitTestsRunnerSettings m_Settings;

        #region GUI Contents
        private readonly GUIContent m_GUIRunSelectedTestsIcon = new GUIContent(Icons.RunImg, "Run selected tests");
        private readonly GUIContent m_GUIRunAllTestsIcon = new GUIContent(Icons.RunAllImg, "Run all tests");
        private readonly GUIContent m_GUIRerunFailedTestsIcon = new GUIContent(Icons.RunFailedImg, "Rerun failed tests");
        private readonly GUIContent m_GUIOptionButton = new GUIContent("Options", Icons.GearImg);
        private readonly GUIContent m_GUIHideButton = new GUIContent("Hide", Icons.GearImg);
        private readonly GUIContent m_GUIRunOnRecompile = new GUIContent("Run on recompile", "Run all tests after recompilation");
        private readonly GUIContent m_GUIShowDetailsBelowTests = new GUIContent("Show details below tests", "Show run details below test list");
        private readonly GUIContent m_GUIRunTestsOnNewScene = new GUIContent("Run tests on a new scene", "Run tests on a new scene");
        private readonly GUIContent m_GUIAutoSaveSceneBeforeRun = new GUIContent("Autosave scene", "The runner will automatically save the current scene changes before it starts");
        private readonly GUIContent m_GUIShowSucceededTests = new GUIContent("Succeeded", Icons.SuccessImg, "Show tests that succeeded");
        private readonly GUIContent m_GUIShowFailedTests = new GUIContent("Failed", Icons.FailImg, "Show tests that failed");
        private readonly GUIContent m_GUIShowIgnoredTests = new GUIContent("Ignored", Icons.IgnoreImg, "Show tests that are ignored");
        private readonly GUIContent m_GUIShowNotRunTests = new GUIContent("Not Run", Icons.UnknownImg, "Show tests that didn't run");
        #endregion

        public UnitTestView()
        {
            title = "Unit Tests Runner";
            m_ResultList.Clear();
        }

        public void OnEnable()
        {
            s_Instance = this;
            m_Settings = ProjectSettingsBase.Load<UnitTestsRunnerSettings>();
            RefreshTests();
            EnableBackgroundRunner(m_Settings.runOnRecompilation);
        }

        public void OnDestroy()
        {
            s_Instance = null;
            EnableBackgroundRunner(false);
        }

        public void Awake()
        {
            RefreshTests();
        }

        public void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            var layoutOptions = new[] { GUILayout.Width(32), GUILayout.Height(24) };
            if (GUILayout.Button(m_GUIRunAllTestsIcon, Styles.buttonLeft, layoutOptions))
            {
                RunTests();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button(m_GUIRunSelectedTestsIcon, Styles.buttonMid, layoutOptions))
            {
                m_TestLines.RunSelectedTests();
            }
            if (GUILayout.Button(m_GUIRerunFailedTestsIcon, Styles.buttonRight, layoutOptions))
            {
                m_TestLines.RunTests(m_ResultList.Where(result => result.IsFailure || result.IsError).Select(l => l.FullName).ToArray());
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(m_Settings.optionsFoldout ? m_GUIHideButton : m_GUIOptionButton, GUILayout.Height(24), GUILayout.Width(80)))
            {
                m_Settings.optionsFoldout = !m_Settings.optionsFoldout;
            }
            EditorGUILayout.EndHorizontal();

            if (m_Settings.optionsFoldout) DrawOptions();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Filter:", GUILayout.Width(35));
            m_Settings.testFilter = EditorGUILayout.TextField(m_Settings.testFilter, EditorStyles.textField);

            if (m_AvailableCategories != null && m_AvailableCategories.Length > 0)
                m_Settings.categoriesMask = EditorGUILayout.MaskField(m_Settings.categoriesMask, m_AvailableCategories, GUILayout.MaxWidth(90));

            if (GUILayout.Button(m_Settings.filtersFoldout ? "Hide" : "Advanced", GUILayout.Width(80), GUILayout.Height(15)))
                m_Settings.filtersFoldout = !m_Settings.filtersFoldout;
            EditorGUILayout.EndHorizontal();

            if (m_Settings.filtersFoldout)
                DrawFilters();

            if (m_Settings.horizontalSplit)
                EditorGUILayout.BeginVertical();
            else
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            RenderTestList();
            RenderTestInfo();

            if (m_Settings.horizontalSplit)
                EditorGUILayout.EndVertical();
            else
                EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private string[] GetSelectedCategories()
        {
            var selectedCategories = new List<string>();
            foreach (var availableCategory in m_AvailableCategories)
            {
                var idx = Array.FindIndex(m_AvailableCategories, a => a == availableCategory);
                var mask = 1 << idx;
                if ((m_Settings.categoriesMask & mask) != 0) selectedCategories.Add(availableCategory);
            }
            return selectedCategories.ToArray();
        }

        private void RenderTestList()
        {
            EditorGUILayout.BeginVertical(Styles.testList);
            m_TestListScroll = EditorGUILayout.BeginScrollView(m_TestListScroll,
                                                               GUILayout.ExpandWidth(true),
                                                               GUILayout.MaxWidth(2000));
            if (m_TestLines != null)
            {
                var options = new RenderingOptions();
                options.showSucceeded = m_Settings.showSucceeded;
                options.showFailed = m_Settings.showFailed;
                options.showIgnored = m_Settings.showIgnored;
                options.showNotRunned = m_Settings.showNotRun;
                options.nameFilter = m_Settings.testFilter;
                options.categories = GetSelectedCategories();

                if (m_TestLines.Render(options)) Repaint();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void RenderTestInfo()
        {
            var ctrlId = GUIUtility.GetControlID(FocusType.Passive);
            var rect = GUILayoutUtility.GetLastRect();
            if (m_Settings.horizontalSplit)
            {
                rect.y = rect.height + rect.y - 1;
                rect.height = 3;
            }
            else
            {
                rect.x = rect.width + rect.x - 1;
                rect.width = 3;
            }

            EditorGUIUtility.AddCursorRect(rect, m_Settings.horizontalSplit ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal);
            var e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (GUIUtility.hotControl == 0 && rect.Contains(e.mousePosition))
                        GUIUtility.hotControl = ctrlId;
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == ctrlId)
                    {
                        m_HorizontalSplitBarPosition -= e.delta.y;
                        if (m_HorizontalSplitBarPosition < 20) m_HorizontalSplitBarPosition = 20;
                        m_VerticalSplitBarPosition -= e.delta.x;
                        if (m_VerticalSplitBarPosition < 20) m_VerticalSplitBarPosition = 20;
                        Repaint();
                    }

                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == ctrlId)
                        GUIUtility.hotControl = 0;
                    break;
            }
            m_TestInfoScroll = EditorGUILayout.BeginScrollView(m_TestInfoScroll, m_Settings.horizontalSplit
                                                               ? GUILayout.MinHeight(m_HorizontalSplitBarPosition)
                                                               : GUILayout.Width(m_VerticalSplitBarPosition));

            var text = "";
            if (m_SelectedLines.Any())
            {
                text = m_SelectedLines.First().GetResultText();
            }
            EditorGUILayout.TextArea(text, Styles.info);

            EditorGUILayout.EndScrollView();
        }

        private void DrawFilters()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            m_Settings.showSucceeded = GUILayout.Toggle(m_Settings.showSucceeded, m_GUIShowSucceededTests, GUI.skin.FindStyle(GUI.skin.button.name + "left"), GUILayout.ExpandWidth(true));
            m_Settings.showFailed = GUILayout.Toggle(m_Settings.showFailed, m_GUIShowFailedTests, GUI.skin.FindStyle(GUI.skin.button.name + "mid"));
            m_Settings.showIgnored = GUILayout.Toggle(m_Settings.showIgnored, m_GUIShowIgnoredTests, GUI.skin.FindStyle(GUI.skin.button.name + "mid"));
            m_Settings.showNotRun = GUILayout.Toggle(m_Settings.showNotRun, m_GUIShowNotRunTests, GUI.skin.FindStyle(GUI.skin.button.name + "right"), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) m_Settings.Save();
        }

        private void DrawOptions()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginChangeCheck();
            m_Settings.runOnRecompilation = EditorGUILayout.Toggle(m_GUIRunOnRecompile, m_Settings.runOnRecompilation);
            if (EditorGUI.EndChangeCheck()) EnableBackgroundRunner(m_Settings.runOnRecompilation);

            m_Settings.runTestOnANewScene = EditorGUILayout.Toggle(m_GUIRunTestsOnNewScene, m_Settings.runTestOnANewScene);
            EditorGUI.BeginDisabledGroup(!m_Settings.runTestOnANewScene);
            m_Settings.autoSaveSceneBeforeRun = EditorGUILayout.Toggle(m_GUIAutoSaveSceneBeforeRun, m_Settings.autoSaveSceneBeforeRun);
            EditorGUI.EndDisabledGroup();
            m_Settings.horizontalSplit = EditorGUILayout.Toggle(m_GUIShowDetailsBelowTests, m_Settings.horizontalSplit);

            if (EditorGUI.EndChangeCheck())
            {
                m_Settings.Save();
            }
            EditorGUILayout.Space();
        }

        private void RefreshTests()
        {
            UnitTestResult[] newResults;
            m_TestLines = k_TestEngine.GetTests(out newResults, out m_AvailableCategories);

            foreach (var newResult in newResults)
            {
                var result = m_ResultList.Where(t => t.Test == newResult.Test && t.FullName == newResult.FullName).ToArray();
                if (result.Count() != 1) continue;
                newResult.Update(result.Single(), true);
            }

            UnitTestRendererLine.SelectedLines = m_SelectedLines;
            UnitTestRendererLine.RunTest = RunTests;
            GroupLine.FoldMarkers = m_FoldMarkers;
            TestLine.GetUnitTestResult = FindTestResult;

            m_ResultList = new List<UnitTestResult>(newResults);

            Repaint();
        }
    }
}
