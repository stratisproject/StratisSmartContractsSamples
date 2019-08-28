Defective Component Counter Smart Contract
====================================================

Overview 
---------
The Defective Component Counter application is an example of using a fixed length array of integers in a contract.  The application lets a manufacturer get the total number of defective components over twelve months. The main objective of this simple application is to show how byte arrays can be declared in a contract's constructor and interpreted as a typed array.

Application Roles 
------------------
| Name       | Description                                                                                         |
|------------|-----------------------------------------------------------------------------------------------------|
| Manufacturer| A person manufacturing the components.                                             |


States 
-------
| Name                 | Description                                                                                                 |
|----------------------|-------------------------------------------------------------------------------------------------------------|
| Create | The state that is reached when a contract is created.                                                    |
| Compute Total | The state that is reached when the total number of defective component is computed.                                                                       |


Workflow Details
----------------

![](diagram.png)

An instance of the Defective Component Counter application's workflow starts when a Manufacturer creates a contract by specifying the number of defective components for the last twelve months.  The manufacturer calls the function 'ComputeTotal' to compute the total number of defective components after the contract is created.  The total number of defective components is tracked as a property in the contract and is updated when the ComputeTotal function executes. 


Application Files
-----------------
[DefectiveComponentCounter.cs](./DefectiveComponentCounterContract/DefectiveComponentCounter.cs)

References
-----------------
[Azure Workbench Defective Component Counter Example](https://github.com/Azure-Samples/blockchain/tree/master/blockchain-workbench/application-and-smart-contract-samples/defective-component-counter)