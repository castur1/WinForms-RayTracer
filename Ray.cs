public class Ray {
    public Vec3 origin;
    public Vec3 dir;

    public Ray() { }

    public Ray(Vec3 _origin, Vec3 _dir) {
        origin = _origin;
        dir = _dir;
    }

    public Vec3 at(double t) {
        return origin.add(dir.mult(t));
    }
}