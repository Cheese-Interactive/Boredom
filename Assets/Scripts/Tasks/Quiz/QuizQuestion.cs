public class QuizQuestion {
    // Start is called before the first frame update
    private static int maxResponses = 4;
    private string question;
    private string[] responses;
    int correctResponseIndex;
    public QuizQuestion(string question, string[] possibleAnswers, int correctResponse) {
        if (possibleAnswers.Length <= maxResponses) {
            this.question = question;
            this.responses = new string[possibleAnswers.Length];
            for (int i = 0; i < responses.Length; i++)
                this.responses[i] = possibleAnswers[i];
            this.correctResponseIndex = correctResponse;
        }
        else
            this.question = "Error: too many responses, max is " + maxResponses + "!\n on: \"" + question + "\"";
    }


    public int getCorrectResponse() {
        return this.correctResponseIndex;
    }

    public string getQuestion() {
        return question;
    }
    public string[] getResponses() {
        return responses;
    }

    public string getResponse(int index) {
        return responses[index];
    }

}
