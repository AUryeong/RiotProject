using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] private Transform viewPos;

    private const float DEFAULT_SIZE = 3.5f;
    private const float SIZE_MULTIPLIER = DEFAULT_SIZE / 3;

    private void Awake()
    {
        if (viewPos == null)
            viewPos = transform.parent;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
    }

    private void FixedUpdate()
    {
        transform.localScale = viewPos.transform.position.y < -1 ? Vector3.zero : Vector3.one * Mathf.Clamp(DEFAULT_SIZE - viewPos.transform.position.y* SIZE_MULTIPLIER, 0, DEFAULT_SIZE);
    }
}
