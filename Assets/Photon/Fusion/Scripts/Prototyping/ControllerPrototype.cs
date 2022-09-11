using UnityEngine;
using Fusion;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class ControllerPrototype : Fusion.NetworkBehaviour
{
    protected NetworkCharacterControllerPrototype _ncc;
    protected NetworkRigidbody _nrb;
    protected NetworkRigidbody2D _nrb2d;
    protected NetworkTransform _nt;

    [Networked] public Vector3 MovementDirection { get; set; }

    public bool TransformLocal = false;

    [DrawIf(nameof(ShowSpeed), DrawIfHideType.Hide, DoIfCompareOperator.NotEqual)]
    public float Speed = 6f;

    bool HasNCC => GetComponent<NetworkCharacterControllerPrototype>();

    bool ShowSpeed => this && !TryGetComponent<NetworkCharacterControllerPrototype>(out _);

    public void Awake()
    {
        CacheComponents();
    }

    public override void Spawned()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        if (!_ncc) _ncc = GetComponent<NetworkCharacterControllerPrototype>();
        if (!_nrb) _nrb = GetComponent<NetworkRigidbody>();
        if (!_nrb2d) _nrb2d = GetComponent<NetworkRigidbody2D>();
        if (!_nt) _nt = GetComponent<NetworkTransform>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.None)
        {
            return;
        }

        if (_ncc)
        {
            if (GetInput(out NetworkInputPrototype inputt))
            {
                if (inputt.IsDown(NetworkInputPrototype.MOUSE_DOWN))
                {
                    if (CameraFollower.instance.target == null)
                    {
                        CameraFollower.instance.target = transform;
                    }

                    _ncc.Move();
                }
            }
        }
    }
}