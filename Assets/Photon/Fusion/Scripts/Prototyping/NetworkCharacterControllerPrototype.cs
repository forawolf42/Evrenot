using Fusion;
using UnityEngine;
using UnityEngine.AI;

[OrderBefore(typeof(NetworkTransform))]
[DisallowMultipleComponent]
// ReSharper disable once CheckNamespace
public class NetworkCharacterControllerPrototype : NetworkTransform
{
    [Networked] [HideInInspector] public Vector3 Velocity { get; set; }
    public NavMeshAgent Controller { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CacheController();
    }

    public override void Spawned()
    {
        base.Spawned();
        CacheController();
        Controller.Move(transform.position);
    }

    private void CacheController()
    {
        if (Controller == null)
        {
            Controller = GetComponent<NavMeshAgent>();
            Assert.Check(Controller != null,
                $"An object with {nameof(NetworkCharacterControllerPrototype)} must also have a {nameof(CharacterController)} component.");
        }
    }

    protected override void CopyFromBufferToEngine()
    {
        // Trick: CC must be disabled before resetting the transform state
        Controller.enabled = false;

        // Pull base (NetworkTransform) state from networked data buffer
        base.CopyFromBufferToEngine();

        // Re-enable CC
        Controller.enabled = true;
    }

    public Vector3 worldPosition = Vector3.back;

    Plane plane = new Plane(Vector3.up, 0);

    public virtual void Move()
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }

        Controller.destination = worldPosition;
    }
}