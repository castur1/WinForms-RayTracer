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

    public Metal(Vec3 _colour) {
        colour = _colour;
    }

    public override Scattered scatter(Ray r, HitRecord rec) {
        Scattered scattered = new Scattered();

        Vec3 reflected = r.dir.normalized().reflected(rec.normal);
        scattered.scatteredRay = new Ray(rec.p, reflected);
        scattered.attenuation = colour;
        scattered.didScatter = scattered.scatteredRay.dir.dot(rec.normal) > 0.0;

        return scattered;
    }
}