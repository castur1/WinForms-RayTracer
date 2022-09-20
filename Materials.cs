using System;

public struct Scattered {
    public Scattered(Vec3 _attenuation, Ray _scatteredRay, bool _didScatter) {
        attenuation = _attenuation;
        scatteredRay = _scatteredRay;
        didScatter = _didScatter;
    }

    public Vec3 attenuation { get; set; }
    public Ray scatteredRay { get; set; }
    public bool didScatter { get; set; }
}

public abstract class Material {
    public abstract Scattered scatter(Ray r, HitRecord rec);
}

public class Lambertian : Material {
    public Vec3 colour;

    public Lambertian(Vec3 _colour) {
        colour = _colour;
    }

    // Implement near zero correction?
    public override Scattered scatter(Ray r, HitRecord rec) {
        Scattered scattered = new Scattered();

        Vec3 scatterDir = rec.normal.add(rec.normal.randomUnit());
        scattered.scatteredRay = new Ray(rec.p, scatterDir);
        scattered.attenuation = colour;
        scattered.didScatter = true;

        return scattered;
    }
}

public class Metal : Material {
    Vec3 colour;
    double fuzz;

    public Metal(Vec3 _colour, double _fuzz) {
        colour = _colour;
        fuzz = _fuzz < 1.0 ? (_fuzz > 0.0 ? _fuzz : 0.0) : 1.0;
    }

    public override Scattered scatter(Ray r, HitRecord rec) {
        Scattered scattered = new Scattered();

        Vec3 reflected = r.dir.normalized().reflected(rec.normal);
        scattered.scatteredRay = new Ray(rec.p, reflected.add(reflected.randomInUnitSphere().mult(fuzz)));
        scattered.attenuation = colour;
        scattered.didScatter = scattered.scatteredRay.dir.dot(rec.normal) > 0.0;

        return scattered;
    }
}

public class Dielectric : Material {
    double refractionIndex;

    public Dielectric(double _refractionIndex) {
        refractionIndex = _refractionIndex;
    }

    public override Scattered scatter(Ray r, HitRecord rec) {
        Scattered scattered = new Scattered();
        scattered.attenuation = new Vec3(1.0);

        Random random = new Random();

        double refractionRatio = rec.frontFace ? (1.0 / refractionIndex) : refractionIndex;

        Vec3 unitDir = r.dir.normalized();
        double cos = Math.Min(unitDir.negate().dot(rec.normal), 1.0);
        double sin = Math.Sqrt(1.0 - cos * cos);

        bool cannotRefract = refractionRatio * sin > 1.0;
        Vec3 dir;
        if (cannotRefract || reflectance(cos, refractionRatio) > random.NextDouble())
            dir = unitDir.reflected(rec.normal);
        else
            dir = unitDir.refracted(rec.normal, refractionRatio);

        scattered.scatteredRay = new Ray(rec.p, dir);
        scattered.didScatter = true;

        return scattered;
    }

    private double reflectance(double cos, double refIdx) {
        double r0 = (1.0 - refIdx) / (1 + refIdx);
        r0 *= r0;
        return r0 + (1.0 - r0) * Math.Pow((1.0 - cos), 5);
    }
}
