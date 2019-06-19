# Differences between the implementation and the official documentation

## Introduction

The Authorization Registry as provided is not intended to be a 'production-ready' system. The Authorization Registry is mainly for demonstrating that the iSHARE Authorization protocol functions properly and for pilots or proof of concepts, and by providing this open source code market parties can make a swift start in providing authorization functions themselves.

As a result of this intended use of this Authorization Registry, the source code is currently not aimed at complying with all different data specifications of iSHARE. Meaning, it does not pass all different 'unhappy flow' test cases that are encompassed by the iSHARE Conformance Test Tool. The iSHARE Maintenance organisation aims to correct these unexpected responses during 2019H1 and will publish a new version of the source code on this Github.

**Note:** the unexpected test results are NOT relating to accidentally providing incorrect delegation evidence, so the core functionality of the Authorization Registry is functioning as expected.

For the Authorization Registry API functions /token, /capabilities and /delegation the following test cases are not responding as should be expected:

## Token endpoint:

#### ASSERTION INVALID IAT Client assertion JWT payload 'iat' field is after current time
status code was: 200, expected: 400

#### HTTP WRONG METHOD HTTP method is not POST
status code was: 404, expected: 405


## Delegation endpoint

#### HTTP WRONG METHOD HTTP method is not POST
status code was: 404, expected: 405

#### ASSERTION INVALID IAT Previous steps Client assertion JWT payload 'iat' field is after current time
status code was: 200, expected: 400

#### AUTHORIZATION NOT BEARER Authorization' header is not 'Bearer' + 'value'
status code was: 401, expected: 400

#### MISSING AUTHORIZATION HEADER Request contains no 'Authorization' header
status code was: 401, expected: 400


## Capabilities endpoint

#### HTTP WRONG METHOD HTTP method is not GET (with access token)
status code was: 404, expected: 405

#### HTTP WRONG METHOD HTTP method is not GET (without access token)
status code was: 404, expected: 405
