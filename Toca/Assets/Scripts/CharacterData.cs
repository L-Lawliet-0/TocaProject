using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData // inheirt mono behaviour so that object can be saved
{
    public int ID_mianju, ID_maozi, ID_glasses, ID_kouzhao, ID_yanjing, ID_bizi, ID_zui, ID_toufa, ID_toufahoumian;
    public int ID_hairColor, ID_skinColor;
    public int ID_skin;
    public string ID_tou;

    public int UNIQUE_ID;

    // initalize data
    public void InitData()
    {
        ID_mianju = ID_maozi = ID_glasses = ID_kouzhao = -1;
        ID_yanjing = ID_bizi = ID_zui = ID_toufa = 1;
        ID_skin = 0;
        ID_tou = "tou";
        ID_toufahoumian = -1;
    }

    public void RandomizeData()
    {
        ID_mianju = ID_maozi = ID_glasses = ID_kouzhao = -1;
        ID_yanjing = Random.Range(1, 33);
        ID_bizi = Random.Range(1, 32);
        ID_zui = Random.Range(1, 29);
        ID_toufa = Random.Range(1, 41);
        ID_tou = Random.Range(0f, 1f) < .5f ? "tou" : "tou2";
        ID_toufahoumian = -1;

        ID_hairColor = Random.Range(0, CharacterCreation.HairColors.Length);
        ID_skinColor = Random.Range(0, CharacterCreation.SkinColors.Length);
        ID_skin = 0;
    }
}
