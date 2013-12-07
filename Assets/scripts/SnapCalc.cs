using UnityEngine;
using System.Collections;

// Food Budget
public class SnapCalc : MonoBehaviour {

    public static SnapCalc instance;

    // poverty level for family of 4
    int householdSize = 4; // 1-8
    int householdIncome = 23550;
    bool useNov1 = false;

    // in the following tables, array index = household size

    // 2013 guidelines, http://aspe.hhs.gov/poverty/13poverty.cfm#thresholds
    int[] povertyTable = new int[] { 0, 11490, 15510, 19530, 23550, 27570, 31590, 35610, 39630 };
    // TODO: use maximum monthly income table instead http://info.dhhs.state.nc.us/olm/manuals/dss/ei-30/man/FSs285.htm

    // as of november 1st 2013 cut, http://info.dhhs.state.nc.us/olm/manuals/dss/ei-30/man/FSs285.htm
    int[] benefitTable = new int[] { 0, 200, 367, 526, 668, 793, 952, 1052, 1202 };
    int[] benefitCutTable = new int[] { 0, 189, 347, 497, 632, 750, 900, 995, 1137 };

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public int GetTotalFoodBudgetPerWeek() {
        return Mathf.RoundToInt( ( ( householdIncome * 0.3f ) + GetSnap() * 1f ) / 12f );
    }

    public int GetSnap() {
        return GetSnap( householdSize, householdIncome, useNov1 );
    }


    public int GetSnap( int houseSize, int houseIncome, bool afterNov1 ) {
        // SNAP rule: you can make no more than 130% of poverty level for your household size
        if ( houseIncome * 1f > povertyTable[houseSize] * 1.3f ) {
            return 0;
        } else {
            // take your possible benefit depending on whether it's before November 1st or not...
            int benefit = 0;
            if ( afterNov1 ) {
                benefit = benefitCutTable[houseSize];
            } else {
                benefit = benefitTable[houseSize];
            }

            // ... then subtract 30% of your monthly income from your possible benefit
            // this is the amount you are expected to spend on food
            // we clamp because this step might make the benefit NEGATIVE (!!!)
            benefit = Mathf.Clamp( Mathf.RoundToInt( benefit * 1f - ( houseIncome / 12f ) * 0.3f ), 0, 100000 );

            return benefit;
        }
    }

    void OnGUI() {
        if ( GUI.Button( new Rect( 256f, 0f, 256f, 32f ), "generate random poor household" ) ) {
            householdSize = Random.Range( 1, 8 );
            householdIncome = Random.Range( 0, povertyTable[householdSize]);
        }
        GUI.Label( new Rect(0f, 8f, 256f, 64f), "household size: " + householdSize.ToString() );
        householdSize = Mathf.RoundToInt( GUI.HorizontalSlider( new Rect( 0f, 0f, 256f, 64f ), householdSize, 1f, 8f ) );
        GUI.Label( new Rect(0f, 72f, 256f, 64f), "household income, annual: $" + householdIncome.ToString() );
        householdIncome = Mathf.RoundToInt( GUI.HorizontalSlider( new Rect( 0f, 64f, 256f, 64f ), householdIncome, 0f, 100000f ) );
        GUI.Label( new Rect( 0f, 96f, 256f, 64f ), "household income, monthly: $" + ( householdIncome / 12 ).ToString() );

        useNov1 = GUI.Toggle( new Rect( 0f, 192f, 256f, 32f ), useNov1, "use post-Nov 1 SNAP cuts?" );
        GUI.Label( new Rect( 0f, 256f, 256f, 64f ), "predicted SNAP benefit per month: $" + GetSnap().ToString() );
    }
}
