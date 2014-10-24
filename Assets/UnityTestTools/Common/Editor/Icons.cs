using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    public static class Icons
    {
        const string k_IconsFolderName = "icons";
        private static readonly string k_IconsFolderPath = String.Format("UnityTestTools{0}Common{0}Editor{0}{1}", Path.DirectorySeparatorChar, k_IconsFolderName);

        private static readonly string k_IconsAssetsPath = "";

        public static readonly Texture2D FailImg;
        public static readonly Texture2D IgnoreImg;
        public static readonly Texture2D RunImg;
        public static readonly Texture2D RunFailedImg;
        public static readonly Texture2D RunAllImg;
        public static readonly Texture2D SuccessImg;
        public static readonly Texture2D UnknownImg;
        public static readonly Texture2D InconclusiveImg;
        public static readonly Texture2D StopwatchImg;
        public static readonly Texture2D PlusImg;
        public static readonly Texture2D GearImg;

        public static readonly GUIContent GUIUnknownImg;
        public static readonly GUIContent GUIInconclusiveImg;
        public static readonly GUIContent GUIIgnoreImg;
        public static readonly GUIContent GUISuccessImg;
        public static readonly GUIContent GUIFailImg;

        static Icons()
        {
            var dirs = Directory.GetDirectories("Assets", k_IconsFolderName, SearchOption.AllDirectories).Where(s => s.EndsWith(k_IconsFolderPath));
            if (dirs.Any())
                k_IconsAssetsPath = dirs.First();
            else
                Debug.LogWarning("The UnityTestTools asset folder path is incorrect. If you relocated the tools please change the path accordingly (Icons.cs).");

            FailImg = LoadTexture("failed.png");
            IgnoreImg = LoadTexture("ignored.png");
            SuccessImg = LoadTexture("passed.png");
            UnknownImg = LoadTexture("normal.png");
            InconclusiveImg = LoadTexture("inconclusive.png");
            StopwatchImg = LoadTexture("stopwatch.png");

            if (EditorGUIUtility.isProSkin)
            {
                RunAllImg = LoadTexture("play-darktheme.png");
                RunImg = LoadTexture("play_selected-darktheme.png");
                RunFailedImg = LoadTexture("rerun-darktheme.png");
                PlusImg = LoadTexture("create-darktheme.png");
                GearImg = LoadTexture("options-darktheme.png");
            }
            else
            {
                RunAllImg = LoadTexture("play-lighttheme.png");
                RunImg = LoadTexture("play_selected-lighttheme.png");
                RunFailedImg = LoadTexture("rerun-lighttheme.png");
                PlusImg = LoadTexture("create-lighttheme.png");
                GearImg = LoadTexture("options-lighttheme.png");
            }

            GUIUnknownImg = new GUIContent(UnknownImg);
            GUIInconclusiveImg = new GUIContent(InconclusiveImg);
            GUIIgnoreImg = new GUIContent(IgnoreImg);
            GUISuccessImg = new GUIContent(SuccessImg);
            GUIFailImg = new GUIContent(FailImg);
        }

        private static Texture2D LoadTexture(string fileName)
        {
            return (Texture2D)Resources.LoadAssetAtPath(k_IconsAssetsPath + Path.DirectorySeparatorChar + fileName, typeof(Texture2D));
        }
    }
}
