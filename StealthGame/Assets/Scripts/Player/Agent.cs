using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Agent : MonoBehaviour {

    protected SquadManager squadManager;

    [Header("DebugDebugging Only")]
    [Tooltip("Do Not Assign")]
    public bool m_turn = false;
    [Tooltip("Do Not Assign")]
    public bool m_knockedout = false;

    [HideInInspector]
    protected List<Tile> m_selectableTiles = new List<Tile>();
    protected List<Vector3> m_selectableOutline = new List<Vector3>();
    Tile m_unreachableTile;

    Stack<Tile> m_path = new Stack<Tile>();
    [Tooltip("Do Not Assign")]
    public Tile m_currentTile;

    [Tooltip("Do Not Assign")]
    public bool m_moving = false;
    [Tooltip("Do Not Assign")]
    public Tile m_actualTargetTile;

    [Space]
    [Space]
    [Header("Unit Editable Variables")]
    public GameObject m_unitCanvas;
    public Image m_APDisplay;
    public Sprite m_defualtAPDisplay;
    public Sprite m_selectedAPDisplay;
    public TextMeshProUGUI m_APNumber;
    [Tooltip("# of actions unit can perform")]
    public int m_maxActionPoints = 2;
    public int m_currentActionPoints = 2;
    [Tooltip("# of tiles unit can move")]
    public int m_maxMove = 2;
    int m_moveAmount;
    [Tooltip("# of tiles unit can jump")]
    public float m_jumpHeight = 1;
    [Tooltip("Move speed between tiles")]
    public float m_moveSpeed = 2;
    [Tooltip("How quickly unit jumps")]
    public float m_jumpVelocity = 4.5f;
    [Tooltip("Aditional height when jumping up")]
    public float m_jumpUpPop = 0.7f;
    [Tooltip("Amount forward movement is / when jumping")]
    public float m_jumpUpVelSlow = 1.2f;
    [Tooltip("Aditional hope when jumping down")]
    public float m_jumpDownPop = 2.7f;
    [Tooltip("Amount forward movement is / when falling")]
    public float m_jumpDownVelSlow = 2.5f;

    Vector3 m_velocity = new Vector3();
    Vector3 m_heading = new Vector3();

    float m_halfHeight;

    bool m_fallingDown = false;
    bool m_jumpingUp = false;
    bool m_movingToEdge = false;
    bool m_isLowerThanJumpTarget = false;
    bool m_isHigherThanJumpTarget = false;
    float m_targetY;
    Vector3 m_jumpTarget;

    private bool m_hiding = false;
    private bool m_haveWallPos = false;
    private Vector3 m_wallTargetPos = new Vector3();

    [Space]
    public float m_highlightInteractablesRange = 6;

    UIController m_uiController;

    protected TurnManager m_turnManager = null;

    public NavNode m_currentNavNode = null;

    public TurnManager.TEAM m_team = TurnManager.TEAM.AI;

    [Space]
    public List<Item> m_currentItems = new List<Item>();

    protected virtual void Start()
    {
        //New Stuff
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("NavNode")))
            m_currentNavNode = hit.collider.GetComponent<NavNode>();

        m_turnManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<TurnManager>();
        m_uiController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
        squadManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SquadManager>();
        m_halfHeight = GetComponent<Collider>().bounds.extents.y;
        m_APDisplay.rectTransform.sizeDelta = new Vector2(-0.14f, -0.16f);
        //m_unitCanvas.SetActive(false);
    }

    public virtual void StartUnitTurn() {}

    public virtual void TurnUpdate() {}

    public void TurnEnd()
    {
        EndTurn();
        m_turnManager.EndUnitTurn(this);
    }

    public void GetCurrentTile() {
        m_currentTile = GetTargetTile(gameObject);
        m_currentTile.current = true;
    }

    public Tile GetTargetTile(GameObject p_target) {
        RaycastHit hit;
        Tile tile = null;
        if (Physics.Raycast(p_target.transform.position, Vector3.down, out hit, 2.0f)) {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    //get all adjacent tiles for each tile in grid and assign them to that tiles adjacentcy list
    public void ComputeAdjacentcyLists(float p_jumpHeight, Tile p_target) {
        foreach (GameObject tile in GameManager.tiles) {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(p_jumpHeight, p_target);
        }
    }

    //process the current tile and its adjacent tiles and their adjacent tiles if in move range to find selectable tiles
    public void FindSelectableTiles() {
        m_hiding = true;
        m_haveWallPos = false;

        m_moveAmount = m_currentActionPoints;
        if (m_moveAmount > m_maxMove) {
            m_moveAmount = m_maxMove;
        }

        ComputeAdjacentcyLists(m_jumpHeight, null);
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(m_currentTile);
        m_currentTile.visited = true;

        while (process.Count > 0) {
            Tile t = process.Dequeue();
            
            m_selectableTiles.Add(t);
            t.selectable = true;
            t.selectableBy = this;

            if (t.distance < m_moveAmount) {
                foreach (Tile tile in t.adjacencyList) {
                    if (!tile.visited) {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;

                        //fade out selectable tile color
                        if (tile.distance == 2) {
                            tile.spriteColor.a = 0.6f;
                            tile.spriteRenderer.color = tile.spriteColor;
                        }
                        if (tile.distance == 3) {
                            tile.spriteColor.a = 0.4f;
                            tile.spriteRenderer.color = tile.spriteColor;
                        }

                        process.Enqueue(tile);
                    }
                }
            }

            if (t.distance == m_moveAmount) {
                m_selectableOutline.Add(t.transform.position);
            }
        }
    }

    //Get m_path in reverse order
    public Vector3[] CheckMoveToTile(Tile p_tile, bool hover) {
        m_path.Clear();
        int pathCost = 0;
        List<Vector3> pathRenderList = new List<Vector3>();

        Tile next = p_tile;
        while (next != null) {
            //get path
            pathCost++;
            m_path.Push(next);

            //Calculate pathRenderList
            pathRenderList.Add(new Vector3(next.transform.position.x, next.transform.position.y + 0.7f, next.transform.position.z));
            if (next.parent != null) {
                //going down
                if (next.parent.transform.position.y > next.transform.position.y) {
                    float offsetX = 0;
                    float offsetZ = 0;
                    if (next.parent.transform.position.x > next.transform.position.x) {
                        offsetX += 0.8f;
                    } else if (next.parent.transform.position.x < next.transform.position.x) {
                        offsetX -= 0.8f;
                    } else if (next.parent.transform.position.z > next.transform.position.z) {
                        offsetZ += 0.8f;
                    } else if (next.parent.transform.position.z < next.transform.position.z) {
                        offsetZ -= 0.8f;
                    }
                    pathRenderList.Add(new Vector3(next.transform.position.x + offsetX, next.transform.position.y + 0.7f, next.transform.position.z + offsetZ));
                    pathRenderList.Add(new Vector3(next.transform.position.x + offsetX, next.parent.transform.position.y + 0.7f, next.transform.position.z + offsetZ));
                }
                //going up
                else if (next.parent.transform.position.y < next.transform.position.y) {
                    float offsetX = 0;
                    float offsetZ = 0;
                    if (next.parent.transform.position.x > next.transform.position.x) {
                        offsetX += 1.2f;
                    } else if (next.parent.transform.position.x < next.transform.position.x) {
                        offsetX -= 1.2f;
                    } else if (next.parent.transform.position.z > next.transform.position.z) {
                        offsetZ += 1.2f;
                    } else if (next.parent.transform.position.z < next.transform.position.z) {
                        offsetZ -= 1.2f;
                    }
                    pathRenderList.Add(new Vector3(next.transform.position.x + offsetX, next.transform.position.y + 0.7f, next.transform.position.z + offsetZ));
                    pathRenderList.Add(new Vector3(next.transform.position.x + offsetX, next.parent.transform.position.y + 0.7f, next.transform.position.z + offsetZ));
                }
            }

            //get next tile
            next = next.parent;
        }

        //-1 from pathcost because while loop counts current Tile
        pathCost -= 1;

        //displayer path AP cost
        if (hover) {
            p_tile.target = true;
            int tempAP = m_currentActionPoints - pathCost;
            m_APNumber.text = tempAP.ToString();
        }
        //Take path AP cost
        else {
            p_tile.target = true;
            m_moving = true;
            m_currentActionPoints -= pathCost;
        }

        return pathRenderList.ToArray();
    }

    public void Move(bool hide) {
        if (m_path.Count > 0) {
            Tile t = m_path.Peek();
            Vector3 targetPos = t.transform.position;

            //calculate the agents position on top of the target tile
            targetPos.y += m_halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, targetPos) >= 0.15f) {
                bool jump = transform.position.y != targetPos.y;

                if (jump) {
                    Jump(targetPos);
                } else {
                    //calculate move forward
                    CalculateHeading(targetPos);
                    SetHorizontalVelocity();
                }

                //Locomotion (add animations here)
                transform.forward = m_heading;
                transform.position += m_velocity * Time.deltaTime;
            }
            else {
                //tile center reached
                transform.position = targetPos;
                m_path.Pop();
            }
        }
        else {
            if (hide && m_hiding) {
                WallHide();
            } else {
                RemoveSelectableTiles();
                m_moving = false;

                if (GetComponent<PlayerController>()) {
                    FindInteractables();
                }

                //end of move action
                EndAction();
            }
        }
    }

    protected void WallHide() {
        if (!m_haveWallPos) {
            RaycastHit hit;
            Vector3 rayPoint = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            if (Physics.Raycast(rayPoint, Vector3.forward, out hit, 2)) {
                m_wallTargetPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.65f);
                m_haveWallPos = true;
            } else if (Physics.Raycast(rayPoint, Vector3.back, out hit, 2)) {
                m_wallTargetPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.65f);
                m_haveWallPos = true;
            } else if (Physics.Raycast(rayPoint, Vector3.left, out hit, 2)) {
                m_wallTargetPos = new Vector3(transform.position.x - 0.65f, transform.position.y, transform.position.z);
                m_haveWallPos = true;
            } else if (Physics.Raycast(rayPoint, Vector3.right, out hit, 2)) {
                m_wallTargetPos = new Vector3(transform.position.x + 0.65f, transform.position.y, transform.position.z);
                m_haveWallPos = true;
            } else {
                m_hiding = false;
            }
        }

        transform.position = transform.position = Vector3.MoveTowards(transform.position, m_wallTargetPos, m_moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, m_wallTargetPos) <= 0.15f) {
            m_hiding = false;
        }
    }

    protected void RemoveSelectableTiles() {
        if (m_currentTile != null) {
            m_currentTile.current = false;
            m_currentTile = null;
        }
        foreach (Tile tile in m_selectableTiles) {
            tile.Reset();
        }
        m_selectableTiles.Clear();
    }

    //calculate the direction we have to head to reach target
    void CalculateHeading(Vector3 p_target) {
        m_heading = p_target - transform.position;
        m_heading.Normalize();
    }

    //set the velocity of agent to the heading direction
    void SetHorizontalVelocity() {
        m_velocity = m_heading * m_moveSpeed;
    }

    void Jump(Vector3 p_target) {
        if (m_fallingDown)
            FallDownward(p_target);
        else if (m_jumpingUp)
            JumpUpward(p_target);
        else if (m_movingToEdge)
            MoveToEdge();
        else
            PrepareJump(p_target);
    }

    void PrepareJump(Vector3 p_target) {
        m_isHigherThanJumpTarget = false;
        m_isLowerThanJumpTarget = false;
        m_targetY = p_target.y;
        p_target.y = transform.position.y;

        CalculateHeading(p_target);

        //if heigher
        if (transform.position.y > m_targetY) {
            m_fallingDown = false;
            m_jumpingUp = false;
            m_movingToEdge = true;

            m_jumpTarget = transform.position + (p_target - transform.position) / 2.0f;

            m_isHigherThanJumpTarget = true;
        }
        //if lower
        else {
            m_fallingDown = false;
            m_jumpingUp = false;
            m_movingToEdge = true;

            m_jumpTarget = transform.position + (p_target - transform.position) / 4.5f;

            m_isLowerThanJumpTarget = true;
        }
    }

    void FallDownward(Vector3 p_target) {
        m_velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y <= p_target.y) {
            m_fallingDown = false;
            m_jumpingUp = false;
            m_movingToEdge = false;

            Vector3 pos = transform.position;
            pos.y = p_target.y;
            transform.position = pos;

            m_velocity = new Vector3();
        }
    }

    void JumpUpward(Vector3 p_target) {
        m_velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > p_target.y) {
            m_jumpingUp = false;
            m_fallingDown = true;
        }
    }

    void MoveToEdge() {
        if (Vector3.Distance(transform.position, m_jumpTarget) >= 0.15f) {
            SetHorizontalVelocity();
        }
        else if (m_isHigherThanJumpTarget) {
            m_movingToEdge = false;
            m_fallingDown = true;

            //devide velocity to slow down movement while falling
            m_velocity /= m_jumpDownVelSlow;
            //add small vertical velocity for 'hop' off edge
            m_velocity.y = m_jumpDownPop;
        }
        else if (m_isLowerThanJumpTarget) {
            m_movingToEdge = false;
            m_jumpingUp = true;

            //devide velocity to slow down movement while jumping
            m_velocity = m_heading * m_moveSpeed / m_jumpUpVelSlow;

            float difference = m_targetY - transform.position.y;
            //jump velocity
            m_velocity.y = m_jumpVelocity * (m_jumpUpPop + difference / 2.0f);
        }
    }

    protected Tile FindEndTile(Tile p_t, bool p_moveOntoTile) {
        Stack<Tile> tempPath = new Stack<Tile>();

        Tile next = p_t.parent;
        //count back from target tile to current tile to get tempPath
        while (next != null) {
            tempPath.Push(next);
            next = next.parent;
        }

        //if in move range return target tile
        if (tempPath.Count <= m_moveAmount) {
            if (p_moveOntoTile)
                return p_t;
            else
                return p_t.parent;
        }

        //if not in range return last tile in range
        Tile endTile = null;
        for (int i = 0; i <= m_moveAmount; i++) {
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    protected Tile FindLowestFCost(List<Tile> p_list) {
        Tile lowest = p_list[0];

        foreach (Tile t in p_list) {
            if (t.fCost < lowest.fCost) {
                lowest = t;
            }
        }

        p_list.Remove(lowest);

        return lowest;
    }

    /*public void DoAction(Action p_action) {
        if (!m_moving && currentActionPoints > 0) {
            //p_action.DoAction();
            EndAction();
        }
    }*/

    public void EndAction() {
        if (GetComponent<NPC>()) {
            NPC AI = GetComponent<NPC>();
            AI.m_currentAction = null;
            AI.m_currentActionNum++;
            if (AI.m_currentActionNum > AI.m_maxActionNum) {
                AI.m_currentActionNum = 0;
            }
        } else {
            m_APNumber.text = m_currentActionPoints.ToString();
            if (m_currentActionPoints <= 0) {
                TurnEnd();
            }
        }
	}

    public void BeginTurn() {
		m_turn = true;
        m_unitCanvas.SetActive(true);
        m_currentActionPoints = m_maxActionPoints;
        m_APNumber.text = m_currentActionPoints.ToString();
		m_moveAmount = m_maxMove;

        if (GetComponent<PlayerController>()) {
            FindInteractables();
        }
	}

	public void EndTurn() {
        //m_unitCanvas.SetActive(false);
        m_APDisplay.sprite = m_defualtAPDisplay;
        m_APDisplay.rectTransform.sizeDelta = new Vector2(-0.14f, -0.16f);
		m_turn = false;			
	}

	public void Knockout() {
		m_knockedout = true;
		transform.position = new Vector3(0, 100, 0);
		TurnEnd();
	}

    //highlight interactable objects in range
    void FindInteractables() {
        foreach (GameObject pickUp in GameObject.FindGameObjectsWithTag("PickUp")) {
            pickUp.GetComponent<PickUp>().TurnOffOutline();
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_highlightInteractablesRange);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].tag == "PickUp") {
                hitColliders[i].GetComponent<PickUp>().TurnOnOutline();
            }
        }
    }

    public void AddItem(Item p_item) {
        m_currentItems.Add(p_item);
        m_uiController.AddItem(this);
    }
}
