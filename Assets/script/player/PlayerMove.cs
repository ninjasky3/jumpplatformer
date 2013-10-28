using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {
	
	public float gravityForce = 80f;
	public float horForce =70.0f;
	public Vector3 playerOrgin;
	
	public AudioClip switchGrafity;
	
	bool grafityUpDown = false;
	bool gravitySwitchDone = false;
	bool gravityswitchHalfDone = false;
	bool gravityswitchTriggerd = false;
	float xAxis;
	bool onGround = false;
	int timer;
	int frameID = 0;
	tk2dSprite sprite;
	
	
	void Awake(){
		playerOrgin = transform.position;
	}
	
	void Start () {
		Physics.gravity = new Vector3(0f,-gravityForce,0f);
		rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		//rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		//get Sprite
		sprite = GetComponent<tk2dSprite>();
		sprite.MakePixelPerfect();
	}
	
	void Update(){
		if(Input.GetKeyUp(KeyCode.Space)){
			audio.PlayOneShot(switchGrafity);
			if(onGround){
				if(grafityUpDown){
					Physics.gravity = new Vector3(0f,-gravityForce,0f);
					grafityUpDown = false;
					onGround = false;
				}else{
					Physics.gravity = new Vector3(0f,gravityForce,0f);
					grafityUpDown = true;
					onGround = false;
				}
				gravityswitchTriggerd = true;
			}
		}
	}
	
	//bool moveVelocityChange = false;
	void FixedUpdate () {
		
		xAxis = Input.GetAxis("Horizontal");
		
		if(xAxis>0){
			rigidbody.AddForce(horForce,0,0,ForceMode.Force);
		}else if(xAxis<0){
			rigidbody.AddForce(-horForce,0,0,ForceMode.Force);
		}
		//animate
		timer+=1;
		if((timer >= 3&&!onGround)||(onGround&&timer >= 7)){
			timer = 0;
			//flipX
			if (xAxis > 0){
				sprite.FlipX = true;	
			}else if( xAxis < 0){
				sprite.FlipX = false;	
			}
			//flipY
			if (gravityswitchHalfDone||onGround){
				if (grafityUpDown){
					sprite.FlipY = true;	
				}else{
				 	sprite.FlipY = false;	
				}
			}
			
			//change frame
			//Debug.Log(frameID+" "+gravityswitchTriggerd+" "+ gravityswitchHalfDone+" "+gravitySwitchDone);
			if (onGround == true){
				if(xAxis==0){
					//idle
					frameID++;
					if(frameID >=3){frameID = 0;};
				}else{
					frameID++;
					if(frameID <=8||frameID >=14){frameID = 9;};	
				}
			}else if(gravityswitchTriggerd){
				//switch
				if(frameID == 25){
					gravityswitchHalfDone = true;
				}
				if(gravityswitchHalfDone && frameID ==18){
					gravitySwitchDone = true;
				}
				if(!gravityswitchHalfDone){
					frameID++;	
				}else{
					if(!gravitySwitchDone){
						frameID--;
					}
				}
				if(frameID <=17){frameID = 18;};
			}
			sprite.spriteId = frameID;
		}
	}
	
	//check if thouching ground
	void onGroundCheck(Collision col){
		//Debug.Log( "Y: "+col.contacts[0].normal.y+" X:" +col.contacts[0].normal.x);
		for(int cols = 0; cols < col.contacts.Length; cols++){
			if (col.contacts[cols].normal.x < 0.1&&col.contacts[cols].normal.x > -0.1){
				onGround = true;	
				gravitySwitchDone = false;
				gravityswitchHalfDone = false;
			}
		}
	}
	
	//collision events 
	void OnCollisionEnter (Collision col)
    {
		onGroundCheck(col);
		gravityswitchTriggerd = false;
	}
	
	void OnCollisionStay ( Collision col ){
		onGroundCheck(col);
	}
	
	void OnCollisionExit(Collision col) {
        onGround = false;
    }
}
