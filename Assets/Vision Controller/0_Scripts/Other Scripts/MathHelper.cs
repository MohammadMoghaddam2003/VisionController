using UnityEngine;

public struct MathHelper
{
    /// <summary>
    ///  Returns a Vector2 by the received degree
    /// </summary>
    public static Vector2 Ang2Vec2(float degree) => 
        new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
    
    
    
    /// <summary>
    ///  Returns a Vector3 by the received degree
    /// </summary>
    public static Vector3 Ang2Vec3(float degree) => 
        new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), 0,Mathf.Sin(degree * Mathf.Deg2Rad));
    

    
    /// <summary>
    /// It receives the known side and the hypotenuse and then returns the unknown side!
    /// </summary>
    public static float Pythagoras_GetSide(float hypotenuse, float knownSide) =>
        Mathf.Sqrt( (hypotenuse * hypotenuse) - (knownSide * knownSide));
    
    
    
    /// <summary>
    /// It receives two known sides and returns the hypotenuse!
    /// </summary>
    public static float Pythagoras_GetHypotenuse(float side1, float side2) =>
        Mathf.Sqrt((side1 * side1) + (side2 * side2));
    
    
    
    /// <summary>
    /// Creates a new matrix based on a position and a rotation!
    /// </summary>
    /// <param name="pos"> The position of matrix </param>
    /// <param name="axis"> The axis that the matrix should turn around </param>
    /// <param name="degree"> The amount of rotation of the matrix by degree </param>
    /// <param name="baseRotation"> The base/current rotation </param>
    public static Matrix4x4 ChangeMatrix(Vector3 pos, Vector3 axis, float degree, Quaternion baseRotation) =>
        Matrix4x4.TRS(pos, baseRotation * Quaternion.AngleAxis(degree, axis), Vector3.one);
    
    
    
    /// <summary>
    /// Calculate projection of degree
    /// </summary>
    public static float CalculateProjection(float degree) => Mathf.Cos(degree * Mathf.Deg2Rad);
}