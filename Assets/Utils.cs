using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

	public static Vector3 GetBottomPoint(Mesh mesh)
    {
        Vector3 bottom = new Vector3(0, float.MaxValue, 0);

        foreach(Vector3 vec in mesh.vertices)
        {
            if(vec.y < bottom.y)
            {
                bottom = vec;
            }
        }

        return bottom;
    }
}
