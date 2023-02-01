using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eclipse2D.Graphics;

namespace Eclipse2D.Content
{
    public class ContentManager
    {
        /*
        public IResource Load<IResource>(String FileName) where IResource : TestA
        {

        }

        public void Test()
        {
            Load<TestA>("Test.png");
        }
        */
    }

    /// <summary>
    /// Represents the type of resource stored.
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// Represents the resource is a texture resource.
        /// </summary>
        Texture = 0,

        /// <summary>
        /// Represents the resource is a music resource.
        /// </summary>
        Music = 1,

        /// <summary>
        /// Represents the resource is a sound resource.
        /// </summary>
        Sound = 2
    }

    public interface IResource
    {
        Int32 ResourceIndex { get; }

        String ResourceName { get; }

        String ResourceFile { get; }

        ResourceType ResourceType { get; }

        IResource GetResource();
    }

    public interface ITextureResource : IResource
    {
        Int32 Width { get; }

        Int32 Height { get; }

        Int32 UnitWidth { get; }

        Int32 UnitHeight { get; }
    }
}