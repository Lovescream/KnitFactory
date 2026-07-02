using UnityEngine;
using UnityEngine.UI;

public class KnitLayoutGroup : LayoutGroup {

    [SerializeField] private float leftX = 0f;
    [SerializeField] private float rightX = 35f;
    [SerializeField] private float lineGap = 30f;
    [SerializeField] private float slantOffset = 10f;
    [SerializeField] private Vector2 size = new(50f, 50f);

    public override void CalculateLayoutInputHorizontal() {
        base.CalculateLayoutInputHorizontal();
        SetDirty();
    }

    public override void CalculateLayoutInputVertical() {
        SetDirty();
    }

    public override void SetLayoutHorizontal() {
        ApplyLayout();
    }

    public override void SetLayoutVertical() {
        ApplyLayout();
    }

    protected override void OnTransformChildrenChanged() {
        base.OnTransformChildrenChanged();
        SetDirty();
    }

    protected override void OnRectTransformDimensionsChange() {
        base.OnRectTransformDimensionsChange();
        SetDirty();
    }

    private void ApplyLayout() {
        int count = rectChildren.Count;
        for (int i = 0; i < count; i++) {
            RectTransform child = rectChildren[i];
            bool isLeft = i % 2 == 0;
            float x = isLeft ? leftX : rightX;
            float y = +(i / 2) * lineGap;
            if (!isLeft) y += slantOffset;

            x -= child.rect.width * child.pivot.x;
            y -= child.rect.height * child.pivot.y;
            
            SetChildAlongAxis(child, 0, x);
            SetChildAlongAxis(child, 1, y);

            child.sizeDelta = size;
        }
        float sizeX = size.x + (rightX - leftX);
        float sizeY = size.y + (int)(count / 2) * slantOffset + (int)((count - 1) / 2) * (lineGap - slantOffset);
        rectTransform.sizeDelta = new(sizeX, sizeY);
    }

}