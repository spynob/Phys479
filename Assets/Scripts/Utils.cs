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
        float theta = sphericalCoords.x; float phi = sphericalCoords.y; float length = sphericalCoords.z;

        float lengthDot = Mathf.Cos(theta) * (cartesianVelocity.x * Mathf.Cos(phi) + cartesianVelocity.z * Mathf.Sin(phi)) - cartesianVelocity.y * Mathf.Sin(theta);

        float omega;
        if (length <= epsilon) { omega = 0; }
        else { omega = (Mathf.Sin(theta) * (cartesianVelocity.x * Mathf.Cos(phi) + cartesianVelocity.z * Mathf.Sin(phi)) + cartesianVelocity.y * Mathf.Cos(theta)) / length; }

        float dividant = length * Mathf.Sin(theta);
        float alpha;
        if (dividant < epsilon) { alpha = 0; }
        else { alpha = (cartesianVelocity.z * Mathf.Cos(phi) - cartesianVelocity.x * Mathf.Sin(phi)) / (length * Mathf.Sin(theta)); }

        return new Vector3(omega, alpha, lengthDot);
    }

    public static Vector2 CartesianToSphericalVelocity(Vector3 cartesianVelocity, Vector2 sphericalCoords, float length, float epsilon) {
        float theta = sphericalCoords.x; float phi = sphericalCoords.y;
        float omega = (Mathf.Sin(theta) * (cartesianVelocity.x * Mathf.Cos(phi) + cartesianVelocity.z * Mathf.Sin(phi)) + cartesianVelocity.y * Mathf.Cos(theta)) / length;
        float dividant = length * Mathf.Sin(theta);
        float alpha;
        if (dividant < epsilon) { alpha = 0; }
        else { alpha = (cartesianVelocity.z * Mathf.Cos(phi) - cartesianVelocity.x * Mathf.Sin(phi)) / (length * Mathf.Sin(theta)); }
        return new Vector2(omega, alpha);
    }

    public static Vector3 SphericalToCartesianVelocity(Vector3 sphericalVelocity, Vector3 sphericalCoords) {
        if (sphericalCoords.z == 0) {
            throw new System.ArgumentException("Length must be non-zero! This may be a cast from Vector2.");
        }
        float theta = sphericalCoords.x; float phi = sphericalCoords.y; float length = sphericalCoords.z;
        float omega = sphericalVelocity.x; float alpha = sphericalVelocity.y; float lengthDot = sphericalVelocity.z;

        float xDot = length * (omega * Mathf.Cos(theta) * Mathf.Cos(phi) - alpha * Mathf.Sin(theta) * Mathf.Sin(phi)) + lengthDot * Mathf.Sin(theta) * Mathf.Cos(phi);
        float yDot = length * omega * Mathf.Sin(theta) - lengthDot * Mathf.Cos(theta);
        float zDot = length * (omega * Mathf.Cos(theta) * Mathf.Sin(phi) + alpha * Mathf.Sin(theta) * Mathf.Cos(phi)) + lengthDot * Mathf.Sin(theta) * Mathf.Sin(phi);
        return new Vector3(xDot, yDot, zDot);
    }

    public static Vector3 SphericalToCartesianVelocity(Vector2 sphericalVelocity, Vector2 sphericalCoords, float length) {
        float theta = sphericalCoords.x; float phi = sphericalCoords.y;
        float omega = sphericalVelocity.x; float alpha = sphericalVelocity.y;
        float xDot = length * (omega * Mathf.Cos(theta) * Mathf.Cos(phi) - alpha * Mathf.Sin(theta) * Mathf.Sin(phi));
        float yDot = length * omega * Mathf.Sin(theta);
        float zDot = length * (omega * Mathf.Cos(theta) * Mathf.Sin(phi) + alpha * Mathf.Sin(theta) * Mathf.Cos(phi));
        return new Vector3(xDot, yDot, zDot);
    }

    public static Vector3 SphericalToCartesianCoords(Vector3 sphericalCoords) {
        if (sphericalCoords.z == 0) {
            throw new System.ArgumentException("Length must be non-zero! This may be a cast from Vector2.");
        }
        float theta = sphericalCoords.x; float phi = sphericalCoords.y; float length = sphericalCoords.z;

        float x = length * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = -length * Mathf.Cos(theta);
        float z = length * Mathf.Sin(theta) * Mathf.Sin(phi);
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
        return sphericalVelocity.x * sphericalVelocity.x + sphericalVelocity.y * sphericalVelocity.y + GameManager.Instance.gravity * Mathf.Cos(sphericalCoords.x) > -GameManager.Instance.epsilon;
    }
}