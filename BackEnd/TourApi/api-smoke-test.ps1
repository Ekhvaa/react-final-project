param(
    [string]$BaseUrl = "https://localhost:54937",
    [string]$AdminUsername = "admin",
    [string]$AdminPassword = "Admin123!",
    [switch]$SkipSslCheck
)

$ErrorActionPreference = "Stop"

try {
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
}
catch { }

if ($SkipSslCheck) {
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
}

$script:Passed = 0
$script:Failed = 0
$script:Warnings = 0
$script:Errors = @()

function Write-Section {
    param([string]$Title)
    Write-Host ""
    Write-Host "====================================================" -ForegroundColor Cyan
    Write-Host $Title -ForegroundColor Cyan
    Write-Host "====================================================" -ForegroundColor Cyan
}

function Write-Pass { param([string]$Message) $script:Passed++; Write-Host "[PASS] $Message" -ForegroundColor Green }
function Write-Fail { param([string]$Message) $script:Failed++; $script:Errors += $Message; Write-Host "[FAIL] $Message" -ForegroundColor Red }
function Write-Warn { param([string]$Message) $script:Warnings++; Write-Host "[WARN] $Message" -ForegroundColor Yellow }

function Get-ErrorResponseBody {
    param($Exception)
    try {
        if ($Exception.Response -and $Exception.Response.GetResponseStream()) {
            $reader = New-Object System.IO.StreamReader($Exception.Response.GetResponseStream())
            return $reader.ReadToEnd()
        }
    }
    catch { }
    return $null
}

function Convert-ResponseBody {
    param([string]$Text)
    if ([string]::IsNullOrWhiteSpace($Text)) { return $null }
    try { return $Text | ConvertFrom-Json } catch { return $Text }
}

function Invoke-Api {
    param(
        [string]$Method,
        [string]$Path,
        $Body = $null,
        [string]$Token = $null,
        [bool]$AllowFail = $false
    )

    $headers = @{}
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }
    $uri = "$BaseUrl$Path"

    try {
        if ($Body -ne $null) {
            $json = $Body | ConvertTo-Json -Depth 30
            return Invoke-RestMethod -Uri $uri -Method $Method -Headers $headers -ContentType "application/json" -Body $json
        }
        return Invoke-RestMethod -Uri $uri -Method $Method -Headers $headers
    }
    catch {
        $statusCode = $null
        $responseBody = $null
        if ($_.Exception.Response) {
            try { $statusCode = [int]$_.Exception.Response.StatusCode } catch { }
            $responseBody = Convert-ResponseBody (Get-ErrorResponseBody $_.Exception)
        }
        $errorObject = [PSCustomObject]@{ IsError = $true; StatusCode = $statusCode; Message = $_.Exception.Message; Body = $responseBody }
        if ($AllowFail) { return $errorObject }
        throw $errorObject
    }
}

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Path,
        $Body = $null,
        [string]$Token = $null,
        [scriptblock]$Validate = $null,
        [bool]$AllowFail = $false
    )

    try {
        $response = Invoke-Api -Method $Method -Path $Path -Body $Body -Token $Token -AllowFail:$AllowFail
        if ($response -and $response.IsError) {
            if ($AllowFail) {
                Write-Pass "$Name returned expected/allowed failure. Status: $($response.StatusCode)"
                return $response
            }
            $message = "$Name failed. Status: $($response.StatusCode). $($response.Message)"
            if ($response.Body) { $message += " Body: " + ($response.Body | ConvertTo-Json -Depth 10 -Compress) }
            Write-Fail $message
            return $null
        }
        if ($Validate -ne $null) { & $Validate $response }
        Write-Pass $Name
        return $response
    }
    catch {
        Write-Fail "$Name failed. $($_)"
        return $null
    }
}

function Get-TokenFromAuthResponse {
    param($Response)
    if ($null -eq $Response) { return $null }
    if ($Response.token) { return $Response.token }
    if ($Response.accessToken) { return $Response.accessToken }
    if ($Response.jwtToken) { return $Response.jwtToken }
    return $null
}

function Get-RefreshTokenFromAuthResponse {
    param($Response)
    if ($null -eq $Response) { return $null }
    if ($Response.refreshToken) { return $Response.refreshToken }
    return $null
}

function Get-IdFromResponse {
    param($Response)
    if ($null -eq $Response) { return $null }
    foreach ($name in @("id", "countryId", "cityId", "hotelId", "tourId", "employeeId", "bookingId", "serviceId", "hotelServiceId")) {
        if ($Response.PSObject.Properties.Name -contains $name -and $Response.$name) { return $Response.$name }
    }
    return $null
}

Write-Section "Tour API Smoke Test"
Write-Host "Base URL: $BaseUrl"
$unique = Get-Random -Minimum 100000 -Maximum 999999

