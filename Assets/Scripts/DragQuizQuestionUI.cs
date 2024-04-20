using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragQuizQuestionUI : MonoBehaviour {

    [Header("References")]
    [SerializeField] private TMP_Text questionText;
    private DragQuizQuestion question;

    public void SetQuestionText(DragQuizQuestion question) {

        this.question = question;
        questionText.text = question.GetQuestionText();

    }

    public string GetQuestionText() => questionText.text;

    public int GetIndex() => transform.parent.GetSiblingIndex();

    public DragQuizQuestion GetQuestion() => question;

}
