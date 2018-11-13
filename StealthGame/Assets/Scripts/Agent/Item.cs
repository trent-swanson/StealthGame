using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [Tooltip("A place to store items when in the agents inventory")]
    public static Vector3 m_itemTempStorage = new Vector3(0, -1000, 0);

    //Add to as more items are added
    public enum ITEM_TYPE {DEFAULT, GUN, CROWBAR, GOLD, WRENCH}
    public ITEM_TYPE m_itemType = ITEM_TYPE.DEFAULT;

    public GameObject m_canvas;

	NavNode m_currentNode;

    public Interactable m_nextInteractable = null;

	public Color m_highlightColour;

    public AIAction m_action;
    public Sprite m_icon;
    public string m_description;

    void Start()
    {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerManager.m_navNodeLayer))
        {
            m_currentNode = hit.collider.GetComponent<NavNode>();
            m_currentNode.NavNodeItem(ADD_REMOVE_FUNCTION.ADD, this);
        }
	}

	public void EquipItem(Agent agent)
    {
        if (!agent.m_agentInventory.AgentHasItem(this))
        {
            agent.m_agentInventory.ItemManagement(ADD_REMOVE_FUNCTION.ADD, this);

            if (m_currentNode != null)
                m_currentNode.NavNodeItem(ADD_REMOVE_FUNCTION.REMOVE);

            //Move away from sight
            transform.position = m_itemTempStorage;

            if (m_nextInteractable != null)
                m_nextInteractable.ToggleCanvas(true);
        }
    }

	public void TurnOnOutline() {
		Shader outlineShader = Shader.Find("Custom/Outline");
		if (outlineShader != null) {
			Renderer renderer = GetComponent<Renderer>();
			renderer.material.shader = outlineShader;
			renderer.material.SetColor("_OutlineColor", m_highlightColour);
            renderer.material.SetFloat("_OutlineWidth", 1.0f);
        }

        m_canvas.SetActive(true);
    }

	public void TurnOffOutline() {
		Shader standardShader = Shader.Find("Standard");
		if (standardShader != null) {
			Renderer renderer = GetComponent<Renderer>();
			renderer.material.shader = standardShader;
		}

        m_canvas.SetActive(false);
    }
}
