using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform cam;

	[Header("Panning")]
	public float panSpeed = 10f;
	public float panBorderPadding = 10f;
	public Vector2 panLimit;

	[Header("Zooming")]
	public float scrollSpeed = 10;
	public float minY = 0f;
	public float maxY = 15f;

	[Header("Rotating")]
	public float rotateSpeed = 95f;
	public float altRotateSpeed = 195f;

	void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(0f, cam.position.y, 0f), new Vector3((panLimit.x * 2f), 1f, (panLimit.y * 2f)));
    }

	public void Focus(Transform p_focusTarget) {
		transform.position = new Vector3(p_focusTarget.position.x, transform.position.y, p_focusTarget.position.z);
	}

	void Update() {
		Vector3 pos = Vector3.zero;

		if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderPadding || Input.GetKey(KeyCode.UpArrow)) {
			pos.z += panSpeed * Time.deltaTime;
		}
		if (Input.GetKey("s") || Input.mousePosition.y <= panBorderPadding || Input.GetKey(KeyCode.DownArrow)) {
			pos.z -= panSpeed * Time.deltaTime;
		}
		if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderPadding || Input.GetKey(KeyCode.RightArrow)) {
			pos.x += panSpeed * Time.deltaTime;
		}
		if (Input.GetKey("a") || Input.mousePosition.x <= panBorderPadding || Input.GetKey(KeyCode.LeftArrow)) {
			pos.x -= panSpeed * Time.deltaTime;
		}

		if (Input.GetKey("q")) {
			transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
		}
		if (Input.GetKey("e")) {
			transform.Rotate(Vector3.down * rotateSpeed * Time.deltaTime);
		}

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

		transform.Translate(pos);
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, -panLimit.x, panLimit.x), Mathf.Clamp(transform.position.y, minY, maxY), Mathf.Clamp(transform.position.z, -panLimit.y, panLimit.y));
	}
}
