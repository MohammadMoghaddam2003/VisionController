using UnityEngine;

public struct MathHelper
{
    public static Vector2 Ang2Vec2(float degree) => 
        new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
    
    
    public static Vector3 Ang2Vec3(float degree) => 
        new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), 0,Mathf.Sin(degree * Mathf.Deg2Rad));
    

    public static float Pythagoras_UnknownSide(float hypotenuse, float knownSide) =>
        Mathf.Sqrt( (hypotenuse * hypotenuse) - (knownSide * knownSide));
    
    
    public static float Pythagoras_UnknownHypotenuse(float side1, float side2) =>
        Mathf.Sqrt((side1 * side1) + (side2 * side2));
    
    
    public static Matrix4x4 RotationMatrix(Vector3 pos, Vector3 axis, float degree) =>
        Matrix4x4.TRS(pos, Quaternion.AngleAxis(degree, axis), Vector3.one);
}