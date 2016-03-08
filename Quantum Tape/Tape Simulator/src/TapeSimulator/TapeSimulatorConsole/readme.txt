Please run Launcher.exe which simulates multiple ESS clients. 
Each ESS client has 54 channels and send 1440MB files per minute(24MBps*60=1440MB).

Before executing it, please change the configuration file TapeSimulatorConfig.xml accordingly:
1. Change Host, Port, UserName, Password of Extended Storage Server
2. Change ClientBaseGuid and make sure different simulator launcher have different GUID (Important)

By default, it will send the sample demo.avi(size:15.9M) video file 
90 times each minute which simulates the video backup speed 15.9M*90/60=24 MBps. 

User can change the sample file and excution times if needed. 
For Example, if you want to use video file demo2.avi:
1). First check the file size. We assume it is 4 MB.
2). Count send times each minute: 1440/4= 360
3). Change VideoFilePath to demo.avi, and change SendTimesPerMinute to 360.
