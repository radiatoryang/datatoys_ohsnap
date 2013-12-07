using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodBuy : MonoBehaviour {

    public static FoodBuy instance;
    public TextMesh textScanner;
    public TextMesh textCart;
    public Renderer scannerMesh;

    public List<FoodItem> cart = new List<FoodItem>();
    public FoodItem currentHeld;

	// Use this for initialization
	void Start () {
        instance = this;
        textScanner.text = "Click + drag items\nover scanner.";
	}
	
	// Update is called once per frame
	void Update () {
        scannerMesh.material.color = Color.Lerp( scannerMesh.material.color, new Color( 0.2f, 0.02f, 0f, 0f ), Time.deltaTime * 5f );
	}

    void OnTriggerEnter(Collider c) {
        if ( c.GetComponent<FoodItem>() ) {
            FoodItem item = c.GetComponent<FoodItem>();

            // register feedback
            audio.Play();
            textScanner.text = item.ToScanString(true);
            scannerMesh.material.color = new Color( 1f, 0.25f, 0f, 1f );

            // add to cart, update cart
            cart.Add( item );
            string cartText = "";
            float cartTotal = 0f;
            foreach ( FoodItem thing in cart ) {
                cartText += thing.ToScanString( false ) + "\n";
                cartTotal += thing.foodPrice;
            }
            textCart.text = cartText + "TOTAL COST: $" + cartTotal.ToString("F2");

            // move item along
            if ( item == currentHeld )
                currentHeld = null;
            item.rigidbody.velocity = transform.forward * 15f - transform.up * 3f;
        }
    }

    void FixedUpdate() {
        if ( currentHeld != null ) {
            RaycastHit rayHit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

            if ( Physics.Raycast( ray, out rayHit, 100000f)) {
                Vector3 idealPos = rayHit.point + Vector3.up * 1f;
                idealPos.z = 0f;
                Vector3 idealForce = ( idealPos - currentHeld.transform.position );
                float force = -0.6f * (-1f + Vector3.Dot( currentHeld.rigidbody.velocity.normalized, idealForce.normalized ));
                currentHeld.rigidbody.AddForce( idealForce * force, ForceMode.VelocityChange );
            }
        }
    }

}
