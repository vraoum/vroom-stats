# vroom-stats

This projet aims to retrieve car engine stats and store them in a webserver to allow any user to know what happens in their car.

## Architecture

The project is divided in multiple parts: 

### OBD Module

It is a Super Mini ELM327 V2.1 that supports Bluetooth.

### Raspberry Pi 4

The Raspberry will allow us to:
 - Gather the data from the OBD module to send them to the webserver
 - Show some stats that are given by the webserver

It will also use LEDs, or a screen, to show a few things we will eventually chose.

### Web app

The webapp (.NET 6 Blazor) will consist of some webpages that summaries the stats about the different cars that connected to our IOT system. 

To retrieve values from the car, there will be HTTP REST API Endpoints and also serve a Websocket server.

The routes for the web API and the protocol used for the websocket part are still unknown.

The different values gathered will be stored in a MongoDB server.

### LTE Module

To send the information from the Raspberry to the Webserver, we will use an LTE 4G module that will allow us to easily connect to the internet.

## Requirements

### Software

- .NET 6 SDK
- Docker & Docker Compose

### Hardware

- Raspberry Pi 4
- Super Mini ELM327
- SIMCOM SIM7600G-H 4G LTE

## Resources 

- [OBD.NET](https://github.com/DarthAffe/OBD.NET)
- [OBD II PIDs](https://en.wikipedia.org/wiki/OBD-II_PIDs)