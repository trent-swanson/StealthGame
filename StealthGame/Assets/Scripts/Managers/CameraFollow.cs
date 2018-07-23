using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform cameraPivot;
	CameraController cameraController;

	[Space]
	public float smoothSpeed = 0.125f;
	public float smoothRotation = 0.125f;

	void Start() {
		cameraController = cameraPivot.GetComponent<CameraController>();
	}

	void Update() {
		Vector3 pos = Vector3.zero;

		if (Input.GetMouseButton(1)) {
			pos.x -= Input.GetAxis("Mouse X") * cameraController.panSpeed * 2 * Time.deltaTime;
			pos.z -= Input.GetAxis("Mouse Y") * cameraController.panSpeed * 2 * Time.deltaTime;

			transform.Translate(pos);
			transform.position = new Vector3(Mathf.Clamp(transform.position.x, -cameraController.panLimit.x, cameraController.panLimit.x), Mathf.Clamp(transform.position.y, cameraController.minY, cameraController.maxY), Mathf.Clamp(transform.position.z, -cameraController.panLimit.y, cameraController.panLimit.y));
			cameraPivot.position = transform.position;
		}
	}

	void LateUpdate() {
		Vector3 desiredPosition = cameraPivot.position;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
		transform.position = new Vector3(smoothedPosition.x, cameraPivot.position.y, smoothedPosition.z);

		transform.rotation = Quaternion.Lerp(transform.rotation, cameraPivot.rotation, smoothRotation);
	}
}
