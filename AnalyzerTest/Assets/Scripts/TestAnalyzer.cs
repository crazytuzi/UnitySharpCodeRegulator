using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestAnalyzer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int a = 0;
        if (a == 0)
        {
            a = 1;
        }
        else
        {
            a = 2;
        }
        TestClass tt = new TestClass();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


/// <summary>
/// ����һ�������࣬��Ҫ����ǿ��ע�͵Ĳ���
/// </summary>
 class TestClass
{
    /// <summary>
    /// ����һ�����Է������û����Է���ע��
    /// </summary>
    public void Test()
    {

    }
}
