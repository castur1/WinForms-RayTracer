using System;

public class Camera {
    private Vec3 origin;
    private Vec3 corner;
    private Vec3 horizontal;
    private Vec3 vertical;
    private Vec3 w;
    private Vec3 u;
    private Vec3 v;
    double lensRadius;

    public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 vup, double vfov, double aspectRatio, double aperture, double focusDist) {
        double theta = vfov * Math.PI / 180.0;
        double viewportHeight = 2.0 * Math.Tan(theta / 2.0);
        double viewportWidth = viewportHeight * aspectRatio;

        w = lookFrom.sub(lookAt).normalized();
        u = vup.cross(w).normalized();
        v = w.cross(u);

        origin = lookFrom;
        horizontal = u.mult(viewportWidth).mult(focusDist);
        vertical = v.mult(viewportHeight).mult(focusDist);
        corner = origin.sub(horizontal.div(2.0)).sub(vertical.div(2.0)).sub(w.mult(focusDist));

        lensRadius = aperture / 2.0;
    }

    public Ray getRay(double s, double t) {
        Vec3 rd = new Vec3().randomInUnitCircle().mult(lensRadius);
        Vec3 offset = u.mult(rd.x).add(v.mult(rd.y));

        return new Ray(origin.add(offset), corner.add(horizontal.mult(s)).add(vertical.mult(t)).sub(origin).sub(offset));
    }
}
