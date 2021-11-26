# Startis-SignatureWeb
The web application to sign documents with a digital signature. It uses Blockchain for better integrity and transparency. With this application, users can create, sign and verify the documents.

The application is suitable for enterprise use and can be used in the following Industries & Functions.

## Use Cases

#### Sales
* NDA’s between Enterprise and  potential partners.
* Statement of work signed between Client and Service provider for a specific project.

#### Purchase
* Sign Invoice and make quick approvals.

#### Legal
* Intellectual Property Licensing to licensing authorities.
* Confidentiality Agreements between internal departments.

#### Resource Management
* Offer Letter sending and acceptance.
* Onboarding documents acceptance: Basic details form, Emergency contact form, Handbooks, HR background check form, etc.

#### Operations
* Service Level Agreements.
* Information Security Policies.
* Equipment Lease Agreement.

#### Finance
* Internal audit reports that are presented to higher management for approvals.
* Client PO/ Invoices approvals.

## Get Started

The project uses ASP.NET Core 2.2 and MSSQL server. Therefore, install and configure the dependencies before proceeding.

1. Visual Studio 2017 or later with .NET Core SDK 2.2 
2. SQL Server 2014 or later 
3. Cirrus Core Wallet (Hackathon Edition)

### Project Configuration
#### 1. Configure Database.

Open up the DB script from the root (`DBScript.sql`) and run it on your SQL server. It will create a new database `SignatureDB`, tables and required stored procedures.

#### 2. Configure Web Project.

Open up the `appsettings.json` file and update the `ConnectionString` and `ContractAddress`, then run the project.

### Modules

#### Registration & Login

Any user can register with the application by providing basic detail such as name, email, password and a valid wallet. Make sure the wallet has sufficient funds to make transactions. 

#### Dashboard

Dashboard showing counts of documents assigned to the user. 

<img src="https://github.com/stratisproject/StratisSmartContractsSamples/tree/master/src/Solutions/StratisSignature-Web/Images/3.Dashboard.jpg" width="90%"></img>

**Awaiting My Sign:** Counts of documents remaining sign by the logged-in user. <br> 
**Completed:** Counts of documents signed by all the signers.

#### Contact

The application user can add other registered users in his/her contact list. These contacts will be displaying during document creation and can choose as signers.

<img src="https://github.com/stratisproject/StratisSmartContractsSamples/tree/master/src/Solutions/StratisSignature-Web/Images/4.%20Contacts.jpg" width="90%"></img>

#### Document Creation

The logged-in user can create a new document by providing the required information along with a document file(in pdf format only). Currently, the system supports only two signers. 

<img src="https://github.com/stratisproject/StratisSmartContractsSamples/tree/master/src/Solutions/StratisSignature-Web/Images/5.Create%20Document.jpg" width="70%"></img>

This creation method will call the smart contract methods `CreateAgreement` and `AddSigners`.

#### Document Signing

A signer can sign the document from the detail page. This will sign the transaction and execute the `SignAgreement` function of the smart contract and stored signing information to the contract state.

<img src="https://github.com/stratisproject/StratisSmartContractsSamples/tree/master/src/Solutions/StratisSignature-Web/Images/7.Document%20Detail.jpg" width="90%"></img>

#### Verify

This module is to check the authenticity of any document, whether the document is signed using a particular wallet address. The verify method get the stamping detail from smart contract and matches with the requested file.

<img src="https://github.com/stratisproject/StratisSmartContractsSamples/tree/master/src/Solutions/StratisSignature-Web/Images/8.Verify.jpg" width="90%"></img>

#### <ins> **Note :** </ins>

This is POC level code and the purpose of this application is to demonstrate a *happy flow* with smart contract usage. The code may break without proper setup and configurations.
