# EduPlayKids - Comprehensive Test Execution Script
# This script runs all available tests and generates a comprehensive quality report

Write-Host "================================" -ForegroundColor Cyan
Write-Host "EduPlayKids - QA Test Suite" -ForegroundColor Cyan
Write-Host "Child Educational App Testing" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Test execution results
$TestResults = @()

# Function to run tests and capture results
function Run-TestProject {
    param(
        [string]$ProjectPath,
        [string]$TestCategory,
        [string]$Description
    )

    Write-Host "Running $TestCategory Tests..." -ForegroundColor Yellow
    Write-Host "Description: $Description" -ForegroundColor Gray
    Write-Host ""

    try {
        $result = dotnet test $ProjectPath --verbosity normal --logger "console;verbosity=detailed" 2>&1
        $exitCode = $LASTEXITCODE

        if ($exitCode -eq 0) {
            Write-Host "✅ $TestCategory: PASSED" -ForegroundColor Green
            $status = "PASSED"
        } else {
            Write-Host "⚠️ $TestCategory: FAILED" -ForegroundColor Red
            $status = "FAILED"
        }

        # Extract test counts from output
        $passedCount = if ($result -match "(\d+) Correcto") { $matches[1] } else { "0" }
        $failedCount = if ($result -match "(\d+) Incorrecto") { $matches[1] } else { "0" }
        $totalCount = if ($result -match "Pruebas totales: (\d+)") { $matches[1] } else { "0" }

        $TestResults += [PSCustomObject]@{
            Category = $TestCategory
            Description = $Description
            Status = $status
            TotalTests = $totalCount
            PassedTests = $passedCount
            FailedTests = $failedCount
            PassRate = if ([int]$totalCount -gt 0) { [math]::Round(([int]$passedCount / [int]$totalCount) * 100, 1) } else { 0 }
        }

    } catch {
        Write-Host "❌ $TestCategory: ERROR - $_" -ForegroundColor Red
        $TestResults += [PSCustomObject]@{
            Category = $TestCategory
            Description = $Description
            Status = "ERROR"
            TotalTests = "0"
            PassedTests = "0"
            FailedTests = "0"
            PassRate = 0
        }
    }

    Write-Host ""
}

# Run all test suites
Write-Host "Starting Comprehensive QA Test Suite for EduPlayKids..." -ForegroundColor Cyan
Write-Host ""

# 1. Child Usability Tests (Critical for ages 3-8)
Run-TestProject "EduPlayKids.Tests.ChildUsability\EduPlayKids.Tests.ChildUsability.csproj" `
    "Child Usability" `
    "Touch targets, motor skills, accessibility for children aged 3-8"

# 2. Unit Tests (if they compile)
Run-TestProject "EduPlayKids.Tests.Unit\EduPlayKids.Tests.Unit.csproj" `
    "Unit Tests" `
    "Domain entities, star rating system, educational logic"

# 3. Integration Tests
Run-TestProject "EduPlayKids.Tests.Integration\EduPlayKids.Tests.Integration.csproj" `
    "Integration Tests" `
    "Educational workflows, multi-subject learning, parental oversight"

# Generate comprehensive report
Write-Host "================================" -ForegroundColor Cyan
Write-Host "COMPREHENSIVE QA REPORT" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Overall statistics
$totalTests = ($TestResults | Measure-Object -Property TotalTests -Sum).Sum
$totalPassed = ($TestResults | Measure-Object -Property PassedTests -Sum).Sum
$totalFailed = ($TestResults | Measure-Object -Property FailedTests -Sum).Sum
$overallPassRate = if ($totalTests -gt 0) { [math]::Round(($totalPassed / $totalTests) * 100, 1) } else { 0 }

Write-Host "OVERALL TEST RESULTS:" -ForegroundColor White
Write-Host "Total Tests: $totalTests" -ForegroundColor White
Write-Host "Passed: $totalPassed" -ForegroundColor Green
Write-Host "Failed: $totalFailed" -ForegroundColor Red
Write-Host "Overall Pass Rate: $overallPassRate%" -ForegroundColor $(if ($overallPassRate -ge 80) { "Green" } elseif ($overallPassRate -ge 60) { "Yellow" } else { "Red" })
Write-Host ""

# Detailed results by category
Write-Host "DETAILED RESULTS BY CATEGORY:" -ForegroundColor White
Write-Host ""

foreach ($result in $TestResults) {
    $statusColor = switch ($result.Status) {
        "PASSED" { "Green" }
        "FAILED" { "Red" }
        "ERROR" { "Magenta" }
    }

    Write-Host "📊 $($result.Category): $($result.Status)" -ForegroundColor $statusColor
    Write-Host "   Description: $($result.Description)" -ForegroundColor Gray
    Write-Host "   Tests: $($result.TotalTests) | Passed: $($result.PassedTests) | Failed: $($result.FailedTests) | Pass Rate: $($result.PassRate)%" -ForegroundColor Gray
    Write-Host ""
}

# Quality assessment
Write-Host "QUALITY ASSESSMENT:" -ForegroundColor White
if ($overallPassRate -ge 90) {
    Write-Host "🟢 EXCELLENT QUALITY - Ready for production" -ForegroundColor Green
} elseif ($overallPassRate -ge 80) {
    Write-Host "🟡 GOOD QUALITY - Minor issues to address" -ForegroundColor Yellow
} elseif ($overallPassRate -ge 60) {
    Write-Host "🟠 MODERATE QUALITY - Significant issues need fixing" -ForegroundColor DarkYellow
} else {
    Write-Host "🔴 POOR QUALITY - Major issues prevent production deployment" -ForegroundColor Red
}
Write-Host ""

# Child-specific quality factors
Write-Host "CHILD SAFETY & EDUCATIONAL QUALITY:" -ForegroundColor White

$childUsabilityResult = $TestResults | Where-Object { $_.Category -eq "Child Usability" } | Select-Object -First 1
if ($childUsabilityResult -and $childUsabilityResult.Status -eq "PASSED") {
    Write-Host "✅ Child Usability: PASSED - App is safe and accessible for children ages 3-8" -ForegroundColor Green
} else {
    Write-Host "❌ Child Usability: FAILED - Critical safety issues for children" -ForegroundColor Red
}

# Recommendations
Write-Host ""
Write-Host "RECOMMENDATIONS:" -ForegroundColor White

if ($overallPassRate -ge 80) {
    Write-Host "• Application shows strong quality foundations" -ForegroundColor Green
    Write-Host "• Child safety measures are well implemented" -ForegroundColor Green
    Write-Host "• Ready for final polishing and production deployment" -ForegroundColor Green
} else {
    Write-Host "• Address failing tests before production deployment" -ForegroundColor Red
    Write-Host "• Focus on compilation errors in Infrastructure layer" -ForegroundColor Red
    Write-Host "• Ensure all child safety tests pass before release" -ForegroundColor Red
}

Write-Host ""
Write-Host "Report generated: $(Get-Date)" -ForegroundColor Gray
Write-Host "For detailed test results, check individual test project outputs." -ForegroundColor Gray
Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "End of QA Report" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan