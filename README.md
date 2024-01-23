# [PGP-Encryption](https://marketplace.uipath.com/listings/pgp-encryption)
[![Build status](https://ci.appveyor.com/api/projects/status/tylwaifrkwt4ip0h/branch/main?svg=true)](https://ci.appveyor.com/project/k2zinger/pgp-encryption/branch/main)



Use the PGPCore library to securely send, share, store, and verify sensitive data; as well as generate pgp keys


## Installation
Studio -> Manage Packages -> Community -> (Search) PGP Encryption -> pick the package(s) to install and click Install -> Save

## Activities
`Decrypt and Verify File`: Decrypt a file using the provided private key and passphrase, then verify the file was signed by the matching public key

`Decrypt File`: Decrypt a file using the matching private key and passphrase

`Encrypt and Sign File`: Encrypt a file using the provided public key and then cryptographically sign the file

`Encrypt File`: Encrypt a file using the provided public key

`Generate Keys`: Generate a new public and private key for the provided Identity and passphrase.  Key Size can also be specified (e.g. 1024, 2048, or 4096)

`Sign File`: Cryptographically signs a file

`Sign Clear File`: Clear signs a file using the provided private key and passphrase so that it is human readable

`Verify Signed Clear File`: Verify a clear signed file was signed by the matching public key

`Verify Signed File`: Verify a file was signed by the matching public key

`Verify Key Validity`: Verify the validity period of a PGP Public Key File