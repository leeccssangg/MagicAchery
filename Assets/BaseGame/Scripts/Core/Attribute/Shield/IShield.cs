using TW.Utility.CustomType;
using UnityEngine;

namespace Core.AttributeShield
{
    public interface IShield
    {
        BigNumber MaxShield { get; }
        BigNumber CurrentShield { get; }
    }
}