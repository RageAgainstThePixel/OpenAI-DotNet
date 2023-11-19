using System;

namespace OpenAI
{
    public enum Role
    {
        System = 1,
        Assistant,
        User,
        [Obsolete("Use Tool")]
        Function,
        Tool
    }
}