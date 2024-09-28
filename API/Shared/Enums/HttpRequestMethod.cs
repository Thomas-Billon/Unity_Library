using System;

namespace UnityExtension.Api
{
    [Flags]
    public enum HttpRequestMethod
    {
        None = 0,           // 0
        Get = 1 << 0,       // 1
        Post = 1 << 1,      // 2
        Put = 1 << 2,       // 4
        Delete = 1 << 3,    // 8
        Head = 1 << 4,      // 16
        Patch = 1 << 5,     // 32
        Options = 1 << 6,   // 64
    }
}