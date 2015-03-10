using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityTest.IntegrationTestRunner;

namespace UnityTest
{
    [Serializable]
    public class IntegrationTestsRunnerWindow : EditorWindow, IHasCustomMenu
    {
        #region GUI Contents
        private readonly GUIContent m_GUICreateNewTest = new GUIContent("Create", "Create new test");
        private readonly GUIContent m_GUIRunSelectedTests = new GUIContent("Run Selected", "Run selected test(s)");
        private readonly GUIContent m_GUIRunAllTests = new GUIContent("Run All", "Run all tests");
        private readonly GUIContent m_GUIAddGoUderTest = new GUIContent("Add GOs under test", "Add new GameObject under selected test");
        private readonly GUIContent m_GUIBlockUI = new GUIContent("Block UI when running", "Block UI when running tests");
        #endregion

        #region runner steerign vars
        private static IntegrationTestsRunnerWindow s_Instance;
        [SerializeField] private List<GameObject> m_TestsToRun;
        [SerializeField] private List<string> m_DynamicTestsToRun;
        [SerializeField] private bool m_ReadyToRun;
        private bool m_IsBuilding;
        public static bool selectedInHierarchy;
        private float m_HorizontalSplitBarPosition = 200;
        private Vector2 m_TestInfoScroll, m_TestListScroll;
        private IntegrationTestRendererBase[] m_TestLines;
        private string m_CurrectSceneName;
        private TestFilterSettings m_FilterSettings;

        [SerializeField] private GameObject m_SelectedLine;
        [SerializeField] private List<TestResult> m_ResultList = new List<TestResult>();
        [SerializeField] private List<GameObject> m_FoldMarkers = new List<GameObject>();

        private IntegrationTestsRunnerSettings m_Settings;

        #endregion


        static IntegrationTestsRunnerWindow()
        {
            InitBackgroundRunners();
        }

        private static void InitBackgroundRunners()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemDraw;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemDraw;
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChangeUpdate;
            EditorApplication.hierarchyWindowChanged += OnHierarchyChangeUpdate;
            EditorApplication.update -= BackgroundSceneChangeWatch;
            EditorApplication.update += BackgroundSceneChangeWatch;
            EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
            EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
        }

        private static void OnPlaymodeStateChanged()
        {
            if (s_Instance && EditorApplication.isPlaying  == EditorApplication.isPlayingOrWillChangePlaymode)
                s_Instance.RebuildTestList();
        }

        public void OnDestroy()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemDraw;
            EditorApplication.update -= BackgroundSceneChangeWatch;
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChangeUpdate;
            EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;

