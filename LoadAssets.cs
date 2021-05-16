using MSCLoader.Helper;
using System;
using System.ComponentModel;
using System.IO;
using UnityEngine;

// GNU GPL 3.0
#pragma warning disable CS1591, IDE1006, CS0618
namespace MSCLoader
{
    [Obsolete("Deprecated, use ModAssets instead."), EditorBrowsable(EditorBrowsableState.Never)]
    public static class LoadAssets
    {
        [Obsolete("Deprecated, use the extension method gameObject.MakePickable() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static void MakeGameObjectPickable(GameObject go) => go.MakePickable();

        [Obsolete("Deprecated, use ModAssets.LoadTexture() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static Texture2D LoadTexture(Mod mod, string fileName, bool normalMap = false) =>
            ModAssets.LoadTexture(Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName), normalMap);

        [Obsolete("Deprecated, use ModAssets.LoadBundle() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static AssetBundle LoadBundle(Mod mod, string bundleName) =>
            ModAssets.LoadBundle(mod, bundleName);

        [Obsolete("LoadOBJ is deprecated, please use AssetBundles instead or ModAssets.LoadMeshOBJ()."), EditorBrowsable(EditorBrowsableState.Never)]
        public static GameObject LoadOBJ(Mod mod, string fileName, bool collider = true, bool rigidbody = false)
        {
            Mesh mesh = ModAssets.LoadMeshOBJ(Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName));
            if (mesh != null)
            {
                GameObject obj = new GameObject();
                obj.AddComponent<MeshFilter>().sharedMesh = mesh;
                obj.AddComponent<MeshRenderer>();
                if (rigidbody)
                    obj.AddComponent<Rigidbody>();
                if (collider)
                {
                    if (rigidbody)
                        obj.AddComponent<MeshCollider>().convex = true;
                    else
                        obj.AddComponent<MeshCollider>();
                }
                return obj;
            }
            else
                return null;
        }

        [Obsolete("LoadOBJMesh is deprecated, please use AssetBundles instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public static Mesh LoadOBJMesh(Mod mod, string fileName)
        {
            string path = Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName);
            if (!File.Exists(path)) throw new FileNotFoundException($"<b>LoadOBJ() Error:</b> File not found: {path}\n", path);

            if (Path.GetExtension(path).ToLower() == ".obj")
            {
                Mesh mesh = ModAssets.LoadMeshOBJ(Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName));
                mesh.name = Path.GetFileNameWithoutExtension(path);

                return mesh;
            }
            else throw new NotSupportedException("<b>LoadOBJ() Error:</b> Only (*.obj) files are supported\n");
        }
    }
}