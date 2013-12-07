using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Google.GData.Client;
using Google.GData.Spreadsheets;

public class FoodSpawn : MonoBehaviour {
    public FoodItem foodItemPrefab; // assign in inspector
    List<FoodItem> foodItems = new List<FoodItem>();

    ListFeed sheet;
    Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
    bool dataIsReady = false;
    const int debugLimit = 3;

	// Use this for initialization
	void Start () {
        StartCoroutine( RefreshData() );
        StartCoroutine( SpawnFood() );
	}

    IEnumerator RefreshData() {
        dataIsReady = false;
        sheet = GDocService.GetSpreadsheet( "0Ak-N8rbAmu7WdGRFdllybTBIaU1Ic0FxYklIbk1vYlE" );
        int counter = 0;
        foreach ( ListEntry row in sheet.Entries ) {
            Texture2D newTexture = new Texture2D( 256, 256, TextureFormat.DXT1, true);
            WWW www = new WWW( row.Elements[3].Value );
            yield return www;
            www.LoadImageIntoTexture( newTexture );
            cachedTextures.Add( row.Elements[3].Value, newTexture );
            counter++;
            if ( counter >= debugLimit ) {
                break;
            }
        }
        dataIsReady = true;
    }

    IEnumerator SpawnFood() {
        while ( !dataIsReady ) {
            yield return 0;
        }
        int counter = 0;
        foreach ( ListEntry row in sheet.Entries ) {
            // 0 = name, 1 = price, 2 = quantity, 3 = packFrontPhoto, 4 = packColor, 5 = packSize
            FoodItem foodItem = Instantiate( foodItemPrefab, Random.insideUnitSphere * 2f, Quaternion.identity ) as FoodItem;
            foodItem.front.material.mainTexture = cachedTextures[ row.Elements[3].Value ];
            foodItem.foodName = row.Elements[0].Value;
            foodItem.foodPrice = float.Parse(row.Elements[1].Value.TrimStart("$" [0]) );
            string[] color = row.Elements[4].Value.Split("," [0]);
            foodItem.renderer.material.color = new Color( float.Parse( color[0] ), float.Parse( color[1] ), float.Parse( color[2] ), 1f );
            string[] size = row.Elements[5].Value.Split( ","[0] );
            foodItem.transform.localScale = new Vector3( float.Parse( size[0] ), float.Parse( size[1] ), float.Parse( size[2] ) );
            foodItems.Add( foodItem );

            counter++;
            if ( counter >= debugLimit ) {
                break;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
