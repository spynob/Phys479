using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UtilsTests {
    [UnityTest]
    public IEnumerator FreefallVelocityUpdateTest() {
        GameObject manager = new GameObject();
        manager.AddComponent<GameManager>();

        Vector3 InitialVel = new Vector3(1, 2, 3);
        float timeStep = 0.1f;
        Vector3 expected = new Vector3(0.99f, 0.999f, 2.97f);
        Vector3 actual = Utils.FreefallVelocityUpdate(InitialVel, timeStep);
        float tolerance = 0.01f;

        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");

        yield return null;
    }

    [UnityTest]
    public IEnumerator CartesianToSphericalCoordsWithZeros() {
        GameObject manager = new GameObject();
        manager.AddComponent<GameManager>();
        float tolerance = 0.001f;
        Assert.That(Vector3.Distance(new Vector3(0, 0, GameManager.Instance.epsilonLength), Utils.RelativeCartesianToSphericalCoords(new Vector3(0, 0, 0))), Is.LessThan(tolerance), "Vector mismatch");
        yield return null;
    }

    [UnityTest]
    public IEnumerator SymmetryTestCoords() {
        GameObject manager = new GameObject();
        manager.AddComponent<GameManager>();
        Vector3 SphericalCoords = new Vector3(3, 4, 5);
        Vector3 CartesianCoords = Utils.SphericalToCartesianCoords(SphericalCoords);
        float tolerance = 0.001f;
        Assert.That(Vector3.Distance(SphericalCoords, Utils.RelativeCartesianToSphericalCoords(CartesianCoords)), Is.LessThan(tolerance), "Vector mismatch");
        yield return null;
    }
}