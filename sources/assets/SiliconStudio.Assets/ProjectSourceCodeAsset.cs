// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using SiliconStudio.Core;

namespace SiliconStudio.Assets
{
    [DataContract("ProjectSourceCodeAsset")]
    public abstract class ProjectSourceCodeAsset : SourceCodeAsset, IProjectAsset
    {
    }
}
