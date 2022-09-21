using System;
using UnityEngine.UI;
using static ARtChat.LocalMessageGOUpdater;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate ()
        {
            OnClick(param);
        });
    }

    public static void AddEventListener<T,S>(this Button button, T param1, S param2, Action<T,S> OnClick)
    {
        button.onClick.AddListener(delegate ()
        {
            OnClick(param1, param2);
        });
    }

    public static void AddEventListener<T, S, M>(this Button button, T param1, S param2, M param3, Action<T, S, M> OnClick)
    {
        button.onClick.AddListener(delegate ()
        {
            OnClick(param1, param2, param3);
        });
    }
}
