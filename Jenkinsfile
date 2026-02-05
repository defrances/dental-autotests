pipeline {
    agent any

    parameters {
        choice(
            name: 'ENVIRONMENT',
            choices: ['Test', 'Development'],
            description: 'Окружение: от него зависят appsettings.{Environment}.json и runsettings'
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
            description: 'Категория тестов (NUnit Category). All = без фильтра'
        )
    }

    options {
        buildDiscarder(logRotator(numToKeepStr: '20'))
        timeout(time: 30, unit: 'MINUTES')
        timestamps()
    }

    environment {
        DOTNET_VERSION = '8.0'
        // ENVIRONMENT и TEST_CATEGORY задаются через parameters
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
                    def pwScript = (isUnix()) ? "${outDir}/playwright.sh" : "${outDir}/playwright.ps1"
                    if (isUnix()) {
                        sh """
                            chmod +x ${pwScript} 2>/dev/null || true
                            ${pwScript} install --with-deps
                        """
                    } else {
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
                    def filterArg = (params.TEST_CATEGORY == null || params.TEST_CATEGORY == 'All')
                        ? ''
                        : "--filter \"Category=${params.TEST_CATEGORY}\""
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
            // Результаты NUnit/JUnit для Jenkins
            junit allowEmptyResults: true, testResults: '**/junit.xml'
            archiveArtifacts artifacts: '**/TestResults.trx,**/test-results/**', allowEmptyArchive: true
            // Опционально: HTML-отчёт Playwright (если включён в тестах)
            archiveArtifacts artifacts: '**/playwright-report/**', allowEmptyArchive: true
        }
    }
}
