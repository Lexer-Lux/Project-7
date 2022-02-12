using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Loader {
    public static object Load(Type type) { // TODO: Make this take the class itself and do typeof() here.
        string typeString = (string)type.GetField("Resource").GetValue(null);
        var createdObject = UnityEngine.Object.Instantiate(Resources.Load(typeString, type));
        return createdObject;
    }
}