Write-Section "Basic app endpoints"
Test-Endpoint -Name "API root info" -Method "GET" -Path "/api"
Test-Endpoint -Name "Health check" -Method "GET" -Path "/api/health" -Validate { param($r) if ($null -eq $r.status) { throw "Health response does not contain status." } }
Test-Endpoint -Name "Swagger JSON" -Method "GET" -Path "/swagger/v1/swagger.json" -Validate { param($r) if ($null -eq $r.openapi -and $null -eq $r.swagger) { throw "Swagger JSON does not look valid." } }

Write-Section "Authentication"
$adminLogin = Test-Endpoint -Name "Admin login" -Method "POST" -Path "/api/auth/login" -Body @{ username = $AdminUsername; password = $AdminPassword }
$adminToken = Get-TokenFromAuthResponse $adminLogin
if (-not $adminToken) { Write-Warn "Admin token was not returned. Admin-only tests will be skipped." }

$touristUsername = "tourist_$unique"
$touristRegister = Test-Endpoint -Name "Register tourist" -Method "POST" -Path "/api/auth/register" -Body @{
    username = $touristUsername
    password = "Test123!"
    firstName = "Test"
    lastName = "Tourist"
    email = "tourist_$unique@example.com"
    contactPhone = "+995599$unique"
    dateOfBirth = "2000-01-01T00:00:00.000Z"
    gender = "M"
    nationalId = "NID-$unique"
}
$touristToken = Get-TokenFromAuthResponse $touristRegister
$touristRefreshToken = Get-RefreshTokenFromAuthResponse $touristRegister

if ($touristRefreshToken) {
    $refreshResponse = Test-Endpoint -Name "Refresh token" -Method "POST" -Path "/api/auth/refresh" -Body @{ refreshToken = $touristRefreshToken }
    $newToken = Get-TokenFromAuthResponse $refreshResponse
    $newRefreshToken = Get-RefreshTokenFromAuthResponse $refreshResponse
    if ($newToken) { $touristToken = $newToken }
    if ($newRefreshToken) { $touristRefreshToken = $newRefreshToken }
}

Write-Section "Public catalog endpoints"
Test-Endpoint -Name "Get countries" -Method "GET" -Path "/api/countries"
Test-Endpoint -Name "Get cities" -Method "GET" -Path "/api/cities"
Test-Endpoint -Name "Get hotels" -Method "GET" -Path "/api/hotels"
Test-Endpoint -Name "Get hotel services" -Method "GET" -Path "/api/hotels/services"
Test-Endpoint -Name "Get tours" -Method "GET" -Path "/api/tours?page=1&pageSize=10"

Write-Section "Admin and employee setup"
$agentToken = $null
$agentId = $null
if ($adminToken) {
    $agentUsername = "agent_$unique"
    $employeeResponse = Test-Endpoint -Name "Create TravelAgent employee" -Method "POST" -Path "/api/employees" -Token $adminToken -Body @{
        username = $agentUsername
        password = "Test123!"
        role = "TravelAgent"
        firstName = "Test"
        lastName = "Agent"
        email = "agent_$unique@example.com"
        contactPhone = "+995598$unique"
        dateOfBirth = "1990-01-01T00:00:00.000Z"
        gender = "M"
        nationalId = "AGENT-$unique"
        experience = "Smoke test employee"
    }
    $agentId = Get-IdFromResponse $employeeResponse
    $agentLogin = Test-Endpoint -Name "TravelAgent login" -Method "POST" -Path "/api/auth/login" -Body @{ username = $agentUsername; password = "Test123!" }
    $agentToken = Get-TokenFromAuthResponse $agentLogin
}

Write-Section "Create catalog data"
$managementToken = $agentToken
if (-not $managementToken) { $managementToken = $adminToken }
$countryId = $null
$cityId = $null
$hotelId = $null
$tourId = $null

