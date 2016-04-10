The original source code has been ported to .NET with deep assistance by AltSoftLab in 2015-2016
The solution source code based on and requires AltSDK (visit http://www.AltSoftLab.com/ for more info),
and is provided "as is" without express or implied warranty of any kind.

The solution can still require several optimizations: some OpenGL display lists has been removed and
render logic changed to be more transparent and be possible to port to other render engines (maybe
MonoGame or Unity). Also vector arrays can be used for positions, texture coords & colors. Audio is
not implemented directly, but all sound calls directed to Audio class. Game menu ported partly.

Thanks so much to AltSoftLab for help!

AltSoftLab on Facebook      - http://www.facebook.com/AltSoftLab
AltSoftLab on Twitter       - http://www.twitter.com/AltSoftLab
AltSoftLab on Instagram     - http://www.instagram.com/AltSoftLab
AltSoftLab on Unity forums  - http://forum.unity3d.com/threads/335966
AltSoftLab website          - http://www.AltSoftLab.com


HOWTO
-----

- Unpack Data.zip to solution Bin folder. The final path must be like:

SolutionMainFolder\
|
|--Bin\
|  |
|  |--Data\
|     |
|     |--back\
|     |
|     |--ball\
|     |
|     |--...
|
|--Neverbal.NET\ (project main folder)
|  |
|  |--... (project folders & files)
|  |
|  |--Neverbal.NET.csproj
|
|--LICENSE.txt
|
|--Neverbal.NET.sln
|
|--README.txt


- Refresh link to OpenTK

- Get AltSDK package and refresh link to Alt.Sketch. If AltSDK version < 1.XX remove project define ALTSDK_1_0
