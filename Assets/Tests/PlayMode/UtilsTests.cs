using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UtilsTests {
    float tolerance = 0.01f;
    GameObject manager = new GameObject();

    [OneTimeSetUp]
    public void Setup() {
        manager.AddComponent<GameManager>();

    }

    [UnityTest]
    public IEnumerator FreefallDisplacementTest() {

        Vector3 InitialVel = new Vector3(1, 2, 3);
        float timeStep = 0.1f;
        Vector3 expected = new Vector3(0.99f, 0.999f, 2.97f);
        Vector3 actual = Utils.FreefallDisplacement(InitialVel, timeStep);

        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");

        yield return null;
    }

    [UnityTest]
    public IEnumerator CartesianToSphericalCoordsWithZeros() {
        Vector3 expected = new Vector3(0, 0, GameManager.Instance.epsilonLength);
        Vector3 actual = Utils.RelativeCartesianToSphericalCoords(new Vector3(0, 0, 0));
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
        yield return null;
    }
}