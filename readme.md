# Gone .NET Aspire/Orleans/MAUI GPS Tracker

This repository shows how to build a GPS tracker application using .NET Aspire, Orleans, and MAUI. The application allows users to track their location in real-time and visualize it on a map.

> [!NOTE]
> In order to see the map at index.html, you will need to add your own subscription key from Azure Maps.

Application Parts
* Aspire App Host to configure Orleans and ASP.NET API
* ASP.NET to receive and stream GPS data
* Orleans Grains to manage GPS data and user sessions
* MAUI App to provide real time tracking application