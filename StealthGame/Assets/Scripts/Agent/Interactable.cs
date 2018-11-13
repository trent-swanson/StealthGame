using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public bool m_usable = true;

    public Item m_requiredItem = null;

    public List<NavNode> m_interactionNodes = new List<NavNode>();

    public Animation m_animation = null;

    public GameObject m_interactableCanvas = null;

    public string m_customAnimation = "";

    protected GameController m_gameController = null;
    protected SoundController m_soundController = null;

    public Interactable m_nextInteractable = null;

    static Color m_fullColour = new Color(1, 1, 1, 1);
    static Color m_fadedColour = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    protected virtual void Start()
    {
#if UNITY_EDITOR
        if (m_interactableCanvas == null)
            Debug.Log("Interactable canvas on interactable has not been set");
#endif
        foreach (NavNode interactionNode in m_interactionNodes)
        {
            if (interactionNode != null)
            {
                interactionNode.SetNodeAsInteractable(this);
            }
        }
        m_interactableCanvas.SetActive(false);

        m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        m_soundController = m_gameController.GetComponent<SoundController>();
    }

    public virtual bool CanPerform(Agent agent)
    {
        return false;
    }

    public virtual void PerformAction(Agent agent)
    {
        if (m_nextInteractable != null)
            m_nextInteractable.ToggleCanvas(true);
    }

    public void DisableInteractable()
    {
        m_usable = false;

        foreach (NavNode interactionNode in m_interactionNodes)
        {
            if (interactionNode != null)
            {
                interactionNode.m_interactable = null;
                interactionNode.SetupNodeType();
            }
        }

        if(m_interactableCanvas!= null)
            m_interactableCanvas.SetActive(false);
    }

    public void FullCanvas()
    {
        foreach (Image canvasImage in GetComponentsInChildren<Image>())
        {
            canvasImage.color = m_fullColour;
        }
    }

    public void FadeCanvas()
    {
        foreach (Image canvasImage in m_interactableCanvas.GetComponentsInChildren<Image>())
        {
            canvasImage.color = m_fadedColour;
        }
    }

    public void ToggleCanvas(bool val)
    {
        m_interactableCanvas.SetActive(val);
    }
}
