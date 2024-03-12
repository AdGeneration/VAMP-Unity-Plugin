using System;
using System.Collections.Generic;
using System.Linq;

public class MainThreadDispatcher : SingletonMonoBehaviour<MainThreadDispatcher>
{
    // メインスレッドで実行したいアクションのQueue
    private readonly Queue<Action> actionQueue = new Queue<Action>();

    private static readonly object lockObject = new object();

    public void Dispatch(Action action) {
        lock (lockObject)
        {
            actionQueue.Enqueue(action);
        }
    }

    private void Update() {
        while (actionQueue.Any()) {
            lock (lockObject)
            {
                var action = actionQueue.Dequeue();
                if (action != null) {
                    action.Invoke();
                }
            }
        }
    }
}
