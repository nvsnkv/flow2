namespace Flow.Domain.Common;

public static class AwaiterExtensions
{
    public static T Await<T>(this Task<T> task, CancellationToken ct)
    {
        task.Wait(ct);
        if (!task.IsCompletedSuccessfully)
        {
            if (task.IsCanceled)
            {
                throw new TaskCanceledException();
            }

            if (task.IsFaulted)
            {
                throw task.Exception!;
            }

            throw new InvalidOperationException("Task.Wait() exited and task is not completed successfully, but it's neither cancelled nor faulted!");
        }

        return task.Result;
    }
}