using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public Tank targetTank;
    public Animator animator;

    private void Start()
    {
        MeshRenderer[] renderers = targetTank.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer rend in renderers)
            rend.material.color = Color.red;
    }

    public void Update()
    {
        if (targetTank.mHealth < 0){
            animator.SetTrigger("FadeOut");
        }
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene("Tutorial AI Build");
    }
}
