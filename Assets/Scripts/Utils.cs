using UnityEngine;
using UnityEngine.UIElements;

public static class Utils {

    public static Vector3 FreefallDisplacement(Vector3 cartesianVel, float timeStep) {
        cartesianVel.y -= (GameManager.Instance.gravity + GameManager.Instance.damping * cartesianVel.y) * timeStep;
        return new Vector3(cartesianVel.x * (1 - GameManager.Instance.damping * timeStep), cartesianVel.y, cartesianVel.z * (1 - GameManager.Instance.damping * timeStep));
    }

    // Not sure about this one
    public static Vector3 CartesianToSphericalVelocity(Vector3 cartesianVelocity, Vector3 sphericalCoords, float epsilon) {
        if (sphericalCoords.z == 0) {
            throw new System.ArgumentException("Length must be non-zero! This may be a cast from Vector2.");
        }
        float length = sphericalCoords.z;
        float cost = Mathf.Cos(sphericalCoords.x); float sint = Mathf.Sin(sphericalCoords.x); float cosp = Mathf.Cos(sphericalCoords.y); float sinp = Mathf.Sin(sphericalCoords.y);

        float lengthDot = cost * (cartesianVelocity.x * cosp + cartesianVelocity.z * sinp) - cartesianVelocity.y * sint;

        float omega;
        if (length <= epsilon) { omega = 0; }
        else { omega = (sint * (cartesianVelocity.x * cosp + cartesianVelocity.z * sinp) + cartesianVelocity.y * cost) / length; }

        float dividant = length * sint;
        float alpha;
        if (dividant < epsilon) { alpha = 0; }
        else { alpha = (cartesianVelocity.z * cosp - cartesianVelocity.x * sinp) / (length * sint); }

        return new Vector3(omega, alpha, lengthDot);
    }

    public static Vector2 CartesianToSphericalVelocity(Vector3 cartesianVelocity, Vector2 sphericalCoords, float length, float epsilon) {
        float cost = Mathf.Cos(sphericalCoords.x); float sint = Mathf.Sin(sphericalCoords.x); float cosp = Mathf.Cos(sphericalCoords.y); float sinp = Mathf.Sin(sphericalCoords.y);
        float omega = (sint * (cartesianVelocity.x * cosp + cartesianVelocity.z * sinp) + cartesianVelocity.y * cost) / length;
        float dividant = length * sint;
        float alpha;
        if (dividant < epsilon) { alpha = 0; }
        else { alpha = (cartesianVelocity.z * cosp - cartesianVelocity.x * sinp) / (length * sint); }
        return new Vector2(omega, alpha);
    }

    public static Vector3 SphericalToCartesianVelocity(Vector3 sphericalVelocity, Vector3 sphericalCoords) {
        if (sphericalCoords.z == 0) {
            throw new System.ArgumentException("Length must be non-zero! This may be a cast from Vector2.");
        }
        float length = sphericalCoords.z;
        float cost = Mathf.Cos(sphericalCoords.x); float sint = Mathf.Sin(sphericalCoords.x); float cosp = Mathf.Cos(sphericalCoords.y); float sinp = Mathf.Sin(sphericalCoords.y);
        float omega = sphericalVelocity.x; float alpha = sphericalVelocity.y; float lengthDot = sphericalVelocity.z;

        float xDot = length * (omega * cost * cosp - alpha * sint * sinp) + lengthDot * sint * cosp;
        float yDot = length * omega * sint - lengthDot * cost;
        float zDot = length * (omega * cost * sinp + alpha * sint * cosp) + lengthDot * sint * sinp;
        return new Vector3(xDot, yDot, zDot);
    }

    public static Vector3 SphericalToCartesianVelocity(Vector2 sphericalVelocity, Vector2 sphericalCoords, float length) {
        float cost = Mathf.Cos(sphericalCoords.x); float sint = Mathf.Sin(sphericalCoords.x); float cosp = Mathf.Cos(sphericalCoords.y); float sinp = Mathf.Sin(sphericalCoords.y);
        float omega = sphericalVelocity.x; float alpha = sphericalVelocity.y;
        float xDot = length * (omega * cost * cosp - alpha * sint * sinp);
        float yDot = length * omega * sint;
        float zDot = length * (omega * cost * sinp + alpha * sint * cosp);
        return new Vector3(xDot, yDot, zDot);
    }

    public static Vector3 SphericalToCartesianCoords(Vector3 sphericalCoords) {
        if (sphericalCoords.z == 0) {
            throw new System.ArgumentException("Length must be non-zero! This may be a cast from Vector2.");
        }
        float length = sphericalCoords.z;
        float cost = Mathf.Cos(sphericalCoords.x); float sint = Mathf.Sin(sphericalCoords.x); float cosp = Mathf.Cos(sphericalCoords.y); float sinp = Mathf.Sin(sphericalCoords.y);

        float x = length * sint * cosp;
        float y = -length * cost;
        float z = length * sint * sinp;
        return new Vector3(x, y, z);
    }

    public static Vector3 SphericalToCartesianCoords(Vector2 sphericalCoords, float length) {
        return SphericalToCartesianCoords(new Vector3(sphericalCoords.x, sphericalCoords.y, length));
    }

    public static Vector3 RelativeCartesianToSphericalCoords(Vector3 relativeCoords) {
        if (relativeCoords.z < GameManager.Instance.epsilonLength) { new Vector3(0, 0, GameManager.Instance.epsilonLength); }
        return new Vector3(Mathf.Acos(-relativeCoords.y / relativeCoords.magnitude), Mathf.Atan2(relativeCoords.z, relativeCoords.x), relativeCoords.magnitude);
    }
    public static Vector2 RelativeCartesianToSphericalCoords(Vector3 relativeCoords, float length) {
        if (length < GameManager.Instance.epsilonLength) { new Vector3(0, 0, GameManager.Instance.epsilonLength); }
        return new Vector2(Mathf.Acos(-relativeCoords.y / length), Mathf.Atan2(relativeCoords.z, relativeCoords.x));
    }

    public static bool IsRadialMovementOutwardRigid(Vector2 sphericalCoords, Vector2 sphericalVelocity) {
        float verticalAcc = (sphericalVelocity.x * sphericalVelocity.x + sphericalVelocity.y * sphericalVelocity.y + GameManager.Instance.gravity * Mathf.Cos(sphericalCoords.x)) * Mathf.Cos(sphericalCoords.x);
        Debug.Log(verticalAcc);
        return sphericalVelocity.x * sphericalVelocity.x + sphericalVelocity.y * sphericalVelocity.y * Mathf.Sin(sphericalCoords.x) * Mathf.Sin(sphericalCoords.x) + GameManager.Instance.gravity * Mathf.Cos(sphericalCoords.x) > -GameManager.Instance.epsilonLength;
    }
}