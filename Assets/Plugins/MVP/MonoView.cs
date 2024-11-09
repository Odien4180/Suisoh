using System;
using UnityEngine;

public class MonoView : MonoBehaviour, IDisposable
{
    private DisposablePoco _presenter;

    private bool _isDisposed = false;

    public virtual T2 Binding<T1, T2>() where T1 : MonoView where T2 : Presenter<T1>, new()
    {
        var presenter = new T2();
        presenter.View = this as T1;

        _presenter?.Dispose();
        _presenter = presenter;

        return presenter;
    }

    public void ClearPresenter()
    {
        _presenter?.Dispose();
        _presenter = null;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        ClearPresenter();
    }
}
