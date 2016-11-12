using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("Sprite/SpriteSize")]
[RequireComponent (typeof (SpriteRenderer))]
public class SpriteSize : MonoBehaviour{

	#region Unity scene settings
	public Vector2 size;
	public bool PreserveAspect = false;
	[Header("Move")]
	public float Speed = 0f;
	[Header("Blink")]
    [SerializeField]
    private bool Blink = false;
	[SerializeField] private float BlinkTime;
	[SerializeField] private float Alpha1;
	[SerializeField] private float Alpha2;
	#endregion

	#region Data
	float Alpha = 0f;
	bool Increase = true;
	SpriteRenderer mySpriteRenderer;
	Transform myTransform;
	#endregion
	
	#region Interface
	public void OnValidate() {
		UpdateSize ();
	}

	public void Initialize () {
		mySpriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		myTransform = transform;
	}

	public void UpdateSize() {
		if (mySpriteRenderer == null)
			Initialize ();
		if (mySpriteRenderer.sprite != null) {
			Vector2 realSize = GetDimensionInPX ();
			if (PreserveAspect) { 
				size.y = size.x / (mySpriteRenderer.sprite.textureRect.width / mySpriteRenderer.sprite.textureRect.height);
			}
			if ((Mathf.RoundToInt (realSize.x * 1000f) != Mathf.RoundToInt (size.x * 1000f)) || (Mathf.RoundToInt (realSize.y * 1000f) != Mathf.RoundToInt (size.y * 1000f))) {
				if ((size.x > 0) && (size.y > 0)) {
					SetSize (size.x, size.y);
				}
				realSize = GetDimensionInPX ();
			}
			size.x = realSize.x;
			size.y = realSize.y;
			if (Blink) {
				if (IsInvoking ("BlinkStars") != true) {
					InvokeRepeating ("BlinkStars", 0f, BlinkTime / (Alpha2 - Alpha1));
				}
			} else {
				if (IsInvoking ("BlinkStars")) {
					CancelInvoke ("BlinkStars");
				}
			}
		} 
		//else
			//Debug.LogWarning ("Sprite is non-assigned!!!");
	}

	public void SetNativeSize() {
		if (myTransform == null)
			Initialize ();
		if (mySpriteRenderer.sprite != null) {
			myTransform.localScale = Vector3.one;
			Vector2 realSize = GetDimensionInPX ();
			size.x = realSize.x;
			size.x = realSize.y;
		}
	}

	#endregion

	#region Methods
	void Awake() {
		Initialize ();
	}
	
	void BlinkStars () {
		Color SpriteColor = Color.blue;
		if (Increase) {
			if (Alpha < Alpha2)
				Alpha++;
			else {
				Alpha --;
				Increase = false;
			}
		} else {
			if (Alpha >Alpha1) Alpha --;
			else {
				Alpha++;
				Increase = true;
			}
		}
		if (mySpriteRenderer == null) {
			mySpriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		} else {
			SpriteColor = mySpriteRenderer.color;
			SpriteColor.a = Alpha/255f;
			mySpriteRenderer.color = SpriteColor;
		}
	}
	
	
	Vector2 GetDimensionInPX() { 
		Vector2 tmpDimension = Vector2.zero;
		if (myTransform == null || mySpriteRenderer==null)
			Initialize ();
		if (mySpriteRenderer.sprite != null) {
			float pixelsPerUnitBack = 1f/mySpriteRenderer.sprite.pixelsPerUnit;
			tmpDimension.x = myTransform.lossyScale.x* mySpriteRenderer.sprite.textureRect.width *pixelsPerUnitBack;
			tmpDimension.y = myTransform.lossyScale.y *	mySpriteRenderer.sprite.textureRect.height *pixelsPerUnitBack;
		}
		return tmpDimension;
	}
	
	
	
	void SetSize (float x_size, float y_size) {
        Vector3 scale = myTransform.localScale;
        Vector3 realSize = GetDimensionInPX();
        float ratio_x = x_size / realSize.x;
        float ratio_y = y_size / realSize.y;
		scale.x *= ratio_x;
		scale.y *= ratio_y;
		myTransform.localScale = scale;
		//Debug.LogFormat ("{0} {1} {2} {3} {4}", gameObject.name, x_size, y_size, GetDimensionInPX().x, GetDimensionInPX().y);
	}

	#endregion
	
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpriteSize))]
public class SpriteSizeEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		SpriteSize myScript = (SpriteSize)target;
		if(GUILayout.Button("Set Native Size"))
		{
			myScript.SetNativeSize ();
		}
       /* if (myScript.Blink)
        {

        }*/
	}
}
#endif