using System;
using UnityEngine;

public class StudRopeObject : Entity
{
    public StudRope StudRope { get; private set; }

    private RopeRenderer _ropeOutter;
    private RopeRenderer _ropeInner;
    private PointFollower _pointCenter;
    private Transform _pointBezier;

    private Transform _start;
    private Transform _end;
    
    private readonly Vector3 _baseScale = Vector3.one; 

    private void Update()
    {
        if (_start == null || _end == null) return;
        UpdateRopeTransform();
    }

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;
        
        _ropeOutter = gameObject.FindChild<RopeRenderer>("Line_Outter");
        _ropeInner = gameObject.FindChild<RopeRenderer>("Line_Inner");
        _pointCenter = gameObject.FindChild<PointFollower>("Point_Center");
        _pointBezier =  gameObject.FindChild<Transform>("Point_Bezier");

        return true;
    }

    public void Set(StudRope studRope)
    {
        Initialize();
        _start = null;
        _end = null;
        StudRope = studRope;
        transform.SetParent(studRope.Group.Object.transform, false);
    }

    public void Destroy()
    {
        Main.Object.Destroy(this);
    }

    public void SetStartTransform(Transform start)
    {
        _start = start;
        _ropeInner.LeftHandle = start;
        _ropeOutter.LeftHandle = start;
        _pointCenter.LeftHandle = start;
        _pointBezier.position = start.position.SetZ(0);
    }

    public void SetEndTransform(Transform end)
    {
        _end = end;
        _ropeInner.RightHandle = end;
        _ropeOutter.RightHandle = end;
        _pointCenter.RightHandle = end;
        _pointBezier.position = end.position.SetZ(0);
    }
    
    private void UpdateRopeTransform()
    {
        // Vector3 startPos = _start.position;
        // RectTransform rect = _end as RectTransform;
        // Vector3 endPos = _end.position;
        //
        // if (rect != null)
        // {
        //     endPos = rect.position;
        //     endPos.z = startPos.z;
        // }
        //
        // transform.position = (startPos + endPos) / 2f;
        //
        // float distance = Vector3.Distance(startPos, endPos);
        // transform.localScale = new Vector3(distance, _baseScale.y, _baseScale.z);
        //
        // Vector3 direction = endPos - startPos;
        //
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //
        // transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}