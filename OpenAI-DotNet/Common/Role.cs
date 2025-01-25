// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace OpenAI
{
    public enum Role
    {
        System = 1,
        Developer = 1,
        Assistant = 2,
        User = 3,
        [Obsolete("Use Tool")]
        Function = 4,
        Tool = 4
    }
}
