
using UnityEngine;

public static class Extensions
{
    public static Vector2 RotateVectorSnapped(this Vector2 vec,int angle)
    {
        switch (angle)
        {
            case 90:
                return vec.RotateToRight();
            case -90: case 270:
                return vec.RotateToLeft();
            case 180: case -180:
                return vec.Rotate180();
            default:
                return vec;
        }
    }

    private static Vector2 RotateToLeft(this Vector2 vec) {

        return new Vector2(vec.y * -1.0f, vec.x).normalized;

    }

    private static Vector2 RotateToRight(this Vector2 vec) {

        return new Vector2(vec.y, vec.x * -1.0f).normalized;

    }

    private static Vector2 Rotate180(this Vector2 vec) {

        vec = new Vector2(vec.y, vec.x * -1);
        vec = new Vector2(vec.y, vec.x * -1);

        return vec;

    }
}
