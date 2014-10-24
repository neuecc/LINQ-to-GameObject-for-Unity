using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    [InitializeOnLoad]
    public partial class UnitTestView
    {
        static UnitTestView()
        {
            if (s_Instance != null && s_Instance.m_Settings.runOnRecompilation)
                EnableBackgroundRunner(true);
        }

        #region Background runner

        private static float s_NextCheck;
        const string k_UttRecompile = "UTT-recompile";

        public static void EnableBackgroundRunner(bool enable)
        {
            EditorApplication.update -= BackgroudRunner;

            if (enable)
            {
                EditorApplication.update += BackgroudRunner;
                s_NextCheck = 0;
            }
        }

        private static void BackgroudRunner()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!s_Instance.m_Settings.runOnRecompilation) EnableBackgroundRunner(false);
            if (EditorApplication.isCompiling)
            {
                EditorPrefs.SetString(k_UttRecompile, Application.dataPath);
                EditorApplication.update -= BackgroudRunner;
                return;
            }

            var t = Time.realtimeSinceStartup;
            if (t < s_NextCheck) return;
            s_NextCheck = t + 0.5f;

            if (EditorPrefs.HasKey(k_UttRecompile))
            {
                var recompile = EditorPrefs.GetString(k_UttRecompile);
                if (recompile == Application.dataPath)
                {
                    s_Instance.RunTests();
                    s_Instance.Repaint();
                }
                EditorPrefs.DeleteKey(k_UttRecompile);
                s_NextCheck = 0;
            }
        }
        #endregion
    }
}
