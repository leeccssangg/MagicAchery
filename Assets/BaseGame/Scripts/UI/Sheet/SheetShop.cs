using System.Collections.Generic;
using R3;
using TW.Reactive.CustomComponent;
using TW.UGUI.MVPPattern;
using TW.UGUI.Core.Sheets;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
public class SheetShop : Sheet, ISetupAble
{
    public static class Events
    {
        
    }
    public void Setup()
    {
        
    }
    public override UniTask Initialize(Memory<object> args)
    {
        return UniTask.CompletedTask;
    }
    public override UniTask WillEnter(Memory<object> args)
    {       
        return UniTask.CompletedTask;
    }
    public override void DidEnter(Memory<object> args)
    {
        base.DidEnter(args);
    }
    public override UniTask Cleanup(Memory<object> args)
    {
        return UniTask.CompletedTask;
    }
}
