# DevOps IoT Edge Module

This project shows how to setup IoT Edge Module DevOps using Azure DevOps.

The idea is to once code is commited a new build starts, runs unit tests and deploy to QA. In QA tests would run inside a test device to validate it.

## IoT Edge Module

The IoT edge module we will be testing is filtering temperature measurements, sending telemetry only if the temperature reaches a threshold.

Based on https://docs.microsoft.com/en-us/azure/iot-edge/how-to-ci-cd
