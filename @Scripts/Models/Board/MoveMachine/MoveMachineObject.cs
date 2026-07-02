using UnityEngine;

public class MoveMachineObject : Entity
{
    public MoveMachine MoveMachine { get; private set; }
    
    private SpriteRenderer _spriter;

    private float _timer;

    public override bool Initialize()
    {
        if (!base.Initialize()) return false;
        
        _spriter = gameObject.FindChild<SpriteRenderer>("Sprite_MoveMachine");

        return true;
    }

    public void Set(MoveMachine moveMachine)
    {
        Initialize();
        MoveMachine = moveMachine;
        transform.SetParent(MoveMachine.Board.Object.transform);
        this.transform.position = MoveMachine.Position;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
        SetSprite();
    }

    private void SetSprite()
    {
        Sprite sprite = Main.Resource.Get<Sprite>($"Knit_MoveMachine_{MoveMachine.Data.Orientation}");
        _spriter.sprite = sprite;
    }

    public void Destroy()
    {
        Main.Object.Destroy(this);
    }
}
