using System;

public class Camera {
    private Vec3 origin;
    private Vec3 corner;
    private Vec3 horizontal;
    private Vec3 vertical;

    public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 vup, double vfov, double aspectRatio) {
        double theta = vfov * Math.PI / 180.0;
        double viewportHeight = 2.0 * Math.Tan(theta / 2.0);
        double viewportWidth = viewportHeight * aspectRatio;

        Vec3 w = lookFrom.sub(lookAt).normalized();
        Vec3 u = vup.cross(w).normalized();
        Vec3 v = w.cross(u);

        origin = lookFrom;
        horizontal = u.mult(viewportWidth);
        vertical = v.mult(viewportHeight);
        corner = origin.sub(horizontal.div(2.0)).sub(vertical.div(2.0)).sub(w);
    }

    public Ray getRay(double s, double t) {
        return new Ray(origin, corner.add(horizontal.mult(s)).add(vertical.mult(t)).sub(origin));
    }
}
