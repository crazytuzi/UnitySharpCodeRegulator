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
/// 这是一个测试类，主要用于强制注释的测试
/// </summary>
 class TestClass
{
    /// <summary>
    /// 这是一个测试方法，用户测试方法注释
    /// </summary>
    public void Test()
    {

    }
}
