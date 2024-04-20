using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TVRepairAnswerUI : MonoBehaviour {

    [Header("References")]
    [SerializeField] private TMP_Text answerText;
    private int index;

    public void Initialize() => index = transform.GetSiblingIndex();

    public void SetAnswerText(string answer) => answerText.text = answer;

    public string GetAnswerText() => answerText.text;

    public int GetIndex() => index;

}
