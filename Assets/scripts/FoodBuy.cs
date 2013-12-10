using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodBuy : MonoBehaviour {

    // TODO:
    // - add categories, automatic stocking shelves
    // - add a "basket" that automatically collects food, then you click to dump it on the belt and begin scanning
    // - add voiding / returning an item
    // - add a rationing scheduler, tells you how you split the food per day per member of family

    // TODO, V2:
    // - add a "shopping list", you choose what meals each member of household can have each day, shop for the list
    // - you define members of your household... parents, single parent, senior, son, daughter... (check statistics, put it in RNG)
    // - add touch support for iOS/Android

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
        UpdateCart();
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
            // UpdateCart();

            // move item along
            if ( item == currentHeld )
                currentHeld = null;
            item.rigidbody.velocity = transform.forward * 15f - transform.up * 3f;
        }
    }

    public void UpdateCart() {
        string cartText = "";
        float cartTotal = 0f;
        foreach ( FoodItem thing in cart ) {
            cartText += thing.ToScanString( false ) + "\n";
            cartTotal += thing.foodPrice;
        }
        float ebt = 1f * SnapCalc.instance.benefit / 4f;
        textCart.text = cartText + "==========\nTOTAL COST: $" + cartTotal.ToString( "F2" ) + "\n" +
                        "weekly EBT: -$" + ebt.ToString( "F2" ) + "\n" +
                        "you pay: $" + Mathf.Clamp( cartTotal - ebt, 0f, 1000f ).ToString( "F2" );
    }

    void FixedUpdate() {
        if ( currentHeld != null ) {
            RaycastHit rayHit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

            if ( Physics.Raycast( ray, out rayHit, 100000f)) {
                Vector3 idealPos = rayHit.point + Vector3.up * 1f;
                idealPos.z = 0f;
                Vector3 idealForce = ( idealPos - currentHeld.transform.position );
                float force = -1f * (-1f + Vector3.Dot( currentHeld.rigidbody.velocity.normalized, idealForce.normalized ));
                currentHeld.rigidbody.AddForce( idealForce * force, ForceMode.VelocityChange );
            }
        }
    }

}
