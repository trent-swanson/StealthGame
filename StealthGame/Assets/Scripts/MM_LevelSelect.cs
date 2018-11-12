using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MM_LevelSelect : MonoBehaviour {

    private Image startButtonImage;

    private GameObject levelList;
    private Animator anim;

    public List<Image> buttonImages = new List<Image>();
    private GameObject levelOne;
    private Image levelOneImage;
    private GameObject levelTwo;
    private Image levelTwoImage;
    private GameObject levelThree;
    private Image levelThreeImage;

    public bool start;

    public float xStartPos = -433f;
    public float xEndPos = 2f;

    [Header("Credits")]
    public GameObject creditObject;
    private Animator creditAnim;

    public GameObject exitButton;
    public GameObject backButton;
    public Animator creditsButtonAnim;

    // Use this for initialization
    void Start () {

        startButtonImage = GameObject.FindGameObjectWithTag("StartButton").GetComponent<Image>();
        levelList = GameObject.FindGameObjectWithTag("LevelList");
        anim = levelList.GetComponent<Animator>();

        Transform levelSelectTrans = levelList.transform;

        levelOneImage = levelSelectTrans.GetChild(0).GetComponent<Image>();
        buttonImages.Add(levelOneImage);
        levelTwoImage = levelSelectTrans.GetChild(1).GetComponent<Image>();
        buttonImages.Add(levelTwoImage);
        levelThreeImage = levelSelectTrans.GetChild(2).GetComponent<Image>();
        buttonImages.Add(levelThreeImage);

        Color one = levelOneImage.color;
        one.a = 0;
        levelOneImage.color = one;
        levelTwoImage.color = one;
        levelThreeImage.color = one;

        ///Credits
        creditAnim = creditObject.GetComponent<Animator>();
        //Exit Button on
        SwapInBack(false);
    }

    public void StartButton()
    {
        start = true;
        anim.SetBool("SlideIn", true);
        startButtonImage.gameObject.GetComponent<Animator>().SetBool("FadeOut", true);
        creditsButtonAnim.SetBool("CreditFade", true);
    }

    public void RollCredits()
    {
        //Credit button Fade + Move Credits
        creditAnim.SetBool("RollCredits", true);
        //StartButton Fade
        startButtonImage.gameObject.GetComponent<Animator>().SetBool("FadeOut", true);
        SwapInBack(true);
    }

    public void BackButton()
    {
        SwapInBack(false);
        startButtonImage.gameObject.GetComponent<Animator>().SetBool("FadeOut", false);
        //Turns off credits scroll
        creditAnim.SetBool("RollCredits", false);
    }
    public void SwapInBack(bool backButtonOn)
    {
        //Turn on Back button
        if (backButtonOn)
        {
            backButton.SetActive(true);
            exitButton.SetActive(false);
        }
        //Turn on Exit Button
        else
        {
            exitButton.SetActive(true);
            backButton.SetActive(false);
        }
    }

}
