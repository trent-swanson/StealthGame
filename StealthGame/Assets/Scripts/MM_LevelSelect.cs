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
    }

    public void StartButton()
    {
        start = true;
        anim.SetBool("SlideIn", true);
        startButtonImage.gameObject.GetComponent<Animator>().SetBool("FadeOut", true);
    }

    //void Update()
    //{
    //    if (start)
    //    {
    //        ColorChange();
    //        ColorFade(startButtonImage);
    //    }
    //}

    //public void ColorChange()
    //{
    //    for (int i = 0; i < buttonImages.Count; i++)
    //    {
    //        Color color = buttonImages[i].color;
    //        color.a = ((buttonImages[i].GetComponent<RectTransform>().localPosition.x - xStartPos)/(-xStartPos + xEndPos));
    //        buttonImages[i].color = color;
    //        Debug.Log(color.a);
    //    }
    //}

    //private float currentAlpha = 255f;
    //public float fadeOutRate = 5f;
    //public void ColorFade(Image image)
    //{
    //    if (currentAlpha > 0) {

    //        currentAlpha -= fadeOutRate * Time.deltaTime;

    //    }
    //    else
    //    {
    //        currentAlpha = 0f;
    //    }
    //    Color color = image.color;
    //    color.a = currentAlpha;
    //    image.color = color;

    //}

}
