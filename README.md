# SpyderClientLibrary

![Build Status]
(https://ci.appveyor.com/api/projects/status/5xhce5xxfpphcsv8/branch/master?svg=true)

Provides a modern, fully async compatible .Net Portable Class Library for creating applications and utilities targeting the 
[Spyder video processor](http://www.vistasystems.net/video-processors-and-matrix-switchers/video-processors/pages/default.aspx) 
from Desktop (.Net 4.5 or later), Windows RT Store applications (Windows 8.1 or later), and Windows Phone 8.1.

Key Features
-------------
* Full implementation of the published Spyder UDP control protocol
* System Config File parsing - provides full Spyder data object model to clients
* Drawing Data (live view) data stream deserialization (Supports 2.10.8 - 4.0.4)
* Spyder quick file transfer procotol (QFT) implementation. List, get, and set files
* Thumbnail Manager - provides background downloading, multi-resolution scaling, and memory lifetime management of system thumbnail images

Usage
-----

This library was built using Visual Studio 2013, and can either be downloaded and compiled from source or downloaded as a
[Nuget Package](https://www.nuget.org/packages/SpyderClientLibrary/) if you prefer - search for SpyderClientLibrary from 
the Nuget package manager to get started.

If you are running Windows 8.1 or later, take a look at the [Spyder Client for Windows 8.1](http://apps.microsoft.com/webpdp/app/6ae527ae-9f38-4f42-aea8-369c1e45ced9)
application in the Windows Store for an example of some of the things possible. 

Future Work
----------------------
In future efforts I"m planning to pull some of the UI out of the Spyder Client application for Windows 8.1 into Windows 
'Controls' libraries (and associated Nuget package), to make it easy to add user interface components like the display simulator 
and register-list panels of buttons for the system's various data types.


If you run into issues
----------------------
I've taken great effort to make this library as solid as possible, but if you run into any problems or want to see something added, 
please feel free to log them in the issue tracker or fix them up and send me a pull request.  

