using System;

public class Vec3 {
    public double x;
    public double y;
    public double z;

    static Random random = new Random();

    public Vec3() {}

    public Vec3(double _x, double _y, double _z) {
        x = _x;
        y = _y;
        z = _z;
    }

    public Vec3(double value) {
        x = value;
        y = value;
        z = value;
    }

    public Vec3 add(Vec3 other) {
        return new Vec3(x + other.x, y + other.y, z + other.z);
    }

    public Vec3 add(double value) {
        return new Vec3(x + value, y + value, z + value);
    }

    public Vec3 sub(Vec3 other) {
        return new Vec3(x - other.x, y - other.y, z - other.z);
    }

    public Vec3 sub(double value) {
        return new Vec3(x - value, y - value, z - value);
    }

    public Vec3 mult(Vec3 other) {
        return new Vec3(x * other.x, y * other.y, z * other.z);
    }

    public Vec3 mult(double value) {
        return new Vec3(x * value, y * value, z * value);
    }

    public Vec3 div(Vec3 other) {
        return new Vec3(x / other.x, y / other.y, z / other.z);
    }

    public Vec3 div(double value) {
        return new Vec3(x / value, y / value, z / value);
    }

    public double lengthSq() {
        return x * x + y * y + z * z;
    }

    public double length() {
        return Math.Sqrt(lengthSq());
    }

    public double dot(Vec3 other) {
        return x * other.x + y * other.y + z * other.z;
    }

    public Vec3 cross(Vec3 other) {
        return new Vec3 {
            x = y * other.z - z * other.y,
            y = z * other.x - x * other.z,
            z = x * other.y - y * other.x
        };
    }

    public Vec3 normalized() {
        return this.div(length());
    }

    public Vec3 negate() {
        return new Vec3(-x, -y, -z);
    }

    public Vec3 rand() {
        return new Vec3(random.NextDouble(), random.NextDouble(), random.NextDouble());
    }

    public Vec3 rand(double min, double max) {
        double range = max - min;
        return new Vec3(
            range * random.NextDouble() + min,
            range * random.NextDouble() + min,
            range * random.NextDouble() + min);
    }

    public Vec3 randomInUnitSphere() {
        while(true) {
            Vec3 p = rand(-1.0, 1.0);
            if (p.lengthSq() < 1.0)
                return p;
        }
    }

    public Vec3 randomUnit() {
        return randomInUnitSphere().normalized();
    }

    public Vec3 randomInUnitCircle() {
        while(true) {
            Vec3 p = rand(-1.0, 1.0);
            p.z = 0.0;
            if (p.lengthSq() < 1.0)
                return p;
        }
    }

    public Vec3 reflected(Vec3 normal) {
        return this.sub(normal.mult(2.0 * this.dot(normal)));
    }

    public Vec3 refracted(Vec3 n, double refractiveIndexFraction) {
        double cos = Math.Min(this.negate().dot(n), 1.0);
        Vec3 perpendicular = this.add(n.mult(cos)).mult(refractiveIndexFraction);
        Vec3 parallel = n.mult(-Math.Sqrt(1.0 - perpendicular.lengthSq()));

        return perpendicular.add(parallel);
    }
}
