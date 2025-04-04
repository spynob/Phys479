using UnityEngine;

public static class RungeKutta {

    public static float[] Step(float timeStep, float[] state, float length) { // state = [theta, thetaDot, phi, PhiDot, Length, LengthDot]
        float[] k1 = Derivatives(state, length);
        float[] k2 = Derivatives(AddVectors(state, MultiplyVector(k1, timeStep / 2)), length);
        float[] k3 = Derivatives(AddVectors(state, MultiplyVector(k2, timeStep / 2)), length);
        float[] k4 = Derivatives(AddVectors(state, MultiplyVector(k3, timeStep)), length);

        for (int i = 0; i < state.Length; i++) {
            state[i] += timeStep / 6 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
        }
        return state;
    }

    private static float[] Derivatives(float[] state, float length) {
        float theta = state[0];
        float omega = state[1];
        float alpha = state[3];


        /*
        Here damping is supposed to be divided by mass. However, I only have one player of constant mass using these equations. Therefore I divided damping by the mass before hand (See Player.cs)
        IF YOU ARE USING VARYING MASS, REMOVE THE CALL OF UpdateDamping(mass) IN Plyaer.cs AND DIVIDE EACH APPEARANCE OF damping IN THE FOLLOWING LINES BY THE APPROPRIATE TERM "damping/mass"
        */
        // theta
        float omegaDot = Mathf.Sin(theta) * (Mathf.Cos(theta) * alpha * alpha - GameManager.Instance.gravity / length);

        // phi
        float dividant = Mathf.Tan(theta);
        float alphaDot;
        if (Mathf.Abs(dividant) > GameManager.Instance.epsilon) { alphaDot = -2 * alpha * omega / dividant - GameManager.Instance.damping * alpha; }
        else { alphaDot = 0; alpha = 0; }

        return new float[] { omega, omegaDot, alpha, alphaDot };
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