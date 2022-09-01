public class Camera {
    private Vec3 origin;
    private Vec3 corner;
    private Vec3 horizontal;
    private Vec3 vertical;

    public Camera() {
        double aspectRatio = 16.0 / 9.0;
        double viewportHeight = 2.0;
        double viewportWidth = viewportHeight * aspectRatio;
        double focalLength = 1.0;

        origin = new Vec3(0.0, 0.0, 0.0);
        horizontal = new Vec3(viewportWidth, 0.0, 0.0);
        vertical = new Vec3(0.0, viewportHeight, 0.0);
        corner = origin.sub(horizontal.div(2.0)).sub(vertical.div(2.0)).sub(new Vec3(0.0, 0.0, focalLength));
    }

    public Ray getRay(double u, double v) {
        return new Ray(origin, corner.add(horizontal.mult(u)).add(vertical.mult(v)).sub(origin));
    }
}