# DevOps IoT Edge Module

This project shows how to setup IoT Edge Module DevOps using Azure DevOps.

The idea is to once code is commited a new build starts, runs unit tests and deploy to QA. In QA tests would run inside a test device to validate it.

## Installing VSTS Agent in Raspberry PI
[TODO]
Reference: https://damianbrady.com.au/2018/08/17/running-a-build-release-deployment-agent-on-a-raspberry-pi/

## Installing Client Certificate

All details are here: https://docs.microsoft.com/en-us/azure/iot-edge/how-to-create-transparent-gateway-linux

Reset certificates
```bash
sudo service iotedge stop

sudo rm /var/lib/iotedge/hsm/cert_keys/*
sudo rm /var/lib/iotedge/hsm/certs/*
```

```bash
cd /home/pi/
git clone https://github.com/Azure/azure-iot-sdk-c.git

mkdir /home/pi/iotedgecerts && cd /home/pi/iotedgecerts

cp ../azure-iot-sdk-c/tools/CACertificates/*.cnf . && cp ../azure-iot-sdk-c/tools/CACertificates/certGen.sh .
chmod 700 certGen.sh

./certGen.sh create_root_and_intermediate


# DO NOT use a name that is the same as the gateway's DNS host name. Doing so will cause client certification against these certificates to fail.
./certGen.sh create_edge_device_certificate "edge_1"


cat ./certs/new-edge-device.cert.pem ./certs/azure-iot-test-only.intermediate.cert.pem ./certs/azure-iot-test-only.root.ca.cert.pem > ./certs/new-edge-device-full-chain.cert.pem

sudo nano /etc/iotedge/config.yaml


sudo service iotedge restart
```

```yaml
certificates:
  device_ca_cert:   "/home/pi/iotedgecerts/certs/new-edge-device-full-chain.cert.pem"
  device_ca_pk:     "/home/pi/iotedgecerts/private/new-edge-device.key.pem"
  trusted_ca_certs: "/home/pi/iotedgecerts/certs/azure-iot-test-only.root.ca.cert.pem"
```

On the leaf device:

```bash
sudo cp /home/pi/iotedgecerts/certs/azure-iot-test-only.root.ca.cert.pem  /usr/local/share/ca-certificates/azure-iot-test-only.root.ca.cert.pem.crt
sudo update-ca-certificates
```

Communicating as a device
```javascript
const fs = require('fs');

var connectionString = 'HostName=xxxxx.azure-devices.net;DeviceId=mydevice;SharedAccessKey=xxx;GatewayHostName=pi';

// use factory function from AMQP-specific package
//var clientFromConnectionString = require('azure-iot-device-amqp').clientFromConnectionString;
var clientFromConnectionString = require('azure-iot-device-mqtt').clientFromConnectionString;

// AMQP-specific factory function returns Client object from core package
var client = clientFromConnectionString(connectionString);


const fileName = "/home/pi/iotedgecerts/certs/azure-iot-test-only.root.ca.cert.pem";
//const fileName = "/home/pi/iotedgecert/certs/new-edge-device.cert.pem";

const options = {
        ca: fs.readFileSync(fileName, "utf-8").toString()
};

client.setOptions(options, () => { console.log("Client transport option set"); });


// use Message object from core package
var Message = require('azure-iot-device').Message;

var connectCallback = function (err) {
  if (err) {
    console.error('Could not connect: ' + err);
  } else {
    console.log('Client connected');
    var msg = new Message(JSON.stringify( {  machine: { temperature: 30, pressure: 0 }, ambient: { temperature: 21, humidity: 0 } }));
    client.sendEvent(msg, function (err) {
      if (err) {
        console.log(err.toString());
        process.exit(1);
      } else {
        console.log('Message sent');
        process.exit(0);
      };
    });
  };
};


client.open(connectCallback);
```

## IoT Edge Module

The IoT edge module we will be testing is filtering temperature measurements, sending telemetry only if the temperature reaches a threshold.

Based on https://docs.microsoft.com/en-us/azure/iot-edge/how-to-ci-cd and https://github.com/toolboc/IoTEdge-DevOps.

The deployment.module.json will only build a single image. We want to build arm32v7 too. For that we add a new docker build task: