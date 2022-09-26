using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RayTracer1 {
    public partial class Form1 : Form {
        static Random random = new Random();

        // Iterative version
        //Vec3 RayColour(Ray r, Hittable world, int depth) {
        //    Vec3 colour = new Vec3(1.0);
        //    HitRecord rec = world.hit(r, 0.001, double.MaxValue);
        //    while (rec.didHit) {
        //        Scattered scattered = rec.material.scatter(r, rec);
        //        if (depth <= 0 || !scattered.didScatter)
        //            return new Vec3(0.0);

        //        colour = colour.mult(scattered.attenuation);
        //        r = scattered.scatteredRay;
        //        rec = world.hit(r, 0.001, double.MaxValue);
        //        --depth;
        //    }

        //    double t = 0.5 * (r.dir.normalized().y + 1.0);
        //    Vec3 colour1 = new Vec3(1.0, 1.0, 1.0);
        //    Vec3 colour2 = new Vec3(0.5, 0.7, 1.0);
        //    return colour.mult(colour1.mult(1.0 - t).add(colour2.mult(t)));
        //}

        Vec3 RayColour(Ray r, Hittable world, int depth) {
            if (depth <= 0)
                return new Vec3(0.0);

            HitRecord rec = world.hit(r, 0.001, double.MaxValue);
            if (rec.didHit) {
                Scattered scattered = rec.material.scatter(r, rec);
                if (scattered.didScatter)
                    return scattered.attenuation.mult(RayColour(scattered.scatteredRay, world, depth - 1));

                return new Vec3(0.0);
            }

            double t = 0.5 * (r.dir.normalized().y + 1.0);
            Vec3 colour1 = new Vec3(1.0, 1.0, 1.0);
            Vec3 colour2 = new Vec3(0.5, 0.7, 1.0);
            return colour1.mult(1.0 - t).add(colour2.mult(t));
        }

        HittableList randomScene() {
            HittableList world = new HittableList();

            Material materialGround = new Lambertian(new Vec3(0.5, 0.5, 0.5));
            world.add(new Sphere(new Vec3(0.0, -1000.0, 0.0), 1000, materialGround));

            // Change back size
            for (int i = -4; i < 4; ++i)
                for (int j = -4; j < 4; ++j) {
                    double chooseMaterial = random.NextDouble();
                    Vec3 centre = new Vec3(i + 0.9 * random.NextDouble(), 0.2, j + 0.9 * random.NextDouble());

                    if (centre.sub(new Vec3(4.0, 0.2, 0)).length() > 0.9) {
                        Material materialSphere;
                        if (chooseMaterial < 0.8) {
                            Vec3 colour = centre.rand().mult(centre.rand());
                            materialSphere = new Lambertian(colour);
                            world.add(new Sphere(centre, 0.2, materialSphere));
                        } else if (chooseMaterial < 0.95) {
                            Vec3 colour = centre.rand(0.5, 1.0);
                            double fuzz = random.NextDouble() / 2.0;
                            materialSphere = new Metal(colour, fuzz);
                            world.add(new Sphere(centre, 0.2, materialSphere));
                        } else {
                            materialSphere = new Dielectric(1.5);
                            world.add(new Sphere(centre, 0.2, materialSphere));
                        }
                    }
                }

            Material materialGlass = new Dielectric(1.5);
            world.add(new Sphere(new Vec3(0.0, 1.0, 0.0), 1.0, materialGlass));

            Material materialLambertian = new Lambertian(new Vec3(0.8, 0.1, 0.2));
            world.add(new Sphere(new Vec3(-4.0, 1.0, 0.0), 1.0, materialLambertian));

            Material materialMetal = new Metal(new Vec3(0.7, 0.6, 0.5), 0.0);
            world.add(new Sphere(new Vec3(4.0, 1.0, 0.0), 1.0, materialMetal));

            return world;
        }

        unsafe Bitmap Render() {
            // Image

            double aspectRatio = (double)Width / Height;
            int imgWidth = 800;
            int imgHeight = Convert.ToInt32(imgWidth / aspectRatio);

            int samplesPerPixel = 50;
            int maxDepth = 8;

            // World

            HittableList world = randomScene();

            // Camera

            Vec3 lookFrom = new Vec3(13.0, 2.0, 3.0);
            Vec3 lookAt = new Vec3(0.0, 0.0, 0.0);
            Vec3 vup = new Vec3(0.0, 1.0, 0.0);
            double focusDist = 10.0;

            Camera cam = new Camera(
                lookFrom,
                lookAt,
                vup,
                20.0,
                aspectRatio,
                0.1,
                focusDist);

            // Render

            Bitmap img = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var imgData = img.LockBits(new Rectangle(0, 0, imgWidth, imgHeight), System.Drawing.Imaging.ImageLockMode.ReadWrite, img.PixelFormat);
            byte bitsPerPixel = 24;
            byte* scan0 = (byte*)imgData.Scan0.ToPointer();

            for (int y = 0; y < imgHeight; ++y)
                for (int x = 0; x < imgWidth; ++x) {
                    byte* data = scan0 + y * imgData.Stride + x * bitsPerPixel / 8;

                    Vec3 colour = new Vec3(0.0);

                    // Anti-aliasing through multi-sampling
                    for (int s = 0; s < samplesPerPixel; ++s) {
                        double flippedY = imgHeight - y - 1;
                        double u = (double)(x + random.NextDouble()) / (imgWidth - 1);
                        double v = (flippedY + random.NextDouble()) / (imgHeight - 1);

                        Ray r = cam.getRay(u, v);

                        colour = colour.add(RayColour(r, world, maxDepth));
                    }

                    colour = colour.div(samplesPerPixel);
                    // Gamma-2 correction
                    colour = new Vec3(
                        255.0 * Math.Sqrt(colour.x),
                        255.0 * Math.Sqrt(colour.y),
                        255.0 * Math.Sqrt(colour.z));

                    data[2] = (byte)colour.x;
                    data[1] = (byte)colour.y;
                    data[0] = (byte)colour.z;
                }

            img.UnlockBits(imgData);

            return img;
        }

        public Form1() {
            InitializeComponent();

            double aspectRatio = 3.0 / 2.0;
            Width = 1200;
            Height = (int)(Width / aspectRatio);
        }

        private unsafe void Form1_Paint(object sender, PaintEventArgs e) {
            Bitmap img = Render();

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(img, new Rectangle(0, 0, Width, Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
        }
    }
}

// Useful links, delete later
// https://stackoverflow.com/questions/29157/how-do-i-make-a-picturebox-use-nearest-neighbor-resampling
// https://raytracing.github.io/books/RayTracingInOneWeekend.html
// https://github.com/ja72/WinFormRayTrace/tree/d89648c195fb176a7690050d7a79b0786b04558b
