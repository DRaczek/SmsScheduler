# SMS Scheduler (Android, .NET MAUI)

A quick and simple Android app built with .NET MAUI that lets you schedule text messages for later delivery.  
I created it because I was missing this functionality in the default system apps.  

## Features
- Schedule SMS messages with a delay  
- Runs in the background using a ForegroundService  
- Persists data locally in an SQLite database  

## Technical Notes
- Timer invokes the scheduler every 1 minute  
- Database implementation is minimal and may not be fully optimized  
- Currently tested only on Samsung Galaxy M35  

## Status
This is an experimental project, done just for my personal use — expect some bugs.
