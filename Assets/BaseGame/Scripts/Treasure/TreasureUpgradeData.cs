using System;
using System.Collections.Generic;
using UnityEngine;
using TW.Reactive.CustomComponent;
using MemoryPack;

[Serializable]
[MemoryPackable]
public partial class TreasureUpgradeData 
{
    [field: SerializeField] public List<EachTreasureUpgradeData> Data { get; private set; } = new();
}
[Serializable]
[MemoryPackable]
public partial class EachTreasureUpgradeData
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public ReactiveValue<int> Level { get; private set; } = new();
    [field: SerializeField] public ReactiveValue<int> Piece { get; private set; } = new();

    public EachTreasureUpgradeData(int id, int level, int piece)
    {
        Id = id;
        Level = new(level);
        Piece = new(piece);
    }
    public void AddLevel(int level)
    {
        Level.Value += level;
    }
    public void AddPiece(int piece)
    {
        Piece.Value += piece;
    }
    public void RemovePiece(int piece)
    {
        Piece.Value -= piece;
    }
}
