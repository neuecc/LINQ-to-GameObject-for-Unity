using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace UnityTest.IntegrationTests
{
    [Serializable]
    public class PlatformRunnerSettingsWindow : EditorWindow
    {
        private BuildTarget m_BuildTarget;
        private readonly List<string> m_SceneList;
        private Vector2 m_ScrollPosition;
        private readonly List<string> m_Interfaces = new List<string>();
        private readonly List<string> m_SelectedScenes = new List<string>();
        private int m_SelectedInterface;
        [SerializeField]
        private bool m_AdvancedNetworkingSettings;

        private PlatformRunnerSettings m_Settings;

        readonly GUIContent m_Label = new GUIContent("Results target directory", "Directory where the results will be saved. If no value is specified, the results will be generated in project's data folder.");


        public PlatformRunnerSettingsWindow()
        {
            title = "Platform runner";
            m_BuildTarget = PlatformRunner.defaultBuildTarget;
            position.Set(position.xMin, position.yMin, 200, position.height);
            m_SceneList = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.unity", SearchOption.AllDirectories).ToList();
            m_SceneList.Sort();
            var currentScene = (Directory.GetCurrentDirectory() + EditorApplication.currentScene).Replace("\\", "").Replace("/", "");
            var currentScenePath = m_SceneList.Where(s => s.Replace("\\", "").Replace("/", "") == currentScene);
            m_SelectedScenes.AddRange(currentScenePath);

            m_Interfaces.Add("(Any)");
            m_Interfaces.AddRange(TestRunnerConfigurator.GetAvailableNetworkIPs());
            m_Interfaces.Add("127.0.0.1");
        }

        public void OnEnable()
        {
            m_Settings = ProjectSettingsBase.Load<PlatformRunnerSettings>();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("List of scenes to build:", EditorStyles.boldLabel);
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, Styles.testList);
            EditorGUI.indentLevel++;
            foreach (var scenePath in m_SceneList)
            {
                var path = Path.GetFileNameWithoutExtension(scenePath);
                var guiContent = new GUIContent(path, scenePath);
                var rect = GUILayoutUtility.GetRect(guiContent, EditorStyles.label);
                if (rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
                    {
						if (!Event.current.control && !Event.current.command)
                            m_SelectedScenes.Clear();
                        if (!m_SelectedScenes.Contains(scenePath))
                            m_SelectedScenes.Add(scenePath);
                        else
                            m_SelectedScenes.Remove(scenePath);
                        Event.current.Use();
                    }
                }
                var style = new GUIStyle(EditorStyles.label);
                if (m_SelectedScenes.Contains(scenePath))
                    style.normal.textColor = new Color(0.3f, 0.5f, 0.85f);
                EditorGUI.LabelField(rect, guiContent, style);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();

            GUILayout.Space(3);

            m_BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build tests for", m_BuildTarget);

            if (PlatformRunner.defaultBuildTarget != m_BuildTarget)
            {
                if (GUILayout.Button("Make default target platform"))
                {
                    PlatformRunner.defaultBuildTarget = m_BuildTarget;
                }
            }
            DrawSetting();
            var build = GUILayout.Button("Build and run tests");
            EditorGUILayout.EndVertical();

            if (!build) return;

            var config = new PlatformRunnerConfiguration
            {
                buildTarget = m_BuildTarget,
                scenes = m_SelectedScenes.ToArray(),
                projectName = m_SelectedScenes.Count > 1 ? "IntegrationTests" : Path.GetFileNameWithoutExtension(EditorApplication.currentScene),
                resultsDir = m_Settings.resultsPath,
                sendResultsOverNetwork = m_Settings.sendResultsOverNetwork,
                ipList = m_Interfaces.Skip(1).ToList(),
                port = m_Settings.port
            };

            if (m_SelectedInterface > 0)
                config.ipList = new List<string> {m_Interfaces.ElementAt(m_SelectedInterface)};

            PlatformRunner.BuildAndRunInPlayer(config);
            Close();
        }

        private void DrawSetting()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            m_Settings.resultsPath = EditorGUILayout.TextField(m_Label, m_Settings.resultsPath);
            if (GUILayout.Button("Search", EditorStyles.miniButton, GUILayout.Width(50)))
            {
                var selectedPath = EditorUtility.SaveFolderPanel("Result files destination", m_Settings.resultsPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                    m_Settings.resultsPath = Path.GetFullPath(selectedPath);
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(m_Settings.resultsPath))
            {
                Uri uri;
                if (!Uri.TryCreate(m_Settings.resultsPath, UriKind.Absolute, out uri) || !uri.IsFile || uri.IsWellFormedOriginalString())
                {
                    EditorGUILayout.HelpBox("Invalid URI path", MessageType.Warning);
                }
            }

            m_Settings.sendResultsOverNetwork = EditorGUILayout.Toggle("Send results to editor", m_Settings.sendResultsOverNetwork);
            EditorGUI.BeginDisabledGroup(!m_Settings.sendResultsOverNetwork);
            m_AdvancedNetworkingSettings = EditorGUILayout.Foldout(m_AdvancedNetworkingSettings, "Advanced network settings");
            if (m_AdvancedNetworkingSettings)
            {
                m_SelectedInterface = EditorGUILayout.Popup("Network interface", m_SelectedInterface, m_Interfaces.ToArray());
                EditorGUI.BeginChangeCheck();
                m_Settings.port = EditorGUILayout.IntField("Network port", m_Settings.port);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_Settings.port > IPEndPoint.MaxPort)
                        m_Settings.port = IPEndPoint.MaxPort;
                    else if (m_Settings.port < IPEndPoint.MinPort)
                        m_Settings.port = IPEndPoint.MinPort;
                }
                EditorGUI.EndDisabledGroup();
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_Settings.Save();
            }
        }
    }
}
