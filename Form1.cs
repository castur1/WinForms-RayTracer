using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// Next up: Fuzzyness! Chapter 9.6

namespace RayTracer1 {
    public partial class Form1 : Form {
        static Random random = new Random();

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

        public Form1() {
            InitializeComponent();
        }

        private unsafe void canvas_Paint(object sender, PaintEventArgs e) {
            // Image

            double aspectRatio = 16.0 / 9.0;

            Width = 1000;
            Height = Convert.ToInt32(Width / aspectRatio);

            int imgWidth = 400;
            int imgHeight = Convert.ToInt32(imgWidth / aspectRatio);
            int samplesPerPixel = 100;
            int maxDepth = 10;

            // World

            HittableList world = new HittableList();
            world.add(new Sphere(new Vec3( 0.0, -1000.5, -1.0), 1000, new Lambertian(new Vec3(0.24, 0.33, 0.64))));
            world.add(new Sphere(new Vec3( 0.0,  0.0,    -1.0), 0.5,  new Lambertian(new Vec3(0.98, 0.00, 0.10))));
            world.add(new Sphere(new Vec3(-1.0,  0.0,    -1.0), 0.5,  new Metal     (new Vec3(0.95, 0.93, 0.78))));
            world.add(new Sphere(new Vec3( 1.0,  0.0,    -1.0), 0.5,  new Metal     (new Vec3(0.08, 0.40, 0.16))));

            // Camera

            Camera cam = new Camera();

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
                        int flippedY = imgHeight - y - 1;
                        double u = Convert.ToDouble(x + random.NextDouble()) / (imgWidth - 1);
                        double v = Convert.ToDouble(flippedY + random.NextDouble()) / (imgHeight - 1);

                        Ray r = cam.getRay(u, v);

                        colour = colour.add(RayColour(r, world, maxDepth));
                    }

                    colour = colour.div(samplesPerPixel);
                    // Gamma-2 correction
                    colour = new Vec3(
                        255.0 * Math.Sqrt(colour.x),
                        255.0 * Math.Sqrt(colour.y),
                        255.0 * Math.Sqrt(colour.z));

                    data[2] = Convert.ToByte(colour.x);
                    data[1] = Convert.ToByte(colour.y);
                    data[0] = Convert.ToByte(colour.z);
                }

            img.UnlockBits(imgData);

            //My god this shouldn't have been this bloody hard
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(img, new Rectangle(0, 0, Width, Height), 0, 0, imgWidth, imgHeight, GraphicsUnit.Pixel);
        }
    }
}

// Useful links, delete later
// https://stackoverflow.com/questions/29157/how-do-i-make-a-picturebox-use-nearest-neighbor-resampling
// https://raytracing.github.io/books/RayTracingInOneWeekend.html
// https://github.com/ja72/WinFormRayTrace/tree/d89648c195fb176a7690050d7a79b0786b04558b