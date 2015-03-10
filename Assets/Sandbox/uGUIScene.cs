using UnityEngine;
using Unity.Linq;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class uGUIScene : MonoBehaviour
{
    public GameObject Panel;
    public GameObject CloneFrom;
    public Button Button;
    public int HogeHoge;

    public void OnEnable()
    {
        var clone = GameObject.Instantiate(CloneFrom);
        Panel.MoveToLast(clone);
    }




    // Use this for initialization
    void Start()
    {

        Button.onClick.AddListener(() =>
        {
            var clone = GameObject.Instantiate(CloneFrom);

            Panel.MoveToLast(clone);
        });

    }

    // Update is called once per frame
    void Update()
    {

    }
}
