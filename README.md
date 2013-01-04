Hi, this site is all about Retlang, REAL RETLANG. This site is awesome. My name is Anthony and I can't stop thinking about Retlang. This library is cool; and by cool, I mean totally sweet.

GitHub fork of Retlang: http://code.google.com/p/retlang/

### Major Changes Since Retlang 1.0.x

- Added support for MonoTouch and MonoDroid.
- Consistent IFiber.Start() and IFiber.Enqueue() behavior across IFiber implementations based on ExecutionState.
- Renamed classes that support adapting Retlang to outside threading models to ContextFiber, rather than defining them as a GuiFiber.
- Renamed inconsistent Subscriber, Subscribable interfaces and classes to Receiver.
- Added IFiber.Assert()
- Added alternate unit test assembly (Retlang.Test) that uses Moq.
- Added DebounceReceiver.
