Caruso
======

Named after the [David Caruso](http://www.youtube.com/watch?v=GeeyWvo1rNg), in honor of **Horatio Caine**, the worlds most prolific fictional forensic investigator.


![Horatio Cane - David Caruso](http://unjouravec.net/wp-content/uploads/2010/03/horatio460.jpg)

## Componenets

```
caruso \
      | Caruso
      | CarusoSample
      | CarusoTest
      | Visual Studio Solution
```

**Caruso** : Library for Digital Forensic Analysis and Measurement of Shreds

**CarusoTest** : VS11 Test Suite for Caruso Class Library

**CarusoSample** : Sample caruso applications for behavioural testing

**VS Solution**  the solution contains all three projects

## Commit Etiquette
`git pull` before doing any work.  Keep in sync!

*ALWAYS* leave a commit message.

Commit *ONE* change at a time.  Please do not commit several additions.  It makes finding errors hard.

## Where to Add What?

Add to the `Utility` class, or the extend the `System.Drawing.Bitmap` Class.

Please write tests to verify each method, and document everything so that others can understand what you wrote.

**ALWAYS** Run tests before pushing `(CTRL+R,A)`

**KITTENS WILL DIE** when you push with failing tests.

## Requirements

VS2012 works with this, (VS2010? Unsure)
Requires .NET Framework 4.5

Requires **EMGU CV**.  To install the Emgu CV library, download the ["emgu_dependencies.zip"](https://github.com/downloads/Algorithmix/Picasso/emgu_dependencies.zip) file.

Copy the `.DLL` files from the **"managed"** folder to the same folder as the visual studio solution file `Caruso.sln`

Copy the `.DLL` files from the **"unmanaged"** folder to the run directory of your program.  Alternatively, add the location of your unmanaged DLL files to your path as an environment variable.


