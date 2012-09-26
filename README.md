# Picasso

Image Manipulation Layer for isolation, manipulation and processing of Shredded pieces.


## Components

```
picasso\
        |Picasso
        |PicassoSample\
        |PicassoTest\
        |TestResult\
	|Visual Studio Solution

```

**Picasso** is a project for a Class Library, i.e. compiles into a DLL.

**PicassoSample** is a solution for showing samples of the Picasso Library and how to use it.

**PicassoTest** is a MS Test Project for the Picasso Library. Write and run tests here to validate the Picasso.dll

**TestResults** this folder should be ignore by the .gitignore file

**Visual Studio Solution** The solution contains both products

## Where to Add What?

Add to the `Utility` class, or the extend the `System.Drawing.Bitmap` Class.

Please write tests to verify each method, and document everything so that others can understand what you wrote.

## Requirements

VS2012 works with this, (VS2010? Unsure)
Requires .NET Framework 4.5

Requires **EMGU CV**.  To install the Emgu CV library, download the ["emgu_dependencies.zip"](https://github.com/downloads/Algorithmix/Picasso/emgu_dependencies.zip) file.

Copy the `.DLL` files from the **"managed"** folder to the same folder as the visual studio solution file `Picasso.sln`

Copy the `.DLL` files from the **"unmanaged"** folder to the run directory of your program.  Alternatively, add the location of your unmanaged DLL files to your path as an environment variable.
