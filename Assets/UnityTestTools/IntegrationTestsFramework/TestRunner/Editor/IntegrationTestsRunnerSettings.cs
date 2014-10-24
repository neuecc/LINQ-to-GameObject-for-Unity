using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class IntegrationTestsRunnerSettings : ProjectSettingsBase
    {
        public bool showOptions;
        public string filterString;
        public bool showAdvancedFilter;
        public bool showSucceededTest = true;
        public bool showFailedTest = true;
        public bool showNotRunnedTest = true;
        public bool showIgnoredTest = true;
        public bool addNewGameObjectUnderSelectedTest;
        public bool blockUIWhenRunning = true;
    }
}
