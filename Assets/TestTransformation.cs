using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransformation : MonoBehaviour
{
    private int depth;

    Vector3 Rotation_row1 = new Vector3(0.999979f, 0.00648094f, - 0.000547361f);
    Vector3 Rotation_row2 = new Vector3(-0.00639222f, 0.994841f, 0.101247f);
    Vector3 Rotation_row3 = new Vector3(0.00120071f, -0.101242f, 0.994861f);

    Vector3 Translation = new Vector3(-32.0199f, -1.9547f, 3.81727f);

    float cx_color = 641.683f;
    float cy_color = 366.42f;
    float fx_color = 608.248f;
    float fy_color = 608.163f;

    Vector3 spaceTable1 = new Vector3(-0.95f, -0.95f, 1.00f);
    Vector3 spaceTable2 = new Vector3(-0.58f, -0.84f, 1.00f);
    Vector3 spaceTable3 = new Vector3(-0.06f, -0.76f, 1.00f);
    Vector3 spaceTable4 = new Vector3(0.18f, -0.77f, 1.00f);
    Vector3 spaceTable5 = new Vector3(0.44f, -0.80f, 1.00f);
    Vector3 spaceTable6 = new Vector3(-0.53f, 0.00f, 1.00f);
    Vector3 spaceTable7 = new Vector3(0.33f, 0.30f, 1.00f);

    Vector3 pointPosition = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        depth = 3000;

        pointPosition.x = depth * spaceTable3.x;
        pointPosition.y = depth * spaceTable3.y;
        pointPosition.z = depth;

        Vector3 Rotated = new Vector3();
        Rotated.x = Rotation_row1.x * pointPosition.x + Rotation_row1.y * pointPosition.y + Rotation_row1.z * pointPosition.z;
        Rotated.y = Rotation_row2.x * pointPosition.x + Rotation_row2.y * pointPosition.y + Rotation_row2.z * pointPosition.z;
        Rotated.z = Rotation_row3.x * pointPosition.x + Rotation_row3.y * pointPosition.y + Rotation_row3.z * pointPosition.z;

        Vector3 Transformed = Rotated + Translation;

        double u = fx_color * Transformed.x / Transformed.z + cx_color;
        double v = fy_color * Transformed.y / Transformed.z + cy_color;

        print("UV coordinates:" + u + "," + v);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
