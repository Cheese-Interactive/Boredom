using UnityEngine;

public class GameData : MonoBehaviour {

    [Header("Quiz")]
    [SerializeField] private Quiz quiz;

    private void Start() {

        quiz.Initialize();

    }

    public Quiz GetQuiz() { return quiz; }

}
