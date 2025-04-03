using NUnit.Framework;
using UnityEngine;
using System;

public class UtilsTests {
    //Cartesian to Spherical VElOCITY SPRING
    private float tolerance = 0.0001f;
    [Test]
    public void CartesianToSphericalVelocitySpringCorrect() {
        Vector3 expected = new Vector3(0.64333113848f, 0.40024040134f, 0.776457135308f);
        Vector3 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector3(Mathf.PI / 4, Mathf.PI / 6, 10), 0.05f);
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    [Test]
    public void CartesianToSphericalVelocitySpringLength0() {
        Exception ex = Assert.Throws<ArgumentException>(() => Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector3(Mathf.PI / 4, Mathf.PI / 6, 0), 0.05f));
        Assert.That(ex.Message, Is.EqualTo("Length must be non-zero! This may be a cast from Vector2."));
    }
    [Test]
    public void CartesianToSphericalVelocitySpringTheta0() {
        Vector3 expected = new Vector3(0.4f, 0, 5.09807621135f);
        Vector3 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector3(0, Mathf.PI / 6, 10), 0.05f);
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    // Cartesian to spherical VELOCITY RIGID
    [Test]
    public void CartesianToSphericalVelocityRigidCorrect() {
        Vector2 expected = new Vector2(0.64333113848f, 0.40024040134f);
        Vector2 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector2(Mathf.PI / 4, Mathf.PI / 6), 10, 0.05f);
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    [Test]
    public void CartesianToSphericalVelocityRigidTheta0() {
        Vector2 expected = new Vector2(0.4f, 0);
        Vector2 actual = Utils.CartesianToSphericalVelocity(new Vector3(3, 4, 5), new Vector2(0, Mathf.PI / 6), 10, 0.05f);
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    // Spherical to cartesian VELOCITY SPRING
    [Test]
    public void SphericalToCartesianVelocitySpringGeneral() {
        Vector3 expected = new Vector3(7.29089962562f, 17.6776695297f, 36.8692660986f);
        Vector3 actual = Utils.SphericalToCartesianVelocity(new Vector3(3, 4, 5), new Vector3(Mathf.PI / 4, Mathf.PI / 6, 10));
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    [Test]
    public void SphericalToCartesianVelocitySpringTheta0() {
        Vector3 expected = new Vector3(25.9807621135f, -5, 15);
        Vector3 actual = Utils.SphericalToCartesianVelocity(new Vector3(3, 4, 5), new Vector3(0, Mathf.PI / 6, 10));
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    // Spherical to cartesian VELOCITY RIGID
    [Test]
    public void SphericalToCartesianVelocityRigidGeneral() {
        Vector3 expected = new Vector3(4.22903744714f, 21.2132034356f, 35.1014991456f);
        Vector3 actual = Utils.SphericalToCartesianVelocity(new Vector2(3, 4), new Vector2(Mathf.PI / 4, Mathf.PI / 6), 10);
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    [Test]
    public void SphericalToCartesianVelocityRigidTheta0() {
        Vector3 expected = new Vector3(25.9807621135f, 0, 15);
        Vector3 actual = Utils.SphericalToCartesianVelocity(new Vector2(3, 4), new Vector2(0, Mathf.PI / 6), 10);
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    // Spherical to Cartesian COORDS
    [Test]
    public void SphericalToCartesianCoordsCorrect() {
        Vector3 expected = new Vector3(6.12372435696f, -7.07106781187f, 3.53553390593f);
        Vector3 actual = Utils.SphericalToCartesianCoords(new Vector3(Mathf.PI / 4, Mathf.PI / 6, 10));
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }

    [Test]
    public void SphericalToCartesianCoordsWithZerolength() {
        Exception ex = Assert.Throws<ArgumentException>(() => Utils.SphericalToCartesianCoords(new Vector3(3, 4, 0)));
        Assert.That(ex.Message, Is.EqualTo("Length must be non-zero! This may be a cast from Vector2."));
    }

    // Cartesian to spherical COORDS 
    [Test]
    public void CartesianToSphericalCoordsCorrect() {
        Vector2 expected = new Vector2(6.12372435696f, -7.07106781187f);
        Vector2 actual = Utils.SphericalToCartesianCoords(new Vector3(Mathf.PI / 4, Mathf.PI / 6), 10);
        Debug.Log($"Expected: {expected}, Actual: {actual}, Distance: {Vector3.Distance(expected, actual)}");
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(tolerance), "Vector mismatch");
    }
}
