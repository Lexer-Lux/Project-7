using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static HUD;
public sealed class Bar : MonoBehaviour {
    public uint ValuesPerPiece;
    [SerializeField] public GameObject pieceType;
    [SerializeField] private uint pieceHeight;
    [SerializeField] private uint pieceWidth;
    [SerializeField] private string variable;
    private MonoBehaviour subject;
    public string ComponentName;
    [SerializeField] private Color color;
    private int variableValue => (int)subject.GetType().GetProperty(variable).GetValue(subject);
    public void Start() {
        subject = activeHUD.Subject.GetComponent(ComponentName) as MonoBehaviour;
        Render();
    }
    public void Render() {
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
        var i = variableValue;
        while (i > 0) {
            var step = (int)Mathf.Min(i, ValuesPerPiece);
            var newPiece = Instantiate(pieceType);
            newPiece.transform.SetParent(transform, false);
            IPiece newPieceInterface = newPiece.GetComponent<IPiece>(); // * Since you can't cast a GameObject to its type because GameObject is a type, I had to find another way around
            newPieceInterface.slider.minValue = 0;
            newPieceInterface.slider.maxValue = ValuesPerPiece;
            newPieceInterface.slider.wholeNumbers = true;
            newPieceInterface.slider.value = step;
            newPieceInterface.image.color = color;
            i -= step;
        }
    }
}
