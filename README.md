# vroom-stats

This projet aims to retrieve car engine stats and store them in a webserver to allow any user to know what happens in their car.

## Architecture

The project is divided in multiple parts: 

![Project Diagram](./docs/diagram.svg)

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

#### API Endpoints

Get the latest known values for a specific car:
- GET /api/v1/cars/{carId} 

Register a new car by its unique id:
- POST /api/v1/cars/{carId}

Append new values for a specific car:
- POST /api/v1/cars/{carId}/data

Get all the known registered cars:
- GET /api/v1/cars

WebSocket connection to a specific car:
- ws://webhost:webport/api/v1/ws/{carId}

The different values gathered are stored in a MongoDB server.

### LTE Module

To send the information from the Raspberry to the Webserver, we will use a WIFI to easily connect to the internet.

## Requirements

### Softwares (With Docker)

- Docker & Docker Compose

### Softwares (Without Docker)

- .NET 6 SDK
- NodeJs
- ReactJs

### Hardware

- Raspberry Pi 4 (with Wifi and Bluetooth)
- Super Mini ELM327

## Resources 

- [OBD.NET](https://github.com/DarthAffe/OBD.NET)
- [OBD II PIDs](https://en.wikipedia.org/wiki/OBD-II_PIDs)

## Run

> docker-compose up -d

Then open your browser at the following address: https://localhost