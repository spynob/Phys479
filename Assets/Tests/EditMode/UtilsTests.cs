using NUnit.Framework;
using UnityEngine;

public class UtilsTests {
    //Cartesian to Spherical VElOCITY SPRING
    [Test]
    public void CartesianToSphericalVelocitySpringCorrect() {
        Vector3 expected = new Vector3(3.393f, 2.001f, 0.776457f);
        Vector3 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector3(Mathf.PI / 4, Mathf.PI / 6, 2), 0.05f);
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    [Test]
    public void CartesianToSphericalVelocitySpringLength0() {
        Vector3 expected = new Vector3(0, 0, 0);
        Vector3 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector3(Mathf.PI / 4, Mathf.PI / 6, 0), 0.05f);
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }
    [Test]
    public void CartesianToSphericalVelocitySpringTheta0() {
        Vector3 expected = new Vector3(2.299f, 0, -4);
        Vector3 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector3(0, Mathf.PI / 6, 0), 0.05f);
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    // Cartesian to spherical VELOCITY RIGID
    [Test]
    public void CartesianToSphericalVelocityRigidCorrect() {
        Vector2 expected = new Vector3(3.393f, 2.001f, 2);
        Vector2 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector3(Mathf.PI / 4, Mathf.PI / 6, 2), 0.05f);
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    [Test]
    public void CartesianToSphericalVelocityRigidLength0() {
        Vector2 expected = new Vector3(0, 0, 0);
        Vector2 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector3(Mathf.PI / 4, Mathf.PI / 6, 0), 0.05f);
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }
    [Test]
    public void CartesianToSphericalVelocityRigidTheta0() {
        Vector2 expected = new Vector3(2.299f, 0, 2);
        Vector2 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector3(0, Mathf.PI / 6, 2), 0.05f);
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    // Spherical to cartesian VELOCITY SPRING
    [Test]
    public void SphericalToCartesianVelocitySpringGeneral() {
        Vector2 expected = new Vector3(3.2165f, 4.24995f, 5.1247f);
        Vector2 actual = Utils.SphericalToCartesianVelocity(new Vector3(3.3934f, 2.001f, 0.776457f), new Vector3(Mathf.PI / 4, Mathf.PI / 6, 2));
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    [Test]
    public void SphericalToCartesianVelocitySpringTheta0() {
        Vector2 expected = new Vector3(2.8284f, 3, 2.8284f);
        Vector2 actual = Utils.SphericalToCartesianVelocity(new Vector3(2, 9999, 3), new Vector3(0, Mathf.PI / 2, 2));
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    // Spherical to cartesian VELOCITY RIGID
    [Test]
    public void SphericalToCartesianVelocityRigidGeneral() { }

    [Test]
    public void SphericalToCartesianVelocityRigidTheta0() { }

    // Spherical to Cartesian COORDS
    [Test]
    public void SphericalToCartesianCoordsCorrect() {
        Vector3 expected = new Vector3(3, -4, 5);
        Vector3 actual = Utils.SphericalToCartesianCoords(new Vector3(0.9695f, 1.03f, 7.071f));
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    [Test]
    public void SphericalToCartesianCoordsWithZeros() {
        Assert.AreEqual(new Vector3(0, 0, 0), Utils.SphericalToCartesianCoords(new Vector3(00.9695f, 1.03f, 0)));
    }

    // Cartesian to spherical COORDS 
    [Test]
    public void CartesianToSphericalCoordsCorrect() {
        Vector3 expected = new Vector3(0.9695f, 1.03f, 7.071f);
        Vector3 actual = Utils.RelativeCartesianToSphericalCoords(new Vector3(3, -4, 5));
        float tolerance = 0.01f;
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }
}