            TestComponent.DestroyAllDynamicTests();
        }

        private static void BackgroundSceneChangeWatch()
        {
        	if (!s_Instance) return;
            if (s_Instance.m_CurrectSceneName != null && s_Instance.m_CurrectSceneName == EditorApplication.currentScene) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            TestComponent.DestroyAllDynamicTests();
            s_Instance.m_CurrectSceneName = EditorApplication.currentScene;
            s_Instance.RebuildTestList();
        }

        public void OnEnable()
        {
            title = "Integration Tests";
            s_Instance = this;

            m_Settings = ProjectSettingsBase.Load<IntegrationTestsRunnerSettings>();
            m_FilterSettings = new TestFilterSettings("UnityTest.IntegrationTestsRunnerWindow");

            InitBackgroundRunners();
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !m_ReadyToRun) RebuildTestList();
        }

        public void OnSelectionChange()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode
                || Selection.objects == null
                || Selection.objects.Length == 0) return;

            if (Selection.gameObjects.Length == 1)
            {
                var go = Selection.gameObjects.Single();
                var temp = go.transform;
                while (temp != null)
                {
                    var tc = temp.GetComponent<TestComponent>();
                    if (tc != null) break;
                    temp = temp.parent;
                }

                if (temp != null)
                {
                    SelectInHierarchy(temp.gameObject);
                    Selection.activeGameObject = temp.gameObject;
                    m_SelectedLine = temp.gameObject;
                }
            }
        }

        public static void OnHierarchyChangeUpdate()
        {
            if (!s_Instance || s_Instance.m_TestLines == null || EditorApplication.isPlayingOrWillChangePlaymode) return;

            // create a test runner if it doesn't exist
            TestRunner.GetTestRunner();

            if (s_Instance.m_Settings.addNewGameObjectUnderSelectedTest
                && s_Instance.m_SelectedLine != null
                && Selection.activeGameObject != null)
            {
                var go = Selection.activeGameObject;
                if (go.transform.parent == null
                    && go.GetComponent<TestComponent>() == null
                    && go.GetComponent<TestRunner>() == null)
                {
                    go.transform.parent = s_Instance.m_SelectedLine.transform;
                }
            }

            // make tests are not places under a go that is not a test itself
            foreach (var test in TestComponent.FindAllTestsOnScene())
            {
                if (test.gameObject.transform.parent != null && test.gameObject.transform.parent.gameObject.GetComponent<TestComponent>() == null)
                {
                    test.gameObject.transform.parent = null;
                    Debug.LogWarning("Tests need to be on top of the hierarchy or directly under another test.");
                }
            }
            if (selectedInHierarchy) selectedInHierarchy = false;
            else s_Instance.RebuildTestList();
        }
        
        public static TestResult GetResultForTest(TestComponent tc)
        {
        	if(!s_Instance) return new TestResult(tc);
            return s_Instance.m_ResultList.FirstOrDefault(r => r.GameObject == tc.gameObject);
        }

        public static void OnHierarchyWindowItemDraw(int id, Rect rect)
        {
            var o = EditorUtility.InstanceIDToObject(id);
            if (o is GameObject)
            {
                var go = o as GameObject;

                if (Event.current.type == EventType.MouseDown
                    && Event.current.button == 0
                    && rect.Contains(Event.current.mousePosition))
                {
                    var temp = go.transform;
                    while (temp != null)
                    {
                        var c = temp.GetComponent<TestComponent>();
                        if (c != null) break;
                        temp = temp.parent;
                    }
                    if (temp != null) SelectInHierarchy(temp.gameObject);
                }
            }
        }

        private static void SelectInHierarchy(GameObject gameObject)
        {
        	if (!s_Instance) return;
            if (gameObject == s_Instance.m_SelectedLine) return;
            if (!gameObject.activeSelf)
            {
                selectedInHierarchy = true;
                gameObject.SetActive(true);
            }

            var tests = TestComponent.FindAllTestsOnScene();
            var skipList = gameObject.GetComponentsInChildren(typeof(TestComponent), true).ToList();
            tests.RemoveAll(skipList.Contains);
            foreach (var test in tests)
            {
                var enable = test.GetComponentsInChildren(typeof(TestComponent), true).Any(c => c.gameObject == gameObject);
                if (test.gameObject.activeSelf != enable) test.gameObject.SetActive(enable);
            }
        }

        private void RunTests(IList<ITestComponent> tests)
        {
            if (!tests.Any() || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            FocusWindowIfItsOpen(GetType());

			var testComponents = tests.Where(t => t is TestComponent).Cast<TestComponent>().ToList();
			var dynaminTests = testComponents.Where(t => t.dynamic).ToList();
			m_DynamicTestsToRun = dynaminTests.Select(c => c.dynamicTypeName).ToList();
			testComponents.RemoveAll(dynaminTests.Contains);

			m_TestsToRun = testComponents.Select( tc => tc.gameObject ).ToList();

            m_ReadyToRun = true;
            TestComponent.DisableAllTests();
            EditorApplication.isPlaying = true;

            if (m_Settings.blockUIWhenRunning)
                EditorUtility.DisplayProgressBar("Integration Test Runner", "Initializing", 0);
        }

        public void Update()
        {
            if (m_ReadyToRun && EditorApplication.isPlaying)
            {
                m_ReadyToRun = false;
                var testRunner = TestRunner.GetTestRunner();
				testRunner.TestRunnerCallback.Add(new RunnerCallback(this));
				var testComponents = m_TestsToRun.Select(go => go.GetComponent<TestComponent>()).ToList();
				testRunner.InitRunner(testComponents, m_DynamicTestsToRun);
            }
        }
        
        private void RebuildTestList()
        {
            m_TestLines = null;
            if (!TestComponent.AnyTestsOnScene()) return;

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var dynamicTestsOnScene = TestComponent.FindAllDynamicTestsOnScene();
                var dynamicTestTypes = TestComponent.GetTypesWithHelpAttribute(EditorApplication.currentScene);

                foreach (var dynamicTestType in dynamicTestTypes)
                {
                    var existingTests = dynamicTestsOnScene.Where(component => component.dynamicTypeName == dynamicTestType.AssemblyQualifiedName);
                    if (existingTests.Any())
                    {
                        var testComponent = existingTests.Single();
                        foreach (var c in testComponent.gameObject.GetComponents<Component>())
                        {
                            var type = Type.GetType(testComponent.dynamicTypeName);
                            if (c is TestComponent || c is Transform || type.IsInstanceOfType(c)) continue;
                            DestroyImmediate(c);
                        }
                        dynamicTestsOnScene.Remove(existingTests.Single());
                        continue;
                    }
                    TestComponent.CreateDynamicTest(dynamicTestType);
                }

                foreach (var testComponent in dynamicTestsOnScene)
                    DestroyImmediate(testComponent.gameObject);
            }

            var topTestList = TestComponent.FindAllTopTestsOnScene();

            var newResultList = new List<TestResult>();
            m_TestLines = ParseTestList(topTestList, newResultList);

            var oldDynamicResults = m_ResultList.Where(result => result.dynamicTest);
            foreach (var oldResult in m_ResultList)
            {
                var result = newResultList.Find(r => r.Id == oldResult.Id);
                if (result == null) continue;
                result.Update(oldResult);
            }
            newResultList.AddRange(oldDynamicResults.Where(r => !newResultList.Contains(r)));
            m_ResultList = newResultList;

            IntegrationTestRendererBase.RunTest = RunTests;
            IntegrationTestGroupLine.FoldMarkers = m_FoldMarkers;
            IntegrationTestLine.Results = m_ResultList;
            
            m_FilterSettings.UpdateCounters(m_ResultList.Cast<ITestResult>());

            m_FoldMarkers.RemoveAll(o => o == null);

            selectedInHierarchy = true;
            Repaint();
        }


        private IntegrationTestRendererBase[] ParseTestList(List<TestComponent> testList, List<TestResult> results)
        {
            var tempList = new List<IntegrationTestRendererBase>();
            foreach (var testObject in testList)
            {
                if (!testObject.IsTestGroup())
                {
                    var result = new TestResult(testObject);
                    if (results != null)
                        results.Add(result);
                    tempList.Add(new IntegrationTestLine(testObject.gameObject, result));
                    continue;
                }
                var group = new IntegrationTestGroupLine(testObject.gameObject);
                var children = testObject.gameObject.GetComponentsInChildren(typeof(TestComponent), true).Cast<TestComponent>().ToList();
                children = children.Where(c => c.gameObject.transform.parent == testObject.gameObject.transform).ToList();
                group.AddChildren(ParseTestList(children, results));
                tempList.Add(group);
            }
            tempList.Sort();
            return tempList.ToArray();
        }

        public void OnGUI()
        {
#if !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            if (BuildPipeline.isBuildingPlayer)
            {
                m_IsBuilding = true;
            }
            else if (m_IsBuilding)
            {
                m_IsBuilding = false;
                Repaint();
            }
#endif  // if !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
            PrintHeadPanel();

            EditorGUILayout.BeginVertical(Styles.testList);
            m_TestListScroll = EditorGUILayout.BeginScrollView(m_TestListScroll);
            bool repaint = PrintTestList(m_TestLines);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            RenderDetails();

            if (repaint) Repaint();
        }

        public void PrintHeadPanel()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            if (GUILayout.Button(m_GUIRunAllTests, EditorStyles.toolbarButton))
            {
                RunTests(TestComponent.FindAllTestsOnScene().Cast<ITestComponent>().ToList());
            }
            EditorGUI.BeginDisabledGroup(!Selection.gameObjects.Any (t => t.GetComponent(typeof(ITestComponent))));
            if (GUILayout.Button(m_GUIRunSelectedTests, EditorStyles.toolbarButton))
            {
                RunTests(Selection.gameObjects.Select(t => t.GetComponent(typeof(TestComponent))).Cast<ITestComponent>().ToList());
            }
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(m_GUICreateNewTest, EditorStyles.toolbarButton))
            {
                var test = TestComponent.CreateTest();
                if (Selection.gameObjects.Length == 1
                    && Selection.activeGameObject != null
                    && Selection.activeGameObject.GetComponent<TestComponent>())
                {
                    test.transform.parent = Selection.activeGameObject.transform.parent;
                }
                Selection.activeGameObject = test;
                RebuildTestList();
            }
            EditorGUI.EndDisabledGroup();
            
            GUILayout.FlexibleSpace ();
            
            m_FilterSettings.OnGUI ();
            
            EditorGUILayout.EndHorizontal ();
        }
        
        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(m_GUIAddGoUderTest, m_Settings.addNewGameObjectUnderSelectedTest, m_Settings.ToggleAddNewGameObjectUnderSelectedTest);
            menu.AddItem(m_GUIBlockUI, m_Settings.blockUIWhenRunning, m_Settings.ToggleBlockUIWhenRunning);
        }
        
        private bool PrintTestList(IntegrationTestRendererBase[] renderedLines)
        {
            if (renderedLines == null) return false;

            var filter = m_FilterSettings.BuildRenderingOptions();

            bool repaint = false;
            foreach (var renderedLine in renderedLines)
            {
                repaint |= renderedLine.Render(filter);
            }
            return repaint;
        }

        private void RenderDetails()
        {
            var ctrlId = GUIUtility.GetControlID(FocusType.Passive);

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.y = rect.height + rect.y - 1;
            rect.height = 3;

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
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
                        Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == ctrlId)
                        GUIUtility.hotControl = 0;
                    break;
            }

            m_TestInfoScroll = EditorGUILayout.BeginScrollView(m_TestInfoScroll, GUILayout.MinHeight(m_HorizontalSplitBarPosition));

            var message = "";

            if (m_SelectedLine != null)
                message = GetResultText(m_SelectedLine);
            EditorGUILayout.TextArea(message, Styles.info);
            EditorGUILayout.EndScrollView();
        }

        private string GetResultText(GameObject go)
        {
            var result = m_ResultList.Find(r => r.GameObject == go);
            if (result == null) return "";
            var messages = result.Name;
            messages += "\n\n" + result.messages;
            if (!string.IsNullOrEmpty(result.stacktrace))
                messages += "\n" + result.stacktrace;
            return messages.Trim();
        }

        public void OnInspectorUpdate()
        {
            if (focusedWindow != this) Repaint();
        }

        private void SetCurrentTest(TestComponent tc)
        {
            foreach (var line in m_TestLines)
                line.SetCurrentTest(tc);
        }

        class RunnerCallback : ITestRunnerCallback
        {
            private readonly IntegrationTestsRunnerWindow m_Window;
            private int m_TestNumber;
            private int m_CurrentTestNumber;

            private readonly bool m_ConsoleErrorOnPauseValue;
            private readonly bool m_RunInBackground;
            private TestComponent m_CurrentTest;

            public RunnerCallback(IntegrationTestsRunnerWindow window)
            {
                m_Window = window;

                m_ConsoleErrorOnPauseValue = GuiHelper.GetConsoleErrorPause();
                GuiHelper.SetConsoleErrorPause(false);
                m_RunInBackground = PlayerSettings.runInBackground;
                PlayerSettings.runInBackground = true;
            }

            public void RunStarted(string platform, List<TestComponent> testsToRun)
            {
                EditorApplication.update += OnEditorUpdate;
                m_TestNumber = testsToRun.Count;
                foreach (var test in testsToRun)
                {
                    var result = m_Window.m_ResultList.Find(r => r.TestComponent == test);
                    if (result != null) result.Reset();
                }
            }

            public void RunFinished(List<TestResult> testResults)
            {
                m_Window.SetCurrentTest(null);
                m_CurrentTest = null;
                EditorApplication.update -= OnEditorUpdate;
                EditorApplication.isPlaying = false;
                EditorUtility.ClearProgressBar();
                GuiHelper.SetConsoleErrorPause(m_ConsoleErrorOnPauseValue);
                PlayerSettings.runInBackground = m_RunInBackground;
            }

            public void TestStarted(TestResult test)
            {
                m_Window.SetCurrentTest(test.TestComponent);
                m_CurrentTest = test.TestComponent;
            }


            public void TestFinished(TestResult test)
            {
                m_CurrentTestNumber++;

                var result = m_Window.m_ResultList.Find(r => r.Id == test.Id);
                if (result != null)
                    result.Update(test);
                else
                    m_Window.m_ResultList.Add(test);
            }

            public void TestRunInterrupted(List<ITestComponent> testsNotRun)
            {
                Debug.Log("Test run interrupted");
                RunFinished(new List<TestResult>());
            }

            private void OnEditorUpdate()
            {
                if (m_Window.m_Settings.blockUIWhenRunning && m_CurrentTest != null
                    && EditorUtility.DisplayCancelableProgressBar("Integration Test Runner",
                                                                  "Running " + m_CurrentTest.Name,
                                                                  (float)m_CurrentTestNumber / m_TestNumber))
                {
                    TestRunInterrupted(null);
                }
            }
        }

        [MenuItem("Unity Test Tools/Integration Test Runner %#&t")]
        public static IntegrationTestsRunnerWindow ShowWindow()
        {
            var w = GetWindow(typeof(IntegrationTestsRunnerWindow));
            w.Show();
            return w as IntegrationTestsRunnerWindow;
        }
    }
}
