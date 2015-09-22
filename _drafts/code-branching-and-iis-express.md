# Code Branching and IIS Express
- Jason Deabill
- incongruousm
- 2014-07-22
- Tech
- draft

While testing some code this morning I was having some issues with an exception that was claiming something the code said was impossible. Given this I had to assume that the code in question was not being executed against the database I expected. A quick look at SQL Profiler confirmed this to be the case, however the configuration appeared to be correct.

The executing code was being run in IIS Express, started from Visual Studio, and was a recent branch of our main codebase. The only explanation left to me was the ISS Express was running the wrong binaries and the wrong config. It was indeed so.

IIS Express stores configuration files in:

> %USERPROFILE%\Documents\IISExpress\config

A quick look here (applicationhost.config) showed that I was indeed running binaries from the wrong branch. It seems that there isn't a check at startup that ensures that IIS Express is configured to resolve a given port to the location of the binaries as VS understands it to be.

The simple solution is to ensure that, when you branch, you update the port number for IIS Express to a unique value.