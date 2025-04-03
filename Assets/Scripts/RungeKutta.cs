using UnityEngine;

public static class RungeKutta {

    public static float[] Step(float timeStep, float[] state, float naturalLength) { // state = [theta, thetaDot, phi, PhiDot, Length, LengthDot]
        float[] k1 = Derivatives(state, naturalLength);
        float[] k2 = Derivatives(AddVectors(state, MultiplyVector(k1, timeStep / 2)), naturalLength);
        float[] k3 = Derivatives(AddVectors(state, MultiplyVector(k2, timeStep / 2)), naturalLength);
        float[] k4 = Derivatives(AddVectors(state, MultiplyVector(k3, timeStep)), naturalLength);

        for (int i = 0; i < state.Length; i++) {
            state[i] += timeStep / 6 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
        }
        return state;
    }

    static float theta;
    static float omega;
    static float alpha;
    static float length;
    static float lengthDot;
    static float sint;
    static float cost;
    private static float[] Derivatives(float[] state, float naturalLength) {
        theta = state[0];
        omega = state[1];
        alpha = state[3];
        length = state[4];
        lengthDot = state[5];
        sint = Mathf.Sin(theta);
        cost = Mathf.Cos(theta);

        /*
        Here damping and k are supposed to be divided by mass. However, I only have one player of constant mass using these equations. Therefore I divided the variables by the mass before hand (See Player.cs)
        IF YOU ARE USING VARYING MASS, REMOVE THE CALL OF UpdateDampingAndK(mass) IN Plyaer.cs AND DIVIDE EACH APPEARANCE OF damping OR k IN THE FOLLOWING LINES BY THE APPROPRIATE MASS
        */
        // theta
        if (length < GameManager.Instance.epsilonLength) { length = GameManager.Instance.epsilonLength; }
        float omegaDot = sint * (cost * alpha * alpha - GameManager.Instance.gravity / length) - GameManager.Instance.damping * omega - 2 * omega * lengthDot / length;

        // phi
        float dividant = sint / cost;
        float phiDot;
        if (Mathf.Abs(dividant) > GameManager.Instance.epsilon) { phiDot = -2 * alpha * lengthDot / length - 2 * alpha * omega / dividant - GameManager.Instance.damping * alpha; }
        else { phiDot = 0; alpha = 0; }

        // length
        float lengthDotDot = length * (omega * omega + alpha * alpha * sint * sint) + GameManager.Instance.gravity * cost - GameManager.Instance.k * (length - naturalLength) - GameManager.Instance.damping * lengthDot;

        return new float[] { omega, omegaDot, alpha, phiDot, lengthDot, lengthDotDot };
    }

    private static float[] AddVectors(float[] a, float[] b) {
        float[] result = new float[a.Length];
        for (int i = 0; i < a.Length; i++) {
            result[i] = a[i] + b[i];
        }
        return result;
    }

    private static float[] MultiplyVector(float[] a, float scalar) {
        float[] result = new float[a.Length];
        for (int i = 0; i < a.Length; i++) {
            result[i] = a[i] * scalar;
        }
        return result;
    }
}