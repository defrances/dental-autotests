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
        // Без libicu в образе Jenkins: запуск dotnet в invariant mode (см. https://aka.ms/dotnet-missing-libicu)
        DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = '1'
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
                    if (isUnix()) {
                        // В NuGet пакете Microsoft.Playwright есть только playwright.ps1; на Linux ставим через dotnet tool
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
            junit allowEmptyResults: true, testResults: '**/junit.xml'
            // Артифакты по каждому тесту: видео, HAR, скриншоты при падении, TRX
            archiveArtifacts artifacts: '**/TestResults.trx,**/test-results/videos/**,**/test-results/har/**,**/test-results/screenshots/**', allowEmptyArchive: true
            archiveArtifacts artifacts: '**/playwright-report/**', allowEmptyArchive: true
        }
    }
}
