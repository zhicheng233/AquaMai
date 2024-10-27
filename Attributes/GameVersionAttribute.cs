using System;

namespace AquaMai.Attributes;

public class GameVersionAttribute(uint minVersion = 0, uint maxVersion = 0) : Attribute
{
    public uint MinVersion { get; } = minVersion;
    public uint MaxVersion { get; } = maxVersion;
}
