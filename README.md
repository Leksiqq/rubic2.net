**Attention!** _This article, as well as this announcement, are automatically translated from Russian_.

The **Net.Leksi.LibRubik2** library allows you to calculate the state of a 2x2x2 Rubik's cube under various movements. It also allows you to generate instructions to restore it to its assembled state. Can be useful in applications for training, for visualization, for returning the cube to its solved state.

All classes are contained in the `Net.Leksi.Rubik2` namespace.

* `Calculator` - the main class that provides methods.
* `Color` - set of edge colors.
* `Completeness` - a set of statuses for the current coloring.
* `Face` - a set of faces and projections.
* `Move` - an object that describes one movement performed by a face or projection.
* `Spin` - a set of movements performed by edges and projections.
* `State` - an object that describes the state (coloring) of the cube.

It is also suggested that you familiarize yourself with the demo project:
- `Demo:Rubik2Console` - a console application that uses the capabilities of the library.

(More info...)[https://github.com/Leksiqq/Leksiqq.github.io/wiki]
