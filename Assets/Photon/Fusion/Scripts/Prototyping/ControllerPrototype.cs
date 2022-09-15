using System;
using System.Collections.Generic;
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
    public GameObject bulletPrefab = null;
    public GameObject bulletsPivot = null;

    public void Awake()
    {
        CacheComponents();
       
    }

    public override void Spawned()
    {
        CacheComponents();
        
   
    }

    private void Start()
    {
        if (Object.HasInputAuthority)
        {
            Rpc_ObjectPoolint();
        }
    }

    private void CacheComponents()
    {
        if (!_ncc) _ncc = GetComponent<NetworkCharacterControllerPrototype>();
        if (!_nrb) _nrb = GetComponent<NetworkRigidbody>();
        if (!_nrb2d) _nrb2d = GetComponent<NetworkRigidbody2D>();
        if (!_nt) _nt = GetComponent<NetworkTransform>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SpawnPlaceable()
    {
        NetworkObject bult = Objects.Dequeue();
        Objects.Enqueue(bult);
        bult.GetComponent<Rigidbody>().Sleep();
        bult.GetComponent<Rigidbody>().WakeUp();
        bult.transform.gameObject.SetActive(true);
        bult.transform.rotation = bulletsPivot.transform.rotation;
        bult.transform.position = bulletsPivot.transform.position;
        bult.GetComponent<Rigidbody>().AddForce(bult.transform.forward * 5000);
    }

    public Queue<NetworkObject> Objects = new Queue<NetworkObject>();
   
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    void Rpc_ObjectPoolint()
    {
        GameObject obj = new GameObject();
        obj.name = "PlayerBullets";
        for (int i = 0; i < 25; i++)
        {
            NetworkObject bult = Runner.Spawn(bulletPrefab, Vector3.zero, Quaternion.identity, Runner.LocalPlayer);
            Objects.Enqueue(bult);
            bult.transform.parent = obj.transform;
            bult.gameObject.SetActive(false);
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            other.gameObject.SetActive(false);
            GetComponent<HealthManager>().ShotShip();
//            Destroy(this.gameObject);
        }    
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

                if (inputt.IsDown(NetworkInputPrototype.SPACE))
                {
                    if (Object.HasInputAuthority)
                    {
                        RPC_SpawnPlaceable();
                    }
                   //
                }
            }
        }
    }
}