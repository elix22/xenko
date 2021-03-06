// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Serialization;
using SiliconStudio.Core.Storage;

namespace SiliconStudio.Assets
{
    [DataContract("AssetId")]
    [DataSerializer(typeof(AssetId.Serializer))]
    public struct AssetId : IComparable<AssetId>, IEquatable<AssetId>
    {
        private readonly Guid guid;

        public static readonly AssetId Empty = new AssetId();

        public AssetId(Guid guid)
        {
            this.guid = guid;
        }

        public AssetId(string guid)
        {
            this.guid = new Guid(guid);
        }

        public static explicit operator AssetId(Guid guid)
        {
            return new AssetId(guid);
        }

        public static explicit operator Guid(AssetId id)
        {
            return id.guid;
        }

        public static AssetId New()
        {
            return new AssetId(Guid.NewGuid());
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(AssetId left, AssetId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(AssetId left, AssetId right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(AssetId other)
        {
            return guid == other.guid;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AssetId && Equals((AssetId)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        /// <inheritdoc/>
        public int CompareTo(AssetId other)
        {
            return guid.CompareTo(other.guid);
        }

        public static bool TryParse(string input, out AssetId result)
        {
            Guid guid;
            var success = Guid.TryParse(input, out guid);
            result = new AssetId(guid);
            return success;
        }

        public static AssetId Parse(string input)
        {
            return new AssetId(Guid.Parse(input));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return guid.ToString();
        }

        internal class Serializer : DataSerializer<AssetId>
        {
            private DataSerializer<Guid> guidSerialier;

            public override void Initialize(SerializerSelector serializerSelector)
            {
                base.Initialize(serializerSelector);

                guidSerialier = serializerSelector.GetSerializer<Guid>();
            }

            public override void Serialize(ref AssetId obj, ArchiveMode mode, SerializationStream stream)
            {
                var guid = obj.guid;
                guidSerialier.Serialize(ref guid, mode, stream);
                if (mode == ArchiveMode.Deserialize)
                    obj = new AssetId(guid);
            }
        }
    }
}
