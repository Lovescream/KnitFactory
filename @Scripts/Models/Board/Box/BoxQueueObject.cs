using System.Collections.Generic;
using UnityEngine;

public class BoxQueueObject : Entity {

    #region Properties

    public BoxQueue Queue { get; private set; }
    public Transform Current { get; private set; }
    public Transform Next { get; private set; }

    #endregion

    #region Fields

    //private readonly List<Vector2> _positions = new();
    private readonly List<Transform> _positions = new();

    #endregion

    #region Initialize / Set

    public override bool Initialize() {
        if (!base.Initialize()) return false;

        Current = this.gameObject.FindChild<Transform>("Current");
        Next = this.gameObject.FindChild<Transform>("Next");
        
        return true;
    }

    public void Set(BoxQueue queue) {
        Initialize();
        Queue = queue;
        
        this.transform.SetParent(Queue.Board.Object.transform);
        this.transform.position = Queue.Position;
        this.transform.localRotation = Quaternion.identity;// Quaternion.Euler(0, 0, (int)Queue.Direction * -90);
        this.transform.localScale = Vector3.one;
        
        Transform positions = this.gameObject.FindChild<Transform>("Positions");
        _positions.Clear();
        for (int i = 0; i < 6; i++) {
            //_positions.Add(positions.GetChild(i).position);
            _positions.Add(positions.GetChild(i));
        }
    }

    #endregion

    public Vector2 GetCurrentSlotPosition(int index) {
        if (_positions.Count <= index) return Vector2.zero;
        return _positions[index].position;
    }

    public Vector2 GetCurrentSlotLocalPosition(int index) {
        if (_positions.Count <= index) return Vector2.zero;
        return _positions[index].localPosition;
    }
    
}