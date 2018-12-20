# Differences between the implementation and the official documentation

## Token endpoint:

#### ASSERTION MISSING TYP Client assertion JWT header 'typ' field missing
status code was: 200, expected: 400

#### ASSERTION INVALID TYP Client assertion JWT header 'typ' field is other value than "jwt"
status code was: 200, expected: 400

#### ASSERTION MISSING JTI Client assertion JWT payload 'jti' field missing
status code was: 200, expected: 400

#### ASSERTION INVALID IAT Client assertion JWT payload 'iat' field is after current time
status code was: 200, expected: 400

#### HTTP WRONG METHOD HTTP method is not POST
status code was: 404, expected: 405


## Delegation endpoint

#### HTTP WRONG METHOD HTTP method is not POST
status code was: 404, expected: 405

#### MISSING CLIENT ASSERTION Request has no client assertion
status code was: 200, expected: 400

#### ASSERTION MISSING ALG Previous steps Client assertion JWT header 'alg' field missing
status code was: 200, expected: 400

#### ASSERTION INVALID ALG Previous steps Client assertion JWT header 'alg' field is different algorithm than RS or a lower value than RS256 (i.e. 128, 64 etc)
status code was: 200, expected: 400

#### ASSERTION MISSING TYP Previous steps Client assertion JWT header 'typ' field missing
status code was: 200, expected: 400

#### ASSERTION INVALID TYP Previous steps Client assertion JWT header 'typ' field is other value than "jwt"
status code was: 200, expected: 400

#### ASSERTION MISSING X5C Previous steps Client assertion JWT header contains no 'x5c' array
status code was: 401, expected: 400

#### ASSERTION INVALID X5C Previous steps Client assertion JWT header 'x5c' contains other value than valid x509 certificate
status code was: 401, expected: 400

#### ASSERTION MISSING ISS Previous steps Client assertion JWT payload 'iss' field missing
status code was: 401, expected: 400

#### ASSERTION INVALID ISS Previous steps Client assertion JWT payload 'iss' field is different value from Client_id request parameter
status code was: 401, expected: 400

#### ASSERTION MISSING SUB Previous steps Client assertion JWT payload 'sub' field missing
status code was: 200, expected: 400

#### ASSERTION INVALID SUB Previous steps Client assertion JWT payload 'sub' field is different value than 'iss' field
status code was: 200, expected: 400

#### ASSERTION MISSING AUD Previous steps Client assertion JWT payload 'aud' field missing
status code was: 200, expected: 400

#### ASSERTION INVALID AUD Previous steps Client assertion JWT payload 'aud' field is different value than the server's iSHARE client id
status code was: 200, expected: 400

#### ASSERTION MISSING JTI Previous steps Client assertion JWT payload 'jti' field missing
status code was: 200, expected: 400

#### ASSERTION MISSING EXP Previous steps Client assertion JWT payload 'exp' field missing
status code was: 200, expected: 400

#### ASSERTION INVALID EXP Previous steps Client assertion JWT payload 'exp' field is different value than 'iat' field + 30 seconds
status code was: 401, expected: 400

#### ASSERTION MISSING IAT Previous steps Client assertion JWT payload 'iat' field missing
status code was: 401, expected: 400

#### ASSERTION INVALID IAT Previous steps Client assertion JWT payload 'iat' field is after current time
status code was: 200, expected: 400

#### ASSERTION MISSING SIGNATURE Previous steps Client assertion JWT signature missing
status code was: 401, expected: 400

#### CLIENT ID NOT ACTIVE Request client assertion client ID value is known at scheme owner but not listed as 'active'
status code was: 401, expected: 400

#### UNKNOWN CLIENT ID Request client assertion client ID value is unknown with scheme owner
status code was: 401, expected: 400

#### INVALID CERTIFICATE Client assertion JWT header 'x5c' contains certificate(s) not issued by iSHARE trusted root
status code was: 401, expected: 400

#### ASSERTION INVALID SIGNATURE Client assertion JWT signature key used does not correspond with public key from the x5c certificate
status code was: 200, expected: 400

#### CLIENT ID NOT ALLOWED Client assertion ID is not the policyIssuer/accessSubject of the requested mask
status code was: 401, expected: 400

#### UNKNOWN MASK ID Delegation mask policyIssuer/accessSubject not registered at the Scheme Owner
status code was: 401, expected: 400

#### MASK ID NOT ACTIVE Delegation mask policyIssuer/accessSubject is known at scheme owner but not listed as 'active'
status code was: 401, expected: 400

## Capabilities endpoint

#### HTTP WRONG METHOD HTTP method is not GET
status code was: 404, expected: 405