GitHub fork of Retlang: http://code.google.com/p/retlang/

=== Major Changes Since Retlang 1.0.x

- Added support for MonoTouch and MonoDroid.
- Consistent IFiber.Start() and IFiber.Enqueue() behavior across IFiber implementations based on ExecutionState.
- Renamed classes that support adapting Retlang to outside threading models to ContextFiber, rather than defining them as a GuiFiber.
- Renamed inconsistent Subscriber, Subscribable interfaces and classes to Receiver.
