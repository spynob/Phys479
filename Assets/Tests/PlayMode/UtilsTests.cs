using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UtilsTests {
    [UnityTest]
    public IEnumerator PlayerTestsWithEnumeratorPasses() {

        GameObject manager = new GameObject();
        manager.AddComponent<GameManager>();
        Vector3 InitialVel = new Vector3(1, 2, 3);
        float timeStep = 0.1f;
        Vector3 expected = new Vector3(0.99f, 0.999f, 2.97f);
        Vector3 actual = Utils.FreefallDisplacement(InitialVel, timeStep);
        float tolerance = 0.01f;

        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");

        yield return null;
    }

    [UnityTest]
    public void CartesianToSphericalCoordsWithZeros() {
        Assert.AreEqual(new Vector3(0, 0, GameManager.Instance.epsilonLength), Utils.RelativeCartesianToSphericalCoords(new Vector3(0, 0, 0)));
    }
}