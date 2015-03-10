using System;
using System.Collections.Generic;
using UnityEngine;

public class ThrowCustomException : MonoBehaviour
{
    public void Start()
    {
        throw new CustomException();
    }

    private class CustomException : Exception
    {
    }
}
