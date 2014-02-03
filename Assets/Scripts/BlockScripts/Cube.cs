using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour {
	public float gravity;								// Rigidbodies can have their own gravity, but this makes it all a bit more tweakable
	public bool squished;

	void FixedUpdate () {
		rigidbody.AddForce(new Vector3(0,gravity,0));
	}

	public void Squish() {
		if(!squished)
		{
			squished = true;
			SoundManager.i.Play(SoundManager.i.Squish);
			iTween.ScaleTo(gameObject,iTween.Hash(
				"y",0.1f, 
				"x",1.3f,
				"time",0.2f,
				"easeType", "linear"));
			iTween.MoveTo(gameObject,iTween.Hash(
				"y",transform.position.y - 0.4f,
				"time",0.2f,
				"easeType", "linear"));
			collider.enabled = false;
			Destroy(gameObject, 0.2f);
			ChainJam.AddPoints(1);
		}
	}
}
