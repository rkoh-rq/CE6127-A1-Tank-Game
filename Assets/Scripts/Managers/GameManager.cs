using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TankManager _tankManager;
    public AITankManager _aiTankManager;
    public Score _score;
    public Animator animator;
    public Helipad heli;
    public Text message;
    public RectTransform hpBar;

    public enum State
    {
        GameLoads = 0,
        GamePrep,
        GameLoop,
        GameEnds,
        GameNewLevel
    };
    private State mState = State.GameLoads;
    private int level = 0;
    private int tanks_defeated = 0;
    private int defeats_required = 0;

    private void Start()
    {
        heli.objectEnter = OnObjectReachGoal;
        _tankManager.dOnTankDeath = OnTankDeath;
        _tankManager.dOnTankDamage = OnTankDamage;
        _aiTankManager.dOnAITankDefeat = OnAITankDefeat;
        state = State.GamePrep;
    }

    public void OnObjectReachGoal(Collider collider)
    {
        if (state == State.GameLoop && collider.gameObject.CompareTag("Player") && tanks_defeated >= defeats_required){
            level++;
            defeats_required++;
            tanks_defeated = 0;
            state = State.GameNewLevel;
        }
    }

    public void OnTankDeath()
    {
        if (state == State.GameLoop)
        {
            // End the round
            state = State.GameEnds;
        }
    }

    public void OnTankDamage(Tank tank)
    {
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(0, tank.mHealth) / tank._maxHealth * 500);
    }

    public void OnAITankDefeat()
    {
        tanks_defeated ++;
        updateText();
    }

    private void updateText(){
        _score.updateText("Tanks defeated:\t" + tanks_defeated + " / " + defeats_required);
        if (tanks_defeated >= defeats_required){
            _score.updateColor(Color.gray);
        }
        else{
            _score.updateColor(Color.green);
        }
    }

    private void InitGamePrep()
    {
        // Initialize all tanks
        _tankManager.Restart();
        _aiTankManager.Restart();

        updateText();

        // Change state to game loop
        state = State.GameLoop;
    }

    private IEnumerator InitGameEnd()
    {
        message.text = "GAME OVER";
        message.color = Color.white;
        animator.SetTrigger("FadeOut");
        
        // Delay before starting a new round
        yield return new WaitForSeconds(2f);
        
        SceneManager.LoadScene("Start");
    }

    private void OnFadeComplete(){
        // Do nothing
    }

    private IEnumerator InitNewLevel()
    {
        message.text = System.String.Format("LEVEL {0} COMPLETED",level);
        message.color = Color.white;
        animator.SetTrigger("FadeOut");
        
        // Delay before starting a new round
        yield return new WaitForSeconds(2f);

        // Display new goal
        updateText();

        message.text = "";
        animator.SetTrigger("FadeIn");

        // Initialize all tanks
        _tankManager.Restart();
        _aiTankManager.Restart();
        hpBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);

        // Change state to game loop
        state = State.GameLoop;
    }

    public State state
    {
        get { return mState; }
        set
        {
            if(mState != value)
            {
                mState = value;

                switch (value)
                {
                    case State.GamePrep:
                        InitGamePrep();
                        break;

                    case State.GameLoop:
                        break;

                    case State.GameEnds:
                        StartCoroutine(InitGameEnd());
                        break;

                    case State.GameNewLevel:
                        StartCoroutine(InitNewLevel());
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
