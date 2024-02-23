using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Quiz : MonoBehaviour {
    // Start is called before the first frame update
    private QuizQuestion[] questionBank = {
        new QuizQuestion("3x + 4 = 13. x = ? ", new string[]{"1", "2", "3", "5"}, 2),
        new QuizQuestion("When did World War 1 begin?", new string[]{"1914", "1921", "1899"}, 0),
        new QuizQuestion("What is the largest continent on Earth?", new string[]{"Russia", "Asia", "North America", "Australia"}, 1),
        new QuizQuestion("What element on the periodic table has an atomic number of one?", new string[]{"Hydrogen", "Oxygen", "Carbon", "Nitrogen"}, 0),
        new QuizQuestion("What force always acts on everyone and everything on Earth?", new string[]{"Friction", "Kinetic", "Gravity", "Photosynthesis"}, 2)
    };
    private GameObject quizPaper;
    QuizQuestion[] questionsToUse;

    [SerializeField] private GameObject[] questions;
    [SerializeField] private GameObject[] questionOneResponses;
    [SerializeField] private GameObject[] questionTwoResponses;
    [SerializeField] private GameObject[] questionThreeResponses;



    void Start() {
        quizPaper = gameObject;
        initializeQuiz();
    }

    // Update is called once per frame
    void Update() {

    }

    private void initializeQuiz() {
        StartCoroutine(pickRandomQuestions());
        //for (int i = 0; i < questionsToUse.Length; i++)
        //print(questionsToUse[i].getQuestion());
        for (int q = 0; q < questions.Length; q++) {
            questions[q].GetComponent<TextMeshPro>().text = questionsToUse[q].getQuestion();
            for (int i = 0; i < questionsToUse[q].getResponses().Length; i++)
                questions[q].transform.GetChild(i).GetComponent<TextMeshPro>().text = questionsToUse[q].getResponse(i);
        }
    }

    private IEnumerator pickRandomQuestions() {
        questionsToUse = new QuizQuestion[3];
        List<int> usedIndexes = new List<int>();
        for (int i = 0; i < 3; i++) {
            int index = Random.Range(0, questionBank.Length);
            bool hasIndexBeenUsed = usedIndexes.Contains(index);
            while (hasIndexBeenUsed) {
                index = Random.Range(0, questionBank.Length);
                hasIndexBeenUsed = usedIndexes.Contains(index);
                yield return null;
            }
            usedIndexes.Add(index);
            questionsToUse[i] = questionBank[index];
        }
    }

    //   public override void OnTaskComplete() 
    //this part will be implemented in a seperate script, i didnt plan this out very well in advance tho lol
}
