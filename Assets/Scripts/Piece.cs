using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IPiece {
    public Slider slider => _slider;
    [SerializeField] private Slider _slider; // * Set this in the Unity Editor, then slider returns that value (as interfaces can't have properties), then the bar holding this piece gets the slider through the IPiece interface (as you can't get a type through a GameObject), so we can set the slider values. Sigh.
    public Image image => _image;
    [SerializeField] private Image _image;
}
interface IPiece {
    public Slider slider { get; }
    public Image image { get; }
}
