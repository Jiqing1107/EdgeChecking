using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestPointCloudEdge : MonoBehaviour
{
    private int megaframe_width = 1280;
    private int megaframe_height = 1296;
    private int color_width = 1280;
    private int color_height = 720;
    private int depth_width = 640;
    private int depth_height = 576;
    private int pointCount;

    private Texture2D videoFrame;
    private string filePath = "Assets/frame_24.png";
    private Color[] pixels;
    private float[,] depthMap;
    private float[,] lowbitMap;
    private float[,] highbitMap;
    private bool[,] edgeMap; // To mark edge pixels

    float cx_depth = 322.973f;
    float cy_depth = 335.085f;
    float fx_depth = 504.224f;
    float fy_depth = 504.436f;

    public Material heatmapMaterial;
    private string lowbitMapFilePath = "Assets/lowbitMap.txt"; // File path to save the depth map
    private string lowbitMapCsvPath = "Assets/lowbitMap.csv"; // File path to save the depth map as CSV
    private string highbitMapFilePath = "Assets/highbitMap.txt"; // File path to save the depth map
    private string highbitMapCsvPath = "Assets/highbitMap.csv"; // File path to save the depth map as CSV
    private string depthMapFilePath = "Assets/depthMap.txt"; // File path to save the depth map
    private string depthMapCsvPath = "Assets/depthMap.csv"; // File path to save the depth map as CSV

    // Start is called before the first frame update
    void Start()
    {
        videoFrame = LoadTextureFromFile(filePath);
        pixels = videoFrame.GetPixels();
        Debug.Log("Pixels length: " + pixels.Length);
        lowbitMap = new float[depth_width, depth_height];
        highbitMap = new float[depth_width, depth_height];
        depthMap = new float[depth_width, depth_height];
        SaveDepthMap();
        SaveDepthMapToFile();
        SaveDepthMapToCsv();
        CreateHeatmap();
    }

    Texture2D LoadTextureFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(megaframe_width, megaframe_width);
        if (tex.LoadImage(fileData))
        {
            Debug.Log("Texture loaded successfully from " + filePath);
            Debug.Log($"Texture2D size: {tex.width}x{tex.height}");
            return tex;
        }
        else
        {
            Debug.LogError("Failed to load texture from " + filePath);
            return null;
        }
    }

    private Vector3 ComputePosition(int x, int y)
    {
        Vector3 newPos = new Vector3(0, 0, 0);
        newPos.x = (x - cx_depth) * depthMap[x, y] / fx_depth;
        newPos.y = (y - cy_depth) * depthMap[x, y] / fy_depth;
        newPos.z = depthMap[x, y];

        return newPos;
    }

    private void SaveDepthMap()
    {
        // Texture2D coordinate system: (0, 0) is at the bottom-left corner
        // y height
        // ^
        // |
        // |
        // +------> x width
        for (int y = 0; y < depth_height / 2; y++)
            for (int x = 0; x < depth_width; x++)
            {
                // upper half
                float lowbit = GetImagePixel(x, depth_height / 2 + y).b;
                float highbit = GetImagePixel(x, y).b;
                float depth_value = lowbit * 255 + highbit * 255 * 256;
                lowbitMap[x, y + depth_height / 2] = lowbit * 255;
                highbitMap[x, y + depth_height / 2] = highbit * 255;
                depthMap[x, y + depth_height / 2] = depth_value;

                // lower half
                lowbit = GetImagePixel(x + depth_width, depth_height / 2 + y).b;
                highbit = GetImagePixel(x + depth_width, y).b;
                depth_value = lowbit * 255 + highbit * 255 * 256;
                lowbitMap[x, y] = lowbit * 255;
                highbitMap[x, y] = highbit * 255;
                depthMap[x, y] = depth_value;
            }
    }

    private Color GetImagePixel(int x, int y)
    {
        int index = y * megaframe_width + x;
        return pixels[index];
    }

    private void DetectEdges()
    {
        // Sobel kernels
        int[,] sobelX = new int[,]
        {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };

        int[,] sobelY = new int[,]
        {
            { 1, 2, 1 },
            { 0, 0, 0 },
            { -1, -2, -1 }
        };

        for (int y = 1; y < depth_height / 2 - 1; y++)
        {
            for (int x = 1; x < depth_width - 1; x++)
            {
                float gx = 0;
                float gy = 0;

                // Apply the Sobel operator
                for (int j = -1; j <= 1; j++)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        float depth = depthMap[x + i, y + j];
                        gx += depth * sobelX[j + 1, i + 1];
                        gy += depth * sobelY[j + 1, i + 1];
                    }
                }

                // Calculate the gradient magnitude
                float edgeStrength = Mathf.Sqrt(gx * gx + gy * gy);

                // Define a threshold to determine edge pixels
                float threshold = 1000f; // Adjust this value based on your depth map range
                edgeMap[x, y] = edgeStrength > threshold;
            }
        }
    }

    private void SaveDepthMapToFile()
    {
        using (StreamWriter writer = new StreamWriter(lowbitMapFilePath))
        {
            for (int y = 0; y < depth_height; y++)
            {
                for (int x = 0; x < depth_width; x++)
                {
                    writer.Write(lowbitMap[x, y].ToString() + " ");
                }
                writer.WriteLine();
            }
        }
        Debug.Log("Depth map saved to " + lowbitMapFilePath);

        using (StreamWriter writer = new StreamWriter(highbitMapFilePath))
        {
            for (int y = 0; y < depth_height; y++)
            {
                for (int x = 0; x < depth_width; x++)
                {
                    writer.Write(lowbitMap[x, y].ToString() + " ");
                }
                writer.WriteLine();
            }
        }
        Debug.Log("Depth map saved to " + highbitMapFilePath);

        using (StreamWriter writer = new StreamWriter(depthMapFilePath))
        {
            for (int y = 0; y < depth_height; y++)
            {
                for (int x = 0; x < depth_width; x++)
                {
                    writer.Write(depthMap[x, y].ToString() + " ");
                }
                writer.WriteLine();
            }
        }
        Debug.Log("Depth map saved to " + depthMapFilePath);
    }

    private void SaveDepthMapToCsv()
    {
        using (StreamWriter writer = new StreamWriter(lowbitMapCsvPath))
        {
            for (int y = 0; y < depth_height; y++)
            {
                for (int x = 0; x < depth_width; x++)
                {
                    writer.Write(lowbitMap[x, y].ToString());
                    if (x < depth_width - 1)
                        writer.Write(","); // Add comma between values
                }
                writer.WriteLine(); // New line after each row
            }
        }
        Debug.Log("Depth map saved to CSV file: " + lowbitMapCsvPath);

        using (StreamWriter writer = new StreamWriter(highbitMapCsvPath))
        {
            for (int y = 0; y < depth_height; y++)
            {
                for (int x = 0; x < depth_width; x++)
                {
                    writer.Write(highbitMap[x, y].ToString());
                    if (x < depth_width - 1)
                        writer.Write(","); // Add comma between values
                }
                writer.WriteLine(); // New line after each row
            }
        }
        Debug.Log("Depth map saved to CSV file: " + highbitMapCsvPath);

        using (StreamWriter writer = new StreamWriter(depthMapCsvPath))
        {
            for (int y = 0; y < depth_height; y++)
            {
                for (int x = 0; x < depth_width; x++)
                {
                    writer.Write(depthMap[x, y].ToString());
                    if (x < depth_width - 1)
                        writer.Write(","); // Add comma between values
                }
                writer.WriteLine(); // New line after each row
            }
        }
        Debug.Log("Depth map saved to CSV file: " + depthMapCsvPath);
    }

    private void CreateHeatmap()
    {
        Texture2D heatmapTexture = new Texture2D(depth_width, depth_height);
        for (int y = 0; y < depth_height; y++)
        {
            for (int x = 0; x < depth_width; x++)
            {
                float depthValue = highbitMap[x, y];
                Color color = GetHeatmapColor(depthValue);
                heatmapTexture.SetPixel(x, y, color);
            }
        }
        heatmapTexture.Apply();

        GameObject heatmapPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        heatmapPlane.transform.localScale = new Vector3(6.4f, 1, 5.76f); // Adjust the scale as needed
        heatmapPlane.GetComponent<Renderer>().material = heatmapMaterial;
        heatmapMaterial.mainTexture = heatmapTexture;
    }

    private Color GetHeatmapColor(float value)
    {
        // This function maps a depth value to a color
        // Adjust the mapping logic as needed
        return Color.Lerp(Color.blue, Color.red, value / 16); // Assuming 16-bit depth values
    }



}
