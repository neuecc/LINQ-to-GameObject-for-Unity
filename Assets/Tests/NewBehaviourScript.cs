using UnityEngine;
using System.Collections;
using Unity.Linq; // using LINQ to GameObject

public class NewBehaviourScript : MonoBehaviour
{
    GameObject origin;

    void Start()
    {
        origin = GameObject.Find("Origin");
    }

    void Assert(string expected, string actual)
    {
        if (expected != actual)
        {
            Debug.LogWarning("NG " + expected + "|" + actual);
        }
        else
        {
            Debug.Log("OK " + expected + "|" + actual);
        }
    }

    void OnGUI()
    {
        if (GUILayout.Button("Parent"))
        {
            Assert(origin.Parent().name, "Root");
        }

        if (GUILayout.Button("bbb"))
        {
            
        }
    }
}
