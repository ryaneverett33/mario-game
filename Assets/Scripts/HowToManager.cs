using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HowToManager : MonoBehaviour
{
    private Animator cameraAnimator, uiAnimator;
    private static HowToManager instance;

    void Start()
    {
        instance = this;
        
        // assign components
        cameraAnimator = GetComponent<Animator>();
        uiAnimator = GameObject.Find("Canvas").GetComponent<Animator>();
        StartCoroutine(waitThenFade());
    }

    public static HowToManager getInstance() {
        return instance;
    }

    public void fadingEnded() {
        SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
    }

    IEnumerator waitThenFade() {
        yield return new WaitForSeconds(7);
        uiAnimator.SetTrigger("fadeOut");
        cameraAnimator.SetTrigger("fadeOut");
    }
}
