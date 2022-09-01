using Sirenix.OdinInspector;
using UnityEngine;

namespace UI {
    /// <summary> Things with this interface can easily track a variable with a bar using UI.SegmentedBar </summary>
    public interface IBarTrackable { }
    public sealed class SegmentedBar : MonoBehaviour {
        public                                               uint          ValuesPerPiece;
        [SerializeField, Required, AssetsOnly]       public  GameObject    pieceType;
        [SerializeField]                             private uint          pieceHeight;
        [SerializeField]                             private uint          pieceWidth;
        [ShowInInspector, Required, SceneObjectsOnly, SerializeField ] private IBarTrackable subject;
        [SerializeField, Required]                   private Color         color;
        [SerializeField]                             private string        variable;
        private                                              int           variableValue => (int)subject.GetType().GetProperty(variable).GetValue(subject);

        [Button, HideInEditorMode] public void Render() {
            foreach(Transform child in transform) {
                Destroy(child.gameObject);
            }
            int i = variableValue;
            while (i > 0) {
                int        step              = (int)Mathf.Min(i, ValuesPerPiece);
                GameObject newPiece          = Instantiate(pieceType, transform, false);
                IPiece     newPieceInterface = newPiece.GetComponent<IPiece>(); // * Since you can't cast a GameObject to its type because GameObject is a type, I had to find another way around
                newPieceInterface.slider.minValue     =  0;
                newPieceInterface.slider.maxValue     =  ValuesPerPiece;
                newPieceInterface.slider.wholeNumbers =  true;
                newPieceInterface.slider.value        =  step;
                newPieceInterface.image.color         =  color;
                i                                     -= step;
            }
        }
    }
}
