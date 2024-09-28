using System;

namespace UnityExtension.Api
{
    [Flags]
    public enum HttpAuthenticationScheme
    {
        None = 0,               // 0
        Basic = 1 << 0,         // 1
        Bearer = 1 << 1,        // 2
        Digest = 1 << 2,        // 4
        Dpop = 1 << 3,          // 8
        Gnap = 1 << 4,          // 16
        Hoba = 1 << 5,          // 32
        Negotiate = 1 << 6,     // 64
        PrivateToken = 1 << 7,  // 128
        SScramSha1 = 1 << 8,    // 256
        ScramSha256 = 1 << 9,   // 512
        Vapid = 1 << 10,        // 1024
    }
}