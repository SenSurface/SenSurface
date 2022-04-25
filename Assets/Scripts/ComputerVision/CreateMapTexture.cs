using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreateMapTexture {

    static int sizeX;
    static int sizeY;

    public static Texture2D CreateTexture(int x, int y)
    {
        sizeX = x;
        sizeY = y;
        return new Texture2D(x, y, TextureFormat.RGB24, false); ;
    }

    public static Texture2D GetTexture(Texture2D mainTexture)
    {
        mainTexture.Apply();
        return mainTexture;
    }

    public static Texture2D FlipTexture(Texture2D mainTexture, bool upSideDown = true)
    {
        Texture2D flipped = new Texture2D(mainTexture.width, mainTexture.height);

        int xN = mainTexture.width;
        int yN = mainTexture.height;


        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                if (upSideDown)
                {
                    flipped.SetPixel(j, xN - i - 1, mainTexture.GetPixel(j, i));
                }
                else
                {
                    flipped.SetPixel(xN - i - 1, j, mainTexture.GetPixel(i, j));
                }
            }
        }
        flipped.Apply();
        mainTexture = flipped;
        return flipped;
    }

    // probleme : ca le fait de bas en haut
    public static Texture2D UpdatePixels(Texture2D mainTexture, Color[] col)
    {
        if (mainTexture == null) return null;
        mainTexture.SetPixels(col);
        mainTexture.Apply();
        return mainTexture;
    }

    public static Texture2D UpdatePixel(Texture2D mainTexture, int x,int y, Color col)
    {
        mainTexture.SetPixel(x, y, col);
        return mainTexture;
    }

    // Update is called once per frame
    public static Texture2D UpdatePixels(Texture2D mainTexture, float[] points) {
        Color[] historyCols = new Color[points.Length];
        for (int e = 0; e < points.Length; e++)
            historyCols[e] = new Color(points[e], points[e], points[e]);

        return UpdatePixels(mainTexture, historyCols);
    }
}