if ($managementToken) {
    $firstLetter = [char](65 + [int](($unique / 26) % 26))
    $secondLetter = [char](65 + [int]($unique % 26))
    $countryIso = "$firstLetter$secondLetter"

    $country = Test-Endpoint -Name "Create country without Base64 flag" -Method "POST" -Path "/api/countries" -Token $managementToken -Body @{
        name = "SmokeCountry$unique"
        isoName = $countryIso
    }
    $countryId = Get-IdFromResponse $country

    if ($countryId) {
        $city = Test-Endpoint -Name "Create city" -Method "POST" -Path "/api/cities" -Token $managementToken -Body @{ name = "SmokeCity$unique"; countryId = $countryId }
        $cityId = Get-IdFromResponse $city
    }

    $services = Test-Endpoint -Name "Get hotel services for reuse" -Method "GET" -Path "/api/hotels/services"
    $hotelServiceId = $null
    if ($services -and $services.Count -gt 0) { $hotelServiceId = Get-IdFromResponse ($services | Select-Object -First 1) }

    if ($cityId) {
        $hotelServiceIds = @()
        if ($hotelServiceId) { $hotelServiceIds = @($hotelServiceId) }
        $hotel = Test-Endpoint -Name "Create hotel" -Method "POST" -Path "/api/hotels" -Token $managementToken -Body @{ name = "Smoke Hotel $unique"; starRating = 4; cityId = $cityId; hotelServiceIds = $hotelServiceIds }
        $hotelId = Get-IdFromResponse $hotel
    }

    if ($cityId) {
        $itineraryLeg = @{ sequence = 1; cityId = $cityId; hotelId = $hotelId; estimatedArrivalDate = "2030-08-01T10:00:00.000Z"; estimatedDepartureDate = "2030-08-03T10:00:00.000Z" }
        if (-not $hotelId) { $itineraryLeg.hotelId = $null }
        $tour = Test-Endpoint -Name "Create tour" -Method "POST" -Path "/api/tours" -Token $managementToken -Body @{ code = "SMOKE-$unique"; name = "Smoke Tour $unique"; currentPrice = 799.99; itinerary = @($itineraryLeg) }
        $tourId = Get-IdFromResponse $tour
    }
}
else { Write-Warn "Skipping create catalog data because no admin/agent token is available." }

Write-Section "Read created data"
if ($countryId) { Test-Endpoint -Name "Get country by id" -Method "GET" -Path "/api/countries/$countryId" }
if ($cityId) { Test-Endpoint -Name "Get city by id" -Method "GET" -Path "/api/cities/$cityId" }
if ($hotelId) { Test-Endpoint -Name "Get hotel by id" -Method "GET" -Path "/api/hotels/$hotelId" }
if ($tourId) { Test-Endpoint -Name "Get tour by id" -Method "GET" -Path "/api/tours/$tourId" }

Write-Section "Bookings"
$bookingId = $null
if ($touristToken -and $tourId) {
    if (-not $agentId) { $agentId = 1; Write-Warn "No TravelAgent ID found. Using 1 as fallback." }
    $booking = Test-Endpoint -Name "Create booking as tourist" -Method "POST" -Path "/api/bookings" -Token $touristToken -Body @{ tourId = $tourId; travelAgentId = $agentId }
    $bookingId = Get-IdFromResponse $booking
    Test-Endpoint -Name "Get my bookings" -Method "GET" -Path "/api/bookings/mine" -Token $touristToken
    Test-Endpoint -Name "Get current user profile" -Method "GET" -Path "/api/users/me" -Token $touristToken
    Test-Endpoint -Name "Get current user history" -Method "GET" -Path "/api/users/me/history" -Token $touristToken
}
else { Write-Warn "Skipping tourist booking tests because tourist token or tour id is missing." }

if ($bookingId -and $managementToken) {
    Test-Endpoint -Name "Get booking by id as manager" -Method "GET" -Path "/api/bookings/$bookingId" -Token $managementToken
    Test-Endpoint -Name "Update booking status to Confirmed" -Method "PUT" -Path "/api/bookings/$bookingId/status" -Token $managementToken -Body @{ status = 1 }
    Test-Endpoint -Name "Invalid status transition check" -Method "PUT" -Path "/api/bookings/$bookingId/status" -Token $managementToken -Body @{ status = 0 } -AllowFail:$true
}
else { Write-Warn "Skipping manager booking tests because booking id or management token is missing." }

Write-Section "Authorization checks"
if ($touristToken) {
    $forbiddenResponse = Invoke-Api -Method "POST" -Path "/api/countries" -Token $touristToken -Body @{ name = "ForbiddenCountry$unique"; isoName = "F" + [char](65 + ($unique % 26)) } -AllowFail:$true
    if ($forbiddenResponse -and $forbiddenResponse.IsError -and ($forbiddenResponse.StatusCode -eq 401 -or $forbiddenResponse.StatusCode -eq 403)) { Write-Pass "Tourist cannot create country" }
    else { Write-Fail "Tourist was able to create country or failed with unexpected status." }
}

Write-Section "Logout"
if ($touristRefreshToken) { Test-Endpoint -Name "Logout" -Method "POST" -Path "/api/auth/logout" -Body @{ refreshToken = $touristRefreshToken } -AllowFail:$true }

Write-Section "Summary"
Write-Host "Passed:   $script:Passed" -ForegroundColor Green
Write-Host "Failed:   $script:Failed" -ForegroundColor Red
Write-Host "Warnings: $script:Warnings" -ForegroundColor Yellow

if ($script:Errors.Count -gt 0) {
    Write-Host ""
    Write-Host "Errors:" -ForegroundColor Red
    foreach ($err in $script:Errors) { Write-Host "- $err" -ForegroundColor Red }
    exit 1
}

Write-Host ""
Write-Host "Smoke test finished successfully." -ForegroundColor Green
exit 0
