namespace ProducerConsumer;

class ProducerConsumer {

    /// Producer and consumer thread counts, should be 1,1 for the easy case, less race conditions.
    const int PRODUCER_THREAD_COUNT = 1, CONSUMER_THREAD_COUNT = 1;

    /// Simple buffer for producer and consumer: Shared resource.
    private static Queue<string> _queue = new Queue<string>();

    /// Lock for accessing the shared resource.
    private static object _lock = new object();

    /// A few configurable integers so that we can play around with various conditions in concurrency.
    private static int producerWaitDelay, consumerWaitDelay, itemsToProduce, consumerAttemptsToConsume;

    /// Entry point of dotnet program: Use 'dotnet run'
    static void Main(string[] args) {
        /// Configure the various conditions here:
        producerWaitDelay = 10;
        consumerWaitDelay = 100;
        itemsToProduce = 10;
        consumerAttemptsToConsume = 12;

        // Spawn Producer threads:
        for (int i = 0; i < PRODUCER_THREAD_COUNT; i++) {
            Thread thread = new Thread(() => Producer());
            thread.Name = $"Thread-{i + 1}";
            thread.Start();
        }

        // Spawn Consumer threads:
        for (int i = 0; i < CONSUMER_THREAD_COUNT; i++) {
            Thread thread = new Thread(() => Consumer());
            thread.Name = $"Thread-{i + 1}";
            thread.Start();
        }
        lock (_lock) {
            Monitor.PulseAll(_lock);
        }
    }

    /// Method executed by Producer thread.
    static void Producer() {
        for(int i = 0; i < itemsToProduce; i++) {
            // Take a lock before accessing the buffer (Queue).
            lock (_lock) {
                _queue.Enqueue($"{Thread.CurrentThread.Name}::{i + 1}");
                Console.WriteLine($"{Thread.CurrentThread.Name} Produced a value {i + 1} | QueueSize: {_queue.Count()}");

                // Signal/Notify all other threads to move from Wait-Queue to Ready-Queue, as we are releasing the lock.
                Monitor.PulseAll(_lock);
                // Wait the current thread which has put a lock for a specific timeout.
                Monitor.Wait(_lock, producerWaitDelay);
            }
        }
    }

    /// Method executed by Consumer thread.
    static void Consumer() {
        int i = 0;
        while(i++ < consumerAttemptsToConsume) {
            // Take a lock before accessing the buffer (Queue).
            lock (_lock) {
                String value;
                if (_queue.TryDequeue(out value)) {
                    Console.WriteLine($"{Thread.CurrentThread.Name} consumed a value \"{value}\" | QueueSize: {_queue.Count()}");
                }
                else if (_queue.Count == 0) {
                    Console.WriteLine($"{Thread.CurrentThread.Name} could not consume as queue is empty. Trying again in a few moments");
                }
                else {
                    Console.WriteLine($"{Thread.CurrentThread.Name} could not consume as thread unable to dequeue. Trying again in a few moments");
                }

                // Signal/Notify all other threads to move from Wait-Queue to Ready-Queue, as we are releasing the lock.
                Monitor.PulseAll(_lock);
                // Wait the current thread which has put a lock for a specific timeout.
                Monitor.Wait(_lock, consumerWaitDelay);
            }
        }
    }
}
