using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour //�÷��̾���� ���ÿ� �����̴� �� �Ƚ�����! NetworkBehavior ���
{
    //NetworkVariable<int> randomNumber = new NetworkVariable<int>(
    //    1, 
    //    NetworkVariableReadPermission.Everyone, // ��� ����ڰ� ���� �� �ִ� ����
    //    NetworkVariableWritePermission.Owner // �����ڸ� �ۼ��� �� �ִ� ����
    //    );
    ////Ŭ���̾�Ʈ�� T Ű�ٿ� ��, ������ �߻����� �ʵ��� ������ �������� ��


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

        // NetworkBehaviour���� IsOwner Ȱ��
        if (!IsOwner) { return; }

        if (Input.GetKeyDown(KeyCode.Z)) //Z�� ������ ȣ��Ʈ(����)������ ��
        {
            TestServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.T)) //ȣ��Ʈ���� T Ű�ٿ� ��, Owner[0] (=ȣ��Ʈ)�� random number�� �ٲ�
        {
            //randomNumber.Value = Random.Range(0, 100);
            randomNumber.Value = new SomeData { _int = Random.Range(0, 100), _string = "32����Ʈ�ȳѰ�" };
        }


        Vector3 moveDir = Vector3.zero;

        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.z = Input.GetAxis("Vertical");

        float moveSpeed = 3;

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

      
    }
    [ServerRpc] //�������� �����Ŵ (�����ų �Լ����� ServerRpc�� ��������)
    void TestServerRpc()
    {
        Debug.Log("Server RPC called");
    }
}