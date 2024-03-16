using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rot : MonoBehaviour
{
    private float valueY;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.rotation = Quaternion.Euler(transform.rotation.x,transform.rotation.y, transform.rotation.z);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Quaternion.Euler(transform.rotation.x,transform.rotation.y + valueY, transform.rotation.z);

    }

    public void ADDY()
    {
        valueY += 10;
    }
    public void ReduceY()
    {
        valueY -= 10;
    }
    public delegate void RotDelegate();
    private bool isPunEnabled;
    public bool IsPunEnabled
    {
        set => isPunEnabled = value;
    }
    public void PUNADDY()
    {
        if (isPunEnabled)
            OnPUNADDY?.Invoke();
        else
            ADDY();
    }
    public event RotDelegate OnPUNADDY;
    public void PUNReduceY()
    {
        if (isPunEnabled)
            OnPUNReduceY?.Invoke();
        else
            ReduceY();
    }
    public event RotDelegate OnPUNReduceY;
}
