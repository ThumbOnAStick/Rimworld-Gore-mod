using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace GoreUponDismemberment.ApperalTearingSystem
{
    // Wrapper graphic that returns per-instance torn materials.
    public class Graphic_SplitWrapper : Graphic
    {
        private readonly Graphic inner;
        private readonly Thing targetThing;
        private readonly Dictionary<Rot4, Material> mats = new Dictionary<Rot4, Material>();
        private bool rendered;
        private int seed;

        public Graphic_SplitWrapper(Graphic inner, Thing targetThing)
        {
            this.inner = inner;
            this.targetThing = targetThing;
            this.seed = targetThing.thingIDNumber;
            this.data = inner.data;
            this.color = inner.color;
            this.colorTwo = inner.colorTwo;
            this.drawSize = inner.drawSize;
        }

        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
 
            if (!mats.TryGetValue(rot, out var mat) || !rendered)
            {
                mats[rot] = mat = BuildTornMaterial(inner.MatAt(rot, thing));
                SetRendered();
            }
            return mat;
        }

        private void SetRendered()
        {
            rendered = true;
        }

        private Material BuildTornMaterial(Material baseMat)
        {
            var shader = baseMat.shader;  
            var newMat = new Material(shader);

            // Copy colors
            if (baseMat.HasProperty(ShaderPropertyIDs.Color)) newMat.color = baseMat.color;
            CopyIfPresent(baseMat, newMat, "_ColorTwo");
            CopyIfPresent(baseMat, newMat, "_ColorThree");

            // Copy maskTex as-is
            var mask = baseMat.GetTexture("_MaskTex");
            if (mask != null) newMat.SetTexture("_MaskTex", mask);

            // Clone and stamp mainTex
            var main = baseMat.mainTexture as Texture2D;
            var stamped = CloneAndStampAlpha(main, seed);
            newMat.mainTexture = stamped;

            // Match render queue and keywords
            newMat.renderQueue = baseMat.renderQueue;
            foreach (var kw in baseMat.shaderKeywords) newMat.EnableKeyword(kw);

            return newMat;
        }

        private static void CopyIfPresent(Material src, Material dst, string prop)
        {
            if (src.HasProperty(prop)) dst.SetColor(prop, src.GetColor(prop));
        }

     

        private static Texture2D CloneAndStampAlpha(Texture2D src, int seed)
        {
            if (src == null) return null;

            var w = src.width; var h = src.height;
            
            // Create a readable copy using RenderTexture
            RenderTexture tmp = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(src, tmp);
            
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            
            Texture2D readableTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            readableTex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            readableTex.Apply();
            
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            // Now get pixels from the readable texture
            var pixels = readableTex.GetPixels32();

            // Compute hole density from damage (more damage => more/larger holes)
            UnityEngine.Random.State old = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed * 73856093 * 19349663);

            // Split into 2
            var holeMask = new byte[w * h]; // 0 keep, 1 hole
            int cy = UnityEngine.Random.Range(h / 3, h / 4);
            SplitInTwo(holeMask, w, h, cy);

            // Mark all pixels transparent
            for (int i = 0; i < pixels.Length; i++)
            {
                var p = pixels[i];
                byte a = p.a;
                if (holeMask[i] == 1) a = 0;
                pixels[i] = new Color32(p.r, p.g, p.b, a);
            }

            readableTex.SetPixels32(pixels);
            readableTex.Apply(false, false);
            UnityEngine.Random.state = old;
            return readableTex;
        }

        void StampHoles(byte[] holeMask, int holes, int w, int h)
        {
            for (int i = 0; i < holes; i++)
            {
                int cx = UnityEngine.Random.Range(w / 6, w - w / 6);
                int cy = UnityEngine.Random.Range(h / 3, h / 4);
                float rx = UnityEngine.Random.Range(w * 0.04f, w * 0.12f);
                float ry = rx * UnityEngine.Random.Range(0.7f, 1.3f);
                float rot = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

                StampHole(holeMask, w, h, cx, cy, rx, ry, rot);
            }
        }

        private static void StampHole(byte[] mask, int w, int h, int cx, int cy, float rx, float ry, float rot)
        {
            float cos = Mathf.Cos(rot), sin = Mathf.Sin(rot);
            int minx = Mathf.Max(0, Mathf.FloorToInt(cx - rx - 1));
            int maxx = Mathf.Min(w - 1, Mathf.CeilToInt(cx + rx + 1));
            int miny = Mathf.Max(0, Mathf.FloorToInt(cy - ry - 1));
            int maxy = Mathf.Min(h - 1, Mathf.CeilToInt(cy + ry + 1));

            for (int y = miny; y <= maxy; y++)
                for (int x = minx; x <= maxx; x++)
                {
                    float dx = x - cx, dy = y - cy;
                    // Rotate point into ellipse frame
                    float ex = (dx * cos + dy * sin) / rx;
                    float ey = (-dx * sin + dy * cos) / ry;
                    if (ex * ex + ey * ey <= 1f) mask[y * w + x] = 1;
                }
        }

        private static void SplitInTwo(byte[] mask, int w, int h, int hight)
        {
       

            for (int y = 0; y <= h; y++)
                for (int x = 0; x <= w; x++)
                {
                    if (y < hight) mask[y * w + x] = 1;
                }
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            inner.DrawWorker(loc, rot, thingDef, thing, extraRotation);
        }

        public override Material MatSingle => inner.MatSingle;
        public override string ToString() => $"Graphic_TornWrapper({inner})";
    }

}
