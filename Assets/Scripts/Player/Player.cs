using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public float speed;									// Horizontal movement speed
	public float jumpStrength;							// Jump power
	public float jumpStrengthMultiplier;				// Should be 1, but it can be changed by other functions
	public float gravity;								// Rigidbodies can have their own gravity, but this makes it all a bit more tweakable
	
	public float feetDepth = 0.7f;
	public float armLength = 0.6f;

	public Transform spawnPoint;
	public Transform bottomLeftPoint;					
	public Transform bottomMiddlePoint;
	public Transform bottomRightPoint;					// This are points inside the player object, from which we cast a "ray" to find out if it's touching something
	
	private float lockLeft;								// Don't allow left movement
	private float lockRight;							// ... or right movement
	private Vector3  startScale;						// Remember the start scale, for the squishing animation
	private bool squished;								// Are we alive or what?
	
	private KeyCode keyLeft;
	private KeyCode keyRight;	
	private KeyCode keyUp;


	void Start () {
		startScale = transform.localScale;
		transform.position = spawnPoint.position;
	}
	
	void Update() {
		if(!squished)
		{
			// Jump and Squish check

			RaycastHit bottomLeft, bottomMiddle, bottomRight, left, right;
			Physics.Raycast (bottomLeftPoint.position, -bottomLeftPoint.up, out bottomLeft, feetDepth * transform.localScale.y);
			Physics.Raycast (bottomMiddlePoint.position, -bottomMiddlePoint.up , out bottomMiddle, feetDepth  * transform.localScale.y);
			Physics.Raycast (bottomRightPoint.position, -bottomRightPoint.up, out bottomRight, feetDepth  * transform.localScale.y);		
			Physics.Raycast (transform.position, transform.right, out right, armLength  * transform.localScale.x);		
			Physics.Raycast (transform.position, -transform.right, out left, armLength  * transform.localScale.x);		

			if(bottomLeft.collider && bottomLeft.collider.tag == "Good") { 
				bottomLeft.collider.transform.GetComponent<Cube>().Squish();

			}
			if(bottomMiddle.collider && bottomMiddle.collider.tag == "Good") { 
				bottomMiddle.collider.transform.GetComponent<Cube>().Squish();
			}
			if(bottomRight.collider && bottomRight.collider.tag == "Good") { 
				bottomRight.collider.transform.GetComponent<Cube>().Squish();
			}
		

			if(ChainJam.GetButtonJustPressed(ChainJam.BUTTON.A) || ChainJam.GetButtonJustPressed(ChainJam.BUTTON.B))
			{
				if (bottomLeft.collider || bottomMiddle.collider || bottomRight.collider) {	
					rigidbody.velocity = (new Vector3(0, jumpStrength * jumpStrengthMultiplier,0));
					SoundManager.i.Play(SoundManager.i.Jump);
					Debug.Log("normal jump");
				}
				else if(left.collider)
				{
					rigidbody.velocity = (new Vector3(jumpStrength* jumpStrengthMultiplier*0.3f, jumpStrength * jumpStrengthMultiplier*0.7f,0));
					SoundManager.i.Play(SoundManager.i.Jump);
					lockLeft = 0.1f;
					Debug.Log("side jump left" + jumpStrength + " " + jumpStrengthMultiplier );
				}
				else if(right.collider)
				{
					rigidbody.velocity = (new Vector3(-jumpStrength* jumpStrengthMultiplier*0.3f, jumpStrength* jumpStrengthMultiplier *0.7f,0));
					SoundManager.i.Play(SoundManager.i.Jump);
					lockRight = 0.1f;
					Debug.Log("side jump right" + jumpStrength + " " + jumpStrengthMultiplier );
				}
			}
			if((ChainJam.GetButtonJustReleased(ChainJam.BUTTON.A) || ChainJam.GetButtonJustReleased(ChainJam.BUTTON.B)) && rigidbody.velocity.y > 0 )
			{
				rigidbody.velocity = (new Vector3(rigidbody.velocity.x, rigidbody.velocity.y / 2,0));
			}
		}
	}
		
	
	// Update is called once per frame
	void FixedUpdate () {

		// MOVEMENT
		if(!squished)
		{
			if(ChainJam.GetButtonPressed(ChainJam.BUTTON.LEFT) && lockLeft <= 0)
			{
				rigidbody.velocity = (new Vector3(-1 * speed * Time.deltaTime,rigidbody.velocity.y,0));
			}
			if(ChainJam.GetButtonPressed(ChainJam.BUTTON.RIGHT)  && lockRight <= 0)
			{
				rigidbody.velocity=(new Vector3(speed * Time.deltaTime,rigidbody.velocity.y,0));
			}
		}

		// FAKE GRAVITY
		rigidbody.AddForce(new Vector3(0,gravity,0));

		// TINY COOLDOWN FOR WALL JUMP
		if(lockLeft > 0) lockLeft -= Time.deltaTime;
		if(lockRight > 0) lockRight -= Time.deltaTime;
	}
	
	void OnGUI() {
		// DRAWS THE LINES UNDER THE PLAYER IN THE SCENE VIEW
		Debug.DrawRay (bottomLeftPoint.position, 	-bottomLeftPoint.up*feetDepth, Color.green,0.1f);
		Debug.DrawRay (bottomMiddlePoint.position, 	-bottomMiddlePoint.up*feetDepth, Color.red,0.1f);
		Debug.DrawRay (bottomRightPoint.position, 	-bottomRightPoint.up*feetDepth, Color.blue,0.1f);
		
	}

	void OnCollisionEnter(Collision collision) {
		if(collision.collider.tag == "Bad" && !squished)
		{
			ChainJam.RemovePoints(1);
			Squish();
		}

	}


	public void Squish() {
		// EFFECTS ON PLAYER WHEN SQUISHED
		if(!squished)
		{
			SoundManager.i.Play(SoundManager.i.Squish);
			squished =true;
			iTween.ScaleTo(gameObject,iTween.Hash(
				"y",0.1f, 
				"x",1.3f,
				"time",0.2f,
				"easeType", "linear"));
			iTween.MoveTo(gameObject,iTween.Hash(
				"y",transform.position.y - 0.4f,
				"time",0.2f,
				"easeType", "linear"));
			StartCoroutine(Respawn());
		}
	}
	
	IEnumerator Respawn() {
		yield return new WaitForSeconds(2);

		iTween.ScaleTo(gameObject,iTween.Hash("scale",startScale,"time", 0.2f,"easeType", "linear"));
		squished = false;
		SoundManager.i.Play(SoundManager.i.Respawn);

		transform.position = spawnPoint.position;

	}
	

}
