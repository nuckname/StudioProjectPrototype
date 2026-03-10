using System;
using TMPro;
using UnityEngine;

public class QuestLogTracker : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        text.text = "QUEST LOG: 1\nPut the bread in the toaster";

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            text.text = "QUEST LOG: 1\nPut pan on stove";
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            text.text = "QUEST LOG: 1\nCook da bread";
        }
    }
}
