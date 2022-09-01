using System;
using System.Collections.Generic;

public class HitRecord {
    public HitRecord(Vec3 _p, Vec3 _normal, Material _material, double _t, bool _frontFace, bool _didHit) {
        p = _p;
        normal = _normal;
        material = _material;
        t = _t;
        frontFace = _frontFace;
        didHit = _didHit;
    }

    public HitRecord() {
        didHit = false;
    }

    public void SetFaceNormal(Ray r, Vec3 outNormal) {
        frontFace = r.dir.dot(outNormal) < 0.0;
        normal = frontFace ? outNormal : outNormal.negate();
    }

    public Vec3 p { get; set; }
    public Vec3 normal { get; set; }
    public Material material { get; set; }
    public double t { get; set; }
    public bool frontFace { get; set; }
    public bool didHit { get; set; }
}

public abstract class Hittable {
    public abstract HitRecord hit(Ray r, double t_min, double t_max);
}

public class Sphere : Hittable {
    public Vec3 centre = new Vec3();
    public double radius;
    public Material material;

    public Sphere() { }

    public Sphere(Vec3 _centre, double _radius, Material _material) {
        centre = _centre;
        radius = _radius;
        material = _material;
    }

    public override HitRecord hit(Ray r, double t_min, double t_max) {
        Vec3 oc = r.origin.sub(centre);

        double a = r.dir.lengthSq();
        double b = oc.dot(r.dir);
        double c = oc.lengthSq() - radius * radius;

        double discriminant = b * b - a * c;
        if (discriminant < 0.0)
            return new HitRecord();

        double sqrtd = Math.Sqrt(discriminant);

        double root = (-b - sqrtd) / a;
        if (root < t_min || root > t_max) {
            root = (-b + sqrtd) / a;
            if (root < t_min || root > t_max)
                return new HitRecord();
        }

        HitRecord rec = new HitRecord();
        rec.t = root;
        rec.p = r.at(rec.t);
        Vec3 outNormal = rec.p.sub(centre).div(radius);
        rec.SetFaceNormal(r, outNormal);
        rec.material = material;
        rec.didHit = true;

        return rec;
    }
}

public class HittableList : Hittable {
    public List<Hittable> objects = new List<Hittable>();

    public HittableList() { }
    public HittableList(Hittable _object) {
        add(_object);
    }

    public void clear() {
        objects.Clear();
    }

    public void add(Hittable o) {
        objects.Add(o);
    }

    public override HitRecord hit(Ray r, double t_min, double t_max) {
        HitRecord rec = new HitRecord();
        rec.didHit = false;
        double closest = t_max;

        foreach (Hittable o in objects) {
            HitRecord hit = o.hit(r, t_min, closest);
            if (hit.didHit) {
                closest = hit.t;
                rec = hit;
            }
        }

        return rec;
    }
}