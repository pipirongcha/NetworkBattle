using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour //플레이어들이 동시에 움직이는 걸 픽스하자! NetworkBehavior 상속
{
    //NetworkVariable<int> randomNumber = new NetworkVariable<int>(
    //    1, 
    //    NetworkVariableReadPermission.Everyone, // 모든 대상자가 읽을 수 있는 권한
    //    NetworkVariableWritePermission.Owner // 소지자만 작성할 수 있는 권한
    //    );
    ////클라이언트가 T 키다운 시, 에러가 발생하지 않도록 권한을 조정해준 것


    struct SomeData : INetworkSerializable
    {
        public bool _bool;
        public int _int;
        public FixedString32Bytes _string;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _string);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += HandleRandomNumber;
    }

    public override void OnNetworkDespawn()
    {
        randomNumber.OnValueChanged -= HandleRandomNumber;
    }

    void HandleRandomNumber(SomeData oldValue, SomeData newValue)
    {
        Debug.Log($"onwer ID : {OwnerClientId}, random number : {randomNumber.Value._int}, string : {randomNumber.Value._string}");
    }

    NetworkVariable<SomeData> randomNumber = new NetworkVariable<SomeData>(new SomeData
    {
        _bool = true,
        _int = 0
    },
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    void Update()
    {
        //Debug.Log($"onwer ID : {OwnerClientId}, random number : {randomNumber.Value}");

        // NetworkBehaviour에서 IsOwner 활용
        if (!IsOwner) { return; }

        if (Input.GetKeyDown(KeyCode.Z)) //Z를 누르면 호스트(서버)에서만 콜
        {
            TestServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.T)) //호스트에서 T 키다운 시, Owner[0] (=호스트)만 random number가 바뀜
        {
            //randomNumber.Value = Random.Range(0, 100);
            randomNumber.Value = new SomeData { _int = Random.Range(0, 100), _string = "32바이트안넘게" };
        }


        Vector3 moveDir = Vector3.zero;

        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.z = Input.GetAxis("Vertical");

        float moveSpeed = 3;

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

      
    }
    [ServerRpc] //서버에서 실행시킴 (실행시킬 함수명이 ServerRpc로 끝나야함)
    void TestServerRpc()
    {
        Debug.Log("Server RPC called");
    }
}