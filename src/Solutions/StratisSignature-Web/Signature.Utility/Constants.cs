namespace Signature.Utility
{
    public static class Constants
    {        
        public const string UserIdType = "StratisSignature:UserId";
        public const string EmailType = "StratisSignature:Email";
        public const string FirstNameType = "StratisSignature:FirstName";
        public const string LastNameType = "StratisSignature:LastName";
        public const string WalletAddressType = "StratisSignature:WalletAdress";

        //Messages
        public const string RequestProcessFailure = "An application error has occurred while processing your request.";

        public const string AccountLoginInvalidCredential = "Incorrect email or password.";
        public const string AccountRegistrationSuccess = "You have successfully registered!";

        public const string ContactUserMustbeRegistered = "Contact must be registered with the system.";
        public const string ContactAddSuccess = "Contact has been added.";
        public const string ContactDeleteSuccess = "Contact has been deleted.";

        public const string InvalidInputs = "Please provide valid inputs.";
        public const string DocumentErrorDuringDbProcessing = "Error occurred during adding document to the database.";
        public const string DocumentErrorDuringBlockchainProcessing = "Error occurred during creating the document into Blockchain.";
        public const string DocumentCreationSuccess = "Document created!";
        public const string DocumentInvalidSigner = "The requested signer is not valid.";
        public const string DocumentWasNotFound = "The document was not found";

        public const string DocumentErrorDuringSigning = "An issue Occurred during file signing, please try again!";
        public const string DocumentSignSuccess = "Document signed successfully!";

        public const string DocumentVerifyFailure = "No record found for the document that you are trying to verify. It has either been tempered or not signed on this platform.";
        public const string DocumentVerifySuccess = "Document is signed by the provided wallet address";
    }
}
