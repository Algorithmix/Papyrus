Caruso
======

Named after the [David Caruso](http://www.youtube.com/watch?v=GeeyWvo1rNg), in honor of **Horatio Caine**, the worlds most prolific fictional forensic investigator.


![Horatio Cane - David Caruso](http://unjouravec.net/wp-content/uploads/2010/03/horatio460.jpg)

## Components

```
caruso \
      | Caruso
      | CarusoSample
      | CarusoTest
      | Jigsaw
      | JigsawSample
      | JigsawTest
      | Drive (submodule)
      | DriveTest (submodule)
```

**Caruso** : Library for Digital Forensic Analysis and Measurement of Shreds

**CarusoTest** : VS11 Test Suite for Caruso Class Library

**CarusoSample** : Sample caruso applications for experiments/testing

**Jigsaw** : Library for Reconstruction and Arrangement of Images

**Drive** : Library for integrating with Test Material Easily

## Tests

You need VS2012 to run the tests.

**ALWAYS** Run tests before pushing `(CTRL+R,A)`

## Install Requirements

VS2012 works with this, (VS2010 doesn't )

Requires .NET Framework 4.5

```
git clone https://github.com/algorithmix/caruso.git
cd caruso
git submodule init
git submodule update
```

- Requires **EMGU CV** version 2.4.0 (corresponding OpenCV version aswell) download from [here](https://www.dropbox.com/sh/23rpauin14wndva/EoK1nzqCiZ)
- Copy the `.DLL` files from the **"managed"** folder to the same folder as the visual studio solution file `Caruso.sln`
- Copy the `.DLL` files from the **"unmanaged"** folder to the run directory of your program.  Alternatively, add the location of your unmanaged DLL files to your path as an environment variable.
- Copy the `tessdata` folder to 'carsuo/Caruso/tessdata'
- Download the [TESTDRIVE](https://github.com/algorithmix/testdrive) folder from [here](https://www.dropbox.com/sh/bq2j6vjaklu1i9b/yn9Xl_3aUv)
- Go to 'Add Environment Variable' in Windows and add an ENVIRONMENT variable in windows called `TESTDRIVE_ROOT` and set it equal to the TESTDRIVE folders path
- Open the solution in Visual Studio 2012
- Right click the Solution > Automatically Restore Packages with Nuget
- Now In VS 2012 Set ` Test > Test Settings> Default Processor Arch >  x64`
- Rebuild All 
- Run All Tests and you should be ready to build

You should be good to go!

### Binary Tree Visualizer

To use the Jigsaw Visualizer in chrome you will want to take a look at [this](http://stackoverflow.com/questions/12003107/resource-interpreted-as-script-but-transferred-with-mime-type-text-plain-for-l) because VS messes something with the Content/Type up. 
