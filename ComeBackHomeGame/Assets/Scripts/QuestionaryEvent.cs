using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Class that will handle the Questionary between the Enemy and the Player
 * The idea is to show some UI where the player can select between yes/no
 * Depending on the dificulty the time of question will be different and the speed of movement
 * This event will block the movements of the player/enemy on creation
 * 
 * */
public class QuestionaryEvent : MonoBehaviour {

    public GameObject QuestPanel;
    public Text QuestionText;
    public Text TimeText;
    public Button YesButton;
    public Button NoButton;
    public int Difficulty = 0;

    GameObject mPlayer = null;
    GameObject mEnemy = null;

    EnemyBehaviour[] Enemies;
    TravelingManager TravelingMng = null;


    private float SecondsQuestionTime = 3;
    private float SwitchTime = 0.5f;
    private int TotalQuestions = 0;
    private int AskedQuestions = 0;
    private float CurrentAnswerTime = 0;
    private float ChangeAnswerTime = 0;

    private bool CorrectAnswer = true;
    private bool CurrentSelectedAnswer = true;

    private bool bAskingQuestion = false;

    private bool bActivePanel = false;


    public void InitNewQuestionary(GameObject Player, GameObject Enemy)
    {
        mPlayer = Player;
        if(!Player || !Player.GetComponent<PlayerMovement>())
        {
            Debug.LogError("No Player set up properly passed to Questionary");
            return;
        }

        mPlayer.GetComponent<PlayerMovement>().ChangeMovement(false);
        mEnemy = Enemy;
        TravelingMng = GameObject.FindObjectOfType<TravelingManager>();
        Enemies = GameObject.FindObjectsOfType<EnemyBehaviour>();

        //Block everything until we finish
        BlockAssets();

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector3 OffsetPosition = mEnemy.transform.position + new Vector3(1.0f, 1.0f, 0.0f);
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(OffsetPosition);

        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);
        QuestPanel.transform.position = new Vector3(screenPoint.x, screenPoint.y, QuestPanel.transform.position.z);

        QuestPanel.SetActive(true);
        UpdateDifficulty();
        bActivePanel = true;
    }

    private void UpdateDifficulty()
    {
        //using the Difficulty parameter, define a time for answer and a number of times to ask
        TotalQuestions = 1;
    }

    private void End()
    {
        //Unblock Everything
        UnBlockAssets();
        QuestPanel.SetActive(false);
    }

    private void BlockAssets()
    {
        if (TravelingMng)
        {
            TravelingMng.Stop();
        }
        foreach (EnemyBehaviour En in Enemies)
        {
            En.BlockEnemy();
        }
    }
    
    private void UnBlockAssets()
    {
        if (TravelingMng)
        {
            TravelingMng.ReStart();
        }
        foreach (EnemyBehaviour En in Enemies)
        {
            En.UnBlockEnemy();
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if(bActivePanel)
        {
            if (Input.GetKeyDown("space"))
            {
                CheckAnswer();
            }

            if (!bAskingQuestion)
            {
                if(AskedQuestions < TotalQuestions)
                {
                    AskedQuestions++;
                    AskNewQuestion();
                }
                else
                {
                    HidePanel();
                }
            }
            else
            {
                CurrentAnswerTime += Time.deltaTime;
                ChangeAnswerTime += Time.deltaTime;
                TimeText.text = (SecondsQuestionTime - CurrentAnswerTime).ToString();
                if (CurrentAnswerTime > SecondsQuestionTime)
                {
                    CheckAnswer();
                }
                else if(ChangeAnswerTime > SwitchTime)
                {
                    ChangeAnswerTime = 0;
                    SwitchButtons();
                }
            }
        }
    }

    private void CheckAnswer()
    {
        //Check the correctness of the answer. if it's correct answer, don't do anything
        if (CurrentSelectedAnswer != CorrectAnswer)
        {
            //Wrong Answer! The player will be moved to a bar
            mEnemy.GetComponent<EnemyBehaviour>().MovePlayerToHome(mPlayer);
        }
        else if (AskedQuestions >= TotalQuestions)
        {
            mPlayer.GetComponent<PlayerMovement>().ChangeMovement(true);
            mEnemy.GetComponent<EnemyBehaviour>().StopTalking();
        }
        HidePanel();
    }

    private void AskNewQuestion()
    {
        //Read Question and correct asnwer
        //Wait the time asked and if answer is incorrect stop
        // if answer is correct continue;
        string Question = "Do you want to dance?";
        CorrectAnswer = false;

        //Set Button of answer with a dark tone
        QuestionText.text = Question;
        if(CorrectAnswer)
        {
            NoButton.Select();
        }
        else
        {
            YesButton.Select();
        }
        CurrentSelectedAnswer = !CorrectAnswer;
        CurrentAnswerTime = 0.0f;
        bAskingQuestion = true;
    }

    private void HidePanel()
    {
        bActivePanel = false;
        UnBlockAssets();
        QuestPanel.SetActive(false);
    }

    private void SwitchButtons()
    {
        //Basically Switch the current select button to the new one
        if(CurrentSelectedAnswer)
        {
            NoButton.Select();
        }
        else
        {
            YesButton.Select();
        }
        CurrentSelectedAnswer = !CurrentSelectedAnswer;
    }
}
