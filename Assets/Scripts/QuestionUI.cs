using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionUI : MonoBehaviour {

    [Header("References")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private QuizOption[] optionButtons;
    private int selectedIndex;

    [Header("Colors")]
    [SerializeField] private Color selectedColor;
    private Color startColor;

    [Header("Checking")]
    [SerializeField] private TMP_Text scoreMarker;

    private void Start() {

        for (int i = 0; i < optionButtons.Length; i++)
            optionButtons[i].Initialize(this, i);

        startColor = optionButtons[0].GetComponentInChildren<TMP_Text>().color;

        scoreMarker.gameObject.SetActive(false); // hide score marker by default

    }

    public void OnOptionSelect(QuizOption option) {

        this.selectedIndex = option.GetOptionIndex();

        foreach (QuizOption btn in optionButtons)
            if (btn == option)
                btn.GetComponentInChildren<TMP_Text>().color = selectedColor;
            else
                btn.GetComponentInChildren<TMP_Text>().color = startColor;

    }

    public void SetQuestionText(string question) { questionText.text = question; }

    public void SetOptionTexts(string[] options) {

        for (int i = 0; i < optionButtons.Length; i++)
            optionButtons[i].GetComponentInChildren<TMP_Text>().text = GetOptionCharacter(i) + ") " + options[i];

    }

    private char GetOptionCharacter(int index) { return (char) (65 + index); }

    public void SetScoreMarker(string scoreText, Color color, float duration) {

        scoreMarker.text = scoreText;
        scoreMarker.color = Color.clear; // reset color for fade
        scoreMarker.gameObject.SetActive(true);
        scoreMarker.DOColor(color, duration); // fade in

    }

    public int GetSelectedIndex() { return selectedIndex; }

}
