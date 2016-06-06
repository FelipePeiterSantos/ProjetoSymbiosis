using UnityEngine;
using System.Collections;

public class cameraOrbitController : MonoBehaviour 
{
	public Transform player;
	
	public Vector3 pivotOffset = new Vector3(0.0f, 1.0f,  0.0f);
	public Vector3 camOffset   = new Vector3(0.0f, 0.7f, -3.0f);

	public float smooth = 10f;

	public Vector3 aimPivotOffset = new Vector3(0.0f, 1.7f,  -0.3f);
	public Vector3 aimCamOffset   = new Vector3(0.8f, 0.0f, -1.0f);

	public float horizontalAimingSpeed = 400f;
	public float verticalAimingSpeed = 400f;
	public float maxVerticalAngle = 30f;
	public float flyMaxVerticalAngle = 60f;
	public float minVerticalAngle = -60f;
	
	public float mouseSensitivity = 0.3f;

	public float aimFov = 100f;
	
	private bool playerControl;
	private float angleH = 0;
	private float angleV = 0;
	private Transform cam;
    private Transform lockEnemy;

	private Vector3 relCameraPos;
	private float relCameraPosMag;
	
	private Vector3 smoothPivotOffset;
	private Vector3 smoothCamOffset;
	private Vector3 targetPivotOffset;
	private Vector3 targetCamOffset;

	private float defaultFOV;
	private float targetFOV;

	void Awake()
	{
		cam = transform;
        playerControl = true;

		relCameraPos = transform.position - player.position;
		relCameraPosMag = relCameraPos.magnitude - 0.5f;

		smoothPivotOffset = pivotOffset;
		smoothCamOffset = camOffset;

		defaultFOV = cam.GetComponent<Camera>().fieldOfView;
	}

	void LateUpdate()
	{
        Quaternion aimRotation;
		Quaternion camYRotation;
        Vector3 baseTempPosition;
		Vector3 tempOffset;

        if (playerControl) {
            angleH += Mathf.Clamp(Input.GetAxis("MovementX"), -1, 1) * horizontalAimingSpeed * Time.deltaTime;
            angleV += Mathf.Clamp(-Input.GetAxis("MovementY"), -1, 1) * verticalAimingSpeed * Time.deltaTime;
            angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
            angleH = angleH % 360;
            if(angleH < 0) {
                angleH += 360;
            }

            aimRotation = Quaternion.Euler(-angleV, angleH, 0);
		    camYRotation = Quaternion.Euler(0, angleH, 0);
            cam.rotation = aimRotation;

            targetPivotOffset = pivotOffset;
		    targetCamOffset = camOffset;
            targetFOV = defaultFOV;

            baseTempPosition = player.position + camYRotation * targetPivotOffset;
            tempOffset = targetCamOffset;
        }
        else {
            Quaternion lockEnemyCamera = Quaternion.LookRotation(lockEnemy.position - transform.position);
            cam.rotation = Quaternion.Slerp(transform.rotation, lockEnemyCamera, (smooth*0.5f) * Time.deltaTime);
            
            /*aimRotation = Quaternion.Euler(-angleV, angleH, 0);
		    camYRotation = Quaternion.Euler(0, angleH, 0);
            cam.rotation = aimRotation;*/

            aimRotation = cam.rotation;
		    camYRotation = Quaternion.Euler(0, cam.rotation.y, 0);
            cam.rotation = aimRotation;
            angleH = cam.rotation.eulerAngles.y;
            angleV = cam.rotation.eulerAngles.x;

            targetPivotOffset = aimPivotOffset;
			targetCamOffset = aimCamOffset;
            float dynamicFOV = Vector3.Distance(player.position, lockEnemy.position);
            dynamicFOV = Mathf.Clamp(dynamicFOV,5,25);
            targetFOV = defaultFOV - (dynamicFOV-5);

            baseTempPosition = player.position + camYRotation * targetPivotOffset;
            tempOffset = targetCamOffset;
        }
        
		for(float zOffset = targetCamOffset.z; zOffset <= 0; zOffset += 0.25f)
		{
			tempOffset.z = zOffset;
			if (DoubleViewingPosCheck (baseTempPosition + aimRotation * tempOffset) || zOffset == 0) 
			{
				targetCamOffset.z = tempOffset.z;
				break;
			} 
		}
        cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp (cam.GetComponent<Camera>().fieldOfView, targetFOV,  smooth * Time.deltaTime);
		smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
		smoothCamOffset = Vector3.Lerp(smoothCamOffset, targetCamOffset, smooth * Time.deltaTime);
        cam.position =  player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
	}

	bool DoubleViewingPosCheck(Vector3 checkPos)
	{
        float playerFocusHeight = player.position.y + pivotOffset.y;
		return ViewingPosCheck (checkPos, playerFocusHeight) && ReverseViewingPosCheck (checkPos, playerFocusHeight);
	}

	bool ViewingPosCheck (Vector3 checkPos, float deltaPlayerHeight)
	{
		RaycastHit hit;
        if (Physics.Raycast(checkPos, player.position + (Vector3.up * deltaPlayerHeight) - checkPos, out hit, relCameraPosMag, 1 << LayerMask.NameToLayer("Default")))
		{
			if(hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				return false;
			}
		}
		return true;
	}

	bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight)
	{
		RaycastHit hit;

		if(Physics.Raycast(player.position+(Vector3.up* deltaPlayerHeight), checkPos - player.position, out hit, relCameraPosMag))
		{
			if(hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				return false;
			}
		}
		return true;
	}

    public void LockCamera(bool active, Transform target) {
        playerControl = active;
        lockEnemy = target.FindChild("target");
    }
}
