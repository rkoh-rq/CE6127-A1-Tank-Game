using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public TankManager _tankManager;
    public AITankManager _aiTankManager;
    public Score _score;
    public Animator animator;
    public Helipad heli;

    public enum State
    {
        GameLoads = 0,
        GamePrep,
        GameLoop,
        GameEnds,
    };

    private State mState = State.GameLoads;
    private int tanks_defeated = 0;
    private int defeats_required = 0;

    private void Start()
    {
        heli.objectEnter = OnObjectReachGoal;
        state = State.GamePrep;
    }

    public void OnObjectReachGoal(Collider collider)
    {
        if (state == State.GameLoop && collider.gameObject.CompareTag("Player") && tanks_defeated >= defeats_required){
            state = State.GameEnds;
        }
    }

    private void updateText(){
        _score.updateText("Tanks defeated:\t" + tanks_defeated + " / " + defeats_required);
        if (tanks_defeated >= defeats_required){
            _score.updateColor(Color.gray);
        }
        else{
            _score.updateColor(Color.white);
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

    private void InitGameEnd()
    {
        animator.SetTrigger("FadeOut");
    }

    private void OnFadeComplete(){
        // Return to start
        SceneManager.LoadScene("AI Build");
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
                        InitGameEnd();
                        break;
                        
                    default:
                        break;
                }
            }
        }
    }
}
