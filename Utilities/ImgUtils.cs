namespace Common
{
    using System;
    using System.IO;
    using UnityEngine;


    internal class ImgUtils
    {
        /// <summary>
        /// Creates a new <see cref="Texture2D" /> from an image file.
        /// </summary>
        /// <param name="filePathToImage">The path to the image file.</param>
        /// <param name="format">
        /// <para>The texture format. By default, this uses <see cref="TextureFormat.BC7" />.</para>
        /// <para>https://docs.unity3d.com/ScriptReference/TextureFormat.BC7.html</para>
        /// <para>Don't change this unless you really know what you're doing.</para>
        /// </param>
        /// <returns>Will return a new <see cref="Texture2D"/> instance if the file exists; Otherwise returns null.</returns>
        /// <remarks>
        /// Ripped from: https://github.com/RandyKnapp/SubnauticaModSystem/blob/master/SubnauticaModSystem/Common/Utility/ImageUtils.cs
        /// </remarks>
        public static Texture2D LoadTextureFromFile(string filePathToImage, TextureFormat format = TextureFormat.BC7)
        {
            if (File.Exists(filePathToImage))
            {
                byte[] imageBytes = File.ReadAllBytes(filePathToImage);
                Texture2D texture2D = new Texture2D(2, 2, format, false);
                if (texture2D.LoadImage(imageBytes))
                {
                    return texture2D;
                }
                else
                {
                    Console.WriteLine("ERROR on LoadTextureFromFile call. Texture cannot be loaded: " + filePathToImage);
                }
            }
            else
            {
                Console.WriteLine("ERROR on LoadTextureFromFile call. File not found at " + filePathToImage);
            }

            return null;
        }
    }
}
