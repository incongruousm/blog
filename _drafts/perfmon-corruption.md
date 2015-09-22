# Perfmon Counter Corruption
- Jason Deabill
- incongruousm
- 2014-10-20T13:38:00+00:00
- Tech
- draft

"Unable to add these counters:
\Memory\Available MBytes
\Memory\% Committed Bytes In Use
\Memory\Cache Faults/sec
\Memory\Cache Faults/sec
\PhysicalDisk(*)\%Idle Time
\PhysicalDisk(*)\Avg. Disk Queue Length
\Network Interface(*)\Bytes Total/sec"


C:\Windows\system32>lodctr /r

Error: Unable to rebuild performance counter setting from system backup store, error code is 2

C:\Windows\system32>cd..
C:\Windows>cd SysWOW64
C:\Windows\SysWOW64>lodctr /r

Info: Successfully rebuilt performance counter setting from system backup store

C:\Windows\SysWOW64>winmgmt /RESYNCPERF
