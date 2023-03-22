# Bushbuckridge Base Model

This is a MARS base model set in the Bushbuckridge (BBR) municipality of South Africa, showing how agents access and interact with different parts of the environment (layer types).

For more information on the MARS Group, please visit our [website](https://www.mars-group.org). For more information on the MARS Framework, please see the [framework documentation](https://www.mars-group.org/docs/tutorial/intro).

## Usage

Follow these steps to set up the model on your local machine:

1. Clone or download this GitHub repository.
2. Build and run the model in one of the following ways:
   - Open the file BushbuckridgeBase.sln in [JetBrains Rider](https://www.jetbrains.com/rider/) and run the model by clicking the Play button at the top right.
   - Alternatively, in a terminal, navigate to the directory `./Scenarios/BushbuckridgeBaseBox` and enter the commands `dotnet build` and `dotnet run`.
3. Simulation results will be placed in the compilation directory of the model. See [Model Outputs and Results](#model-outputs-and-results) for more details.

## Model Description

The model consists of the following environment components and agent type.

### Environment (Layer Types and Features)

The environment is made up of a set of layer types and features, each representing a part of the BBR municipality.

- `RasterPrecipitationLayer`: A raster-based time series layer that holds precipitation data per month in 2018.
- `RasterTemperatureMaxLayer`: A raster-based time series layer that holds *maximum* temperatures per month in 2010&mdash;2018.
- `RasterTemperatureMinLayer`: A raster-based time series layer that holds *minimum* temperatures per month in 2010&mdash;2018.
- `RasterVegetationLayer`: A raster layer that holds vegetation data for 2018.
- `ResidentLayer`: An abstract layer responsible for initializing and managing `Resident` agents during the simulation.
- `VectorAirportsLayer`: A vector layer that holds `Airport` features.
  - `Airport`: A point feature representing an airport.
- `VectorLandfillLayer`: A vector layer that holds landfill features.
- `VectorPoiLayer`: A vector layer that holds various vector-based points of interest (POIs).
- `VectorWaterLayer`: A vector layer that holds `River` features.
  - `River`: A line feature representing a river.
- `SpatialGraphMediatorLayer`: A vector layer that holds a travel network for pedestrian and vehicle travel.
- `CarParkingLayer`: A vector layer that holds parking spaces for vehicles.

### Agent Type

- `Resident`: This agent type represents a resident of the BBR municipality. It can pursue different activities, which require trips on foot or by car on the BBR traffic network.
  - Three activities are modeled:
    - "airport": The `Resident` makes a one-way trip from its current location to the nearest `Airport`.
    - "river": The `Resident` makes a one-way trip from its current location to a `River`.
    - "picnic": The `Resident` makes a roundtrip from its current location to a random location for a picnic activity.
  - `PicnicState`: An enumeration of states (`AtHome`, `GoingToPlace`, `ArrivedAtPlace`, and `ReturningHome`) that a `Resident` can be in while engaged in a picnic activity.

## Model Configuration

The JSON file config.json enables external configuration of the model's layers and agents. Additional files required for model initialization are located in the directory [resources](./Scenarios/BushbuckridgeBaseBox/resources).

### Layers

In the file config.json, a file with pertinent data can be specified for each layer. For example, the layer `WaterVectorLayer` is initialized with a GeoJSON file bushbuckridge_waterways.geojson. This file holds line features representing rivers. These line features are mapped to `River` instances that are held by `WaterVectorLayer` and accessible to the `Resident` agents.

### Agents

In the file config.json, the number of agent instances of the agent type `Resident` can be specified. Furthermore, some initial attribute values of the agent instances can be specified in a CSV file.

## Model Outputs and Results

Upon executing the model (see [Usage](#usage)), the model is built and run. The following simulation results are stored in the build directory.

- Resident.csv: A CSV file that lists the attributes values of each `Resident` agent at each simulation step (tick).
- Resident_trips.geojson: A GeoJSON file that contains trip data of each `Resident` agent at each tick. The data are formatted such that the trips can be visualized over time with the browser-based data visualization tool [kepler.gl](https://kepler.gl).

Parts of the environment can be visualized in kepler.gl as well. For example, in the following screenshot, the vehicle travel network (yellow), the airport locations (green), and the rivers (purple) are visualized.

![Screenshot of BBR Municipality](https://github.com/MARS-Group-HAW/model-bbr-base/blob/main/docs/bbr_screenshot.png "Screenshot of the BBR Municipality showing the vehicle travel network, airport locations, and rivers")

## Literature

Lenfers, U.A., Ahmady-Moghaddam, N., Glake, D., Ocker, F., Weyl, J., Clemen, T., 2022. Modeling the Future Tree Distribution in a South African Savanna Ecosystem: An Agent-Based Model Approach. Land 11, 619. <https://doi.org/10.3390/land11050619>

Lenfers, U.A., Weyl, J., Clemen, T., 2018. Firewood collection in South Africa: Adaptive behavior in social-ecological models. Land 7, 97. <https://doi.org/10.3390/land7030097>

## Availability Disclaimer

Because of file size limits, we can no longer provide model box deployments for different architectures. Please contact [thomas.clemen@haw-hamburg.de](mailto:thomas.clemen@haw-hamburg.de) to receive a download link.
