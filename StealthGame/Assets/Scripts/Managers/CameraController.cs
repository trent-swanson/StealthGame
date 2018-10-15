using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Pivot Panning")]
    public float m_pivotSpeed = 10f;
    public Vector2 m_pivotLimit;
    private Vector3 m_desiredPosition;

    [Header("Camera Movement")]
    public GameObject m_camera = null;
    public GameObject m_cameraPivot = null;
    public float m_maxMovementSpeed = 10.0f;
    public float m_distanceFromPivot = 3.0f;
    public float m_rotationSpeed = 30.0f;

    [Header("Camera Zooming")]
    public float m_scrollSpeed = 10;
    public float m_minY = 0f;
    public float m_maxY = 15f;

    private FACING_DIR m_camDirection = FACING_DIR.NORTH;
    private Vector3 m_movementExtents;
    private float m_cameraHeight = 0.0f;

    [Space]
    public WallFade NorthWalls;
    public WallFade SouthWalls;
    public WallFade EastWalls;
    public WallFade WestWalls;

    void Start()
    {
        Vector3 parentPos = transform.parent.gameObject.transform.position;

        m_movementExtents = parentPos + new Vector3(m_pivotLimit.x, 0.0f, m_pivotLimit.y);

        SouthWalls.FadeWall();

#if UNITY_EDITOR
        if (m_camera == null || m_cameraPivot == null)
            Debug.Log("Need to assign camera in camera controller");
#endif

        m_cameraHeight = m_maxY;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(transform.parent.position.x, 5, transform.parent.position.z), new Vector3((m_pivotLimit.x * 2f), 1f, (m_pivotLimit.y * 2f)));
    }

    public void Focus(Transform p_focusTarget)
    {
        m_desiredPosition = new Vector3(p_focusTarget.position.x, transform.position.y, p_focusTarget.position.z);
    }

    void Update()
    {
        m_desiredPosition += (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")) * Time.deltaTime * m_pivotSpeed;
        //Clamp within boundary
        m_desiredPosition.x = Mathf.Clamp(m_desiredPosition.x, -m_movementExtents.x, m_movementExtents.x);
        m_desiredPosition.z = Mathf.Clamp(m_desiredPosition.z, -m_movementExtents.z, m_movementExtents.z);

        Vector3 diffVector = m_desiredPosition - transform.position;
        Vector3 frameDiffVector = diffVector.normalized * m_maxMovementSpeed * Time.deltaTime;

        if (frameDiffVector.magnitude > -diffVector.magnitude && frameDiffVector.magnitude < diffVector.magnitude)
            transform.position += frameDiffVector;
        else
            transform.position = m_desiredPosition;

        if (Input.GetKeyDown("q"))
        {
            //left
            transform.rotation *= Quaternion.Euler(0, -90, 0);

            m_camDirection--;
            if (m_camDirection < 0)
                m_camDirection = FACING_DIR.WEST;
            
            WallFade();
        }
        if (Input.GetKeyDown("e"))
        {
            //right
            transform.rotation *= Quaternion.Euler(0, 90, 0);
            
            m_camDirection++;
            if ((int)m_camDirection > 3)
                m_camDirection = FACING_DIR.NORTH;
            
            WallFade();
        }

        m_cameraHeight += Input.GetAxis("Mouse ScrollWheel") * m_scrollSpeed;
        m_cameraHeight = Mathf.Clamp(m_cameraHeight, m_minY, m_maxY);

        UpdateCameraPos();
    }

    void WallFade() {
        NorthWalls.UnFadeWall();
        SouthWalls.UnFadeWall();
        EastWalls.UnFadeWall();
        WestWalls.UnFadeWall();

        switch (m_camDirection)
        {
            case FACING_DIR.NORTH:
                SouthWalls.FadeWall();
                break;
            case FACING_DIR.EAST:
                WestWalls.FadeWall();
                break;
            case FACING_DIR.SOUTH:
                NorthWalls.FadeWall();
            break;
            case FACING_DIR.WEST:
                EastWalls.FadeWall();
            break;
        }
    }

    private void UpdateCameraPos()
    {
        float diffAngle = Vector3.SignedAngle(m_cameraPivot.transform.forward, transform.forward, Vector3.up);
        diffAngle = Mathf.Clamp(diffAngle, -m_rotationSpeed, m_rotationSpeed);

        m_cameraPivot.transform.Rotate(Vector3.up, diffAngle);
        m_cameraPivot.transform.position = transform.position + m_cameraPivot.transform.forward * m_distanceFromPivot;

        m_camera.transform.localPosition = new Vector3(0.0f, m_cameraHeight, 0.0f);

        m_camera.transform.LookAt(transform.position);

        Quaternion cameraRot = m_camera.transform.localRotation;
        cameraRot.y = 0.0f;
        cameraRot.z = 0.0f;

        m_camera.transform.localRotation = cameraRot;
    }
}
