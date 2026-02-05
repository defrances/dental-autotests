pipeline {
    agent any

    parameters {
        choice(
            name: 'ENVIRONMENT',
            choices: ['Test', 'Development'],
            description: 'Environment: determines appsettings.{Environment}.json and runsettings'
        )
        choice(
            name: 'TEST_CATEGORY',
            choices: [
                'All',
                'Smoke',
                'Dashboard',
                'Case',
                'Connection',
                'Login',
                'User',
                'Patient',
                'TopBar',
                'Help',
                'Notification'
            ],
            description: 'Test category (NUnit Category). All = no filter'
        )
    }

    options {
        buildDiscarder(logRotator(numToKeepStr: '20'))
        timeout(time: 30, unit: 'MINUTES')
        timestamps()
    }

    environment {
        DOTNET_VERSION = '8.0'
        // Without libicu in Jenkins image: run dotnet in invariant mode (see https://aka.ms/dotnet-missing-libicu)
        DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = '1'
        // ENVIRONMENT and TEST_CATEGORY are set via parameters
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                script {
                    if (isUnix()) { sh 'dotnet restore' }
                    else { bat 'dotnet restore' }
                }
            }
        }

        stage('Build') {
            steps {
                script {
                    if (isUnix()) { sh 'dotnet build -c Release --no-restore' }
                    else { bat 'dotnet build -c Release --no-restore' }
                }
            }
        }

        stage('Install Playwright Browsers') {
            steps {
                script {
                    def outDir = "bin/Release/net${env.DOTNET_VERSION}"
                    if (isUnix()) {
                        // Microsoft.Playwright NuGet has only playwright.ps1; on Linux install via dotnet tool
                        sh '''
                            dotnet tool install --global Microsoft.Playwright.CLI 2>/dev/null || true
                            export PATH="$HOME/.dotnet/tools:$PATH"
                            playwright install chromium
                        '''
                    } else {
                        def pwScript = "${outDir}/playwright.ps1"
                        powershell """
                            if (Test-Path '${pwScript}') { & '${pwScript}' install }
                            else { dotnet tool install --global Microsoft.Playwright.CLI 2>\$null; playwright install }
                        """
                    }
                }
            }
        }

        stage('Test') {
            steps {
                script {
                    def envName = params.ENVIRONMENT ?: 'Test'
                    def runsettings = "test.${envName.toLowerCase()}.runsettings"
                    // VSTest uses TestCategory for NUnit [Category(...)]
                    def filterArg = (params.TEST_CATEGORY == null || params.TEST_CATEGORY == 'All')
                        ? ''
                        : "--filter \"TestCategory=${params.TEST_CATEGORY}\""
                    def testCmd = """
                        dotnet test -c Release --no-build --verbosity normal
                            --logger "trx;LogFileName=TestResults.trx"
                            --logger "junit;LogFileName=junit.xml"
                            ${filterArg}
                            -- RunConfiguration.RunSettingsFilePath=${runsettings}
                    """.stripIndent().trim()
                    echo "Environment: ${envName}, RunSettings: ${runsettings}, Category: ${params.TEST_CATEGORY ?: 'All'}"
                    if (isUnix()) {
                        sh "export DOTNET_ENVIRONMENT=${envName} && export DMG_ENV=${envName} && ${testCmd}"
                    } else {
                        bat "set DOTNET_ENVIRONMENT=${envName} && set DMG_ENV=${envName} && ${testCmd}"
                    }
                }
            }
        }
    }

    post {
        always {
            junit allowEmptyResults: true, testResults: '**/junit.xml'
            // Per-test artifacts: video, HAR, screenshots on failure, TRX
            archiveArtifacts artifacts: '**/TestResults.trx,**/test-results/videos/**,**/test-results/har/**,**/test-results/screenshots/**', allowEmptyArchive: true
            archiveArtifacts artifacts: '**/playwright-report/**', allowEmptyArchive: true
        }
    }
}
