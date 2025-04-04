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

    [UnityTest]
    public IEnumerator SymmetrySpringTestStoC_Coords() {
        Vector3 sphericalCoords = new Vector3(0.9876789f, 1.34543456f, 10);
        Vector3 cartesianCoords = Utils.SphericalToCartesianCoords(sphericalCoords);
        Assert.That(Vector3.Distance(sphericalCoords, cartesianCoords), Is.LessThan(tolerance), "Vector mismatch");
        yield return null;
    }

    [UnityTest]
    public IEnumerator SymmetrySpringTestCtoS_Coords() {
        Vector3 cartesianCoords = new Vector3(4.567865f, 2.987654f, 7.654f);
        Vector3 sphericalCoords = Utils.RelativeCartesianToSphericalCoords(cartesianCoords);
        Assert.That(Vector3.Distance(sphericalCoords, cartesianCoords), Is.LessThan(tolerance), "Vector mismatch");
        yield return null;
    }

    [UnityTest]
    public IEnumerator SymmetrySpringTestStoC_Vel() {
        Vector3 sphericalVel = new Vector3(0.9876789f, 1.34543456f, 2);
        Vector3 cartesianVel = Utils.SphericalToCartesianVelocity(sphericalVel, new Vector3(0.764245f, 1.764535f, 5.24324f));
        Assert.That(Vector3.Distance(sphericalVel, cartesianVel), Is.LessThan(tolerance), "Vector mismatch");
        yield return null;
    }

    [UnityTest]
    public IEnumerator SymmetrySpringTestCtoS_Vel() {
        Vector3 cartesianVel = new Vector3(4.567865f, 2.987654f, 7.654f);
        Vector3 sphericalVel = Utils.CartesianToSphericalVelocity(cartesianVel, new Vector3(0.764245f, 1.764535f, 5.24324f), 0.005f);
        Assert.That(Vector3.Distance(cartesianVel, sphericalVel), Is.LessThan(tolerance), "Vector mismatch");
        yield return null;
    }
}