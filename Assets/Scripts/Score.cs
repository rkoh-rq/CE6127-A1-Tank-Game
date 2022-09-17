using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
public class Score : MonoBehaviour {
    public Text myText;
    public void updateText(string text){
        myText.text = text;
    }
    public void updateColor(Color color){
        myText.color = color;
    }
    void Update () {
        
    }
}
 
