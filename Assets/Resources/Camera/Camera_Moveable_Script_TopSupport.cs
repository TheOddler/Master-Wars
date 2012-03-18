using UnityEngine;
using System.Collections;

public class Camera_Moveable_Script_TopSupport : MonoBehaviour
{
	//Options
	public bool _invertX = false, _invertY = true;
	public float _speedX = 1.0f, _speedY = 1.0f;
	public float _moveSpeed = 10.0f;
	public Board_Script _board;
	public float _maxRot = 70;
	public float _minRot = 30;
	
	public float _zoomMinDist = 5;
	public float _zoomMaxDist = 50;
	
	public bool _topView = false;
	
	public float _zoomPercentage = .5f; //0 zoomed in, 1 zoomed out
	
	//Cam
	bool _inTopView = false;
	float _oldXRotation = 50;
	public Camera _camera;
	public Camera _topViewCamera;
	Transform _cameraTransform;
	
	// Use this for initialization
	void Start ()
	{
		_cameraTransform = _camera.GetComponentInChildren(typeof(Transform)) as Transform;;
		_topViewCamera.enabled = false;
		
		//Start rotation vertically
		transform.Rotate(50.0f, 0, 0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetButtonUp("Toggle TopView"))
		{
			ToggleTopView();
		}
		
		Zooming();
		RotateView();
		MoveAround();

	}
	
	void Zooming()
	{
		if (!_inTopView)
		{
			//Zooming
			float zoom = Input.GetAxis("Zoom");
			//float dist = (_cameraTransform.position - transform.position).magnitude;
			_zoomPercentage -= zoom;
			_zoomPercentage = Mathf.Clamp(_zoomPercentage, 0.0f,1.0f);
			
			float newZ = Mathf.Lerp(_zoomMinDist, _zoomMaxDist, _zoomPercentage);
			_cameraTransform.localPosition = new Vector3(0,0, -newZ);
		}
		else
		{
			//_topViewCamera.
		}
	}
	void RotateView()
	{
		//Rotate view
		if( Input.GetMouseButton(2) || ( Input.GetMouseButton(0) && Input.GetAxisRaw("CameraMove") > 0 ) )
		{
			float mDX = Input.GetAxis("Mouse X") / 20.0f * _speedX;
			if(_invertX) mDX *= -1.0f;
			float mDY = Input.GetAxis("Mouse Y") * 2.0f * _speedY;
			if(_invertY) mDY *= -1.0f;
			
			//Rotate horizontally
			transform.RotateAround(new Vector3(0,1,0), mDX);
			
			if (!_inTopView)
			{
				//Rotate vertically
				transform.Rotate(mDY, 0, 0);
				
				if(transform.eulerAngles.x > _maxRot && transform.eulerAngles.x <= 90.0f)
				{
					transform.eulerAngles = new Vector3(_maxRot, transform.eulerAngles.y, transform.eulerAngles.z);
				}
				
				if(transform.eulerAngles.x < _minRot)
				{
					transform.eulerAngles = new Vector3(_minRot, transform.eulerAngles.y, transform.eulerAngles.z);
				}
				
				_cameraTransform.LookAt(transform);
			}
			else
			{
				transform.eulerAngles = new Vector3(90.0f,transform.eulerAngles.y, transform.eulerAngles.z);
			}
		}
	}
	void MoveAround()
	{
		//Move around
		float hor = Input.GetAxis("Horizontal");
		float ver = Input.GetAxis("Vertical");
		if(hor != 0 || ver != 0)
		{
			Vector3 right = transform.right * hor;
			Vector3 forward = transform.up * ver;
			Vector3 total = right + forward;
			float lengthTotal = total.magnitude;
				total.y = 0;
				total.Normalize();
				total *= lengthTotal;
				total *= _moveSpeed;
				total *= Time.deltaTime;
			transform.Translate(total, Space.World);
		}
		
		//newPos is current pos
		Vector3 newPos = transform.position;
		//Cal max pos
		int cols = _board.GetNumberOfColumns();
		int rows = _board.GetNumberOfRows();
		Vector3 maxPos = _board.GetWorldPosition(cols-1, rows-1);
		//Calc min pos
		Vector3 minPos = _board.GetWorldPosition(0, 0);
		//Make sure newPos isn't bigger or smaller than max or min pos
		newPos.x = Mathf.Max(minPos.x, newPos.x);
		newPos.z = Mathf.Max(minPos.z, newPos.z);
		newPos.x = Mathf.Min(maxPos.x, newPos.x);
		newPos.z = Mathf.Min(maxPos.z, newPos.z);
		newPos.y = 0;
		transform.position = newPos;
	}
	
	void ToggleTopView()
	{
		_inTopView = !_inTopView;
		_camera.enabled = !_camera.enabled;
		_topViewCamera.enabled = !_topViewCamera.enabled;
		
		if(_inTopView)
		{
			_oldXRotation = transform.eulerAngles.x;
			transform.eulerAngles = new Vector3(90.0f, transform.eulerAngles.y, transform.eulerAngles.z);
		}
		else
		{
			transform.eulerAngles = new Vector3(_oldXRotation, transform.eulerAngles.y, transform.eulerAngles.z);
		}
	}
}
