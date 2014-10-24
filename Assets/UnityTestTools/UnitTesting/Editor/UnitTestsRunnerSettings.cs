using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class UnitTestsRunnerSettings : ProjectSettingsBase
    {
        public bool optionsFoldout;
        public bool filtersFoldout;
        public bool runOnRecompilation;
        public bool horizontalSplit = true;
        public bool autoSaveSceneBeforeRun;
        public bool runTestOnANewScene;

        public int categoriesMask;
        public string testFilter = "";
        public bool showFailed = true;
        public bool showIgnored = true;
        public bool showNotRun = true;
        public bool showSucceeded = true;
    }
}
