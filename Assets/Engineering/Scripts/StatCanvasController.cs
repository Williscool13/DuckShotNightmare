using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StatCanvasController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;

        shotCount.text = shotCountVar.Value.ToString();
        hitCount.text = hitCountVar.Value.ToString();
        killCount.text = killCountVar.Value.ToString();
        scoreCount.text = scoreCountVar.Value.ToString();
        comboCount.text = highestComboCountVar.Value.ToString() + "x";
    }
    [SerializeField] TextMeshProUGUI shotCount;
    [SerializeField] TextMeshProUGUI hitCount;
    [SerializeField] TextMeshProUGUI killCount;
    [SerializeField] TextMeshProUGUI scoreCount;
    [SerializeField] TextMeshProUGUI comboCount;

    [SerializeField] IntegerReference shotCountVar;
    [SerializeField] IntegerReference hitCountVar;
    [SerializeField] IntegerReference killCountVar;
    [SerializeField] IntegerReference highestComboCountVar;
    [SerializeField] IntegerReference scoreCountVar;


    // Update is called once per frame
    void Update()
    {
        
    }
}
