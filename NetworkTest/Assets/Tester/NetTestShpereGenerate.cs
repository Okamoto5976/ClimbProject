using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NetTestShpereGenerate : MonoBehaviour
{


    [SerializeField] private GameObject m_testSphere;

    [SerializeField] private int m_count;

    private List<Transform> m_sphereObj = new();

    private Vector2 moveInput = Vector2.zero;

    private void Start()
    {
        for(int i = 0; i < m_count; i++)
        {
            var obj = Instantiate(m_testSphere, this.transform);

            float x = Random.Range(0f, 10f);
            float z = Random.Range(0f, 10f);

            obj.transform.position = new Vector3(x, 0, z);

            m_sphereObj.Add(obj.transform);
        }

    }

    private void Update()
    {
        var move = new Vector3(moveInput.x, 0f, moveInput.y) * 5f * Time.deltaTime;

        foreach(var obj in m_sphereObj)
        {
            obj.Translate(move);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();


    }

}
