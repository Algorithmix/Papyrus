# Picasso

Image Manipulation Layer for isolation, manipulation and processing of Shredded pieces.

## Components

```
picasso\
        |Picasso\
        |PicassoTest\
        |TestResult\
	|Visual Studio Solution

```

**Picasso** is a project for a Class Library, i.e. compiles into a DLL.
**PicassoTest** is a MS Test Project for the Picasso Library. Write and run tests here to validate the Picasso.dll
**TestResults** this folder should be ignore by the .gitignore file
**Visual Studio Solution** The solution contains both products

## Where to Add What?

Add to the `Utility` class, or the extend the `System.Drawing.Bitmap` Class.

Please write tests to verify each method, and document everything so that others can understand what you wrote.

## Requirements

VS2012 works with this, (VS2010? Unsure)
Requires .NET Framework 4.5

No external dependencies as of yet.