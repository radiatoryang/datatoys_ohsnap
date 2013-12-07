using UnityEngine;
using System.Collections;

public class FoodItem : MonoBehaviour {

    // these should be automatically generated from a spreadsheet
    public string foodName = "Grocery";
    public float foodPrice = 1.50f;
    public int foodWeight = 0;
    public Renderer front;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public string ToScanString(bool newLine) {
        return foodName + ( newLine ? "\n" : "    ") + "$" + foodPrice.ToString( "F2" );
    }

    void OnMouseDown() {
        FoodBuy.instance.currentHeld = this;
    }

    void OnMouseUp() {
        if ( FoodBuy.instance.currentHeld == this )
            FoodBuy.instance.currentHeld = null;
    }
}
