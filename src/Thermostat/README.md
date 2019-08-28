Room Thermostat Sample - Stratis Platform C# Contract
====================================================

Preface
---------
This is a C# port of a sample contract from the [Azure Workbench Applications and Smart Contract Samples](https://github.com/Azure-Samples/blockchain/tree/master/blockchain-workbench/application-and-smart-contract-samples) project.

Overview 
---------

The room thermostat scenario expresses a workflow around thermostat installation and use. In this scenario, a person will install a thermostat and indicate who is the intended user for the thermostat. The assigned user can do things such as set the target temperature and set the mode for the thermostat.

Application Roles 
------------------

| Name       | Description                                                                                         |
|------------|-----------------------------------------------------------------------------------------------------|
| Installer | A person who is responsible for installing the thermostat.                                             |
| User | A person who uses the thermostat.  |


States 
-------

| Name                 | Description                                                                                                 |
|----------------------|-------------------------------------------------------------------------------------------------------------|
| Created | Indicates that a thermostat installation has been requested. |
| InUse | Indicates that the thermostat is in use. |


Workflow Details
----------------

![](diagram.png)

The room thermostat is a simple workflow to demonstrate how to use the enum data type. Once the installer has installed and started the thermostat, the user can take two main actions. As a user, you can set the target temperature to a temperature you specify, or you can set the mode to one of four modes: Off, Cool, Heat, and Auto. 

References
-----------------
[Azure Workbench Thermostat Example](https://github.com/Azure-Samples/blockchain/tree/master/blockchain-workbench/application-and-smart-contract-samples/room-thermostat)