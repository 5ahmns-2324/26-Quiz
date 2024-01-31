using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class quizScript : MonoBehaviour
{
    public TMP_Text questionText;
    public Image questionImage;
    public Button[] answerButtons;
    public Button nextButton;
    public TMP_Text scoreText;
    public TMP_Text timerText;

    private float maxTime = 10f;
    private float elapsedTime = 0f;

    public class Question
    {
        public string Text { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> WrongAnswers { get; set; }
        public Sprite QuestionImage { get; set; }
    }

    private List<Question> questions = new List<Question>
    {
        new Question
        {
            Text = "Wer ist Tom Turbo?",
            CorrectAnswer = "1 A hinigs Foahrradl",
            WrongAnswers = new List<string> { "2 Ein Meisterdetektiv", "3 Thomas Brezinas imaginärer Freund" },
            QuestionImage = null,
        },
        new Question
        {
            Text = "Warum ist die Banane krumm?",
            CorrectAnswer = "1 Weils ihr daug",
            WrongAnswers = new List<string> { "2 So hoid", "3 Weil grod fad is" },
            QuestionImage = null,
        },

        new Question
        {
            Text = "Halli hallo wer sitzt am Klo?",
            CorrectAnswer = "1 Da Krampus und da Nikolo",
            WrongAnswers = new List<string> { "2 Die Omama", "3 da Peda" },
            QuestionImage = null,
        },

        new Question
        {
            Text = "Fischers Firtz, fischt frische Fische,...",
            CorrectAnswer = "1 ...frische Fische, fischt Fischers Fritz",
            WrongAnswers = new List<string> { "2 ...frische Tische, fischt Tischers Fritz", "3 ...frische Zische, fischt Zischers Fritz" },
            QuestionImage = null,
        },

    };

    private int currentQuestionIndex = 0;
    private Color originalButtonColor;
    private int score = 0;
    private bool questionAnswered = false;

    void Start()
    {
        ShowQuestion();
        nextButton.onClick.AddListener(NextQuestion);

        questions[0].QuestionImage = Resources.Load<Sprite>("TomTurboImage.jpg");

        originalButtonColor = answerButtons[0].GetComponent<Image>().color;

        StartTimer();

    }

    void DisableAnswerButtons()
    {
        foreach (var button in answerButtons)
        {
            button.interactable = false;
        }
    }

    void EnableAnswerButtons()
    {
        foreach (var button in answerButtons)
        {
            button.interactable = true;
        }
    }

    void Update()
    {
        UpdateTimer();
    }

    void StartTimer()
    {
        elapsedTime = maxTime;
    }

    void UpdateTimer()
    {
        elapsedTime -= Time.deltaTime;

        elapsedTime = Mathf.Max(elapsedTime, 0f);

        timerText.text = "Time: " + elapsedTime.ToString("F1");

        if (elapsedTime <= 0f)
        {
            Debug.Log("Zeit abgelaufen!");
        }
    }


    void ShowQuestion()
    {
        questionText.text = questions[currentQuestionIndex].Text;

        questionImage.sprite = questions[currentQuestionIndex].QuestionImage;

        List<string> answerOptions = new List<string>(3);
        answerOptions.Add(questions[currentQuestionIndex].CorrectAnswer);
        answerOptions.AddRange(GetWrongAnswers(currentQuestionIndex));

        answerOptions = ShuffleList(answerOptions);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < answerOptions.Count)
            {
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = answerOptions[i];
                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
            }
            else
            {
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = "Keine Antwort";
            }
        }
    }

    List<string> ShuffleList(List<string> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    List<string> GetWrongAnswers(int questionIndex)
    {
        return questions[questionIndex].WrongAnswers;
    }

    void CheckAnswer(int selectedAnswer)
    {

        if (questionAnswered) return;  

        questionAnswered = true;  

        if (questions[currentQuestionIndex].CorrectAnswer == answerButtons[selectedAnswer].GetComponentInChildren<TMP_Text>().text)
        {
            StartCoroutine(ChangeButtonColor(answerButtons[selectedAnswer], Color.green, 0.5f));
            score++;
            Debug.Log("Richtig! Aktueller Score: " + score);
        }
        else
        {
            StartCoroutine(ChangeButtonColor(answerButtons[selectedAnswer], Color.red, 0.5f));
            score--;
            Debug.Log("Falsch! Aktueller Score: " + score);
        }
        UpdateScoreText();
        DisableAnswerButtons();
    }

    IEnumerator ChangeButtonColor(Button button, Color color, float duration)
    {
        button.GetComponent<Image>().color = color;
        yield return new WaitForSeconds(duration);
        button.GetComponent<Image>().color = originalButtonColor;
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void NextQuestion()
    {
        questionAnswered = false;

        StartTimer();

        if (currentQuestionIndex >= questions.Count)
        {
            Debug.Log("Quiz beendet!");
            return;
        }

        currentQuestionIndex++;
        ShowQuestion();
        EnableAnswerButtons();
    }
}