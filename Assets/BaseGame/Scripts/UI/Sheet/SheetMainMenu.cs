using Cysharp.Threading.Tasks;
using R3;
using TW.UGUI.MVPPattern;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Sheets;
using UnityEngine;
using UnityEngine.UI;
using TW.UGUI.Core.Views;
using System;
using TW.Utility.CustomType;

public class SheetMainMenu : Sheet, ISetupAble
{
    public static class Events
    {
        
    }
    
    public override UniTask Initialize(Memory<object> args)
    {
        
        return UniTask.CompletedTask;
    }

    public void Setup()
    {
        
    }
    public override UniTask Cleanup(Memory<object> args)
    {
        return UniTask.CompletedTask;
    }
}