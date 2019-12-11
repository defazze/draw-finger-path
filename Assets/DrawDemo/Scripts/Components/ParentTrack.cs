using System;
using Unity.Entities;

public struct ParentTrack : ISharedComponentData, IEquatable<ParentTrack>
{
    public Entity track;

    public bool Equals(ParentTrack other)
    {
        return track == other.track;
    }

    public override int GetHashCode()
    {
        return track.GetHashCode();
    }
}