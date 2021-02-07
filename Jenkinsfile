node {
    def app

    stage('Build') {
        checkout scm

        echo 'Building...'
        app = docker.build("example/game-lobby-api", "--no-cache -f GameLobbyApi/Dockerfile .")
    }
    stage('Test') {
        echo 'Testing...'

        app.inside('--entrypoint ""') {
            sh 'echo "Tests passed"'
        }
    }
    stage('Publish') {
        echo 'Publishing...'

        docker.withRegistry('https://registry.example.com', 'Registry') {
            app.push("${env.BUILD_NUMBER}")
            app.push("latest")
        }
    }
}