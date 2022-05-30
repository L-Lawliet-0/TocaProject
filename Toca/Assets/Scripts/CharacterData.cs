using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData // inheirt mono behaviour so that object can be saved
{
    public int ID_mianju, ID_maozi, ID_glasses, ID_kouzhao, ID_yanjing, ID_bizi, ID_zui, ID_toufa, ID_toufahoumian;
    public int ID_hairColor, ID_skinColor;
    public string ID_tou;

    // initalize data
    public void InitData()
    {
        ID_mianju = ID_maozi = ID_glasses = ID_kouzhao = -1;
        ID_yanjing = ID_bizi = ID_zui = ID_toufa = 1;
        ID_tou = "tou";
        ID_toufahoumian = -1;
    }
}
