Papyrus
=======

Papyrus is the repo for the autonomous algorithmic deshredder.

![algorithmix logo](http://i.imgur.com/Ll77jMr.png)

## Components

```
Papyrus
|-- Algorithmix
|    |-- Preprocessing 
|    |-- Reconstructor
|    |-- Forensics
|    `-- tessdata
|
|-- AlgorithmixSample
|
|-- AlgorithmixTest
|    |-- Preprocessing 
|    |-- Reconstructor
|    |-- Forensics
|    `-- visualizer
|
|-- Drive
`-- DriveTest
```

**Algorithmix** : Class library for rearranging shreds

**AlgorithmixTest** : VS 2012 Test Suite for Algorithmix Class Library

**AlgorithmixSample** : Sample Applications

**Drive** and **DriveTest** : Library for integrating with Test Material Easily (and its tests)

## Setup

### Requirements

x64 bit machine
Windows (Development Environment)
VS2012 (Required for Development)
Requires .NET Framework 4.5
~200-1GB disk space for dependencies

### Cloning Repository

```
git clone https://github.com/algorithmix/papyrus.git
cd papyrus
```

## Configure Dependencies

All the necessary Dependencies can be found [here](https://www.dropbox.com/sh/grxao4iblmiwnke/KnPQ05zXMt)

Currently OpenCV 2.4.0 is being used via Emgu 2.4.0 wrapper.

- Requires **EMGU CV** version 2.4.0 (corresponding OpenCV version aswell) download from [here](https://www.dropbox.com/sh/23rpauin14wndva/EoK1nzqCiZ)
- Copy the `.DLL` files from the **"managed"** folder to the same folder as the visual studio solution file `Papyrus.sln`
- Copy the `.DLL` files from the **"unmanaged"** folder to the run directory of your program.  Alternatively, add the location of your unmanaged DLL files to your path as a system environment variable.
- Copy the `tessdata` folder to 'Papyrus/Algorithmix/tessdata'

### Configuring the TestDrive

- Download the [TESTDRIVE](https://github.com/algorithmix/testdrive) folder from [here](https://www.dropbox.com/sh/bq2j6vjaklu1i9b/yn9Xl_3aUv)
- Go to 'Add Environment Variable' in Windows and add an ENVIRONMENT variable in windows called `TESTDRIVE_ROOT` and set it equal to the TESTDRIVE folders path

### Configuring Visual Studio

- Open the solution in Visual Studio 2012
- Nuget should automagically load dependecnies, if not Right click the Solution > Automatically Restore Packages with Nuget
- Now In VS 2012 Set ` Test > Test Settings> Default Processor Arch >  x64`
- Rebuild All
- Run All Tests and you should be ready to build

You should be good to go!

## Shred Cluster Visualizer

The visualizer can be helpful in debug, it shows you how the shreds are clustered

![visure](https://f.cloud.github.com/assets/839972/147925/6251c200-74e0-11e2-9c91-a1706e0ea438.PNG)

To use the Jigsaw Visualizer in chrome you will want to take a look at [this](http://stackoverflow.com/questions/12003107/resource-interpreted-as-script-but-transferred-with-mime-type-text-plain-for-l) because VS messes something with the Content/Type up. 


## Unit Tests

You need VS2012 to run the tests.

**ALWAYS** Run tests before pushing `(CTRL+R,A)`

