using NUnit.Framework;
using UnityEngine;

public class UtilsTests {
    // A Test behaves as an ordinary method
    [Test]
    public void SphericalToCartesianCoordsCorrect() {
        Assert.AreEqual(new Vector3(0, -1, 0), Utils.SphericalToCartesianCoords(new Vector3(0, 0, 1)));
    }
}
