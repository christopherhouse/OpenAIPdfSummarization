az deployment group create -g OPEN_AI_DEV -n deploy1 --template-file .\main.bicep --parameters '@.\parameters\dev\parameters.json' --parameters buildId=1