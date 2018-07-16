using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Agent : MonoBehaviour {

    //public List<Action> actionList = new List<Action>();

    [Header("Debugging Only")]
    [Tooltip("Do Not Assign")]
    public bool turn = false;
    [Tooltip("Do Not Assign")]
    public bool knockedout = false;

    List<Tile> selectableTiles = new List<Tile>();
    Tile unreachableTile;

    Stack<Tile> path = new Stack<Tile>();
    [Tooltip("Do Not Assign")]
    public Tile currentTile;

    [Tooltip("Do Not Assign")]
    public bool moving = false;
    [Tooltip("Do Not Assign")]
    public Tile actualTargetTile;

    [Space]
    [Space]
    [Header("Unit Editable Variables")]
    public GameObject unitCanvas;
    public Text APNumber;
    [Tooltip("# of actions unit can perform")]
    public int actionPoints = 2;
    [Tooltip("# of tiles unit can move")]
    public int maxMove = 2;
    int moveAmount;
    [Tooltip("# of tiles unit can jump")]
    public float jumpHeight = 1;
    [Tooltip("Move speed between tiles")]
    public float moveSpeed = 2;
    [Tooltip("How quickly unit jumps")]
    public float jumpVelocity = 4.5f;
    [Tooltip("Aditional height when jumping up")]
    public float jumpUpPop = 0.7f;
    [Tooltip("Amount forward movement is / when jumping")]
    public float jumpUpVelSlow = 1.2f;
    [Tooltip("Aditional hope when jumping down")]
    public float jumpDownPop = 2.7f;
    [Tooltip("Amount forward movement is / when falling")]
    public float jumpDownVelSlow = 2.5f;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    float halfHeight;

    bool fallingDown = false;
    bool jumpingUp = false;
    bool movingToEdge = false;
    bool isLowerThanJumpTarget = false;
    bool isHigherThanJumpTarget = false;
    float targetY;
    Vector3 jumpTarget;

    protected int currentActionPoints;
    private bool hiding = false;
    private bool haveWallPos = false;
    private Vector3 wallTargetPos = new Vector3();

    //Initialise agents
    protected void Init() {
        currentActionPoints = actionPoints;
        halfHeight = GetComponent<Collider>().bounds.extents.y;
        unitCanvas.SetActive(false);
        TurnManager.AddUnit(this);
    }

    public void GetCurrentTile() {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
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
        hiding = true;
        haveWallPos = false;

        moveAmount = currentActionPoints;
        if (moveAmount > maxMove) {
            moveAmount = maxMove;
        }

        ComputeAdjacentcyLists(jumpHeight, null);
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile);
        currentTile.visited = true;

        while (process.Count > 0) {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;

            if (t.distance < moveAmount) {
                foreach (Tile tile in t.adjacencyList) {
                    if (!tile.visited) {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    //Get Path in reverse order
    public void CheckMoveToTile(Tile p_tile, bool hover) {
        path.Clear();
        int pathCost = 0;

        Tile next = p_tile;
        while (next != null) {
            pathCost++;
            path.Push(next);
            next = next.parent;
        }
        //-1 from pathcost because while loop counts current Tile
        pathCost -= 1;

        if (hover) {
            p_tile.target = true;
            int tempAP = currentActionPoints - pathCost;
            APNumber.text = tempAP.ToString();
        }
        else {
            p_tile.target = true;
            moving = true;
            currentActionPoints -= pathCost;
        }
    }

    public void Move(bool hide) {
        if (path.Count > 0) {
            Tile t = path.Peek();
            Vector3 targetPos = t.transform.position;

            //calculate the agents position on top of the target tile
            targetPos.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

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
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else {
                //tile center reached
                transform.position = targetPos;
                path.Pop();
            }
        }
        else {
            if (hide && hiding) {
                WallHide();
            } else {
                RemoveSelectableTiles();
                moving = false;

                //end of move action
                EndAction();
            }
        }
    }

    protected void WallHide() {
        if (!haveWallPos) {
            RaycastHit hit;
            Vector3 rayPoint = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            if (Physics.Raycast(rayPoint, Vector3.forward, out hit, 2)) {
                wallTargetPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.65f);
                haveWallPos = true;
            } else if (Physics.Raycast(rayPoint, Vector3.back, out hit, 2)) {
                wallTargetPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.65f);
                haveWallPos = true;
            } else if (Physics.Raycast(rayPoint, Vector3.left, out hit, 2)) {
                wallTargetPos = new Vector3(transform.position.x - 0.65f, transform.position.y, transform.position.z);
                haveWallPos = true;
            } else if (Physics.Raycast(rayPoint, Vector3.right, out hit, 2)) {
                wallTargetPos = new Vector3(transform.position.x + 0.65f, transform.position.y, transform.position.z);
                haveWallPos = true;
            } else {
                hiding = false;
            }
        }

        transform.position = transform.position = Vector3.MoveTowards(transform.position, wallTargetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, wallTargetPos) <= 0.15f) {
            hiding = false;
        }
    }

    protected void RemoveSelectableTiles() {
        if (currentTile != null) {
            currentTile.current = false;
            currentTile = null;
        }
        foreach (Tile tile in selectableTiles) {
            tile.Reset();
        }
        selectableTiles.Clear();
    }

    //calculate the direction we have to head to reach target
    void CalculateHeading(Vector3 p_target) {
        heading = p_target - transform.position;
        heading.Normalize();
    }

    //set the velocity of agent to the heading direction
    void SetHorizontalVelocity() {
        velocity = heading * moveSpeed;
    }

    void Jump(Vector3 p_target) {
        if (fallingDown)
            FallDownward(p_target);
        else if (jumpingUp)
            JumpUpward(p_target);
        else if (movingToEdge)
            MoveToEdge();
        else
            PrepareJump(p_target);
    }

    void PrepareJump(Vector3 p_target) {
        isHigherThanJumpTarget = false;
        isLowerThanJumpTarget = false;
        targetY = p_target.y;
        p_target.y = transform.position.y;

        CalculateHeading(p_target);

        //if heigher
        if (transform.position.y > targetY) {
            fallingDown = false;
            jumpingUp = false;
            movingToEdge = true;

            jumpTarget = transform.position + (p_target - transform.position) / 2.0f;

            isHigherThanJumpTarget = true;
        }
        //if lower
        else {
            fallingDown = false;
            jumpingUp = false;
            movingToEdge = true;

            jumpTarget = transform.position + (p_target - transform.position) / 4.5f;

            isLowerThanJumpTarget = true;
        }
    }

    void FallDownward(Vector3 p_target) {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y <= p_target.y) {
            fallingDown = false;
            jumpingUp = false;
            movingToEdge = false;

            Vector3 pos = transform.position;
            pos.y = p_target.y;
            transform.position = pos;

            velocity = new Vector3();
        }
    }

    void JumpUpward(Vector3 p_target) {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > p_target.y) {
            jumpingUp = false;
            fallingDown = true;
        }
    }

    void MoveToEdge() {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.05f) {
            SetHorizontalVelocity();
        }
        else if (isHigherThanJumpTarget) {
            movingToEdge = false;
            fallingDown = true;

            //devide velocity to slow down movement while falling
            velocity /= jumpDownVelSlow;
            //add small vertical velocity for 'hop' off edge
            velocity.y = jumpDownPop;
        }
        else if (isLowerThanJumpTarget) {
            movingToEdge = false;
            jumpingUp = true;

            //devide velocity to slow down movement while jumping
            velocity = heading * moveSpeed / jumpUpVelSlow;

            float difference = targetY - transform.position.y;
            //jump velocity
            velocity.y = jumpVelocity * (jumpUpPop + difference / 2.0f);
        }
    }

    //A* pathfinding
    protected void FindPath(Tile p_target, bool p_isWaypoint) {
        ComputeAdjacentcyLists(jumpHeight, p_target);
        GetCurrentTile();

        List<Tile> openList = new List<Tile>();
        List<Tile> closeList = new List<Tile>();

        openList.Add(currentTile);

        currentTile.hCost = Vector3.SqrMagnitude(currentTile.transform.position - p_target.transform.position);
        currentTile.fCost = currentTile.hCost;

        while (openList.Count > 0) {
            Tile t = FindLowestFCost(openList);
            closeList.Add(t);

            if (t == p_target) {
                //found path
                actualTargetTile = FindEndTile(t, p_isWaypoint);
                CheckMoveToTile(actualTargetTile, false);
                return;
            }

            foreach (Tile tile in t.adjacencyList) {
                if (closeList.Contains(tile)) {
                    //Do nothing, already processed
                }
                else if (openList.Contains(tile)) {
                    //check if path is faster
                    float tempG = t.gCost + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (tempG < tile.gCost) {
                        tile.parent = t;
                        tile.gCost = tempG;
                        tile.fCost = tile.gCost + tile.hCost;
                    }
                    //else is path not fast, do nothing
                }
                else {
                    //new tile, calculate fCost and add to openList
                    tile.parent = t;

                    tile.gCost = t.gCost + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.hCost = Vector3.Distance(tile.transform.position, p_target.transform.position);
                    tile.fCost = tile.gCost + tile.hCost;

                    openList.Add(tile);
                }
            }
        }

        //todo - what to do if no path to target tile
        Debug.Log("Path not found");
    }

    protected Tile FindEndTile(Tile p_t, bool p_isWaypoint) {
        Stack<Tile> tempPath = new Stack<Tile>();

        Tile next = p_t.parent;
        //count back from target tile to current tile to get tempPath
        while (next != null) {
            tempPath.Push(next);
            next = next.parent;
        }

        //if in move range return target tile
        if (tempPath.Count <= moveAmount) {
            if (p_isWaypoint)
                return p_t;
            else
                return p_t.parent;
        }

        //if not in range return last tile in range
        Tile endTile = null;
        for (int i = 0; i <= moveAmount; i++) {
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
        if (!moving && currentActionPoints > 0) {
            //p_action.DoAction();
            EndAction();
        }
    }*/

    void EndAction() {
        APNumber.text = currentActionPoints.ToString();
        if (currentActionPoints <= 0) {
			TurnManager.EndTurn();
		}
	}

    public void BeginTurn() {
		turn = true;
        unitCanvas.SetActive(true);
        currentActionPoints = actionPoints;
        APNumber.text = currentActionPoints.ToString();
		moveAmount = maxMove;
	}

	public void EndTurn() {
        unitCanvas.SetActive(false);
		turn = false;			
	}

	public void Knockout() {
		knockedout = true;
		transform.position = new Vector3(0, 100, 0);
		TurnManager.EndTurn();
	}
}
